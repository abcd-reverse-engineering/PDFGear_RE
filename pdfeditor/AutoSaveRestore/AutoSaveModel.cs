using System;

namespace pdfeditor.AutoSaveRestore
{
	// Token: 0x020002C2 RID: 706
	public class AutoSaveModel
	{
		// Token: 0x17000C55 RID: 3157
		// (get) Token: 0x06002895 RID: 10389 RVA: 0x000BF257 File Offset: 0x000BD457
		// (set) Token: 0x06002896 RID: 10390 RVA: 0x000BF25F File Offset: 0x000BD45F
		public int SpanMinutes { get; set; }

		// Token: 0x17000C56 RID: 3158
		// (get) Token: 0x06002897 RID: 10391 RVA: 0x000BF268 File Offset: 0x000BD468
		// (set) Token: 0x06002898 RID: 10392 RVA: 0x000BF270 File Offset: 0x000BD470
		public bool IsAuto { get; set; }

		// Token: 0x17000C57 RID: 3159
		// (get) Token: 0x06002899 RID: 10393 RVA: 0x000BF279 File Offset: 0x000BD479
		// (set) Token: 0x0600289A RID: 10394 RVA: 0x000BF281 File Offset: 0x000BD481
		public string SourceFileName { get; set; }
	}
}
