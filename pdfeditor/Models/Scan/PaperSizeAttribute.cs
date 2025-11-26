using System;

namespace pdfeditor.Models.Scan
{
	// Token: 0x0200013E RID: 318
	public class PaperSizeAttribute : Attribute
	{
		// Token: 0x170007C4 RID: 1988
		// (get) Token: 0x0600133B RID: 4923 RVA: 0x0004E73B File Offset: 0x0004C93B
		// (set) Token: 0x0600133C RID: 4924 RVA: 0x0004E743 File Offset: 0x0004C943
		public double Width { get; set; }

		// Token: 0x170007C5 RID: 1989
		// (get) Token: 0x0600133D RID: 4925 RVA: 0x0004E74C File Offset: 0x0004C94C
		// (set) Token: 0x0600133E RID: 4926 RVA: 0x0004E754 File Offset: 0x0004C954
		public double Height { get; set; }

		// Token: 0x170007C6 RID: 1990
		// (get) Token: 0x0600133F RID: 4927 RVA: 0x0004E75D File Offset: 0x0004C95D
		// (set) Token: 0x06001340 RID: 4928 RVA: 0x0004E765 File Offset: 0x0004C965
		public PaperSizeUnit Unit { get; set; }

		// Token: 0x06001341 RID: 4929 RVA: 0x0004E76E File Offset: 0x0004C96E
		public PaperSizeAttribute(double width, double height, PaperSizeUnit unit)
		{
			this.Width = width;
			this.Height = height;
			this.Unit = unit;
		}
	}
}
