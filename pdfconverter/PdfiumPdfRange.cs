using System;

namespace pdfconverter
{
	// Token: 0x0200001D RID: 29
	public class PdfiumPdfRange
	{
		// Token: 0x060000D2 RID: 210 RVA: 0x00003C25 File Offset: 0x00001E25
		public PdfiumPdfRange(string filePath, int startPageIndex, int endPageIndex, string password = null)
		{
			this.FilePath = filePath;
			this.StartPageIndex = startPageIndex;
			this.EndPageIndex = endPageIndex;
			this.Password = password;
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x00003C4A File Offset: 0x00001E4A
		public string FilePath { get; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00003C52 File Offset: 0x00001E52
		public int StartPageIndex { get; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00003C5A File Offset: 0x00001E5A
		public int EndPageIndex { get; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00003C62 File Offset: 0x00001E62
		public string Password { get; }
	}
}
