using System;

namespace pdfeditor.Models.PageContents
{
	// Token: 0x02000151 RID: 337
	public class ImageMatrix
	{
		// Token: 0x06001431 RID: 5169 RVA: 0x000506CC File Offset: 0x0004E8CC
		public ImageMatrix()
		{
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x000506D4 File Offset: 0x0004E8D4
		public ImageMatrix(float a, float b, float c, float d, float e, float f)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
			this.e = e;
			this.f = f;
		}

		// Token: 0x0400069F RID: 1695
		public float a;

		// Token: 0x040006A0 RID: 1696
		public float b;

		// Token: 0x040006A1 RID: 1697
		public float c;

		// Token: 0x040006A2 RID: 1698
		public float d;

		// Token: 0x040006A3 RID: 1699
		public float e;

		// Token: 0x040006A4 RID: 1700
		public float f;
	}
}
