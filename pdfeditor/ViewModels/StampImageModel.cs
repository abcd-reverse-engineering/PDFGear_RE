using System;
using pdfeditor.Controls.Annotations;

namespace pdfeditor.ViewModels
{
	// Token: 0x02000059 RID: 89
	public class StampImageModel
	{
		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060004F4 RID: 1268 RVA: 0x00019B35 File Offset: 0x00017D35
		// (set) Token: 0x060004F5 RID: 1269 RVA: 0x00019B3D File Offset: 0x00017D3D
		public string ImageFilePath { get; set; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060004F6 RID: 1270 RVA: 0x00019B46 File Offset: 0x00017D46
		// (set) Token: 0x060004F7 RID: 1271 RVA: 0x00019B4E File Offset: 0x00017D4E
		public bool RemoveBackground { get; set; }

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x060004F8 RID: 1272 RVA: 0x00019B57 File Offset: 0x00017D57
		// (set) Token: 0x060004F9 RID: 1273 RVA: 0x00019B5F File Offset: 0x00017D5F
		public bool IsSignature { get; set; }

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x060004FA RID: 1274 RVA: 0x00019B68 File Offset: 0x00017D68
		// (set) Token: 0x060004FB RID: 1275 RVA: 0x00019B70 File Offset: 0x00017D70
		public ImageStampModel ImageStampControlModel { get; set; }
	}
}
