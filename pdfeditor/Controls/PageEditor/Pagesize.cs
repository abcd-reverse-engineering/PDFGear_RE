using System;
using System.Collections.Generic;
using System.Drawing.Printing;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x02000251 RID: 593
	public static class Pagesize
	{
		// Token: 0x04000E8E RID: 3726
		public static List<PaperSize> paperSizes = new List<PaperSize>
		{
			new PaperSize("A4", 827, 1169),
			new PaperSize("A3", 1169, 1654),
			new PaperSize("A2", 1654, 2339),
			new PaperSize("A1", 2339, 3311),
			new PaperSize("A0", 3311, 4681),
			new PaperSize("Arch A", 648, 864),
			new PaperSize("Arch B", 864, 1296),
			new PaperSize("Arch C", 1296, 1728),
			new PaperSize("Arch D", 1728, 2592),
			new PaperSize("Arch E", 2592, 3456),
			new PaperSize("B4", 729, 1032),
			new PaperSize("B3", 1032, 1458),
			new PaperSize("B2", 1458, 2067),
			new PaperSize("B1", 2067, 2920),
			new PaperSize("B0", 2920, 4127),
			new PaperSize("C3", 324, 458),
			new PaperSize("C4", 229, 324),
			new PaperSize("C5", 162, 229),
			new PaperSize("C6", 114, 162),
			new PaperSize("DL Envelope", 110, 220),
			new PaperSize("Executive", 725, 1050),
			new PaperSize("Folio", 840, 1250),
			new PaperSize("Ledger", 1224, 792),
			new PaperSize("Legal", 850, 1400),
			new PaperSize("Letter", 850, 1100),
			new PaperSize("Tabloid", 1100, 1700)
		};
	}
}
