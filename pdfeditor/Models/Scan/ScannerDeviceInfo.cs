using System;
using NAPS2.Wia;

namespace pdfeditor.Models.Scan
{
	// Token: 0x02000140 RID: 320
	public class ScannerDeviceInfo
	{
		// Token: 0x170007D3 RID: 2003
		// (get) Token: 0x06001365 RID: 4965 RVA: 0x0004EBE0 File Offset: 0x0004CDE0
		// (set) Token: 0x06001366 RID: 4966 RVA: 0x0004EBE8 File Offset: 0x0004CDE8
		public string Id { get; set; }

		// Token: 0x170007D4 RID: 2004
		// (get) Token: 0x06001367 RID: 4967 RVA: 0x0004EBF1 File Offset: 0x0004CDF1
		// (set) Token: 0x06001368 RID: 4968 RVA: 0x0004EBF9 File Offset: 0x0004CDF9
		public string Name { get; set; }

		// Token: 0x170007D5 RID: 2005
		// (get) Token: 0x06001369 RID: 4969 RVA: 0x0004EC02 File Offset: 0x0004CE02
		// (set) Token: 0x0600136A RID: 4970 RVA: 0x0004EC0A File Offset: 0x0004CE0A
		public int Version { get; set; }

		// Token: 0x0600136B RID: 4971 RVA: 0x0004EC13 File Offset: 0x0004CE13
		public ScannerDeviceInfo()
		{
		}

		// Token: 0x0600136C RID: 4972 RVA: 0x0004EC1B File Offset: 0x0004CE1B
		public ScannerDeviceInfo(WiaDeviceInfo wia)
		{
			this.Id = wia.Id();
			this.Name = wia.Name();
			this.Version = (int)wia.Version;
		}
	}
}
