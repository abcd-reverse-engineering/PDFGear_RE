using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using CommonLib.Config.ConfigModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using pdfeditor.Controls.PageEditor;
using pdfeditor.Controls.Printer;
using pdfeditor.Models;
using pdfeditor.Properties;
using pdfeditor.Utils.Enums;
using PDFKit;

namespace pdfeditor.ViewModels
{
	// Token: 0x0200005F RID: 95
	public class AppSettingsViewModel : ObservableObject
	{
		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000516 RID: 1302 RVA: 0x00019C72 File Offset: 0x00017E72
		public FileWatcherHelper RecentFilesHelper
		{
			get
			{
				return FileWatcherHelper.Instance;
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x00019C79 File Offset: 0x00017E79
		public RenderUtils RenderUtils
		{
			get
			{
				return RenderUtils.Instance;
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000518 RID: 1304 RVA: 0x00019C80 File Offset: 0x00017E80
		// (set) Token: 0x06000519 RID: 1305 RVA: 0x00019C88 File Offset: 0x00017E88
		public bool LanguageChangedFlag { get; private set; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x0600051A RID: 1306 RVA: 0x00019C91 File Offset: 0x00017E91
		// (set) Token: 0x0600051B RID: 1307 RVA: 0x00019C99 File Offset: 0x00017E99
		public int BlankPageWidth
		{
			get
			{
				return this._blankPageWidth;
			}
			set
			{
				base.SetProperty<int>(ref this._blankPageWidth, value, "BlankPageWidth");
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x0600051C RID: 1308 RVA: 0x00019CAE File Offset: 0x00017EAE
		// (set) Token: 0x0600051D RID: 1309 RVA: 0x00019CB6 File Offset: 0x00017EB6
		public int BlankPageHeight
		{
			get
			{
				return this._blankPageHeight;
			}
			set
			{
				base.SetProperty<int>(ref this._blankPageHeight, value, "BlankPageHeight");
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x0600051E RID: 1310 RVA: 0x00019CCB File Offset: 0x00017ECB
		public bool IsDefaultAppForPdf
		{
			get
			{
				return AppManager.GetDefaultFileExts().All((string c) => AppIdHelper.GetDefaultAppProgId(c) == "PdfGear.App.1");
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x0600051F RID: 1311 RVA: 0x00019CF6 File Offset: 0x00017EF6
		// (set) Token: 0x06000520 RID: 1312 RVA: 0x00019CFE File Offset: 0x00017EFE
		public bool ChatButtonSettings
		{
			get
			{
				return this._chatButtonSettings;
			}
			set
			{
				base.SetProperty<bool>(ref this._chatButtonSettings, value, "ChatButtonSettings");
			}
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000521 RID: 1313 RVA: 0x00019D13 File Offset: 0x00017F13
		// (set) Token: 0x06000522 RID: 1314 RVA: 0x00019D1B File Offset: 0x00017F1B
		public bool ShowToolbar
		{
			get
			{
				return this._showToolbar;
			}
			set
			{
				base.SetProperty<bool>(ref this._showToolbar, value, "ShowToolbar");
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000523 RID: 1315 RVA: 0x00019D30 File Offset: 0x00017F30
		// (set) Token: 0x06000524 RID: 1316 RVA: 0x00019D38 File Offset: 0x00017F38
		public bool ShowStatusbar
		{
			get
			{
				return this._showStatusbar;
			}
			set
			{
				base.SetProperty<bool>(ref this._showStatusbar, value, "ShowStatusbar");
			}
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000525 RID: 1317 RVA: 0x00019D4D File Offset: 0x00017F4D
		// (set) Token: 0x06000526 RID: 1318 RVA: 0x00019D55 File Offset: 0x00017F55
		public bool ShowOCR
		{
			get
			{
				return this._showOcr;
			}
			set
			{
				base.SetProperty<bool>(ref this._showOcr, value, "ShowOCR");
			}
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000527 RID: 1319 RVA: 0x00019D6A File Offset: 0x00017F6A
		// (set) Token: 0x06000528 RID: 1320 RVA: 0x00019D72 File Offset: 0x00017F72
		public bool ShowRightMenu
		{
			get
			{
				return this._showRightMenu;
			}
			set
			{
				base.SetProperty<bool>(ref this._showRightMenu, value, "ShowRightMenu");
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000529 RID: 1321 RVA: 0x00019D87 File Offset: 0x00017F87
		// (set) Token: 0x0600052A RID: 1322 RVA: 0x00019D8F File Offset: 0x00017F8F
		public bool IsFillFormHighlightedSettings
		{
			get
			{
				return this._isFillFormHighlightedSettings;
			}
			set
			{
				base.SetProperty<bool>(ref this._isFillFormHighlightedSettings, value, "IsFillFormHighlightedSettings");
			}
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x00019DA4 File Offset: 0x00017FA4
		public Task RefreshSettingsAsync()
		{
			string appLang = ConfigManager.GetApplicationLanugageName();
			this.selectedLanguage = this.Languages.FirstOrDefault((LanguageModel c) => c.Name == appLang) ?? LanguageModel.Fallback;
			base.OnPropertyChanged("SelectedLanguage");
			LanguageModel languageModel = this.selectedLanguage;
			this.initLanguageName = ((languageModel != null) ? languageModel.Name : null);
			FileWatcherHelper.Instance.Refresh();
			this.ChatButtonSettings = ConfigManager.GetShowcaseChatButtonFlag();
			this.IsFillFormHighlightedSettings = ConfigManager.GetIsFillFormHighlightedFlag();
			this.BlankPageHeight = ConfigManager.GetDefaultBlankPageHeight();
			this.BlankPageWidth = ConfigManager.GetDefaultBlankPageWidth();
			this.ShowToolbar = ConfigManager.GetLaunchAPPShowFlag("LaunchToolbar");
			this.ShowStatusbar = ConfigManager.GetLaunchAPPShowFlag("LaunchStatusbar");
			this.ShowOCR = ConfigManager.GetLaunchAPPShowFlag("LaunchOCR");
			this.ShowRightMenu = ConfigManager.GetSelectTextShowMenuFlag();
			base.OnPropertyChanged("IsDefaultAppForPdf");
			return Task.CompletedTask;
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x0600052C RID: 1324 RVA: 0x00019E8C File Offset: 0x0001808C
		public string[] Orinentations
		{
			get
			{
				return new List<string>
				{
					Resources.WinPageInsertSettingPagePortrait,
					Resources.WinPageInsertSettingPageLandscape
				}.ToArray();
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x0600052D RID: 1325 RVA: 0x00019EB0 File Offset: 0x000180B0
		public string[] DicItem
		{
			get
			{
				List<string> list = new List<string>();
				List<PaperSizeInfo> list2 = Pagesize.paperSizes.Select((PaperSize c) => new PaperSizeInfo
				{
					FriendlyName = c.PaperName,
					PaperSize = c
				}).ToList<PaperSizeInfo>();
				list.Add(list2[list2.FindIndex((PaperSizeInfo x) => x.FriendlyName == "A4")].ToString());
				list.Add(list2[list2.FindIndex((PaperSizeInfo x) => x.FriendlyName == "A3")].ToString());
				list.Add(list2[list2.FindIndex((PaperSizeInfo x) => x.FriendlyName == "Letter")].ToString());
				list.Add(list2[list2.FindIndex((PaperSizeInfo x) => x.FriendlyName == "Tabloid")].ToString());
				list.Add(list2[list2.FindIndex((PaperSizeInfo x) => x.FriendlyName == "Legal")].ToString());
				list.Add(Resources.WinPageInsertSettingPageSizeItemCustomize);
				return list.ToArray();
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x0600052E RID: 1326 RVA: 0x0001A010 File Offset: 0x00018210
		public ObservableCollection<LanguageModel> Languages
		{
			get
			{
				if (this.languages == null)
				{
					this.languages = new ObservableCollection<LanguageModel>();
					this.languages.Add(LanguageModel.Fallback);
					foreach (LanguageModel languageModel in LanguageModel.AllLanguageModel)
					{
						this.languages.Add(languageModel);
					}
				}
				return this.languages;
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x0600052F RID: 1327 RVA: 0x0001A08C File Offset: 0x0001828C
		public string[] SelectionModes
		{
			get
			{
				return new List<string>
				{
					Resources.AppSettingsSelectionMode,
					Resources.AppSettingsHandMode
				}.ToArray();
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000530 RID: 1328 RVA: 0x0001A0AE File Offset: 0x000182AE
		public string[] PageViewModes
		{
			get
			{
				return new List<string>
				{
					Resources.AppSettingsSingleContinuous,
					Resources.AppSettingsSingle,
					Resources.AppSettingsDoubleContinuous,
					Resources.AppSettingsDouble
				}.ToArray();
			}
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000531 RID: 1329 RVA: 0x0001A0E8 File Offset: 0x000182E8
		public string[] SizeModes
		{
			get
			{
				return new List<string>
				{
					Resources.MenuViewFullSizeContent,
					Resources.MenuViewFitPageContent,
					Resources.MenuViewHeightContent,
					Resources.MenuViewWidthContent,
					"10%",
					"25%",
					"50%",
					"75%",
					"100%",
					"125%",
					"150%",
					"200%",
					"400%",
					"600%"
				}.ToArray();
			}
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000532 RID: 1330 RVA: 0x0001A19C File Offset: 0x0001839C
		// (set) Token: 0x06000533 RID: 1331 RVA: 0x0001A1DC File Offset: 0x000183DC
		public string Orientation
		{
			get
			{
				string defaultBlankPageOrinentation = ConfigManager.GetDefaultBlankPageOrinentation();
				if (defaultBlankPageOrinentation == "Portrait")
				{
					return Resources.WinPageInsertSettingPagePortrait;
				}
				if (!(defaultBlankPageOrinentation == "Landscape"))
				{
					return Resources.WinPageInsertSettingPagePortrait;
				}
				return Resources.WinPageInsertSettingPageLandscape;
			}
			set
			{
				if (value == Resources.WinPageInsertSettingPagePortrait)
				{
					this._orientation = "Portrait";
					return;
				}
				this._orientation = "Landscape";
			}
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000534 RID: 1332 RVA: 0x0001A204 File Offset: 0x00018404
		// (set) Token: 0x06000535 RID: 1333 RVA: 0x0001A244 File Offset: 0x00018444
		public string SelectionMode
		{
			get
			{
				string defaultSelectionMode = ConfigManager.GetDefaultSelectionMode();
				if (defaultSelectionMode == "Selection")
				{
					return Resources.AppSettingsSelectionMode;
				}
				if (!(defaultSelectionMode == "Hand"))
				{
					return Resources.AppSettingsSelectionMode;
				}
				return Resources.AppSettingsHandMode;
			}
			set
			{
				if (value == Resources.AppSettingsHandMode)
				{
					this._selectionMode = "Hand";
					return;
				}
				this._selectionMode = "Selection";
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000536 RID: 1334 RVA: 0x0001A26C File Offset: 0x0001846C
		// (set) Token: 0x06000537 RID: 1335 RVA: 0x0001A438 File Offset: 0x00018638
		public string BlankPageSize
		{
			get
			{
				string defaultBlankPageSize = ConfigManager.GetDefaultBlankPageSize();
				List<PaperSizeInfo> list = Pagesize.paperSizes.Select((PaperSize c) => new PaperSizeInfo
				{
					FriendlyName = c.PaperName,
					PaperSize = c
				}).ToList<PaperSizeInfo>();
				if (defaultBlankPageSize == "A4")
				{
					return list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "A4")].ToString();
				}
				if (defaultBlankPageSize == "A3")
				{
					return list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "A3")].ToString();
				}
				if (defaultBlankPageSize == "Letter")
				{
					return list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "Letter")].ToString();
				}
				if (defaultBlankPageSize == "Tabloid")
				{
					return list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "Tabloid")].ToString();
				}
				if (defaultBlankPageSize == "Legal")
				{
					return list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "Legal")].ToString();
				}
				if (!(defaultBlankPageSize == "Custom"))
				{
					return list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "A4")].ToString();
				}
				return Resources.WinPageInsertSettingPageSizeItemCustomize;
			}
			set
			{
				List<PaperSizeInfo> list = Pagesize.paperSizes.Select((PaperSize c) => new PaperSizeInfo
				{
					FriendlyName = c.PaperName,
					PaperSize = c
				}).ToList<PaperSizeInfo>();
				if (value == list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "A4")].ToString())
				{
					this._blankPageSize = "A4";
					return;
				}
				if (value == list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "A3")].ToString())
				{
					this._blankPageSize = "A3";
					return;
				}
				if (value == list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "Letter")].ToString())
				{
					this._blankPageSize = "Letter";
					return;
				}
				if (value == list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "Tabloid")].ToString())
				{
					this._blankPageSize = "Tabloid";
					return;
				}
				if (value == list[list.FindIndex((PaperSizeInfo x) => x.FriendlyName == "Legal")].ToString())
				{
					this._blankPageSize = "Legal";
					return;
				}
				if (value == Resources.WinPageInsertSettingPageSizeItemCustomize)
				{
					this._blankPageSize = "Custom";
				}
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000538 RID: 1336 RVA: 0x0001A5E0 File Offset: 0x000187E0
		// (set) Token: 0x06000539 RID: 1337 RVA: 0x0001A680 File Offset: 0x00018880
		public string SelectedPageViewMode
		{
			get
			{
				PageDisplayModel result = ConfigManager.GetPageDisplayModelAsync(default(CancellationToken)).GetAwaiter().GetResult();
				if (result == null)
				{
					return Resources.AppSettingsSingleContinuous;
				}
				if (result.DisplayMode == SubViewModePage.SinglePage.ToString())
				{
					if (result.ContinuousDisplayMode == SubViewModeContinuous.Verticalcontinuous.ToString())
					{
						return Resources.AppSettingsSingleContinuous;
					}
					return Resources.AppSettingsSingle;
				}
				else
				{
					if (result.ContinuousDisplayMode == SubViewModeContinuous.Verticalcontinuous.ToString())
					{
						return Resources.AppSettingsDoubleContinuous;
					}
					return Resources.AppSettingsDouble;
				}
			}
			set
			{
				if (value == Resources.AppSettingsSingleContinuous)
				{
					this._selectedPageViewMode = "Singlecontinuous";
					return;
				}
				if (value == Resources.AppSettingsSingle)
				{
					this._selectedPageViewMode = "Single";
					return;
				}
				if (value == Resources.AppSettingsDoubleContinuous)
				{
					this._selectedPageViewMode = "Doublecontinuous";
					return;
				}
				if (value == Resources.AppSettingsDouble)
				{
					this._selectedPageViewMode = "Double";
					return;
				}
				this._selectedPageViewMode = "Singlecontinuous";
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x0600053A RID: 1338 RVA: 0x0001A6FC File Offset: 0x000188FC
		// (set) Token: 0x0600053B RID: 1339 RVA: 0x0001A90C File Offset: 0x00018B0C
		public string SelectedSizeMode
		{
			get
			{
				string pageDefaultSize = ConfigManager.GetPageDefaultSize();
				if (pageDefaultSize != null)
				{
					int length = pageDefaultSize.Length;
					switch (length)
					{
					case 3:
						switch (pageDefaultSize[0])
						{
						case '1':
							if (pageDefaultSize == "10%")
							{
								return "10%";
							}
							break;
						case '2':
							if (pageDefaultSize == "25%")
							{
								return "25%";
							}
							break;
						case '5':
							if (pageDefaultSize == "50%")
							{
								return "50%";
							}
							break;
						case '7':
							if (pageDefaultSize == "75%")
							{
								return "75%";
							}
							break;
						}
						break;
					case 4:
						switch (pageDefaultSize[0])
						{
						case '1':
							if (pageDefaultSize == "100%")
							{
								return "100%";
							}
							if (pageDefaultSize == "125%")
							{
								return "125%";
							}
							if (pageDefaultSize == "150%")
							{
								return "150%";
							}
							break;
						case '2':
							if (pageDefaultSize == "200%")
							{
								return "200%";
							}
							break;
						case '4':
							if (pageDefaultSize == "400%")
							{
								return "400%";
							}
							break;
						case '6':
							if (pageDefaultSize == "600%")
							{
								return "600%";
							}
							break;
						}
						break;
					case 5:
					case 6:
					case 7:
					case 8:
						break;
					case 9:
						if (pageDefaultSize == "FitToSize")
						{
							return Resources.MenuViewFitPageContent;
						}
						break;
					case 10:
						if (pageDefaultSize == "FitToWidth")
						{
							return Resources.MenuViewWidthContent;
						}
						break;
					case 11:
						if (pageDefaultSize == "FitToHeight")
						{
							return Resources.MenuViewHeightContent;
						}
						break;
					default:
						if (length == 14)
						{
							if (pageDefaultSize == "ZoomActualSize")
							{
								return Resources.MenuViewFullSizeContent;
							}
						}
						break;
					}
				}
				return Resources.MenuViewFullSizeContent;
			}
			set
			{
				if (value == Resources.MenuViewFullSizeContent)
				{
					this._selectedSizemode = "ZoomActualSize";
					return;
				}
				if (value == Resources.MenuViewFitPageContent)
				{
					this._selectedSizemode = "FitToSize";
					return;
				}
				if (value == Resources.MenuViewWidthContent)
				{
					this._selectedSizemode = "FitToWidth";
					return;
				}
				if (value == Resources.MenuViewHeightContent)
				{
					this._selectedSizemode = "FitToHeight";
					return;
				}
				if (value == "10%")
				{
					this._selectedSizemode = "10%";
					return;
				}
				if (value == "25%")
				{
					this._selectedSizemode = "25%";
					return;
				}
				if (value == "50%")
				{
					this._selectedSizemode = "50%";
					return;
				}
				if (value == "75%")
				{
					this._selectedSizemode = "75%";
					return;
				}
				if (value == "100%")
				{
					this._selectedSizemode = "100%";
					return;
				}
				if (value == "125%")
				{
					this._selectedSizemode = "125%";
					return;
				}
				if (value == "150%")
				{
					this._selectedSizemode = "150%";
					return;
				}
				if (value == "200%")
				{
					this._selectedSizemode = "200%";
					return;
				}
				if (value == "400%")
				{
					this._selectedSizemode = "400%";
					return;
				}
				if (value == "600%")
				{
					this._selectedSizemode = "600%";
					return;
				}
				this._selectedSizemode = "ZoomActualSize";
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x0600053C RID: 1340 RVA: 0x0001AA82 File Offset: 0x00018C82
		// (set) Token: 0x0600053D RID: 1341 RVA: 0x0001AA8A File Offset: 0x00018C8A
		public LanguageModel SelectedLanguage
		{
			get
			{
				return this.selectedLanguage;
			}
			set
			{
				base.SetProperty<LanguageModel>(ref this.selectedLanguage, value, "SelectedLanguage");
			}
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x0600053E RID: 1342 RVA: 0x0001AAA0 File Offset: 0x00018CA0
		public LanguageModel ActualLanguage
		{
			get
			{
				string appLang = ConfigManager.GetApplicationLanugageName();
				return this.Languages.FirstOrDefault((LanguageModel c) => c.Name == appLang) ?? LanguageModel.Fallback;
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x0600053F RID: 1343 RVA: 0x0001AAE0 File Offset: 0x00018CE0
		public AsyncRelayCommand RestartCommand
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.restartCommand) == null)
				{
					asyncRelayCommand = (this.restartCommand = new AsyncRelayCommand(async delegate
					{
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000540 RID: 1344 RVA: 0x0001AB24 File Offset: 0x00018D24
		public AsyncRelayCommand SetAsDefaultCommand
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.setAsDefaultCommand) == null)
				{
					asyncRelayCommand = (this.setAsDefaultCommand = new AsyncRelayCommand(async delegate
					{
						this.SetDefaultApp();
						base.OnPropertyChanged("IsDefaultAppForPdf");
						if (!this.IsDefaultAppForPdf)
						{
							await Task.Delay(2000);
							base.OnPropertyChanged("IsDefaultAppForPdf");
						}
					}, () => !this.SetAsDefaultCommand.IsRunning));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x0001AB64 File Offset: 0x00018D64
		public void SetDefaultApp()
		{
			if (this.IsDefaultAppForPdf)
			{
				ModernMessageBox.Show(Resources.AppSettingsFileAssociations_AlreadyDefaultMessage.Replace("XXX", UtilManager.GetProductName()), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				GAManager.SendEvent("SettingsWindow", "SetDefaultApp", "IsDefaultAppNow", 1L);
				return;
			}
			GAManager.SendEvent("SettingsWindow", "SetDefaultApp", "UpdateDefaultApp", 1L);
			ConfigManager.SetDefaultAppAction(null);
			AppManager.RegisterFileAssociations(true);
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x0001ABD8 File Offset: 0x00018DD8
		public void UpdateAppSettingsState()
		{
			if (this.initLanguageName != null)
			{
				string text = this.initLanguageName;
				LanguageModel languageModel = this.selectedLanguage;
				if (text != ((languageModel != null) ? languageModel.Name : null))
				{
					LanguageModel languageModel2 = this.selectedLanguage;
					ConfigManager.SetApplicationLanugageName(((languageModel2 != null) ? languageModel2.Name : null) ?? "");
					this.LanguageChangedFlag = true;
					ModernMessageBox.Show(Resources.AppSettingsLanguageRestartTips, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					GAManager.SendEvent("SettingsWindow", "ChangeLanguage", "Count", 1L);
				}
			}
			if (!string.IsNullOrEmpty(this._selectedSizemode) && this._selectedSizemode != ConfigManager.GetPageDefaultSize())
			{
				ConfigManager.SetPageDefaultSize(this._selectedSizemode);
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				DocumentWrapper documentWrapper = requiredService.DocumentWrapper;
				ConfigManager.SetPageSizeZoomModelAsync((documentWrapper != null) ? documentWrapper.DocumentPath : null, requiredService.ViewToolbar.DocSizeModeWrap.ToString(), requiredService.ViewToolbar.DocZoom);
				GAManager.SendEvent("SettingsWindow", "ChangeDefaultSizemode", this._selectedSizemode, 1L);
			}
			if (!string.IsNullOrEmpty(this._selectedPageViewMode) && this._selectedPageViewMode != ConfigManager.GetDefaultPageviewMode())
			{
				if (this._selectedPageViewMode == "Singlecontinuous")
				{
					ConfigManager.SetPageDisplayModeAsync(SubViewModePage.SinglePage.ToString(), SubViewModeContinuous.Verticalcontinuous.ToString());
				}
				else if (this._selectedPageViewMode == "Single")
				{
					ConfigManager.SetPageDisplayModeAsync(SubViewModePage.SinglePage.ToString(), SubViewModeContinuous.Discontinuous.ToString());
				}
				else if (this._selectedPageViewMode == "Doublecontinuous")
				{
					ConfigManager.SetPageDisplayModeAsync(SubViewModePage.DoublePages.ToString(), SubViewModeContinuous.Verticalcontinuous.ToString());
				}
				else
				{
					ConfigManager.SetPageDisplayModeAsync(SubViewModePage.DoublePages.ToString(), SubViewModeContinuous.Discontinuous.ToString());
				}
				ConfigManager.SetDefaultPageviewMode(this._selectedPageViewMode);
				GAManager.SendEvent("SettingsWindow", "ChangeDefaultPageviewmode", this._selectedPageViewMode, 1L);
			}
			if (!string.IsNullOrEmpty(this._selectionMode) && this._selectionMode != ConfigManager.GetDefaultSelectionMode())
			{
				ConfigManager.SetDefaultSelectionMode(this._selectionMode);
				GAManager.SendEvent("SettingsWindow", "ChangeDefaultSelectionmode", this._selectionMode, 1L);
			}
			if (this.ChatButtonSettings != ConfigManager.GetShowcaseChatButtonFlag())
			{
				ConfigManager.SetShowcaseChatButtonFlag(this.ChatButtonSettings);
				MainViewModel requiredService2 = Ioc.Default.GetRequiredService<MainViewModel>();
				requiredService2.ChatButtonVisible = this.ChatButtonSettings;
				if (!this.ChatButtonSettings)
				{
					GAManager.SendEvent("SettingsWindow", "EnableChat", "False", 1L);
					requiredService2.ChatPanelVisible = false;
				}
				else
				{
					GAManager.SendEvent("SettingsWindow", "EnableChat", "True", 1L);
					requiredService2.ChatPanelVisible = true;
				}
			}
			if (this.IsFillFormHighlightedSettings != ConfigManager.GetIsFillFormHighlightedFlag())
			{
				GAManager.SendEvent("SettingsWindow", "FillFormHighlightedSettings", string.Format("{0}", this.IsFillFormHighlightedSettings), 1L);
				ConfigManager.SetIsFillFormHighlightedFlag(this.IsFillFormHighlightedSettings);
				PdfControl pdfControl = PdfControl.GetPdfControl(Ioc.Default.GetRequiredService<MainViewModel>().Document);
				if (pdfControl != null)
				{
					pdfControl.Viewer.IsFillFormHighlighted = this.IsFillFormHighlightedSettings;
				}
			}
			if (this.ShowToolbar != ConfigManager.GetLaunchAPPShowFlag("LaunchToolbar"))
			{
				ConfigManager.SetLaunchAPPShowFlag("LaunchToolbar", this.ShowToolbar);
				GAManager.SendEvent("SettingsWindow", "ShowToolbar", this.ShowToolbar.ToString(), 1L);
			}
			if (this.ShowStatusbar != ConfigManager.GetLaunchAPPShowFlag("LaunchStatusbar"))
			{
				ConfigManager.SetLaunchAPPShowFlag("LaunchStatusbar", this.ShowStatusbar);
				GAManager.SendEvent("SettingsWindow", "ShowStatusbar", this.ShowStatusbar.ToString(), 1L);
			}
			if (this.ShowOCR != ConfigManager.GetLaunchAPPShowFlag("LaunchOCR"))
			{
				ConfigManager.SetLaunchAPPShowFlag("LaunchOCR", this.ShowOCR);
				GAManager.SendEvent("SettingsWindow", "ShowOCR", this.ShowOCR.ToString(), 1L);
			}
			if (this.ShowRightMenu != ConfigManager.GetSelectTextShowMenuFlag())
			{
				ConfigManager.SetSelectTextShowMenuFlag(this.ShowRightMenu);
				GAManager.SendEvent("SettingsWindow", "SelecteTextShowMenu", this.ShowRightMenu.ToString(), 1L);
			}
			if (!string.IsNullOrEmpty(this._orientation) && this._orientation != ConfigManager.GetDefaultBlankPageOrinentation())
			{
				ConfigManager.SetDefaultBlankPageOrinentation(this._orientation);
				GAManager.SendEvent("SettingsWindow", "DefaultBlankOrientation", this._orientation, 1L);
			}
			if (!string.IsNullOrEmpty(this._blankPageSize) && this._blankPageSize != ConfigManager.GetDefaultBlankPageSize())
			{
				ConfigManager.SetDefaultBlankPageSize(this._blankPageSize);
				GAManager.SendEvent("SettingsWindow", "DefaultBlankBlankPageSize", this._orientation, 1L);
				if (this._blankPageSize == "Custom")
				{
					ConfigManager.SetDefaultBlankPageHeight(this.BlankPageHeight);
					ConfigManager.SetDefaultBlankPageWidth(this.BlankPageWidth);
				}
			}
		}

		// Token: 0x040002B2 RID: 690
		private string initLanguageName;

		// Token: 0x040002B3 RID: 691
		private AsyncRelayCommand restartCommand;

		// Token: 0x040002B4 RID: 692
		private AsyncRelayCommand setAsDefaultCommand;

		// Token: 0x040002B5 RID: 693
		private ObservableCollection<LanguageModel> languages;

		// Token: 0x040002B6 RID: 694
		private LanguageModel selectedLanguage;

		// Token: 0x040002B7 RID: 695
		private string _selectedSizemode;

		// Token: 0x040002B8 RID: 696
		private string _selectedPageViewMode;

		// Token: 0x040002B9 RID: 697
		private string _blankPageSize;

		// Token: 0x040002BA RID: 698
		private string _selectionMode;

		// Token: 0x040002BB RID: 699
		private string _orientation;

		// Token: 0x040002BC RID: 700
		public bool _chatButtonSettings = true;

		// Token: 0x040002BD RID: 701
		public bool _isFillFormHighlightedSettings = true;

		// Token: 0x040002BE RID: 702
		private SizeF A4SizeF = new SizeF(210f, 297f);

		// Token: 0x040002BF RID: 703
		private SizeF A3SizeF = new SizeF(297f, 420f);

		// Token: 0x040002C0 RID: 704
		private SizeF DefaultSize = new SizeF(210f, 297f);

		// Token: 0x040002C1 RID: 705
		public bool _showToolbar = true;

		// Token: 0x040002C2 RID: 706
		public bool _showStatusbar = true;

		// Token: 0x040002C3 RID: 707
		public bool _showOcr = true;

		// Token: 0x040002C4 RID: 708
		public bool _showRightMenu = true;

		// Token: 0x040002C5 RID: 709
		private int _blankPageWidth;

		// Token: 0x040002C6 RID: 710
		private int _blankPageHeight;
	}
}
