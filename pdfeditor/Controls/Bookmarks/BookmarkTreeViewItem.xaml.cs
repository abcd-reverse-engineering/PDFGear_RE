using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Patagames.Pdf.Net;
using pdfeditor.Models.Bookmarks;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;

namespace pdfeditor.Controls.Bookmarks
{
	// Token: 0x02000299 RID: 665
	internal sealed partial class BookmarkTreeViewItem : TreeViewItem
	{
		// Token: 0x06002632 RID: 9778 RVA: 0x000B2390 File Offset: 0x000B0590
		static BookmarkTreeViewItem()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(BookmarkTreeViewItem), new FrameworkPropertyMetadata(typeof(BookmarkTreeViewItem)));
			EventManager.RegisterClassHandler(typeof(BookmarkTreeViewItem), FrameworkElement.RequestBringIntoViewEvent, new RequestBringIntoViewEventHandler(BookmarkTreeViewItem.OnRequestBringIntoView));
		}

		// Token: 0x06002633 RID: 9779 RVA: 0x000B2410 File Offset: 0x000B0610
		public BookmarkTreeViewItem()
		{
			base.DataContextChanged += this.BookmarkTreeViewItem_DataContextChanged;
			base.Loaded += delegate(object s, RoutedEventArgs a)
			{
				VisualStateManager.GoToState(this, "DropIndicatorInvisible", true);
				this.UpdateDraggingState(false);
			};
			Border border = new Border();
			TextBlock textBlock = new TextBlock();
			textBlock.TextWrapping = TextWrapping.Wrap;
			TextBlock textBlock2 = textBlock;
			this.toolTipText = textBlock;
			border.Child = textBlock2;
			border.MaxWidth = 320.0;
			this.toolTipContent = border;
		}

		// Token: 0x17000BC7 RID: 3015
		// (get) Token: 0x06002634 RID: 9780 RVA: 0x000B247C File Offset: 0x000B067C
		// (set) Token: 0x06002635 RID: 9781 RVA: 0x000B248E File Offset: 0x000B068E
		public bool IsHighlighted
		{
			get
			{
				return (bool)base.GetValue(BookmarkTreeViewItem.IsHighlightedProperty);
			}
			set
			{
				base.SetValue(BookmarkTreeViewItem.IsHighlightedProperty, value);
			}
		}

		// Token: 0x06002636 RID: 9782 RVA: 0x000B24A4 File Offset: 0x000B06A4
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (this.ContentBorder != null)
			{
				this.ContentBorder.MouseDown -= this.Control_MouseDown;
				this.ContentBorder.ToolTipOpening -= this.Control_ToolTipOpening;
				this.<OnApplyTemplate>g__SetToolTip|14_0(this.ContentBorder, false);
			}
			if (this.BackgroundBorder != null)
			{
				this.BackgroundBorder.MouseDown -= this.Control_MouseDown;
				this.BackgroundBorder.ToolTipOpening -= this.Control_ToolTipOpening;
				this.<OnApplyTemplate>g__SetToolTip|14_0(this.BackgroundBorder, false);
			}
			if (this.Expander != null)
			{
				this.Expander.PreviewMouseLeftButtonDown -= this.Expander_PreviewMouseLeftButtonDown;
			}
			this.LayoutRoot = base.GetTemplateChild("LayoutRoot") as Grid;
			this.MarginColumn = base.GetTemplateChild("MarginColumn") as ColumnDefinition;
			this.BackgroundBorder = base.GetTemplateChild("BackgroundBorder") as Border;
			this.ContentBorder = base.GetTemplateChild("Bd") as Border;
			this.Expander = base.GetTemplateChild("Expander") as ToggleButton;
			if (this.ContentBorder != null)
			{
				this.ContentBorder.MouseDown += this.Control_MouseDown;
				this.ContentBorder.ToolTipOpening += this.Control_ToolTipOpening;
				this.<OnApplyTemplate>g__SetToolTip|14_0(this.ContentBorder, true);
			}
			if (this.BackgroundBorder != null)
			{
				this.BackgroundBorder.MouseDown += this.Control_MouseDown;
				this.BackgroundBorder.ToolTipOpening += this.Control_ToolTipOpening;
				this.<OnApplyTemplate>g__SetToolTip|14_0(this.BackgroundBorder, true);
			}
			if (this.Expander != null)
			{
				this.Expander.PreviewMouseLeftButtonDown += this.Expander_PreviewMouseLeftButtonDown;
			}
			this.UpdateBackgroundSize();
			this.UpdateDraggingState(false);
			VisualStateManager.GoToState(this, "DropIndicatorInvisible", false);
		}

		// Token: 0x06002637 RID: 9783 RVA: 0x000B268C File Offset: 0x000B088C
		private void UpdateDraggingState(bool useTransitions)
		{
			BookmarkModel bookmarkModel = base.DataContext as BookmarkModel;
			BookmarkTreeViewItem.DragDropDataModel dragDropDataModel;
			if (bookmarkModel != null && BookmarkTreeViewItem.draggingModel != null && BookmarkTreeViewItem.draggingModel.TryGetTarget(out dragDropDataModel) && bookmarkModel == dragDropDataModel.Bookmark)
			{
				VisualStateManager.GoToState(this, "Dragging", useTransitions);
				return;
			}
			VisualStateManager.GoToState(this, "NotDragging", useTransitions);
		}

		// Token: 0x06002638 RID: 9784 RVA: 0x000B26E4 File Offset: 0x000B08E4
		private void Control_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
			{
				e.Handled = true;
				BookmarkModel bookmarkModel = base.DataContext as BookmarkModel;
				if (bookmarkModel != null)
				{
					BookmarkRenameDialog.Create(bookmarkModel).ShowDialog();
				}
			}
		}

		// Token: 0x06002639 RID: 9785 RVA: 0x000B2724 File Offset: 0x000B0924
		private void Control_ToolTipOpening(object sender, ToolTipEventArgs e)
		{
			e.Handled = true;
			Border contentBorder = this.ContentBorder;
			FrameworkElement frameworkElement = ((contentBorder != null) ? contentBorder.Child : null) as FrameworkElement;
			if (frameworkElement != null)
			{
				BookmarkModel bookmarkModel = base.DataContext as BookmarkModel;
				if (bookmarkModel != null)
				{
					ItemsControl itemsControl = this;
					while ((itemsControl = ItemsControl.ItemsControlFromItemContainer(itemsControl)) is BookmarkTreeViewItem)
					{
					}
					BookmarkTreeView bookmarkTreeView = itemsControl as BookmarkTreeView;
					if (bookmarkTreeView != null && BookmarkTreeViewItem.<Control_ToolTipOpening>g__IsOverflow|17_0(bookmarkTreeView, frameworkElement))
					{
						this.toolTipText.Text = bookmarkModel.Title ?? string.Empty;
						e.Handled = false;
					}
				}
			}
		}

		// Token: 0x0600263A RID: 9786 RVA: 0x000B27A8 File Offset: 0x000B09A8
		private void Expander_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			bool? isChecked = ((ToggleButton)sender).IsChecked;
			if (isChecked != null && isChecked.GetValueOrDefault())
			{
				this.lastClickCollapseExpanderTime = DateTime.Now;
			}
		}

		// Token: 0x17000BC8 RID: 3016
		// (get) Token: 0x0600263B RID: 9787 RVA: 0x000B27E0 File Offset: 0x000B09E0
		internal bool ShouldUpdatePageIndex
		{
			get
			{
				return (DateTime.Now - this.lastClickCollapseExpanderTime).TotalMilliseconds > 800.0;
			}
		}

		// Token: 0x0600263C RID: 9788 RVA: 0x000B2810 File Offset: 0x000B0A10
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && base.CaptureMouse())
			{
				e.Handled = true;
				this.lastMouseDownPosition = new Point?(e.GetPosition(this));
				return;
			}
			base.OnMouseDown(e);
			this.lastMouseDownPosition = null;
		}

		// Token: 0x0600263D RID: 9789 RVA: 0x000B2850 File Offset: 0x000B0A50
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			BookmarkTreeView parentTreeView = this.ParentTreeView;
			if (parentTreeView != null && !parentTreeView.IsKeyboardFocusWithin)
			{
				FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
				while (frameworkElement != null && !(frameworkElement is TreeViewItem))
				{
					frameworkElement = (frameworkElement.Parent ?? VisualTreeHelper.GetParent(frameworkElement)) as FrameworkElement;
				}
				if (frameworkElement != null)
				{
					Keyboard.Focus(frameworkElement);
					return;
				}
				Keyboard.Focus(this);
			}
		}

		// Token: 0x0600263E RID: 9790 RVA: 0x000B28B0 File Offset: 0x000B0AB0
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			Point? point = this.lastMouseDownPosition;
			this.lastMouseDownPosition = null;
			if (base.IsMouseCaptured)
			{
				e.Handled = true;
				base.ReleaseMouseCapture();
				Point position = e.GetPosition(this);
				if (point != null && Math.Abs(point.Value.X - position.X) < 10.0 && Math.Abs(point.Value.Y - position.Y) < 10.0)
				{
					BookmarkModel bookmarkModel = base.DataContext as BookmarkModel;
					if (bookmarkModel != null)
					{
						if (!bookmarkModel.IsSelected)
						{
							bookmarkModel.IsSelected = true;
							return;
						}
						if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
						{
							if (bookmarkModel.IsSelected)
							{
								bookmarkModel.IsSelected = false;
								return;
							}
						}
						else
						{
							BookmarkControl parentBookmarkControl = this.ParentBookmarkControl;
							if (((parentBookmarkControl != null) ? parentBookmarkControl.PdfViewer : null) != null)
							{
								int? num;
								if (parentBookmarkControl == null)
								{
									num = null;
								}
								else
								{
									PdfViewer pdfViewer = parentBookmarkControl.PdfViewer;
									num = ((pdfViewer != null) ? new int?(pdfViewer.CurrentIndex) : null);
								}
								int? num2 = num;
								int pageIndex = bookmarkModel.PageIndex;
								if (!((num2.GetValueOrDefault() == pageIndex) & (num2 != null)))
								{
									parentBookmarkControl.UpdateSelectedPageIndex();
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600263F RID: 9791 RVA: 0x000B2A0C File Offset: 0x000B0C0C
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Point? point = this.lastMouseDownPosition;
			BookmarkTreeView parentTreeView = this.ParentTreeView;
			bool? flag = ((parentTreeView != null) ? new bool?(parentTreeView.CanDragItems) : null);
			if (flag != null && flag.GetValueOrDefault() && base.DataContext is BookmarkModel && point != null && base.IsMouseCaptured && (e.LeftButton & MouseButtonState.Pressed) != MouseButtonState.Released)
			{
				Point position = e.GetPosition(this);
				if (Math.Abs(point.Value.X - position.X) >= 10.0 || Math.Abs(point.Value.Y - position.Y) >= 10.0)
				{
					e.Handled = true;
					this.DoDragDropOperation(point.Value, false);
				}
			}
		}

		// Token: 0x06002640 RID: 9792 RVA: 0x000B2AFC File Offset: 0x000B0CFC
		protected override void OnLostStylusCapture(StylusEventArgs e)
		{
			base.OnLostStylusCapture(e);
			BookmarkTreeViewItem.DragDropDataModel dragDropDataModel;
			if (BookmarkTreeViewItem.draggingModel != null && BookmarkTreeViewItem.draggingModel.TryGetTarget(out dragDropDataModel) && dragDropDataModel.BookmarkControl == null)
			{
				BookmarkTreeViewItem.ResetCachedDraggingModel();
			}
			this.UpdateDraggingState(true);
		}

		// Token: 0x06002641 RID: 9793 RVA: 0x000B2B3C File Offset: 0x000B0D3C
		protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
		{
			base.OnStylusSystemGesture(e);
			BookmarkTreeView parentTreeView = this.ParentTreeView;
			bool? flag = ((parentTreeView != null) ? new bool?(parentTreeView.CanDragItems) : null);
			if (flag != null && flag.GetValueOrDefault())
			{
				if (e.SystemGesture == SystemGesture.HoldEnter && base.CaptureStylus())
				{
					ScrollViewer parentScrollViewer = this.ParentScrollViewer;
					if (this.ContentBorder != null && parentScrollViewer != null)
					{
						e.Handled = true;
						Point position = e.GetPosition(this.ContentBorder);
						if (position.Y >= 0.0 && position.Y <= this.ContentBorder.ActualHeight)
						{
							parentScrollViewer.PanningMode = PanningMode.None;
							Point position2 = e.GetPosition(this);
							BookmarkTreeViewItem.draggingModel = new WeakReference<BookmarkTreeViewItem.DragDropDataModel>(new BookmarkTreeViewItem.DragDropDataModel
							{
								Bookmark = (base.DataContext as BookmarkModel),
								TreeViewScrollViewer = parentScrollViewer,
								IsTouching = true,
								DragStartPoint = position2
							});
							e.Handled = true;
						}
					}
				}
				else if (e.SystemGesture == SystemGesture.RightDrag)
				{
					BookmarkTreeViewItem.DragDropDataModel dragDropDataModel;
					if (BookmarkTreeViewItem.draggingModel != null && BookmarkTreeViewItem.draggingModel.TryGetTarget(out dragDropDataModel))
					{
						base.CaptureStylus();
						e.Handled = true;
						this.DoDragDropOperation(dragDropDataModel.DragStartPoint, true);
					}
				}
				else
				{
					base.ReleaseStylusCapture();
					BookmarkTreeViewItem.ResetCachedDraggingModel();
				}
				this.UpdateDraggingState(true);
			}
		}

		// Token: 0x06002642 RID: 9794 RVA: 0x000B2C97 File Offset: 0x000B0E97
		protected override void OnDragEnter(DragEventArgs e)
		{
			base.OnDragEnter(e);
			this.ProcessDragEvent(e);
		}

		// Token: 0x06002643 RID: 9795 RVA: 0x000B2CA8 File Offset: 0x000B0EA8
		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);
			BookmarkTreeViewItem.DragDropProcessResult dragDropProcessResult = this.ProcessDragEvent(e);
			if ((e.Effects & DragDropEffects.Scroll) != DragDropEffects.None && dragDropProcessResult.ScrollOffset != 0.0 && dragDropProcessResult.BookmarkTreeViewScrollViewer != null)
			{
				double num = dragDropProcessResult.BookmarkTreeViewScrollViewer.VerticalOffset + dragDropProcessResult.ScrollOffset;
				dragDropProcessResult.BookmarkTreeViewScrollViewer.ScrollToVerticalOffset(Math.Max(Math.Min(num, dragDropProcessResult.BookmarkTreeViewScrollViewer.ScrollableHeight), 0.0));
			}
			if ((e.Effects & DragDropEffects.Move) == DragDropEffects.None || dragDropProcessResult.Model == null)
			{
				VisualStateManager.GoToState(this, "DropIndicatorInvisible", true);
				return;
			}
			switch (dragDropProcessResult.InsertPosition)
			{
			case BookmarkTreeViewItem.DragDropProcessResult.InsertPositionEnum.InsertToChildren:
				VisualStateManager.GoToState(this, "DropIndicatorForChildren", true);
				return;
			case BookmarkTreeViewItem.DragDropProcessResult.InsertPositionEnum.InsertAsPreviousSibling:
				VisualStateManager.GoToState(this, "DropIndicatorForPreviousSibling", true);
				return;
			case BookmarkTreeViewItem.DragDropProcessResult.InsertPositionEnum.InsertAsNextSibling:
				VisualStateManager.GoToState(this, "DropIndicatorForNextSibling", true);
				return;
			default:
				return;
			}
		}

		// Token: 0x06002644 RID: 9796 RVA: 0x000B2D8B File Offset: 0x000B0F8B
		protected override void OnPreviewDragLeave(DragEventArgs e)
		{
			base.OnPreviewDragLeave(e);
			VisualStateManager.GoToState(this, "DropIndicatorInvisible", true);
		}

		// Token: 0x06002645 RID: 9797 RVA: 0x000B2DA4 File Offset: 0x000B0FA4
		protected override async void OnDrop(DragEventArgs e)
		{
			base.OnDrop(e);
			VisualStateManager.GoToState(this, "DropIndicatorInvisible", true);
			BookmarkModel bookmarkModel = base.DataContext as BookmarkModel;
			if (bookmarkModel != null)
			{
				BookmarkTreeViewItem.DragDropProcessResult dragDropProcessResult = this.ProcessDragEvent(e);
				BookmarkControl bookmarkControl = dragDropProcessResult.Model.BookmarkControl;
				if (dragDropProcessResult.Model != null)
				{
					PdfViewer pdfViewer = bookmarkControl.PdfViewer;
					MainViewModel mainViewModel = ((pdfViewer != null) ? pdfViewer.DataContext : null) as MainViewModel;
					if (mainViewModel != null)
					{
						bool selected = dragDropProcessResult.Model.Bookmark.IsSelected;
						BookmarkModel bookmarkModel2 = null;
						int num = -1;
						if (dragDropProcessResult.InsertPosition == BookmarkTreeViewItem.DragDropProcessResult.InsertPositionEnum.InsertToChildren)
						{
							bookmarkModel2 = bookmarkModel;
							num = 0;
						}
						else if (dragDropProcessResult.InsertPosition == BookmarkTreeViewItem.DragDropProcessResult.InsertPositionEnum.InsertAsPreviousSibling)
						{
							bookmarkModel2 = bookmarkModel.Parent;
							num = BookmarkTreeViewItem.<OnDrop>g__IndexOf|32_0(bookmarkModel2.Children, bookmarkModel);
							if (num == -1)
							{
								num = 0;
							}
						}
						else if (dragDropProcessResult.InsertPosition == BookmarkTreeViewItem.DragDropProcessResult.InsertPositionEnum.InsertAsNextSibling)
						{
							bookmarkModel2 = bookmarkModel.Parent;
							num = BookmarkTreeViewItem.<OnDrop>g__IndexOf|32_0(bookmarkModel2.Children, bookmarkModel);
							if (num == -1)
							{
								num = bookmarkModel2.Children.Count;
							}
							else
							{
								num++;
							}
						}
						if (bookmarkModel2 != null)
						{
							PdfDocument document = bookmarkControl.PdfViewer.Document;
							bookmarkControl.IsHitTestVisible = false;
							try
							{
								BookmarkModel bookmarkModel3 = await mainViewModel.OperationManager.MoveBookmarkAsync(document, dragDropProcessResult.Model.Bookmark, bookmarkModel2, num, "");
								if (bookmarkModel3 != null && selected)
								{
									bookmarkControl.SelectedItem = bookmarkModel3;
								}
							}
							finally
							{
								bookmarkControl.IsHitTestVisible = true;
							}
						}
					}
				}
				bookmarkControl = null;
			}
		}

		// Token: 0x06002646 RID: 9798 RVA: 0x000B2DE4 File Offset: 0x000B0FE4
		private BookmarkTreeViewItem.DragDropProcessResult ProcessDragEvent(DragEventArgs e)
		{
			e.Handled = true;
			if (e.Data.GetDataPresent(typeof(BookmarkTreeViewItem.DragDropDataModel)))
			{
				BookmarkTreeViewItem.DragDropDataModel dragDropDataModel = (BookmarkTreeViewItem.DragDropDataModel)e.Data.GetData(typeof(BookmarkTreeViewItem.DragDropDataModel));
				double num = 0.0;
				ScrollViewer treeViewScrollViewer = dragDropDataModel.TreeViewScrollViewer;
				Point? point = null;
				Point? point2 = null;
				BookmarkTreeViewItem._POINT point3;
				if (this.ContentBorder != null && BookmarkTreeViewItem.GetCursorPos(out point3))
				{
					point = new Point?(base.PointFromScreen(new Point((double)point3.X, (double)point3.Y)));
					point2 = new Point?(base.TransformToVisual(this.ContentBorder).Transform(point.Value));
					if (point2.Value.Y >= 0.0 && point2.Value.Y <= this.ContentBorder.ActualHeight)
					{
						Point point4 = base.TransformToVisual(treeViewScrollViewer).Transform(point.Value);
						if (point4.Y < 50.0 && treeViewScrollViewer.ContentVerticalOffset > 0.0)
						{
							e.Effects = DragDropEffects.Scroll;
							num = -20.0;
						}
						else if (point4.Y > treeViewScrollViewer.ActualHeight - 50.0 && treeViewScrollViewer.ContentVerticalOffset < treeViewScrollViewer.ScrollableHeight)
						{
							e.Effects = DragDropEffects.Scroll;
							num = 20.0;
						}
					}
					else
					{
						dragDropDataModel = null;
					}
				}
				if (dragDropDataModel != null)
				{
					BookmarkTreeViewItem.DragDropProcessResult.InsertPositionEnum insertPositionEnum = BookmarkTreeViewItem.DragDropProcessResult.InsertPositionEnum.InsertAsNextSibling;
					if (dragDropDataModel.BookmarkControl != null && BookmarkTreeViewItem.AllowDropToItem(dragDropDataModel.Bookmark, base.DataContext as BookmarkModel))
					{
						e.Effects = DragDropEffects.Move;
						global::System.Collections.Generic.IReadOnlyList<BookmarkModel> children = dragDropDataModel.BookmarkControl.Bookmarks.Children;
						if (((children != null) ? children.FirstOrDefault<BookmarkModel>() : null) == base.DataContext && point2 != null && point2.Value.Y < this.ContentBorder.ActualHeight / 5.0 * 2.0)
						{
							insertPositionEnum = BookmarkTreeViewItem.DragDropProcessResult.InsertPositionEnum.InsertAsPreviousSibling;
						}
						else if (this.ContentBorder != null && point != null)
						{
							if (dragDropDataModel.IsTouching)
							{
								if (base.TransformToVisual(dragDropDataModel.TreeViewScrollViewer).Transform(point.Value).X > dragDropDataModel.TreeViewScrollViewer.ActualWidth / 2.0)
								{
									insertPositionEnum = BookmarkTreeViewItem.DragDropProcessResult.InsertPositionEnum.InsertToChildren;
								}
							}
							else if (base.TransformToVisual(this.ContentBorder).Transform(point.Value).X > 20.0)
							{
								insertPositionEnum = BookmarkTreeViewItem.DragDropProcessResult.InsertPositionEnum.InsertToChildren;
							}
						}
					}
					else
					{
						e.Effects = DragDropEffects.None;
					}
					if (num != 0.0)
					{
						e.Effects |= DragDropEffects.Scroll;
					}
					return new BookmarkTreeViewItem.DragDropProcessResult
					{
						Model = dragDropDataModel,
						ScrollOffset = num,
						BookmarkTreeViewScrollViewer = treeViewScrollViewer,
						MousePositionForItem = point,
						InsertPosition = insertPositionEnum
					};
				}
			}
			e.Effects = DragDropEffects.None;
			return default(BookmarkTreeViewItem.DragDropProcessResult);
		}

		// Token: 0x06002647 RID: 9799 RVA: 0x000B3104 File Offset: 0x000B1304
		private static bool AllowDropToItem(BookmarkModel draggingModel, BookmarkModel currentModel)
		{
			if (draggingModel == null || currentModel == null)
			{
				return false;
			}
			if (draggingModel == currentModel)
			{
				return false;
			}
			for (BookmarkModel bookmarkModel = currentModel.Parent; bookmarkModel != null; bookmarkModel = bookmarkModel.Parent)
			{
				if (bookmarkModel == draggingModel)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06002648 RID: 9800 RVA: 0x000B313C File Offset: 0x000B133C
		private bool DoDragDropOperation(Point startPoint, bool isTouching)
		{
			BookmarkTreeViewItem.DragDropDataModel dragDropDataModel;
			if (BookmarkTreeViewItem.draggingModel != null && BookmarkTreeViewItem.draggingModel.TryGetTarget(out dragDropDataModel) && dragDropDataModel.BookmarkControl == null)
			{
				BookmarkTreeViewItem.ResetCachedDraggingModel();
			}
			if (BookmarkTreeViewItem.draggingModel == null)
			{
				BookmarkModel bookmarkModel = base.DataContext as BookmarkModel;
				if (bookmarkModel != null)
				{
					ScrollViewer parentScrollViewer = this.ParentScrollViewer;
					BookmarkControl parentBookmarkControl = this.ParentBookmarkControl;
					if (parentScrollViewer == null || parentBookmarkControl == null)
					{
						return false;
					}
					if (isTouching)
					{
						parentScrollViewer.PanningMode = PanningMode.None;
					}
					dragDropDataModel = new BookmarkTreeViewItem.DragDropDataModel
					{
						Bookmark = bookmarkModel,
						DragStartPoint = startPoint,
						ContainerSize = new Size(base.ActualWidth, base.ActualHeight),
						TreeViewScrollViewer = parentScrollViewer,
						BookmarkControl = parentBookmarkControl,
						IsTouching = isTouching
					};
					BookmarkTreeViewItem.draggingModel = new WeakReference<BookmarkTreeViewItem.DragDropDataModel>(dragDropDataModel);
					this.UpdateDraggingState(true);
					DragDrop.DoDragDrop(this, dragDropDataModel, DragDropEffects.Move);
					this.OnDropCompleted(dragDropDataModel);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002649 RID: 9801 RVA: 0x000B320C File Offset: 0x000B140C
		private void OnDropCompleted(BookmarkTreeViewItem.DragDropDataModel dragDropModel)
		{
			dragDropModel.TreeViewScrollViewer.PanningMode = PanningMode.VerticalFirst;
			BookmarkTreeViewItem.ResetCachedDraggingModel();
			FrameworkElement frameworkElement = dragDropModel.TreeViewScrollViewer;
			while (frameworkElement != null && !(frameworkElement is BookmarkTreeView))
			{
				frameworkElement = (frameworkElement.Parent ?? VisualTreeHelper.GetParent(frameworkElement)) as FrameworkElement;
			}
			if (frameworkElement != null)
			{
				BookmarkTreeViewItem bookmarkTreeViewItem = ((BookmarkTreeView)frameworkElement).TreeViewItemFromElement(dragDropModel.Bookmark) as BookmarkTreeViewItem;
				if (bookmarkTreeViewItem != null)
				{
					bookmarkTreeViewItem.UpdateDraggingState(true);
				}
			}
			if (dragDropModel.IsTouching)
			{
				base.ReleaseStylusCapture();
			}
		}

		// Token: 0x0600264A RID: 9802 RVA: 0x000B3288 File Offset: 0x000B1488
		private static void ResetCachedDraggingModel()
		{
			WeakReference<BookmarkTreeViewItem.DragDropDataModel> weakReference = BookmarkTreeViewItem.draggingModel;
			BookmarkTreeViewItem.draggingModel = null;
			BookmarkTreeViewItem.DragDropDataModel dragDropDataModel;
			if (weakReference != null && weakReference.TryGetTarget(out dragDropDataModel) && dragDropDataModel.TreeViewScrollViewer != null)
			{
				dragDropDataModel.TreeViewScrollViewer.PanningMode = PanningMode.VerticalFirst;
			}
		}

		// Token: 0x0600264B RID: 9803 RVA: 0x000B32C2 File Offset: 0x000B14C2
		private void BookmarkTreeViewItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.UpdateBackgroundSize();
		}

		// Token: 0x0600264C RID: 9804 RVA: 0x000B32CC File Offset: 0x000B14CC
		private void UpdateBackgroundSize()
		{
			if (this.MarginColumn == null)
			{
				return;
			}
			if (this.BackgroundBorder == null)
			{
				return;
			}
			BookmarkModel bookmarkModel = base.DataContext as BookmarkModel;
			if (bookmarkModel != null)
			{
				int level = bookmarkModel.Level;
				double num = 0.0;
				if (level > 0)
				{
					num = (double)(19 * level);
				}
				this.MarginColumn.MaxWidth = num;
				this.MarginColumn.Width = new GridLength(num, GridUnitType.Pixel);
			}
		}

		// Token: 0x0600264D RID: 9805 RVA: 0x000B3333 File Offset: 0x000B1533
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is BookmarkTreeViewItem;
		}

		// Token: 0x0600264E RID: 9806 RVA: 0x000B333E File Offset: 0x000B153E
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new BookmarkTreeViewItem();
		}

		// Token: 0x0600264F RID: 9807 RVA: 0x000B3348 File Offset: 0x000B1548
		private static void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
		{
			if (e.TargetObject == sender || e.TargetObject is ContentPresenter || e.TargetObject is TextBox)
			{
				BookmarkTreeViewItem container = sender as BookmarkTreeViewItem;
				if (container != null)
				{
					if (container.LayoutRoot != null)
					{
						e.Handled = true;
						container.LayoutRoot.BringIntoView(new Rect(0.0, 0.0, 20.0, 20.0));
						return;
					}
					container.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
					{
						Grid layoutRoot = container.LayoutRoot;
						if (layoutRoot == null)
						{
							return;
						}
						layoutRoot.BringIntoView(new Rect(0.0, 0.0, 20.0, 20.0));
					}));
				}
			}
		}

		// Token: 0x17000BC9 RID: 3017
		// (get) Token: 0x06002650 RID: 9808 RVA: 0x000B3404 File Offset: 0x000B1604
		private BookmarkTreeView ParentTreeView
		{
			get
			{
				ItemsControl itemsControl = this;
				while (itemsControl != null && !(itemsControl is BookmarkTreeView))
				{
					itemsControl = ItemsControl.ItemsControlFromItemContainer(itemsControl);
				}
				return itemsControl as BookmarkTreeView;
			}
		}

		// Token: 0x17000BCA RID: 3018
		// (get) Token: 0x06002651 RID: 9809 RVA: 0x000B3430 File Offset: 0x000B1630
		private ScrollViewer ParentScrollViewer
		{
			get
			{
				BookmarkTreeView parentTreeView = this.ParentTreeView;
				if (parentTreeView != null && VisualTreeHelper.GetChildrenCount(parentTreeView) > 0)
				{
					FrameworkElement frameworkElement = VisualTreeHelper.GetChild(parentTreeView, 0) as FrameworkElement;
					if (frameworkElement != null)
					{
						ScrollViewer scrollViewer = frameworkElement.FindName("_tv_scrollviewer_") as ScrollViewer;
						if (scrollViewer != null)
						{
							return scrollViewer;
						}
					}
				}
				return null;
			}
		}

		// Token: 0x17000BCB RID: 3019
		// (get) Token: 0x06002652 RID: 9810 RVA: 0x000B3478 File Offset: 0x000B1678
		private BookmarkControl ParentBookmarkControl
		{
			get
			{
				FrameworkElement frameworkElement = this.ParentScrollViewer;
				while (frameworkElement != null && !(frameworkElement is BookmarkControl))
				{
					frameworkElement = (frameworkElement.Parent ?? VisualTreeHelper.GetParent(frameworkElement)) as FrameworkElement;
				}
				return frameworkElement as BookmarkControl;
			}
		}

		// Token: 0x06002653 RID: 9811
		[DllImport("user32.dll")]
		private static extern bool GetCursorPos(out BookmarkTreeViewItem._POINT lpPoint);

		// Token: 0x06002655 RID: 9813 RVA: 0x000B34CB File Offset: 0x000B16CB
		[CompilerGenerated]
		private void <OnApplyTemplate>g__SetToolTip|14_0(UIElement element, bool enable)
		{
			ToolTipService.SetToolTip(element, enable ? this.toolTipContent : null);
			ToolTipService.SetPlacement(element, PlacementMode.Mouse);
		}

		// Token: 0x06002656 RID: 9814 RVA: 0x000B34E8 File Offset: 0x000B16E8
		[CompilerGenerated]
		internal static bool <Control_ToolTipOpening>g__IsOverflow|17_0(FrameworkElement _parent, FrameworkElement _child)
		{
			Rect rect = new Rect(0.0, 0.0, _parent.ActualWidth, _parent.ActualHeight);
			Rect rect2 = _child.TransformToVisual(_parent).TransformBounds(new Rect(0.0, 0.0, _child.ActualWidth, _child.ActualHeight));
			return !rect.Contains(rect2);
		}

		// Token: 0x06002657 RID: 9815 RVA: 0x000B3558 File Offset: 0x000B1758
		[CompilerGenerated]
		internal static int <OnDrop>g__IndexOf|32_0(global::System.Collections.Generic.IReadOnlyList<BookmarkModel> _bookmarks, BookmarkModel _bookmark)
		{
			if (_bookmarks == null)
			{
				return -1;
			}
			for (int i = 0; i < _bookmarks.Count; i++)
			{
				if (_bookmarks[i] == _bookmark)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x04001073 RID: 4211
		private static WeakReference<BookmarkTreeViewItem.DragDropDataModel> draggingModel;

		// Token: 0x04001074 RID: 4212
		private ColumnDefinition MarginColumn;

		// Token: 0x04001075 RID: 4213
		private Grid LayoutRoot;

		// Token: 0x04001076 RID: 4214
		private Border BackgroundBorder;

		// Token: 0x04001077 RID: 4215
		private Border ContentBorder;

		// Token: 0x04001078 RID: 4216
		private ToggleButton Expander;

		// Token: 0x04001079 RID: 4217
		private TextBlock toolTipText;

		// Token: 0x0400107A RID: 4218
		private Border toolTipContent;

		// Token: 0x0400107B RID: 4219
		public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(BookmarkTreeViewItem), new PropertyMetadata(false));

		// Token: 0x0400107C RID: 4220
		private DateTime lastClickCollapseExpanderTime;

		// Token: 0x0400107D RID: 4221
		private Point? lastMouseDownPosition;

		// Token: 0x02000768 RID: 1896
		private struct _POINT
		{
			// Token: 0x04002584 RID: 9604
			public int X;

			// Token: 0x04002585 RID: 9605
			public int Y;
		}

		// Token: 0x02000769 RID: 1897
		private struct DragDropProcessResult
		{
			// Token: 0x04002586 RID: 9606
			public BookmarkTreeViewItem.DragDropDataModel Model;

			// Token: 0x04002587 RID: 9607
			public ScrollViewer BookmarkTreeViewScrollViewer;

			// Token: 0x04002588 RID: 9608
			public double ScrollOffset;

			// Token: 0x04002589 RID: 9609
			public Point? MousePositionForItem;

			// Token: 0x0400258A RID: 9610
			public BookmarkTreeViewItem.DragDropProcessResult.InsertPositionEnum InsertPosition;

			// Token: 0x02000860 RID: 2144
			public enum InsertPositionEnum
			{
				// Token: 0x04002A3C RID: 10812
				InsertToChildren,
				// Token: 0x04002A3D RID: 10813
				InsertAsPreviousSibling,
				// Token: 0x04002A3E RID: 10814
				InsertAsNextSibling
			}
		}

		// Token: 0x0200076A RID: 1898
		private class DragDropDataModel
		{
			// Token: 0x17000D9D RID: 3485
			// (get) Token: 0x0600370A RID: 14090 RVA: 0x00114B82 File Offset: 0x00112D82
			// (set) Token: 0x0600370B RID: 14091 RVA: 0x00114B8A File Offset: 0x00112D8A
			public BookmarkModel Bookmark { get; set; }

			// Token: 0x17000D9E RID: 3486
			// (get) Token: 0x0600370C RID: 14092 RVA: 0x00114B93 File Offset: 0x00112D93
			// (set) Token: 0x0600370D RID: 14093 RVA: 0x00114B9B File Offset: 0x00112D9B
			public Point DragStartPoint { get; set; }

			// Token: 0x17000D9F RID: 3487
			// (get) Token: 0x0600370E RID: 14094 RVA: 0x00114BA4 File Offset: 0x00112DA4
			// (set) Token: 0x0600370F RID: 14095 RVA: 0x00114BAC File Offset: 0x00112DAC
			public Size ContainerSize { get; set; }

			// Token: 0x17000DA0 RID: 3488
			// (get) Token: 0x06003710 RID: 14096 RVA: 0x00114BB5 File Offset: 0x00112DB5
			// (set) Token: 0x06003711 RID: 14097 RVA: 0x00114BBD File Offset: 0x00112DBD
			public BookmarkControl BookmarkControl { get; set; }

			// Token: 0x17000DA1 RID: 3489
			// (get) Token: 0x06003712 RID: 14098 RVA: 0x00114BC6 File Offset: 0x00112DC6
			// (set) Token: 0x06003713 RID: 14099 RVA: 0x00114BCE File Offset: 0x00112DCE
			public ScrollViewer TreeViewScrollViewer { get; set; }

			// Token: 0x17000DA2 RID: 3490
			// (get) Token: 0x06003714 RID: 14100 RVA: 0x00114BD7 File Offset: 0x00112DD7
			// (set) Token: 0x06003715 RID: 14101 RVA: 0x00114BDF File Offset: 0x00112DDF
			public bool IsTouching { get; set; }
		}
	}
}
