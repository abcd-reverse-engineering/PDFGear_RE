using System;

namespace pdfeditor.Controls
{
	// Token: 0x020001C5 RID: 453
	public class PdfPagePreviewGridViewItemEventArgs
	{
		// Token: 0x060019BA RID: 6586 RVA: 0x000666F1 File Offset: 0x000648F1
		public PdfPagePreviewGridViewItemEventArgs(object item)
		{
			this.Item = item;
		}

		// Token: 0x170009C3 RID: 2499
		// (get) Token: 0x060019BB RID: 6587 RVA: 0x00066700 File Offset: 0x00064900
		public object Item { get; }
	}
}
