using System;

namespace pdfeditor.ViewModels
{
	// Token: 0x0200005A RID: 90
	public class StampFormControlModel
	{
		// Token: 0x170000DE RID: 222
		// (get) Token: 0x060004FD RID: 1277 RVA: 0x00019B81 File Offset: 0x00017D81
		// (set) Token: 0x060004FE RID: 1278 RVA: 0x00019B89 File Offset: 0x00017D89
		public string Name { get; set; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x060004FF RID: 1279 RVA: 0x00019B92 File Offset: 0x00017D92
		// (set) Token: 0x06000500 RID: 1280 RVA: 0x00019B9A File Offset: 0x00017D9A
		public double Width { get; set; } = 10.0;

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000501 RID: 1281 RVA: 0x00019BA3 File Offset: 0x00017DA3
		// (set) Token: 0x06000502 RID: 1282 RVA: 0x00019BAB File Offset: 0x00017DAB
		public double Height { get; set; } = 10.0;
	}
}
