using System;

namespace pdfconverter.Models
{
	// Token: 0x02000073 RID: 115
	public enum SplitStatus
	{
		// Token: 0x040002DB RID: 731
		Init,
		// Token: 0x040002DC RID: 732
		Loading,
		// Token: 0x040002DD RID: 733
		Loaded,
		// Token: 0x040002DE RID: 734
		LoadedFailed,
		// Token: 0x040002DF RID: 735
		Unsupport,
		// Token: 0x040002E0 RID: 736
		Spliting,
		// Token: 0x040002E1 RID: 737
		Fail,
		// Token: 0x040002E2 RID: 738
		Succ
	}
}
