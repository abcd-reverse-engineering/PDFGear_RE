using System;
using pdfeditor.Utils.Scan;

namespace pdfeditor.Models.Scan
{
	// Token: 0x02000145 RID: 325
	public enum ScanArea
	{
		// Token: 0x04000644 RID: 1604
		[ScannerResource("ScanningAreaEntire1")]
		Auto,
		// Token: 0x04000645 RID: 1605
		[ScannerResource("ScanningAreaEntire2")]
		[PaperSize(8.5, 11.0, PaperSizeUnit.Inch)]
		Letter,
		// Token: 0x04000646 RID: 1606
		[ScannerResource("ScanningAreaEntire3")]
		[PaperSize(8.5, 14.0, PaperSizeUnit.Inch)]
		Legal,
		// Token: 0x04000647 RID: 1607
		[ScannerResource("ScanningAreaEntire6")]
		[PaperSize(148.0, 210.0, PaperSizeUnit.Millimeter)]
		A5,
		// Token: 0x04000648 RID: 1608
		[ScannerResource("ScanningAreaEntire7")]
		[PaperSize(210.0, 297.0, PaperSizeUnit.Millimeter)]
		A4,
		// Token: 0x04000649 RID: 1609
		[ScannerResource("ScanningAreaEntire8")]
		[PaperSize(297.0, 420.0, PaperSizeUnit.Millimeter)]
		A3,
		// Token: 0x0400064A RID: 1610
		[ScannerResource("ScanningAreaEntire4")]
		[PaperSize(176.0, 250.0, PaperSizeUnit.Millimeter)]
		B5,
		// Token: 0x0400064B RID: 1611
		[ScannerResource("ScanningAreaEntire5")]
		[PaperSize(250.0, 353.0, PaperSizeUnit.Millimeter)]
		B4,
		// Token: 0x0400064C RID: 1612
		[ScannerResource("ScanningAreaEntire10")]
		[PaperSize(100.0, 150.0, PaperSizeUnit.Millimeter)]
		Inch4x6,
		// Token: 0x0400064D RID: 1613
		[ScannerResource("ScanningAreaEntire9")]
		[PaperSize(130.0, 180.0, PaperSizeUnit.Millimeter)]
		Inch5x7
	}
}
