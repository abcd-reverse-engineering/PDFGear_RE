using System;

namespace pdfconverter.Models
{
	// Token: 0x0200006E RID: 110
	public class OnlineAuthResponsModel
	{
		// Token: 0x17000209 RID: 521
		// (get) Token: 0x060005DC RID: 1500 RVA: 0x000168FE File Offset: 0x00014AFE
		// (set) Token: 0x060005DD RID: 1501 RVA: 0x00016906 File Offset: 0x00014B06
		public bool Success { get; set; }

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x060005DE RID: 1502 RVA: 0x0001690F File Offset: 0x00014B0F
		// (set) Token: 0x060005DF RID: 1503 RVA: 0x00016917 File Offset: 0x00014B17
		public string Token { get; set; } = string.Empty;

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x060005E0 RID: 1504 RVA: 0x00016920 File Offset: 0x00014B20
		// (set) Token: 0x060005E1 RID: 1505 RVA: 0x00016928 File Offset: 0x00014B28
		public string Message { get; set; } = string.Empty;
	}
}
