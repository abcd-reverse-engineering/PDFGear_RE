using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Models.Annotations;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;
using PDFKit.Utils.PdfRichTextStrings;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x0200029D RID: 669
	public partial class AnnotationFreeTextEditor : UserControl, IAnnotationControl<PdfFreeTextAnnotation>, IAnnotationControl
	{
		// Token: 0x06002686 RID: 9862 RVA: 0x000B54F0 File Offset: 0x000B36F0
		public AnnotationFreeTextEditor(PdfFreeTextAnnotation annot, FreeTextAnnotationHolder holder)
		{
			this.InitializeComponent();
			if (annot == null)
			{
				throw new ArgumentNullException("annot");
			}
			this.Annotation = annot;
			if (holder == null)
			{
				throw new ArgumentNullException("holder");
			}
			this.Holder = holder;
			this.curRect = this.Annotation.GetRECT();
			base.Loaded += this.AnnotationFreeTextEditor_Loaded;
			this.page = this.Annotation.Page;
			this.annotModel = (FreeTextAnnotation)AnnotationFactory.Create(this.Annotation);
			this.createTime = DateTime.Now;
			this.rtb = RichTextBoxUtils.CreateRichTextBox(this.Annotation);
			if (string.IsNullOrEmpty(this.Annotation.Contents))
			{
				if (this.Annotation.Intent != AnnotationIntent.FreeTextTypeWriter)
				{
					PdfBorderStyle borderStyle = this.Annotation.BorderStyle;
					if (borderStyle != null)
					{
						float width = borderStyle.Width;
					}
				}
				this.TextPlaceholderContainer.Visibility = Visibility.Visible;
			}
			PageRotate? pageRotate;
			PageRotate pageRotate2;
			annot.GetRotate(out pageRotate, out pageRotate2);
			this.actualRotate = PdfRotateUtils.AnnotRotation(pageRotate2, annot.Page.Rotation);
			if (this.actualRotate != PageRotate.Normal)
			{
				RotateTransform rotateTransform = new RotateTransform((double)((PageRotate)360 - this.actualRotate * (PageRotate)90));
				this.RichTextBoxContainer.RenderTransform = rotateTransform;
			}
			if (annot.Intent == AnnotationIntent.FreeTextTypeWriter)
			{
				if (this.actualRotate == PageRotate.Normal || this.actualRotate == PageRotate.Rotate180)
				{
					this.dragModeWithoutMove = ResizeViewOperation.LeftCenter | ResizeViewOperation.RightCenter;
				}
				else
				{
					this.dragModeWithoutMove = ResizeViewOperation.CenterTop | ResizeViewOperation.CenterBottom;
				}
				this.DraggerResizeView.BorderBrush = new SolidColorBrush(Color.FromArgb(60, 0, 0, 0));
				this.RichTextBoxBorder.Opacity = 0.0;
				this.TextAnnotionPlaceholder.Margin = new Thickness(6.0, 2.5, 0.0, 0.0);
				this.TextAnnotionPlaceholder.VerticalAlignment = VerticalAlignment.Center;
				this.TextAnnotionPlaceholder.TextWrapping = TextWrapping.NoWrap;
			}
			else
			{
				this.dragModeWithoutMove = ResizeViewOperation.ResizeAll;
			}
			this.DraggerResizeView.DragMode = this.dragModeWithoutMove | ResizeViewOperation.Move;
		}

		// Token: 0x17000BD7 RID: 3031
		// (get) Token: 0x06002687 RID: 9863 RVA: 0x000B56FD File Offset: 0x000B38FD
		public PdfFreeTextAnnotation Annotation { get; }

		// Token: 0x17000BD8 RID: 3032
		// (get) Token: 0x06002688 RID: 9864 RVA: 0x000B5705 File Offset: 0x000B3905
		public IAnnotationHolder Holder { get; }

		// Token: 0x17000BD9 RID: 3033
		// (get) Token: 0x06002689 RID: 9865 RVA: 0x000B570D File Offset: 0x000B390D
		public AnnotationCanvas ParentCanvas
		{
			get
			{
				return (AnnotationCanvas)base.Parent;
			}
		}

		// Token: 0x17000BDA RID: 3034
		// (get) Token: 0x0600268A RID: 9866 RVA: 0x000B571A File Offset: 0x000B391A
		PdfAnnotation IAnnotationControl.Annotation
		{
			get
			{
				return this.Annotation;
			}
		}

		// Token: 0x0600268B RID: 9867 RVA: 0x000B5722 File Offset: 0x000B3922
		public RichTextBox GetRichTextBox()
		{
			return this.rtb;
		}

		// Token: 0x0600268C RID: 9868 RVA: 0x000B572A File Offset: 0x000B392A
		private void SetBoundsChangedFlag()
		{
			if (!this.boundsChanged)
			{
				this.boundsChanged = true;
			}
		}

		// Token: 0x0600268D RID: 9869 RVA: 0x000B573C File Offset: 0x000B393C
		public async void OnPageClientBoundsChanged()
		{
			if (this.rtb != null && this.rtbTrans != null)
			{
				AnnotationCanvas parentCanvas = this.ParentCanvas;
				PdfViewer viewer = ((parentCanvas != null) ? parentCanvas.PdfViewer : null);
				if (!base.IsLoaded || viewer == null)
				{
					base.Opacity = 0.0;
					base.IsHitTestVisible = false;
				}
				else
				{
					Size oldSize = new Size(this.rtb.ActualWidth, this.rtb.ActualHeight);
					await this.ExtendRichTextBoxAsync();
					Size size = new Size(this.rtb.ActualWidth, this.rtb.ActualHeight);
					if (oldSize != size)
					{
						this.UpdateCurrentRect();
					}
					float num = Math.Min(100f, this.page.Width);
					Rect rect;
					if (viewer.TryGetClientRect(this.page.PageIndex, new FS_RECTF(0f, 1f, num, 0f), out rect))
					{
						double num2 = ((this.page.Rotation == PageRotate.Normal || this.page.Rotation == PageRotate.Rotate180) ? (rect.Width / (double)(num * 96f / 72f)) : (rect.Height / (double)(num * 96f / 72f)));
						this.rtbTrans.ScaleX = num2;
						this.rtbTrans.ScaleY = num2;
						PdfBorderStyleModel borderStyle = this.annotModel.BorderStyle;
						float num3 = ((borderStyle != null) ? borderStyle.Width : 1f) * 96f / 72f;
						this.borderWidth = (double)num3 * num2;
						this.RichTextBoxBorder.BorderThickness = new Thickness(this.borderWidth);
						double num4 = this.rtb.Width * num2 + this.borderWidth * 2.0;
						double num5 = this.rtb.Height * num2 + this.borderWidth * 2.0;
						Rect rect2;
						if (viewer.TryGetClientRect(this.page.PageIndex, this.curRect, out rect2))
						{
							if (Math.Abs(rect2.Width - num4) / num2 > 0.2)
							{
								this.RichTextBoxBorder.Width = Math.Max(num4, 0.0);
							}
							else
							{
								this.RichTextBoxBorder.Width = rect2.Width;
							}
							if (Math.Abs(rect2.Height - num5) / num2 > 0.2)
							{
								this.RichTextBoxBorder.Height = Math.Max(num5, 0.0);
							}
							else
							{
								this.RichTextBoxBorder.Height = rect2.Height;
							}
							double num6 = 0.0;
							double num7 = 0.0;
							if (num2 > 0.0)
							{
								num6 = this.RichTextBoxBorder.Width / num2;
								num7 = this.RichTextBoxBorder.Height / num2;
							}
							this.TextPlaceholderContainer.Width = Math.Max(num6 - (double)(num3 * 2f), 0.0);
							this.TextPlaceholderContainer.Height = Math.Max(num7 - (double)(num3 * 2f), 0.0);
							this.UpdateRotatePosition();
							Canvas.SetLeft(this, rect2.Left);
							Canvas.SetTop(this, rect2.Top);
						}
						this.ResetDraggers();
					}
				}
			}
		}

		// Token: 0x0600268E RID: 9870 RVA: 0x000B5774 File Offset: 0x000B3974
		private void UpdateRotatePosition()
		{
			foreach (UIElement uielement in this.RichTextBoxContainer.Children.OfType<UIElement>())
			{
				double num = 0.0;
				if (uielement == this.rtb || uielement == this.TextPlaceholderContainer)
				{
					num = this.borderWidth;
				}
				if (this.actualRotate == PageRotate.Normal)
				{
					Canvas.SetTop(uielement, num);
					Canvas.SetLeft(uielement, num);
				}
				if (this.actualRotate == PageRotate.Rotate90)
				{
					Canvas.SetTop(uielement, num);
					Canvas.SetLeft(uielement, -this.RichTextBoxBorder.ActualWidth + num);
				}
				else if (this.actualRotate == PageRotate.Rotate180)
				{
					Canvas.SetLeft(uielement, -this.RichTextBoxBorder.ActualWidth + num);
					Canvas.SetTop(uielement, -this.RichTextBoxBorder.ActualHeight + num);
				}
				else if (this.actualRotate == PageRotate.Rotate270)
				{
					Canvas.SetTop(uielement, -this.RichTextBoxBorder.ActualHeight + num);
					Canvas.SetLeft(uielement, num);
				}
			}
		}

		// Token: 0x0600268F RID: 9871 RVA: 0x000B5880 File Offset: 0x000B3A80
		private void ResetDraggers()
		{
			if (this.actualRotate == PageRotate.Normal || this.actualRotate == PageRotate.Rotate180)
			{
				this.DraggerCanvas.Width = this.RichTextBoxBorder.ActualWidth;
				this.DraggerCanvas.Height = this.RichTextBoxBorder.ActualHeight;
				this.DraggerResizeView.Width = this.RichTextBoxBorder.ActualWidth;
				this.DraggerResizeView.Height = this.RichTextBoxBorder.ActualHeight;
				return;
			}
			this.DraggerCanvas.Width = this.RichTextBoxBorder.ActualHeight;
			this.DraggerCanvas.Height = this.RichTextBoxBorder.ActualWidth;
			this.DraggerResizeView.Width = this.RichTextBoxBorder.ActualHeight;
			this.DraggerResizeView.Height = this.RichTextBoxBorder.ActualWidth;
		}

		// Token: 0x06002690 RID: 9872 RVA: 0x000B5950 File Offset: 0x000B3B50
		private void AnnotationFreeTextEditor_Loaded(object sender, RoutedEventArgs e)
		{
			AnnotationCanvas parentCanvas = this.ParentCanvas;
			object obj;
			if (parentCanvas == null)
			{
				obj = null;
			}
			else
			{
				PdfViewer pdfViewer = parentCanvas.PdfViewer;
				obj = ((pdfViewer != null) ? pdfViewer.Parent : null);
			}
			this.sv = obj as ScrollViewer;
			this.InitRichTextBox();
			this.Annotation.CreateEmptyAppearance(AppearanceStreamModes.Normal);
			this.ParentCanvas.PdfViewer.InvalidateVisual();
			this.UpdateRotatePosition();
		}

		// Token: 0x06002691 RID: 9873 RVA: 0x000B59B0 File Offset: 0x000B3BB0
		private void InitRichTextBox()
		{
			if (!PdfDefaultAppearance.TryParse(this.annotModel.DefaultAppearance, out this.da))
			{
				this.da = PdfDefaultAppearance.Default;
			}
			if (this.annotModel.Color != FS_COLOR.Empty)
			{
				this.da.FillColor = this.annotModel.Color;
			}
			Color color = Colors.Black;
			Color color2 = Colors.Transparent;
			if (this.annotModel.Intent != AnnotationIntent.FreeTextTypeWriter)
			{
				color = this.da.StrokeColor.ToColor();
				color2 = this.da.FillColor.ToColor();
			}
			this.RichTextBoxBorder.BorderBrush = new SolidColorBrush(color);
			this.rtbTrans = new ScaleTransform();
			this.rtb.RenderTransform = this.rtbTrans;
			this.TextPlaceholderContainer.RenderTransform = this.rtbTrans;
			this.rtb.Background = new SolidColorBrush(color2);
			this.rtb.BorderThickness = default(Thickness);
			FrameworkElement frameworkElement = VisualTreeHelper.GetChild(this.rtb, 0) as FrameworkElement;
			if (frameworkElement != null)
			{
				ScrollViewer scrollViewer = frameworkElement.FindName("PART_ContentHost") as ScrollViewer;
				if (scrollViewer != null)
				{
					this.rtbSv = scrollViewer;
					this.rtbSv.ScrollChanged += this.RtbSv_ScrollChanged;
				}
			}
			this.RichTextBoxContainer.Children.Add(this.rtb);
			this.rtb.TextChanged += this.Rtb_TextChanged;
			this.rtb.PreviewKeyDown += this.Rtb_PreviewKeyDown;
			this.rtb.KeyDown += this.Rtb_KeyDown;
			this.rtb.SizeChanged += this.Rtb_SizeChanged;
			if (this.gotoEditingFromOuter)
			{
				this.GoToEditingCore(new Point?(new Point(0.0, 0.0)));
			}
			else
			{
				this.ExitEditing(false, false);
			}
			this.OnPageClientBoundsChanged();
		}

		// Token: 0x06002692 RID: 9874 RVA: 0x000B5BA1 File Offset: 0x000B3DA1
		private void Rtb_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Delete)
			{
				e.Handled = true;
			}
		}

		// Token: 0x06002693 RID: 9875 RVA: 0x000B5BB4 File Offset: 0x000B3DB4
		private async void Rtb_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (!this.sizeChanging)
			{
				this.sizeChanging = true;
				try
				{
					MainViewModel vm = base.DataContext as MainViewModel;
					if (vm != null)
					{
						if (this.firstSizeChanged)
						{
							this.firstSizeChanged = false;
							await this.ExtendRichTextBoxAsync();
						}
						double scaleX = this.rtbTrans.ScaleX;
						double left = this.RichTextBoxBorder.BorderThickness.Left;
						double num = this.rtb.Width * scaleX + left * 2.0;
						double num2 = this.rtb.Height * scaleX + left * 2.0;
						if (double.IsNaN(this.RichTextBoxBorder.Width) || Math.Abs(this.RichTextBoxBorder.Width - num) / scaleX > 0.2)
						{
							this.RichTextBoxBorder.Width = num;
						}
						if (double.IsNaN(this.RichTextBoxBorder.Height) || Math.Abs(this.RichTextBoxBorder.Height - num2) / scaleX > 0.2)
						{
							this.RichTextBoxBorder.Height = num2;
						}
						this.UpdateCurrentRect();
						bool flag = this.sizeChanged && this.boundsChanged;
						if (!flag && this.Annotation.Intent == AnnotationIntent.FreeTextTypeWriter && !string.IsNullOrEmpty(this.GetRichTextBoxText()))
						{
							flag = true;
						}
						if (flag)
						{
							using (vm.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
							{
								this.Annotation.Rectangle = this.curRect;
							}
							this.sizeChanged = false;
						}
						vm = null;
					}
				}
				finally
				{
					this.sizeChanging = false;
				}
			}
		}

		// Token: 0x06002694 RID: 9876 RVA: 0x000B5BEC File Offset: 0x000B3DEC
		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
		{
			base.OnPreviewMouseWheel(e);
			AnnotationCanvas parentCanvas = this.ParentCanvas;
			ScrollViewer scrollViewer;
			if (parentCanvas == null)
			{
				scrollViewer = null;
			}
			else
			{
				PdfViewer pdfViewer = parentCanvas.PdfViewer;
				scrollViewer = ((pdfViewer != null) ? pdfViewer.ScrollOwner : null);
			}
			ScrollViewer scrollViewer2 = scrollViewer;
			if (scrollViewer2 != null)
			{
				MouseWheelEventArgs mouseWheelEventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
				mouseWheelEventArgs.RoutedEvent = UIElement.MouseWheelEvent;
				mouseWheelEventArgs.Source = scrollViewer2;
				InputManager.Current.ProcessInput(mouseWheelEventArgs);
			}
			e.Handled = true;
		}

		// Token: 0x06002695 RID: 9877 RVA: 0x000B5C5F File Offset: 0x000B3E5F
		private void RichTextBoxContainer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.rtb != null)
			{
				this.GoToEditingCore(new Point?(e.GetPosition(this.rtb)));
			}
		}

		// Token: 0x06002696 RID: 9878 RVA: 0x000B5C80 File Offset: 0x000B3E80
		private async Task ExtendRichTextBoxAsync()
		{
			if (base.IsLoaded && this.rtb != null)
			{
				this.rtb.SizeChanged -= this.Rtb_SizeChanged;
				await RichTextBoxUtils.ExtendRichTextBoxAsync(this.rtb, this.Annotation.Intent == AnnotationIntent.FreeTextTypeWriter, default(CancellationToken));
				RichTextBox richTextBox = this.rtb;
				lock (richTextBox)
				{
					this.rtb.SizeChanged -= this.Rtb_SizeChanged;
					this.rtb.SizeChanged += this.Rtb_SizeChanged;
				}
			}
		}

		// Token: 0x06002697 RID: 9879 RVA: 0x000B5CC4 File Offset: 0x000B3EC4
		private void UpdateCurrentRect()
		{
			if (this.ParentCanvas == null)
			{
				return;
			}
			double left = Canvas.GetLeft(this);
			double top = Canvas.GetTop(this);
			double num = this.RichTextBoxBorder.Width;
			double num2 = this.RichTextBoxBorder.Height;
			if (this.actualRotate == PageRotate.Rotate90 || this.actualRotate == PageRotate.Rotate270)
			{
				double num3 = num2;
				num2 = num;
				num = num3;
			}
			if (double.IsNaN(left) || double.IsNaN(top) || double.IsNaN(num) || double.IsNaN(num2))
			{
				return;
			}
			AnnotationCanvas parentCanvas = this.ParentCanvas;
			FS_RECTF fs_RECTF;
			if (((parentCanvas != null) ? parentCanvas.PdfViewer : null).TryGetPageRect(this.Annotation.Page.PageIndex, new Rect(left, top, num, num2), out fs_RECTF) && ((double)Math.Abs(this.curRect.left - fs_RECTF.left) > 0.8 || (double)Math.Abs(this.curRect.top - fs_RECTF.top) > 0.8 || (double)Math.Abs(this.curRect.right - fs_RECTF.right) > 0.8 || (double)Math.Abs(this.curRect.bottom - fs_RECTF.bottom) > 0.8))
			{
				this.curRect = fs_RECTF;
				this.SetBoundsChangedFlag();
			}
		}

		// Token: 0x06002698 RID: 9880 RVA: 0x000B5E0C File Offset: 0x000B400C
		private void Rtb_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				e.Handled = true;
				this.ExitEditing(this.textChanged, false);
				this.textChanged = false;
			}
		}

		// Token: 0x06002699 RID: 9881 RVA: 0x000B5E33 File Offset: 0x000B4033
		private void RtbSv_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			this.OnPageClientBoundsChanged();
		}

		// Token: 0x0600269A RID: 9882 RVA: 0x000B5E3C File Offset: 0x000B403C
		private void Rtb_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.textChanged = true;
			if (!string.IsNullOrEmpty(this.GetRichTextBoxText()))
			{
				this.TextPlaceholderContainer.Visibility = Visibility.Collapsed;
				return;
			}
			AnnotationCanvas parentCanvas = this.ParentCanvas;
			PdfViewer pdfViewer = ((parentCanvas != null) ? parentCanvas.PdfViewer : null);
			if (!base.IsLoaded || pdfViewer == null)
			{
				return;
			}
			float num = Math.Min(100f, this.page.Width);
			Rect rect;
			if (!pdfViewer.TryGetClientRect(this.page.PageIndex, new FS_RECTF(0f, 1f, num, 0f), out rect))
			{
				return;
			}
			if (this.page.Rotation != PageRotate.Normal && this.page.Rotation != PageRotate.Rotate180)
			{
				double num2 = rect.Height / (double)(num * 96f / 72f);
			}
			else
			{
				double num3 = rect.Width / (double)(num * 96f / 72f);
			}
			if (this.Annotation.Intent != AnnotationIntent.FreeTextTypeWriter)
			{
				float width = this.Annotation.BorderStyle.Width;
			}
			this.TextPlaceholderContainer.Visibility = Visibility.Visible;
		}

		// Token: 0x0600269B RID: 9883 RVA: 0x000B5F43 File Offset: 0x000B4143
		private void RichTextBoxBorder_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ResetDraggers();
			this.UpdateRotatePosition();
		}

		// Token: 0x0600269C RID: 9884 RVA: 0x000B5F51 File Offset: 0x000B4151
		private void DraggerResizeView_ResizeDragStarted(object sender, ResizeViewResizeDragStartedEventArgs e)
		{
			this.ExitEditing(false, false);
		}

		// Token: 0x0600269D RID: 9885 RVA: 0x000B5F5C File Offset: 0x000B415C
		private void DraggerResizeView_ResizeDragCompleted(object sender, ResizeViewResizeDragEventArgs e)
		{
			if (!(base.DataContext is MainViewModel))
			{
				return;
			}
			this.RichTextBoxBorder.Width = e.NewSize.Width;
			this.RichTextBoxBorder.Height = e.NewSize.Height;
			double num = this.RichTextBoxBorder.Width - this.RichTextBoxBorder.BorderThickness.Left - this.RichTextBoxBorder.BorderThickness.Right;
			double num2 = this.RichTextBoxBorder.Height - this.RichTextBoxBorder.BorderThickness.Top - this.RichTextBoxBorder.BorderThickness.Bottom;
			if (this.actualRotate == PageRotate.Rotate90 || this.actualRotate == PageRotate.Rotate270)
			{
				double num3 = num2;
				num2 = num;
				num = num3;
			}
			double num4 = Math.Max(0.0, num / this.rtbTrans.ScaleX);
			double num5 = Math.Max(0.0, num2 / this.rtbTrans.ScaleX);
			this.sizeChanged = false;
			if (num4 != this.rtb.Width || num5 != this.rtb.Height)
			{
				this.rtb.Width = Math.Max(0.0, num4);
				this.rtb.Height = Math.Max(0.0, num5);
				this.sizeChanged = true;
			}
			if (e.OffsetX != 0.0 || e.OffsetY != 0.0)
			{
				double left = Canvas.GetLeft(this);
				double top = Canvas.GetTop(this);
				Canvas.SetLeft(this, left + e.OffsetX);
				Canvas.SetTop(this, top + e.OffsetY);
			}
			this.UpdateCurrentRect();
			bool flag = this.textChanged;
			if (this.Annotation.Intent == AnnotationIntent.FreeTextTypeWriter && string.IsNullOrEmpty(this.GetRichTextBoxText()))
			{
				flag = false;
			}
			this.ExitEditing(flag, false);
			if (!this.curRect.IntersectsWith(new FS_RECTF(0f, this.page.Height, this.page.Width, 0f)))
			{
				this.Holder.Cancel();
				return;
			}
			this.OnPageClientBoundsChanged();
		}

		// Token: 0x0600269E RID: 9886 RVA: 0x000B618C File Offset: 0x000B438C
		private void DraggerResizeView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if ((DateTime.Now - this.createTime).TotalSeconds < 0.1)
			{
				e.Handled = true;
				this.GoToEditingCore(new Point?(e.GetPosition(this.rtb)));
			}
		}

		// Token: 0x0600269F RID: 9887 RVA: 0x000B61DA File Offset: 0x000B43DA
		private void DraggerResizeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.GoToEditingCore(new Point?(e.GetPosition(this.rtb)));
		}

		// Token: 0x060026A0 RID: 9888 RVA: 0x000B61F4 File Offset: 0x000B43F4
		public void GoToEditing()
		{
			this.gotoEditingFromOuter = true;
			this.GoToEditingCore(null);
		}

		// Token: 0x060026A1 RID: 9889 RVA: 0x000B6218 File Offset: 0x000B4418
		private void GoToEditingCore(Point? position)
		{
			if (this.rtb == null)
			{
				return;
			}
			this.DraggerResizeView.DragMode = this.dragModeWithoutMove;
			this.rtb.IsReadOnly = false;
			this.rtb.Focus();
			if (position != null)
			{
				TextPointer positionFromPoint = this.rtb.GetPositionFromPoint(position.Value, true);
				this.rtb.CaretPosition = positionFromPoint;
			}
		}

		// Token: 0x060026A2 RID: 9890 RVA: 0x000B6280 File Offset: 0x000B4480
		private async void ExitEditing(bool saveChange, bool applying = false)
		{
			if (this.rtb != null && this.DraggerResizeView != null && !(this.Annotation == null))
			{
				this.rtb.IsReadOnly = true;
				if (this.rtb.IsFocused)
				{
					TraversalRequest traversalRequest = new TraversalRequest(FocusNavigationDirection.Next);
					if (!this.rtb.MoveFocus(traversalRequest))
					{
						Keyboard.ClearFocus();
					}
				}
				this.DraggerResizeView.DragMode = this.dragModeWithoutMove | ResizeViewOperation.Move;
				AnnotationCanvas parentCanvas = this.ParentCanvas;
				MainViewModel vm = ((parentCanvas != null) ? parentCanvas.DataContext : null) as MainViewModel;
				if ((saveChange || applying) && this.annotModel.Intent == AnnotationIntent.FreeTextTypeWriter)
				{
					try
					{
						if (string.IsNullOrEmpty(this.GetRichTextBoxText()))
						{
							await this.ParentCanvas.HolderManager.DeleteAnnotationAsync(this.Annotation, false);
							return;
						}
					}
					catch
					{
					}
				}
				IDisposable disposable = null;
				try
				{
					MainViewModel mainViewModel = vm;
					disposable = ((mainViewModel != null) ? mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, "") : null);
					this.Annotation.Rectangle = this.curRect;
					if (saveChange)
					{
						PdfRichTextStyle? pdfRichTextStyle = null;
						PdfRichTextStyle pdfRichTextStyle2;
						if (PdfRichTextStyle.TryParse(this.Annotation.DefaultStyle, out pdfRichTextStyle2))
						{
							pdfRichTextStyle = new PdfRichTextStyle?(pdfRichTextStyle2);
						}
						RichTextBox richTextBox = this.rtb;
						PdfRichTextStyle? pdfRichTextStyle3 = pdfRichTextStyle;
						PdfFreeTextAnnotation annotation = this.Annotation;
						PdfRichTextString pdfRichTextString = PdfRichTextStringHelper.FromRichTextBox(richTextBox, pdfRichTextStyle3, (annotation != null) ? annotation.Name : null);
						this.Annotation.RichText = pdfRichTextString.ToString();
						if (this.Annotation.Contents != pdfRichTextString.ContentText)
						{
							this.Annotation.Contents = pdfRichTextString.ContentText;
							MainViewModel mainViewModel2 = vm;
							if (mainViewModel2 != null)
							{
								PageEditorViewModel pageEditors = mainViewModel2.PageEditors;
								if (pageEditors != null)
								{
									pageEditors.NotifyPageAnnotationChanged(this.Annotation.Page.PageIndex);
								}
							}
						}
						this.Annotation.DefaultStyle = pdfRichTextString.DefaultStyle.ToString();
					}
				}
				finally
				{
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		// Token: 0x060026A3 RID: 9891 RVA: 0x000B62C8 File Offset: 0x000B44C8
		public void Apply()
		{
			if (this.rtb == null)
			{
				return;
			}
			if (this.rtbSv == null)
			{
				return;
			}
			this.UpdateCurrentRect();
			this.ExitEditing(this.textChanged, true);
			ScrollViewer scrollViewer = this.sv;
			if (scrollViewer != null)
			{
				scrollViewer.ScrollChanged += this.<Apply>g__Sv_ScrollChanged|53_0;
			}
			this.Annotation.TryRedrawAnnotation(false);
			if (scrollViewer != null)
			{
				scrollViewer.ScrollChanged -= this.<Apply>g__Sv_ScrollChanged|53_0;
			}
		}

		// Token: 0x060026A4 RID: 9892 RVA: 0x000B6338 File Offset: 0x000B4538
		private string GetRichTextBoxText()
		{
			if (this.rtb == null)
			{
				return string.Empty;
			}
			string text = new TextRange(this.rtb.Document.ContentStart, this.rtb.Document.ContentEnd).Text;
			if (string.IsNullOrEmpty(text) || text == "\r\n" || text == "\n")
			{
				return string.Empty;
			}
			return text;
		}

		// Token: 0x060026A5 RID: 9893 RVA: 0x000B63A8 File Offset: 0x000B45A8
		public bool OnPropertyChanged(string propertyName)
		{
			MainViewModel mainViewModel = base.DataContext as MainViewModel;
			if (mainViewModel == null)
			{
				return false;
			}
			if (propertyName == "TextFontColor" || propertyName == "TextFontSize")
			{
				PdfFreeTextAnnotation annotation = this.Annotation;
				if (annotation == null || annotation.Intent != AnnotationIntent.FreeTextTypeWriter)
				{
					return false;
				}
			}
			if (propertyName == "TextBoxFontColor" || propertyName == "TextBoxFontSize")
			{
				PdfFreeTextAnnotation annotation2 = this.Annotation;
				if (annotation2 != null && annotation2.Intent == AnnotationIntent.FreeTextTypeWriter)
				{
					return false;
				}
			}
			TextPointer contentStart = this.rtb.Document.ContentStart;
			TextPointer contentEnd = this.rtb.Document.ContentEnd;
			TextRange textRange = new TextRange(contentStart, contentEnd);
			PdfRichTextStyle pdfRichTextStyle;
			bool flag = PdfRichTextStyle.TryParse(this.Annotation.DefaultStyle, out pdfRichTextStyle);
			PdfDefaultAppearance pdfDefaultAppearance;
			bool flag2 = PdfDefaultAppearance.TryParse(this.Annotation.DefaultAppearance, out pdfDefaultAppearance);
			bool flag3 = false;
			if (!(propertyName == "TextFontColor"))
			{
				if (!(propertyName == "TextBoxFontColor"))
				{
					if (!(propertyName == "TextFontSize"))
					{
						if (propertyName == "TextBoxFontSize")
						{
							float textBoxFontSize = mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxFontSize;
							double num = (double)textBoxFontSize * 96.0 / 72.0;
							textRange.ApplyPropertyValue(Control.FontSizeProperty, num);
							if (flag)
							{
								pdfRichTextStyle.FontSize = new float?(textBoxFontSize);
							}
							if (flag2)
							{
								pdfDefaultAppearance.FontSize = textBoxFontSize;
							}
							flag3 = true;
						}
					}
					else
					{
						float textFontSize = mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextFontSize;
						double num2 = (double)textFontSize * 96.0 / 72.0;
						textRange.ApplyPropertyValue(Control.FontSizeProperty, num2);
						if (flag)
						{
							pdfRichTextStyle.FontSize = new float?(textFontSize);
						}
						if (flag2)
						{
							pdfDefaultAppearance.FontSize = textFontSize;
						}
						flag3 = true;
					}
				}
				else
				{
					Color color = (Color)ColorConverter.ConvertFromString(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextBoxFontColor);
					textRange.ApplyPropertyValue(Control.ForegroundProperty, new SolidColorBrush(color));
					if (flag)
					{
						pdfRichTextStyle.Color = new Color?(color);
					}
					if (flag2)
					{
						pdfDefaultAppearance.FillColor = color.ToPdfColor();
					}
					flag3 = true;
				}
			}
			else
			{
				Color color2 = (Color)ColorConverter.ConvertFromString(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.TextFontColor);
				textRange.ApplyPropertyValue(Control.ForegroundProperty, new SolidColorBrush(color2));
				if (flag)
				{
					pdfRichTextStyle.Color = new Color?(color2);
				}
				if (flag2)
				{
					pdfDefaultAppearance.FillColor = color2.ToPdfColor();
				}
				flag3 = true;
			}
			if (flag3 && flag)
			{
				using (mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
				{
					this.Annotation.DefaultStyle = pdfRichTextStyle.ToString();
					this.Annotation.DefaultAppearance = pdfDefaultAppearance.ToString();
				}
			}
			return flag3;
		}

		// Token: 0x060026A9 RID: 9897 RVA: 0x000B67A6 File Offset: 0x000B49A6
		[CompilerGenerated]
		private void <Apply>g__Sv_ScrollChanged|53_0(object sender, ScrollChangedEventArgs e)
		{
			if (base.IsLoaded)
			{
				base.Opacity = 0.0;
				base.IsHitTestVisible = false;
			}
		}

		// Token: 0x04001099 RID: 4249
		private ResizeViewOperation dragModeWithoutMove;

		// Token: 0x0400109A RID: 4250
		private DateTime createTime;

		// Token: 0x0400109B RID: 4251
		private RichTextBox rtb;

		// Token: 0x0400109C RID: 4252
		private ScrollViewer rtbSv;

		// Token: 0x0400109D RID: 4253
		private FS_RECTF curRect;

		// Token: 0x0400109E RID: 4254
		private PdfDefaultAppearance da;

		// Token: 0x0400109F RID: 4255
		private ScaleTransform rtbTrans;

		// Token: 0x040010A0 RID: 4256
		private bool boundsChanged;

		// Token: 0x040010A1 RID: 4257
		private bool sizeChanged;

		// Token: 0x040010A2 RID: 4258
		private bool textChanged;

		// Token: 0x040010A3 RID: 4259
		private FreeTextAnnotation annotModel;

		// Token: 0x040010A4 RID: 4260
		private PdfPage page;

		// Token: 0x040010A5 RID: 4261
		private bool gotoEditingFromOuter;

		// Token: 0x040010A8 RID: 4264
		private ScrollViewer sv;

		// Token: 0x040010A9 RID: 4265
		private PageRotate actualRotate;

		// Token: 0x040010AA RID: 4266
		private double borderWidth;

		// Token: 0x040010AB RID: 4267
		private bool sizeChanging;

		// Token: 0x040010AC RID: 4268
		private bool firstSizeChanged = true;
	}
}
