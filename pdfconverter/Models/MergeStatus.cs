using System;

namespace pdfconverter.Models
{
	// Token: 0x0200006A RID: 106
	public enum MergeStatus
	{
		// Token: 0x040002B5 RID: 693
		Init,
		// Token: 0x040002B6 RID: 694
		Loading,
		// Token: 0x040002B7 RID: 695
		Loaded,
		// Token: 0x040002B8 RID: 696
		LoadedFailed,
		// Token: 0x040002B9 RID: 697
		Unsupport,
		// Token: 0x040002BA RID: 698
		Merging,
		// Token: 0x040002BB RID: 699
		Fail,
		// Token: 0x040002BC RID: 700
		Succ
	}
}
