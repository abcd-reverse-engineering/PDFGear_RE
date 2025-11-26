using System;
using pdfeditor.Models.Bookmarks;

namespace pdfeditor.Controls.Bookmarks
{
	// Token: 0x02000296 RID: 662
	public class BookmarkControlSelectionChangedEventArgs
	{
		// Token: 0x17000BC4 RID: 3012
		// (get) Token: 0x06002621 RID: 9761 RVA: 0x000B223A File Offset: 0x000B043A
		public BookmarkModel Bookmark { get; }

		// Token: 0x06002622 RID: 9762 RVA: 0x000B2242 File Offset: 0x000B0442
		public BookmarkControlSelectionChangedEventArgs(BookmarkModel bookmark)
		{
			this.Bookmark = bookmark;
		}
	}
}
