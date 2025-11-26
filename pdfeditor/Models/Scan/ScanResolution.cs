using System;
using System.ComponentModel;

namespace pdfeditor.Models.Scan
{
	// Token: 0x02000144 RID: 324
	public enum ScanResolution
	{
		// Token: 0x0400063C RID: 1596
		[Description("    75 DPI")]
		Dpi75 = 75,
		// Token: 0x0400063D RID: 1597
		[Description("  100 DPI")]
		Dpi100 = 100,
		// Token: 0x0400063E RID: 1598
		[Description("  150 DPI")]
		Dpi150 = 150,
		// Token: 0x0400063F RID: 1599
		[Description("  200 DPI")]
		Dpi200 = 200,
		// Token: 0x04000640 RID: 1600
		[Description("  300 DPI (Best for documents)")]
		Dpi300 = 300,
		// Token: 0x04000641 RID: 1601
		[Description("  600 DPI")]
		Dpi600 = 600,
		// Token: 0x04000642 RID: 1602
		[Description("1200 DPI")]
		Dpi1200 = 1200
	}
}
