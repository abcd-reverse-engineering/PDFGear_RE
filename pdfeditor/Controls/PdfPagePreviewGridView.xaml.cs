using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using pdfeditor.Models.Thumbnails;

namespace pdfeditor.Controls
{
	// Token: 0x020001C2 RID: 450
	public partial class PdfPagePreviewGridView : PdfPagePreviewListView
	{
		// Token: 0x0600198D RID: 6541 RVA: 0x000652DF File Offset: 0x000634DF
		static PdfPagePreviewGridView()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfPagePreviewGridView), new FrameworkPropertyMetadata(typeof(PdfPagePreviewGridView)));
		}

		// Token: 0x0600198E RID: 6542 RVA: 0x00065304 File Offset: 0x00063504
		public PdfPagePreviewGridView()
		{
			base.Loaded += this.PdfPagePreviewGridView_Loaded;
			base.Unloaded += this.PdfPagePreviewGridView_Unloaded;
			this.dragTimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(100.0)
			};
			this.dragTimer.Tick += this.DragTimer_Tick;
		}

		// Token: 0x0600198F RID: 6543 RVA: 0x00065380 File Offset: 0x00063580
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (this.DragInfo != null)
			{
				this.DragInfo.SizeChanged -= this.DragInfo_SizeChanged;
			}
			this.Bd = base.GetTemplateChild("Bd") as Border;
			this.ScrollViewer = base.GetTemplateChild("ScrollViewer") as ScrollViewer;
			this.DragInfo = base.GetTemplateChild("DragInfo") as ContentPresenter;
			this.InsertPlaceholder = base.GetTemplateChild("InsertPlaceholder") as FrameworkElement;
			if (this.DragInfo != null)
			{
				this.DragInfo.SizeChanged += this.DragInfo_SizeChanged;
			}
			VisualStateManager.GoToState(this, "NotDraging", true);
		}

		// Token: 0x06001990 RID: 6544 RVA: 0x00065438 File Offset: 0x00063638
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.OriginalSource == this.Bd || e.OriginalSource is ScrollViewer)
			{
				this.UnselectAll();
				ScrollViewer scrollViewer = base.GetTemplateChild("ScrollViewer") as ScrollViewer;
				if (scrollViewer != null)
				{
					ItemsPresenter itemsPresenter = scrollViewer.Content as ItemsPresenter;
					if (itemsPresenter != null && VisualTreeHelper.GetChildrenCount(itemsPresenter) > 0)
					{
						Panel panel = VisualTreeHelper.GetChild(itemsPresenter, 0) as Panel;
						if (panel != null)
						{
							foreach (FrameworkElement frameworkElement in panel.Children.OfType<FrameworkElement>())
							{
								VisualStateManager.GoToState(frameworkElement, "FocusBorderInvisible", true);
							}
						}
					}
				}
			}
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x000654F4 File Offset: 0x000636F4
		private void PdfPagePreviewGridView_Loaded(object sender, RoutedEventArgs e)
		{
			if (base.IsVisible)
			{
				PdfPageEditList pdfPageEditList = base.ItemsSource as PdfPageEditList;
				if (pdfPageEditList != null && pdfPageEditList.SelectedItems.Count > 0)
				{
					base.ScrollIntoView(pdfPageEditList.SelectedItems.First<PdfPageEditListModel>());
				}
			}
			else
			{
				base.IsVisibleChanged += this.PdfPagePreviewGridView_IsVisibleChanged;
			}
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				StrongReferenceMessenger.Default.Unregister<ValueChangedMessage<global::System.ValueTuple<int, int>>, string>(this, "MESSAGE_PAGE_EDITOR_SELECT_INDEX");
				StrongReferenceMessenger.Default.Register(this, "MESSAGE_PAGE_EDITOR_SELECT_INDEX", new MessageHandler<object, ValueChangedMessage<global::System.ValueTuple<int, int>>>(this.OnSelectIndexChangeNotified));
			}
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x00065580 File Offset: 0x00063780
		private void DragInfo_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			TranslateTransform translateTransform = ((FrameworkElement)sender).RenderTransform as TranslateTransform;
			if (translateTransform == null)
			{
				translateTransform = new TranslateTransform();
				((FrameworkElement)sender).RenderTransform = translateTransform;
			}
			translateTransform.X = -e.NewSize.Width / 2.0;
			translateTransform.Y = -e.NewSize.Height / 2.0;
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x000655F1 File Offset: 0x000637F1
		private void PdfPagePreviewGridView_Unloaded(object sender, RoutedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				StrongReferenceMessenger.Default.Unregister<ValueChangedMessage<global::System.ValueTuple<int, int>>, string>(this, "MESSAGE_PAGE_EDITOR_SELECT_INDEX");
			}
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x0006560C File Offset: 0x0006380C
		private void OnSelectIndexChangeNotified(object recipient, [global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "startPage", "endPage" })] ValueChangedMessage<global::System.ValueTuple<int, int>> message)
		{
			global::System.ValueTuple<int, int> value = message.Value;
			int startPage = value.Item1;
			int endPage = value.Item2;
			IEnumerable itemsSource = base.ItemsSource;
			IList list = itemsSource as IList;
			if (list != null && startPage >= 0 && startPage < list.Count && endPage >= 0 && endPage < list.Count)
			{
				base.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
				{
					if (this.IsLoaded && this.IsVisible)
					{
						for (int i = 0; i < list.Count; i++)
						{
							((PdfPageEditList)list)[i].Selected = i >= startPage && i <= endPage;
						}
						this.ScrollIntoView(list[startPage]);
					}
				}));
			}
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x000656B1 File Offset: 0x000638B1
		private void PdfPagePreviewGridView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (base.IsVisible)
			{
				base.IsVisibleChanged -= this.PdfPagePreviewGridView_IsVisibleChanged;
				if (base.SelectedItem != null)
				{
					base.ScrollIntoView(base.SelectedItem);
				}
			}
		}

		// Token: 0x06001996 RID: 6550 RVA: 0x000656E4 File Offset: 0x000638E4
		public new void UnselectAll()
		{
			base.UnselectAll();
			PdfPageEditList pdfPageEditList = base.ItemsSource as PdfPageEditList;
			if (pdfPageEditList != null)
			{
				pdfPageEditList.AllItemSelected = new bool?(false);
			}
		}

		// Token: 0x06001997 RID: 6551 RVA: 0x00065714 File Offset: 0x00063914
		public new void SelectAll()
		{
			base.SelectAll();
			PdfPageEditList pdfPageEditList = base.ItemsSource as PdfPageEditList;
			if (pdfPageEditList != null)
			{
				pdfPageEditList.AllItemSelected = new bool?(true);
			}
		}

		// Token: 0x170009BA RID: 2490
		// (get) Token: 0x06001998 RID: 6552 RVA: 0x00065742 File Offset: 0x00063942
		protected override double ViewportThreshold
		{
			get
			{
				return 40.0;
			}
		}

		// Token: 0x06001999 RID: 6553 RVA: 0x0006574D File Offset: 0x0006394D
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new PdfPagePreviewGridViewItem();
		}

		// Token: 0x0600199A RID: 6554 RVA: 0x00065754 File Offset: 0x00063954
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is PdfPagePreviewGridViewItem;
		}

		// Token: 0x0600199B RID: 6555 RVA: 0x00065760 File Offset: 0x00063960
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			PdfPagePreviewGridViewItem pdfPagePreviewGridViewItem = element as PdfPagePreviewGridViewItem;
			if (pdfPagePreviewGridViewItem != null)
			{
				base.SelectedItems.Remove(item);
				Binding binding = new Binding("Selected")
				{
					Mode = BindingMode.TwoWay
				};
				pdfPagePreviewGridViewItem.SetBinding(ListBoxItem.IsSelectedProperty, binding);
				VisualStateManager.GoToState(pdfPagePreviewGridViewItem, "FocusBorderInvisible", true);
			}
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x000657B8 File Offset: 0x000639B8
		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			base.ClearContainerForItemOverride(element, item);
			PdfPagePreviewGridViewItem pdfPagePreviewGridViewItem = element as PdfPagePreviewGridViewItem;
			if (pdfPagePreviewGridViewItem != null)
			{
				BindingOperations.ClearBinding(pdfPagePreviewGridViewItem, ListBoxItem.IsSelectedProperty);
				VisualStateManager.GoToState(pdfPagePreviewGridViewItem, "FocusBorderInvisible", true);
			}
		}

		// Token: 0x0600199D RID: 6557 RVA: 0x000657F0 File Offset: 0x000639F0
		internal void OnItemsDragStart(PdfPagePreviewGridViewItem dragContainer)
		{
			if (Mouse.LeftButton == MouseButtonState.Pressed)
			{
				this.itemDraging = true;
				if (this.itemDraging)
				{
					bool flag = true;
					PdfPageEditListModel[] array = (base.ItemsSource as PdfPageEditList).SelectedItems.ToArray<PdfPageEditListModel>();
					if (array.Length == base.Items.Count)
					{
						this.itemDraging = false;
						base.ReleaseMouseCapture();
					}
					PdfPagePreviewGridViewItem.draging = this.itemDraging;
					if (!this.itemDraging)
					{
						return;
					}
					if (array.Length == 1)
					{
						this.dragStartItem = array[0];
						this.dragEndItem = array[0];
						this.dragStartItemIdx = base.Items.IndexOf(this.dragStartItem);
						this.dragEndItemIdx = this.dragStartItemIdx;
					}
					else
					{
						object obj = null;
						int num = -1;
						object obj2 = null;
						int num2 = -1;
						HashSet<object> hashSet = new HashSet<object>(array.Distinct<PdfPageEditListModel>());
						for (int i = 0; i < base.Items.Count; i++)
						{
							object obj3 = base.Items[i];
							if (hashSet.Contains(obj3))
							{
								if (num == -1 || i < num)
								{
									num = i;
									obj = obj3;
								}
								if (i > num2)
								{
									num2 = i;
									obj2 = obj3;
								}
							}
						}
						if (num2 - num + 1 != hashSet.Count)
						{
							flag = false;
						}
						this.dragStartItem = obj;
						this.dragStartItemIdx = num;
						this.dragEndItem = obj2;
						this.dragEndItemIdx = num2;
					}
					this.dragingContinuousRange = flag;
					Point position = Mouse.GetPosition(this);
					this.UpdateDragPosition(position);
					VisualStateManager.GoToState(this, "Draging", true);
					object[] array2 = array;
					PdfPagePreviewGridViewItemDragStartEventArgs pdfPagePreviewGridViewItemDragStartEventArgs = new PdfPagePreviewGridViewItemDragStartEventArgs(dragContainer, array2);
					EventHandler<PdfPagePreviewGridViewItemDragStartEventArgs> itemsDragStart = this.ItemsDragStart;
					if (itemsDragStart != null)
					{
						itemsDragStart(this, pdfPagePreviewGridViewItemDragStartEventArgs);
					}
					this.DragInfo.Content = pdfPagePreviewGridViewItemDragStartEventArgs.UIOverride;
				}
			}
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x00065998 File Offset: 0x00063B98
		private void OnItemsDragCompleted(bool cancel)
		{
			VisualStateManager.GoToState(this, "NotDraging", true);
			this.itemDraging = false;
			this.dragTimer.Stop();
			if (this.DragInfo != null)
			{
				this.DragInfo.Content = null;
			}
			PdfPagePreviewGridViewItem.draging = false;
			object obj = this.leftItem;
			object obj2 = this.rightItem;
			PdfPageEditListModel[] array = (base.ItemsSource as PdfPageEditList).SelectedItems.ToArray<PdfPageEditListModel>();
			int num = this.leftItemIdx;
			int num2 = this.rightItemIdx;
			bool flag = this.dragingContinuousRange;
			int num3 = this.dragStartItemIdx;
			int num4 = this.dragEndItemIdx;
			this.leftItem = null;
			this.leftItemIdx = -1;
			this.leftBounds = Rect.Empty;
			this.rightItem = null;
			this.rightItemIdx = -1;
			this.rightBounds = Rect.Empty;
			this.dragingContinuousRange = false;
			this.dragStartItem = null;
			this.dragEndItem = null;
			this.dragStartItemIdx = -1;
			this.dragEndItemIdx = -1;
			bool flag2 = true;
			if (cancel)
			{
				obj = null;
				obj2 = null;
				array = null;
				flag = false;
				flag2 = false;
			}
			else if (num == -1 && num2 == -1)
			{
				flag2 = false;
			}
			else if (flag && ((num != -1 && num >= num3 && num <= num4) || (num2 != -1 && num2 >= num3 && num2 <= num4)))
			{
				flag2 = false;
			}
			object obj3 = obj;
			object obj4 = obj2;
			object[] array2 = array;
			PdfPagePreviewGridViewItemDragCompletedEventArgs pdfPagePreviewGridViewItemDragCompletedEventArgs = new PdfPagePreviewGridViewItemDragCompletedEventArgs(obj3, obj4, array2, flag, flag2);
			EventHandler<PdfPagePreviewGridViewItemDragCompletedEventArgs> itemsDragCompleted = this.ItemsDragCompleted;
			if (itemsDragCompleted == null)
			{
				return;
			}
			itemsDragCompleted(this, pdfPagePreviewGridViewItemDragCompletedEventArgs);
		}

		// Token: 0x0600199F RID: 6559 RVA: 0x00065AF0 File Offset: 0x00063CF0
		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseLeftButtonDown(e);
			ScrollViewer scrollViewer = this.ScrollViewer;
			FrameworkElement frameworkElement = ((scrollViewer != null) ? scrollViewer.Content : null) as FrameworkElement;
			if (frameworkElement != null && VisualTreeHelper.GetChildrenCount(frameworkElement) > 0)
			{
				Panel panel = VisualTreeHelper.GetChild(frameworkElement, 0) as Panel;
				if (panel != null && panel.IsMouseOver)
				{
					this.lastLeftBtnDownPos = e.GetPosition(this);
					this.lastLeftBtnDownTime = DateTime.UtcNow;
				}
			}
		}

		// Token: 0x060019A0 RID: 6560 RVA: 0x00065B58 File Offset: 0x00063D58
		protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseLeftButtonUp(e);
			base.ReleaseMouseCapture();
			Point point = this.lastLeftBtnDownPos;
			DateTime dateTime = this.lastLeftBtnDownTime;
			bool itemClick = this.ItemClick != null;
			EventHandler<PdfPagePreviewGridViewItemEventArgs> itemDoubleClick = this.ItemDoubleClick;
			this.lastLeftBtnDownPos = default(Point);
			this.lastLeftBtnDownTime = default(DateTime);
			Point position = e.GetPosition(this);
			DateTime utcNow = DateTime.UtcNow;
			Point point2 = this.lastLeftBtnClickPos;
			if ((itemClick || itemDoubleClick != null) && (utcNow - dateTime).TotalMilliseconds < 300.0 && Math.Abs(point.X - position.X) < 10.0 && Math.Abs(point.Y - position.Y) < 10.0)
			{
				Panel panel = null;
				ScrollViewer scrollViewer = this.ScrollViewer;
				FrameworkElement frameworkElement = ((scrollViewer != null) ? scrollViewer.Content : null) as FrameworkElement;
				if (frameworkElement != null && VisualTreeHelper.GetChildrenCount(frameworkElement) > 0)
				{
					panel = VisualTreeHelper.GetChild(frameworkElement, 0) as Panel;
				}
				if (panel != null)
				{
					FrameworkElement[] array = panel.Children.OfType<FrameworkElement>().ToArray<FrameworkElement>();
					int i = 0;
					while (i < array.Length)
					{
						Rect layoutSlot = LayoutInformation.GetLayoutSlot(array[i]);
						if (!layoutSlot.IsEmpty && layoutSlot.Contains(position))
						{
							if ((utcNow - this.lastClickTime).TotalMilliseconds < 500.0 && Math.Abs(position.X - point2.X) < 10.0 && Math.Abs(position.Y - point2.Y) < 10.0)
							{
								this.lastLeftBtnClickPos = default(Point);
								this.lastClickTime = default(DateTime);
								EventHandler<PdfPagePreviewGridViewItemEventArgs> itemDoubleClick2 = this.ItemDoubleClick;
								if (itemDoubleClick2 == null)
								{
									return;
								}
								itemDoubleClick2(this, new PdfPagePreviewGridViewItemEventArgs(array[i].DataContext));
								return;
							}
							else
							{
								this.lastLeftBtnClickPos = position;
								this.lastClickTime = utcNow;
								EventHandler<PdfPagePreviewGridViewItemEventArgs> itemClick2 = this.ItemClick;
								if (itemClick2 == null)
								{
									return;
								}
								itemClick2(this, new PdfPagePreviewGridViewItemEventArgs(array[i].DataContext));
								return;
							}
						}
						else
						{
							i++;
						}
					}
				}
				this.lastClickTime = default(DateTime);
			}
		}

		// Token: 0x060019A1 RID: 6561 RVA: 0x00065D8C File Offset: 0x00063F8C
		protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseRightButtonDown(e);
			this.OnItemsDragCompleted(true);
			base.ReleaseMouseCapture();
		}

		// Token: 0x060019A2 RID: 6562 RVA: 0x00065DA2 File Offset: 0x00063FA2
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (e.Key == Key.Escape && this.itemDraging)
			{
				e.Handled = true;
				this.OnItemsDragCompleted(true);
				base.ReleaseMouseCapture();
			}
		}

		// Token: 0x060019A3 RID: 6563 RVA: 0x00065DD4 File Offset: 0x00063FD4
		protected override void OnLostMouseCapture(MouseEventArgs e)
		{
			base.OnLostMouseCapture(e);
			if (e.Source == this && this.itemDraging)
			{
				Point position = Mouse.GetPosition(this);
				bool flag = position.X > 0.0 && position.X < base.ActualWidth && position.Y > 0.0 && position.Y < base.ActualHeight;
				this.OnItemsDragCompleted(!flag);
			}
		}

		// Token: 0x060019A4 RID: 6564 RVA: 0x00065E50 File Offset: 0x00064050
		protected override void OnPreviewMouseMove(MouseEventArgs e)
		{
			base.OnPreviewMouseMove(e);
			if (Mouse.LeftButton == MouseButtonState.Pressed && this.itemDraging)
			{
				Point position = e.GetPosition(this);
				this.UpdateDragPosition(position);
				if (position.Y < 40.0 || position.Y > base.ActualHeight - 40.0)
				{
					if (!this.dragTimer.IsEnabled)
					{
						this.dragTimer.Start();
						return;
					}
				}
				else
				{
					this.dragTimer.Stop();
				}
			}
		}

		// Token: 0x060019A5 RID: 6565 RVA: 0x00065ED4 File Offset: 0x000640D4
		private void UpdateDragPosition(Point mousePos)
		{
			Panel panel = null;
			FrameworkElement frameworkElement = this.ScrollViewer.Content as FrameworkElement;
			if (frameworkElement != null && VisualTreeHelper.GetChildrenCount(frameworkElement) > 0)
			{
				panel = VisualTreeHelper.GetChild(frameworkElement, 0) as Panel;
			}
			FrameworkElement[] array = panel.Children.OfType<FrameworkElement>().ToArray<FrameworkElement>();
			Rect? rect = null;
			int i = 0;
			while (i < array.Length)
			{
				Rect layoutSlot = LayoutInformation.GetLayoutSlot(array[i]);
				if (layoutSlot.IsEmpty)
				{
					this.leftBounds = Rect.Empty;
					this.rightBounds = Rect.Empty;
					this.leftItem = null;
					this.rightItem = null;
					break;
				}
				Point point = new Point(layoutSlot.X + layoutSlot.Width / 2.0, layoutSlot.Y + layoutSlot.Height / 2.0);
				if (layoutSlot.Contains(mousePos))
				{
					if (mousePos.X < point.X)
					{
						if (rect != null && rect.Value.Top + rect.Value.Height / 2.0 > layoutSlot.Top)
						{
							this.leftItem = array[i - 1].DataContext;
							this.leftBounds = rect.Value;
						}
						else
						{
							this.leftItem = null;
							this.leftBounds = Rect.Empty;
						}
						this.rightItem = array[i].DataContext;
						this.rightBounds = layoutSlot;
						break;
					}
					Rect? rect2 = null;
					if (i < array.Length - 1)
					{
						rect2 = new Rect?(LayoutInformation.GetLayoutSlot(array[i + 1]));
						if (rect2.Value.IsEmpty)
						{
							rect2 = null;
						}
					}
					if (rect2 != null && layoutSlot.Top + layoutSlot.Height / 2.0 > rect2.Value.Top)
					{
						this.rightItem = array[i + 1].DataContext;
						this.rightBounds = rect2.Value;
					}
					else
					{
						this.rightItem = null;
						this.rightBounds = Rect.Empty;
					}
					this.leftItem = array[i].DataContext;
					this.leftBounds = layoutSlot;
					break;
				}
				else
				{
					rect = new Rect?(layoutSlot);
					i++;
				}
			}
			this.leftItemIdx = ((this.leftItem != null) ? base.Items.IndexOf(this.leftItem) : (-1));
			this.rightItemIdx = ((this.rightItem != null) ? base.Items.IndexOf(this.rightItem) : (-1));
			if (this.DragInfo != null)
			{
				Canvas.SetLeft(this.DragInfo, mousePos.X);
				Canvas.SetTop(this.DragInfo, mousePos.Y);
			}
			if (this.InsertPlaceholder != null)
			{
				if (this.leftItem == null && this.rightItem == null)
				{
					this.InsertPlaceholder.Visibility = Visibility.Collapsed;
				}
				else if (this.dragingContinuousRange)
				{
					if ((this.leftItemIdx != -1 && this.leftItemIdx >= this.dragStartItemIdx && this.leftItemIdx <= this.dragEndItemIdx) || (this.rightItemIdx != -1 && this.rightItemIdx >= this.dragStartItemIdx && this.rightItemIdx <= this.dragEndItemIdx))
					{
						this.InsertPlaceholder.Visibility = Visibility.Collapsed;
					}
					else
					{
						this.InsertPlaceholder.Visibility = Visibility.Visible;
					}
				}
				else
				{
					this.InsertPlaceholder.Visibility = Visibility.Visible;
				}
				Rect rect3 = this.leftBounds;
				if (rect3.IsEmpty)
				{
					rect3 = this.rightBounds;
				}
				this.InsertPlaceholder.Height = Math.Max(50.0, rect3.Height - 50.0);
				double num = rect3.Top + (rect3.Height - this.InsertPlaceholder.Height) / 2.0;
				double num2;
				if (this.leftItem != null && this.rightItem != null)
				{
					num2 = this.leftBounds.Right + (this.rightBounds.Left - this.leftBounds.Right) / 2.0;
					num2 += 6.0;
				}
				else if (this.leftItem != null)
				{
					num2 = this.leftBounds.Right + 4.0;
				}
				else
				{
					num2 = this.rightBounds.Left - 4.0;
				}
				Canvas.SetLeft(this.InsertPlaceholder, num2);
				Canvas.SetTop(this.InsertPlaceholder, num);
			}
		}

		// Token: 0x060019A6 RID: 6566 RVA: 0x00066350 File Offset: 0x00064550
		private void DragTimer_Tick(object sender, EventArgs e)
		{
			ScrollViewer scrollViewer = this.ScrollViewer;
			if (scrollViewer == null)
			{
				this.dragTimer.Stop();
				return;
			}
			Point p = Mouse.GetPosition(this);
			if (p.Y < 40.0)
			{
				double num = 300.0 * (40.0 - Math.Max(0.0, p.Y)) / 40.0;
				double num2 = scrollViewer.VerticalOffset - num;
				scrollViewer.ScrollToVerticalOffset(num2);
				scrollViewer.UpdateLayout();
				base.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(delegate
				{
					this.UpdateDragPosition(p);
				}));
				return;
			}
			if (p.Y > base.ActualHeight - 40.0)
			{
				double num3 = 300.0 * (40.0 - Math.Max(0.0, base.ActualHeight - p.Y)) / 40.0;
				double num4 = scrollViewer.VerticalOffset + num3;
				scrollViewer.ScrollToVerticalOffset(num4);
				scrollViewer.UpdateLayout();
				base.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(delegate
				{
					this.UpdateDragPosition(p);
				}));
				return;
			}
			this.dragTimer.Stop();
		}

		// Token: 0x1400002E RID: 46
		// (add) Token: 0x060019A7 RID: 6567 RVA: 0x000664A8 File Offset: 0x000646A8
		// (remove) Token: 0x060019A8 RID: 6568 RVA: 0x000664E0 File Offset: 0x000646E0
		public event EventHandler<PdfPagePreviewGridViewItemDragStartEventArgs> ItemsDragStart;

		// Token: 0x1400002F RID: 47
		// (add) Token: 0x060019A9 RID: 6569 RVA: 0x00066518 File Offset: 0x00064718
		// (remove) Token: 0x060019AA RID: 6570 RVA: 0x00066550 File Offset: 0x00064750
		public event EventHandler<PdfPagePreviewGridViewItemDragCompletedEventArgs> ItemsDragCompleted;

		// Token: 0x14000030 RID: 48
		// (add) Token: 0x060019AB RID: 6571 RVA: 0x00066588 File Offset: 0x00064788
		// (remove) Token: 0x060019AC RID: 6572 RVA: 0x000665C0 File Offset: 0x000647C0
		public event EventHandler<PdfPagePreviewGridViewItemEventArgs> ItemClick;

		// Token: 0x14000031 RID: 49
		// (add) Token: 0x060019AD RID: 6573 RVA: 0x000665F8 File Offset: 0x000647F8
		// (remove) Token: 0x060019AE RID: 6574 RVA: 0x00066630 File Offset: 0x00064830
		public event EventHandler<PdfPagePreviewGridViewItemEventArgs> ItemDoubleClick;

		// Token: 0x040008C8 RID: 2248
		private DispatcherTimer dragTimer;

		// Token: 0x040008C9 RID: 2249
		private bool itemDraging;

		// Token: 0x040008CA RID: 2250
		private bool dragingContinuousRange;

		// Token: 0x040008CB RID: 2251
		private object dragStartItem;

		// Token: 0x040008CC RID: 2252
		private int dragStartItemIdx = -1;

		// Token: 0x040008CD RID: 2253
		private object dragEndItem;

		// Token: 0x040008CE RID: 2254
		private int dragEndItemIdx = -1;

		// Token: 0x040008CF RID: 2255
		private Point lastLeftBtnDownPos;

		// Token: 0x040008D0 RID: 2256
		private DateTime lastLeftBtnDownTime;

		// Token: 0x040008D1 RID: 2257
		private Point lastLeftBtnClickPos;

		// Token: 0x040008D2 RID: 2258
		private DateTime lastClickTime;

		// Token: 0x040008D3 RID: 2259
		private Border Bd;

		// Token: 0x040008D4 RID: 2260
		private ScrollViewer ScrollViewer;

		// Token: 0x040008D5 RID: 2261
		private ContentPresenter DragInfo;

		// Token: 0x040008D6 RID: 2262
		private FrameworkElement InsertPlaceholder;

		// Token: 0x040008D7 RID: 2263
		private Rect leftBounds;

		// Token: 0x040008D8 RID: 2264
		private Rect rightBounds;

		// Token: 0x040008D9 RID: 2265
		private object leftItem;

		// Token: 0x040008DA RID: 2266
		private int leftItemIdx;

		// Token: 0x040008DB RID: 2267
		private object rightItem;

		// Token: 0x040008DC RID: 2268
		private int rightItemIdx;
	}
}
