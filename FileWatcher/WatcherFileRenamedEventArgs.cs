using System;

namespace FileWatcher
{
	// Token: 0x0200000E RID: 14
	public class WatcherFileRenamedEventArgs
	{
		// Token: 0x0600002D RID: 45 RVA: 0x000028D3 File Offset: 0x00000AD3
		public WatcherFileRenamedEventArgs(string watchingPath, string oldFileName, string newFileName)
		{
			this.WatchingPath = watchingPath;
			this.OldFileName = oldFileName;
			this.NewFileName = newFileName;
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600002E RID: 46 RVA: 0x000028F0 File Offset: 0x00000AF0
		public string WatchingPath { get; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002F RID: 47 RVA: 0x000028F8 File Offset: 0x00000AF8
		public string OldFileName { get; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002900 File Offset: 0x00000B00
		public string NewFileName { get; }
	}
}
