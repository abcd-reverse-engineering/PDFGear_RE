using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommonLib.Account;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommonLib.Properties;
using CommunityToolkit.Mvvm.DependencyInjection;
using PDFLauncher.Properties;
using PDFLauncher.Utils;
using PDFLauncher.ViewModels;

namespace PDFLauncher
{
	// Token: 0x0200000E RID: 14
	public partial class App : Application
	{
		// Token: 0x0600003F RID: 63 RVA: 0x00002B6C File Offset: 0x00000D6C
		public App()
		{
			PDFLauncher.Properties.Resources.Culture = CultureInfo.CurrentUICulture;
			CommonLib.Properties.Resources.Culture = CultureInfo.CurrentUICulture;
			this.systemThemeListener = new SystemThemeListener(base.Dispatcher);
			this.systemThemeListener.ActualAppThemeChanged += delegate(object s, EventArgs a)
			{
				ThemeResourceDictionary.GetForCurrentApp().Theme = this.GetCurrentActualAppTheme();
			};
			WindowThemeHelper.Initialize();
			ProcessMessageHelper.MessageReceived += this.ProcessMessageHelper_MessageReceived;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002BD4 File Offset: 0x00000DD4
		private void ProcessMessageHelper_MessageReceived(object sender, ProcessMessageReceivedEventArgs e)
		{
			if (e.Message == "UpdateTheme")
			{
				base.Dispatcher.InvokeAsync(delegate
				{
					ThemeResourceDictionary.GetForCurrentApp().Theme = this.GetCurrentActualAppTheme();
				});
				return;
			}
			if (e.Message == "UpdateFileHistory")
			{
				base.Dispatcher.InvokeAsync<Task>(async delegate
				{
					await Ioc.Default.GetRequiredService<MainViewModel>().ReadHistory();
				});
				return;
			}
			if (e.Message == "UserInfoUpdated")
			{
				base.Dispatcher.InvokeAsync(delegate
				{
					this.UpdateUserInfo();
				}, DispatcherPriority.Send);
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002C78 File Offset: 0x00000E78
		private string GetCurrentActualAppTheme()
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

		// Token: 0x06000042 RID: 66 RVA: 0x00002CF4 File Offset: 0x00000EF4
		protected override async void OnStartup(StartupEventArgs e)
		{
			App.<>c__DisplayClass5_0 CS$<>8__locals1 = new App.<>c__DisplayClass5_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.sleepInt = 3000;
			DispatcherHelper.Initialize();
			base.OnStartup(e);
			UpdateHelper.RemoveUpdateDialogShownFlag();
			ThemeResourceDictionary.GetForCurrentApp().Theme = this.GetCurrentActualAppTheme();
			if (!Program.AppLaunchEventsResult)
			{
				App.OpenUpdateAdConfigWindow();
			}
			else
			{
				bool flag = IAPUtils.NeedToShowIAP();
				if (Program.NeedShowRecover)
				{
					this.appLifeWin = new AppLifeWindow();
					PDFLauncher.Utils.AppManager.ShowRecoverWindows();
				}
				if (Program.OpenFileOnLaunchResult == null)
				{
					Program.OpenFileOnLaunch(e.Args);
				}
				if (Program.OpenFileOnLaunchResult == null || !Program.OpenFileOnLaunchResult.FileOpened)
				{
					CS$<>8__locals1.sleepInt = 500;
					MainWindow mainWindow = new MainWindow();
					mainWindow.Show();
					Application.Current.MainWindow = mainWindow;
					if (this.appLifeWin != null)
					{
						this.appLifeWin.Close();
						this.appLifeWin = null;
					}
				}
				App.OpenUpdateAdConfigWindow();
				if (flag)
				{
					await base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
					{
						App.<>c__DisplayClass5_0.<<OnStartup>b__0>d <<OnStartup>b__0>d;
						<<OnStartup>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
						<<OnStartup>b__0>d.<>4__this = CS$<>8__locals1;
						<<OnStartup>b__0>d.<>1__state = -1;
						<<OnStartup>b__0>d.<>t__builder.Start<App.<>c__DisplayClass5_0.<<OnStartup>b__0>d>(ref <<OnStartup>b__0>d);
					}));
				}
				if (this.appLifeWin != null)
				{
					base.Dispatcher.Invoke(delegate
					{
						CS$<>8__locals1.<>4__this.appLifeWin.Close();
					});
				}
				base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
				{
					App.<>c__DisplayClass5_0.<<OnStartup>b__2>d <<OnStartup>b__2>d;
					<<OnStartup>b__2>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<OnStartup>b__2>d.<>4__this = CS$<>8__locals1;
					<<OnStartup>b__2>d.<>1__state = -1;
					<<OnStartup>b__2>d.<>t__builder.Start<App.<>c__DisplayClass5_0.<<OnStartup>b__2>d>(ref <<OnStartup>b__2>d);
				}));
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002D33 File Offset: 0x00000F33
		private void UserStore_UserInfoUpdated(object sender, EventArgs e)
		{
			ProcessMessageHelper.SendNamedMessage("UserInfoUpdated", IntPtr.Zero, IntPtr.Zero);
			this.UpdateUserInfo();
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002D50 File Offset: 0x00000F50
		private void UpdateUserInfo()
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			if (requiredService != null)
			{
				requiredService.UserInfoModel = UserStore.GetUserInfoModel();
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002D76 File Offset: 0x00000F76
		public static bool AppLaunchEvents(string sourceFrom = "Launch")
		{
			GAManager.SendEvent("App", "Launch", "count", 1L);
			ConfigManager.GetOriginVersion();
			ConfigManager.GetInstallDate();
			IAPUtils.GetIAPProductsAsync();
			return true;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002DA0 File Offset: 0x00000FA0
		public static void OpenUpdateAdConfigWindow()
		{
			new UpdateAdConfigWindow().Show();
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002DDC File Offset: 0x00000FDC
		[STAThread]
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public static void Main()
		{
			App app = new App();
			app.InitializeComponent();
			app.Run();
		}

		// Token: 0x04000019 RID: 25
		public AppLifeWindow appLifeWin;

		// Token: 0x0400001A RID: 26
		private SystemThemeListener systemThemeListener;
	}
}
