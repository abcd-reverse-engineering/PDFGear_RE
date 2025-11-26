using System;
using System.Collections.Generic;
using System.Linq;
using Patagames.Pdf;
using Patagames.Pdf.Net;

namespace pdfeditor.Utils
{
	// Token: 0x02000094 RID: 148
	public class TextInfo
	{
		// Token: 0x060009B6 RID: 2486 RVA: 0x00031CC8 File Offset: 0x0002FEC8
		public TextInfo(int pageIndex, int startIndex, int endIndex, PdfTextInfo pdfTextInfo)
			: this(pageIndex, startIndex, endIndex, pdfTextInfo.Text, pdfTextInfo.Rects.ToArray<FS_RECTF>())
		{
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x00031CE6 File Offset: 0x0002FEE6
		public TextInfo(int pageIndex, int startIndex, int endIndex, string text, global::System.Collections.Generic.IReadOnlyList<FS_RECTF> rects)
		{
			this.PageIndex = pageIndex;
			this.StartIndex = startIndex;
			this.EndIndex = endIndex;
			this.Text = text;
			this.Rects = rects.ToArray<FS_RECTF>();
		}

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x060009B8 RID: 2488 RVA: 0x00031D18 File Offset: 0x0002FF18
		public int PageIndex { get; }

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x060009B9 RID: 2489 RVA: 0x00031D20 File Offset: 0x0002FF20
		public int StartIndex { get; }

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x060009BA RID: 2490 RVA: 0x00031D28 File Offset: 0x0002FF28
		public int EndIndex { get; }

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x060009BB RID: 2491 RVA: 0x00031D30 File Offset: 0x0002FF30
		public string Text { get; }

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x060009BC RID: 2492 RVA: 0x00031D38 File Offset: 0x0002FF38
		public global::System.Collections.Generic.IReadOnlyList<FS_RECTF> Rects { get; }
	}
}
