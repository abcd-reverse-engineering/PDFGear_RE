using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommonLib.Account;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommonLib.Config;
using CommonLib.Properties;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xaml.Behaviors.Layout;
using Patagames.Pdf.Net;
using pdfeditor.AutoSaveRestore;
using pdfeditor.Properties;
using pdfeditor.Services;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using pdfeditor.Views;
using PDFKit;

namespace pdfeditor
{
	// Token: 0x02000047 RID: 71
	public partial class App : Application
	{
		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000331 RID: 817 RVA: 0x0000F4A7 File Offset: 0x0000D6A7
		public new static App Current
		{
			get
			{
				return (App)Application.Current;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000332 RID: 818 RVA: 0x0000F4B3 File Offset: 0x0000D6B3
		// (set) Token: 0x06000333 RID: 819 RVA: 0x0000F4BB File Offset: 0x0000D6BB
		public AppHotKeyManager AppHotKeyManager { get; private set; }

		// Token: 0x06000334 RID: 820 RVA: 0x0000F4C4 File Offset: 0x0000D6C4
		public App()
		{
			AppIdHelper.RegisterAppUserModelId();
			SqliteUtils.InitializeDatabase("pdfdata.db");
			Common.SetAppDataFolder(UtilManager.GetAppDataPath());
			Common.Initialize(CultureInfoUtils.ActualAppLanguage, new Func<string>(UtilManager.GetProductName), new Func<string>(UtilManager.GetAppVersion), new Action<string, string, string, long>(GAManager.SendEvent), new Action<string>(Log.WriteLog));
			CultureInfoUtils.Initialize();
			pdfeditor.Properties.Resources.Culture = CultureInfo.CurrentUICulture;
			CommonLib.Properties.Resources.Culture = CultureInfo.CurrentUICulture;
			this.systemThemeListener = new SystemThemeListener(base.Dispatcher);
			this.systemThemeListener.ActualAppThemeChanged += delegate(object s, EventArgs a)
			{
				ThemeResourceDictionary.GetForCurrentApp().Theme = this.GetCurrentActualAppTheme();
			};
			WindowThemeHelper.Initialize();
			ProcessMessageHelper.RegisterMessageName("PDFgearAutoSave");
			ProcessMessageHelper.MessageReceived += this.ProcessMessageHelper_MessageReceived;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0000F590 File Offset: 0x0000D790
		private void ProcessMessageHelper_MessageReceived(object sender, ProcessMessageReceivedEventArgs e)
		{
			Log.WriteLog(e.Message);
			if (e.Message == "UpdateTheme")
			{
				base.Dispatcher.InvokeAsync(delegate
				{
					ThemeResourceDictionary.GetForCurrentApp().Theme = this.GetCurrentActualAppTheme();
				});
				return;
			}
			if (e.Message == "PDFgearAutoSave")
			{
				base.Dispatcher.InvokeAsync(delegate
				{
					pdfeditor.AutoSaveRestore.AutoSaveManager.GetInstance().TrySaveImmediately();
				}, DispatcherPriority.Send);
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

		// Token: 0x06000336 RID: 822 RVA: 0x0000F640 File Offset: 0x0000D840
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

		// Token: 0x06000337 RID: 823 RVA: 0x0000F6BC File Offset: 0x0000D8BC
		protected override async void OnStartup(StartupEventArgs e)
		{
			LaunchUtils.Initialize(e);
			DispatcherHelper.Initialize();
			ThemeResourceDictionary forCurrentApp = ThemeResourceDictionary.GetForCurrentApp();
			forCurrentApp.Theme = this.GetCurrentActualAppTheme();
			forCurrentApp.ActualThemeChanged += delegate(object s, ActualThemeChangedEventArgs a)
			{
				MainView mainView = base.MainWindow as MainView;
				if (mainView != null)
				{
					mainView.UpdateViewerThemeValues();
					this.ThemeChangedFadeOut();
				}
			};
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
			Application.Current.Dispatcher.UnhandledException += this.Dispatcher_UnhandledException;
			TaskExceptionHelper.UnhandledException += this.TaskExceptionHelper_UnhandledException;
			base.OnStartup(e);
			ConfigManager.GetOriginVersion();
			ConfigManager.GetInstallDate();
			ConfigManager.setAppLaunchCount(ConfigManager.getAppLaunchCount() + 1L);
			IAPUtils.GetIAPProductsAsync();
			if (!string.IsNullOrWhiteSpace(SDKUtils.GetLibPath()))
			{
				PdfCommon.Initialize(SDKUtils.GetLinceseKey(), SDKUtils.GetLibPath(), null);
			}
			LicenceManage.SycfusionRegisterLicence();
			Ioc.Default.ConfigureServices(App.ConfigureServices(e));
			RenderUtils.Init();
			this.AppHotKeyManager = new AppHotKeyManager();
			await base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async delegate
			{
				FileWatcherHelper.Instance.UpdateState(700);
				TaskAwaiter<bool> taskAwaiter = UpdateHelper.EndOfServicing().GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<bool> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<bool>);
				}
				if (taskAwaiter.GetResult())
				{
					base.Shutdown();
				}
				else
				{
					SetDefaultAppUtils.TrySetDefaultApp();
					if (UpdateHelper.ShouldShowUpdateDialog())
					{
						await UpdateHelper.UpdateAndExit(false);
					}
				}
			}));
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0000F6FB File Offset: 0x0000D8FB
		private void UserStore_UserInfoUpdated(object sender, EventArgs e)
		{
			ProcessMessageHelper.SendNamedMessage("UserInfoUpdated", IntPtr.Zero, IntPtr.Zero);
			this.UpdateUserInfo();
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0000F718 File Offset: 0x0000D918
		private void UpdateUserInfo()
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			if (requiredService != null)
			{
				requiredService.UserInfoModel = UserStore.GetUserInfoModel();
			}
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0000F740 File Offset: 0x0000D940
		private void ThemeChangedFadeOut()
		{
			MainView mainView = base.MainWindow as MainView;
			if (mainView != null)
			{
				FrameworkElement frameworkElement = mainView.Content as FrameworkElement;
				if (frameworkElement != null)
				{
					AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(frameworkElement);
					if (adornerLayer != null)
					{
						DpiScale dpi = VisualTreeHelper.GetDpi(mainView);
						RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)(frameworkElement.ActualWidth * dpi.PixelsPerDip), (int)(frameworkElement.ActualHeight * dpi.PixelsPerDip), dpi.PixelsPerInchY, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
						renderTargetBitmap.Render(frameworkElement);
						Image image = new Image
						{
							Width = frameworkElement.ActualWidth,
							Height = frameworkElement.ActualHeight,
							Source = renderTargetBitmap,
							Stretch = Stretch.None
						};
						AdornerContainer adornerContainer = new AdornerContainer(frameworkElement)
						{
							Width = frameworkElement.ActualWidth,
							Height = frameworkElement.ActualHeight,
							Child = image
						};
						adornerLayer.Add(adornerContainer);
						DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, TimeSpan.FromSeconds(0.15));
						doubleAnimation.Completed += delegate(object s1, EventArgs a1)
						{
							adornerLayer.Remove(adornerContainer);
						};
						image.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
					}
				}
			}
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0000F884 File Offset: 0x0000DA84
		private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			if (e.Exception != this.lastException)
			{
				this.lastException = e.Exception;
				this.LogException(e.Exception);
			}
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0000F8AC File Offset: 0x0000DAAC
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject != this.lastException)
			{
				this.lastException = e.ExceptionObject;
				this.LogException(e.ExceptionObject);
			}
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0000F8D4 File Offset: 0x0000DAD4
		private void TaskExceptionHelper_UnhandledException(TaskUnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject != this.lastException && !(e.ExceptionObject is OperationCanceledException))
			{
				this.lastException = e.ExceptionObject;
				this.LogException(e.ExceptionObject);
			}
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0000F90C File Offset: 0x0000DB0C
		private void LogException(object exceptionObj)
		{
			string text = "";
			Exception ex = exceptionObj as Exception;
			if (ex != null)
			{
				text = ex.CreateUnhandledExceptionMessage();
				GAManager.SendEvent("Exception", "UnhandledException", ex.GetType().Name + ", " + ex.Message, 1L);
			}
			else if (exceptionObj != null)
			{
				text = exceptionObj.ToString();
			}
			Log.WriteLog(text);
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0000F970 File Offset: 0x0000DB70
		private static IServiceProvider ConfigureServices(StartupEventArgs e)
		{
			ServiceCollection serviceCollection = new ServiceCollection();
			string filePath = string.Empty;
			if (e.Args != null && e.Args.Length != 0 && e.Args[0] != "CreateNewFile")
			{
				filePath = e.Args[0];
				string text;
				string text2;
				if (AppManager.TryUnwrapLongArguments(ref filePath, out text, out text2))
				{
					filePath = text;
				}
			}
			serviceCollection.AddSingleton<PdfThumbnailService>();
			serviceCollection.AddSingleton((IServiceProvider _) => new MainViewModel(filePath));
			serviceCollection.AddSingleton((IServiceProvider _) => new AppSettingsViewModel());
			return serviceCollection.BuildServiceProvider();
		}

		// Token: 0x04000148 RID: 328
		private SystemThemeListener systemThemeListener;

		// Token: 0x04000149 RID: 329
		private object lastException;
	}
}
