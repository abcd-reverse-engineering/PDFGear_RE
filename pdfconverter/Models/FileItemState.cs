using System;

namespace pdfconverter.Models
{
	// Token: 0x0200006B RID: 107
	public enum FileItemState
	{
		// Token: 0x040002BE RID: 702
		Init,
		// Token: 0x040002BF RID: 703
		Loading,
		// Token: 0x040002C0 RID: 704
		Loaded,
		// Token: 0x040002C1 RID: 705
		LoadedFailed,
		// Token: 0x040002C2 RID: 706
		Unsupport,
		// Token: 0x040002C3 RID: 707
		Merging,
		// Token: 0x040002C4 RID: 708
		Fail,
		// Token: 0x040002C5 RID: 709
		Succ
	}
}
