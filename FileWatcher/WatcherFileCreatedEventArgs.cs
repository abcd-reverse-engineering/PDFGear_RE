using System;

namespace FileWatcher
{
	// Token: 0x0200000D RID: 13
	public class WatcherFileCreatedEventArgs
	{
		// Token: 0x0600002A RID: 42 RVA: 0x000028AD File Offset: 0x00000AAD
		public WatcherFileCreatedEventArgs(string watchingPath, string createdFile)
		{
			this.WatchingPath = watchingPath;
			this.CreatedFile = createdFile;
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000028C3 File Offset: 0x00000AC3
		public string WatchingPath { get; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600002C RID: 44 RVA: 0x000028CB File Offset: 0x00000ACB
		public string CreatedFile { get; }
	}
}
