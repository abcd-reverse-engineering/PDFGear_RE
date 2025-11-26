using System;
using System.Windows;

namespace pdfeditor.Controls
{
	// Token: 0x020001CF RID: 463
	public class ResizeViewResizeDragEventArgs
	{
		// Token: 0x06001A4E RID: 6734 RVA: 0x00069D76 File Offset: 0x00067F76
		public ResizeViewResizeDragEventArgs(Size oldSize, Size newSize, double offsetX, double offsetY, ResizeViewOperation operation)
		{
			this.OldSize = oldSize;
			this.NewSize = newSize;
			this.OffsetX = offsetX;
			this.OffsetY = offsetY;
			this.Operation = operation;
		}

		// Token: 0x170009D9 RID: 2521
		// (get) Token: 0x06001A4F RID: 6735 RVA: 0x00069DA3 File Offset: 0x00067FA3
		public Size OldSize { get; }

		// Token: 0x170009DA RID: 2522
		// (get) Token: 0x06001A50 RID: 6736 RVA: 0x00069DAB File Offset: 0x00067FAB
		public Size NewSize { get; }

		// Token: 0x170009DB RID: 2523
		// (get) Token: 0x06001A51 RID: 6737 RVA: 0x00069DB3 File Offset: 0x00067FB3
		public double OffsetX { get; }

		// Token: 0x170009DC RID: 2524
		// (get) Token: 0x06001A52 RID: 6738 RVA: 0x00069DBB File Offset: 0x00067FBB
		public double OffsetY { get; }

		// Token: 0x170009DD RID: 2525
		// (get) Token: 0x06001A53 RID: 6739 RVA: 0x00069DC3 File Offset: 0x00067FC3
		public ResizeViewOperation Operation { get; }
	}
}
