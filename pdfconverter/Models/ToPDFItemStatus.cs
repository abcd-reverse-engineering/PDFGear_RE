using System;

namespace pdfconverter.Models
{
	// Token: 0x02000085 RID: 133
	public enum ToPDFItemStatus
	{
		// Token: 0x04000318 RID: 792
		Init,
		// Token: 0x04000319 RID: 793
		Loading,
		// Token: 0x0400031A RID: 794
		Loaded,
		// Token: 0x0400031B RID: 795
		LoadedFailed,
		// Token: 0x0400031C RID: 796
		Unsupport,
		// Token: 0x0400031D RID: 797
		Working,
		// Token: 0x0400031E RID: 798
		Fail,
		// Token: 0x0400031F RID: 799
		Succ,
		// Token: 0x04000320 RID: 800
		Queuing
	}
}
