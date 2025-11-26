using System;

namespace pdfeditor.Utils.Print
{
	// Token: 0x020000C1 RID: 193
	public class PrintPageIndexMapper
	{
		// Token: 0x06000B94 RID: 2964 RVA: 0x0003DA1C File Offset: 0x0003BC1C
		public PrintPageIndexMapper(int[] documentPageRange, int documentPageCount)
		{
			this.documentPageRange = documentPageRange;
			this.DocumentPageCount = documentPageCount;
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06000B95 RID: 2965 RVA: 0x0003DA32 File Offset: 0x0003BC32
		public int PrintPageCount
		{
			get
			{
				if (this.documentPageRange == null)
				{
					return this.DocumentPageCount;
				}
				return this.documentPageRange.Length;
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06000B96 RID: 2966 RVA: 0x0003DA4B File Offset: 0x0003BC4B
		public int PrintFromPage
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06000B97 RID: 2967 RVA: 0x0003DA4E File Offset: 0x0003BC4E
		public int DocumentPageCount { get; }

		// Token: 0x06000B98 RID: 2968 RVA: 0x0003DA56 File Offset: 0x0003BC56
		public int[] GetPageRange()
		{
			return this.documentPageRange;
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x0003DA60 File Offset: 0x0003BC60
		public bool TryGetDocumentPageIndex(int printPageIndex, int printPageStartIndex, out int documentPageIndex)
		{
			documentPageIndex = -1;
			if (this.documentPageRange == null)
			{
				documentPageIndex = printPageIndex - printPageStartIndex;
				if (documentPageIndex >= 0 && documentPageIndex < this.DocumentPageCount)
				{
					return true;
				}
				documentPageIndex = -1;
				return false;
			}
			else
			{
				if (printPageIndex >= printPageStartIndex && printPageIndex - printPageStartIndex < this.PrintPageCount)
				{
					documentPageIndex = this.documentPageRange[printPageIndex - printPageStartIndex];
					return true;
				}
				return false;
			}
		}

		// Token: 0x04000504 RID: 1284
		private readonly int[] documentPageRange;
	}
}
