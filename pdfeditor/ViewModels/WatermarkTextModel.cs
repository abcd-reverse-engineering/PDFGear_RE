using System;

namespace pdfeditor.ViewModels
{
	// Token: 0x0200005D RID: 93
	public class WatermarkTextModel
	{
		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x0600050F RID: 1295 RVA: 0x00019C37 File Offset: 0x00017E37
		// (set) Token: 0x06000510 RID: 1296 RVA: 0x00019C3F File Offset: 0x00017E3F
		public string Content { get; set; }

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000511 RID: 1297 RVA: 0x00019C48 File Offset: 0x00017E48
		// (set) Token: 0x06000512 RID: 1298 RVA: 0x00019C50 File Offset: 0x00017E50
		public string Foreground { get; set; }

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000513 RID: 1299 RVA: 0x00019C59 File Offset: 0x00017E59
		// (set) Token: 0x06000514 RID: 1300 RVA: 0x00019C61 File Offset: 0x00017E61
		public float FontSize { get; set; }
	}
}
