using System;
using System.IO;
using System.Threading;
using CommonLib.Common;
using CommonLib.Config;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using PDFLauncher.Utils;
using PDFLauncher.ViewModels;

namespace PDFLauncher
{
	// Token: 0x02000009 RID: 9
	public static class Program
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00002275 File Offset: 0x00000475
		// (set) Token: 0x0600001A RID: 26 RVA: 0x0000227C File Offset: 0x0000047C
		internal static bool NeedShowRecover { get; private set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002284 File Offset: 0x00000484
		// (set) Token: 0x0600001C RID: 28 RVA: 0x0000228B File Offset: 0x0000048B
		internal static Program.OpenFileResult OpenFileOnLaunchResult { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002293 File Offset: 0x00000493
		// (set) Token: 0x0600001E RID: 30 RVA: 0x0000229A File Offset: 0x0000049A
		internal static bool AppLaunchEventsResult { get; private set; }

		// Token: 0x0600001F RID: 31 RVA: 0x000022A4 File Offset: 0x000004A4
		[STAThread]
		public static void Main(string[] args)
		{
			SqliteUtils.InitializeDatabase("pdfdata.db");
			CultureInfoUtils.Initialize();
			Program.NeedShowRecover = AutoSaveManager.IsNeedShowRecover();
			if (!Program.NeedShowRecover)
			{
				Program.OpenFileOnLaunch(args);
			}
			Program.AppLaunchEventsResult = App.AppLaunchEvents("Launch");
			if (Program.OpenFileOnLaunchResult != null && Program.OpenFileOnLaunchResult.FileOpened && Program.AppLaunchEventsResult)
			{
				Thread.Sleep(5000);
				return;
			}
			AppIdHelper.RegisterAppUserModelId();
			ServiceCollection serviceCollection = new ServiceCollection();
			serviceCollection.AddSingleton((IServiceProvider _) => new MainViewModel());
			serviceCollection.AddSingleton((IServiceProvider _) => new RecoverViewModel());
			IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
			Ioc.Default.ConfigureServices(serviceProvider);
			App app = new App();
			app.InitializeComponent();
			app.Run();
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002384 File Offset: 0x00000584
		internal static void OpenFileOnLaunch(string[] args)
		{
			if (args != null && args.Length != 0)
			{
				string text = args[0];
				string text2 = string.Empty;
				text2 = text;
				if (!string.IsNullOrEmpty(text2))
				{
					FileInfo fileInfo = text2.FileInfo;
					bool flag = FileManager.OpenOneFile(text2, null);
					Program.OpenFileOnLaunchResult = new Program.OpenFileResult(fileInfo, flag);
				}
			}
		}

		// Token: 0x0200002C RID: 44
		internal class OpenFileResult
		{
			// Token: 0x06000200 RID: 512 RVA: 0x000077BA File Offset: 0x000059BA
			public OpenFileResult(FileInfo fileInfo, bool fileOpened)
			{
				this.FileInfo = fileInfo;
				this.FileOpened = fileOpened;
			}

			// Token: 0x170000D6 RID: 214
			// (get) Token: 0x06000201 RID: 513 RVA: 0x000077D0 File Offset: 0x000059D0
			public FileInfo FileInfo { get; }

			// Token: 0x170000D7 RID: 215
			// (get) Token: 0x06000202 RID: 514 RVA: 0x000077D8 File Offset: 0x000059D8
			public bool FileOpened { get; }
		}
	}
}
