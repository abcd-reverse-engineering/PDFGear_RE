using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Patagames.Pdf;
using pdfeditor.Utils;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Models.Viewer
{
	// Token: 0x0200012D RID: 301
	internal class DragOperationModel : HoverOperationModel
	{
		// Token: 0x17000796 RID: 1942
		// (get) Token: 0x06001284 RID: 4740 RVA: 0x0004B6E1 File Offset: 0x000498E1
		protected override bool RotatePreviewElementWithPage
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001285 RID: 4741 RVA: 0x0004B6E4 File Offset: 0x000498E4
		public DragOperationModel(PdfViewer viewer)
			: base(viewer)
		{
			this.rectangle = new Rectangle
			{
				Stroke = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 24, 146, byte.MaxValue)),
				StrokeThickness = 2.0,
				Fill = new SolidColorBrush(Color.FromArgb(127, byte.MaxValue, byte.MaxValue, byte.MaxValue)),
				IsHitTestVisible = false
			};
			base.PreviewElement = this.rectangle;
			viewer.OverrideCursor = Cursors.Cross;
		}

		// Token: 0x06001286 RID: 4742 RVA: 0x0004B779 File Offset: 0x00049979
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			if (base.ElementContainer != null && this.startPage == -1)
			{
				this.startPage = base.CurrentPage;
				this.startPoint = base.PositionFromDocument;
			}
		}

		// Token: 0x06001287 RID: 4743 RVA: 0x0004B7AB File Offset: 0x000499AB
		protected override void OnMouseMove(MouseEventArgs e)
		{
		}

		// Token: 0x06001288 RID: 4744 RVA: 0x0004B7B0 File Offset: 0x000499B0
		protected override void UpdatePagePosition()
		{
			Point? point = null;
			base.UpdatePagePosition();
			Border elementContainer = base.ElementContainer;
			Point point2;
			if (base.CurrentPage != -1 && elementContainer != null && this.startPage != -1 && base.Viewer.TryGetClientPoint(this.startPage, this.startPoint.ToPoint(), out point2))
			{
				base.CurrentPage = -1;
				Point mousePositionFromViewer = base.MousePositionFromViewer;
				Point point3 = new Point(Math.Min(point2.X, mousePositionFromViewer.X), Math.Min(point2.Y, mousePositionFromViewer.Y));
				Point point4;
				FS_RECTF fs_RECTF;
				if (base.Viewer.TryGetPagePoint(this.startPage, point3, out point4) && base.Viewer.TryGetPageRect(this.startPage, new Rect(point2, mousePositionFromViewer), out fs_RECTF))
				{
					base.CurrentPage = this.startPage;
					base.PositionFromDocument = new FS_POINTF(fs_RECTF.left, fs_RECTF.top);
					base.SizeInDocument = new FS_SIZEF(fs_RECTF.Width, fs_RECTF.Height);
					point = new Point?(point3);
				}
			}
			if (point != null)
			{
				Canvas.SetLeft(base.ElementContainer, point.Value.X);
				Canvas.SetTop(base.ElementContainer, point.Value.Y);
			}
		}

		// Token: 0x06001289 RID: 4745 RVA: 0x0004B908 File Offset: 0x00049B08
		protected override void UpdatePreview()
		{
			if (base.IsDisposed)
			{
				throw new ObjectDisposedException("DataOperationModel");
			}
			if (base.IsCompleted)
			{
				return;
			}
			Point mousePositionFromViewer = base.MousePositionFromViewer;
			int currentPage = base.CurrentPage;
			Border elementContainer = base.ElementContainer;
			Point? point = null;
			Point point2;
			if (elementContainer != null && this.startPage != -1 && base.Viewer.TryGetClientPoint(this.startPage, this.startPoint.ToPoint(), out point2))
			{
				Point mousePositionFromViewer2 = base.MousePositionFromViewer;
				Point point3 = new Point(Math.Min(point2.X, mousePositionFromViewer2.X), Math.Min(point2.Y, mousePositionFromViewer2.Y));
				Point point4;
				FS_RECTF fs_RECTF;
				if (base.Viewer.TryGetPagePoint(this.startPage, point3, out point4) && base.Viewer.TryGetPageRect(this.startPage, new Rect(point2, mousePositionFromViewer2), out fs_RECTF))
				{
					base.PositionFromDocument = new FS_POINTF(fs_RECTF.left, fs_RECTF.top);
					base.SizeInDocument = new FS_SIZEF(fs_RECTF.Width, fs_RECTF.Height);
					point = new Point?(point3);
					Size previewSize = base.GetPreviewSize(currentPage);
					elementContainer.Width = previewSize.Width;
					elementContainer.Height = previewSize.Height;
					FrameworkElement frameworkElement = base.PreviewElement as FrameworkElement;
					if (frameworkElement != null)
					{
						frameworkElement.Width = previewSize.Width;
						frameworkElement.Height = previewSize.Height;
					}
				}
			}
			if (point != null)
			{
				Canvas.SetLeft(base.ElementContainer, point.Value.X);
				Canvas.SetTop(base.ElementContainer, point.Value.Y);
			}
		}

		// Token: 0x0600128A RID: 4746 RVA: 0x0004BABC File Offset: 0x00049CBC
		protected override void OnCompleted(bool success, bool result)
		{
			if (success)
			{
				this.UpdatePagePosition();
			}
			base.OnCompleted(success, result);
		}

		// Token: 0x040005DF RID: 1503
		private Rectangle rectangle;

		// Token: 0x040005E0 RID: 1504
		private int startPage = -1;

		// Token: 0x040005E1 RID: 1505
		private FS_POINTF startPoint;
	}
}
