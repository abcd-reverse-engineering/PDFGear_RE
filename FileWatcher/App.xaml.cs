using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using CommonLib.AppTheme;
using CommonLib.Common;
using FileWatcher.Views;

namespace FileWatcher
{
	// Token: 0x02000013 RID: 19
	public partial class App : global::System.Windows.Application
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00002AB3 File Offset: 0x00000CB3
		internal Watcher Watcher
		{
			get
			{
				return this.watcher;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00002ABB File Offset: 0x00000CBB
		public new static App Current
		{
			get
			{
				return (App)global::System.Windows.Application.Current;
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002AC8 File Offset: 0x00000CC8
		public App()
		{
			this.systemThemeListener = new SystemThemeListener(base.Dispatcher);
			this.systemThemeListener.ActualAppThemeChanged += delegate(object s, EventArgs a)
			{
				ThemeResourceDictionary.GetForCurrentApp().Theme = this.GetCurrentActualAppTheme();
			};
			WindowThemeHelper.Initialize();
			ProcessMessageHelper.MessageReceived += this.ProcessMessageHelper_MessageReceived;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002B25 File Offset: 0x00000D25
		private void ProcessMessageHelper_MessageReceived(object sender, ProcessMessageReceivedEventArgs e)
		{
			if (e.Message == "UpdateTheme")
			{
				base.Dispatcher.InvokeAsync(delegate
				{
					ThemeResourceDictionary.GetForCurrentApp().Theme = this.GetCurrentActualAppTheme();
				});
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002B54 File Offset: 0x00000D54
		public string GetCurrentActualAppTheme()
		{
			string currentAppTheme = ConfigManager.GetCurrentAppTheme();
			string text = ((currentAppTheme != null) ? currentAppTheme.ToLowerInvariant() : null);
			if (!(text == "light") && (text == null || text.Length != 0) && text != null)
			{
				if (text == "dark")
				{
					return "Dark";
				}
				if (text == "auto")
				{
					if (this.systemThemeListener.ActualAppTheme != SystemThemeListener.ActualTheme.Light)
					{
						return "Dark";
					}
					return "Light";
				}
			}
			return "Light";
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002BD0 File Offset: 0x00000DD0
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
			contextMenuStrip.Items.Add(new ToolStripMenuItem("Settings"));
			contextMenuStrip.Items.Add(new ToolStripMenuItem("Exit"));
			ThemeResourceDictionary.GetForCurrentApp().Theme = this.GetCurrentActualAppTheme();
			contextMenuStrip.ItemClicked += this.ContextMenu_ItemClicked;
			this.watcher = new Watcher();
			this.watcher.FileCreated += this.Watcher_FileCreated;
			this.watcher.FileRenamed += this.Watcher_FileRenamed;
			this.UpdateListenFolders();
			this.msgListener = new WindowMessageListener();
			this.msgListener.MessageReceived += this.MsgListener_MessageReceived;
			GAManager2.SendEvent("FileWatcher", "FileWatcherServiceOn", "Count", 1L);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002CB0 File Offset: 0x00000EB0
		public void UpdateListenFolders()
		{
			if (this.Watcher == null)
			{
				return;
			}
			this.Watcher.Clear();
			foreach (string text in SettingsHelper.ListenFolders)
			{
				string text2 = "";
				if (!(text == "Desktop"))
				{
					if (!(text == "Downloads"))
					{
						if (!(text == "Documents"))
						{
							if (text == "Foxmail")
							{
								text2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Foxmail7");
							}
						}
						else
						{
							text2 = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
						}
					}
					else
					{
						text2 = KnownFolders.GetPath(KnownFolder.Downloads);
					}
				}
				else
				{
					text2 = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
				}
				this.Watcher.AddPath(text2, "*.pdf", true);
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002D6C File Offset: 0x00000F6C
		private void MsgListener_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			string type = e.MessageData.Type;
			if (type == "exit")
			{
				global::System.Windows.Application.Current.Shutdown();
				return;
			}
			if (!(type == "restart"))
			{
				return;
			}
			SingleInstance.TryReleaseMutex();
			Process.Start(Process.GetCurrentProcess().MainModule.FileName);
			global::System.Windows.Application.Current.Shutdown();
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002DCF File Offset: 0x00000FCF
		private void Watcher_FileCreated(Watcher sender, WatcherFileCreatedEventArgs args)
		{
			this.ProcessFile(args.CreatedFile);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002DE0 File Offset: 0x00000FE0
		private void Watcher_FileRenamed(Watcher sender, WatcherFileRenamedEventArgs args)
		{
			try
			{
				string oldFileName = args.OldFileName;
				string newFileName = args.NewFileName;
				if (newFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) && !oldFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
				{
					this.ProcessFile(newFileName);
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002E34 File Offset: 0x00001034
		private void ProcessFile(string fileName)
		{
			App.<>c__DisplayClass18_0 CS$<>8__locals1 = new App.<>c__DisplayClass18_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.fileName = fileName;
			if (string.IsNullOrEmpty(CS$<>8__locals1.fileName))
			{
				return;
			}
			CS$<>8__locals1.tokenSource = null;
			object obj = this.openNotifyWindowlocker;
			lock (obj)
			{
				CancellationTokenSource cancellationTokenSource = this.openNotifyWindowCancellationToken;
				if (cancellationTokenSource != null)
				{
					cancellationTokenSource.Cancel();
				}
				CS$<>8__locals1.tokenSource = new CancellationTokenSource();
				this.openNotifyWindowCancellationToken = CS$<>8__locals1.tokenSource;
			}
			base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				App.<>c__DisplayClass18_0.<<ProcessFile>b__0>d <<ProcessFile>b__0>d;
				<<ProcessFile>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<ProcessFile>b__0>d.<>4__this = CS$<>8__locals1;
				<<ProcessFile>b__0>d.<>1__state = -1;
				<<ProcessFile>b__0>d.<>t__builder.Start<App.<>c__DisplayClass18_0.<<ProcessFile>b__0>d>(ref <<ProcessFile>b__0>d);
			}));
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002ED8 File Offset: 0x000010D8
		private void ContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			ToolStripItem clickedItem = e.ClickedItem;
			if (clickedItem != null)
			{
				string text = clickedItem.Text;
				if (text == "Settings")
				{
					SettingsView settingsView = base.Windows.OfType<SettingsView>().FirstOrDefault<SettingsView>();
					if (settingsView == null)
					{
						settingsView = new SettingsView();
					}
					settingsView.Show();
					settingsView.Activate();
					return;
				}
				if (!(text == "Exit"))
				{
					return;
				}
				global::System.Windows.Application.Current.Shutdown();
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002F44 File Offset: 0x00001144
		protected override void OnExit(ExitEventArgs e)
		{
			SingleInstance.TryReleaseMutex();
			this.msgListener.MessageReceived -= this.MsgListener_MessageReceived;
			this.msgListener.DestroyHandle();
			this.msgListener = null;
			Watcher watcher = this.watcher;
			if (watcher != null)
			{
				watcher.Dispose();
			}
			this.watcher = null;
			base.OnExit(e);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00002FD0 File Offset: 0x000011D0
		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public static void Main()
		{
			App app = new App();
			app.InitializeComponent();
			app.Run();
		}

		// Token: 0x04000034 RID: 52
		private NotifyIcon notifyIcon;

		// Token: 0x04000035 RID: 53
		private Watcher watcher;

		// Token: 0x04000036 RID: 54
		private CancellationTokenSource openNotifyWindowCancellationToken;

		// Token: 0x04000037 RID: 55
		private object openNotifyWindowlocker = new object();

		// Token: 0x04000038 RID: 56
		private WindowMessageListener msgListener;

		// Token: 0x04000039 RID: 57
		private SystemThemeListener systemThemeListener;
	}
}
