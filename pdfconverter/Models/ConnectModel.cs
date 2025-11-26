using System;

namespace pdfconverter.Models
{
	// Token: 0x02000059 RID: 89
	public class ConnectModel
	{
		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06000588 RID: 1416 RVA: 0x000161C1 File Offset: 0x000143C1
		// (set) Token: 0x06000589 RID: 1417 RVA: 0x000161C9 File Offset: 0x000143C9
		public string appVersion { get; set; }

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x0600058A RID: 1418 RVA: 0x000161D2 File Offset: 0x000143D2
		// (set) Token: 0x0600058B RID: 1419 RVA: 0x000161DA File Offset: 0x000143DA
		public string utcTimestamp { get; set; }

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x0600058C RID: 1420 RVA: 0x000161E3 File Offset: 0x000143E3
		// (set) Token: 0x0600058D RID: 1421 RVA: 0x000161EB File Offset: 0x000143EB
		public string convertType { get; set; }

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x0600058E RID: 1422 RVA: 0x000161F4 File Offset: 0x000143F4
		// (set) Token: 0x0600058F RID: 1423 RVA: 0x000161FC File Offset: 0x000143FC
		public string uuid { get; set; }

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000590 RID: 1424 RVA: 0x00016205 File Offset: 0x00014405
		// (set) Token: 0x06000591 RID: 1425 RVA: 0x0001620D File Offset: 0x0001440D
		public int convertCountToday { get; set; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000592 RID: 1426 RVA: 0x00016216 File Offset: 0x00014416
		// (set) Token: 0x06000593 RID: 1427 RVA: 0x0001621E File Offset: 0x0001441E
		public string fileName { get; set; }

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x06000594 RID: 1428 RVA: 0x00016227 File Offset: 0x00014427
		// (set) Token: 0x06000595 RID: 1429 RVA: 0x0001622F File Offset: 0x0001442F
		public long fileSize { get; set; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x06000596 RID: 1430 RVA: 0x00016238 File Offset: 0x00014438
		// (set) Token: 0x06000597 RID: 1431 RVA: 0x00016240 File Offset: 0x00014440
		public int pageCount { get; set; }

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000598 RID: 1432 RVA: 0x00016249 File Offset: 0x00014449
		// (set) Token: 0x06000599 RID: 1433 RVA: 0x00016251 File Offset: 0x00014451
		public int pageFrom { get; set; }

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x0600059A RID: 1434 RVA: 0x0001625A File Offset: 0x0001445A
		// (set) Token: 0x0600059B RID: 1435 RVA: 0x00016262 File Offset: 0x00014462
		public int pageTo { get; set; }

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x0600059C RID: 1436 RVA: 0x0001626B File Offset: 0x0001446B
		// (set) Token: 0x0600059D RID: 1437 RVA: 0x00016273 File Offset: 0x00014473
		public bool needOcr { get; set; }

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x0600059E RID: 1438 RVA: 0x0001627C File Offset: 0x0001447C
		// (set) Token: 0x0600059F RID: 1439 RVA: 0x00016284 File Offset: 0x00014484
		public string OcrLang { get; set; }
	}
}
