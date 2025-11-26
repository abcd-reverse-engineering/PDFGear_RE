using System;
using System.Windows;

namespace pdfeditor.Controls
{
	// Token: 0x020001C3 RID: 451
	public class PdfPagePreviewGridViewItemDragStartEventArgs
	{
		// Token: 0x060019AF RID: 6575 RVA: 0x00066665 File Offset: 0x00064865
		public PdfPagePreviewGridViewItemDragStartEventArgs(FrameworkElement dragContainer, object[] dragItems)
		{
			this.DragContainer = dragContainer;
			this.DragItems = dragItems;
		}

		// Token: 0x170009BB RID: 2491
		// (get) Token: 0x060019B0 RID: 6576 RVA: 0x0006667B File Offset: 0x0006487B
		public FrameworkElement DragContainer { get; }

		// Token: 0x170009BC RID: 2492
		// (get) Token: 0x060019B1 RID: 6577 RVA: 0x00066683 File Offset: 0x00064883
		public object[] DragItems { get; }

		// Token: 0x170009BD RID: 2493
		// (get) Token: 0x060019B2 RID: 6578 RVA: 0x0006668B File Offset: 0x0006488B
		// (set) Token: 0x060019B3 RID: 6579 RVA: 0x00066693 File Offset: 0x00064893
		public object UIOverride { get; set; }
	}
}
