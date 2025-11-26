using System;
using CommonLib.Common;

namespace FileWatcher
{
	// Token: 0x02000008 RID: 8
	internal static class SettingsHelper
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000D RID: 13 RVA: 0x00002256 File Offset: 0x00000456
		public static bool IsEnabled
		{
			get
			{
				return SettingsHelper.startupTaskHelper.IsStartupTaskEnabled;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002262 File Offset: 0x00000462
		public static string[] ListenFolders
		{
			get
			{
				return SettingsHelper.defaultListenFolders;
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002269 File Offset: 0x00000469
		public static void SetIsEnabled(bool isEnabled)
		{
			if (isEnabled)
			{
				SettingsHelper.startupTaskHelper.Enable();
				return;
			}
			SettingsHelper.startupTaskHelper.Disable();
		}

		// Token: 0x04000018 RID: 24
		private const string StartupTaskId = "FileWatcherStartupTask";

		// Token: 0x04000019 RID: 25
		private const string LastCheckUpdateTimeKey = "LastCheckUpdateTimeKey";

		// Token: 0x0400001A RID: 26
		private const string HasUpdateKey = "HasUpdateKey";

		// Token: 0x0400001B RID: 27
		private const string DisabledByServerKey = "FileWatcherDisabledByServer";

		// Token: 0x0400001C RID: 28
		private const string LastCheckDisabledKey = "FileWatcherLastCheckDisabled";

		// Token: 0x0400001D RID: 29
		private static StartupTaskHelper startupTaskHelper = new StartupTaskHelper();

		// Token: 0x0400001E RID: 30
		private static readonly string[] defaultListenFolders = new string[] { "Desktop", "Downloads", "Documents", "Foxmail" };
	}
}
