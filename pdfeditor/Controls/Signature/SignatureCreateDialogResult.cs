using System;

namespace pdfeditor.Controls.Signature
{
	// Token: 0x020001F8 RID: 504
	public class SignatureCreateDialogResult
	{
		// Token: 0x17000A2C RID: 2604
		// (get) Token: 0x06001C61 RID: 7265 RVA: 0x00076AD7 File Offset: 0x00074CD7
		// (set) Token: 0x06001C62 RID: 7266 RVA: 0x00076ADF File Offset: 0x00074CDF
		public string ImageFilePath { get; set; }

		// Token: 0x17000A2D RID: 2605
		// (get) Token: 0x06001C63 RID: 7267 RVA: 0x00076AE8 File Offset: 0x00074CE8
		// (set) Token: 0x06001C64 RID: 7268 RVA: 0x00076AF0 File Offset: 0x00074CF0
		public bool RemoveBackground { get; set; }
	}
}
