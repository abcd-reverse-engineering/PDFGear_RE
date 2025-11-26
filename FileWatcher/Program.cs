using System;
using CommonLib.Common;
using CommonLib.Config;

namespace FileWatcher
{
	// Token: 0x02000007 RID: 7
	public class Program
	{
		// Token: 0x0600000B RID: 11 RVA: 0x0000221D File Offset: 0x0000041D
		[STAThread]
		public static void Main()
		{
			if (!SingleInstance.IsMainInstance || !SettingsHelper.IsEnabled)
			{
				return;
			}
			SqliteUtils.InitializeDatabase("pdfdata.db");
			CultureInfoUtils.Initialize();
			App app = new App();
			app.InitializeComponent();
			app.Run();
		}
	}
}
