using System;

namespace pdfeditor.Controls
{
	// Token: 0x020001CE RID: 462
	[Flags]
	public enum ResizeViewOperation
	{
		// Token: 0x04000919 RID: 2329
		None = 0,
		// Token: 0x0400091A RID: 2330
		Move = 1,
		// Token: 0x0400091B RID: 2331
		LeftTop = 2,
		// Token: 0x0400091C RID: 2332
		CenterTop = 4,
		// Token: 0x0400091D RID: 2333
		RightTop = 8,
		// Token: 0x0400091E RID: 2334
		LeftCenter = 16,
		// Token: 0x0400091F RID: 2335
		RightCenter = 32,
		// Token: 0x04000920 RID: 2336
		LeftBottom = 64,
		// Token: 0x04000921 RID: 2337
		CenterBottom = 128,
		// Token: 0x04000922 RID: 2338
		RightBottom = 256,
		// Token: 0x04000923 RID: 2339
		ResizeCorner = 330,
		// Token: 0x04000924 RID: 2340
		ResizeCornerAndMove = 331,
		// Token: 0x04000925 RID: 2341
		ResizeAll = 510,
		// Token: 0x04000926 RID: 2342
		All = 511
	}
}
