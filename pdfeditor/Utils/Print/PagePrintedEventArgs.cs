using System;

namespace pdfeditor.Utils.Print
{
	// Token: 0x020000BE RID: 190
	public class PagePrintedEventArgs : EventArgs
	{
		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06000B3D RID: 2877 RVA: 0x00039D48 File Offset: 0x00037F48
		// (set) Token: 0x06000B3E RID: 2878 RVA: 0x00039D50 File Offset: 0x00037F50
		public int PageNumber { get; private set; }

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06000B3F RID: 2879 RVA: 0x00039D59 File Offset: 0x00037F59
		// (set) Token: 0x06000B40 RID: 2880 RVA: 0x00039D61 File Offset: 0x00037F61
		public int TotalToPrint { get; set; }

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06000B41 RID: 2881 RVA: 0x00039D6A File Offset: 0x00037F6A
		// (set) Token: 0x06000B42 RID: 2882 RVA: 0x00039D72 File Offset: 0x00037F72
		public bool Cancel { get; set; }

		// Token: 0x06000B43 RID: 2883 RVA: 0x00039D7B File Offset: 0x00037F7B
		public PagePrintedEventArgs(int PageNumber, int TotalToPrint)
		{
			this.PageNumber = PageNumber;
			this.TotalToPrint = TotalToPrint;
			this.Cancel = false;
		}
	}
}
