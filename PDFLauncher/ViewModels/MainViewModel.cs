using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommonLib.Account;
using CommonLib.Common;
using CommonLib.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PDFLauncher.Models;
using PDFLauncher.Properties;
using PDFLauncher.Utils;

namespace PDFLauncher.ViewModels
{
	// Token: 0x02000012 RID: 18
	public class MainViewModel : ObservableObject
	{
		// Token: 0x060000ED RID: 237 RVA: 0x000047FC File Offset: 0x000029FC
		public MainViewModel()
		{
			this.ReadPropertySource();
			Application.Current.Dispatcher.InvokeAsync<Task>(async delegate
			{
				await this.ReadHistory();
			}, DispatcherPriority.Loaded);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x000048B0 File Offset: 0x00002AB0
		private void ReadPropertySource()
		{
			this.menus.Add(Resources.WinListHeaderTabHotTools);
			this.menus.Add(Resources.WinListHeaderConvertFromPDF);
			this.menus.Add(Resources.WinListHeaderConvertToPDF);
			this.menus.Add(Resources.WinListHeaderMergeOrSplit);
			this.menus.Add(Resources.WinListHeaderTabAllTools);
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060000EF RID: 239 RVA: 0x0000490D File Offset: 0x00002B0D
		// (set) Token: 0x060000F0 RID: 240 RVA: 0x00004915 File Offset: 0x00002B15
		public bool ClearbtnIsEnable
		{
			get
			{
				return this.clearbtnIsEnable;
			}
			set
			{
				base.SetProperty<bool>(ref this.clearbtnIsEnable, value, "ClearbtnIsEnable");
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x060000F1 RID: 241 RVA: 0x0000492A File Offset: 0x00002B2A
		// (set) Token: 0x060000F2 RID: 242 RVA: 0x00004932 File Offset: 0x00002B32
		public Visibility OpenBtnTipsVisibility
		{
			get
			{
				return this.openBtnTipsVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.openBtnTipsVisibility, value, "OpenBtnTipsVisibility");
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060000F3 RID: 243 RVA: 0x00004947 File Offset: 0x00002B47
		// (set) Token: 0x060000F4 RID: 244 RVA: 0x0000494F File Offset: 0x00002B4F
		public Visibility AllToolsSwitchIsVisile
		{
			get
			{
				return this.allToolsSwitchIsVisile;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.allToolsSwitchIsVisile, value, "AllToolsSwitchIsVisile");
			}
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x00004964 File Offset: 0x00002B64
		// (set) Token: 0x060000F6 RID: 246 RVA: 0x0000496C File Offset: 0x00002B6C
		public Visibility PDFContextVisibility
		{
			get
			{
				return this.pdfContextVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.pdfContextVisibility, value, "PDFContextVisibility");
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060000F7 RID: 247 RVA: 0x00004981 File Offset: 0x00002B81
		// (set) Token: 0x060000F8 RID: 248 RVA: 0x00004989 File Offset: 0x00002B89
		public Visibility PDFMenuItemVisibility
		{
			get
			{
				return this.pdfMenuItemVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.pdfMenuItemVisibility, value, "PDFMenuItemVisibility");
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x0000499E File Offset: 0x00002B9E
		// (set) Token: 0x060000FA RID: 250 RVA: 0x000049A6 File Offset: 0x00002BA6
		public Visibility OtherContextVisibility
		{
			get
			{
				return this.otherContextVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.otherContextVisibility, value, "OtherContextVisibility");
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060000FB RID: 251 RVA: 0x000049BB File Offset: 0x00002BBB
		// (set) Token: 0x060000FC RID: 252 RVA: 0x000049C3 File Offset: 0x00002BC3
		public Visibility ImgContextVisibility
		{
			get
			{
				return this.imgContextVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.imgContextVisibility, value, "ImgContextVisibility");
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060000FD RID: 253 RVA: 0x000049D8 File Offset: 0x00002BD8
		// (set) Token: 0x060000FE RID: 254 RVA: 0x000049E0 File Offset: 0x00002BE0
		public Visibility DocContextVisibility
		{
			get
			{
				return this.docContextVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.docContextVisibility, value, "DocContextVisibility");
			}
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060000FF RID: 255 RVA: 0x000049F5 File Offset: 0x00002BF5
		// (set) Token: 0x06000100 RID: 256 RVA: 0x000049FD File Offset: 0x00002BFD
		public Visibility XlsContextVisibility
		{
			get
			{
				return this.xlsContextVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.xlsContextVisibility, value, "XlsContextVisibility");
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000101 RID: 257 RVA: 0x00004A12 File Offset: 0x00002C12
		// (set) Token: 0x06000102 RID: 258 RVA: 0x00004A1A File Offset: 0x00002C1A
		public Visibility PPTContextVisibility
		{
			get
			{
				return this.pptContextVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.pptContextVisibility, value, "PPTContextVisibility");
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00004A2F File Offset: 0x00002C2F
		// (set) Token: 0x06000104 RID: 260 RVA: 0x00004A37 File Offset: 0x00002C37
		public Visibility RTFContextVisibility
		{
			get
			{
				return this.rtfContextVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.rtfContextVisibility, value, "RTFContextVisibility");
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000105 RID: 261 RVA: 0x00004A4C File Offset: 0x00002C4C
		// (set) Token: 0x06000106 RID: 262 RVA: 0x00004A54 File Offset: 0x00002C54
		public Visibility TXTContextVisibility
		{
			get
			{
				return this.txtContextVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.txtContextVisibility, value, "TXTContextVisibility");
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000107 RID: 263 RVA: 0x00004A69 File Offset: 0x00002C69
		// (set) Token: 0x06000108 RID: 264 RVA: 0x00004A71 File Offset: 0x00002C71
		public Visibility OtherFormatVisibility
		{
			get
			{
				return this.otherFormatVisibility;
			}
			set
			{
				base.SetProperty<Visibility>(ref this.otherFormatVisibility, value, "OtherFormatVisibility");
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000109 RID: 265 RVA: 0x00004A88 File Offset: 0x00002C88
		// (set) Token: 0x0600010A RID: 266 RVA: 0x00004AAD File Offset: 0x00002CAD
		public ObservableCollection<OpenHistoryModel> HistoryModels
		{
			get
			{
				ObservableCollection<OpenHistoryModel> observableCollection;
				if ((observableCollection = this.historyModels) == null)
				{
					observableCollection = (this.historyModels = new ObservableCollection<OpenHistoryModel>());
				}
				return observableCollection;
			}
			set
			{
				base.SetProperty<ObservableCollection<OpenHistoryModel>>(ref this.historyModels, value, "HistoryModels");
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x0600010B RID: 267 RVA: 0x00004AC2 File Offset: 0x00002CC2
		// (set) Token: 0x0600010C RID: 268 RVA: 0x00004ACA File Offset: 0x00002CCA
		public bool? IsAllFileSelect
		{
			get
			{
				return this.isAllFileSelect;
			}
			set
			{
				base.SetProperty<bool?>(ref this.isAllFileSelect, value, "IsAllFileSelect");
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x0600010D RID: 269 RVA: 0x00004ADF File Offset: 0x00002CDF
		// (set) Token: 0x0600010E RID: 270 RVA: 0x00004AE7 File Offset: 0x00002CE7
		public bool AllToolsSwitchIsChecked
		{
			get
			{
				return this.allSwitchChecked;
			}
			set
			{
				base.SetProperty<bool>(ref this.allSwitchChecked, value, "AllToolsSwitchIsChecked");
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x0600010F RID: 271 RVA: 0x00004AFC File Offset: 0x00002CFC
		// (set) Token: 0x06000110 RID: 272 RVA: 0x00004B04 File Offset: 0x00002D04
		public UserInfoModel UserInfoModel
		{
			get
			{
				return this.userInfoModel;
			}
			set
			{
				base.SetProperty<UserInfoModel>(ref this.userInfoModel, value, "UserInfoModel");
			}
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00004B1C File Offset: 0x00002D1C
		public async Task ReadHistory()
		{
			try
			{
				ObservableCollection<OpenHistoryModel> historyModels = new ObservableCollection<OpenHistoryModel>();
				IReadOnlyList<OpenedInfo> readOnlyList = await HistoryManager.ReadHistoryAsync(default(CancellationToken));
				if (readOnlyList.Count == 0)
				{
					this.HistoryModels.Clear();
					this.selectedModels.Clear();
					this.SelectItemsPropertyChange();
				}
				else
				{
					foreach (OpenedInfo openedInfo in readOnlyList)
					{
						if (Path.GetDirectoryName(openedInfo.FilePath) == Path.Combine(UtilManager.GetTemporaryPath(), "Documents"))
						{
							bool flag;
							HistoryManager.RemoveHistoryItem(openedInfo.FilePath, out flag);
						}
						else
						{
							LongPathFile longPathFile = openedInfo.FilePath;
							if (!longPathFile.IsExists)
							{
								historyModels.Add(new OpenHistoryModel
								{
									FilePath = openedInfo.FilePath,
									FileName = Path.GetFileName(openedInfo.FilePath),
									FileSize = "0",
									FileLastOpen = openedInfo.LastAccessTime.LocalDateTime.ToString("yyyy/MM/dd HH:mm:ss")
								});
							}
							else
							{
								OpenHistoryModel openHistoryModel = new OpenHistoryModel();
								openHistoryModel.FilePath = longPathFile;
								openHistoryModel.FileName = FileInfoUtils.GetFileName(openedInfo.FilePath);
								openHistoryModel.FileSize = FileInfoUtils.GetFileSize(openedInfo.FilePath);
								openHistoryModel.FileLastOpen = openedInfo.LastAccessTime.LocalDateTime.ToString("yyyy/MM/dd HH:mm:ss");
								long num = 0L;
								long.TryParse(openedInfo.OpenDate, out num);
								historyModels.Add(openHistoryModel);
							}
						}
					}
					this.HistoryModels = historyModels;
					this.SelectItemsPropertyChange();
					historyModels = null;
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00004B60 File Offset: 0x00002D60
		public void SelectAll()
		{
			this.selectedModels = null;
			this.selectedModels = this.HistoryModels.ToList<OpenHistoryModel>();
			foreach (OpenHistoryModel openHistoryModel in this.selectedModels)
			{
				openHistoryModel.IsSelect = true;
			}
			this.IsAllFileSelect = new bool?(true);
			List<string> list = this.distinctFileFormat(this.HistoryModels.ToList<OpenHistoryModel>());
			if (list.Count != 1)
			{
				this.AllToolsSwitchIsVisile = Visibility.Collapsed;
				return;
			}
			if (this.parseBtnVisibility(list.First<string>()))
			{
				this.AllToolsSwitchIsVisile = Visibility.Visible;
				return;
			}
			this.AllToolsSwitchIsVisile = Visibility.Collapsed;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00004C18 File Offset: 0x00002E18
		public void ClearHistory()
		{
			this.HistoryModels.Clear();
			this.selectedModels.Clear();
			HistoryManager.RemoveAllItems();
			this.SelectItemsPropertyChange();
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00004C3C File Offset: 0x00002E3C
		public void SelectItemsPropertyChange()
		{
			this.selectedModels = this.HistoryModels.Where((OpenHistoryModel x) => x.IsSelect).ToList<OpenHistoryModel>();
			if (this.selectedModels == null)
			{
				this.IsAllFileSelect = new bool?(false);
				this.allToolsSwitchIsVisile = Visibility.Hidden;
			}
			List<OpenHistoryModel> list = this.selectedModels;
			if (list != null && list.Count == 0)
			{
				this.IsAllFileSelect = new bool?(false);
				this.allToolsSwitchIsVisile = Visibility.Hidden;
			}
			else
			{
				List<OpenHistoryModel> list2 = this.selectedModels;
				if (list2 != null && list2.Count > 0)
				{
					if (this.selectedModels.Count == this.HistoryModels.Count)
					{
						this.IsAllFileSelect = new bool?(true);
					}
					else
					{
						this.IsAllFileSelect = null;
					}
				}
			}
			List<OpenHistoryModel> list3 = this.selectedModels;
			if (list3 != null && list3.Count > 1)
			{
				List<string> list4 = this.distinctFileFormat(this.selectedModels);
				if (list4.Count == 1)
				{
					if (this.parseBtnVisibility(list4.First<string>()))
					{
						this.AllToolsSwitchIsVisile = Visibility.Visible;
					}
					else
					{
						this.AllToolsSwitchIsVisile = Visibility.Collapsed;
					}
				}
				else
				{
					this.AllToolsSwitchIsVisile = Visibility.Collapsed;
				}
				this.ClearbtnIsEnable = true;
				return;
			}
			this.AllToolsSwitchIsVisile = Visibility.Hidden;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00004D74 File Offset: 0x00002F74
		private bool IsSupportedToPDFFormat(string ext)
		{
			return !string.IsNullOrEmpty(ext) && (this.wordExtention.Contains(ext) || this.excelExtention.Contains(ext) || this.pptxExtention.Contains(ext) || this.imageExtention.Contains(ext) || this.rtfExtention.Contains(ext) || this.txtExtention.Contains(ext));
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00004DEC File Offset: 0x00002FEC
		public void SelectFilesSupportPropertChange(OpenHistoryModel model)
		{
			if (!string.IsNullOrEmpty(model.Extension))
			{
				if (model.Extension == ".pdf")
				{
					this.PDFContextVisibility = Visibility.Visible;
					this.OtherContextVisibility = Visibility.Collapsed;
					return;
				}
				if (this.IsSupportedToPDFFormat(model.Extension))
				{
					this.PDFContextVisibility = Visibility.Collapsed;
					this.OtherContextVisibility = Visibility.Visible;
					return;
				}
			}
			this.PDFContextVisibility = Visibility.Collapsed;
			this.OtherContextVisibility = Visibility.Collapsed;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00004E54 File Offset: 0x00003054
		public void SelectNone()
		{
			foreach (OpenHistoryModel openHistoryModel in this.HistoryModels)
			{
				openHistoryModel.IsSelect = false;
			}
			this.IsAllFileSelect = new bool?(false);
			this.AllToolsSwitchIsVisile = Visibility.Collapsed;
			this.selectedModels = null;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00004EBC File Offset: 0x000030BC
		private bool parseBtnVisibility(string extention)
		{
			bool flag = false;
			this.DocContextVisibility = Visibility.Collapsed;
			this.XlsContextVisibility = Visibility.Collapsed;
			this.ImgContextVisibility = Visibility.Collapsed;
			this.PPTContextVisibility = Visibility.Collapsed;
			this.RTFContextVisibility = Visibility.Collapsed;
			this.TXTContextVisibility = Visibility.Collapsed;
			this.PDFMenuItemVisibility = Visibility.Collapsed;
			if (this.wordExtention.Contains(extention))
			{
				this.DocContextVisibility = Visibility.Visible;
				flag = true;
			}
			else if (this.excelExtention.Contains(extention))
			{
				this.XlsContextVisibility = Visibility.Visible;
				flag = true;
			}
			else if (this.imageExtention.Contains(extention))
			{
				this.ImgContextVisibility = Visibility.Visible;
				flag = true;
			}
			else if (this.pptxExtention.Contains(extention))
			{
				this.PPTContextVisibility = Visibility.Visible;
				flag = true;
			}
			else if (this.rtfExtention.Contains(extention))
			{
				this.RTFContextVisibility = Visibility.Visible;
				flag = true;
			}
			else if (this.txtExtention.Contains(extention))
			{
				this.TXTContextVisibility = Visibility.Visible;
				flag = true;
			}
			else if (extention == ".pdf")
			{
				this.PDFMenuItemVisibility = Visibility.Visible;
				flag = true;
			}
			return flag;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00004FAC File Offset: 0x000031AC
		private List<string> distinctFileFormat(List<OpenHistoryModel> models)
		{
			if (models == null)
			{
				return new List<string>();
			}
			List<string> list = new List<string>();
			foreach (OpenHistoryModel openHistoryModel in models)
			{
				if (string.IsNullOrEmpty(openHistoryModel.Extension))
				{
					if (!list.Contains(".notsupport"))
					{
						list.Add(".notsupport");
					}
				}
				else if (openHistoryModel.Extension == ".pdf")
				{
					if (!list.Contains(".pdf"))
					{
						list.Add(".pdf");
					}
				}
				else if (this.wordExtention.Contains(openHistoryModel.Extension))
				{
					if (!list.Contains(".doc"))
					{
						list.Add(".doc");
					}
				}
				else if (this.excelExtention.Contains(openHistoryModel.Extension))
				{
					if (!list.Contains(".xls"))
					{
						list.Add(".xls");
					}
				}
				else if (this.pptxExtention.Contains(openHistoryModel.Extension))
				{
					if (!list.Contains(".ppt"))
					{
						list.Add(".ppt");
					}
				}
				else if (this.imageExtention.Contains(openHistoryModel.Extension))
				{
					if (!list.Contains(".png"))
					{
						list.Add(".png");
					}
				}
				else if (this.rtfExtention.Contains(openHistoryModel.Extension))
				{
					if (!list.Contains(".rtf"))
					{
						list.Add(".rtf");
					}
				}
				else if (this.txtExtention.Contains(openHistoryModel.Extension))
				{
					if (!list.Contains(".txt"))
					{
						list.Add(".txt");
					}
				}
				else if (!list.Contains(".notsupport"))
				{
					list.Add(".notsupport");
				}
			}
			return list;
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600011A RID: 282 RVA: 0x000051AC File Offset: 0x000033AC
		// (set) Token: 0x0600011B RID: 283 RVA: 0x000051B4 File Offset: 0x000033B4
		public List<string> Menu
		{
			get
			{
				return this.menus;
			}
			set
			{
				base.SetProperty<List<string>>(ref this.menus, value, "Menu");
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600011C RID: 284 RVA: 0x000051CC File Offset: 0x000033CC
		public RelayCommand<object> RemoveOne
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.removeOne) == null)
				{
					relayCommand = (this.removeOne = new RelayCommand<object>(delegate(object args)
					{
						OpenHistoryModel openHistoryModel = args as OpenHistoryModel;
						this.HistoryModels.Remove(openHistoryModel);
						if (this.selectedModels != null)
						{
							this.selectedModels.Remove(openHistoryModel);
						}
						bool flag;
						HistoryManager.RemoveHistoryItem(openHistoryModel.FilePath, out flag);
						this.SelectItemsPropertyChange();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x0600011D RID: 285 RVA: 0x00005200 File Offset: 0x00003400
		public RelayCommand<object> OpenFolder
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.openFolder) == null)
				{
					relayCommand = (this.openFolder = new RelayCommand<object>(delegate(object args)
					{
						OpenHistoryModel openHistoryModel = args as OpenHistoryModel;
						string text = ((openHistoryModel != null) ? openHistoryModel.FilePath : null);
						LongPathFile longPathFile = text;
						if (!longPathFile.IsExists)
						{
							ModernMessageBox.Show(Resources.OpenFileNoExistMsg + text, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							this.RemoveOne.Execute(openHistoryModel);
							return;
						}
						ExplorerUtils.SelectItemInExplorerAsync(longPathFile, default(CancellationToken));
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x0600011E RID: 286 RVA: 0x00005234 File Offset: 0x00003434
		public RelayCommand ClearSelectModel
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.clearSelectModel) == null)
				{
					relayCommand = (this.clearSelectModel = new RelayCommand(delegate
					{
						if (this.HistoryModels.Count == 0)
						{
							return;
						}
						if (ModernMessageBox.Show(Resources.WinHistoryClearLabelClickTips, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.Yes)
						{
							this.ClearHistory();
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00005268 File Offset: 0x00003468
		public RelayCommand<object> RunEditor
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.runEditor) == null)
				{
					relayCommand = (this.runEditor = new RelayCommand<object>(delegate(object args)
					{
						OpenHistoryModel openHistoryModel = args as OpenHistoryModel;
						GAManager.SendEvent("WelcomeWindow", "MenuOpenFileBtnClick", "Count", 1L);
						FileManager.OpenOneFile(openHistoryModel.FilePath, null);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000120 RID: 288 RVA: 0x000052AC File Offset: 0x000034AC
		public RelayCommand OpenOneFileCMD
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.openOneFileCMD) == null)
				{
					relayCommand = (this.openOneFileCMD = new RelayCommand(delegate
					{
						ConfigManager.SetWelcomeOpenBtnTipsFlag(true);
						this.OpenBtnTipsVisibility = Visibility.Collapsed;
						string text = FileManager.SelectFileForOpen();
						if (string.IsNullOrWhiteSpace(text))
						{
							return;
						}
						GAManager.SendEvent("WelcomeWindow", "OpenOneFileBtnClick", "Count", 1L);
						DocsPathUtils.WriteFilesPathJson("unknow", text, null);
						FileManager.OpenOneFile(text, null);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000121 RID: 289 RVA: 0x000052E0 File Offset: 0x000034E0
		public RelayCommand CreateFileFromScannerCMD
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.createFileFromScannerCMD) == null)
				{
					relayCommand = (this.createFileFromScannerCMD = new RelayCommand(delegate
					{
						GAManager.SendEvent("WelcomeWindow", "NewFileFromScannerBtnClick", "Count", 1L);
						CreateNewFileUtils.CreatePDFFromScannner();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000122 RID: 290 RVA: 0x00005324 File Offset: 0x00003524
		public RelayCommand CreateFileCMD
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.createFileCMD) == null)
				{
					relayCommand = (this.createFileCMD = new RelayCommand(delegate
					{
						GAManager.SendEvent("WelcomeWindow", "NewFileBtnClick", "Count", 1L);
						CreateNewFileUtils.CreateBlankPDFByEditor();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000123 RID: 291 RVA: 0x00005368 File Offset: 0x00003568
		public RelayCommand UpgradeCMD
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.upgradeCMD) == null)
				{
					relayCommand = (this.upgradeCMD = new RelayCommand(delegate
					{
						GAManager.SendEvent("WelcomeWindow", "UpgradeBtnClick", "Count", 1L);
						new IAPWin(IAPType.APP).ShowDialog();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000124 RID: 292 RVA: 0x000053AC File Offset: 0x000035AC
		public RelayCommand FillFormCMD
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.fillFormCMD) == null)
				{
					relayCommand = (this.fillFormCMD = new RelayCommand(delegate
					{
						string text = FileManager.SelectPDFFileForOpen();
						if (string.IsNullOrWhiteSpace(text))
						{
							return;
						}
						GAManager.SendEvent("WelcomeWindow", "FillFormBtnClick", "Count", 1L);
						DocsPathUtils.WriteFilesPathJson("unknow", text, null);
						FileManager.OpenOneFile(text, "tab:fillform");
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000125 RID: 293 RVA: 0x000053F0 File Offset: 0x000035F0
		public RelayCommand<object> CompressPDF
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.compressPDF) == null)
				{
					relayCommand = (this.compressPDF = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvToPDFType.CompressPDF, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000126 RID: 294 RVA: 0x00005424 File Offset: 0x00003624
		public RelayCommand<object> PdftoWord
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.pdftoWord) == null)
				{
					relayCommand = (this.pdftoWord = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvFromPDFType.PDFToWord, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000127 RID: 295 RVA: 0x00005458 File Offset: 0x00003658
		public RelayCommand<object> PdftoExcel
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.pdftoExcel) == null)
				{
					relayCommand = (this.pdftoExcel = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvFromPDFType.PDFToExcel, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x06000128 RID: 296 RVA: 0x0000548C File Offset: 0x0000368C
		public RelayCommand<object> PdftoPng
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.pdftoPng) == null)
				{
					relayCommand = (this.pdftoPng = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvFromPDFType.PDFToPng, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000129 RID: 297 RVA: 0x000054C0 File Offset: 0x000036C0
		public RelayCommand<object> PdftoRtf
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.pdftoRtf) == null)
				{
					relayCommand = (this.pdftoRtf = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvFromPDFType.PDFToRTF, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x0600012A RID: 298 RVA: 0x000054F4 File Offset: 0x000036F4
		public RelayCommand<object> PdftoTxt
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.pdftoTxt) == null)
				{
					relayCommand = (this.pdftoTxt = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvFromPDFType.PDFToTxt, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600012B RID: 299 RVA: 0x00005528 File Offset: 0x00003728
		public RelayCommand<object> PdftoWeb
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.pdftoWeb) == null)
				{
					relayCommand = (this.pdftoWeb = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvFromPDFType.PDFToHtml, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600012C RID: 300 RVA: 0x0000555C File Offset: 0x0000375C
		public RelayCommand<object> PdftoXML
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.pdftoXML) == null)
				{
					relayCommand = (this.pdftoXML = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvFromPDFType.PDFToXml, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600012D RID: 301 RVA: 0x00005590 File Offset: 0x00003790
		public RelayCommand<object> PdftoJPG
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.pdftoJPG) == null)
				{
					relayCommand = (this.pdftoJPG = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvFromPDFType.PDFToJpg, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x0600012E RID: 302 RVA: 0x000055C4 File Offset: 0x000037C4
		public RelayCommand<object> PdftoPPT
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.pdftoPPT) == null)
				{
					relayCommand = (this.pdftoPPT = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvFromPDFType.PDFToPPT, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x0600012F RID: 303 RVA: 0x000055F8 File Offset: 0x000037F8
		public RelayCommand<object> SplitPDF
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.splitPDF) == null)
				{
					relayCommand = (this.splitPDF = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvToPDFType.SplitPDF, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000130 RID: 304 RVA: 0x0000562C File Offset: 0x0000382C
		public RelayCommand<object> MergePDF
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.mergePDF) == null)
				{
					relayCommand = (this.mergePDF = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvToPDFType.MergePDF, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00005660 File Offset: 0x00003860
		public RelayCommand<object> WordtoPDF
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.wordtoPDF) == null)
				{
					relayCommand = (this.wordtoPDF = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvToPDFType.WordToPDF, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000132 RID: 306 RVA: 0x00005694 File Offset: 0x00003894
		public RelayCommand<object> ConvtoPDF
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.covtoPDF) == null)
				{
					relayCommand = (this.covtoPDF = new RelayCommand<object>(delegate(object args)
					{
						ConvToPDFType convType = this.GetConvType((args as OpenHistoryModel).FilePath);
						if (convType == ConvToPDFType.WordToPDF || convType == ConvToPDFType.ExcelToPDF || convType == ConvToPDFType.PPTToPDF || convType == ConvToPDFType.ImageToPDF || convType == ConvToPDFType.RtfToPDF || convType == ConvToPDFType.TxtToPDF)
						{
							this.StartApp(convType, args);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000133 RID: 307 RVA: 0x000056C8 File Offset: 0x000038C8
		public RelayCommand<object> ExceltoPDF
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.exceltoPDF) == null)
				{
					relayCommand = (this.exceltoPDF = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvToPDFType.ExcelToPDF, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x06000134 RID: 308 RVA: 0x000056FC File Offset: 0x000038FC
		public RelayCommand<object> PPTtoPDF
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.ppttoPDF) == null)
				{
					relayCommand = (this.ppttoPDF = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvToPDFType.PPTToPDF, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000135 RID: 309 RVA: 0x00005730 File Offset: 0x00003930
		public RelayCommand<object> IMGtoPDF
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.imgtoPDF) == null)
				{
					relayCommand = (this.imgtoPDF = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvToPDFType.ImageToPDF, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x06000136 RID: 310 RVA: 0x00005764 File Offset: 0x00003964
		public RelayCommand<object> RTFtoPDF
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.rtftoPDF) == null)
				{
					relayCommand = (this.rtftoPDF = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvToPDFType.RtfToPDF, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000137 RID: 311 RVA: 0x00005798 File Offset: 0x00003998
		public RelayCommand<object> TXTtoPDF
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.txttoPDF) == null)
				{
					relayCommand = (this.txttoPDF = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvToPDFType.TxtToPDF, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000138 RID: 312 RVA: 0x000057CC File Offset: 0x000039CC
		public RelayCommand<object> WebtoPDF
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.webtoPDF) == null)
				{
					relayCommand = (this.webtoPDF = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvToPDFType.HtmlToPDF, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x06000139 RID: 313 RVA: 0x00005800 File Offset: 0x00003A00
		public RelayCommand<object> XMLtoPDF
		{
			get
			{
				RelayCommand<object> relayCommand;
				if ((relayCommand = this.xmltoPDF) == null)
				{
					relayCommand = (this.xmltoPDF = new RelayCommand<object>(delegate(object args)
					{
						this.StartApp(ConvToPDFType.XmlToPDf, args);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00005834 File Offset: 0x00003A34
		private void StartApp(object opType, object arg)
		{
			if (opType is ConvFromPDFType)
			{
				GAManager.SendEvent("PDFConverterBtnClick", ((ConvFromPDFType)opType).ToString(), "Welcome", 1L);
				if (!IAPUtils.IsPaidUserWrapper() && !ConfigManager.GetTest())
				{
					IAPUtils.ShowPurchaseWindows("Welcome", ".welcome");
					return;
				}
			}
			else if (opType is ConvToPDFType)
			{
				ConvToPDFType convToPDFType = (ConvToPDFType)opType;
				GAManager.SendEvent("PDFConverterBtnClick", convToPDFType.ToString(), "Welcome", 1L);
				if ((convToPDFType == ConvToPDFType.MergePDF || convToPDFType == ConvToPDFType.CompressPDF || convToPDFType == ConvToPDFType.SplitPDF) && !IAPUtils.IsPaidUserWrapper() && !ConfigManager.GetTest())
				{
					IAPUtils.ShowPurchaseWindows("Welcome", ".welcome");
					return;
				}
			}
			OpenHistoryModel openHistoryModel = arg as OpenHistoryModel;
			List<string> list = new List<string>();
			if (openHistoryModel != null)
			{
				LongPathFile longPathFile = openHistoryModel.FilePath;
				if (!longPathFile.IsExists)
				{
					ModernMessageBox.Show(Resources.OpenFileNoExistMsg + openHistoryModel.FilePath, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					this.RemoveOne.Execute(openHistoryModel);
					return;
				}
				list.Add(longPathFile.FullPathWithoutPrefix);
			}
			else if (this.selectedModels != null)
			{
				foreach (OpenHistoryModel openHistoryModel2 in this.selectedModels)
				{
					LongPathFile longPathFile2 = openHistoryModel2.FilePath;
					if (longPathFile2.IsExists)
					{
						list.Add(longPathFile2.FullPathWithoutPrefix);
					}
				}
			}
			DocsPathUtils.WriteFilesPathJson("", list, null);
			if (opType is ConvToPDFType)
			{
				ConvToPDFType convToPDFType2 = (ConvToPDFType)opType;
				CommonLib.Common.AppManager.OpenPDFConverterToPdf(convToPDFType2, list.ToArray());
			}
			else if (opType is ConvFromPDFType)
			{
				ConvFromPDFType convFromPDFType = (ConvFromPDFType)opType;
				CommonLib.Common.AppManager.OpenPDFConverterFromPdf(convFromPDFType, list.ToArray());
			}
			this.AllToolsSwitchIsChecked = false;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00005A00 File Offset: 0x00003C00
		private ConvToPDFType GetConvType(string path)
		{
			string extension = new FileInfo(path).Extension;
			if (string.IsNullOrEmpty(extension))
			{
				return ConvToPDFType.MergePDF;
			}
			if (this.wordExtention.Contains(extension))
			{
				return ConvToPDFType.WordToPDF;
			}
			if (this.excelExtention.Contains(extension))
			{
				return ConvToPDFType.ExcelToPDF;
			}
			if (this.pptxExtention.Contains(extension))
			{
				return ConvToPDFType.PPTToPDF;
			}
			if (this.imageExtention.Contains(extension))
			{
				return ConvToPDFType.ImageToPDF;
			}
			if (this.rtfExtention.Contains(extension))
			{
				return ConvToPDFType.RtfToPDF;
			}
			if (this.txtExtention.Contains(extension))
			{
				return ConvToPDFType.TxtToPDF;
			}
			return ConvToPDFType.MergePDF;
		}

		// Token: 0x0400007E RID: 126
		private Visibility openBtnTipsVisibility = Visibility.Collapsed;

		// Token: 0x0400007F RID: 127
		private Visibility allToolsSwitchIsVisile = Visibility.Hidden;

		// Token: 0x04000080 RID: 128
		private Visibility pdfContextVisibility;

		// Token: 0x04000081 RID: 129
		private Visibility pdfMenuItemVisibility;

		// Token: 0x04000082 RID: 130
		private Visibility imgContextVisibility;

		// Token: 0x04000083 RID: 131
		private Visibility docContextVisibility;

		// Token: 0x04000084 RID: 132
		private Visibility xlsContextVisibility;

		// Token: 0x04000085 RID: 133
		private Visibility pptContextVisibility;

		// Token: 0x04000086 RID: 134
		private Visibility rtfContextVisibility;

		// Token: 0x04000087 RID: 135
		private Visibility txtContextVisibility;

		// Token: 0x04000088 RID: 136
		private Visibility otherFormatVisibility;

		// Token: 0x04000089 RID: 137
		private Visibility otherContextVisibility;

		// Token: 0x0400008A RID: 138
		private ObservableCollection<OpenHistoryModel> historyModels;

		// Token: 0x0400008B RID: 139
		private bool? isAllFileSelect = new bool?(false);

		// Token: 0x0400008C RID: 140
		private bool allSwitchChecked;

		// Token: 0x0400008D RID: 141
		private bool clearbtnIsEnable;

		// Token: 0x0400008E RID: 142
		private readonly string wordExtention = ".docx;.doc";

		// Token: 0x0400008F RID: 143
		private readonly string excelExtention = ".xlsx;.xls";

		// Token: 0x04000090 RID: 144
		private readonly string pptxExtention = ".pptx;.ppt";

		// Token: 0x04000091 RID: 145
		private readonly string imageExtention = ".bmp;.ico;.jpeg;.jpg;.png";

		// Token: 0x04000092 RID: 146
		private readonly string rtfExtention = ".rtf";

		// Token: 0x04000093 RID: 147
		private readonly string txtExtention = ".txt";

		// Token: 0x04000094 RID: 148
		private UserInfoModel userInfoModel = UserStore.GetUserInfoModel();

		// Token: 0x04000095 RID: 149
		private List<string> menus = new List<string>();

		// Token: 0x04000096 RID: 150
		private List<OpenHistoryModel> selectedModels = new List<OpenHistoryModel>();

		// Token: 0x04000097 RID: 151
		private RelayCommand<object> compressPDF;

		// Token: 0x04000098 RID: 152
		private RelayCommand<object> pdftoWord;

		// Token: 0x04000099 RID: 153
		private RelayCommand<object> pdftoExcel;

		// Token: 0x0400009A RID: 154
		private RelayCommand<object> pdftoPng;

		// Token: 0x0400009B RID: 155
		private RelayCommand<object> pdftoRtf;

		// Token: 0x0400009C RID: 156
		private RelayCommand<object> pdftoTxt;

		// Token: 0x0400009D RID: 157
		private RelayCommand<object> pdftoWeb;

		// Token: 0x0400009E RID: 158
		private RelayCommand<object> pdftoXML;

		// Token: 0x0400009F RID: 159
		private RelayCommand<object> pdftoJPG;

		// Token: 0x040000A0 RID: 160
		private RelayCommand<object> pdftoPPT;

		// Token: 0x040000A1 RID: 161
		private RelayCommand<object> splitPDF;

		// Token: 0x040000A2 RID: 162
		private RelayCommand<object> mergePDF;

		// Token: 0x040000A3 RID: 163
		private RelayCommand<object> wordtoPDF;

		// Token: 0x040000A4 RID: 164
		private RelayCommand<object> exceltoPDF;

		// Token: 0x040000A5 RID: 165
		private RelayCommand<object> ppttoPDF;

		// Token: 0x040000A6 RID: 166
		private RelayCommand<object> imgtoPDF;

		// Token: 0x040000A7 RID: 167
		private RelayCommand<object> rtftoPDF;

		// Token: 0x040000A8 RID: 168
		private RelayCommand<object> txttoPDF;

		// Token: 0x040000A9 RID: 169
		private RelayCommand<object> xmltoPDF;

		// Token: 0x040000AA RID: 170
		private RelayCommand<object> webtoPDF;

		// Token: 0x040000AB RID: 171
		private RelayCommand<object> covtoPDF;

		// Token: 0x040000AC RID: 172
		private RelayCommand<object> runEditor;

		// Token: 0x040000AD RID: 173
		private RelayCommand<object> removeOne;

		// Token: 0x040000AE RID: 174
		private RelayCommand<object> openFolder;

		// Token: 0x040000AF RID: 175
		private RelayCommand clearSelectModel;

		// Token: 0x040000B0 RID: 176
		private RelayCommand openOneFileCMD;

		// Token: 0x040000B1 RID: 177
		private RelayCommand fillFormCMD;

		// Token: 0x040000B2 RID: 178
		private RelayCommand createFileCMD;

		// Token: 0x040000B3 RID: 179
		private RelayCommand createFileFromScannerCMD;

		// Token: 0x040000B4 RID: 180
		private RelayCommand upgradeCMD;
	}
}
