using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommonLib.Config;
using CommonLib.Properties;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Patagames.Pdf.Net;
using pdfconverter.Properties;
using pdfconverter.ViewModels;
using pdfconverter.Views;
using Syncfusion.Licensing;

namespace pdfconverter
{
	// Token: 0x02000021 RID: 33
	public partial class App : Application
	{
		// Token: 0x060000EA RID: 234 RVA: 0x00004458 File Offset: 0x00002658
		public App()
		{
			AppIdHelper.RegisterAppUserModelId();
			SqliteUtils.InitializeDatabase("pdfdata.db");
			CultureInfoUtils.Initialize();
			pdfconverter.Properties.Resources.Culture = CultureInfo.CurrentUICulture;
			CommonLib.Properties.Resources.Culture = CultureInfo.CurrentUICulture;
			this.systemThemeListener = new SystemThemeListener(base.Dispatcher);
			this.systemThemeListener.ActualAppThemeChanged += delegate(object s, EventArgs a)
			{
				ThemeResourceDictionary.GetForCurrentApp().Theme = this.GetCurrentActualAppTheme();
			};
			WindowThemeHelper.Initialize();
			ProcessMessageHelper.MessageReceived += this.ProcessMessageHelper_MessageReceived;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000044D2 File Offset: 0x000026D2
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

		// Token: 0x060000EC RID: 236 RVA: 0x00004500 File Offset: 0x00002700
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

		// Token: 0x060000ED RID: 237 RVA: 0x0000457C File Offset: 0x0000277C
		protected override async void OnStartup(StartupEventArgs e)
		{
			DispatcherHelper.Initialize();
			base.OnStartup(e);
			ThemeResourceDictionary.GetForCurrentApp().Theme = this.GetCurrentActualAppTheme();
			if (e.Args.Length > 2)
			{
				string text = e.Args[0];
				if (text.Equals("app1", StringComparison.OrdinalIgnoreCase))
				{
					string text2 = e.Args[1];
					try
					{
						App.convertType = Enum.Parse(typeof(ConvFromPDFType), text2);
						goto IL_0125;
					}
					catch
					{
						MessageBox.Show("Failed to launch the application.", UtilManager.GetProductName());
						Application.Current.Shutdown();
						return;
					}
				}
				if (text.Equals("app2", StringComparison.OrdinalIgnoreCase))
				{
					string text2 = e.Args[1];
					try
					{
						App.convertType = Enum.Parse(typeof(ConvToPDFType), text2);
						goto IL_0125;
					}
					catch
					{
						MessageBox.Show("Failed to launch the application.", UtilManager.GetProductName());
						Application.Current.Shutdown();
						return;
					}
				}
				MessageBox.Show("Failed to launch the application.", UtilManager.GetProductName());
				Application.Current.Shutdown();
				return;
				IL_0125:
				if (e.Args.Length > 2 && File.Exists(e.Args[2]))
				{
					FilesArgsModel filesArgsModel = DocsPathUtils.ReadFilesPathJson(e.Args[2]);
					if (filesArgsModel != null)
					{
						App.selectedFile = new string[filesArgsModel.FilesPath.Count];
						App.seletedPassword = new string[filesArgsModel.FilesPath.Count];
						for (int i = 0; i < filesArgsModel.FilesPath.Count; i++)
						{
							App.selectedFile[i] = filesArgsModel.FilesPath[i].FilePath;
							if (filesArgsModel.FilesPath[i].Password != null)
							{
								App.seletedPassword[i] = filesArgsModel.FilesPath[i].Password;
							}
							else
							{
								App.seletedPassword[i] = "";
							}
						}
					}
				}
			}
			if (!string.IsNullOrWhiteSpace(SDKUtils.GetLibPath()))
			{
				PdfCommon.Initialize(SDKUtils.GetLinceseKey(), SDKUtils.GetLibPath(), null);
				GAManager.SendEvent("App", "LaunchConverter", "Count", 1L);
			}
			else
			{
				GAManager.SendEvent("App", "LaunchConverter", "Error", 1L);
			}
			SyncfusionLicenseProvider.RegisterLicense("NDEyOTg1OEAzMTM5MmUzMzJlMzBCeVlkQlM0Vmw5U2Ivbm0vSW9OWFA1am1BSGtzYk9vT2liOXB0Y1lzNklrPQ==");
			SqliteUtils.InitializeDatabase("pdfdata.db");
			if (App.convertType is ConvFromPDFType)
			{
				object obj = App.convertType;
				if (!(obj is ConvFromPDFType) || (ConvFromPDFType)obj != ConvFromPDFType.PDFToJpg)
				{
					obj = App.convertType;
					if (!(obj is ConvFromPDFType) || (ConvFromPDFType)obj != ConvFromPDFType.PDFToPng)
					{
						new MainWindow().Show();
						goto IL_03B7;
					}
				}
				new PDFToImageWindow().Show();
			}
			else if (App.convertType is ConvToPDFType)
			{
				ServiceCollection serviceCollection = new ServiceCollection();
				serviceCollection.AddSingleton(new MainWindow2ViewModel());
				serviceCollection.AddSingleton(new SplitPDFUCViewModel());
				serviceCollection.AddSingleton(new MergePDFUCViewModel());
				serviceCollection.AddSingleton(new WordToPDFUCViewModel());
				serviceCollection.AddSingleton(new ExcelToPDFUCViewModel());
				serviceCollection.AddSingleton(new ImageToPDFUCViewModel());
				serviceCollection.AddSingleton(new ImageToPDFViewModel());
				serviceCollection.AddSingleton(new RTFToPDFUCViewModel());
				serviceCollection.AddSingleton(new TXTToPDFUCViewModel());
				serviceCollection.AddSingleton(new PPTToPDFUCViewModel());
				serviceCollection.AddSingleton(new CompressPDFUCViewModel());
				ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
				Ioc.Default.ConfigureServices(serviceProvider);
				if ((ConvToPDFType)App.convertType == ConvToPDFType.ImageToPDF)
				{
					new ImageToPDF().ShowDialog();
				}
				else
				{
					new MainWindow2().Show();
					if (App.selectedFile != null && App.selectedFile.Length != 0)
					{
						this.FileListInMainWindows2();
					}
				}
			}
			else
			{
				Application.Current.Shutdown();
			}
			IL_03B7:
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
				Application.Current.Shutdown();
			}
			else if (UpdateHelper.ShouldShowUpdateDialog())
			{
				await UpdateHelper.UpdateAndExit(false);
			}
		}

		// Token: 0x060000EE RID: 238 RVA: 0x000045BB File Offset: 0x000027BB
		private void Application_Startup(object sender, StartupEventArgs e)
		{
		}

		// Token: 0x060000EF RID: 239 RVA: 0x000045C0 File Offset: 0x000027C0
		private void FileListInMainWindows2()
		{
			ConvToPDFType convToPDFType = (ConvToPDFType)App.convertType;
			if (convToPDFType == ConvToPDFType.MergePDF)
			{
				for (int i = 0; i <= App.selectedFile.Length - 1; i++)
				{
					Ioc.Default.GetRequiredService<MergePDFUCViewModel>().AddOneFileToMergeList(App.selectedFile[i], App.seletedPassword[i]);
				}
				return;
			}
			if (convToPDFType == ConvToPDFType.SplitPDF)
			{
				for (int j = 0; j <= App.selectedFile.Length - 1; j++)
				{
					Ioc.Default.GetRequiredService<SplitPDFUCViewModel>().AddOneFileToSplitList(App.selectedFile[j], App.seletedPassword[j]);
				}
				return;
			}
			if (convToPDFType == ConvToPDFType.CompressPDF)
			{
				for (int k = 0; k <= App.selectedFile.Length - 1; k++)
				{
					Ioc.Default.GetRequiredService<CompressPDFUCViewModel>().AddOneFileToFileList(App.selectedFile[k], App.seletedPassword[k]);
				}
			}
		}

		// Token: 0x040000FE RID: 254
		public static object convertType = ConvToPDFType.SplitPDF;

		// Token: 0x040000FF RID: 255
		public static string[] selectedFile;

		// Token: 0x04000100 RID: 256
		public static string[] seletedPassword;

		// Token: 0x04000101 RID: 257
		private SystemThemeListener systemThemeListener;
	}
}
