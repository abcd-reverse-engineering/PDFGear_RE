using System;

namespace pdfeditor.Controls
{
	// Token: 0x020001C4 RID: 452
	public class PdfPagePreviewGridViewItemDragCompletedEventArgs
	{
		// Token: 0x060019B4 RID: 6580 RVA: 0x0006669C File Offset: 0x0006489C
		public PdfPagePreviewGridViewItemDragCompletedEventArgs(object beforeItem, object afterItem, object[] dragItems, bool dragingContinuousRange, bool reordered)
		{
			this.BeforeItem = beforeItem;
			this.AfterItem = afterItem;
			this.DragItems = dragItems;
			this.DragingContinuousRange = dragingContinuousRange;
			this.Reordered = reordered;
		}

		// Token: 0x170009BE RID: 2494
		// (get) Token: 0x060019B5 RID: 6581 RVA: 0x000666C9 File Offset: 0x000648C9
		public object BeforeItem { get; }

		// Token: 0x170009BF RID: 2495
		// (get) Token: 0x060019B6 RID: 6582 RVA: 0x000666D1 File Offset: 0x000648D1
		public object AfterItem { get; }

		// Token: 0x170009C0 RID: 2496
		// (get) Token: 0x060019B7 RID: 6583 RVA: 0x000666D9 File Offset: 0x000648D9
		public object[] DragItems { get; }

		// Token: 0x170009C1 RID: 2497
		// (get) Token: 0x060019B8 RID: 6584 RVA: 0x000666E1 File Offset: 0x000648E1
		public bool DragingContinuousRange { get; }

		// Token: 0x170009C2 RID: 2498
		// (get) Token: 0x060019B9 RID: 6585 RVA: 0x000666E9 File Offset: 0x000648E9
		public bool Reordered { get; }
	}
}
