using System;
using System.Collections.Generic;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;

namespace pdfeditor.Models.Bookmarks
{
	// Token: 0x02000190 RID: 400
	public class BookmarkRecord
	{
		// Token: 0x17000926 RID: 2342
		// (get) Token: 0x06001712 RID: 5906 RVA: 0x00058108 File Offset: 0x00056308
		// (set) Token: 0x06001713 RID: 5907 RVA: 0x00058110 File Offset: 0x00056310
		public int Index { get; set; }

		// Token: 0x17000927 RID: 2343
		// (get) Token: 0x06001714 RID: 5908 RVA: 0x00058119 File Offset: 0x00056319
		// (set) Token: 0x06001715 RID: 5909 RVA: 0x00058121 File Offset: 0x00056321
		public string Title { get; set; }

		// Token: 0x17000928 RID: 2344
		// (get) Token: 0x06001716 RID: 5910 RVA: 0x0005812A File Offset: 0x0005632A
		// (set) Token: 0x06001717 RID: 5911 RVA: 0x00058132 File Offset: 0x00056332
		public BookmarkRecord Parent { get; set; }

		// Token: 0x17000929 RID: 2345
		// (get) Token: 0x06001718 RID: 5912 RVA: 0x0005813B File Offset: 0x0005633B
		// (set) Token: 0x06001719 RID: 5913 RVA: 0x00058143 File Offset: 0x00056343
		public IList<BookmarkRecord> Childs { get; set; } = new List<BookmarkRecord>();

		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x0600171A RID: 5914 RVA: 0x0005814C File Offset: 0x0005634C
		// (set) Token: 0x0600171B RID: 5915 RVA: 0x00058154 File Offset: 0x00056354
		public BookmarkRecord.BookmarkDestination Destination { get; set; }

		// Token: 0x1700092B RID: 2347
		// (get) Token: 0x0600171C RID: 5916 RVA: 0x0005815D File Offset: 0x0005635D
		// (set) Token: 0x0600171D RID: 5917 RVA: 0x00058165 File Offset: 0x00056365
		public PdfAction Action { get; set; }

		// Token: 0x020005A5 RID: 1445
		public class BookmarkDestination
		{
			// Token: 0x17000D2C RID: 3372
			// (get) Token: 0x060031B8 RID: 12728 RVA: 0x000F38E5 File Offset: 0x000F1AE5
			// (set) Token: 0x060031B9 RID: 12729 RVA: 0x000F38ED File Offset: 0x000F1AED
			public string Name { get; set; }

			// Token: 0x17000D2D RID: 3373
			// (get) Token: 0x060031BA RID: 12730 RVA: 0x000F38F6 File Offset: 0x000F1AF6
			// (set) Token: 0x060031BB RID: 12731 RVA: 0x000F38FE File Offset: 0x000F1AFE
			public int PageIndex { get; set; }

			// Token: 0x17000D2E RID: 3374
			// (get) Token: 0x060031BC RID: 12732 RVA: 0x000F3907 File Offset: 0x000F1B07
			// (set) Token: 0x060031BD RID: 12733 RVA: 0x000F390F File Offset: 0x000F1B0F
			public DestinationTypes DestinationType { get; set; }

			// Token: 0x17000D2F RID: 3375
			// (get) Token: 0x060031BE RID: 12734 RVA: 0x000F3918 File Offset: 0x000F1B18
			// (set) Token: 0x060031BF RID: 12735 RVA: 0x000F3920 File Offset: 0x000F1B20
			public float? Left { get; set; }

			// Token: 0x17000D30 RID: 3376
			// (get) Token: 0x060031C0 RID: 12736 RVA: 0x000F3929 File Offset: 0x000F1B29
			// (set) Token: 0x060031C1 RID: 12737 RVA: 0x000F3931 File Offset: 0x000F1B31
			public float? Top { get; set; }

			// Token: 0x17000D31 RID: 3377
			// (get) Token: 0x060031C2 RID: 12738 RVA: 0x000F393A File Offset: 0x000F1B3A
			// (set) Token: 0x060031C3 RID: 12739 RVA: 0x000F3942 File Offset: 0x000F1B42
			public float? Right { get; set; }

			// Token: 0x17000D32 RID: 3378
			// (get) Token: 0x060031C4 RID: 12740 RVA: 0x000F394B File Offset: 0x000F1B4B
			// (set) Token: 0x060031C5 RID: 12741 RVA: 0x000F3953 File Offset: 0x000F1B53
			public float? Bottom { get; set; }

			// Token: 0x17000D33 RID: 3379
			// (get) Token: 0x060031C6 RID: 12742 RVA: 0x000F395C File Offset: 0x000F1B5C
			// (set) Token: 0x060031C7 RID: 12743 RVA: 0x000F3964 File Offset: 0x000F1B64
			public float? Zoom { get; set; }
		}
	}
}
