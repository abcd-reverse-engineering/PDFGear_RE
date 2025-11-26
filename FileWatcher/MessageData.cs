using System;

namespace FileWatcher
{
	// Token: 0x02000012 RID: 18
	internal class MessageData
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00002A78 File Offset: 0x00000C78
		// (set) Token: 0x0600003D RID: 61 RVA: 0x00002A80 File Offset: 0x00000C80
		public string From { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00002A89 File Offset: 0x00000C89
		// (set) Token: 0x0600003F RID: 63 RVA: 0x00002A91 File Offset: 0x00000C91
		public string Type { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00002A9A File Offset: 0x00000C9A
		// (set) Token: 0x06000041 RID: 65 RVA: 0x00002AA2 File Offset: 0x00000CA2
		public string Msg { get; set; }
	}
}
