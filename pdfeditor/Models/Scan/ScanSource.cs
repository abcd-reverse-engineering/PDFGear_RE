using System;
using pdfeditor.Utils.Scan;

namespace pdfeditor.Models.Scan
{
	// Token: 0x02000142 RID: 322
	public enum ScanSource
	{
		// Token: 0x04000635 RID: 1589
		[ScannerResource("ScanningPaperSourceGlass")]
		Glass,
		// Token: 0x04000636 RID: 1590
		[ScannerResource("ScanningPaperSourceFeeder")]
		Feeder,
		// Token: 0x04000637 RID: 1591
		[ScannerResource("ScanningPaperSourceDuplex")]
		Duplex
	}
}
