using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace pdfconverter.Models
{
	// Token: 0x0200006F RID: 111
	public class PageMarginItem : ObservableObject
	{
		// Token: 0x060005E3 RID: 1507 RVA: 0x0001694F File Offset: 0x00014B4F
		public PageMarginItem(string cap, ContentMargin pagesize)
		{
			this.capital = cap;
			this.pdfPageSize = pagesize;
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x060005E4 RID: 1508 RVA: 0x00016965 File Offset: 0x00014B65
		public ContentMargin PDFPageSize
		{
			get
			{
				return this.pdfPageSize;
			}
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x060005E5 RID: 1509 RVA: 0x0001696D File Offset: 0x00014B6D
		public string Capital
		{
			get
			{
				return this.capital;
			}
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x00016975 File Offset: 0x00014B75
		public override string ToString()
		{
			return this.Capital;
		}

		// Token: 0x040002CE RID: 718
		private string capital;

		// Token: 0x040002CF RID: 719
		private ContentMargin pdfPageSize;
	}
}
