using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Services;
using PDFKit.Utils;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x0200029B RID: 667
	public partial class AnnotationDragEditControl : UserControl, IAnnotationControl<PdfFigureAnnotation>, IAnnotationControl
	{
		// Token: 0x06002665 RID: 9829 RVA: 0x000B3E34 File Offset: 0x000B2034
		public AnnotationDragEditControl(PdfFigureAnnotation annot, IAnnotationHolder holder)
		{
			this.InitializeComponent();
			this.Annotation = annot;
			this.Holder = holder;
			this.curRect = annot.GetRECT();
			base.Foreground = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 24, 146, byte.MaxValue));
		}

		// Token: 0x06002666 RID: 9830 RVA: 0x000B3E88 File Offset: 0x000B2088
		public AnnotationDragEditControl(PdfLinkAnnotation annot, IAnnotationHolder holder)
		{
			this.InitializeComponent();
			this.LinkAnnotation = annot;
			this.Holder = holder;
			this.curRect = annot.GetRECT();
			base.Foreground = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 24, 146, byte.MaxValue));
		}

		// Token: 0x17000BD0 RID: 3024
		// (get) Token: 0x06002667 RID: 9831 RVA: 0x000B3EDC File Offset: 0x000B20DC
		public PdfFigureAnnotation Annotation { get; }

		// Token: 0x17000BD1 RID: 3025
		// (get) Token: 0x06002668 RID: 9832 RVA: 0x000B3EE4 File Offset: 0x000B20E4
		public PdfLinkAnnotation LinkAnnotation { get; }

		// Token: 0x17000BD2 RID: 3026
		// (get) Token: 0x06002669 RID: 9833 RVA: 0x000B3EEC File Offset: 0x000B20EC
		public IAnnotationHolder Holder { get; }

		// Token: 0x17000BD3 RID: 3027
		// (get) Token: 0x0600266A RID: 9834 RVA: 0x000B3EF4 File Offset: 0x000B20F4
		public AnnotationCanvas ParentCanvas
		{
			get
			{
				return (AnnotationCanvas)base.Parent;
			}
		}

		// Token: 0x17000BD4 RID: 3028
		// (get) Token: 0x0600266B RID: 9835 RVA: 0x000B3F01 File Offset: 0x000B2101
		PdfAnnotation IAnnotationControl.Annotation
		{
			get
			{
				return this.Annotation;
			}
		}

		// Token: 0x0600266C RID: 9836 RVA: 0x000B3F09 File Offset: 0x000B2109
		private void ResetDraggers()
		{
			this.DragResizeView.Width = this.LayoutRoot.ActualWidth;
			this.DragResizeView.Height = this.LayoutRoot.ActualHeight;
		}

		// Token: 0x0600266D RID: 9837 RVA: 0x000B3F38 File Offset: 0x000B2138
		private FS_RECTF? GetNewRect()
		{
			double left = Canvas.GetLeft(this);
			double top = Canvas.GetTop(this);
			double width = this.LayoutRoot.Width;
			double height = this.LayoutRoot.Height;
			AnnotationCanvas parentCanvas = this.ParentCanvas;
			FS_RECTF fs_RECTF;
			if (((parentCanvas != null) ? parentCanvas.PdfViewer : null).TryGetPageRect(this.Annotation.Page.PageIndex, new Rect(left, top, width, height), out fs_RECTF))
			{
				if (fs_RECTF.Width < 2f)
				{
					fs_RECTF.right = fs_RECTF.left + 2f;
				}
				if (fs_RECTF.Height < 2f)
				{
					fs_RECTF.bottom = fs_RECTF.top - 2f;
				}
				return new FS_RECTF?(fs_RECTF);
			}
			return null;
		}

		// Token: 0x0600266E RID: 9838 RVA: 0x000B3FF8 File Offset: 0x000B21F8
		private FS_RECTF? GetLinkNewRect()
		{
			double left = Canvas.GetLeft(this);
			double top = Canvas.GetTop(this);
			double width = this.LayoutRoot.Width;
			double height = this.LayoutRoot.Height;
			AnnotationCanvas parentCanvas = this.ParentCanvas;
			FS_RECTF fs_RECTF;
			if (((parentCanvas != null) ? parentCanvas.PdfViewer : null).TryGetPageRect(this.LinkAnnotation.Page.PageIndex, new Rect(left, top, width, height), out fs_RECTF))
			{
				if (fs_RECTF.Width < 2f)
				{
					fs_RECTF.right = fs_RECTF.left + 2f;
				}
				if (fs_RECTF.Height < 2f)
				{
					fs_RECTF.bottom = fs_RECTF.top - 2f;
				}
				return new FS_RECTF?(fs_RECTF);
			}
			return null;
		}

		// Token: 0x0600266F RID: 9839 RVA: 0x000B40B8 File Offset: 0x000B22B8
		private void ResizeView_ResizeDragStarted(object sender, ResizeViewResizeDragStartedEventArgs e)
		{
			if (this.Annotation is PdfCircleAnnotation)
			{
				this.ResizePlaceholderEllipse.Opacity = 1.0;
			}
			if (this.Annotation is PdfSquareAnnotation || e.Operation != ResizeViewOperation.Move || this.LinkAnnotation != null)
			{
				this.DragResizeView.BorderThickness = new Thickness(1.0);
				return;
			}
			this.DragResizeView.BorderThickness = default(Thickness);
		}

		// Token: 0x06002670 RID: 9840 RVA: 0x000B4134 File Offset: 0x000B2334
		private void DragResizeView_ResizeDragging(object sender, ResizeViewResizeDragEventArgs e)
		{
			if (this.Annotation is PdfCircleAnnotation)
			{
				this.ResizePlaceholderEllipse.Width = e.NewSize.Width;
				this.ResizePlaceholderEllipse.Height = e.NewSize.Height;
			}
		}

		// Token: 0x06002671 RID: 9841 RVA: 0x000B4180 File Offset: 0x000B2380
		private void ResizeView_ResizeDragCompleted(object sender, ResizeViewResizeDragEventArgs e)
		{
			MainViewModel mainViewModel = base.DataContext as MainViewModel;
			if (mainViewModel == null)
			{
				return;
			}
			this.ResizePlaceholderEllipse.Opacity = 0.0;
			this.DragResizeView.BorderThickness = default(Thickness);
			this.LayoutRoot.Width = e.NewSize.Width;
			this.LayoutRoot.Height = e.NewSize.Height;
			double num = Canvas.GetLeft(this);
			double num2 = Canvas.GetTop(this);
			num += e.OffsetX;
			num2 += e.OffsetY;
			Canvas.SetLeft(this, num);
			Canvas.SetTop(this, num2);
			bool pdfViewer = this.ParentCanvas.PdfViewer != null;
			PdfFigureAnnotation annotation = this.Annotation;
			PdfPage pdfPage = ((annotation != null) ? annotation.Page : null);
			PdfLinkAnnotation linkAnnotation = this.LinkAnnotation;
			PdfPage pdfPage2 = ((linkAnnotation != null) ? linkAnnotation.Page : null);
			if (!pdfViewer || (pdfPage == null && pdfPage2 == null))
			{
				return;
			}
			if (this.Annotation != null)
			{
				FS_RECTF? newRect = this.GetNewRect();
				if (newRect == null)
				{
					return;
				}
				using (mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
				{
					this.Annotation.Opacity = 1f;
					this.Annotation.Rectangle = newRect.Value;
				}
				this.Annotation.TryRedrawAnnotation(false);
				this.OnPageClientBoundsChanged();
				if (!this.Annotation.GetRECT().IntersectsWith(new FS_RECTF(0f, pdfPage.Height, pdfPage.Width, 0f)))
				{
					this.Holder.Cancel();
					return;
				}
			}
			else
			{
				using (mainViewModel.OperationManager.TraceAnnotationChange(this.LinkAnnotation.Page, ""))
				{
					this.LinkAnnotation.Rectangle = this.GetLinkNewRect().Value;
				}
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(pdfPage2.Document);
				if (pdfControl != null)
				{
					AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(pdfControl);
					int? num3;
					if (annotationHolderManager == null)
					{
						num3 = null;
					}
					else
					{
						IAnnotationHolder currentHolder = annotationHolderManager.CurrentHolder;
						if (currentHolder == null)
						{
							num3 = null;
						}
						else
						{
							PdfPage currentPage = currentHolder.CurrentPage;
							num3 = ((currentPage != null) ? new int?(currentPage.PageIndex) : null);
						}
					}
					int? num4 = num3;
					int pageIndex = pdfPage2.PageIndex;
					bool flag = (num4.GetValueOrDefault() == pageIndex) & (num4 != null);
				}
				for (int i = 0; i < 3; i++)
				{
					bool flag2 = pdfPage2.IsDisposed;
					if (!flag2)
					{
						flag2 = PdfDocumentStateService.CanDisposePage(pdfPage2);
					}
					ProgressiveStatus progressiveStatus;
					if (!flag2 && PdfObjectExtensions.TryGetProgressiveStatus(pdfPage2, out progressiveStatus))
					{
						flag2 = progressiveStatus != ProgressiveStatus.ToBeContinued && progressiveStatus != ProgressiveStatus.Failed;
					}
					if (flag2)
					{
						try
						{
							PageDisposeHelper.DisposePage(pdfPage2);
							PdfDocumentStateService.TryRedrawViewerCurrentPage(pdfPage2);
						}
						catch
						{
						}
						return;
					}
				}
			}
		}

		// Token: 0x06002672 RID: 9842 RVA: 0x000B446C File Offset: 0x000B266C
		public void OnPageClientBoundsChanged()
		{
			Rect rect;
			if (!this.ParentCanvas.PdfViewer.TryGetClientRect(this.Annotation.Page.PageIndex, this.Annotation.GetRECT(), out rect))
			{
				return;
			}
			double num = rect.Width / (double)this.Annotation.GetRECT().Width;
			PdfBorderStyle borderStyle = this.Annotation.BorderStyle;
			if (borderStyle != null)
			{
				float width = borderStyle.Width;
			}
			this.LayoutRoot.Width = rect.Width;
			this.LayoutRoot.Height = rect.Height;
			Canvas.SetLeft(this, rect.Left);
			Canvas.SetTop(this, rect.Top);
			this.ResetDraggers();
		}

		// Token: 0x06002673 RID: 9843 RVA: 0x000B4524 File Offset: 0x000B2724
		public void OnPageClientLinkBoundsChanged()
		{
			Rect rect;
			if (!this.ParentCanvas.PdfViewer.TryGetClientRect(this.LinkAnnotation.Page.PageIndex, this.LinkAnnotation.GetRECT(), out rect))
			{
				return;
			}
			double num = rect.Width / (double)this.LinkAnnotation.GetRECT().Width;
			this.LayoutRoot.Width = rect.Width;
			this.LayoutRoot.Height = rect.Height;
			Canvas.SetLeft(this, rect.Left);
			Canvas.SetTop(this, rect.Top);
			this.ResetDraggers();
		}

		// Token: 0x06002674 RID: 9844 RVA: 0x000B45C2 File Offset: 0x000B27C2
		private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ResetDraggers();
		}

		// Token: 0x06002675 RID: 9845 RVA: 0x000B45CC File Offset: 0x000B27CC
		public bool OnPropertyChanged(string propertyName)
		{
			MainViewModel mainViewModel = base.DataContext as MainViewModel;
			if (mainViewModel == null)
			{
				return false;
			}
			PdfSquareAnnotation pdfSquareAnnotation = this.Annotation as PdfSquareAnnotation;
			if (pdfSquareAnnotation != null)
			{
				if (propertyName == "ShapeFill" || propertyName == "ShapeStroke" || propertyName == "ShapeThickness")
				{
					using (mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
					{
						if (propertyName == "ShapeThickness")
						{
							if (pdfSquareAnnotation.BorderStyle == null)
							{
								pdfSquareAnnotation.BorderStyle = new PdfBorderStyle();
							}
							pdfSquareAnnotation.BorderStyle.Width = mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.ShapeThickness;
						}
						if (propertyName == "ShapeFill")
						{
							FS_COLOR fs_COLOR = ((Color)ColorConverter.ConvertFromString(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.ShapeFill)).ToPdfColor();
							pdfSquareAnnotation.InteriorColor = fs_COLOR;
						}
						if (propertyName == "ShapeStroke")
						{
							FS_COLOR fs_COLOR2 = ((Color)ColorConverter.ConvertFromString(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.ShapeStroke)).ToPdfColor();
							pdfSquareAnnotation.Color = fs_COLOR2;
						}
						pdfSquareAnnotation.RegenerateAppearances();
					}
					pdfSquareAnnotation.TryRedrawAnnotation(false);
				}
			}
			else
			{
				PdfCircleAnnotation pdfCircleAnnotation = this.Annotation as PdfCircleAnnotation;
				if (pdfCircleAnnotation != null && (propertyName == "EllipseFill" || propertyName == "EllipseStroke" || propertyName == "EllipseThickness"))
				{
					using (mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
					{
						if (propertyName == "EllipseThickness")
						{
							if (pdfCircleAnnotation.BorderStyle == null)
							{
								pdfCircleAnnotation.BorderStyle = new PdfBorderStyle();
							}
							pdfCircleAnnotation.BorderStyle.Width = mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.EllipseThickness;
						}
						if (propertyName == "EllipseFill")
						{
							FS_COLOR fs_COLOR3 = ((Color)ColorConverter.ConvertFromString(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.EllipseFill)).ToPdfColor();
							pdfCircleAnnotation.InteriorColor = fs_COLOR3;
						}
						if (propertyName == "EllipseStroke")
						{
							FS_COLOR fs_COLOR4 = ((Color)ColorConverter.ConvertFromString(mainViewModel.AnnotationToolbar.AnnotationMenuPropertyAccessor.EllipseStroke)).ToPdfColor();
							pdfCircleAnnotation.Color = fs_COLOR4;
						}
						pdfCircleAnnotation.RegenerateAppearances();
					}
					pdfCircleAnnotation.TryRedrawAnnotation(false);
				}
			}
			return false;
		}

		// Token: 0x04001089 RID: 4233
		private FS_RECTF curRect;

		// Token: 0x0400108A RID: 4234
		private bool changed;

		// Token: 0x0400108B RID: 4235
		private bool isPressed;
	}
}
