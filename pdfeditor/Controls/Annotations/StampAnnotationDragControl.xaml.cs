using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;
using PDFKit.Utils.StampUtils;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002A9 RID: 681
	public partial class StampAnnotationDragControl : UserControl, IAnnotationControl<PdfStampAnnotation>, IAnnotationControl
	{
		// Token: 0x06002748 RID: 10056 RVA: 0x000B9CE8 File Offset: 0x000B7EE8
		public StampAnnotationDragControl(PdfStampAnnotation annot, IAnnotationHolder holder)
		{
			this.InitializeComponent();
			this.Annotation = annot;
			this.Holder = holder;
			base.Loaded += this.StampAnnotationDragControl_Loaded;
			if (StampUtil.IsFormControl(annot))
			{
				this.DragResizeView.DragMode = ResizeViewOperation.ResizeCornerAndMove;
				this.DragResizeView.IsCompactMode = true;
			}
		}

		// Token: 0x06002749 RID: 10057 RVA: 0x000B9D45 File Offset: 0x000B7F45
		private void StampAnnotationDragControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.OnPageClientBoundsChanged();
		}

		// Token: 0x17000C07 RID: 3079
		// (get) Token: 0x0600274A RID: 10058 RVA: 0x000B9D4D File Offset: 0x000B7F4D
		public PdfStampAnnotation Annotation { get; }

		// Token: 0x17000C08 RID: 3080
		// (get) Token: 0x0600274B RID: 10059 RVA: 0x000B9D55 File Offset: 0x000B7F55
		public IAnnotationHolder Holder { get; }

		// Token: 0x17000C09 RID: 3081
		// (get) Token: 0x0600274C RID: 10060 RVA: 0x000B9D5D File Offset: 0x000B7F5D
		public AnnotationCanvas ParentCanvas
		{
			get
			{
				return (AnnotationCanvas)base.Parent;
			}
		}

		// Token: 0x17000C0A RID: 3082
		// (get) Token: 0x0600274D RID: 10061 RVA: 0x000B9D6A File Offset: 0x000B7F6A
		PdfAnnotation IAnnotationControl.Annotation
		{
			get
			{
				return this.Annotation;
			}
		}

		// Token: 0x0600274E RID: 10062 RVA: 0x000B9D74 File Offset: 0x000B7F74
		private void ResizeView_ResizeDragStarted(object sender, ResizeViewResizeDragStartedEventArgs e)
		{
			if (e.Operation == ResizeViewOperation.Move)
			{
				this.DragResizeView.DragPlaceholderFill = new SolidColorBrush(Color.FromArgb(51, 0, 122, 204));
				this.DragResizeView.BorderBrush = Brushes.Transparent;
				return;
			}
			this.DragResizeView.DragPlaceholderFill = Brushes.Transparent;
			this.DragResizeView.BorderBrush = new SolidColorBrush(Color.FromArgb(51, 0, 122, 204));
		}

		// Token: 0x0600274F RID: 10063 RVA: 0x000B9DEC File Offset: 0x000B7FEC
		private async void ResizeView_ResizeDragCompleted(object sender, ResizeViewResizeDragEventArgs e)
		{
			this.DragResizeView.BorderBrush = Brushes.Transparent;
			Canvas.SetLeft(this.AnnotationDrag, 0.0);
			Canvas.SetTop(this.AnnotationDrag, 0.0);
			MainViewModel mainViewModel = base.DataContext as MainViewModel;
			if (mainViewModel != null)
			{
				double num = Canvas.GetLeft(this);
				double num2 = Canvas.GetTop(this);
				this.LayoutRoot.Width = e.NewSize.Width;
				this.LayoutRoot.Height = e.NewSize.Height;
				num += e.OffsetX;
				num2 += e.OffsetY;
				Canvas.SetLeft(this, num);
				Canvas.SetTop(this, num2);
				bool pdfViewer = this.ParentCanvas.PdfViewer != null;
				PdfStampAnnotation annotation = this.Annotation;
				PdfPage page = ((annotation != null) ? annotation.Page : null);
				if (pdfViewer && page != null)
				{
					PdfPageObjectsCollection normalAppearance = this.Annotation.NormalAppearance;
					PdfImageObject pdfImageObject = ((normalAppearance != null) ? normalAppearance.OfType<PdfImageObject>().FirstOrDefault<PdfImageObject>() : null);
					FS_RECTF? newRect = this.GetNewRect();
					if (newRect != null)
					{
						using (mainViewModel.OperationManager.TraceAnnotationChange(this.Annotation.Page, ""))
						{
							this.Annotation.Opacity = 1f;
							this.Annotation.Rectangle = newRect.Value;
						}
					}
					if (pdfImageObject != null)
					{
						await page.TryRedrawPageAsync(default(CancellationToken));
						base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async delegate
						{
							await Task.Delay(10);
							if (this.Holder.State == AnnotationHolderState.None)
							{
								this.Holder.Select(this.Annotation, false);
							}
						}));
					}
					else
					{
						this.Annotation.TryRedrawAnnotation(false);
						this.OnPageClientBoundsChanged();
					}
					if (!this.Annotation.GetRECT().IntersectsWith(new FS_RECTF(0f, page.Height, page.Width, 0f)))
					{
						this.Holder.Cancel();
					}
				}
			}
		}

		// Token: 0x06002750 RID: 10064 RVA: 0x000B9E2C File Offset: 0x000B802C
		private FS_RECTF? GetNewRect()
		{
			AnnotationCanvas parentCanvas = this.ParentCanvas;
			PdfViewer pdfViewer = ((parentCanvas != null) ? parentCanvas.PdfViewer : null);
			double left = Canvas.GetLeft(this);
			double top = Canvas.GetTop(this);
			double width = this.LayoutRoot.Width;
			double height = this.LayoutRoot.Height;
			if (width == 0.0 || height == 0.0)
			{
				return null;
			}
			FS_RECTF fs_RECTF;
			if (pdfViewer.TryGetPageRect(this.Annotation.Page.PageIndex, new Rect(left, top, width, height), out fs_RECTF))
			{
				return new FS_RECTF?(fs_RECTF);
			}
			return null;
		}

		// Token: 0x06002751 RID: 10065 RVA: 0x000B9ED0 File Offset: 0x000B80D0
		public void OnPageClientBoundsChanged()
		{
			object dataContext = base.DataContext;
			Rect rect;
			if (!this.ParentCanvas.PdfViewer.TryGetClientRect(this.Annotation.Page.PageIndex, this.Annotation.GetRECT(), out rect))
			{
				return;
			}
			this.AnnotationDrag.Width = rect.Width;
			this.AnnotationDrag.Height = rect.Height;
			this.LayoutRoot.Width = rect.Width;
			this.LayoutRoot.Height = rect.Height;
			Canvas.SetLeft(this, rect.Left);
			Canvas.SetTop(this, rect.Top);
			this.ResetDraggers();
		}

		// Token: 0x06002752 RID: 10066 RVA: 0x000B9F7C File Offset: 0x000B817C
		private void ResetDraggers()
		{
			this.DragResizeView.Width = this.LayoutRoot.ActualWidth;
			this.DragResizeView.Height = this.LayoutRoot.ActualHeight;
		}

		// Token: 0x06002753 RID: 10067 RVA: 0x000B9FAA File Offset: 0x000B81AA
		public bool OnPropertyChanged(string propertyName)
		{
			return false;
		}

		// Token: 0x06002754 RID: 10068 RVA: 0x000B9FAD File Offset: 0x000B81AD
		private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ResetDraggers();
		}
	}
}
