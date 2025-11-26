using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002A1 RID: 673
	public partial class AnnotationTextControl : UserControl, IAnnotationControl<PdfTextAnnotation>, IAnnotationControl
	{
		// Token: 0x060026CE RID: 9934 RVA: 0x000B7B94 File Offset: 0x000B5D94
		public AnnotationTextControl(PdfTextAnnotation annot, TextAnnotationHolder holder)
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
			base.Loaded += this.AnnotationTextControl_Loaded;
		}

		// Token: 0x17000BE0 RID: 3040
		// (get) Token: 0x060026CF RID: 9935 RVA: 0x000B7BEB File Offset: 0x000B5DEB
		public PdfTextAnnotation Annotation { get; }

		// Token: 0x17000BE1 RID: 3041
		// (get) Token: 0x060026D0 RID: 9936 RVA: 0x000B7BF3 File Offset: 0x000B5DF3
		public IAnnotationHolder Holder { get; }

		// Token: 0x17000BE2 RID: 3042
		// (get) Token: 0x060026D1 RID: 9937 RVA: 0x000B7BFB File Offset: 0x000B5DFB
		public AnnotationCanvas ParentCanvas
		{
			get
			{
				return base.Parent as AnnotationCanvas;
			}
		}

		// Token: 0x17000BE3 RID: 3043
		// (get) Token: 0x060026D2 RID: 9938 RVA: 0x000B7C08 File Offset: 0x000B5E08
		PdfAnnotation IAnnotationControl.Annotation
		{
			get
			{
				return this.Annotation;
			}
		}

		// Token: 0x060026D3 RID: 9939 RVA: 0x000B7C10 File Offset: 0x000B5E10
		public void OnPageClientBoundsChanged()
		{
			Rect deviceBounds = this.Annotation.GetDeviceBounds();
			Canvas.SetLeft(this, deviceBounds.Left);
			Canvas.SetTop(this, deviceBounds.Top);
			base.Width = deviceBounds.Width;
			base.Height = deviceBounds.Height;
			this.DragResizeView.Width = deviceBounds.Width;
			this.DragResizeView.Height = deviceBounds.Height;
		}

		// Token: 0x060026D4 RID: 9940 RVA: 0x000B7C81 File Offset: 0x000B5E81
		private void AnnotationTextControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.OnPageClientBoundsChanged();
		}

		// Token: 0x060026D5 RID: 9941 RVA: 0x000B7C89 File Offset: 0x000B5E89
		public bool OnPropertyChanged(string propertyName)
		{
			return false;
		}

		// Token: 0x060026D6 RID: 9942 RVA: 0x000B7C8C File Offset: 0x000B5E8C
		private void ResizeView_ResizeDragCompleted(object sender, ResizeViewResizeDragEventArgs e)
		{
			MainViewModel mainViewModel = base.DataContext as MainViewModel;
			if (mainViewModel == null)
			{
				return;
			}
			double left = Canvas.GetLeft(this);
			double top = Canvas.GetTop(this);
			Point point = new Point(left + e.OffsetX, top + e.OffsetY);
			AnnotationCanvas parentCanvas = this.ParentCanvas;
			PdfViewer pdfViewer = ((parentCanvas != null) ? parentCanvas.PdfViewer : null);
			Point point2;
			if (pdfViewer != null && pdfViewer.TryGetPagePoint(this.Annotation.Page.PageIndex, point, out point2))
			{
				using (mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
				{
					this.Annotation.TrySetBounds(new Rect(point, new Size(this.DragResizeView.Width, this.DragResizeView.Height)));
				}
				AnnotationHolderManager holderManager = this.ParentCanvas.HolderManager;
				this.Annotation.Page.TryRedrawPageAsync(default(CancellationToken));
				AnnotationCanvas parentCanvas2 = this.ParentCanvas;
				if (parentCanvas2 != null)
				{
					parentCanvas2.PopupHolder.ClearAnnotationPopup();
				}
				AnnotationCanvas parentCanvas3 = this.ParentCanvas;
				if (parentCanvas3 != null)
				{
					parentCanvas3.PopupHolder.InitAnnotationPopup(pdfViewer.CurrentPage);
				}
				PdfPage page = this.Annotation.Page;
				if (this.Annotation.GetRECT().IntersectsWith(new FS_RECTF(0f, page.Height, page.Width, 0f)))
				{
					holderManager.Select(this.Annotation, false);
				}
			}
		}
	}
}
