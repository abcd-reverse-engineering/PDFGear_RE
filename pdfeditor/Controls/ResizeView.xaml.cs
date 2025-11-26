using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace pdfeditor.Controls
{
	// Token: 0x020001CD RID: 461
	public partial class ResizeView : ContentControl
	{
		// Token: 0x06001A0A RID: 6666 RVA: 0x00067904 File Offset: 0x00065B04
		static ResizeView()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeView), new FrameworkPropertyMetadata(typeof(ResizeView)));
		}

		// Token: 0x06001A0B RID: 6667 RVA: 0x00067ADD File Offset: 0x00065CDD
		public ResizeView()
		{
			base.SizeChanged += this.ResizeView_SizeChanged;
			base.Unloaded += this.ResizeView_Unloaded;
		}

		// Token: 0x170009CF RID: 2511
		// (get) Token: 0x06001A0C RID: 6668 RVA: 0x00067B09 File Offset: 0x00065D09
		// (set) Token: 0x06001A0D RID: 6669 RVA: 0x00067B14 File Offset: 0x00065D14
		private Grid DraggerContainer
		{
			get
			{
				return this.draggerContainer;
			}
			set
			{
				if (this.draggerContainer != null)
				{
					this.draggerContainer.MouseDown -= this.DraggerContainer_MouseDown;
				}
				this.draggerContainer = value;
				if (this.draggerContainer != null)
				{
					this.draggerContainer.MouseDown += this.DraggerContainer_MouseDown;
				}
				this.draggerContainer.Width = base.ActualWidth;
				this.draggerContainer.Height = base.ActualHeight;
			}
		}

		// Token: 0x06001A0E RID: 6670 RVA: 0x00067B88 File Offset: 0x00065D88
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.MoveDragger = this.GetTemplateChild<Rectangle>("MoveDragger");
			this.LeftTopDragger = this.GetTemplateChild<Rectangle>("LeftTopDragger");
			this.CenterTopDragger = this.GetTemplateChild<Rectangle>("CenterTopDragger");
			this.RightTopDragger = this.GetTemplateChild<Rectangle>("RightTopDragger");
			this.LeftCenterDragger = this.GetTemplateChild<Rectangle>("LeftCenterDragger");
			this.RightCenterDragger = this.GetTemplateChild<Rectangle>("RightCenterDragger");
			this.LeftBottomDragger = this.GetTemplateChild<Rectangle>("LeftBottomDragger");
			this.CenterBottomDragger = this.GetTemplateChild<Rectangle>("CenterBottomDragger");
			this.RightBottomDragger = this.GetTemplateChild<Rectangle>("RightBottomDragger");
			this.DraggerContainer = this.GetTemplateChild<Grid>("DraggerContainer");
			this.DraggerContainerBorder = this.GetTemplateChild<Border>("DraggerContainerBorder");
			this.DraggerCanvas = this.GetTemplateChild<Canvas>("DraggerCanvas");
			this.UpdateDraggersEnabledState();
			this.UpdateMoveState();
			this.UpdateDraggerVisible();
			this.UpdateSizeMode();
		}

		// Token: 0x170009D0 RID: 2512
		// (get) Token: 0x06001A0F RID: 6671 RVA: 0x00067C7F File Offset: 0x00065E7F
		// (set) Token: 0x06001A10 RID: 6672 RVA: 0x00067C91 File Offset: 0x00065E91
		public ResizeViewOperation DragMode
		{
			get
			{
				return (ResizeViewOperation)base.GetValue(ResizeView.DragModeProperty);
			}
			set
			{
				base.SetValue(ResizeView.DragModeProperty, value);
			}
		}

		// Token: 0x06001A11 RID: 6673 RVA: 0x00067CA4 File Offset: 0x00065EA4
		private static void OnDragModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ResizeView resizeView = d as ResizeView;
			if (resizeView != null)
			{
				resizeView.UpdateDraggersEnabledState();
				resizeView.UpdateMoveState();
			}
		}

		// Token: 0x170009D1 RID: 2513
		// (get) Token: 0x06001A12 RID: 6674 RVA: 0x00067CC7 File Offset: 0x00065EC7
		// (set) Token: 0x06001A13 RID: 6675 RVA: 0x00067CD9 File Offset: 0x00065ED9
		public bool CanDragCross
		{
			get
			{
				return (bool)base.GetValue(ResizeView.CanDragCrossProperty);
			}
			set
			{
				base.SetValue(ResizeView.CanDragCrossProperty, value);
			}
		}

		// Token: 0x170009D2 RID: 2514
		// (get) Token: 0x06001A14 RID: 6676 RVA: 0x00067CEC File Offset: 0x00065EEC
		// (set) Token: 0x06001A15 RID: 6677 RVA: 0x00067CFE File Offset: 0x00065EFE
		public Brush DragPlaceholderFill
		{
			get
			{
				return (Brush)base.GetValue(ResizeView.DragPlaceholderFillProperty);
			}
			set
			{
				base.SetValue(ResizeView.DragPlaceholderFillProperty, value);
			}
		}

		// Token: 0x170009D3 RID: 2515
		// (get) Token: 0x06001A16 RID: 6678 RVA: 0x00067D0C File Offset: 0x00065F0C
		// (set) Token: 0x06001A17 RID: 6679 RVA: 0x00067D1E File Offset: 0x00065F1E
		public bool IsDraggerVisible
		{
			get
			{
				return (bool)base.GetValue(ResizeView.IsDraggerVisibleProperty);
			}
			set
			{
				base.SetValue(ResizeView.IsDraggerVisibleProperty, value);
			}
		}

		// Token: 0x06001A18 RID: 6680 RVA: 0x00067D34 File Offset: 0x00065F34
		private static void OnIsDraggerVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ResizeView resizeView = d as ResizeView;
			if (resizeView != null)
			{
				resizeView.UpdateDraggerVisible();
			}
		}

		// Token: 0x170009D4 RID: 2516
		// (get) Token: 0x06001A19 RID: 6681 RVA: 0x00067D51 File Offset: 0x00065F51
		// (set) Token: 0x06001A1A RID: 6682 RVA: 0x00067D5E File Offset: 0x00065F5E
		public object PlaceholderContent
		{
			get
			{
				return base.GetValue(ResizeView.PlaceholderContentProperty);
			}
			set
			{
				base.SetValue(ResizeView.PlaceholderContentProperty, value);
			}
		}

		// Token: 0x06001A1B RID: 6683 RVA: 0x00067D6C File Offset: 0x00065F6C
		private static void OnPlaceholderContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			ResizeView resizeView = d as ResizeView;
			if (resizeView != null)
			{
				DependencyObject dependencyObject = e.OldValue as DependencyObject;
				if (dependencyObject != null)
				{
					resizeView.RemoveLogicalChild(dependencyObject);
				}
				DependencyObject dependencyObject2 = e.NewValue as DependencyObject;
				if (dependencyObject2 != null)
				{
					resizeView.AddLogicalChild(dependencyObject2);
				}
			}
		}

		// Token: 0x170009D5 RID: 2517
		// (get) Token: 0x06001A1C RID: 6684 RVA: 0x00067DB1 File Offset: 0x00065FB1
		// (set) Token: 0x06001A1D RID: 6685 RVA: 0x00067DC3 File Offset: 0x00065FC3
		public ControlTemplate PlaceholderContentTemplate
		{
			get
			{
				return (ControlTemplate)base.GetValue(ResizeView.PlaceholderContentTemplateProperty);
			}
			set
			{
				base.SetValue(ResizeView.PlaceholderContentTemplateProperty, value);
			}
		}

		// Token: 0x170009D6 RID: 2518
		// (get) Token: 0x06001A1E RID: 6686 RVA: 0x00067DD1 File Offset: 0x00065FD1
		// (set) Token: 0x06001A1F RID: 6687 RVA: 0x00067DE3 File Offset: 0x00065FE3
		public bool IsCompactMode
		{
			get
			{
				return (bool)base.GetValue(ResizeView.IsCompactModeProperty);
			}
			set
			{
				base.SetValue(ResizeView.IsCompactModeProperty, value);
			}
		}

		// Token: 0x170009D7 RID: 2519
		// (get) Token: 0x06001A20 RID: 6688 RVA: 0x00067DF6 File Offset: 0x00065FF6
		// (set) Token: 0x06001A21 RID: 6689 RVA: 0x00067E08 File Offset: 0x00066008
		public bool IsProportionalScaleEnabled
		{
			get
			{
				return (bool)base.GetValue(ResizeView.IsProportionalScaleEnabledProperty);
			}
			set
			{
				base.SetValue(ResizeView.IsProportionalScaleEnabledProperty, value);
			}
		}

		// Token: 0x06001A22 RID: 6690 RVA: 0x00067E1B File Offset: 0x0006601B
		private void Dragger_MouseDown(Rectangle dragger, MouseButtonEventArgs e)
		{
			if (this.ProcessMousePressed(dragger, e))
			{
				e.Handled = true;
			}
		}

		// Token: 0x06001A23 RID: 6691 RVA: 0x00067E30 File Offset: 0x00066030
		private void Dragger_MouseMove(object sender, MouseEventArgs e)
		{
			Rectangle rectangle = e.MouseDevice.Captured as Rectangle;
			if (rectangle != null && this.ProcessMouseMove(rectangle, e))
			{
				e.Handled = true;
			}
		}

		// Token: 0x06001A24 RID: 6692 RVA: 0x00067E64 File Offset: 0x00066064
		private void Dragger_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				Rectangle rectangle = e.MouseDevice.Captured as Rectangle;
				if (rectangle != null && this.ProcessMouseRelease(rectangle, e))
				{
					e.Handled = true;
				}
			}
		}

		// Token: 0x06001A25 RID: 6693 RVA: 0x00067E9E File Offset: 0x0006609E
		private void Dragger_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
			{
				this.UpdateMouseMoveProperties();
			}
		}

		// Token: 0x06001A26 RID: 6694 RVA: 0x00067EBB File Offset: 0x000660BB
		private void Dragger_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
			{
				this.UpdateMouseMoveProperties();
			}
		}

		// Token: 0x06001A27 RID: 6695 RVA: 0x00067ED8 File Offset: 0x000660D8
		private void DraggerContainer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				Rectangle rectangle = e.OriginalSource as Rectangle;
				if (rectangle != null)
				{
					string name = rectangle.Name;
					if (name != null && name.EndsWith("Dragger") && rectangle.Parent == sender)
					{
						this.Dragger_MouseDown(rectangle, e);
					}
				}
			}
		}

		// Token: 0x06001A28 RID: 6696 RVA: 0x00067F28 File Offset: 0x00066128
		private ResizeViewOperation? GetOperation(string draggerName)
		{
			string text = draggerName;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			if (text.EndsWith("Dragger"))
			{
				text = text.Substring(0, text.Length - "Dragger".Length);
			}
			if (text != null)
			{
				switch (text.Length)
				{
				case 4:
					if (text == "Move")
					{
						return new ResizeViewOperation?(ResizeViewOperation.Move);
					}
					break;
				case 7:
					if (text == "LeftTop")
					{
						return new ResizeViewOperation?(ResizeViewOperation.LeftTop);
					}
					break;
				case 8:
					if (text == "RightTop")
					{
						return new ResizeViewOperation?(ResizeViewOperation.RightTop);
					}
					break;
				case 9:
					if (text == "CenterTop")
					{
						return new ResizeViewOperation?(ResizeViewOperation.CenterTop);
					}
					break;
				case 10:
				{
					char c = text[4];
					if (c != 'B')
					{
						if (c == 'C')
						{
							if (text == "LeftCenter")
							{
								return new ResizeViewOperation?(ResizeViewOperation.LeftCenter);
							}
						}
					}
					else if (text == "LeftBottom")
					{
						return new ResizeViewOperation?(ResizeViewOperation.LeftBottom);
					}
					break;
				}
				case 11:
				{
					char c = text[5];
					if (c != 'B')
					{
						if (c == 'C')
						{
							if (text == "RightCenter")
							{
								return new ResizeViewOperation?(ResizeViewOperation.RightCenter);
							}
						}
					}
					else if (text == "RightBottom")
					{
						return new ResizeViewOperation?(ResizeViewOperation.RightBottom);
					}
					break;
				}
				case 12:
					if (text == "CenterBottom")
					{
						return new ResizeViewOperation?(ResizeViewOperation.CenterBottom);
					}
					break;
				}
			}
			return null;
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x000680C8 File Offset: 0x000662C8
		private void UpdateDragDataContext(Rectangle dragger, MouseEventArgs args, bool isDragging)
		{
			Window window = this.curWindow;
			if (((window != null) ? window.Content : null) == null)
			{
				return;
			}
			Interlocked.MemoryBarrier();
			ResizeView.DragDataContext? dragDataContext = this.dragDataContext;
			if (isDragging && dragDataContext == null)
			{
				return;
			}
			Point position = args.GetPosition((IInputElement)this.curWindow.Content);
			Size size = new Size(base.ActualWidth, base.ActualHeight);
			ResizeViewOperation? operation = this.GetOperation((dragger != null) ? dragger.Name : null);
			this.dragDataContext = new ResizeView.DragDataContext?(new ResizeView.DragDataContext
			{
				Operation = operation,
				CanXCross = (this.CanDragCross && base.MinWidth == 0.0),
				CanYCross = (this.CanDragCross && base.MinHeight == 0.0),
				StartPoint = (isDragging ? dragDataContext.Value.StartPoint : position),
				StartSize = (isDragging ? dragDataContext.Value.StartSize : size),
				CurrentPoint = position
			});
			Interlocked.MemoryBarrier();
		}

		// Token: 0x06001A2A RID: 6698 RVA: 0x000681E4 File Offset: 0x000663E4
		private bool ProcessMousePressed(Rectangle dragger, MouseEventArgs args)
		{
			if (this.GetOperation((dragger != null) ? dragger.Name : null) == null)
			{
				return false;
			}
			TextBoxBase textBoxBase = Keyboard.FocusedElement as TextBoxBase;
			if (textBoxBase != null)
			{
				TraversalRequest traversalRequest = new TraversalRequest(FocusNavigationDirection.Next);
				if (!textBoxBase.MoveFocus(traversalRequest))
				{
					Keyboard.ClearFocus();
				}
			}
			dragger.CaptureMouse();
			if (this.curWindow != null)
			{
				this.curWindow.MouseMove -= this.Dragger_MouseMove;
				this.curWindow.MouseLeftButtonUp -= this.Dragger_MouseLeftButtonUp;
				this.curWindow.PreviewKeyDown -= this.Dragger_PreviewKeyDown;
				this.curWindow.PreviewKeyUp -= this.Dragger_PreviewKeyUp;
			}
			this.curWindow = Window.GetWindow(this);
			if (this.curWindow != null)
			{
				this.curWindow.MouseMove += this.Dragger_MouseMove;
				this.curWindow.MouseLeftButtonUp += this.Dragger_MouseLeftButtonUp;
				this.curWindow.PreviewKeyDown += this.Dragger_PreviewKeyDown;
				this.curWindow.PreviewKeyUp += this.Dragger_PreviewKeyUp;
				this.UpdateDragDataContext(null, args, false);
				return true;
			}
			return false;
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x0006831C File Offset: 0x0006651C
		private bool UpdateMouseMoveProperties()
		{
			ResizeView.DragDataContext? dragDataContext = this.dragDataContext;
			if (dragDataContext != null)
			{
				ResizeView.DragDataContext dragDataContext2 = dragDataContext.Value;
				if (dragDataContext2.Operation != null && this.dragging)
				{
					Rect rect = this.ProcessDragOperation(dragDataContext.Value);
					if (!rect.IsEmpty)
					{
						this.DraggerContainer.Width = rect.Width;
						this.DraggerContainer.Height = rect.Height;
						Canvas.SetLeft(this.DraggerContainerBorder, rect.Left);
						Canvas.SetTop(this.DraggerContainerBorder, rect.Top);
					}
					if (this.dragging || (!rect.IsEmpty && (rect.X != 0.0 || rect.Y != 0.0 || rect.Size != dragDataContext.Value.StartSize)))
					{
						Size startSize = dragDataContext.Value.StartSize;
						Size size = (rect.IsEmpty ? new Size(base.ActualWidth, base.ActualHeight) : new Size(rect.Width, rect.Height));
						double num = (rect.IsEmpty ? 0.0 : rect.Left);
						double num2 = (rect.IsEmpty ? 0.0 : rect.Top);
						dragDataContext2 = dragDataContext.Value;
						ResizeViewResizeDragEventArgs resizeViewResizeDragEventArgs = new ResizeViewResizeDragEventArgs(startSize, size, num, num2, dragDataContext2.Operation.Value);
						EventHandler<ResizeViewResizeDragEventArgs> resizeDragging = this.ResizeDragging;
						if (resizeDragging != null)
						{
							resizeDragging(this, resizeViewResizeDragEventArgs);
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001A2C RID: 6700 RVA: 0x000684B4 File Offset: 0x000666B4
		private bool ProcessMouseMove(Rectangle dragger, MouseEventArgs args)
		{
			this.UpdateDragDataContext(dragger, args, true);
			ResizeView.DragDataContext? dragDataContext = this.dragDataContext;
			if (dragDataContext != null)
			{
				ResizeView.DragDataContext dragDataContext2 = dragDataContext.Value;
				if (dragDataContext2.Operation != null)
				{
					if (!this.dragging)
					{
						this.dragging = true;
						dragDataContext2 = dragDataContext.Value;
						VisualStateManager.GoToState(this, (dragDataContext2.Operation.GetValueOrDefault() == ResizeViewOperation.Move) ? "DragMoving" : "Dragging", true);
						base.Cursor = dragger.Cursor;
						EventHandler<ResizeViewResizeDragStartedEventArgs> resizeDragStarted = this.ResizeDragStarted;
						if (resizeDragStarted != null)
						{
							dragDataContext2 = dragDataContext.Value;
							resizeDragStarted(this, new ResizeViewResizeDragStartedEventArgs(dragDataContext2.Operation.Value));
						}
					}
					return this.UpdateMouseMoveProperties();
				}
			}
			return false;
		}

		// Token: 0x06001A2D RID: 6701 RVA: 0x0006856C File Offset: 0x0006676C
		private bool ProcessMouseRelease(Rectangle dragger, MouseEventArgs args)
		{
			if (this.curWindow == null)
			{
				return false;
			}
			this.curWindow.MouseMove -= this.Dragger_MouseMove;
			this.curWindow.MouseLeftButtonUp -= this.Dragger_MouseLeftButtonUp;
			base.Cursor = null;
			Mouse.Captured.ReleaseMouseCapture();
			this.dragging = false;
			VisualStateManager.GoToState(this, "DragCompleted", true);
			this.UpdateDragDataContext(dragger, args, true);
			ResizeView.DragDataContext? dragDataContext = this.dragDataContext;
			if (dragDataContext != null)
			{
				ResizeView.DragDataContext dragDataContext2 = dragDataContext.Value;
				if (dragDataContext2.Operation != null)
				{
					Rect rect = this.ProcessDragOperation(dragDataContext.Value);
					this.DraggerContainer.Width = base.ActualWidth;
					this.DraggerContainer.Height = base.ActualHeight;
					Canvas.SetLeft(this.DraggerContainerBorder, 0.0);
					Canvas.SetTop(this.DraggerContainerBorder, 0.0);
					if (this.dragging || (!rect.IsEmpty && (rect.X != 0.0 || rect.Y != 0.0 || rect.Size != dragDataContext.Value.StartSize)))
					{
						Size startSize = dragDataContext.Value.StartSize;
						Size size = (rect.IsEmpty ? new Size(base.ActualWidth, base.ActualHeight) : new Size(rect.Width, rect.Height));
						double num = (rect.IsEmpty ? 0.0 : rect.Left);
						double num2 = (rect.IsEmpty ? 0.0 : rect.Top);
						dragDataContext2 = dragDataContext.Value;
						ResizeViewResizeDragEventArgs resizeViewResizeDragEventArgs = new ResizeViewResizeDragEventArgs(startSize, size, num, num2, dragDataContext2.Operation.Value);
						EventHandler<ResizeViewResizeDragEventArgs> resizeDragCompleted = this.ResizeDragCompleted;
						if (resizeDragCompleted != null)
						{
							resizeDragCompleted(this, resizeViewResizeDragEventArgs);
						}
					}
					this.dragDataContext = null;
					this.curWindow = null;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001A2E RID: 6702 RVA: 0x0006876C File Offset: 0x0006696C
		private void ResizeView_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (this.DraggerContainer != null)
			{
				this.DraggerContainer.Width = e.NewSize.Width;
				this.DraggerContainer.Height = e.NewSize.Height;
			}
		}

		// Token: 0x06001A2F RID: 6703 RVA: 0x000687B4 File Offset: 0x000669B4
		private void ResizeView_Unloaded(object sender, RoutedEventArgs e)
		{
			if (this.curWindow != null)
			{
				this.curWindow.MouseMove -= this.Dragger_MouseMove;
				this.curWindow.MouseLeftButtonUp -= this.Dragger_MouseLeftButtonUp;
			}
			base.Cursor = null;
			Rectangle rectangle = Mouse.Captured as Rectangle;
			if (rectangle != null)
			{
				string name = rectangle.Name;
				if (name != null && name.EndsWith("Dragger") && rectangle.Parent == this.DraggerContainer)
				{
					rectangle.ReleaseMouseCapture();
				}
			}
			this.dragging = false;
			VisualStateManager.GoToState(this, "DragCompleted", true);
		}

		// Token: 0x06001A30 RID: 6704 RVA: 0x00068850 File Offset: 0x00066A50
		private Rect ProcessDragOperation(ResizeView.DragDataContext context)
		{
			Rect rect = default(Rect);
			ResizeViewOperation? operation = context.Operation;
			if (operation != null)
			{
				ResizeViewOperation valueOrDefault = operation.GetValueOrDefault();
				if (valueOrDefault <= ResizeViewOperation.LeftCenter)
				{
					switch (valueOrDefault)
					{
					case ResizeViewOperation.Move:
						rect = this.ProcessDragMove(context);
						break;
					case ResizeViewOperation.LeftTop:
						rect = this.ProcessDragLeftTop(context);
						break;
					case ResizeViewOperation.Move | ResizeViewOperation.LeftTop:
						break;
					case ResizeViewOperation.CenterTop:
						rect = this.ProcessDragCenterTop(context);
						break;
					default:
						if (valueOrDefault != ResizeViewOperation.RightTop)
						{
							if (valueOrDefault == ResizeViewOperation.LeftCenter)
							{
								rect = this.ProcessDragLeftCenter(context);
							}
						}
						else
						{
							rect = this.ProcessDragRightTop(context);
						}
						break;
					}
				}
				else if (valueOrDefault <= ResizeViewOperation.LeftBottom)
				{
					if (valueOrDefault != ResizeViewOperation.RightCenter)
					{
						if (valueOrDefault == ResizeViewOperation.LeftBottom)
						{
							rect = this.ProcessDragLeftBottom(context);
						}
					}
					else
					{
						rect = this.ProcessDragRightCenter(context);
					}
				}
				else if (valueOrDefault != ResizeViewOperation.CenterBottom)
				{
					if (valueOrDefault == ResizeViewOperation.RightBottom)
					{
						rect = this.ProcessDragRightBottom(context);
					}
				}
				else
				{
					rect = this.ProcessDragCenterBottom(context);
				}
			}
			return rect;
		}

		// Token: 0x170009D8 RID: 2520
		// (get) Token: 0x06001A31 RID: 6705 RVA: 0x00068924 File Offset: 0x00066B24
		internal static bool IsShiftPressedInternal
		{
			get
			{
				return (Keyboard.GetKeyStates(Key.LeftShift) & KeyStates.Down) > KeyStates.None || (Keyboard.GetKeyStates(Key.RightShift) & KeyStates.Down) > KeyStates.None;
			}
		}

		// Token: 0x06001A32 RID: 6706 RVA: 0x00068940 File Offset: 0x00066B40
		private void ProportionalScaleSize(ResizeView.DragDataContext context, ref double width, ref double height)
		{
			double num = 1.0;
			if (context.StartSize.Width != 0.0 && context.StartSize.Height != 0.0)
			{
				num = context.StartSize.Width / context.StartSize.Height;
			}
			double num2 = Math.Max(width, base.MinWidth) / num;
			if (num2 >= base.MinHeight)
			{
				height = num2;
				width = Math.Max(width, base.MinWidth);
				return;
			}
			height = base.MinHeight;
			width = base.MinHeight * num;
		}

		// Token: 0x06001A33 RID: 6707 RVA: 0x000689E0 File Offset: 0x00066BE0
		private Rect ProcessDragMove(ResizeView.DragDataContext context)
		{
			double num = context.CurrentPoint.X - context.StartPoint.X;
			double num2 = context.CurrentPoint.Y - context.StartPoint.Y;
			return new Rect(num, num2, context.StartSize.Width, context.StartSize.Height);
		}

		// Token: 0x06001A34 RID: 6708 RVA: 0x00068A40 File Offset: 0x00066C40
		private Rect ProcessDragLeftTop(ResizeView.DragDataContext context)
		{
			double num = context.CurrentPoint.X - context.StartPoint.X;
			double num2 = context.CurrentPoint.Y - context.StartPoint.Y;
			double num3 = context.StartSize.Width - num;
			double num4 = context.StartSize.Height - num2;
			bool flag = this.IsProportionalScaleEnabled && ResizeView.IsShiftPressedInternal;
			bool flag2 = context.CanXCross && context.CanYCross;
			if (flag2)
			{
				flag2 = (num3 < 0.0 && num4 < 0.0) || (num3 < 0.0 && flag);
			}
			if (flag2)
			{
				ResizeView.DragDataContext dragDataContext = context;
				dragDataContext.StartPoint.X = dragDataContext.StartPoint.X + dragDataContext.StartSize.Width;
				dragDataContext.StartPoint.Y = dragDataContext.StartPoint.Y + dragDataContext.StartSize.Height;
				dragDataContext.CurrentPoint.X = dragDataContext.CurrentPoint.X - dragDataContext.StartSize.Width;
				dragDataContext.CurrentPoint.Y = dragDataContext.CurrentPoint.Y - dragDataContext.StartSize.Height;
				Rect rect = this.ProcessDragRightBottom(dragDataContext);
				rect.X += dragDataContext.StartSize.Width;
				rect.Y += dragDataContext.StartSize.Height;
				return rect;
			}
			if (num3 < 0.0 && !flag)
			{
				if (context.CanXCross)
				{
					ResizeView.DragDataContext dragDataContext2 = context;
					dragDataContext2.StartPoint.X = dragDataContext2.StartPoint.X + dragDataContext2.StartSize.Width;
					dragDataContext2.CurrentPoint.X = dragDataContext2.CurrentPoint.X - dragDataContext2.StartSize.Width;
					Rect rect2 = this.ProcessDragRightTop(dragDataContext2);
					rect2.X += dragDataContext2.StartSize.Width;
					return rect2;
				}
				num3 = base.MinWidth;
			}
			else if (num4 < 0.0 && !flag)
			{
				if (context.CanYCross)
				{
					ResizeView.DragDataContext dragDataContext3 = context;
					dragDataContext3.StartPoint.Y = dragDataContext3.StartPoint.Y + dragDataContext3.StartSize.Height;
					dragDataContext3.CurrentPoint.Y = dragDataContext3.CurrentPoint.Y - dragDataContext3.StartSize.Height;
					Rect rect3 = this.ProcessDragLeftBottom(dragDataContext3);
					rect3.Y += dragDataContext3.StartSize.Height;
					return rect3;
				}
				num4 = base.MinHeight;
			}
			if (flag)
			{
				this.ProportionalScaleSize(context, ref num3, ref num4);
			}
			else
			{
				if (num3 < base.MinWidth)
				{
					num3 = base.MinWidth;
				}
				if (num4 < base.MinHeight)
				{
					num4 = base.MinHeight;
				}
			}
			num = context.StartSize.Width - num3;
			num2 = context.StartSize.Height - num4;
			return new Rect(num, num2, num3, num4);
		}

		// Token: 0x06001A35 RID: 6709 RVA: 0x00068D2C File Offset: 0x00066F2C
		private Rect ProcessDragCenterTop(ResizeView.DragDataContext context)
		{
			int num = 0;
			double num2 = context.CurrentPoint.Y - context.StartPoint.Y;
			double width = context.StartSize.Width;
			double num3 = context.StartSize.Height - num2;
			if (context.CanYCross && num3 < 0.0)
			{
				ResizeView.DragDataContext dragDataContext = context;
				dragDataContext.StartPoint.Y = dragDataContext.StartPoint.Y + dragDataContext.StartSize.Height;
				dragDataContext.CurrentPoint.Y = dragDataContext.CurrentPoint.Y - dragDataContext.StartSize.Height;
				Rect rect = this.ProcessDragCenterBottom(dragDataContext);
				rect.Y += dragDataContext.StartSize.Height;
				return rect;
			}
			if (num3 < base.MinHeight)
			{
				num3 = base.MinHeight;
			}
			num2 = context.StartSize.Height - num3;
			return new Rect((double)num, num2, width, num3);
		}

		// Token: 0x06001A36 RID: 6710 RVA: 0x00068E18 File Offset: 0x00067018
		private Rect ProcessDragRightTop(ResizeView.DragDataContext context)
		{
			double num = 0.0;
			double num2 = context.CurrentPoint.Y - context.StartPoint.Y;
			double num3 = context.StartSize.Width + (context.CurrentPoint.X - context.StartPoint.X);
			double num4 = context.StartSize.Height - num2;
			bool flag = this.IsProportionalScaleEnabled && ResizeView.IsShiftPressedInternal;
			bool flag2 = context.CanXCross && context.CanYCross;
			if (flag2)
			{
				flag2 = (num3 < 0.0 && num4 < 0.0) || (num3 < 0.0 && flag);
			}
			if (flag2)
			{
				ResizeView.DragDataContext dragDataContext = context;
				dragDataContext.StartPoint.X = dragDataContext.StartPoint.X - dragDataContext.StartSize.Width;
				dragDataContext.StartPoint.Y = dragDataContext.StartPoint.Y + dragDataContext.StartSize.Height;
				dragDataContext.CurrentPoint.X = dragDataContext.CurrentPoint.X + dragDataContext.StartSize.Width;
				dragDataContext.CurrentPoint.Y = dragDataContext.CurrentPoint.Y - dragDataContext.StartSize.Height;
				Rect rect = this.ProcessDragLeftBottom(dragDataContext);
				rect.X -= dragDataContext.StartSize.Width;
				rect.Y += dragDataContext.StartSize.Height;
				return rect;
			}
			if (num3 < 0.0 && !flag)
			{
				if (context.CanXCross)
				{
					ResizeView.DragDataContext dragDataContext2 = context;
					dragDataContext2.StartPoint.X = dragDataContext2.StartPoint.X - dragDataContext2.StartSize.Width;
					dragDataContext2.CurrentPoint.X = dragDataContext2.CurrentPoint.X + dragDataContext2.StartSize.Width;
					Rect rect2 = this.ProcessDragLeftTop(dragDataContext2);
					rect2.X -= dragDataContext2.StartSize.Width;
					return rect2;
				}
				num3 = base.MinWidth;
			}
			else if (num4 < 0.0 && !flag)
			{
				if (context.CanYCross)
				{
					ResizeView.DragDataContext dragDataContext3 = context;
					dragDataContext3.StartPoint.Y = dragDataContext3.StartPoint.Y + dragDataContext3.StartSize.Height;
					dragDataContext3.CurrentPoint.Y = dragDataContext3.CurrentPoint.Y - dragDataContext3.StartSize.Height;
					Rect rect3 = this.ProcessDragRightBottom(dragDataContext3);
					rect3.Y += dragDataContext3.StartSize.Height;
					return rect3;
				}
				num4 = base.MinHeight;
			}
			if (flag)
			{
				this.ProportionalScaleSize(context, ref num3, ref num4);
			}
			else
			{
				if (num3 < base.MinWidth)
				{
					num3 = base.MinWidth;
				}
				if (num4 < base.MinHeight)
				{
					num4 = base.MinHeight;
				}
			}
			num2 = context.StartSize.Height - num4;
			return new Rect(num, num2, num3, num4);
		}

		// Token: 0x06001A37 RID: 6711 RVA: 0x000690FC File Offset: 0x000672FC
		private Rect ProcessDragLeftCenter(ResizeView.DragDataContext context)
		{
			double num = context.CurrentPoint.X - context.StartPoint.X;
			int num2 = 0;
			double num3 = context.StartSize.Width - num;
			double height = context.StartSize.Height;
			if (context.CanXCross && num3 < 0.0)
			{
				ResizeView.DragDataContext dragDataContext = context;
				dragDataContext.StartPoint.X = dragDataContext.StartPoint.X + dragDataContext.StartSize.Width;
				dragDataContext.CurrentPoint.X = dragDataContext.CurrentPoint.X - dragDataContext.StartSize.Width;
				Rect rect = this.ProcessDragRightCenter(dragDataContext);
				rect.X += dragDataContext.StartSize.Width;
				return rect;
			}
			if (num3 < base.MinWidth)
			{
				num3 = base.MinWidth;
			}
			num = context.StartSize.Width - num3;
			return new Rect(num, (double)num2, num3, height);
		}

		// Token: 0x06001A38 RID: 6712 RVA: 0x000691E8 File Offset: 0x000673E8
		private Rect ProcessDragRightCenter(ResizeView.DragDataContext context)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = context.StartSize.Width + (context.CurrentPoint.X - context.StartPoint.X);
			double height = context.StartSize.Height;
			if (context.CanXCross && num3 < 0.0)
			{
				ResizeView.DragDataContext dragDataContext = context;
				dragDataContext.StartPoint.X = dragDataContext.StartPoint.X - dragDataContext.StartSize.Width;
				dragDataContext.CurrentPoint.X = dragDataContext.CurrentPoint.X + dragDataContext.StartSize.Width;
				Rect rect = this.ProcessDragLeftCenter(dragDataContext);
				rect.X -= dragDataContext.StartSize.Width;
				return rect;
			}
			if (num3 < base.MinWidth)
			{
				num3 = base.MinWidth;
			}
			return new Rect(num, num2, num3, height);
		}

		// Token: 0x06001A39 RID: 6713 RVA: 0x000692D4 File Offset: 0x000674D4
		private Rect ProcessDragLeftBottom(ResizeView.DragDataContext context)
		{
			double num = context.CurrentPoint.X - context.StartPoint.X;
			double num2 = 0.0;
			double num3 = context.StartSize.Width - num;
			double num4 = context.StartSize.Height + (context.CurrentPoint.Y - context.StartPoint.Y);
			bool flag = this.IsProportionalScaleEnabled && ResizeView.IsShiftPressedInternal;
			bool flag2 = context.CanXCross && context.CanYCross;
			if (flag2)
			{
				flag2 = (num3 < 0.0 && num4 < 0.0) || (num3 < 0.0 && flag);
			}
			if (flag2)
			{
				ResizeView.DragDataContext dragDataContext = context;
				dragDataContext.StartPoint.X = dragDataContext.StartPoint.X + dragDataContext.StartSize.Width;
				dragDataContext.StartPoint.Y = dragDataContext.StartPoint.Y - dragDataContext.StartSize.Height;
				dragDataContext.CurrentPoint.X = dragDataContext.CurrentPoint.X - dragDataContext.StartSize.Width;
				dragDataContext.CurrentPoint.Y = dragDataContext.CurrentPoint.Y + dragDataContext.StartSize.Height;
				Rect rect = this.ProcessDragRightTop(dragDataContext);
				rect.X += dragDataContext.StartSize.Width;
				rect.Y -= dragDataContext.StartSize.Height;
				return rect;
			}
			if (num3 < 0.0 && !flag)
			{
				if (context.CanXCross)
				{
					ResizeView.DragDataContext dragDataContext2 = context;
					dragDataContext2.StartPoint.X = dragDataContext2.StartPoint.X + dragDataContext2.StartSize.Width;
					dragDataContext2.CurrentPoint.X = dragDataContext2.CurrentPoint.X - dragDataContext2.StartSize.Width;
					Rect rect2 = this.ProcessDragRightBottom(dragDataContext2);
					rect2.X += dragDataContext2.StartSize.Width;
					return rect2;
				}
				num3 = base.MinWidth;
			}
			else if (num4 < 0.0 && !flag)
			{
				if (context.CanYCross)
				{
					ResizeView.DragDataContext dragDataContext3 = context;
					dragDataContext3.StartPoint.Y = dragDataContext3.StartPoint.Y - dragDataContext3.StartSize.Height;
					dragDataContext3.CurrentPoint.Y = dragDataContext3.CurrentPoint.Y + dragDataContext3.StartSize.Height;
					Rect rect3 = this.ProcessDragLeftTop(dragDataContext3);
					rect3.Y -= dragDataContext3.StartSize.Height;
					return rect3;
				}
				num4 = base.MinHeight;
			}
			if (flag)
			{
				this.ProportionalScaleSize(context, ref num3, ref num4);
			}
			else
			{
				if (num3 < base.MinWidth)
				{
					num3 = base.MinWidth;
				}
				if (num4 < base.MinHeight)
				{
					num4 = base.MinHeight;
				}
			}
			num = context.StartSize.Width - num3;
			return new Rect(num, num2, num3, num4);
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x000695B8 File Offset: 0x000677B8
		private Rect ProcessDragCenterBottom(ResizeView.DragDataContext context)
		{
			double num = 0.0;
			double num2 = 0.0;
			double width = context.StartSize.Width;
			double num3 = context.StartSize.Height + (context.CurrentPoint.Y - context.StartPoint.Y);
			if (context.CanYCross && num3 < 0.0)
			{
				ResizeView.DragDataContext dragDataContext = context;
				dragDataContext.StartPoint.Y = dragDataContext.StartPoint.Y - dragDataContext.StartSize.Height;
				dragDataContext.CurrentPoint.Y = dragDataContext.CurrentPoint.Y + dragDataContext.StartSize.Height;
				Rect rect = this.ProcessDragCenterTop(dragDataContext);
				rect.Y -= dragDataContext.StartSize.Height;
				return rect;
			}
			if (num3 < base.MinHeight)
			{
				num3 = base.MinHeight;
			}
			return new Rect(num, num2, width, num3);
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x000696A4 File Offset: 0x000678A4
		private Rect ProcessDragRightBottom(ResizeView.DragDataContext context)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = context.StartSize.Width + (context.CurrentPoint.X - context.StartPoint.X);
			double num4 = context.StartSize.Height + (context.CurrentPoint.Y - context.StartPoint.Y);
			bool flag = this.IsProportionalScaleEnabled && ResizeView.IsShiftPressedInternal;
			bool flag2 = context.CanXCross && context.CanYCross;
			if (flag2)
			{
				flag2 = (num3 < 0.0 && num4 < 0.0) || (num3 < 0.0 && flag);
			}
			if (flag2)
			{
				ResizeView.DragDataContext dragDataContext = context;
				dragDataContext.StartPoint.X = dragDataContext.StartPoint.X - dragDataContext.StartSize.Width;
				dragDataContext.StartPoint.Y = dragDataContext.StartPoint.Y - dragDataContext.StartSize.Height;
				dragDataContext.CurrentPoint.X = dragDataContext.CurrentPoint.X + dragDataContext.StartSize.Width;
				dragDataContext.CurrentPoint.Y = dragDataContext.CurrentPoint.Y + dragDataContext.StartSize.Height;
				Rect rect = this.ProcessDragLeftTop(dragDataContext);
				rect.X -= dragDataContext.StartSize.Width;
				rect.Y -= dragDataContext.StartSize.Height;
				return rect;
			}
			if (num3 < 0.0 && !flag)
			{
				if (context.CanXCross)
				{
					ResizeView.DragDataContext dragDataContext2 = context;
					dragDataContext2.StartPoint.X = dragDataContext2.StartPoint.X - dragDataContext2.StartSize.Width;
					dragDataContext2.CurrentPoint.X = dragDataContext2.CurrentPoint.X + dragDataContext2.StartSize.Width;
					Rect rect2 = this.ProcessDragLeftBottom(dragDataContext2);
					rect2.X -= dragDataContext2.StartSize.Width;
					return rect2;
				}
				num3 = base.MinWidth;
			}
			else if (num4 < 0.0 && !flag)
			{
				if (context.CanYCross)
				{
					ResizeView.DragDataContext dragDataContext3 = context;
					dragDataContext3.StartPoint.Y = dragDataContext3.StartPoint.Y - dragDataContext3.StartSize.Height;
					dragDataContext3.CurrentPoint.Y = dragDataContext3.CurrentPoint.Y + dragDataContext3.StartSize.Height;
					Rect rect3 = this.ProcessDragRightTop(dragDataContext3);
					rect3.Y -= dragDataContext3.StartSize.Height;
					return rect3;
				}
				num4 = base.MinHeight;
			}
			if (flag)
			{
				this.ProportionalScaleSize(context, ref num3, ref num4);
			}
			else
			{
				if (num3 < base.MinWidth)
				{
					num3 = base.MinWidth;
				}
				if (num4 < base.MinHeight)
				{
					num4 = base.MinHeight;
				}
			}
			return new Rect(num, num2, num3, num4);
		}

		// Token: 0x06001A3C RID: 6716 RVA: 0x00069980 File Offset: 0x00067B80
		private void UpdateDraggersEnabledState()
		{
			ResizeViewOperation dragMode = this.DragMode;
			this.<UpdateDraggersEnabledState>g__UpdateVisibility|83_0(this.LeftTopDragger, dragMode);
			this.<UpdateDraggersEnabledState>g__UpdateVisibility|83_0(this.CenterTopDragger, dragMode);
			this.<UpdateDraggersEnabledState>g__UpdateVisibility|83_0(this.RightTopDragger, dragMode);
			this.<UpdateDraggersEnabledState>g__UpdateVisibility|83_0(this.LeftCenterDragger, dragMode);
			this.<UpdateDraggersEnabledState>g__UpdateVisibility|83_0(this.RightCenterDragger, dragMode);
			this.<UpdateDraggersEnabledState>g__UpdateVisibility|83_0(this.LeftBottomDragger, dragMode);
			this.<UpdateDraggersEnabledState>g__UpdateVisibility|83_0(this.CenterBottomDragger, dragMode);
			this.<UpdateDraggersEnabledState>g__UpdateVisibility|83_0(this.RightBottomDragger, dragMode);
		}

		// Token: 0x06001A3D RID: 6717 RVA: 0x000699FC File Offset: 0x00067BFC
		private void UpdateMoveState()
		{
			if ((this.DragMode & ResizeViewOperation.Move) > ResizeViewOperation.None)
			{
				VisualStateManager.GoToState(this, "DragMoveEnabled", true);
				return;
			}
			VisualStateManager.GoToState(this, "DragMoveDisabled", true);
		}

		// Token: 0x06001A3E RID: 6718 RVA: 0x00069A24 File Offset: 0x00067C24
		private void UpdateDraggerVisible()
		{
			VisualStateManager.GoToState(this, this.IsDraggerVisible ? "IsDraggerVisible" : "IsDraggerNotVisible", true);
		}

		// Token: 0x06001A3F RID: 6719 RVA: 0x00069A42 File Offset: 0x00067C42
		private void UpdateSizeMode()
		{
			VisualStateManager.GoToState(this, this.IsCompactMode ? "CompactSize" : "NormalSize", true);
		}

		// Token: 0x06001A40 RID: 6720 RVA: 0x00069A60 File Offset: 0x00067C60
		private T GetTemplateChild<T>(string name) where T : DependencyObject
		{
			return base.GetTemplateChild(name) as T;
		}

		// Token: 0x14000032 RID: 50
		// (add) Token: 0x06001A41 RID: 6721 RVA: 0x00069A74 File Offset: 0x00067C74
		// (remove) Token: 0x06001A42 RID: 6722 RVA: 0x00069AAC File Offset: 0x00067CAC
		public event EventHandler<ResizeViewResizeDragStartedEventArgs> ResizeDragStarted;

		// Token: 0x14000033 RID: 51
		// (add) Token: 0x06001A43 RID: 6723 RVA: 0x00069AE4 File Offset: 0x00067CE4
		// (remove) Token: 0x06001A44 RID: 6724 RVA: 0x00069B1C File Offset: 0x00067D1C
		public event EventHandler<ResizeViewResizeDragEventArgs> ResizeDragging;

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x06001A45 RID: 6725 RVA: 0x00069B54 File Offset: 0x00067D54
		// (remove) Token: 0x06001A46 RID: 6726 RVA: 0x00069B8C File Offset: 0x00067D8C
		public event EventHandler<ResizeViewResizeDragEventArgs> ResizeDragCompleted;

		// Token: 0x06001A47 RID: 6727 RVA: 0x00069BC4 File Offset: 0x00067DC4
		public bool StartDrag(ResizeViewOperation operation, MouseEventArgs args)
		{
			if (args.LeftButton == MouseButtonState.Released)
			{
				return false;
			}
			if ((this.DragMode & operation) == ResizeViewOperation.None)
			{
				throw new ArgumentException("operation");
			}
			Rectangle rectangle;
			if (operation <= ResizeViewOperation.LeftCenter)
			{
				switch (operation)
				{
				case ResizeViewOperation.Move:
					rectangle = this.MoveDragger;
					goto IL_00C8;
				case ResizeViewOperation.LeftTop:
					rectangle = this.LeftTopDragger;
					goto IL_00C8;
				case ResizeViewOperation.Move | ResizeViewOperation.LeftTop:
					break;
				case ResizeViewOperation.CenterTop:
					rectangle = this.CenterTopDragger;
					goto IL_00C8;
				default:
					if (operation == ResizeViewOperation.RightTop)
					{
						rectangle = this.RightTopDragger;
						goto IL_00C8;
					}
					if (operation == ResizeViewOperation.LeftCenter)
					{
						rectangle = this.LeftCenterDragger;
						goto IL_00C8;
					}
					break;
				}
			}
			else if (operation <= ResizeViewOperation.LeftBottom)
			{
				if (operation == ResizeViewOperation.RightCenter)
				{
					rectangle = this.RightCenterDragger;
					goto IL_00C8;
				}
				if (operation == ResizeViewOperation.LeftBottom)
				{
					rectangle = this.LeftBottomDragger;
					goto IL_00C8;
				}
			}
			else
			{
				if (operation == ResizeViewOperation.CenterBottom)
				{
					rectangle = this.CenterBottomDragger;
					goto IL_00C8;
				}
				if (operation == ResizeViewOperation.RightBottom)
				{
					rectangle = this.RightBottomDragger;
					goto IL_00C8;
				}
			}
			throw new ArgumentException("operation");
			IL_00C8:
			return rectangle != null && this.ProcessMousePressed(rectangle, args);
		}

		// Token: 0x06001A48 RID: 6728 RVA: 0x00069CA6 File Offset: 0x00067EA6
		public void AddUIElementToCanvas(UIElement element)
		{
			this.DraggerCanvas.Children.Add(element);
		}

		// Token: 0x06001A49 RID: 6729 RVA: 0x00069CBA File Offset: 0x00067EBA
		public void ClearDrawUIElementOfCanvas()
		{
			if (this.DraggerCanvas.Children.Count > 1)
			{
				this.DraggerCanvas.Children.RemoveRange(1, this.DraggerCanvas.Children.Count - 1);
			}
		}

		// Token: 0x06001A4A RID: 6730 RVA: 0x00069CF2 File Offset: 0x00067EF2
		public Canvas GetDraggerCanvas()
		{
			return this.DraggerCanvas;
		}

		// Token: 0x06001A4B RID: 6731 RVA: 0x00069CFA File Offset: 0x00067EFA
		public void RemoveDrawControl(UIElement element)
		{
			if (element != null && this.DraggerCanvas.Children.Contains(element))
			{
				this.DraggerCanvas.Children.Remove(element);
			}
		}

		// Token: 0x06001A4C RID: 6732 RVA: 0x00069D24 File Offset: 0x00067F24
		[CompilerGenerated]
		private void <UpdateDraggersEnabledState>g__UpdateVisibility|83_0(Rectangle dragger, ResizeViewOperation source)
		{
			if (dragger == null)
			{
				return;
			}
			ResizeViewOperation? operation = this.GetOperation(dragger.Name);
			if (operation != null)
			{
				dragger.Visibility = ResizeView.<UpdateDraggersEnabledState>g__ToVisibility|83_1(source, operation.Value);
			}
		}

		// Token: 0x06001A4D RID: 6733 RVA: 0x00069D5E File Offset: 0x00067F5E
		[CompilerGenerated]
		internal static Visibility <UpdateDraggersEnabledState>g__ToVisibility|83_1(ResizeViewOperation source, ResizeViewOperation flag)
		{
			if (!source.HasFlag(flag))
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		// Token: 0x040008FE RID: 2302
		private Grid draggerContainer;

		// Token: 0x040008FF RID: 2303
		private Canvas DraggerCanvas;

		// Token: 0x04000900 RID: 2304
		private Rectangle MoveDragger;

		// Token: 0x04000901 RID: 2305
		private Rectangle LeftTopDragger;

		// Token: 0x04000902 RID: 2306
		private Rectangle CenterTopDragger;

		// Token: 0x04000903 RID: 2307
		private Rectangle RightTopDragger;

		// Token: 0x04000904 RID: 2308
		private Rectangle LeftCenterDragger;

		// Token: 0x04000905 RID: 2309
		private Rectangle RightCenterDragger;

		// Token: 0x04000906 RID: 2310
		private Rectangle LeftBottomDragger;

		// Token: 0x04000907 RID: 2311
		private Rectangle CenterBottomDragger;

		// Token: 0x04000908 RID: 2312
		private Rectangle RightBottomDragger;

		// Token: 0x04000909 RID: 2313
		private Border DraggerContainerBorder;

		// Token: 0x0400090A RID: 2314
		public static readonly DependencyProperty DragModeProperty = DependencyProperty.Register("DragMode", typeof(ResizeViewOperation), typeof(ResizeView), new PropertyMetadata(ResizeViewOperation.All, new PropertyChangedCallback(ResizeView.OnDragModePropertyChanged)));

		// Token: 0x0400090B RID: 2315
		public static readonly DependencyProperty CanDragCrossProperty = DependencyProperty.Register("CanDragCross", typeof(bool), typeof(ResizeView), new PropertyMetadata(true));

		// Token: 0x0400090C RID: 2316
		public static readonly DependencyProperty DragPlaceholderFillProperty = DependencyProperty.Register("DragPlaceholderFill", typeof(Brush), typeof(ResizeView), new PropertyMetadata(null));

		// Token: 0x0400090D RID: 2317
		public static readonly DependencyProperty IsDraggerVisibleProperty = DependencyProperty.Register("IsDraggerVisible", typeof(bool), typeof(ResizeView), new PropertyMetadata(true, new PropertyChangedCallback(ResizeView.OnIsDraggerVisiblePropertyChanged)));

		// Token: 0x0400090E RID: 2318
		public static readonly DependencyProperty PlaceholderContentProperty = DependencyProperty.Register("PlaceholderContent", typeof(object), typeof(ResizeView), new PropertyMetadata(null, new PropertyChangedCallback(ResizeView.OnPlaceholderContentPropertyChanged)));

		// Token: 0x0400090F RID: 2319
		public static readonly DependencyProperty PlaceholderContentTemplateProperty = DependencyProperty.Register("PlaceholderContentTemplate", typeof(ControlTemplate), typeof(ResizeView), new PropertyMetadata(null));

		// Token: 0x04000910 RID: 2320
		public static readonly DependencyProperty IsCompactModeProperty = DependencyProperty.Register("IsCompactMode", typeof(bool), typeof(ResizeView), new PropertyMetadata(false, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			ResizeView resizeView = s as ResizeView;
			if (resizeView != null && !object.Equals(a.NewValue, a.OldValue))
			{
				resizeView.UpdateSizeMode();
			}
		}));

		// Token: 0x04000911 RID: 2321
		public static readonly DependencyProperty IsProportionalScaleEnabledProperty = DependencyProperty.Register("IsProportionalScaleEnabled", typeof(bool), typeof(ResizeView), new PropertyMetadata(true, delegate(DependencyObject s, DependencyPropertyChangedEventArgs a)
		{
			ResizeView resizeView2 = s as ResizeView;
			if (resizeView2 != null && !object.Equals(a.NewValue, a.OldValue))
			{
				resizeView2.UpdateMouseMoveProperties();
			}
		}));

		// Token: 0x04000912 RID: 2322
		private bool dragging;

		// Token: 0x04000913 RID: 2323
		private Window curWindow;

		// Token: 0x04000914 RID: 2324
		private ResizeView.DragDataContext? dragDataContext;

		// Token: 0x020005F6 RID: 1526
		private struct DragDataContext
		{
			// Token: 0x04002008 RID: 8200
			public ResizeViewOperation? Operation;

			// Token: 0x04002009 RID: 8201
			public Size StartSize;

			// Token: 0x0400200A RID: 8202
			public Point StartPoint;

			// Token: 0x0400200B RID: 8203
			public Point CurrentPoint;

			// Token: 0x0400200C RID: 8204
			public bool CanXCross;

			// Token: 0x0400200D RID: 8205
			public bool CanYCross;
		}
	}
}
