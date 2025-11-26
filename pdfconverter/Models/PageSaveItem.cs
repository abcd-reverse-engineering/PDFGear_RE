using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace pdfconverter.Models
{
	// Token: 0x02000070 RID: 112
	public class PageSaveItem : ObservableObject
	{
		// Token: 0x060005E7 RID: 1511 RVA: 0x0001697D File Offset: 0x00014B7D
		public PageSaveItem(string cap, SaveFileWay pagesize)
		{
			this.capital = cap;
			this.pdfPageSize = pagesize;
		}

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x060005E8 RID: 1512 RVA: 0x00016993 File Offset: 0x00014B93
		public SaveFileWay PDFPageSize
		{
			get
			{
				return this.pdfPageSize;
			}
		}

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x060005E9 RID: 1513 RVA: 0x0001699B File Offset: 0x00014B9B
		public string Capital
		{
			get
			{
				return this.capital;
			}
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x000169A3 File Offset: 0x00014BA3
		public override string ToString()
		{
			return this.Capital;
		}

		// Token: 0x040002D0 RID: 720
		private string capital;

		// Token: 0x040002D1 RID: 721
		private SaveFileWay pdfPageSize;
	}
}
