using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommonLib.Common;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit.Utils;
using PDFKit.Utils.PageContents;

namespace pdfeditor.Controls.PageContents
{
	// Token: 0x02000255 RID: 597
	public partial class TextObjectEditControl : UserControl
	{
		// Token: 0x06002296 RID: 8854 RVA: 0x000A32DC File Offset: 0x000A14DC
		public TextObjectEditControl(AnnotationCanvas annotationCanvas, int pageIndex, PdfTextObject textObject)
		{
			this.InitializeComponent();
			if (annotationCanvas == null)
			{
				throw new ArgumentNullException("annotationCanvas");
			}
			this.annotationCanvas = annotationCanvas;
			this.pageIndex = pageIndex;
			if (textObject == null)
			{
				throw new ArgumentNullException("textObject");
			}
			this.textObject = textObject;
			this.textObjectBounds = this.textObject.BoundingBox;
			base.Loaded += this.TextObjectEditControl_Loaded;
		}

		// Token: 0x17000B30 RID: 2864
		// (get) Token: 0x06002297 RID: 8855 RVA: 0x000A334B File Offset: 0x000A154B
		public int PageIndex
		{
			get
			{
				return this.pageIndex;
			}
		}

		// Token: 0x17000B31 RID: 2865
		// (get) Token: 0x06002298 RID: 8856 RVA: 0x000A3353 File Offset: 0x000A1553
		public PdfTextObject TextObject
		{
			get
			{
				return this.textObject;
			}
		}

		// Token: 0x06002299 RID: 8857 RVA: 0x000A335B File Offset: 0x000A155B
		private void TextObjectEditControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.UpdatePosition();
		}

		// Token: 0x0600229A RID: 8858 RVA: 0x000A3364 File Offset: 0x000A1564
		public void UpdatePosition()
		{
			if (!base.IsLoaded)
			{
				return;
			}
			try
			{
				if (this.annotationCanvas.PdfViewer.TryGetClientRect(this.pageIndex, this.textObjectBounds, out this.deviceRect))
				{
					base.Visibility = Visibility.Visible;
					base.Width = this.deviceRect.Width;
					base.Height = this.deviceRect.Height;
					Canvas.SetLeft(this, this.deviceRect.Left);
					Canvas.SetTop(this, this.deviceRect.Top);
					this.ResizeControl.Width = this.deviceRect.Width;
					this.ResizeControl.Height = this.deviceRect.Height;
					Canvas.SetLeft(this.ResizeControl, -1.0);
					Canvas.SetTop(this.ResizeControl, -1.0);
					this.PlaceholderRect.Margin = new Thickness(1.0 - this.ResizeControl.BorderThickness.Left, 1.0 - this.ResizeControl.BorderThickness.Top, 1.0 - this.ResizeControl.BorderThickness.Right, 1.0 - this.ResizeControl.BorderThickness.Bottom);
					this.PlaceholderRect.Width = this.deviceRect.Width;
					this.PlaceholderRect.Height = this.deviceRect.Height;
					return;
				}
			}
			catch
			{
			}
			this.deviceRect = default(Rect);
			base.Visibility = Visibility.Collapsed;
		}

		// Token: 0x0600229B RID: 8859 RVA: 0x000A3528 File Offset: 0x000A1728
		private async void ResizeControl_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				if (base.IsLoaded)
				{
					e.Handled = true;
					await this.annotationCanvas.TextObjectHolder.EditSelectedTextObjectAsync();
				}
			}
		}

		// Token: 0x0600229C RID: 8860 RVA: 0x000A3568 File Offset: 0x000A1768
		private void ResizeControl_ResizeDragStarted(object sender, ResizeViewResizeDragStartedEventArgs e)
		{
			if (!base.IsLoaded)
			{
				return;
			}
			Point point;
			if (!e.Operation.HasFlag(ResizeViewOperation.Move) || !this.annotationCanvas.PdfViewer.TryGetClientPoint(this.pageIndex, this.textObject.Location.ToPoint(), out point))
			{
				this.dragStartPoint = null;
				return;
			}
			GAManager.SendEvent("TextEditor", "ResizeDragStarted", "Count", 1L);
			this.dragStartPoint = new Point?(point);
			WriteableBitmap pageObjectImage = PageContentUtils.GetPageObjectImage(this.annotationCanvas.PdfViewer.Document.Pages[this.pageIndex], this.textObject, Color.FromArgb(204, 0, 134, 237));
			if (pageObjectImage != null)
			{
				ImageBrush imageBrush = new ImageBrush
				{
					ImageSource = pageObjectImage,
					Stretch = Stretch.Fill
				};
				this.PlaceholderRect.Fill = imageBrush;
				return;
			}
			this.PlaceholderRect.Fill = null;
		}

		// Token: 0x0600229D RID: 8861 RVA: 0x000A3668 File Offset: 0x000A1868
		private async void ResizeControl_ResizeDragCompleted(object sender, ResizeViewResizeDragEventArgs e)
		{
			if (base.IsLoaded)
			{
				this.PlaceholderRect.Fill = null;
				if (this.dragStartPoint != null)
				{
					Point value = this.dragStartPoint.Value;
					Point point = new Point(value.X + e.OffsetX, value.Y + e.OffsetY);
					Point point2;
					if (this.annotationCanvas.PdfViewer.TryGetPagePoint(this.pageIndex, point, out point2))
					{
						PdfPage page = this.annotationCanvas.PdfViewer.Document.Pages[this.pageIndex];
						MainViewModel mainViewModel = base.DataContext as MainViewModel;
						if (mainViewModel != null)
						{
							this.annotationCanvas.TextObjectHolder.CancelTextObject();
							PdfTextObject pdfTextObject = await mainViewModel.OperationManager.MoveTextObjectAsync(page, this.textObject, point2.ToPdfPoint(), "");
							this.annotationCanvas.TextObjectHolder.SelectTextObject(page, pdfTextObject, true);
						}
						page = null;
					}
				}
			}
		}

		// Token: 0x0600229E RID: 8862 RVA: 0x000A36A7 File Offset: 0x000A18A7
		private void ResizeControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!base.IsLoaded)
			{
				return;
			}
			if (e.ChangedButton == MouseButton.Right)
			{
				e.Handled = true;
				this.annotationCanvas.TryShowTextObjectContextMenu();
			}
		}

		// Token: 0x04000EB4 RID: 3764
		private readonly AnnotationCanvas annotationCanvas;

		// Token: 0x04000EB5 RID: 3765
		private readonly int pageIndex;

		// Token: 0x04000EB6 RID: 3766
		private readonly PdfTextObject textObject;

		// Token: 0x04000EB7 RID: 3767
		private FS_RECTF textObjectBounds;

		// Token: 0x04000EB8 RID: 3768
		private Rect deviceRect;

		// Token: 0x04000EB9 RID: 3769
		private Point? dragStartPoint;
	}
}
