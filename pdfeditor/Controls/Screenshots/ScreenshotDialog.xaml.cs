using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Win32;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.BasicTypes;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using pdfeditor.Views;
using PDFKit;
using PDFKit.Services;
using PDFKit.Utils;
using PDFKit.Utils.PageHeaderFooters;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000219 RID: 537
	public partial class ScreenshotDialog : global::System.Windows.Controls.UserControl, INotifyPropertyChanged
	{
		// Token: 0x17000A67 RID: 2663
		// (get) Token: 0x06001DBE RID: 7614 RVA: 0x00080820 File Offset: 0x0007EA20
		public MainViewModel VM
		{
			get
			{
				return Ioc.Default.GetRequiredService<MainViewModel>();
			}
		}

		// Token: 0x17000A68 RID: 2664
		// (get) Token: 0x06001DBF RID: 7615 RVA: 0x0008082C File Offset: 0x0007EA2C
		// (set) Token: 0x06001DC0 RID: 7616 RVA: 0x00080834 File Offset: 0x0007EA34
		public FS_RECTF? BeforeCropBox { get; private set; }

		// Token: 0x17000A69 RID: 2665
		// (get) Token: 0x06001DC1 RID: 7617 RVA: 0x0008083D File Offset: 0x0007EA3D
		// (set) Token: 0x06001DC2 RID: 7618 RVA: 0x00080845 File Offset: 0x0007EA45
		public FS_RECTF? CropBox { get; private set; }

		// Token: 0x17000A6A RID: 2666
		// (get) Token: 0x06001DC3 RID: 7619 RVA: 0x0008084E File Offset: 0x0007EA4E
		// (set) Token: 0x06001DC4 RID: 7620 RVA: 0x00080858 File Offset: 0x0007EA58
		public DrawControlMode DrawControlMode
		{
			get
			{
				return this.drawControlMode;
			}
			set
			{
				this.drawControlMode = value;
				if (value == DrawControlMode.None)
				{
					this.DragResizeView.DragMode = ResizeViewOperation.All;
				}
				else
				{
					this.DragResizeView.DragMode = ResizeViewOperation.None;
				}
				if (this.SelectedDrawControl != null)
				{
					Border border = this.SelectedDrawControl as Border;
					if (border == null || border.Child != null || this.drawControlMode != DrawControlMode.DrawRectangle)
					{
						Border border2 = this.SelectedDrawControl as Border;
						if ((border2 == null || !(border2.Child is global::System.Windows.Controls.TextBox) || this.drawControlMode != DrawControlMode.DrawText) && (!(this.SelectedDrawControl is Ellipse) || this.drawControlMode != DrawControlMode.DrawCircle) && (!(this.SelectedDrawControl is Polyline) || this.drawControlMode != DrawControlMode.DrawInk) && (!(this.SelectedDrawControl is global::System.Windows.Controls.Control) || this.drawControlMode != DrawControlMode.DrawArrow))
						{
							this.RemoveSelected();
						}
					}
				}
				this.RaisePropertyChanged("DrawControlMode");
			}
		}

		// Token: 0x17000A6B RID: 2667
		// (get) Token: 0x06001DC5 RID: 7621 RVA: 0x00080930 File Offset: 0x0007EB30
		// (set) Token: 0x06001DC6 RID: 7622 RVA: 0x00080938 File Offset: 0x0007EB38
		public global::System.Windows.Media.Brush CurrentBrush
		{
			get
			{
				return this.currentBrush;
			}
			set
			{
				this.currentBrush = value;
			}
		}

		// Token: 0x17000A6C RID: 2668
		// (get) Token: 0x06001DC7 RID: 7623 RVA: 0x00080941 File Offset: 0x0007EB41
		// (set) Token: 0x06001DC8 RID: 7624 RVA: 0x00080949 File Offset: 0x0007EB49
		public double CurrentThickness
		{
			get
			{
				return this.currentThickness;
			}
			set
			{
				this.currentThickness = value;
			}
		}

		// Token: 0x17000A6D RID: 2669
		// (get) Token: 0x06001DC9 RID: 7625 RVA: 0x00080952 File Offset: 0x0007EB52
		// (set) Token: 0x06001DCA RID: 7626 RVA: 0x0008095A File Offset: 0x0007EB5A
		public double CurrentFontSize
		{
			get
			{
				return this.currentFontSize;
			}
			set
			{
				this.currentFontSize = value;
			}
		}

		// Token: 0x17000A6E RID: 2670
		// (get) Token: 0x06001DCB RID: 7627 RVA: 0x00080963 File Offset: 0x0007EB63
		// (set) Token: 0x06001DCC RID: 7628 RVA: 0x0008096B File Offset: 0x0007EB6B
		public UIElement SelectedDrawControl
		{
			get
			{
				return this.selectedDrawControl;
			}
			set
			{
				this.selectedDrawControl = value;
			}
		}

		// Token: 0x06001DCD RID: 7629 RVA: 0x00080974 File Offset: 0x0007EB74
		public ScreenshotDialog()
		{
			this.InitializeComponent();
			base.Loaded += this.ScreenshotDialog_Loaded;
			base.Unloaded += this.ScreenshotDialog_Unloaded;
			base.SizeChanged += this.ScreenshotDialog_SizeChanged;
			this.autoScrollTimer = new DispatcherTimer(DispatcherPriority.Normal);
			this.autoScrollTimer.Interval = TimeSpan.FromMilliseconds(50.0);
			this.autoScrollTimer.Tick += this.AutoScrollTimer_Tick;
			this.ExtractTextToolbar.ScreenshotDialog = this;
			this.ImageToolbar.ScreenshotDialog = this;
			this.OcrToolbar.ScreenshotDialog = this;
			this.CropPageToolbar.ScreenshotDialog = this;
			this.controlArrowTemplate = (ControlTemplate)base.FindResource("PART_ControlArrow");
			this.undoDrawControlStack = new Stack<DrawOperation>();
		}

		// Token: 0x06001DCE RID: 7630 RVA: 0x00080A98 File Offset: 0x0007EC98
		private void ScreenshotDialog_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.BackgroundRectangle.Rect = new Rect(default(global::System.Windows.Point), e.NewSize);
		}

		// Token: 0x06001DCF RID: 7631 RVA: 0x00080AC4 File Offset: 0x0007ECC4
		private void ScreenshotDialog_Loaded(object sender, RoutedEventArgs e)
		{
			AnnotationCanvas annotationCanvas = base.Parent as AnnotationCanvas;
			if (annotationCanvas != null)
			{
				this.viewer = annotationCanvas.PdfViewer;
				this.viewerSv = (ScrollViewer)this.viewer.Parent;
			}
		}

		// Token: 0x06001DD0 RID: 7632 RVA: 0x00080B02 File Offset: 0x0007ED02
		private void ScreenshotDialog_Unloaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06001DD1 RID: 7633 RVA: 0x00080B04 File Offset: 0x0007ED04
		private void CancelCore()
		{
			if (this.mode == ScreenshotDialogMode.CropPage)
			{
				this.ResetCropSize(this.pageIdx);
			}
			this.mode = ScreenshotDialogMode.Screenshot;
			this.DragResizeView.Opacity = 0.0;
			this.UpdateToolbarVisibility(false);
			Canvas.SetLeft(this.DragResizeView, 0.0);
			Canvas.SetTop(this.DragResizeView, 0.0);
			this.DragResizeView.Width = 0.0;
			this.DragResizeView.Height = 0.0;
			this.DraggerRectangle.Rect = default(Rect);
			this.RootBorder.Cursor = global::System.Windows.Input.Cursors.Cross;
			this.pageIdx = -1;
			this.startPt = default(FS_POINTF);
			this.curPt = default(FS_POINTF);
			this.curPoint = default(global::System.Windows.Point);
			SortedDictionary<int, FS_RECTF> pageCropBoxInfo = this.viewer.PageCropBoxInfo;
			if (pageCropBoxInfo != null)
			{
				pageCropBoxInfo.Clear();
			}
			this.startDrawPoint = new global::System.Windows.Point(-1.0, -1.0);
			this.curDrawPoint = new global::System.Windows.Point(-1.0, -1.0);
			this.SelectedDrawControl = null;
			this.controlLocation = default(Rect);
			this.undoDrawControlStack.Clear();
			this.DragResizeView.ClearDrawUIElementOfCanvas();
			this.DrawControlMode = DrawControlMode.None;
		}

		// Token: 0x06001DD2 RID: 7634 RVA: 0x00080C68 File Offset: 0x0007EE68
		public async Task<ScreenshotDialogResult> ShowDialogAsync(ScreenshotDialogMode mode, int[] pages = null)
		{
			if (this.tcs != null)
			{
				this.Close(null);
			}
			this.Pages = pages;
			this.mode = mode;
			this.viewerSv.ScrollChanged -= this.ViewerSv_ScrollChanged;
			this.viewerSv.ScrollChanged += this.ViewerSv_ScrollChanged;
			if (this.window != null)
			{
				this.window.PreviewKeyDown -= this.Window_PreviewKeyDown;
				this.window = null;
			}
			this.window = Window.GetWindow(this);
			if (this.window != null)
			{
				this.window.PreviewKeyDown += this.Window_PreviewKeyDown;
			}
			base.Visibility = Visibility.Visible;
			if (this.MoveBoxSize.Text != "")
			{
				this.MoveBoxSize.Text = "";
			}
			if (mode == ScreenshotDialogMode.CropPage)
			{
				GAManager.SendEvent("CropPage", "ShowDialog", "Count", 1L);
				this.CropPageToolbar.ApplypageIndex = null;
				if (this.BeforeCropBox != null)
				{
					this.BeforeCropBox = null;
				}
				if (this.DraggerParent.Children.Count > 7)
				{
					for (int i = 7; i <= this.DraggerParent.Children.Count - 1; i++)
					{
						this.DraggerParent.Children.RemoveAt(i);
					}
				}
				if (((this.DraggerParent.Children[0] as global::System.Windows.Shapes.Path).Data as GeometryGroup).Children.Count > 2)
				{
					for (int j = 2; j <= ((this.DraggerParent.Children[0] as global::System.Windows.Shapes.Path).Data as GeometryGroup).Children.Count - 1; j++)
					{
						((this.DraggerParent.Children[0] as global::System.Windows.Shapes.Path).Data as GeometryGroup).Children.RemoveAt(j);
					}
				}
				this.viewer.UpdateLayout();
				this.viewer.UpdateDocLayout();
				double contentVerticalOffset = this.viewerSv.ContentVerticalOffset;
				this.viewerSv.ScrollToVerticalOffset(contentVerticalOffset - 10.0);
			}
			this.tcs = new TaskCompletionSource<ScreenshotDialogResult>();
			return await this.tcs.Task;
		}

		// Token: 0x06001DD3 RID: 7635 RVA: 0x00080CBB File Offset: 0x0007EEBB
		public void ScrollToPage()
		{
			this.viewer.ScrollToPage(this.pageIdx);
			this.viewer.CurrentIndex = this.pageIdx;
		}

		// Token: 0x06001DD4 RID: 7636 RVA: 0x00080CE0 File Offset: 0x0007EEE0
		public void Close(ScreenshotDialogResult result = null)
		{
			this.CancelCore();
			if (this.window != null)
			{
				this.window.PreviewKeyDown -= this.Window_PreviewKeyDown;
				this.window = null;
			}
			this.viewerSv.ScrollChanged -= this.ViewerSv_ScrollChanged;
			this.autoScrollTimer.Stop();
			if (this.tcs == null || this.tcs.Task.IsCompleted || this.tcs.Task.IsCanceled)
			{
				return;
			}
			TaskCompletionSource<ScreenshotDialogResult> taskCompletionSource = this.tcs;
			this.tcs = null;
			base.Visibility = Visibility.Collapsed;
			taskCompletionSource.SetResult(result ?? ScreenshotDialogResult.CreateCancel());
		}

		// Token: 0x06001DD5 RID: 7637 RVA: 0x00080D8C File Offset: 0x0007EF8C
		private async void Window_PreviewKeyDown(object sender, global::System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				this.Close(null);
			}
			else if (e.Key == Key.Return)
			{
				if (this.mode == ScreenshotDialogMode.ExtractText)
				{
					await this.CompleteExtractTextAsync(false);
				}
				else if (this.mode == ScreenshotDialogMode.Screenshot)
				{
					if (this.DrawControlMode == DrawControlMode.DrawText)
					{
						Border border = this.SelectedDrawControl as Border;
						if (border != null && border.Child is global::System.Windows.Controls.TextBox)
						{
							return;
						}
					}
					await this.CompleteImageAsync();
				}
				else if (this.mode == ScreenshotDialogMode.Ocr)
				{
					await this.CompleteExtractTextAsync(true);
				}
			}
			else if (e.Key == Key.Delete && this.mode == ScreenshotDialogMode.Screenshot && this.SelectedDrawControl != null)
			{
				Border border2 = this.SelectedDrawControl as Border;
				if (border2 != null)
				{
					global::System.Windows.Controls.TextBox textBox = border2.Child as global::System.Windows.Controls.TextBox;
					if (textBox != null && string.IsNullOrEmpty(textBox.Text))
					{
						return;
					}
				}
				FrameworkElement frameworkElement = (FrameworkElement)this.SelectedDrawControl;
				double left = Canvas.GetLeft(frameworkElement);
				double top = Canvas.GetTop(frameworkElement);
				ResizeView dragResizeView = this.DragResizeView;
				if (dragResizeView != null)
				{
					dragResizeView.RemoveDrawControl(frameworkElement);
				}
				this.undoDrawControlStack.Push(new DeleteControlOperation(frameworkElement, left, top));
				this.SelectedDrawControl = null;
			}
		}

		// Token: 0x06001DD6 RID: 7638 RVA: 0x00080DCC File Offset: 0x0007EFCC
		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);
			if (this.viewerSv != null && (this.curRect.IsEmpty || this.curRect == default(Rect)))
			{
				if (this.pageIdx != -1)
				{
					FS_RECTF effectiveBox = this.viewer.Document.Pages[this.pageIdx].GetEffectiveBox(PageRotate.Normal, false);
					Rect rect;
					if (!this.viewer.TryGetClientRect(this.pageIdx, effectiveBox, out rect))
					{
						return;
					}
					if ((rect.Top > 0.0 && e.Delta > 0) || (rect.Bottom < base.ActualHeight && e.Delta < 0))
					{
						return;
					}
				}
				this.viewerSv.RaiseEvent(e);
			}
		}

		// Token: 0x06001DD7 RID: 7639 RVA: 0x00080E94 File Offset: 0x0007F094
		private void ViewerSv_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (this.pageIdx != -1)
			{
				if (this.DragResizeView.Opacity == 0.0)
				{
					this.curPt = this.GetCurrentPagePoint(this.curPoint);
				}
				this.UpdateBounds(false, false);
			}
		}

		// Token: 0x06001DD8 RID: 7640 RVA: 0x00080ED0 File Offset: 0x0007F0D0
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				if (this.mode != ScreenshotDialogMode.Screenshot)
				{
					if (this.DragResizeView.Opacity == 0.0)
					{
						this.autoScrollTimer.Start();
						this.curPoint = e.GetPosition(this);
						this.pageIdx = this.viewer.PointInPage(this.curPoint);
						this.StartPoint = this.curPoint;
						global::System.Windows.Point point;
						if (this.pageIdx != -1 && this.viewer.TryGetPagePoint(this.pageIdx, this.curPoint, out point))
						{
							if (this.mode == ScreenshotDialogMode.CropPage)
							{
								this.RenderFirstCropBox(this.pageIdx);
								int[] array = new int[] { this.pageIdx };
								this.CropPageToolbar.ApplypageIndex = array;
							}
							this.startPt = point.ToPdfPoint();
							this.curPt = this.startPt;
							base.CaptureMouse();
							e.Handled = true;
							return;
						}
					}
				}
				else
				{
					Border border = this.SelectedDrawControl as Border;
					if (border != null)
					{
						global::System.Windows.Controls.TextBox textBox = border.Child as global::System.Windows.Controls.TextBox;
						if (textBox != null)
						{
							if (textBox.Text != this.controlTextBoxText && !string.IsNullOrEmpty(this.controlTextBoxText))
							{
								this.undoDrawControlStack.Push(new ChangeTextBoxTextOperation(this.SelectedDrawControl, this.controlTextBoxText));
							}
							this.controlTextBoxText = null;
							this.RemoveSelected();
							return;
						}
					}
					this.RemoveSelected();
					if (!this.isDragDrawControl)
					{
						if (this.DragResizeView.Opacity == 0.0)
						{
							this.autoScrollTimer.Start();
							this.curPoint = e.GetPosition(this);
							this.pageIdx = this.viewer.PointInPage(this.curPoint);
							this.StartPoint = this.curPoint;
							global::System.Windows.Point point2;
							if (this.pageIdx != -1 && this.viewer.TryGetPagePoint(this.pageIdx, this.curPoint, out point2))
							{
								if (this.mode == ScreenshotDialogMode.CropPage)
								{
									this.RenderFirstCropBox(this.pageIdx);
									int[] array2 = new int[] { this.pageIdx };
									this.CropPageToolbar.ApplypageIndex = array2;
								}
								this.startPt = point2.ToPdfPoint();
								this.curPt = this.startPt;
								base.CaptureMouse();
								e.Handled = true;
								return;
							}
						}
						else if (this.DragResizeView.Opacity != 0.0 && this.DrawControlMode != DrawControlMode.None)
						{
							this.startDrawPoint = new global::System.Windows.Point(-1.0, -1.0);
							global::System.Windows.Point position = e.GetPosition(this.DragResizeView);
							if (this.CheckPointInDraggerCanvas(position))
							{
								this.startDrawPoint = position;
								base.CaptureMouse();
								if (this.ControlTextBox != null)
								{
									base.Focus();
								}
								if (this.DrawControlMode == DrawControlMode.DrawText)
								{
									this.DrawText();
								}
								else
								{
									base.Focus();
								}
								e.Handled = true;
								return;
							}
						}
					}
				}
			}
			global::System.Windows.Point position2 = e.GetPosition(this);
			int num;
			if (this.HitSeleteBoxTest(position2, out num))
			{
				this.pageIdx = num;
				this.ResetSelectedBox(this.pageIdx);
			}
			base.OnMouseDown(e);
		}

		// Token: 0x06001DD9 RID: 7641 RVA: 0x000811E0 File Offset: 0x0007F3E0
		private bool HitSeleteBoxTest(global::System.Windows.Point mousePoint, out int pagenum)
		{
			List<global::System.Windows.Shapes.Rectangle> list = this.DraggerParent.Children.OfType<global::System.Windows.Shapes.Rectangle>().ToList<global::System.Windows.Shapes.Rectangle>();
			int[] applypageIndex = this.CropPageToolbar.ApplypageIndex;
			pagenum = this.pageIdx;
			foreach (global::System.Windows.Shapes.Rectangle rectangle in list)
			{
				if (rectangle.IsMouseOver)
				{
					pagenum = this.viewer.PointInPage(mousePoint);
					double top = Canvas.GetTop(rectangle);
					double left = Canvas.GetLeft(rectangle);
					Rect rect = new Rect(left, top, rectangle.Width, rectangle.Height);
					FS_RECTF fs_RECTF;
					this.viewer.TryGetPageRect(pagenum, rect, out fs_RECTF);
					this.CropBox = new FS_RECTF?(fs_RECTF);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001DDA RID: 7642 RVA: 0x000812B0 File Offset: 0x0007F4B0
		protected override void OnMouseMove(global::System.Windows.Input.MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				if (this.mode != ScreenshotDialogMode.Screenshot)
				{
					if (this.DragResizeView.Opacity == 0.0 && this.pageIdx != -1)
					{
						this.curPoint = e.GetPosition(this);
						this.curPt = this.GetCurrentPagePoint(this.curPoint);
						this.UpdateBounds(false, false);
						e.Handled = true;
						return;
					}
				}
				else if (!this.isDragDrawControl)
				{
					if (this.DragResizeView.Opacity == 0.0 && this.pageIdx != -1)
					{
						this.curPoint = e.GetPosition(this);
						this.curPt = this.GetCurrentPagePoint(this.curPoint);
						this.UpdateBounds(false, false);
						this.curZoomFactor = this.viewer.Zoom;
						e.Handled = true;
						return;
					}
					if (this.DragResizeView.Opacity != 0.0 && this.DrawControlMode != DrawControlMode.None)
					{
						if (this.startDrawPoint.X == -1.0 && this.startDrawPoint.Y == -1.0)
						{
							return;
						}
						this.curDrawPoint = new global::System.Windows.Point(-1.0, -1.0);
						this.curDrawPoint = e.GetPosition(this.DragResizeView);
						this.KeepPointInDraggerCanvas(ref this.curDrawPoint);
						if (this.startDrawPoint == this.curDrawPoint)
						{
							return;
						}
						this.DrawControl(this.curDrawPoint, this.DrawControlMode);
					}
				}
			}
			base.OnMouseMove(e);
		}

		// Token: 0x06001DDB RID: 7643 RVA: 0x0008144C File Offset: 0x0007F64C
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				base.ReleaseMouseCapture();
				this.curPoint = e.GetPosition(this);
				this.autoScrollTimer.Stop();
				if (this.mode != ScreenshotDialogMode.Screenshot)
				{
					if (this.DragResizeView.Opacity == 0.0 && this.pageIdx != -1)
					{
						this.curPt = this.GetCurrentPagePoint(this.curPoint);
						if (this.startPt.X == 0f && this.startPt.Y == 0f)
						{
							this.startPt = this.GetCurrentPagePoint(new global::System.Windows.Point(this.curPoint.X, this.StartPoint.Y));
							this.startPt.X = 0f;
						}
						this.DragResizeView.Opacity = 1.0;
						this.RootBorder.Cursor = global::System.Windows.Input.Cursors.Arrow;
						this.UpdateToolbarVisibility(true);
						this.UpdateBounds(false, false);
						if (this.mode == ScreenshotDialogMode.CropPage)
						{
							try
							{
								this.sizeModes = this.VM.ViewToolbar.DocSizeMode;
								this.pageZoom = this.VM.ViewToolbar.DocZoom;
							}
							catch
							{
							}
							this.CropPageToolbar.OpenAdvanceWindow();
						}
					}
				}
				else if (!this.isDragDrawControl)
				{
					if (this.DragResizeView.Opacity == 0.0 && this.pageIdx != -1)
					{
						this.curPt = this.GetCurrentPagePoint(this.curPoint);
						this.DragResizeView.Opacity = 1.0;
						this.RootBorder.Cursor = global::System.Windows.Input.Cursors.Arrow;
						this.UpdateToolbarVisibility(true);
						this.UpdateBounds(false, false);
					}
					else if (this.DragResizeView.Opacity != 0.0 && this.DrawControlMode != DrawControlMode.None && this.startDrawPoint.X != -1.0 && this.startDrawPoint.Y != -1.0 && this.startDrawPoint != this.curDrawPoint)
					{
						switch (this.DrawControlMode)
						{
						case DrawControlMode.DrawRectangle:
							if (this.ControlRectangle != null && this.controlLocation != default(Rect))
							{
								this.undoDrawControlStack.Push(new DrawControlOperation(OperationType.DrawRectangle, this.ControlRectangle));
							}
							break;
						case DrawControlMode.DrawCircle:
							if (this.ControlCircle != null && this.controlLocation != default(Rect))
							{
								this.undoDrawControlStack.Push(new DrawControlOperation(OperationType.DrawCircle, this.ControlCircle));
							}
							break;
						case DrawControlMode.DrawArrow:
							if (this.ControlArrow != null && this.controlLocation != default(Rect))
							{
								this.undoDrawControlStack.Push(new DrawControlOperation(OperationType.DrawArrow, this.ControlArrow));
							}
							break;
						case DrawControlMode.DrawInk:
							if (this.ControlInk != null)
							{
								this.undoDrawControlStack.Push(new DrawControlOperation(OperationType.DrawInk, this.ControlInk));
							}
							break;
						}
					}
				}
			}
			else if (e.ChangedButton == MouseButton.Right && e.LeftButton != MouseButtonState.Pressed)
			{
				base.ReleaseMouseCapture();
				this.Cancel();
				this.Close(null);
			}
			this.ClearDraw();
			base.OnMouseUp(e);
		}

		// Token: 0x06001DDC RID: 7644 RVA: 0x000817BC File Offset: 0x0007F9BC
		public void ResetSelectedBox(int pageIdx)
		{
			if (pageIdx == -1)
			{
				return;
			}
			this.pageIdx = pageIdx;
			this.DragResizeView.Opacity = 1.0;
			PdfPage pdfPage = this.viewer.Document.Pages[pageIdx];
			this.VM.ViewToolbar.DocSizeMode = SizeModes.FitToSize;
			FS_RECTF? fs_RECTF = null;
			if (this.BeforeCropBox != null && PageHeaderFooterUtils.PdfPointToCm((double)Math.Abs(this.BeforeCropBox.Value.Width)) >= 0.15 && PageHeaderFooterUtils.PdfPointToCm((double)Math.Abs(this.BeforeCropBox.Value.Width)) >= 0.15)
			{
				fs_RECTF = new FS_RECTF?(this.BeforeCropBox.Value);
			}
			if (fs_RECTF == null)
			{
				fs_RECTF = new FS_RECTF?(pdfPage.MediaBox);
			}
			this.curPt.X = fs_RECTF.Value.right;
			this.curPt.Y = fs_RECTF.Value.bottom;
			this.startPt.Y = fs_RECTF.Value.top;
			this.startPt.X = fs_RECTF.Value.left;
			pdfPage.ReloadPage();
			PdfDocumentStateService.TryRedrawViewerCurrentPage(pdfPage);
			this.UpdateBounds(false, false);
			this.UpdateToolbarVisibility(true);
			this.RootBorder.Cursor = global::System.Windows.Input.Cursors.Arrow;
		}

		// Token: 0x06001DDD RID: 7645 RVA: 0x00081934 File Offset: 0x0007FB34
		private void AutoScrollTimer_Tick(object sender, EventArgs e)
		{
			if (this.pageIdx != -1)
			{
				double num = 0.0;
				double num2 = 0.0;
				Thickness thickness = new Thickness(16.0);
				bool flag = false;
				bool flag2 = false;
				if (this.curPoint.X < 50.0)
				{
					flag = true;
					double num3 = Math.Max(Math.Min((50.0 - this.curPoint.X) / 50.0, 1.0), 0.0);
					num = -30.0 * num3;
				}
				else if (this.curPoint.X > base.ActualWidth - 50.0)
				{
					double num4 = Math.Max(Math.Min((50.0 - (base.ActualWidth - this.curPoint.X)) / 50.0, 1.0), 0.0);
					num = 30.0 * num4;
				}
				if (this.curPoint.Y < 50.0)
				{
					flag2 = true;
					double num5 = Math.Max(Math.Min((50.0 - this.curPoint.Y) / 50.0, 1.0), 0.0);
					num2 = -30.0 * num5;
				}
				else if (this.curPoint.Y > base.ActualHeight - 50.0)
				{
					double num6 = Math.Max(Math.Min((50.0 - (base.ActualHeight - this.curPoint.Y)) / 50.0, 1.0), 0.0);
					num2 = 30.0 * num6;
				}
				if (num == 0.0 && num2 == 0.0)
				{
					return;
				}
				this.viewer.UpdateLayout();
				FS_RECTF effectiveBox = this.viewer.Document.Pages[this.pageIdx].GetEffectiveBox(PageRotate.Normal, false);
				Rect rect;
				if (!this.viewer.TryGetClientRect(this.pageIdx, effectiveBox, out rect))
				{
					return;
				}
				if (flag)
				{
					if (rect.Left - thickness.Left <= 0.0)
					{
						if (num < rect.Left - thickness.Left)
						{
							num = rect.Left - thickness.Left;
						}
					}
					else
					{
						num = 0.0;
					}
				}
				else if (rect.Right + thickness.Right >= base.ActualWidth)
				{
					if (num > rect.Right + thickness.Right - base.ActualWidth)
					{
						num = rect.Right + thickness.Right - base.ActualWidth;
					}
				}
				else
				{
					num = 0.0;
				}
				if (flag2)
				{
					if (rect.Top - thickness.Top <= 0.0)
					{
						if (num2 < rect.Top - thickness.Top)
						{
							num2 = rect.Top - thickness.Top;
						}
					}
					else
					{
						num2 = 0.0;
					}
				}
				else if (rect.Bottom + thickness.Bottom >= base.ActualHeight)
				{
					if (num2 > rect.Bottom + thickness.Bottom - base.ActualHeight)
					{
						num2 = rect.Bottom + thickness.Bottom - base.ActualHeight;
					}
				}
				else
				{
					num2 = 0.0;
				}
				bool flag3 = false;
				if (Math.Abs(num) > 1.0)
				{
					double num7 = num + this.viewerSv.HorizontalOffset;
					this.viewerSv.ScrollToHorizontalOffset(num7);
					flag3 = true;
				}
				if (Math.Abs(num2) > 1.0)
				{
					double num8 = num2 + this.viewerSv.VerticalOffset;
					this.viewerSv.ScrollToVerticalOffset(num8);
					flag3 = true;
				}
				if (flag3)
				{
					this.viewer.UpdateDocLayout();
				}
			}
		}

		// Token: 0x06001DDE RID: 7646 RVA: 0x00081D30 File Offset: 0x0007FF30
		private void UpdateToolbarPosition(Rect rect, bool isInput = false)
		{
			FrameworkElement frameworkElement = null;
			if (this.mode == ScreenshotDialogMode.Screenshot)
			{
				frameworkElement = this.ImageToolbar;
			}
			else if (this.mode == ScreenshotDialogMode.ExtractText)
			{
				frameworkElement = this.ExtractTextToolbar;
			}
			else if (this.mode == ScreenshotDialogMode.Ocr)
			{
				frameworkElement = this.OcrToolbar;
			}
			else if (this.mode == ScreenshotDialogMode.CropPage)
			{
				frameworkElement = this.CropPageToolbar;
			}
			if (this.pageIdx != -1 && frameworkElement != null)
			{
				double actualWidth = frameworkElement.ActualWidth;
				double actualHeight = frameworkElement.ActualHeight;
				double num = rect.Right - actualWidth;
				Rect rect2 = new Rect(0.0, 0.0, base.ActualHeight, base.ActualHeight);
				double num2;
				if (rect2.Bottom - rect.Bottom > actualHeight)
				{
					num2 = rect.Bottom;
				}
				else if (rect.Top - rect2.Top > actualHeight)
				{
					num2 = rect.Top - actualHeight;
				}
				else
				{
					double num3 = rect.Top - rect2.Top;
					double num4 = rect2.Bottom - rect.Bottom;
					if (num3 > num4)
					{
						num2 = rect.Top;
					}
					else
					{
						num2 = rect.Bottom - actualHeight;
					}
				}
				if (this.mode != ScreenshotDialogMode.CropPage)
				{
					Canvas.SetLeft(frameworkElement, num);
					Canvas.SetTop(frameworkElement, num2);
					return;
				}
				PdfPage pdfPage = this.viewer.Document.Pages[this.pageIdx];
				pdfPage.GetEffectiveBox(PageRotate.Normal, false);
				float height = pdfPage.MediaBox.Height;
				float width = pdfPage.MediaBox.Width;
				Canvas.SetLeft(frameworkElement, rect.Right);
				if (rect.Top < 0.0)
				{
					Canvas.SetTop(frameworkElement, 0.0);
				}
				else
				{
					Canvas.SetTop(frameworkElement, rect.Top);
				}
				float num5 = Math.Min(this.startPt.X, this.curPt.X);
				float num6 = Math.Min(this.startPt.Y, this.curPt.Y);
				float num7 = Math.Max(this.startPt.X, this.curPt.X);
				float num8 = Math.Max(this.startPt.Y, this.curPt.Y);
				float num9 = num8 - num6;
				this.CropPageToolbar.PageMargin = new MarginModel
				{
					Left = (double)num5,
					Bottom = (double)(num8 - num9),
					Top = (double)(height - num8),
					Right = (double)(width - num7),
					PageWidth = (double)width,
					PageHeight = (double)height,
					Screenshot = this
				};
				if (height - num8 <= 20f)
				{
					this.MoveBoxSize.Margin = new Thickness(rect.Left, rect.Top, 0.0, 0.0);
				}
				else
				{
					this.MoveBoxSize.Margin = new Thickness(rect.Left, rect.Top - 20.0, 0.0, 0.0);
				}
				string text = PageHeaderFooterUtils.PdfPointToCm((double)num9).ToString("F2");
				string text2 = PageHeaderFooterUtils.PdfPointToCm((double)(num7 - num5)).ToString("F2");
				this.MoveBoxSize.Text = text2 + "cm*" + text + "cm";
			}
		}

		// Token: 0x06001DDF RID: 7647 RVA: 0x00082089 File Offset: 0x00080289
		public MarginModel reflashData()
		{
			return this.CropPageToolbar.PageMargin;
		}

		// Token: 0x06001DE0 RID: 7648 RVA: 0x00082096 File Offset: 0x00080296
		public PageSizeModel reflashSizeData()
		{
			return this.CropPageToolbar.PageSize;
		}

		// Token: 0x06001DE1 RID: 7649 RVA: 0x000820A4 File Offset: 0x000802A4
		public void ReflashViewerselectedBox(int[] pageindexs)
		{
			foreach (int num in pageindexs)
			{
				PdfPage pdfPage = this.viewer.Document.Pages[num];
				pdfPage.CropBox = pdfPage.MediaBox;
				pdfPage.TrimBox = pdfPage.MediaBox;
			}
			this.viewer.UpdateLayout();
			this.viewer.UpdateDocLayout();
			this.viewer.TryRedrawVisiblePageAsync(default(CancellationToken));
		}

		// Token: 0x06001DE2 RID: 7650 RVA: 0x00082120 File Offset: 0x00080320
		private FS_POINTF GetCurrentPagePoint(global::System.Windows.Point point)
		{
			if (this.pageIdx == -1)
			{
				return default(FS_POINTF);
			}
			try
			{
				global::System.Windows.Point point2;
				if (this.viewer.TryGetPagePoint(this.pageIdx, point, out point2))
				{
					FS_POINTF fs_POINTF = point2.ToPdfPoint();
					PdfPage pdfPage = this.viewer.Document.Pages[this.pageIdx];
					FS_RECTF fs_RECTF = pdfPage.GetEffectiveBox(PageRotate.Normal, false);
					if (this.mode == ScreenshotDialogMode.CropPage)
					{
						if (pdfPage.Dictionary.ContainsKey("CropBox"))
						{
							fs_RECTF = pdfPage.CropBox;
						}
						else if (pdfPage.Dictionary.ContainsKey("MediaBox"))
						{
							fs_RECTF = pdfPage.MediaBox;
						}
					}
					fs_POINTF.X = Math.Max(fs_RECTF.left, Math.Min(fs_POINTF.X, fs_RECTF.right));
					fs_POINTF.Y = Math.Max(fs_RECTF.bottom, Math.Min(fs_POINTF.Y, fs_RECTF.top));
					return fs_POINTF;
				}
			}
			catch
			{
				try
				{
					GAManager.SendEvent("Exception", "GetCurrentPagePoint", "Count", 1L);
				}
				catch
				{
				}
			}
			return default(FS_POINTF);
		}

		// Token: 0x06001DE3 RID: 7651 RVA: 0x00082260 File Offset: 0x00080460
		public void UpdatePriviewBounds(bool isDragging, bool isInput = false)
		{
			if (this.pageIdx == -1)
			{
				return;
			}
			global::System.Windows.Point point;
			global::System.Windows.Point point2;
			if (this.viewer.TryGetClientPoint(this.pageIdx, this.startPt.ToPoint(), out point) && this.viewer.TryGetClientPoint(this.pageIdx, this.curPt.ToPoint(), out point2))
			{
				global::System.Windows.Point point3 = new global::System.Windows.Point(Math.Min(point.X, point2.X), Math.Min(point.Y, point2.Y));
				global::System.Windows.Point point4 = new global::System.Windows.Point(Math.Max(point.X, point2.X), Math.Max(point.Y, point2.Y));
				Rect rect = new Rect(point3, point4);
				if (this.pdfCopyDocument != null)
				{
					this.UpdatePagemarginModel(rect, isInput);
				}
				CropPageWindow cropPageWindow = global::System.Windows.Application.Current.Windows.Cast<Window>().FirstOrDefault((Window window) => window is CropPageWindow) as CropPageWindow;
				if (cropPageWindow != null)
				{
					FS_RECTF fs_RECTF = new FS_RECTF
					{
						right = this.curPt.X,
						bottom = this.curPt.Y,
						left = this.startPt.X,
						top = this.startPt.Y
					};
					cropPageWindow.UpdateCropbox(fs_RECTF);
				}
			}
		}

		// Token: 0x06001DE4 RID: 7652 RVA: 0x000823CC File Offset: 0x000805CC
		public void UpdateCropPagemarginModel()
		{
			if (this.pdfCopyDocument == null)
			{
				return;
			}
			PdfPage pdfPage = this.pdfCopyDocument.Pages[this.PageIndexs];
			pdfPage.GetEffectiveBox(PageRotate.Normal, false);
			float height = pdfPage.MediaBox.Height;
			float width = pdfPage.MediaBox.Width;
			float num = Math.Min(this.startPt.X, this.curPt.X);
			float num2 = Math.Min(this.startPt.Y, this.curPt.Y);
			float num3 = Math.Max(this.startPt.X, this.curPt.X);
			float num4 = Math.Max(this.startPt.Y, this.curPt.Y);
			float num5 = num4 - num2;
			this.CropPageToolbar.PageMargin = new MarginModel
			{
				Left = (double)num,
				Bottom = (double)(num4 - num5),
				Top = (double)(height - num4),
				Right = (double)(width - num3),
				PageWidth = (double)width,
				PageHeight = (double)height,
				Screenshot = this
			};
		}

		// Token: 0x06001DE5 RID: 7653 RVA: 0x000824EC File Offset: 0x000806EC
		public void UpdatePagemarginModel(Rect rect, bool isInput = false)
		{
			if (this.pdfCopyDocument == null || this.pdfCopyDocument.IsDisposed)
			{
				return;
			}
			PdfPage pdfPage = this.pdfCopyDocument.Pages[this.PageIndexs];
			pdfPage.GetEffectiveBox(PageRotate.Normal, false);
			float height = pdfPage.MediaBox.Height;
			float width = pdfPage.MediaBox.Width;
			float num = Math.Min(this.startPt.X, this.curPt.X);
			float num2 = Math.Min(this.startPt.Y, this.curPt.Y);
			float num3 = Math.Max(this.startPt.X, this.curPt.X);
			float num4 = Math.Max(this.startPt.Y, this.curPt.Y);
			float num5 = num4 - num2;
			this.CropPageToolbar.PageMargin = new MarginModel
			{
				Left = (double)num,
				Bottom = (double)(num4 - num5),
				Top = (double)(height - num4),
				Right = (double)(width - num3),
				PageWidth = (double)width,
				PageHeight = (double)height,
				Screenshot = this
			};
			if (height - num4 <= 20f)
			{
				this.MoveBoxSize.Margin = new Thickness(rect.Left, rect.Top, 0.0, 0.0);
				return;
			}
			this.MoveBoxSize.Margin = new Thickness(rect.Left, rect.Top - 20.0, 0.0, 0.0);
		}

		// Token: 0x06001DE6 RID: 7654 RVA: 0x0008268C File Offset: 0x0008088C
		public void UpdateBounds(bool isDragging, bool isInput = false)
		{
			if (this.pageIdx == -1)
			{
				return;
			}
			PdfPage pdfPage = this.viewer.Document.Pages[this.pageIdx];
			global::System.Windows.Point point;
			global::System.Windows.Point point2;
			if (this.viewer.TryGetClientPoint(this.pageIdx, this.startPt.ToPoint(), out point) && this.viewer.TryGetClientPoint(this.pageIdx, this.curPt.ToPoint(), out point2))
			{
				global::System.Windows.Point point3 = new global::System.Windows.Point(Math.Min(point.X, point2.X), Math.Min(point.Y, point2.Y));
				global::System.Windows.Point point4 = new global::System.Windows.Point(Math.Max(point.X, point2.X), Math.Max(point.Y, point2.Y));
				Rect rect = new Rect(point3, point4);
				this.DraggerRectangle.Rect = rect;
				if (!isDragging && this.DragResizeView.Opacity > 0.0)
				{
					Canvas.SetLeft(this.DragResizeView, rect.Left);
					Canvas.SetTop(this.DragResizeView, rect.Top);
					this.DragResizeView.Width = rect.Width;
					this.DragResizeView.Height = rect.Height;
				}
				this.UpdateToolbarPosition(rect, isInput);
			}
		}

		// Token: 0x06001DE7 RID: 7655 RVA: 0x000827DF File Offset: 0x000809DF
		private void CloseBtn_Click(object sender, RoutedEventArgs e)
		{
			this.Close(null);
		}

		// Token: 0x06001DE8 RID: 7656 RVA: 0x000827E8 File Offset: 0x000809E8
		private void DragResizeView_ResizeDragStarted(object sender, ResizeViewResizeDragStartedEventArgs e)
		{
			this.curRect = this.DraggerRectangle.Rect;
		}

		// Token: 0x06001DE9 RID: 7657 RVA: 0x000827FC File Offset: 0x000809FC
		private void DragResizeView_ResizeDragging(object sender, ResizeViewResizeDragEventArgs e)
		{
			Rect rect = new Rect(this.curRect.Left + e.OffsetX, this.curRect.Top + e.OffsetY, e.NewSize.Width, e.NewSize.Height);
			global::System.Windows.Point point = new global::System.Windows.Point(rect.Left, rect.Top);
			global::System.Windows.Point point2 = new global::System.Windows.Point(rect.Right, rect.Bottom);
			global::System.Windows.Point point3;
			global::System.Windows.Point point4;
			if (this.viewer.TryGetPagePoint(this.pageIdx, point, out point3) && this.viewer.TryGetPagePoint(this.pageIdx, point2, out point4))
			{
				this.startPt = point3.ToPdfPoint();
				this.curPt = point4.ToPdfPoint();
				this.CropPageToolbar.SelectionBorderVisible = false;
				this.UpdateBounds(true, false);
			}
		}

		// Token: 0x06001DEA RID: 7658 RVA: 0x000828D8 File Offset: 0x00080AD8
		private void DragResizeView_ResizeDragCompleted(object sender, ResizeViewResizeDragEventArgs e)
		{
			Rect rect = new Rect(this.curRect.Left + e.OffsetX, this.curRect.Top + e.OffsetY, e.NewSize.Width, e.NewSize.Height);
			global::System.Windows.Point point = new global::System.Windows.Point(rect.Left, rect.Top);
			global::System.Windows.Point point2 = new global::System.Windows.Point(rect.Right, rect.Bottom);
			global::System.Windows.Point point3;
			global::System.Windows.Point point4;
			if (this.viewer.TryGetPagePoint(this.pageIdx, point, out point3) && this.viewer.TryGetPagePoint(this.pageIdx, point2, out point4))
			{
				this.startPt = new FS_POINTF(Math.Min(point3.X, point4.X), Math.Max(point3.Y, point4.Y));
				this.curPt = new FS_POINTF(Math.Max(point3.X, point4.X), Math.Min(point3.Y, point4.Y));
				PdfPage pdfPage = this.viewer.Document.Pages[this.pageIdx];
				FS_RECTF fs_RECTF = pdfPage.GetEffectiveBox(PageRotate.Normal, false);
				if (this.mode == ScreenshotDialogMode.CropPage)
				{
					if (pdfPage.Dictionary.ContainsKey("CropBox"))
					{
						fs_RECTF = pdfPage.CropBox;
					}
					else if (pdfPage.Dictionary.ContainsKey("MediaBox"))
					{
						fs_RECTF = pdfPage.MediaBox;
					}
				}
				if (this.startPt.X < fs_RECTF.left)
				{
					this.startPt.X = fs_RECTF.left;
				}
				else if (this.startPt.X > fs_RECTF.right)
				{
					this.startPt.X = Math.Max(fs_RECTF.right - 100f, 0f);
				}
				if (this.startPt.Y > fs_RECTF.top)
				{
					this.startPt.Y = fs_RECTF.top;
				}
				else if (this.startPt.Y < fs_RECTF.bottom)
				{
					this.startPt.Y = Math.Min(fs_RECTF.bottom, 100f);
				}
				if (this.curPt.X > fs_RECTF.right)
				{
					this.curPt.X = fs_RECTF.right;
				}
				else if (this.curPt.X < fs_RECTF.left)
				{
					this.curPt.X = Math.Min(fs_RECTF.left, this.startPt.X + 100f);
				}
				if (this.curPt.Y < fs_RECTF.bottom)
				{
					this.curPt.Y = fs_RECTF.bottom;
				}
				else if (this.curPt.Y > fs_RECTF.top)
				{
					this.curPt.Y = Math.Max(0f, fs_RECTF.top - 100f);
				}
				this.UpdateBounds(false, false);
				this.curRect = Rect.Empty;
				return;
			}
		}

		// Token: 0x06001DEB RID: 7659 RVA: 0x00082BE0 File Offset: 0x00080DE0
		private async void DragResizeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (this.mode == ScreenshotDialogMode.ExtractText)
			{
				await this.CompleteExtractTextAsync(false);
			}
			else if (this.mode == ScreenshotDialogMode.Screenshot)
			{
				await this.CompleteImageAsync();
			}
			else if (this.mode == ScreenshotDialogMode.Ocr)
			{
				await this.CompleteExtractTextAsync(true);
			}
		}

		// Token: 0x06001DEC RID: 7660 RVA: 0x00082C18 File Offset: 0x00080E18
		private void UpdateToolbarVisibility(bool value)
		{
			if (value)
			{
				FrameworkElement frameworkElement = null;
				if (this.mode == ScreenshotDialogMode.Screenshot)
				{
					frameworkElement = this.ImageToolbar;
				}
				else if (this.mode == ScreenshotDialogMode.ExtractText)
				{
					frameworkElement = this.ExtractTextToolbar;
				}
				else if (this.mode == ScreenshotDialogMode.Ocr)
				{
					frameworkElement = this.OcrToolbar;
				}
				else if (this.mode == ScreenshotDialogMode.CropPage)
				{
					frameworkElement = this.CropPageToolbar;
				}
				if (frameworkElement != null)
				{
					frameworkElement.IsEnabled = true;
					frameworkElement.IsHitTestVisible = true;
					frameworkElement.Opacity = 1.0;
					return;
				}
			}
			else
			{
				this.ImageToolbar.IsEnabled = false;
				this.ImageToolbar.IsHitTestVisible = false;
				this.ImageToolbar.Opacity = 0.0;
				this.ExtractTextToolbar.IsEnabled = false;
				this.ExtractTextToolbar.IsHitTestVisible = false;
				this.ExtractTextToolbar.Opacity = 0.0;
				this.OcrToolbar.IsEnabled = false;
				this.OcrToolbar.IsHitTestVisible = false;
				this.OcrToolbar.Opacity = 0.0;
				this.CropPageToolbar.IsEnabled = false;
				this.CropPageToolbar.IsHitTestVisible = false;
				this.CropPageToolbar.Opacity = 0.0;
			}
		}

		// Token: 0x06001DED RID: 7661 RVA: 0x00082D44 File Offset: 0x00080F44
		public void Cancel()
		{
			if (this.DragResizeView.Opacity != 0.0)
			{
				this.CancelCore();
				return;
			}
			if (this.mode == ScreenshotDialogMode.CropPage)
			{
				this.ResetPage();
				this.ResetCropSize(this.pageIdx);
			}
			this.Close(null);
		}

		// Token: 0x06001DEE RID: 7662 RVA: 0x00082D90 File Offset: 0x00080F90
		public async Task CompleteExtractTextAsync(bool ocr)
		{
			if (this.pageIdx != -1 && this.startPt != default(FS_POINTF) && this.curPt != default(FS_POINTF))
			{
				PdfViewer pdfViewer = this.viewer;
				PdfDocument document = ((pdfViewer != null) ? pdfViewer.Document : null);
				if (document != null)
				{
					WriteableBitmap writeableBitmap = await this.GetScaledPageImageAsync();
					WriteableBitmap bitmap = writeableBitmap;
					PdfPage page = document.Pages[this.pageIdx];
					WriteableBitmap writeableBitmap2 = await this.RotateImageAsync(bitmap, page.Rotation);
					FS_POINTF fs_POINTF = this.startPt;
					FS_POINTF fs_POINTF2 = this.curPt;
					FS_RECTF fs_RECTF = new FS_RECTF(Math.Min(fs_POINTF.X, fs_POINTF2.X), Math.Max(fs_POINTF.Y, fs_POINTF2.Y), Math.Max(fs_POINTF.X, fs_POINTF2.X), Math.Min(fs_POINTF.Y, fs_POINTF2.Y));
					string boundedText = page.Text.GetBoundedText(fs_RECTF);
					Rect rect;
					if (this.viewer.TryGetClientRect(this.pageIdx, fs_RECTF, out rect))
					{
						this.Close(ScreenshotDialogResult.CreateExtractedText(this.pageIdx, boundedText, bitmap, writeableBitmap2, fs_RECTF, rect, ocr));
						return;
					}
					bitmap = null;
					page = null;
				}
				document = null;
			}
			this.Close(null);
		}

		// Token: 0x06001DEF RID: 7663 RVA: 0x00082DDC File Offset: 0x00080FDC
		public async Task CompleteImageAsync()
		{
			if (this.pageIdx >= 0)
			{
				WriteableBitmap writeableBitmap = await this.GetScaledPageImageAsync();
				WriteableBitmap bitmap = writeableBitmap;
				PdfViewer pdfViewer = this.viewer;
				PdfPage pdfPage = ((pdfViewer != null) ? pdfViewer.Document : null).Pages[this.pageIdx];
				WriteableBitmap writeableBitmap2 = await this.RotateImageAsync(bitmap, pdfPage.Rotation);
				WriteableBitmap writeableBitmap3 = this.AttachDrawControl2FinalImage(writeableBitmap2);
				if (writeableBitmap3 != null)
				{
					FS_POINTF fs_POINTF = this.startPt;
					FS_POINTF fs_POINTF2 = this.curPt;
					FS_RECTF fs_RECTF = new FS_RECTF(Math.Min(fs_POINTF.X, fs_POINTF2.X), Math.Max(fs_POINTF.Y, fs_POINTF2.Y), Math.Max(fs_POINTF.X, fs_POINTF2.X), Math.Min(fs_POINTF.Y, fs_POINTF2.Y));
					Rect rect;
					if (this.viewer.TryGetClientRect(this.pageIdx, fs_RECTF, out rect))
					{
						try
						{
							global::System.Windows.Forms.Clipboard.SetImage(this.BitmapSourceToImage(writeableBitmap3));
						}
						catch
						{
						}
						this.Close(ScreenshotDialogResult.CreateImage(this.pageIdx, bitmap, writeableBitmap3, fs_RECTF, rect));
						return;
					}
				}
				this.Close(null);
			}
		}

		// Token: 0x06001DF0 RID: 7664 RVA: 0x00082E20 File Offset: 0x00081020
		public async Task CopyToClipboardAsync()
		{
			WriteableBitmap writeableBitmap = await this.GetScaledPageImageAsync();
			PdfViewer pdfViewer = this.viewer;
			PdfPage pdfPage = ((pdfViewer != null) ? pdfViewer.Document : null).Pages[this.pageIdx];
			WriteableBitmap writeableBitmap2 = await this.RotateImageAsync(writeableBitmap, pdfPage.Rotation);
			WriteableBitmap writeableBitmap3 = this.AttachDrawControl2FinalImage(writeableBitmap2);
			if (writeableBitmap3 != null)
			{
				try
				{
					global::System.Windows.Forms.Clipboard.SetImage(this.BitmapSourceToImage(writeableBitmap3));
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x06001DF1 RID: 7665 RVA: 0x00082E64 File Offset: 0x00081064
		public async Task EditImageAsync()
		{
			WriteableBitmap writeableBitmap = await this.GetScaledPageImageAsync();
			PdfViewer pdfViewer = this.viewer;
			PdfPage pdfPage = ((pdfViewer != null) ? pdfViewer.Document : null).Pages[this.pageIdx];
			WriteableBitmap writeableBitmap2 = await this.RotateImageAsync(writeableBitmap, pdfPage.Rotation);
			if (writeableBitmap2 != null)
			{
				string temporaryPath = UtilManager.GetTemporaryPath();
				string text = "";
				do
				{
					text = global::System.IO.Path.Combine(temporaryPath, Guid.NewGuid().ToString("N").ToUpperInvariant() + ".png");
				}
				while (File.Exists(text));
				using (FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
					BitmapFrame bitmapFrame = BitmapFrame.Create(writeableBitmap2);
					pngBitmapEncoder.Frames.Add(bitmapFrame);
					pngBitmapEncoder.Save(fileStream);
				}
				try
				{
					Process.Start("mspaint", "\"" + text + "\"");
				}
				catch (Exception ex)
				{
					Log.WriteLog(ex.ToString());
				}
			}
			this.Close(null);
		}

		// Token: 0x06001DF2 RID: 7666 RVA: 0x00082EA8 File Offset: 0x000810A8
		public async Task SaveImageAsync()
		{
			WriteableBitmap writeableBitmap = await this.GetScaledPageImageAsync();
			PdfViewer pdfViewer = this.viewer;
			PdfPage pdfPage = ((pdfViewer != null) ? pdfViewer.Document : null).Pages[this.pageIdx];
			WriteableBitmap writeableBitmap2 = await this.RotateImageAsync(writeableBitmap, pdfPage.Rotation);
			WriteableBitmap writeableBitmap3 = this.AttachDrawControl2FinalImage(writeableBitmap2);
			if (writeableBitmap3 != null)
			{
				Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
				saveFileDialog.AddExtension = true;
				saveFileDialog.Filter = "png|*.png";
				saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				saveFileDialog.FileName = "PdfImage.png";
				if (saveFileDialog.ShowDialog().GetValueOrDefault())
				{
					string fileName = saveFileDialog.FileName;
					try
					{
						if (File.Exists(fileName))
						{
							try
							{
								File.Delete(fileName);
							}
							catch
							{
								this.Close(null);
								return;
							}
						}
						using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
						{
							PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
							BitmapFrame bitmapFrame = BitmapFrame.Create(writeableBitmap3);
							pngBitmapEncoder.Frames.Add(bitmapFrame);
							pngBitmapEncoder.Save(fileStream);
						}
						await ExplorerUtils.SelectItemInExplorerAsync(fileName, default(CancellationToken));
						this.Close(null);
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x06001DF3 RID: 7667 RVA: 0x00082EEC File Offset: 0x000810EC
		private async Task<WriteableBitmap> RotateImageAsync(WriteableBitmap source, PageRotate rotate)
		{
			WriteableBitmap writeableBitmap;
			if (source == null || rotate == PageRotate.Normal)
			{
				writeableBitmap = source;
			}
			else
			{
				Rotation rotation = Rotation.Rotate0;
				switch (rotate)
				{
				case PageRotate.Rotate90:
					rotation = Rotation.Rotate90;
					goto IL_00B7;
				case PageRotate.Rotate180:
					rotation = Rotation.Rotate180;
					goto IL_00B7;
				case PageRotate.Rotate270:
					rotation = Rotation.Rotate270;
					goto IL_00B7;
				}
				rotation = Rotation.Rotate0;
				IL_00B7:
				BitmapSource result = null;
				await base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
				{
					using (Bitmap bitmap = new Bitmap(source.PixelWidth, source.PixelHeight, global::System.Drawing.Imaging.PixelFormat.Format32bppArgb))
					{
						BitmapData bitmapData = bitmap.LockBits(new global::System.Drawing.Rectangle(0, 0, source.PixelWidth, source.PixelHeight), ImageLockMode.WriteOnly, global::System.Drawing.Imaging.PixelFormat.Format32bppArgb);
						source.CopyPixels(new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight), bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);
						bitmap.UnlockBits(bitmapData);
						BitmapSizeOptions bitmapSizeOptions = BitmapSizeOptions.FromRotation(rotation);
						Int32Rect int32Rect = new Int32Rect(0, 0, bitmap.Width, bitmap.Height);
						IntPtr hbitmap = bitmap.GetHbitmap();
						try
						{
							result = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, int32Rect, bitmapSizeOptions);
						}
						finally
						{
							try
							{
								if (hbitmap != IntPtr.Zero)
								{
									DrawUtils.DeleteObject(hbitmap);
								}
							}
							catch
							{
							}
						}
					}
				}));
				if (result != null)
				{
					writeableBitmap = new WriteableBitmap(result);
				}
				else
				{
					writeableBitmap = null;
				}
			}
			return writeableBitmap;
		}

		// Token: 0x06001DF4 RID: 7668 RVA: 0x00082F40 File Offset: 0x00081140
		private async Task<WriteableBitmap> GetScaledPageImageAsync()
		{
			PdfViewer pdfViewer = this.viewer;
			PdfDocument pdfDocument = ((pdfViewer != null) ? pdfViewer.Document : null);
			WriteableBitmap writeableBitmap;
			if (pdfDocument != null)
			{
				FS_POINTF fs_POINTF = this.startPt;
				FS_POINTF fs_POINTF2 = this.curPt;
				FS_RECTF fs_RECTF = new FS_RECTF(Math.Min(fs_POINTF.X, fs_POINTF2.X), Math.Max(fs_POINTF.Y, fs_POINTF2.Y), Math.Max(fs_POINTF.X, fs_POINTF2.X), Math.Min(fs_POINTF.Y, fs_POINTF2.Y));
				Rect rect;
				if (!this.viewer.TryGetClientRect(this.pageIdx, fs_RECTF, out rect))
				{
					writeableBitmap = null;
				}
				else
				{
					PdfPage pdfPage = pdfDocument.Pages[this.pageIdx];
					FS_SIZEF effectiveSize = pdfPage.GetEffectiveSize(PageRotate.Normal, false);
					float num = effectiveSize.Width * 96f / 72f * 2f;
					float num2 = fs_RECTF.Width * num / effectiveSize.Width;
					double num3 = rect.Width;
					double num4 = rect.Height;
					if (pdfPage.Rotation == PageRotate.Rotate90 || pdfPage.Rotation == PageRotate.Rotate270)
					{
						double num5 = num4;
						num4 = num3;
						num3 = num5;
					}
					if (num3 < (double)num2)
					{
						double num6 = (double)num2 / num3;
						num3 *= num6;
						num4 *= num6;
					}
					writeableBitmap = await ScreenshotDialog.GetPageImageAsync(num3, num4, pdfPage, new FS_RECTF?(fs_RECTF));
				}
			}
			else
			{
				writeableBitmap = null;
			}
			return writeableBitmap;
		}

		// Token: 0x06001DF5 RID: 7669 RVA: 0x00082F84 File Offset: 0x00081184
		public void UpdateSeleteBounds(bool IssizeControl = false)
		{
			List<global::System.Windows.Shapes.Rectangle> list = this.DraggerParent.Children.OfType<global::System.Windows.Shapes.Rectangle>().ToList<global::System.Windows.Shapes.Rectangle>();
			int[] applypageIndex = this.CropPageToolbar.ApplypageIndex;
			if (list != null && applypageIndex != null && (list.Count<global::System.Windows.Shapes.Rectangle>() == applypageIndex.Length - 1 || list.Count<global::System.Windows.Shapes.Rectangle>() == 10 || list.Count<global::System.Windows.Shapes.Rectangle>() == 11))
			{
				List<int> list2 = applypageIndex.ToList<int>();
				int num = 0;
				foreach (int num2 in list2)
				{
					if (num2 != this.pageIdx && num < list.Count<global::System.Windows.Shapes.Rectangle>())
					{
						PdfPage pdfPage = this.viewer.Document.Pages[num2];
						PdfPage pdfPage2 = this.viewer.Document.Pages[this.pageIdx];
						float num3 = 1f;
						float num4 = 1f;
						if (!IssizeControl)
						{
							if (((pdfPage.Rotation == PageRotate.Rotate90 || pdfPage.Rotation == PageRotate.Rotate270) && (pdfPage2.Rotation == PageRotate.Rotate180 || pdfPage2.Rotation == PageRotate.Normal)) || ((pdfPage2.Rotation == PageRotate.Rotate270 || pdfPage2.Rotation == PageRotate.Rotate90) && (pdfPage.Rotation == PageRotate.Normal || pdfPage.Rotation == PageRotate.Rotate180)))
							{
								num3 = pdfPage.MediaBox.Width / pdfPage2.MediaBox.Width;
								num4 = pdfPage.MediaBox.Height / pdfPage2.MediaBox.Width;
							}
							else
							{
								num3 = pdfPage.MediaBox.Width / pdfPage2.MediaBox.Width;
								num4 = pdfPage.MediaBox.Height / pdfPage2.MediaBox.Height;
							}
						}
						FS_POINTF fs_POINTF = default(FS_POINTF);
						FS_POINTF fs_POINTF2 = default(FS_POINTF);
						FS_RECTF fs_RECTF = default(FS_RECTF);
						fs_RECTF = pdfPage2.MediaBox;
						fs_POINTF.X = fs_RECTF.right;
						fs_POINTF.Y = fs_RECTF.bottom;
						fs_POINTF2.Y = fs_RECTF.top;
						fs_POINTF2.X = fs_RECTF.left;
						global::System.Windows.Point point;
						global::System.Windows.Point point2;
						global::System.Windows.Point point3;
						global::System.Windows.Point point4;
						if (this.viewer.TryGetClientPoint(num2, this.startPt.ToPoint(), out point) && this.viewer.TryGetClientPoint(num2, this.curPt.ToPoint(), out point2) && this.viewer.TryGetClientPoint(num2, fs_POINTF2.ToPoint(), out point3) && this.viewer.TryGetClientPoint(num2, fs_POINTF.ToPoint(), out point4))
						{
							global::System.Windows.Point point5 = new global::System.Windows.Point(Math.Min(point.X, point2.X), Math.Min(point.Y, point2.Y));
							global::System.Windows.Point point6 = new global::System.Windows.Point(Math.Max(point.X, point2.X), Math.Max(point.Y, point2.Y));
							global::System.Windows.Point point7 = new global::System.Windows.Point(Math.Min(point3.X, point4.X), Math.Min(point3.Y, point4.Y));
							global::System.Windows.Point point8 = new global::System.Windows.Point(Math.Max(point3.X, point4.X), Math.Max(point3.Y, point4.Y));
							Rect rect = new Rect(point5, point6);
							Rect rect2 = new Rect(point7, point8);
							FS_RECTF fs_RECTF2 = default(FS_RECTF);
							fs_RECTF2 = pdfPage.MediaBox;
							Rect rect3;
							this.viewer.TryGetClientRect(num2, fs_RECTF2, out rect3);
							Canvas.SetLeft(list[num], (rect.Left - rect2.Left) * (double)num3 + rect3.Left);
							Canvas.SetTop(list[num], (rect.Top - rect2.Top) * (double)num4 + rect3.Top);
							list[num].Width = Math.Abs(rect.Width * (double)num3);
							list[num].Height = Math.Abs(rect.Height * (double)num4);
							list[num].Opacity = 0.0;
							(((GeometryGroup)this.myPath.Data).Children[num + 2] as RectangleGeometry).Rect = new Rect((rect.Left - rect2.Left) * (double)num3 + rect3.Left, (rect.Top - rect2.Top) * (double)num4 + rect3.Top, list[num].Width, list[num].Height);
							num++;
						}
					}
				}
			}
		}

		// Token: 0x06001DF6 RID: 7670 RVA: 0x0008344C File Offset: 0x0008164C
		public void CreateSeleteBounds(bool IssizeControl = false)
		{
			int[] applypageIndex = this.CropPageToolbar.ApplypageIndex;
			List<global::System.Windows.Shapes.Rectangle> list = this.DraggerParent.Children.OfType<global::System.Windows.Shapes.Rectangle>().ToList<global::System.Windows.Shapes.Rectangle>();
			List<RectangleGeometry> list2 = ((GeometryGroup)this.myPath.Data).Children.OfType<RectangleGeometry>().ToList<RectangleGeometry>();
			if (list.Count<global::System.Windows.Shapes.Rectangle>() > 0)
			{
				foreach (global::System.Windows.Shapes.Rectangle rectangle in list)
				{
					if (rectangle.IsHitTestVisible)
					{
						this.DraggerParent.Children.Remove(rectangle);
					}
				}
			}
			if (list2.Count<RectangleGeometry>() > 2)
			{
				for (int i = 2; i < list2.Count<RectangleGeometry>(); i++)
				{
					((GeometryGroup)this.myPath.Data).Children.RemoveAt(2);
				}
			}
			if (applypageIndex != null && applypageIndex.Length != 0)
			{
				foreach (int num in applypageIndex.ToList<int>())
				{
					if (num != this.pageIdx)
					{
						global::System.Windows.Shapes.Rectangle rectangle2 = new global::System.Windows.Shapes.Rectangle();
						FS_POINTF fs_POINTF = default(FS_POINTF);
						FS_POINTF fs_POINTF2 = default(FS_POINTF);
						PdfPage pdfPage = this.viewer.Document.Pages[num];
						FS_RECTF? fs_RECTF = null;
						if (this.BeforeCropBox != null && PageHeaderFooterUtils.PdfPointToCm((double)Math.Abs(this.BeforeCropBox.Value.Width)) >= 0.15 && PageHeaderFooterUtils.PdfPointToCm((double)Math.Abs(this.BeforeCropBox.Value.Width)) >= 0.15)
						{
							fs_RECTF = new FS_RECTF?(this.BeforeCropBox.Value);
						}
						if (fs_RECTF == null)
						{
							fs_RECTF = new FS_RECTF?(pdfPage.MediaBox);
						}
						fs_POINTF2.X = fs_RECTF.Value.right;
						fs_POINTF2.Y = fs_RECTF.Value.bottom;
						fs_POINTF.Y = fs_RECTF.Value.top;
						fs_POINTF.X = fs_RECTF.Value.left;
						global::System.Windows.Point point;
						global::System.Windows.Point point2;
						if (this.viewer.TryGetClientPoint(num, fs_POINTF.ToPoint(), out point) && this.viewer.TryGetClientPoint(num, fs_POINTF2.ToPoint(), out point2))
						{
							global::System.Windows.Point point3 = new global::System.Windows.Point(Math.Min(point.X, point2.X), Math.Min(point.Y, point2.Y));
							global::System.Windows.Point point4 = new global::System.Windows.Point(Math.Max(point.X, point2.X), Math.Max(point.Y, point2.Y));
							Rect rect = new Rect(point3, point4);
							rectangle2.Fill = new SolidColorBrush(Colors.Transparent);
							rectangle2.Stroke = new SolidColorBrush(Colors.Black);
							rectangle2.StrokeDashArray = new DoubleCollection { 2.0, 4.0 };
							rectangle2.StrokeThickness = 1.0;
							this.DraggerParent.Children.Add(rectangle2);
							Canvas.SetLeft(rectangle2, rect.Left);
							Canvas.SetTop(rectangle2, rect.Top);
							rectangle2.Width = rect.Width;
							rectangle2.Height = rect.Height;
							RectangleGeometry rectangleGeometry = new RectangleGeometry(rect);
							((GeometryGroup)this.myPath.Data).Children.Add(rectangleGeometry);
						}
					}
				}
			}
			this.UpdateSeleteBounds(IssizeControl);
		}

		// Token: 0x06001DF7 RID: 7671 RVA: 0x00083834 File Offset: 0x00081A34
		public async Task<bool> GetSeleteBounds(bool IsZeroMargin)
		{
			int[] seleteidex = this.CropPageToolbar.ApplypageIndex;
			List<global::System.Windows.Shapes.Rectangle> list = this.DraggerParent.Children.OfType<global::System.Windows.Shapes.Rectangle>().ToList<global::System.Windows.Shapes.Rectangle>();
			List<RectangleGeometry> list2 = ((GeometryGroup)this.myPath.Data).Children.OfType<RectangleGeometry>().ToList<RectangleGeometry>();
			if (list.Count<global::System.Windows.Shapes.Rectangle>() > 0)
			{
				foreach (global::System.Windows.Shapes.Rectangle rectangle in list)
				{
					if (rectangle.IsHitTestVisible)
					{
						this.DraggerParent.Children.Remove(rectangle);
					}
				}
			}
			if (list2.Count<RectangleGeometry>() > 2)
			{
				for (int i = 2; i < list2.Count<RectangleGeometry>(); i++)
				{
					((GeometryGroup)this.myPath.Data).Children.RemoveAt(2);
				}
			}
			if (seleteidex != null && seleteidex.Length != 0)
			{
				await base.Dispatcher.BeginInvoke(new Action(delegate
				{
					if (IsZeroMargin)
					{
						foreach (int num in seleteidex)
						{
							if (num != this.pageIdx)
							{
								global::System.Windows.Shapes.Rectangle rectangle2 = new global::System.Windows.Shapes.Rectangle();
								PdfPage pdfPage = this.viewer.Document.Pages[num];
								List<FS_RECTF> list3 = new List<FS_RECTF>();
								foreach (PdfPageObject pdfPageObject in this.viewer.Document.Pages[num].PageObjects)
								{
									list3.Add(pdfPageObject.BoundingBox);
								}
								FS_RECTF mediaBox = this.viewer.Document.Pages[num].MediaBox;
								float num2 = mediaBox.Width;
								float num3 = 0f;
								float num4 = 0f;
								float num5 = mediaBox.Height;
								for (int k = 0; k < list3.Count; k++)
								{
									num2 = Math.Min(list3[k].left, num2);
									num3 = Math.Max(list3[k].top, num3);
									num4 = Math.Max(list3[k].right, num4);
									num5 = Math.Min(list3[k].bottom, num5);
								}
								FS_POINTF fs_POINTF = new FS_POINTF(num2, num3);
								FS_POINTF fs_POINTF2 = new FS_POINTF(num4, num5);
								global::System.Windows.Point point;
								global::System.Windows.Point point2;
								if (this.viewer.TryGetClientPoint(num, fs_POINTF.ToPoint(), out point) && this.viewer.TryGetClientPoint(num, fs_POINTF2.ToPoint(), out point2))
								{
									global::System.Windows.Point point3 = new global::System.Windows.Point(Math.Min(point.X, point2.X), Math.Min(point.Y, point2.Y));
									global::System.Windows.Point point4 = new global::System.Windows.Point(Math.Max(point.X, point2.X), Math.Max(point.Y, point2.Y));
									Rect rect = new Rect(point3, point4);
									rectangle2.Fill = new SolidColorBrush(Colors.Transparent);
									rectangle2.Stroke = new SolidColorBrush(Colors.Black);
									rectangle2.StrokeDashArray = new DoubleCollection { 2.0, 4.0 };
									rectangle2.StrokeThickness = 1.0;
									this.DraggerParent.Children.Add(rectangle2);
									Canvas.SetLeft(rectangle2, rect.Left);
									Canvas.SetTop(rectangle2, rect.Top);
									rectangle2.Width = rect.Width;
									rectangle2.Height = rect.Height;
									rectangle2.Opacity = 0.0;
									RectangleGeometry rectangleGeometry = new RectangleGeometry(rect);
									((GeometryGroup)this.myPath.Data).Children.Add(rectangleGeometry);
								}
							}
						}
						return;
					}
					foreach (int num6 in seleteidex)
					{
						if (num6 != this.pageIdx)
						{
							global::System.Windows.Shapes.Rectangle rectangle3 = new global::System.Windows.Shapes.Rectangle();
							PdfPage pdfPage2 = this.viewer.Document.Pages[num6];
							PdfPage pdfPage3 = this.viewer.Document.Pages[this.pageIdx];
							if (((pdfPage2.Rotation == PageRotate.Rotate90 || pdfPage2.Rotation == PageRotate.Rotate270) && (pdfPage3.Rotation == PageRotate.Rotate180 || pdfPage3.Rotation == PageRotate.Normal)) || ((pdfPage3.Rotation == PageRotate.Rotate270 || pdfPage3.Rotation == PageRotate.Rotate90) && (pdfPage2.Rotation == PageRotate.Normal || pdfPage2.Rotation == PageRotate.Rotate180)))
							{
								float num7 = pdfPage2.MediaBox.Width / pdfPage3.MediaBox.Width;
								float num8 = pdfPage2.MediaBox.Height / pdfPage3.MediaBox.Width;
							}
							else
							{
								float num9 = pdfPage2.MediaBox.Width / pdfPage3.MediaBox.Width;
								float num10 = pdfPage2.MediaBox.Height / pdfPage3.MediaBox.Height;
							}
							FS_POINTF fs_POINTF3 = default(FS_POINTF);
							FS_POINTF fs_POINTF4 = default(FS_POINTF);
							FS_RECTF fs_RECTF = default(FS_RECTF);
							fs_RECTF = pdfPage2.MediaBox;
							float num11 = (float)this.CropPageToolbar.PageMargin.Left;
							float num12 = fs_RECTF.Height - (float)this.CropPageToolbar.PageMargin.Top;
							float num13 = fs_RECTF.Width - (float)this.CropPageToolbar.PageMargin.Right;
							float num14 = (float)this.CropPageToolbar.PageMargin.Bottom;
							if (num11 > fs_RECTF.Width || num12 > fs_RECTF.Height || num13 > fs_RECTF.Width || num14 > fs_RECTF.Height || num12 < 0f || num13 < 0f)
							{
								FS_POINTF fs_POINTF5 = default(FS_POINTF);
								FS_POINTF fs_POINTF6 = default(FS_POINTF);
								fs_POINTF5.X = fs_RECTF.right;
								fs_POINTF5.Y = fs_RECTF.bottom;
								fs_POINTF6.Y = fs_RECTF.top;
								fs_POINTF6.X = fs_RECTF.left;
								global::System.Windows.Point point5;
								global::System.Windows.Point point6;
								if (this.viewer.TryGetClientPoint(num6, fs_POINTF6.ToPoint(), out point5) && this.viewer.TryGetClientPoint(num6, fs_POINTF5.ToPoint(), out point6))
								{
									global::System.Windows.Point point7 = new global::System.Windows.Point(Math.Min(point5.X, point6.X), Math.Min(point5.Y, point6.Y));
									global::System.Windows.Point point8 = new global::System.Windows.Point(Math.Max(point5.X, point6.X), Math.Max(point5.Y, point6.Y));
									Rect rect2 = new Rect(point7, point8);
									FS_RECTF fs_RECTF2 = default(FS_RECTF);
									fs_RECTF2 = pdfPage2.MediaBox;
									rectangle3.Fill = new SolidColorBrush(Colors.Transparent);
									rectangle3.Stroke = new SolidColorBrush(Colors.Black);
									rectangle3.StrokeDashArray = new DoubleCollection { 2.0, 4.0 };
									rectangle3.StrokeThickness = 1.0;
									this.DraggerParent.Children.Add(rectangle3);
									Rect rect3;
									this.viewer.TryGetClientRect(num6, fs_RECTF2, out rect3);
									Canvas.SetLeft(rectangle3, rect2.Left);
									Canvas.SetTop(rectangle3, rect2.Top);
									rectangle3.Width = rect2.Width;
									rectangle3.Height = rect2.Height;
									RectangleGeometry rectangleGeometry2 = new RectangleGeometry(rect2);
									((GeometryGroup)this.myPath.Data).Children.Add(rectangleGeometry2);
									goto IL_08D7;
								}
							}
							fs_POINTF3.X = num13;
							fs_POINTF3.Y = num14;
							fs_POINTF4.Y = num12;
							fs_POINTF4.X = num11;
							global::System.Windows.Point point9;
							global::System.Windows.Point point10;
							if (this.viewer.TryGetClientPoint(num6, fs_POINTF4.ToPoint(), out point9) && this.viewer.TryGetClientPoint(num6, fs_POINTF3.ToPoint(), out point10))
							{
								global::System.Windows.Point point11 = new global::System.Windows.Point(Math.Min(point9.X, point10.X), Math.Min(point9.Y, point10.Y));
								global::System.Windows.Point point12 = new global::System.Windows.Point(Math.Max(point9.X, point10.X), Math.Max(point9.Y, point10.Y));
								Rect rect4 = new Rect(point11, point12);
								FS_RECTF fs_RECTF3 = default(FS_RECTF);
								fs_RECTF3 = pdfPage2.MediaBox;
								rectangle3.Fill = new SolidColorBrush(Colors.Transparent);
								rectangle3.Stroke = new SolidColorBrush(Colors.Black);
								rectangle3.StrokeDashArray = new DoubleCollection { 2.0, 4.0 };
								rectangle3.StrokeThickness = 1.0;
								this.DraggerParent.Children.Add(rectangle3);
								Rect rect5;
								this.viewer.TryGetClientRect(num6, fs_RECTF3, out rect5);
								Canvas.SetLeft(rectangle3, rect4.Left);
								Canvas.SetTop(rectangle3, rect4.Top);
								rectangle3.Width = rect4.Width;
								rectangle3.Height = rect4.Height;
								RectangleGeometry rectangleGeometry3 = new RectangleGeometry(rect4);
								((GeometryGroup)this.myPath.Data).Children.Add(rectangleGeometry3);
							}
						}
						IL_08D7:;
					}
				}), Array.Empty<object>());
			}
			return false;
		}

		// Token: 0x06001DF8 RID: 7672 RVA: 0x00083880 File Offset: 0x00081A80
		public async void CompleteCropPageAsync(int[] pageIndexs = null, bool IsZeroMargin = false)
		{
			FS_RECTF[] BeforfS_RECTFs = new FS_RECTF[pageIndexs.Length];
			FS_RECTF[] SelectedfS_RECTFs = new FS_RECTF[pageIndexs.Length];
			Rect[] SelectedClientRects = new Rect[pageIndexs.Length];
			if (pageIndexs.Length == 1 && pageIndexs[0] == this.pageIdx)
			{
				FS_POINTF fs_POINTF = this.startPt;
				FS_POINTF fs_POINTF2 = this.curPt;
				FS_RECTF fs_RECTF = new FS_RECTF(Math.Min(fs_POINTF.X, fs_POINTF2.X), Math.Max(fs_POINTF.Y, fs_POINTF2.Y), Math.Max(fs_POINTF.X, fs_POINTF2.X), Math.Min(fs_POINTF.Y, fs_POINTF2.Y));
				if (PageHeaderFooterUtils.PdfPointToCm((double)Math.Abs(fs_RECTF.Width)) < 0.5 || PageHeaderFooterUtils.PdfPointToCm((double)Math.Abs(fs_RECTF.Height)) < 0.5)
				{
					global::System.Windows.MessageBox.Show(pdfeditor.Properties.Resources.MainCropPageSelectLittleAreaNote, UtilManager.GetProductName());
				}
				else
				{
					Rect rect;
					if (this.viewer.TryGetClientRect(this.pageIdx, fs_RECTF, out rect))
					{
						SelectedfS_RECTFs.SetValue(fs_RECTF, 0);
					}
					BeforfS_RECTFs.SetValue(this.BeforeCropBox, 0);
					SelectedClientRects.SetValue(rect, 0);
					this.Close(ScreenshotDialogResult.GetCropBoxs(this.pageIdx, pageIndexs, BeforfS_RECTFs, SelectedfS_RECTFs, SelectedClientRects));
				}
			}
			else
			{
				if (pageIndexs.Length >= 1)
				{
					await this.GetSeleteBounds(IsZeroMargin);
					List<global::System.Windows.Shapes.Rectangle> list = this.DraggerParent.Children.OfType<global::System.Windows.Shapes.Rectangle>().ToList<global::System.Windows.Shapes.Rectangle>();
					int num = 0;
					for (int i = 0; i < pageIndexs.Length; i++)
					{
						if (pageIndexs[i] == this.pageIdx)
						{
							FS_POINTF fs_POINTF3 = this.startPt;
							FS_POINTF fs_POINTF4 = this.curPt;
							FS_RECTF fs_RECTF2 = default(FS_RECTF);
							fs_RECTF2 = this.viewer.Document.Pages[this.pageIdx].MediaBox;
							float num2 = (float)this.CropPageToolbar.PageMargin.Left;
							float num3 = fs_RECTF2.Height - (float)this.CropPageToolbar.PageMargin.Top;
							float num4 = fs_RECTF2.Width - (float)this.CropPageToolbar.PageMargin.Right;
							float num5 = (float)this.CropPageToolbar.PageMargin.Bottom;
							fs_POINTF3.X = num2;
							fs_POINTF3.Y = num3;
							fs_POINTF4.X = num4;
							fs_POINTF4.Y = num5;
							FS_RECTF fs_RECTF3 = new FS_RECTF(Math.Min(fs_POINTF3.X, fs_POINTF4.X), Math.Max(fs_POINTF3.Y, fs_POINTF4.Y), Math.Max(fs_POINTF3.X, fs_POINTF4.X), Math.Min(fs_POINTF3.Y, fs_POINTF4.Y));
							if (PageHeaderFooterUtils.PdfPointToCm((double)Math.Abs(fs_RECTF3.Width)) < 0.5 || PageHeaderFooterUtils.PdfPointToCm((double)Math.Abs(fs_RECTF3.Height)) < 0.5)
							{
								global::System.Windows.MessageBox.Show(pdfeditor.Properties.Resources.MainCropPageSelectLittleAreaNote, UtilManager.GetProductName());
								if (this.DraggerParent.Children.Count > 7)
								{
									for (int j = 7; j <= this.DraggerParent.Children.Count - 1; j++)
									{
										this.DraggerParent.Children.RemoveAt(j);
									}
								}
								if (((this.DraggerParent.Children[0] as global::System.Windows.Shapes.Path).Data as GeometryGroup).Children.Count > 2)
								{
									for (int k = 2; k <= ((this.DraggerParent.Children[0] as global::System.Windows.Shapes.Path).Data as GeometryGroup).Children.Count - 1; k++)
									{
										((this.DraggerParent.Children[0] as global::System.Windows.Shapes.Path).Data as GeometryGroup).Children.RemoveAt(k);
									}
								}
								return;
							}
							Rect rect2;
							if (this.viewer.TryGetClientRect(this.pageIdx, fs_RECTF3, out rect2))
							{
								SelectedfS_RECTFs.SetValue(fs_RECTF3, i);
							}
							BeforfS_RECTFs.SetValue(this.BeforeCropBox, i);
							SelectedClientRects.SetValue(rect2, i);
						}
						else
						{
							PdfPage pdfPage = this.viewer.Document.Pages[pageIndexs[i]];
							double top = Canvas.GetTop(list[num]);
							Rect rect3 = new Rect(Canvas.GetLeft(list[num]), top, list[num].Width, list[num].Height);
							num++;
							if (PageHeaderFooterUtils.PdfPointToCm(Math.Abs(rect3.Width)) < 0.5 || PageHeaderFooterUtils.PdfPointToCm(Math.Abs(rect3.Height)) < 0.5)
							{
								global::System.Windows.MessageBox.Show(pdfeditor.Properties.Resources.MainCropPageSelectLittleAreaNote, UtilManager.GetProductName());
								if (this.DraggerParent.Children.Count > 7)
								{
									for (int l = 7; l <= this.DraggerParent.Children.Count - 1; l++)
									{
										this.DraggerParent.Children.RemoveAt(l);
									}
								}
								if (((this.DraggerParent.Children[0] as global::System.Windows.Shapes.Path).Data as GeometryGroup).Children.Count > 2)
								{
									for (int m = 2; m <= ((this.DraggerParent.Children[0] as global::System.Windows.Shapes.Path).Data as GeometryGroup).Children.Count - 1; m++)
									{
										((this.DraggerParent.Children[0] as global::System.Windows.Shapes.Path).Data as GeometryGroup).Children.RemoveAt(m);
									}
								}
								return;
							}
							FS_RECTF fs_RECTF4 = default(FS_RECTF);
							BeforfS_RECTFs.SetValue((!pdfPage.Dictionary.ContainsKey("CropBox")) ? pdfPage.GetEffectiveBox(PageRotate.Normal, false) : pdfPage.CropBox, i);
							SelectedClientRects.SetValue(rect3, i);
							FS_RECTF fs_RECTF5;
							this.viewer.TryGetPageRect(pageIndexs[i], rect3, out fs_RECTF5);
							SelectedfS_RECTFs.SetValue(fs_RECTF5, i);
						}
					}
					this.Close(ScreenshotDialogResult.GetCropBoxs(this.pageIdx, pageIndexs, BeforfS_RECTFs, SelectedfS_RECTFs, SelectedClientRects));
				}
				this.Close(null);
			}
		}

		// Token: 0x06001DF9 RID: 7673 RVA: 0x000838C8 File Offset: 0x00081AC8
		public static async Task<WriteableBitmap> GetPageImageAsync(double width, double height, PdfPage page, FS_RECTF? pageRect = null)
		{
			FS_SIZEF effectiveSize = page.GetEffectiveSize(PageRotate.Normal, false);
			FS_RECTF fs_RECTF = pageRect ?? page.GetEffectiveBox(PageRotate.Normal, false);
			fs_RECTF = PdfLocationUtils.MediaBoxRectToEffectiveBox(page, fs_RECTF);
			WriteableBitmap writeableBitmap;
			using (PdfBitmap pdfBitmap = new PdfBitmap((int)width, (int)height, BitmapFormats.FXDIB_Argb))
			{
				if (pdfBitmap == null)
				{
					throw new ArgumentNullException("pdfBitmap");
				}
				if (page == null)
				{
					throw new ArgumentNullException("page");
				}
				int width2 = pdfBitmap.Width;
				int height2 = pdfBitmap.Height;
				try
				{
					pdfBitmap.FillRectEx(0, 0, width2, height2, -1);
					double num = width / (double)fs_RECTF.Width;
					double num2 = height / (double)fs_RECTF.Height;
					double num3 = (double)effectiveSize.Width * num;
					double num4 = (double)effectiveSize.Height * num2;
					double num5 = (double)(-(double)fs_RECTF.left) * num;
					double num6 = (double)(-(double)(effectiveSize.Height - fs_RECTF.top)) * num2;
					global::PDFKit.PdfControl viewer = global::PDFKit.PdfControl.GetPdfControl(page.Document);
					bool showAnnot = true;
					if (viewer != null)
					{
						Action action = delegate
						{
							showAnnot = viewer.IsAnnotationVisible;
						};
						if (viewer.Dispatcher.CheckAccess())
						{
							action();
						}
						else
						{
							viewer.Dispatcher.Invoke(action);
						}
					}
					IntPtr intPtr = Pdfium.FPDF_LoadPage(page.Document.Handle, page.PageIndex);
					try
					{
						if (intPtr != IntPtr.Zero)
						{
							PageRotate pageRotate = Pdfium.FPDFPage_GetRotation(intPtr);
							int num7 = (int)(PageRotate.Normal - pageRotate);
							if (num7 < 0)
							{
								num7 += 4;
							}
							Pdfium.FPDF_RenderPageBitmap(pdfBitmap.Handle, intPtr, (int)num5, (int)num6, (int)num3, (int)num4, (PageRotate)num7, showAnnot ? RenderFlags.FPDF_ANNOT : RenderFlags.FPDF_NONE);
						}
					}
					finally
					{
						try
						{
							if (intPtr != IntPtr.Zero)
							{
								Pdfium.FPDF_ClosePage(intPtr);
							}
						}
						catch
						{
						}
					}
					return await pdfBitmap.ToWriteableBitmapAsync(default(CancellationToken));
				}
				catch (Exception ex) when (!(ex is OperationCanceledException))
				{
				}
				writeableBitmap = null;
			}
			return writeableBitmap;
		}

		// Token: 0x06001DFA RID: 7674 RVA: 0x00083924 File Offset: 0x00081B24
		public void RenderFirstCropBox(int pageIdx)
		{
			if (pageIdx == -1)
			{
				return;
			}
			PdfPage pdfPage = this.viewer.Document.Pages[pageIdx];
			this.BeforeCropBox = new FS_RECTF?(pdfPage.GetEffectiveBox(PageRotate.Normal, false));
			this.CropPageToolbar.PageSize = new PageSizeModel
			{
				PageHeight = (double)pdfPage.MediaBox.Height,
				PageWidth = (double)pdfPage.MediaBox.Width,
				Height = pdfPage.MediaBox.Height,
				Width = pdfPage.MediaBox.Width,
				Xoffset = -pdfPage.MediaBox.left,
				Yoffset = -pdfPage.MediaBox.bottom,
				Screenshot = this
			};
			this.viewer.UpdateLayout();
			this.viewer.UpdateDocLayout();
			this.viewer.TryRedrawVisiblePageAsync(default(CancellationToken));
		}

		// Token: 0x06001DFB RID: 7675 RVA: 0x00083A18 File Offset: 0x00081C18
		public void RenderBoxPriview(int pageIdx)
		{
			if (pageIdx == -1)
			{
				pageIdx = this.PageIndexs;
			}
			if (this.pdfCopyDocument == null)
			{
				return;
			}
			PdfPage pdfPage = this.pdfCopyDocument.Pages[pageIdx];
			pdfPage.TrimBox = pdfPage.MediaBox;
			pdfPage.CropBox = pdfPage.MediaBox;
			this.CropPageToolbar.PageSize = new PageSizeModel
			{
				PageHeight = (double)pdfPage.MediaBox.Height,
				PageWidth = (double)pdfPage.MediaBox.Width,
				Height = pdfPage.MediaBox.Height,
				Width = pdfPage.MediaBox.Width,
				Xoffset = -pdfPage.MediaBox.left,
				Yoffset = -pdfPage.MediaBox.bottom,
				Screenshot = this
			};
		}

		// Token: 0x06001DFC RID: 7676 RVA: 0x00083AF1 File Offset: 0x00081CF1
		public void setCopyDocument(PdfDocument pdfDocument)
		{
			this.pdfCopyDocument = pdfDocument;
		}

		// Token: 0x06001DFD RID: 7677 RVA: 0x00083AFC File Offset: 0x00081CFC
		public void RenderBox(int pageIdx)
		{
			if (pageIdx == -1)
			{
				return;
			}
			PdfPage pdfPage = this.viewer.Document.Pages[pageIdx];
			this.CropPageToolbar.PageSize = new PageSizeModel
			{
				PageHeight = (double)pdfPage.MediaBox.Height,
				PageWidth = (double)pdfPage.MediaBox.Width,
				Height = pdfPage.MediaBox.Height,
				Width = pdfPage.MediaBox.Width,
				Xoffset = -pdfPage.MediaBox.left,
				Yoffset = -pdfPage.MediaBox.bottom,
				Screenshot = this
			};
			this.viewer.UpdateLayout();
			this.viewer.UpdateDocLayout();
			this.viewer.TryRedrawVisiblePageAsync(default(CancellationToken));
		}

		// Token: 0x06001DFE RID: 7678 RVA: 0x00083BE0 File Offset: 0x00081DE0
		public void ResetPage()
		{
			if (this.pageIdx == -1)
			{
				return;
			}
			PdfPage pdfPage = this.viewer.Document.Pages[this.pageIdx];
			if (this.BeforeCropBox != null)
			{
				this.CropPageToolbar.PageSize.Xoffset = 0f;
				this.CropPageToolbar.PageSize.Yoffset = 0f;
				this.viewer.UpdateLayout();
				this.viewer.UpdateDocLayout();
				this.viewer.TryRedrawVisiblePageAsync(default(CancellationToken));
			}
		}

		// Token: 0x06001DFF RID: 7679 RVA: 0x00083C78 File Offset: 0x00081E78
		public void ResetPages(int index)
		{
			PdfPage pdfPage = this.viewer.Document.Pages[index];
			if (this.pdfCopyDocument == null)
			{
				return;
			}
			PdfPage pdfPage2 = this.pdfCopyDocument.Pages[index];
			pdfPage2.MediaBox = pdfPage.MediaBox;
			pdfPage2.TrimBox = pdfPage.MediaBox;
			pdfPage2.CropBox = pdfPage.MediaBox;
			this.RenderBoxPriview(index);
			pdfPage2.ReloadPage();
		}

		// Token: 0x06001E00 RID: 7680 RVA: 0x00083CE8 File Offset: 0x00081EE8
		public void ResetPagesize(double height, double width, bool iscenter = true, int index = -1)
		{
			if (this.pdfCopyDocument == null)
			{
				return;
			}
			PdfPage pdfPage;
			if (index == -1)
			{
				pdfPage = this.pdfCopyDocument.Pages[this.pageIdx];
			}
			else
			{
				pdfPage = this.pdfCopyDocument.Pages[index];
			}
			if (iscenter)
			{
				float height2 = pdfPage.MediaBox.Height;
				float width2 = pdfPage.MediaBox.Width;
				float left = pdfPage.MediaBox.left;
				float right = pdfPage.MediaBox.right;
				float top = pdfPage.MediaBox.top;
				float bottom = pdfPage.MediaBox.bottom;
				float num = (float)(width - (double)width2) / 2f;
				float num2 = (float)(height - (double)height2) / 2f;
				pdfPage.MediaBox = new FS_RECTF
				{
					right = right + num,
					top = top + num2,
					bottom = bottom - num2,
					left = left - num
				};
				this.RenderBoxPriview(index);
				pdfPage.TrimBox = pdfPage.MediaBox;
				pdfPage.CropBox = pdfPage.MediaBox;
				pdfPage.ReloadPage();
				return;
			}
			float left2 = this.BeforeCropBox.Value.left;
			float right2 = this.BeforeCropBox.Value.right;
			float top2 = this.BeforeCropBox.Value.top;
			float bottom2 = this.BeforeCropBox.Value.bottom;
			float num3 = 0f;
			float num4 = 0f;
			if (pdfPage.MediaBox.Width - this.BeforeCropBox.Value.Width != 0f)
			{
				num3 = pdfPage.MediaBox.Width - this.BeforeCropBox.Value.Width;
			}
			if (pdfPage.MediaBox.Height - this.BeforeCropBox.Value.Height != 0f)
			{
				num4 = pdfPage.MediaBox.Height - this.BeforeCropBox.Value.Height;
			}
			pdfPage.MediaBox = new FS_RECTF
			{
				right = right2 - (float)width + num3,
				top = top2 - (float)height + num4,
				bottom = bottom2 - (float)height,
				left = left2 - (float)width
			};
			this.RenderBoxPriview(index);
			pdfPage.TrimBox = pdfPage.MediaBox;
			pdfPage.CropBox = pdfPage.MediaBox;
			pdfPage.ReloadPage();
		}

		// Token: 0x06001E01 RID: 7681 RVA: 0x00083F90 File Offset: 0x00082190
		public void ResetCropSize(int currentpageIdx)
		{
			if (this.BeforeCropBox == null)
			{
				return;
			}
			if (this.CropPageToolbar.ApplypageIndex != null)
			{
				this.viewer.ScrollToPage(this.CropPageToolbar.ApplypageIndex[0]);
			}
			PdfPage pdfPage = this.viewer.Document.Pages[currentpageIdx];
			pdfPage.CropBox = this.BeforeCropBox.Value;
			pdfPage.TrimBox = this.BeforeCropBox.Value;
			this.viewer.UpdateLayout();
			this.viewer.UpdateDocLayout();
			this.viewer.TryRedrawVisiblePageAsync(default(CancellationToken));
			base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				if (this.CropPageToolbar.ApplypageIndex != null)
				{
					this.viewer.CurrentIndex = this.CropPageToolbar.ApplypageIndex[0];
					return;
				}
				this.viewer.CurrentIndex = currentpageIdx;
			}));
			this.viewer.CurrentIndex = pdfPage.PageIndex;
			if (this.pageZoom != -1f)
			{
				this.VM.ViewToolbar.DocSizeMode = this.sizeModes;
				this.VM.ViewToolbar.DocZoom = this.pageZoom;
				return;
			}
			this.VM.ViewToolbar.DocSizeMode = SizeModes.Zoom;
			this.VM.ViewToolbar.DocZoom = this.VM.ViewToolbar.DocZoom;
		}

		// Token: 0x06001E02 RID: 7682 RVA: 0x000840EC File Offset: 0x000822EC
		private void SetCropRect(PdfPage page, FS_RECTF rect)
		{
			if (!page.Dictionary.ContainsKey("CropRect"))
			{
				PdfTypeDictionary pdfTypeDictionary = PdfTypeDictionary.Create();
				pdfTypeDictionary.SetRectAt("CropRect", rect);
				page.Dictionary["CropRect"] = pdfTypeDictionary;
				return;
			}
			PdfTypeDictionary pdfTypeDictionary2 = page.Dictionary["CropRect"] as PdfTypeDictionary;
			if (pdfTypeDictionary2.ContainsKey("CropRect"))
			{
				pdfTypeDictionary2.SetRectAt("CropRect", rect);
			}
		}

		// Token: 0x1400003B RID: 59
		// (add) Token: 0x06001E03 RID: 7683 RVA: 0x00084160 File Offset: 0x00082360
		// (remove) Token: 0x06001E04 RID: 7684 RVA: 0x00084198 File Offset: 0x00082398
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06001E05 RID: 7685 RVA: 0x000841CD File Offset: 0x000823CD
		private void RaisePropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		// Token: 0x06001E06 RID: 7686 RVA: 0x000841E8 File Offset: 0x000823E8
		internal void SetDrawControlBrush(UIElement drawControl, global::System.Windows.Media.Brush brush, bool isAddUndoStack = false)
		{
			if (drawControl != null)
			{
				global::System.Windows.Media.Brush brush2 = null;
				Border border = drawControl as Border;
				if (border != null)
				{
					global::System.Windows.Controls.TextBox textBox = border.Child as global::System.Windows.Controls.TextBox;
					if (textBox != null)
					{
						brush2 = textBox.Foreground;
						textBox.Foreground = brush;
					}
					else
					{
						brush2 = border.BorderBrush;
						border.BorderBrush = brush;
					}
				}
				else
				{
					Ellipse ellipse = drawControl as Ellipse;
					if (ellipse != null)
					{
						brush2 = ellipse.Stroke;
						ellipse.Stroke = brush;
					}
					else
					{
						Polyline polyline = drawControl as Polyline;
						if (polyline != null)
						{
							brush2 = polyline.Stroke;
							polyline.Stroke = brush;
						}
						else
						{
							global::System.Windows.Controls.Control control = drawControl as global::System.Windows.Controls.Control;
							if (control != null)
							{
								brush2 = control.Background;
								control.Background = brush;
							}
						}
					}
				}
				if (isAddUndoStack && brush2.ToString() != brush.ToString())
				{
					this.undoDrawControlStack.Push(new ChangeColorOperation(drawControl, brush2));
				}
			}
		}

		// Token: 0x06001E07 RID: 7687 RVA: 0x000842B4 File Offset: 0x000824B4
		internal void SetDrawControlThickness(UIElement drawControl, double thickness, bool isAddUndoStack = false)
		{
			if (drawControl != null)
			{
				double num = 0.0;
				Border border = drawControl as Border;
				if (border != null)
				{
					num = border.BorderThickness.Left;
					border.BorderThickness = new Thickness(thickness);
				}
				else
				{
					Ellipse ellipse = drawControl as Ellipse;
					if (ellipse != null)
					{
						num = ellipse.StrokeThickness;
						ellipse.StrokeThickness = thickness;
					}
					else
					{
						Polyline polyline = drawControl as Polyline;
						if (polyline != null)
						{
							num = polyline.StrokeThickness;
							polyline.StrokeThickness = thickness;
						}
						else
						{
							global::System.Windows.Controls.Control control = drawControl as global::System.Windows.Controls.Control;
							if (control != null)
							{
								num = this.ArrowHeight2Thickness(control.Height);
								control.Height = this.Thickness2ArrowHeight(thickness);
							}
						}
					}
				}
				if (isAddUndoStack && num != thickness)
				{
					this.undoDrawControlStack.Push(new ChangeThicknessOperation(drawControl, num));
				}
			}
		}

		// Token: 0x06001E08 RID: 7688 RVA: 0x00084374 File Offset: 0x00082574
		internal void SetDrawTextFontSize(UIElement drawControl, double fontSize, bool isAddUndoStack = false)
		{
			Border border = drawControl as Border;
			if (border != null)
			{
				global::System.Windows.Controls.TextBox textBox = border.Child as global::System.Windows.Controls.TextBox;
				if (textBox != null)
				{
					double fontSize2 = textBox.FontSize;
					textBox.FontSize = fontSize;
					if (isAddUndoStack && fontSize2 != fontSize)
					{
						this.undoDrawControlStack.Push(new ChangeFontSizeOperation(drawControl, fontSize2));
					}
				}
			}
		}

		// Token: 0x06001E09 RID: 7689 RVA: 0x000843C1 File Offset: 0x000825C1
		private void RemoveSelected()
		{
			UIElement uielement = this.SelectedDrawControl;
			if (uielement != null)
			{
				uielement.UnSelect();
			}
			this.SelectedDrawControl = null;
		}

		// Token: 0x06001E0A RID: 7690 RVA: 0x000843DC File Offset: 0x000825DC
		private bool CheckPointInDraggerCanvas(global::System.Windows.Point point)
		{
			return point.X >= 0.0 && point.X <= this.DragResizeView.Width && point.Y >= 0.0 && point.Y <= this.DragResizeView.Height;
		}

		// Token: 0x06001E0B RID: 7691 RVA: 0x00084438 File Offset: 0x00082638
		private void DrawText()
		{
			double num = this.startDrawPoint.X + 40.0;
			if (this.ControlTextBox == null)
			{
				this.ControlTextBox = new Border
				{
					BorderBrush = (global::System.Windows.Media.Brush)global::System.Windows.Application.Current.FindResource("DottedLineDrawingBrush"),
					BorderThickness = new Thickness(1.0),
					SnapsToDevicePixels = true,
					UseLayoutRounding = true
				};
				global::System.Windows.Controls.TextBox textBox = new global::System.Windows.Controls.TextBox();
				textBox.Style = null;
				textBox.Background = null;
				textBox.BorderThickness = new Thickness(0.0);
				textBox.Foreground = this.CurrentBrush;
				textBox.FontSize = this.CurrentFontSize;
				textBox.TextWrapping = TextWrapping.Wrap;
				textBox.FontWeight = FontWeights.Normal;
				textBox.MinWidth = 40.0;
				textBox.MaxWidth = this.DragResizeView.Width - this.startDrawPoint.X;
				textBox.MaxHeight = this.DragResizeView.Height - 4.0;
				textBox.Cursor = global::System.Windows.Input.Cursors.SizeAll;
				textBox.Padding = new Thickness(4.0);
				textBox.AcceptsReturn = true;
				textBox.LostFocus += delegate(object s, RoutedEventArgs e1)
				{
					global::System.Windows.Controls.TextBox textBox2 = s as global::System.Windows.Controls.TextBox;
					DependencyObject parent = VisualTreeHelper.GetParent(textBox2);
					if (parent != null)
					{
						Border border = parent as Border;
						if (border != null)
						{
							border.BorderThickness = new Thickness(0.0);
							if (string.IsNullOrWhiteSpace(textBox2.Text))
							{
								this.DragResizeView.RemoveDrawControl(border);
							}
						}
					}
				};
				this.ControlTextBox.PreviewMouseLeftButtonDown += this.DrawControl_MouseLeftButtonDown;
				this.ControlTextBox.MouseMove += this.DrawControl_MouseMove;
				this.ControlTextBox.MouseLeftButtonUp += this.DrawControl_MouseLeftButtonUp;
				this.ControlTextBox.SizeChanged += delegate(object s, SizeChangedEventArgs e1)
				{
					Border border2 = s as Border;
					double y = this.startDrawPoint.Y;
					if (y + border2.ActualHeight > this.DragResizeView.Height)
					{
						double num3 = Math.Abs(this.DragResizeView.Height - (y + border2.ActualHeight));
						this.startDrawPoint.Y = y - num3;
						Canvas.SetTop(border2, this.startDrawPoint.Y + 2.0);
					}
				};
				this.ControlTextBox.Child = textBox;
				this.DragResizeView.AddUIElementToCanvas(this.ControlTextBox);
				this.undoDrawControlStack.Push(new DrawControlOperation(OperationType.DrawText, this.ControlTextBox));
				textBox.Focus();
				double num2 = this.startDrawPoint.X;
				if (num > this.DragResizeView.Width)
				{
					num2 -= num - this.DragResizeView.Width;
				}
				Canvas.SetLeft(this.ControlTextBox, num2 - 2.0);
				Canvas.SetTop(this.ControlTextBox, this.startDrawPoint.Y - 2.0);
				this.SelectedDrawControl = this.ControlTextBox;
			}
		}

		// Token: 0x06001E0C RID: 7692 RVA: 0x00084684 File Offset: 0x00082884
		private void DrawControl(global::System.Windows.Point current, DrawControlMode drawControlMode)
		{
			switch (drawControlMode)
			{
			case DrawControlMode.DrawRectangle:
			{
				Rect rect = new Rect(this.startDrawPoint, current);
				if (this.ControlRectangle == null)
				{
					this.ControlRectangle = new Border
					{
						BorderBrush = this.CurrentBrush,
						BorderThickness = new Thickness(this.CurrentThickness),
						CornerRadius = new CornerRadius(3.0),
						Cursor = global::System.Windows.Input.Cursors.SizeAll
					};
					this.ControlRectangle.MouseLeftButtonDown += this.DrawControl_MouseLeftButtonDown;
					this.ControlRectangle.MouseMove += this.DrawControl_MouseMove;
					this.ControlRectangle.MouseLeftButtonUp += this.DrawControl_MouseLeftButtonUp;
					this.DragResizeView.AddUIElementToCanvas(this.ControlRectangle);
				}
				this.ControlRectangle.Width = rect.Width;
				this.ControlRectangle.Height = rect.Height;
				Canvas.SetLeft(this.ControlRectangle, rect.Left);
				Canvas.SetTop(this.ControlRectangle, rect.Top);
				this.controlLocation = new Rect(rect.Left, rect.Top, rect.Width, rect.Height);
				return;
			}
			case DrawControlMode.DrawCircle:
			{
				Rect rect2 = new Rect(this.startDrawPoint, current);
				if (this.ControlCircle == null)
				{
					this.ControlCircle = new Ellipse
					{
						Stroke = this.CurrentBrush,
						StrokeThickness = this.CurrentThickness,
						Cursor = global::System.Windows.Input.Cursors.SizeAll
					};
					this.ControlCircle.MouseLeftButtonDown += this.DrawControl_MouseLeftButtonDown;
					this.ControlCircle.MouseMove += this.DrawControl_MouseMove;
					this.ControlCircle.MouseLeftButtonUp += this.DrawControl_MouseLeftButtonUp;
					this.DragResizeView.AddUIElementToCanvas(this.ControlCircle);
				}
				this.ControlCircle.Width = rect2.Width;
				this.ControlCircle.Height = rect2.Height;
				Canvas.SetLeft(this.ControlCircle, rect2.Left);
				Canvas.SetTop(this.ControlCircle, rect2.Top);
				this.controlLocation = new Rect(rect2.Left, rect2.Top, rect2.Width, rect2.Height);
				return;
			}
			case DrawControlMode.DrawArrow:
			{
				Rect rect3 = new Rect(this.startDrawPoint, current);
				if (this.ControlArrow == null)
				{
					this.ControlArrow = new global::System.Windows.Controls.Control
					{
						Background = this.CurrentBrush,
						BorderThickness = new Thickness(this.CurrentThickness),
						Template = this.controlArrowTemplate,
						Cursor = global::System.Windows.Input.Cursors.SizeAll
					};
					this.ControlArrow.MouseLeftButtonDown += this.DrawControl_MouseLeftButtonDown;
					this.ControlArrow.MouseMove += this.DrawControl_MouseMove;
					this.ControlArrow.MouseLeftButtonUp += this.DrawControl_MouseLeftButtonUp;
					this.ControlArrow.Height = this.Thickness2ArrowHeight(this.CurrentThickness);
					this.DragResizeView.AddUIElementToCanvas(this.ControlArrow);
					Canvas.SetLeft(this.ControlArrow, rect3.Left);
					Canvas.SetTop(this.ControlArrow, rect3.Top - this.ControlArrow.Height / 2.0);
				}
				TransformGroup transformGroup = new TransformGroup();
				RotateTransform rotateTransform = new RotateTransform();
				transformGroup.Children.Add(rotateTransform);
				global::System.Windows.Point point = new global::System.Windows.Point(0.0, 0.5);
				this.ControlArrow.RenderTransformOrigin = point;
				this.ControlArrow.RenderTransform = transformGroup;
				rotateTransform.Angle = ScreenshotDialog.CalculeAngle(this.startDrawPoint, current);
				double num = current.X - this.startDrawPoint.X;
				double num2 = current.Y - this.startDrawPoint.Y;
				double num3 = Math.Sqrt(Math.Pow(num, 2.0) + Math.Pow(num2, 2.0));
				num3 = ((num3 < 15.0) ? 15.0 : num3);
				this.ControlArrow.Width = num3;
				this.controlLocation = new Rect(rect3.Left, rect3.Top - this.ControlArrow.Height / 2.0, this.ControlArrow.Width, this.ControlArrow.Height);
				return;
			}
			case DrawControlMode.DrawInk:
				if (this.ControlInk == null)
				{
					this.ControlInk = new Polyline
					{
						Stroke = this.CurrentBrush,
						Cursor = global::System.Windows.Input.Cursors.SizeAll,
						StrokeThickness = this.CurrentThickness,
						StrokeLineJoin = PenLineJoin.Round,
						StrokeStartLineCap = PenLineCap.Round,
						StrokeEndLineCap = PenLineCap.Round
					};
					this.ControlInk.MouseLeftButtonDown += this.DrawControl_MouseLeftButtonDown;
					this.ControlInk.MouseMove += this.DrawControl_MouseMove;
					this.ControlInk.MouseLeftButtonUp += this.DrawControl_MouseLeftButtonUp;
					this.DragResizeView.AddUIElementToCanvas(this.ControlInk);
				}
				this.ControlInk.Points.Add(current);
				return;
			default:
				return;
			}
		}

		// Token: 0x06001E0D RID: 7693 RVA: 0x00084BB0 File Offset: 0x00082DB0
		private void DrawControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Border border = this.SelectedDrawControl as Border;
			if (border != null)
			{
				global::System.Windows.Controls.TextBox textBox = border.Child as global::System.Windows.Controls.TextBox;
				if (textBox != null)
				{
					if (textBox.Text != this.controlTextBoxText && !string.IsNullOrEmpty(this.controlTextBoxText))
					{
						this.undoDrawControlStack.Push(new ChangeTextBoxTextOperation(this.SelectedDrawControl, this.controlTextBoxText));
					}
					this.controlTextBoxText = null;
				}
			}
			this.RemoveSelected();
			this.SelectedDrawControl = (UIElement)sender;
			if (this.SelectedDrawControl == null)
			{
				return;
			}
			bool flag = false;
			Border border2 = sender as Border;
			if (border2 != null)
			{
				if (border2.Child == null)
				{
					this.DrawControlMode = DrawControlMode.DrawRectangle;
					this.CurrentBrush = border2.BorderBrush;
					this.CurrentThickness = border2.BorderThickness.Left;
				}
				else
				{
					global::System.Windows.Controls.TextBox textBox2 = border2.Child as global::System.Windows.Controls.TextBox;
					if (textBox2 != null)
					{
						this.DrawControlMode = DrawControlMode.DrawText;
						this.CurrentBrush = textBox2.Foreground;
						this.CurrentFontSize = textBox2.FontSize;
						this.controlTextBoxText = textBox2.Text;
						Keyboard.Focus(textBox2);
						flag = true;
					}
				}
			}
			else
			{
				Ellipse ellipse = sender as Ellipse;
				if (ellipse != null)
				{
					this.DrawControlMode = DrawControlMode.DrawCircle;
					this.CurrentBrush = ellipse.Stroke;
					this.CurrentThickness = ellipse.StrokeThickness;
				}
				else
				{
					Polyline polyline = sender as Polyline;
					if (polyline != null)
					{
						this.DrawControlMode = DrawControlMode.DrawInk;
						this.CurrentBrush = polyline.Stroke;
						this.CurrentThickness = polyline.StrokeThickness;
					}
					else
					{
						global::System.Windows.Controls.Control control = sender as global::System.Windows.Controls.Control;
						if (control != null)
						{
							this.DrawControlMode = DrawControlMode.DrawArrow;
							this.CurrentBrush = control.Background;
							this.CurrentThickness = this.ArrowHeight2Thickness(control.Height);
						}
					}
				}
			}
			this.RaisePropertyChanged("CurrentBrush");
			if (!flag)
			{
				this.RaisePropertyChanged("CurrentThickness");
			}
			else
			{
				this.RaisePropertyChanged("CurrentFontSize");
			}
			this.SelectedDrawControl.Select();
			this.dragStartPoint = Mouse.GetPosition(this.DragResizeView.GetDraggerCanvas());
			TransformGroup transformGroup = this.selectedDrawControl.RenderTransform as TransformGroup;
			if (transformGroup != null && transformGroup.Children.Count > 0)
			{
				this.orignalTransformGroup = transformGroup.Clone();
				if (sender is global::System.Windows.Controls.Control && transformGroup.Children.Count == 1)
				{
					this.totalTranslate = new TranslateTransform();
				}
				else
				{
					this.totalTranslate = (TranslateTransform)transformGroup.Children.Last<Transform>();
				}
			}
			else
			{
				this.orignalTransformGroup = new TransformGroup();
				this.totalTranslate = new TranslateTransform();
			}
			this.tempTranslate = new TranslateTransform();
			this.SelectedDrawControl.CaptureMouse();
			e.Handled = true;
		}

		// Token: 0x06001E0E RID: 7694 RVA: 0x00084E4C File Offset: 0x0008304C
		private void DrawControl_MouseMove(object sender, global::System.Windows.Input.MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && this.SelectedDrawControl != null)
			{
				this.isDragDrawControl = true;
				global::System.Windows.Point position = Mouse.GetPosition(this.DragResizeView.GetDraggerCanvas());
				this.KeepPointInDraggerCanvas(ref position);
				double num = position.X - this.dragStartPoint.X;
				double num2 = position.Y - this.dragStartPoint.Y;
				this.tempTranslate.X = this.totalTranslate.X + num;
				this.tempTranslate.Y = this.totalTranslate.Y + num2;
				TransformGroup transformGroup = new TransformGroup();
				if (this.SelectedDrawControl is global::System.Windows.Controls.Control)
				{
					TransformGroup transformGroup2 = this.selectedDrawControl.RenderTransform as TransformGroup;
					if (transformGroup2 != null && transformGroup2.Children.Count > 0)
					{
						foreach (Transform transform in transformGroup2.Children)
						{
							RotateTransform rotateTransform = transform as RotateTransform;
							if (rotateTransform != null)
							{
								transformGroup.Children.Add(rotateTransform);
							}
						}
					}
				}
				transformGroup.Children.Add(this.tempTranslate);
				this.SelectedDrawControl.RenderTransform = transformGroup;
				e.Handled = true;
			}
		}

		// Token: 0x06001E0F RID: 7695 RVA: 0x00084F9C File Offset: 0x0008319C
		private void DrawControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			UIElement uielement = this.SelectedDrawControl;
			if (uielement != null)
			{
				uielement.ReleaseMouseCapture();
			}
			if (this.isDragDrawControl)
			{
				this.isDragDrawControl = false;
				global::System.Windows.Point position = e.GetPosition(this.DragResizeView.GetDraggerCanvas());
				if (position.Equals(this.dragStartPoint))
				{
					return;
				}
				this.totalTranslate.X += position.X - this.dragStartPoint.X;
				this.totalTranslate.Y += position.Y - this.dragStartPoint.Y;
				this.undoDrawControlStack.Push(new MoveControlOperation(this.SelectedDrawControl, this.orignalTransformGroup));
			}
		}

		// Token: 0x06001E10 RID: 7696 RVA: 0x00085054 File Offset: 0x00083254
		private void Viewer_ZoomChanged(object sender, EventArgs e)
		{
			if (this.pageIdx != -1)
			{
				Canvas draggerCanvas = this.DragResizeView.GetDraggerCanvas();
				double num = (double)(this.viewer.Zoom / this.curZoomFactor);
				for (int i = 1; i < draggerCanvas.Children.Count; i++)
				{
					draggerCanvas.Children[i].Zoom(num);
				}
				foreach (DrawOperation drawOperation in this.undoDrawControlStack)
				{
					MoveControlOperation moveControlOperation = drawOperation as MoveControlOperation;
					if (moveControlOperation != null && moveControlOperation.OriginalTransformGroup != null)
					{
						foreach (Transform transform in moveControlOperation.OriginalTransformGroup.Children)
						{
							if (!(transform is RotateTransform))
							{
								TranslateTransform translateTransform = (TranslateTransform)transform;
								translateTransform.X *= num;
								translateTransform.Y *= num;
							}
						}
					}
				}
				this.curZoomFactor = this.viewer.Zoom;
			}
		}

		// Token: 0x06001E11 RID: 7697 RVA: 0x0008518C File Offset: 0x0008338C
		private double ArrowHeight2Thickness(double arrowHeight)
		{
			int num = Array.IndexOf<double>(DrawSettingConstants.ArrowHeight, arrowHeight);
			if (num < 0 || num > DrawSettingConstants.Thicknesses.Count<double>() - 1)
			{
				num = 0;
			}
			return DrawSettingConstants.Thicknesses[num];
		}

		// Token: 0x06001E12 RID: 7698 RVA: 0x000851C4 File Offset: 0x000833C4
		private void KeepPointInDraggerCanvas(ref global::System.Windows.Point point)
		{
			if (point.X < 0.0)
			{
				point.X = 0.0;
			}
			else if (point.X > this.DragResizeView.Width)
			{
				point.X = this.DragResizeView.Width;
			}
			if (point.Y < 0.0)
			{
				point.Y = 0.0;
				return;
			}
			if (point.Y > this.DragResizeView.Height)
			{
				point.Y = this.DragResizeView.Height;
			}
		}

		// Token: 0x06001E13 RID: 7699 RVA: 0x0008525C File Offset: 0x0008345C
		private static double CalculeAngle(global::System.Windows.Point start, global::System.Windows.Point end)
		{
			return (Math.Atan2(end.Y - start.Y, end.X - start.X) * 57.295779513082323 + 360.0) % 360.0;
		}

		// Token: 0x06001E14 RID: 7700 RVA: 0x000852AC File Offset: 0x000834AC
		private double Thickness2ArrowHeight(double thickness)
		{
			int num = Array.IndexOf<double>(DrawSettingConstants.Thicknesses, thickness);
			if (num < 0 || num > DrawSettingConstants.ArrowHeight.Count<double>() - 1)
			{
				num = 0;
			}
			return DrawSettingConstants.ArrowHeight[num];
		}

		// Token: 0x06001E15 RID: 7701 RVA: 0x000852E4 File Offset: 0x000834E4
		private void ClearDraw()
		{
			this.startDrawPoint = new global::System.Windows.Point(-1.0, -1.0);
			this.ControlInk = null;
			this.ControlRectangle = null;
			this.ControlCircle = null;
			this.ControlArrow = null;
			this.ControlTextBox = null;
			this.controlLocation = default(Rect);
		}

		// Token: 0x06001E16 RID: 7702 RVA: 0x00085340 File Offset: 0x00083540
		public void UndoDrawControl()
		{
			if (this.undoDrawControlStack.Count == 0)
			{
				return;
			}
			DrawOperation drawOperation = this.undoDrawControlStack.Pop();
			switch (drawOperation.Type)
			{
			case OperationType.DrawRectangle:
			case OperationType.DrawCircle:
			case OperationType.DrawArrow:
			case OperationType.DrawInk:
			case OperationType.DrawText:
			{
				ResizeView dragResizeView = this.DragResizeView;
				if (dragResizeView == null)
				{
					return;
				}
				dragResizeView.RemoveDrawControl(drawOperation.Element);
				return;
			}
			case OperationType.ChangeColor:
			{
				ChangeColorOperation changeColorOperation = drawOperation as ChangeColorOperation;
				if (changeColorOperation != null)
				{
					this.SetDrawControlBrush(changeColorOperation.Element, changeColorOperation.OriginalBrush, false);
					return;
				}
				break;
			}
			case OperationType.ChangeThickness:
			{
				ChangeThicknessOperation changeThicknessOperation = drawOperation as ChangeThicknessOperation;
				if (changeThicknessOperation != null)
				{
					this.SetDrawControlThickness(changeThicknessOperation.Element, changeThicknessOperation.OriginalThickness, false);
					return;
				}
				break;
			}
			case OperationType.MoveControl:
			{
				MoveControlOperation moveControlOperation = drawOperation as MoveControlOperation;
				if (moveControlOperation != null)
				{
					moveControlOperation.Element.RenderTransform = moveControlOperation.OriginalTransformGroup;
					return;
				}
				break;
			}
			case OperationType.ChangeTextBoxText:
			{
				ChangeTextBoxTextOperation changeTextBoxTextOperation = drawOperation as ChangeTextBoxTextOperation;
				if (changeTextBoxTextOperation != null)
				{
					Border border = changeTextBoxTextOperation.Element as Border;
					if (border != null)
					{
						global::System.Windows.Controls.TextBox textBox = border.Child as global::System.Windows.Controls.TextBox;
						if (textBox != null)
						{
							textBox.Text = changeTextBoxTextOperation.OriginalText;
							return;
						}
					}
				}
				break;
			}
			case OperationType.ChangeFontSize:
			{
				ChangeFontSizeOperation changeFontSizeOperation = drawOperation as ChangeFontSizeOperation;
				if (changeFontSizeOperation != null)
				{
					this.SetDrawTextFontSize(changeFontSizeOperation.Element, changeFontSizeOperation.OriginalFontSize, false);
					return;
				}
				break;
			}
			case OperationType.DeleteControl:
			{
				DeleteControlOperation deleteControlOperation = drawOperation as DeleteControlOperation;
				if (deleteControlOperation != null)
				{
					this.DragResizeView.AddUIElementToCanvas(deleteControlOperation.Element);
					Canvas.SetLeft(deleteControlOperation.Element, deleteControlOperation.Left);
					Canvas.SetTop(deleteControlOperation.Element, deleteControlOperation.Top);
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x06001E17 RID: 7703 RVA: 0x000854C8 File Offset: 0x000836C8
		private WriteableBitmap AttachDrawControl2FinalImage(WriteableBitmap source)
		{
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			{
				ResizeViewOperation dragMode = this.DragResizeView.DragMode;
				this.DragResizeView.DragMode = ResizeViewOperation.None;
				drawingContext.DrawImage(source, new Rect(0.0, 0.0, source.Width, source.Height));
				Canvas draggerCanvas = this.DragResizeView.GetDraggerCanvas();
				RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)draggerCanvas.ActualWidth, (int)draggerCanvas.ActualHeight, 96.0, 96.0, PixelFormats.Pbgra32);
				renderTargetBitmap.Render(draggerCanvas);
				drawingContext.PushTransform(new ScaleTransform(source.Width / draggerCanvas.ActualWidth, source.Height / draggerCanvas.ActualHeight));
				drawingContext.DrawImage(renderTargetBitmap, new Rect(0.0, 0.0, renderTargetBitmap.Width, renderTargetBitmap.Height));
				this.DragResizeView.DragMode = dragMode;
			}
			RenderTargetBitmap renderTargetBitmap2 = new RenderTargetBitmap((int)source.Width, (int)source.Height, 96.0, 96.0, PixelFormats.Pbgra32);
			renderTargetBitmap2.Render(drawingVisual);
			return new WriteableBitmap(renderTargetBitmap2);
		}

		// Token: 0x06001E18 RID: 7704 RVA: 0x00085614 File Offset: 0x00083814
		private global::System.Drawing.Image BitmapSourceToImage(BitmapSource bitmapSource)
		{
			global::System.Drawing.Image image;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new BmpBitmapEncoder
				{
					Frames = { BitmapFrame.Create(bitmapSource) }
				}.Save(memoryStream);
				image = new Bitmap(memoryStream);
			}
			return image;
		}

		// Token: 0x06001E19 RID: 7705 RVA: 0x00085668 File Offset: 0x00083868
		public async Task ExtractText()
		{
			try
			{
				WriteableBitmap writeableBitmap = await this.GetScaledPageImageAsync();
				WriteableBitmap bitmap = writeableBitmap;
				PdfViewer pdfViewer = this.viewer;
				PdfDocument document = ((pdfViewer != null) ? pdfViewer.Document : null);
				PdfPage pdfPage = document.Pages[this.pageIdx];
				WriteableBitmap writeableBitmap2 = await this.RotateImageAsync(bitmap, pdfPage.Rotation);
				WriteableBitmap writeableBitmap3 = this.AttachDrawControl2FinalImage(writeableBitmap2);
				if (writeableBitmap3 != null)
				{
					FS_POINTF fs_POINTF = this.startPt;
					FS_POINTF fs_POINTF2 = this.curPt;
					FS_RECTF fs_RECTF = new FS_RECTF(Math.Min(fs_POINTF.X, fs_POINTF2.X), Math.Max(fs_POINTF.Y, fs_POINTF2.Y), Math.Max(fs_POINTF.X, fs_POINTF2.X), Math.Min(fs_POINTF.Y, fs_POINTF2.Y));
					Rect rect;
					if (this.viewer.TryGetClientRect(this.pageIdx, fs_RECTF, out rect))
					{
						global::System.Drawing.Image image = this.BitmapSourceToImage(writeableBitmap3);
						try
						{
							global::System.Windows.Forms.Clipboard.SetImage(image);
						}
						catch
						{
						}
						ScreenshotDialogResult screenshotDialogResult = ScreenshotDialogResult.CreateExtractImageText(this.pageIdx, "", bitmap, new Bitmap(image), fs_RECTF, rect, true);
						this.Close(null);
						if (screenshotDialogResult != null && screenshotDialogResult.Completed)
						{
							ExtractTextResultDialog extractTextResultDialog = ExtractTextResultDialog.FromImage(document, screenshotDialogResult);
							extractTextResultDialog.Owner = global::System.Windows.Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
							extractTextResultDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
							extractTextResultDialog.ShowDialog();
						}
					}
				}
				bitmap = null;
				document = null;
			}
			catch
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.ImageControl_OcrFailed, "PDFgear", MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
		}

		// Token: 0x04000B4D RID: 2893
		private Window window;

		// Token: 0x04000B4E RID: 2894
		private TaskCompletionSource<ScreenshotDialogResult> tcs;

		// Token: 0x04000B4F RID: 2895
		private PdfViewer viewer;

		// Token: 0x04000B50 RID: 2896
		private ScrollViewer viewerSv;

		// Token: 0x04000B51 RID: 2897
		public Rect curRect;

		// Token: 0x04000B52 RID: 2898
		public int pageIdx = -1;

		// Token: 0x04000B53 RID: 2899
		public FS_POINTF startPt;

		// Token: 0x04000B54 RID: 2900
		public FS_POINTF curPt;

		// Token: 0x04000B55 RID: 2901
		private global::System.Windows.Point curPoint;

		// Token: 0x04000B56 RID: 2902
		private SizeModes sizeModes = SizeModes.Zoom;

		// Token: 0x04000B57 RID: 2903
		private float pageZoom = -1f;

		// Token: 0x04000B58 RID: 2904
		private DispatcherTimer autoScrollTimer;

		// Token: 0x04000B59 RID: 2905
		private ScreenshotDialogMode mode;

		// Token: 0x04000B5C RID: 2908
		public int[] Pages;

		// Token: 0x04000B5D RID: 2909
		public ScreenshotDialog.Dimensionunits units;

		// Token: 0x04000B5E RID: 2910
		public PageRotate cropPageRotate;

		// Token: 0x04000B5F RID: 2911
		private DrawControlMode drawControlMode;

		// Token: 0x04000B60 RID: 2912
		private global::System.Windows.Point startDrawPoint;

		// Token: 0x04000B61 RID: 2913
		private global::System.Windows.Point curDrawPoint;

		// Token: 0x04000B62 RID: 2914
		private global::System.Windows.Media.Brush currentBrush = new SolidColorBrush(DrawSettingConstants.Colors[1]);

		// Token: 0x04000B63 RID: 2915
		private double currentThickness = DrawSettingConstants.Thicknesses[1];

		// Token: 0x04000B64 RID: 2916
		private double currentFontSize = DrawSettingConstants.DefaultFontSize;

		// Token: 0x04000B65 RID: 2917
		private Border ControlRectangle;

		// Token: 0x04000B66 RID: 2918
		private global::System.Windows.Controls.Control ControlArrow;

		// Token: 0x04000B67 RID: 2919
		private ControlTemplate controlArrowTemplate;

		// Token: 0x04000B68 RID: 2920
		private Ellipse ControlCircle;

		// Token: 0x04000B69 RID: 2921
		private Polyline ControlInk;

		// Token: 0x04000B6A RID: 2922
		private Border ControlTextBox;

		// Token: 0x04000B6B RID: 2923
		private string controlTextBoxText;

		// Token: 0x04000B6C RID: 2924
		private const int _width = 40;

		// Token: 0x04000B6D RID: 2925
		private Rect controlLocation;

		// Token: 0x04000B6E RID: 2926
		private Stack<DrawOperation> undoDrawControlStack;

		// Token: 0x04000B6F RID: 2927
		private bool isDragDrawControl;

		// Token: 0x04000B70 RID: 2928
		private UIElement selectedDrawControl;

		// Token: 0x04000B71 RID: 2929
		private TranslateTransform totalTranslate;

		// Token: 0x04000B72 RID: 2930
		private TranslateTransform tempTranslate;

		// Token: 0x04000B73 RID: 2931
		private global::System.Windows.Point dragStartPoint;

		// Token: 0x04000B74 RID: 2932
		private TransformGroup orignalTransformGroup;

		// Token: 0x04000B75 RID: 2933
		private float curZoomFactor;

		// Token: 0x04000B76 RID: 2934
		private global::System.Windows.Point StartPoint;

		// Token: 0x04000B77 RID: 2935
		public bool fixedRate;

		// Token: 0x04000B78 RID: 2936
		public int PageIndexs;

		// Token: 0x04000B79 RID: 2937
		public PdfDocument pdfCopyDocument;

		// Token: 0x0200064A RID: 1610
		public enum Dimensionunits
		{
			// Token: 0x04002107 RID: 8455
			CM,
			// Token: 0x04002108 RID: 8456
			Point,
			// Token: 0x04002109 RID: 8457
			Inch,
			// Token: 0x0400210A RID: 8458
			Pica,
			// Token: 0x0400210B RID: 8459
			MM
		}
	}
}
