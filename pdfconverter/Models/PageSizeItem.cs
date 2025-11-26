using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace pdfconverter.Models
{
	// Token: 0x02000071 RID: 113
	public class PageSizeItem : ObservableObject
	{
		// Token: 0x060005EB RID: 1515 RVA: 0x000169AB File Offset: 0x00014BAB
		public PageSizeItem(string cap, PDFPageSize pagesize)
		{
			this.capital = cap;
			this.pdfPageSize = pagesize;
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x060005EC RID: 1516 RVA: 0x000169C1 File Offset: 0x00014BC1
		public PDFPageSize PDFPageSize
		{
			get
			{
				return this.pdfPageSize;
			}
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x060005ED RID: 1517 RVA: 0x000169C9 File Offset: 0x00014BC9
		public string Capital
		{
			get
			{
				return this.capital;
			}
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x000169D1 File Offset: 0x00014BD1
		public override string ToString()
		{
			return this.Capital;
		}

		// Token: 0x040002D2 RID: 722
		private string capital;

		// Token: 0x040002D3 RID: 723
		private PDFPageSize pdfPageSize;
	}
}
