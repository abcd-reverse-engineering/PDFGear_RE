using System;
using System.Windows.Input;
using PDFKit;

namespace pdfeditor.Models.Viewer
{
	// Token: 0x0200012E RID: 302
	internal class HoverOperationModel : DataOperationModel
	{
		// Token: 0x0600128B RID: 4747 RVA: 0x0004BACF File Offset: 0x00049CCF
		public HoverOperationModel(PdfViewer viewer)
			: base(viewer)
		{
		}

		// Token: 0x0600128C RID: 4748 RVA: 0x0004BAD8 File Offset: 0x00049CD8
		public HoverOperationModel(PdfViewer viewer, Cursor cursor)
			: base(viewer, cursor)
		{
		}

		// Token: 0x0600128D RID: 4749 RVA: 0x0004BAE2 File Offset: 0x00049CE2
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			e.Handled = true;
			if (e.ChangedButton == MouseButton.Right)
			{
				this.OnCompleted(false, false);
			}
		}

		// Token: 0x0600128E RID: 4750 RVA: 0x0004BB03 File Offset: 0x00049D03
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			e.Handled = true;
			if (e.ChangedButton == MouseButton.Left && base.CurrentPage != -1)
			{
				this.OnCompleted(true, true);
			}
		}

		// Token: 0x0600128F RID: 4751 RVA: 0x0004BB2C File Offset: 0x00049D2C
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (e.Key == Key.Escape)
			{
				e.Handled = true;
				this.OnCompleted(false, false);
				return;
			}
			if (e.Key == Key.Return && base.CurrentPage != -1)
			{
				e.Handled = true;
				this.OnCompleted(true, true);
			}
		}
	}
}
