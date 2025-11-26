using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommonLib.Common;
using CommonLib.Config;
using RegExt.FileAssociations;

namespace RegExt
{
	// Token: 0x02000002 RID: 2
	internal class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		private static void Main(string[] args)
		{
			bool flag = !args.Contains("-uninstall", StringComparer.OrdinalIgnoreCase);
			bool flag2 = args.Contains("-setAsDefault", StringComparer.OrdinalIgnoreCase);
			bool flag3 = args.Contains("-A54C4E30729A469DA8F0864FDB400881", StringComparer.OrdinalIgnoreCase);
			bool flag4 = args.Contains("-stopwatcher", StringComparer.OrdinalIgnoreCase);
			bool flag5 = args.Contains("-resetDefault", StringComparer.OrdinalIgnoreCase);
			bool flag6 = args.Contains("-pinTaskBand", StringComparer.OrdinalIgnoreCase);
			bool flag7 = args.Contains("-installWebView2", StringComparer.OrdinalIgnoreCase);
			SqliteUtils.InitializeDatabase("pdfdata.db");
			if (flag4)
			{
				return;
			}
			if (flag)
			{
				if (flag3)
				{
					if (flag6)
					{
						Program.PinAppToTaskbar(true);
					}
					try
					{
						DateTime creationTimeUtc = File.GetCreationTimeUtc(Path.Combine(AppDataHelper.LocalFolder, "pdfdata.db"));
						if ((DateTime.UtcNow - creationTimeUtc).TotalMinutes < 1.0)
						{
							ConfigUtils.TrySet<Guid>("ID_A54C4E30729A469DA8F0864FDB400881", new Guid(3146596894U, 32475, 18483, 180, 111, 226, 20, 55, 169, 187, 135));
						}
					}
					catch
					{
					}
					foreach (string text in args)
					{
						if (text.StartsWith("-name:"))
						{
							ChannelHelper.SaveSetupName(text);
						}
					}
				}
				Program.CreateProgId();
				Program.CreateFileExts(flag2, flag5, flag3);
				DefaultAppHelper.Refresh();
				Version version;
				if (flag7 && !WebView2Helper.IsWebView2Installed(out version))
				{
					WebView2Helper.StartInstall().Wait();
					return;
				}
			}
			else
			{
				if (flag6)
				{
					Program.PinAppToTaskbar(false);
				}
				FileWatcherHelper.Instance.IsEnabled = false;
				FileWatcherHelper.Instance.UpdateState(0);
				Program.RemoveProgId();
				SqliteUtils.InitializeDatabase("pdfdata.db");
				ConfigManager.SetDefaultAppAction(null);
				Program.PinAppToTaskbar(false);
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002214 File Offset: 0x00000414
		private static void PinAppToTaskbar(bool pinning)
		{
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PDFLauncher.exe");
			ShellHelper.UpdateFilePinState(pinning, text, "PDFgear", "578ab678-3bcf-4410-8b82-675d5d214865");
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002248 File Offset: 0x00000448
		private static void CreateProgId()
		{
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PDFLauncher.exe");
			RegisterProgram registerProgram = new RegisterProgram("PdfGear.App.1")
			{
				Operation = "open",
				Command = "\"" + text + "\" \"%1\"",
				FriendlyAppName = "PDFgear",
				AppUserModelID = "578ab678-3bcf-4410-8b82-675d5d214865",
				DefaultIcon = "\"" + text + "\",1"
			};
			if (AppIdHelper.IsAdmin)
			{
				try
				{
					registerProgram.WriteToAllUser();
				}
				catch
				{
				}
			}
			try
			{
				registerProgram.WriteToCurrentUser();
			}
			catch
			{
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000022FC File Offset: 0x000004FC
		private static void RemoveProgId()
		{
			RegisterProgram registerProgram = new RegisterProgram("PdfGear.App.1");
			if (AppIdHelper.IsAdmin)
			{
				try
				{
					registerProgram.RemoveFromAllUser();
				}
				catch
				{
				}
			}
			try
			{
				registerProgram.RemoveFromCurrentUser();
			}
			catch
			{
			}
			DefaultAppHelper.Refresh();
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002354 File Offset: 0x00000554
		private static void CreateFileExts(bool setAsDefault, bool resetDefault, bool init)
		{
			string[] defaultFileExts = AppManager.GetDefaultFileExts();
			if (defaultFileExts != null && defaultFileExts.Length != 0)
			{
				RegisterFileExtension[] array = defaultFileExts.Select((string c) => new RegisterFileExtension(c)
				{
					DefaultProgramId = "PdfGear.App.1",
					OpenWithProgramIds = { "PdfGear.App.1" }
				}).ToArray<RegisterFileExtension>();
				if (AppIdHelper.IsAdmin)
				{
					foreach (RegisterFileExtension registerFileExtension in array)
					{
						try
						{
							registerFileExtension.WriteToAllUser();
							if (setAsDefault)
							{
								string text;
								if (resetDefault)
								{
									DefaultAppHelper.ResetDefaultApp(registerFileExtension.FileExtension);
								}
								else if (!SqliteUtils.TryGet("OriginVersion", out text))
								{
									DefaultAppHelper.SetDefaultApp("PdfGear.App.1", registerFileExtension.FileExtension);
								}
								else if (DefaultAppHelper.GetDefaultAppProgId(registerFileExtension.FileExtension) != "PdfGear.App.1")
								{
									if (AppIdHelper.HasUserChoiceLatest)
									{
										AppIdHelper.RemoveOpenWithListAppFlag(registerFileExtension.FileExtension);
										if (!init && array.Length == 1)
										{
											Program.ShowOpenWithPicker(registerFileExtension.FileExtension);
										}
									}
									else
									{
										DefaultAppHelper.SetDefaultApp("PdfGear.App.1", registerFileExtension.FileExtension);
									}
								}
							}
						}
						catch
						{
						}
					}
				}
				foreach (RegisterFileExtension registerFileExtension2 in array)
				{
					try
					{
						registerFileExtension2.WriteToCurrentUser();
						if (setAsDefault)
						{
							string text;
							if (resetDefault)
							{
								DefaultAppHelper.ResetDefaultApp(registerFileExtension2.FileExtension);
							}
							else if (!SqliteUtils.TryGet("OriginVersion", out text))
							{
								DefaultAppHelper.SetDefaultApp("PdfGear.App.1", registerFileExtension2.FileExtension);
							}
							else if (DefaultAppHelper.GetDefaultAppProgId(registerFileExtension2.FileExtension) != "PdfGear.App.1")
							{
								if (AppIdHelper.HasUserChoiceLatest)
								{
									AppIdHelper.RemoveOpenWithListAppFlag(registerFileExtension2.FileExtension);
									if (!init && array.Length == 1)
									{
										Program.ShowOpenWithPicker(registerFileExtension2.FileExtension);
									}
								}
								else
								{
									DefaultAppHelper.SetDefaultApp("PdfGear.App.1", registerFileExtension2.FileExtension);
								}
							}
						}
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000252C File Offset: 0x0000072C
		private static void ShowOpenWithPicker(string ext)
		{
			if (!ext.StartsWith("."))
			{
				ext = "." + ext;
			}
			string text = Path.Combine(AppContext.BaseDirectory, "OpenWithPicker.exe");
			if (File.Exists(text))
			{
				try
				{
					Process.Start(text, ext);
				}
				catch
				{
				}
			}
		}
	}
}
