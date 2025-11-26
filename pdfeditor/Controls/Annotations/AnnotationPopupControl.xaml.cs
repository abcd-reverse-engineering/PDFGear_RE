using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Utils;
using pdfeditor.Utils.Behaviors;
using pdfeditor.ViewModels;
using PDFKit.Utils;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x0200029F RID: 671
	public partial class AnnotationPopupControl : UserControl
	{
		// Token: 0x060026BC RID: 9916 RVA: 0x000B7608 File Offset: 0x000B5808
		public AnnotationPopupControl(AnnotationCanvas annotationCanvas, PopupAnnotationWrapper wrapper)
		{
			this.annotationCanvas = annotationCanvas;
			if (wrapper == null)
			{
				throw new ArgumentNullException("wrapper");
			}
			this.wrapper = wrapper;
			this.InitializeComponent();
			this.LayoutRoot.DataContext = wrapper;
			this.PopupContentTextBehavior.Text = wrapper.Contents;
			base.Loaded += this.AnnotationPopupControl_Loaded;
			base.SizeChanged += this.AnnotationPopupControl_SizeChanged;
		}

		// Token: 0x17000BDF RID: 3039
		// (get) Token: 0x060026BD RID: 9917 RVA: 0x000B767F File Offset: 0x000B587F
		public PopupAnnotationWrapper Wrapper
		{
			get
			{
				return this.wrapper;
			}
		}

		// Token: 0x060026BE RID: 9918 RVA: 0x000B7687 File Offset: 0x000B5887
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.wrapper.IsOpen = false;
		}

		// Token: 0x060026BF RID: 9919 RVA: 0x000B7695 File Offset: 0x000B5895
		private void ResizeView_ResizeDragStarted(object sender, ResizeViewResizeDragStartedEventArgs e)
		{
			Panel.SetZIndex(this.DragResizeView, 1);
		}

		// Token: 0x060026C0 RID: 9920 RVA: 0x000B76A4 File Offset: 0x000B58A4
		private void ResizeView_ResizeDragCompleted(object sender, ResizeViewResizeDragEventArgs e)
		{
			if (base.IsLoaded)
			{
				MainViewModel mainViewModel = this.mainViewModel;
				if (((mainViewModel != null) ? mainViewModel.Document : null) != null)
				{
					Panel.SetZIndex(this.DragResizeView, 0);
					double num = Canvas.GetLeft(this);
					double num2 = Canvas.GetTop(this);
					num += e.OffsetX;
					num2 += e.OffsetY;
					DpiScale dpi = VisualTreeHelper.GetDpi(this);
					Point point = new Point(num, num2);
					Point point2;
					if (!this.annotationCanvas.PdfViewer.TryGetPagePoint(this.wrapper.Page.PageIndex, point, out point2))
					{
						return;
					}
					FS_RECTF rectangle = this.wrapper.Rectangle;
					FS_RECTF fs_RECTF = new FS_RECTF(point2.X, point2.Y, point2.X + (double)rectangle.Width, point2.Y - (double)rectangle.Height);
					if (e.Operation != ResizeViewOperation.Move)
					{
						double num3 = Math.Max(10.0, e.NewSize.Width / dpi.PixelsPerDip);
						double num4 = Math.Max(10.0, e.NewSize.Height / dpi.PixelsPerDip);
						fs_RECTF.right = (float)((double)fs_RECTF.left + num3);
						fs_RECTF.bottom = (float)((double)fs_RECTF.top - num4);
					}
					using (this.mainViewModel.OperationManager.TraceAnnotationChange(this.wrapper.Page, ""))
					{
						this.wrapper.Rectangle = fs_RECTF;
					}
					PopupAnnotationCollection popupAnnotationCollection = base.Parent as PopupAnnotationCollection;
					if (popupAnnotationCollection != null)
					{
						popupAnnotationCollection.UpdatePosition();
					}
					this.annotationCanvas.UpdateViewerFlyoutExtendWidth();
					return;
				}
			}
		}

		// Token: 0x060026C1 RID: 9921 RVA: 0x000B7860 File Offset: 0x000B5A60
		private void TextBoxEditBehavior_TextChanged(object sender, EventArgs e)
		{
			MainViewModel mainViewModel = this.mainViewModel;
			if (((mainViewModel != null) ? mainViewModel.Document : null) == null)
			{
				return;
			}
			if (((TextBoxEditBehavior)sender).Text == this.wrapper.Contents)
			{
				return;
			}
			using (this.mainViewModel.OperationManager.TraceAnnotationChange(this.wrapper.Page, ""))
			{
				this.wrapper.Contents = ((TextBoxEditBehavior)sender).Text;
			}
			PageEditorViewModel pageEditors = this.mainViewModel.PageEditors;
			if (pageEditors == null)
			{
				return;
			}
			PopupAnnotationWrapper popupAnnotationWrapper = this.wrapper;
			int? num;
			if (popupAnnotationWrapper == null)
			{
				num = null;
			}
			else
			{
				PdfPopupAnnotation annotation = popupAnnotationWrapper.Annotation;
				if (annotation == null)
				{
					num = null;
				}
				else
				{
					PdfPage page = annotation.Page;
					num = ((page != null) ? new int?(page.PageIndex) : null);
				}
			}
			int? num2 = num;
			pageEditors.NotifyPageAnnotationChanged(num2.GetValueOrDefault(-1));
		}

		// Token: 0x060026C2 RID: 9922 RVA: 0x000B7958 File Offset: 0x000B5B58
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			this.annotationCanvas.PopupHolder.SetPopupHovered(this.wrapper.Annotation, true);
		}

		// Token: 0x060026C3 RID: 9923 RVA: 0x000B797D File Offset: 0x000B5B7D
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			this.annotationCanvas.PopupHolder.SetPopupHovered(this.wrapper.Annotation, false);
		}

		// Token: 0x060026C4 RID: 9924 RVA: 0x000B79A4 File Offset: 0x000B5BA4
		protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			if (!typeof(TextBoxBase).IsAssignableFrom(e.OriginalSource.GetType()))
			{
				this.annotationCanvas.HolderManager.Select(this.wrapper.Parent, false);
			}
		}

		// Token: 0x060026C5 RID: 9925 RVA: 0x000B79F0 File Offset: 0x000B5BF0
		private void AnnotationPopupControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.mainViewModel = base.DataContext as MainViewModel;
		}

		// Token: 0x060026C6 RID: 9926 RVA: 0x000B7A04 File Offset: 0x000B5C04
		private void AnnotationPopupControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Width < 170.0)
			{
				this.ModificationDateTextShort.Visibility = Visibility.Visible;
				this.ModificationDateText.Visibility = Visibility.Collapsed;
				return;
			}
			this.ModificationDateTextShort.Visibility = Visibility.Collapsed;
			this.ModificationDateText.Visibility = Visibility.Visible;
		}

		// Token: 0x060026C7 RID: 9927 RVA: 0x000B7A5B File Offset: 0x000B5C5B
		public void Apply()
		{
			this.PopupContentTextBehavior.Apply();
		}

		// Token: 0x040010C4 RID: 4292
		private readonly AnnotationCanvas annotationCanvas;

		// Token: 0x040010C5 RID: 4293
		private PopupAnnotationWrapper wrapper;

		// Token: 0x040010C6 RID: 4294
		private MainViewModel mainViewModel;
	}
}
