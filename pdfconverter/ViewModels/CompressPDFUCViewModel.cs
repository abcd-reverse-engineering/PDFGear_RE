using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using CommonLib.IAP;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using pdfconverter.Models;
using pdfconverter.Properties;
using pdfconverter.Utils;
using pdfconverter.Views;

namespace pdfconverter.ViewModels
{
	// Token: 0x0200002E RID: 46
	public class CompressPDFUCViewModel : ObservableObject
	{
		// Token: 0x06000291 RID: 657 RVA: 0x0000AA6E File Offset: 0x00008C6E
		public CompressPDFUCViewModel()
		{
			this.InitializeEnv();
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000AA9C File Offset: 0x00008C9C
		private void InitializeEnv()
		{
			string text = string.Empty;
			try
			{
				text = ConfigManager.GetConvertPath();
				if (string.IsNullOrEmpty(text) || !Directory.Exists(text))
				{
					text = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\PDFgear";
				}
			}
			catch
			{
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.OutputPath = text;
			}
			CompressMode? defaultMode = this.GetDefaultMode();
			if (defaultMode != null)
			{
				if (defaultMode.Value == CompressMode.Low)
				{
					this.IsSetAllLowCompress = true;
					this.SetLowCompress.Execute(null);
				}
				if (defaultMode.Value == CompressMode.Medium)
				{
					this.IsSetAllMidCompress = true;
					this.SetMidCompress.Execute(null);
				}
				if (defaultMode.Value == CompressMode.High)
				{
					this.IsSetAllHighCompress = true;
					this.SetHighCompress.Execute(null);
				}
			}
		}

		// Token: 0x06000293 RID: 659 RVA: 0x0000AB6C File Offset: 0x00008D6C
		private CompressMode? GetDefaultMode()
		{
			try
			{
				string text = Path.Combine(AppDataHelper.LocalCacheFolder, "TmpSetting", "compress");
				if (File.Exists(text))
				{
					string text2 = File.ReadAllText(text);
					File.Delete(text2);
					int num;
					if (int.TryParse(text2, out num))
					{
						return new CompressMode?((CompressMode)num);
					}
				}
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x06000294 RID: 660 RVA: 0x0000ABD4 File Offset: 0x00008DD4
		// (set) Token: 0x06000295 RID: 661 RVA: 0x0000ABDC File Offset: 0x00008DDC
		public bool IsSetAllLowCompress
		{
			get
			{
				return this.isSetAllLowCompress;
			}
			set
			{
				base.SetProperty<bool>(ref this.isSetAllLowCompress, value, "IsSetAllLowCompress");
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000296 RID: 662 RVA: 0x0000ABF1 File Offset: 0x00008DF1
		// (set) Token: 0x06000297 RID: 663 RVA: 0x0000ABF9 File Offset: 0x00008DF9
		public bool IsSetAllMidCompress
		{
			get
			{
				return this.isSetAllMidCompress;
			}
			set
			{
				base.SetProperty<bool>(ref this.isSetAllMidCompress, value, "IsSetAllMidCompress");
			}
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000298 RID: 664 RVA: 0x0000AC0E File Offset: 0x00008E0E
		// (set) Token: 0x06000299 RID: 665 RVA: 0x0000AC16 File Offset: 0x00008E16
		public bool IsSetAllHighCompress
		{
			get
			{
				return this.isSetAllHighCompress;
			}
			set
			{
				base.SetProperty<bool>(ref this.isSetAllHighCompress, value, "IsSetAllHighCompress");
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x0600029A RID: 666 RVA: 0x0000AC2B File Offset: 0x00008E2B
		// (set) Token: 0x0600029B RID: 667 RVA: 0x0000AC33 File Offset: 0x00008E33
		public CompressMode CompressMode
		{
			get
			{
				return this.compressMode;
			}
			set
			{
				base.SetProperty<CompressMode>(ref this.compressMode, value, "CompressMode");
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x0600029C RID: 668 RVA: 0x0000AC48 File Offset: 0x00008E48
		// (set) Token: 0x0600029D RID: 669 RVA: 0x0000AC50 File Offset: 0x00008E50
		public string OutputFilename
		{
			get
			{
				return this._OutputFilename;
			}
			set
			{
				base.SetProperty<string>(ref this._OutputFilename, value, "OutputFilename");
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x0600029E RID: 670 RVA: 0x0000AC65 File Offset: 0x00008E65
		// (set) Token: 0x0600029F RID: 671 RVA: 0x0000AC6D File Offset: 0x00008E6D
		public string OutputFileFullName
		{
			get
			{
				return this._OutputFileFullName;
			}
			set
			{
				base.SetProperty<string>(ref this._OutputFileFullName, value, "OutputFileFullName");
			}
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060002A0 RID: 672 RVA: 0x0000AC82 File Offset: 0x00008E82
		// (set) Token: 0x060002A1 RID: 673 RVA: 0x0000AC8A File Offset: 0x00008E8A
		public string OutputPath
		{
			get
			{
				return this._OutputPath;
			}
			set
			{
				this.UIStatus = WorkQueenState.Init;
				base.SetProperty<string>(ref this._OutputPath, value, "OutputPath");
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060002A2 RID: 674 RVA: 0x0000ACA6 File Offset: 0x00008EA6
		// (set) Token: 0x060002A3 RID: 675 RVA: 0x0000ACAE File Offset: 0x00008EAE
		public WorkQueenState UIStatus
		{
			get
			{
				return this._UIStatus;
			}
			set
			{
				base.SetProperty<WorkQueenState>(ref this._UIStatus, value, "UIStatus");
				if (this._UIStatus == WorkQueenState.Init)
				{
					this.OutputFileFullName = "";
				}
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060002A4 RID: 676 RVA: 0x0000ACD6 File Offset: 0x00008ED6
		// (set) Token: 0x060002A5 RID: 677 RVA: 0x0000ACDE File Offset: 0x00008EDE
		public bool? ViewFileInExplore
		{
			get
			{
				return this._ViewFileInExplore;
			}
			set
			{
				base.SetProperty<bool?>(ref this._ViewFileInExplore, value, "ViewFileInExplore");
				ConfigManager.SetConvertViewFileInExplore(value.Value);
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060002A6 RID: 678 RVA: 0x0000AD00 File Offset: 0x00008F00
		// (set) Token: 0x060002A7 RID: 679 RVA: 0x0000AD80 File Offset: 0x00008F80
		public bool? IsAllMergeFilesSelected
		{
			get
			{
				if (this.FileList == null || this.FileList.Count<CompressItem>() <= 0)
				{
					return new bool?(false);
				}
				int num = this.FileList.Count<CompressItem>();
				int num2 = this.FileList.Count((CompressItem f) => f.IsFileSelected.GetValueOrDefault());
				if (num2 <= 0)
				{
					return new bool?(false);
				}
				if (num == num2)
				{
					return new bool?(true);
				}
				return null;
			}
			set
			{
				bool? flag = value;
				if (flag == null)
				{
					flag = new bool?(false);
				}
				this.SelectAllMergeFiles(flag.Value);
			}
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0000ADB0 File Offset: 0x00008FB0
		public void SelectModeChanged()
		{
			if (!this.IsLock)
			{
				if ((from f in this.FileList
					group f by f.Compress_Mode).Count<IGrouping<int, CompressItem>>() == 1)
				{
					int compress_Mode = this.FileList.FirstOrDefault<CompressItem>().Compress_Mode;
					if (compress_Mode == 0)
					{
						this.IsSetAllHighCompress = true;
						this.IsSetAllMidCompress = false;
						this.IsSetAllLowCompress = false;
						return;
					}
					if (compress_Mode == 1)
					{
						this.IsSetAllHighCompress = false;
						this.IsSetAllMidCompress = true;
						this.IsSetAllLowCompress = false;
						return;
					}
					if (compress_Mode == 2)
					{
						this.IsSetAllHighCompress = false;
						this.IsSetAllMidCompress = false;
						this.IsSetAllLowCompress = true;
						return;
					}
				}
				else
				{
					this.IsSetAllHighCompress = false;
					this.IsSetAllMidCompress = false;
					this.IsSetAllLowCompress = false;
				}
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x060002A9 RID: 681 RVA: 0x0000AE70 File Offset: 0x00009070
		public RelayCommand BeginWorks
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.beginWorks) == null)
				{
					relayCommand = (this.beginWorks = new RelayCommand(delegate
					{
						this.ProcessingFiles();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x060002AA RID: 682 RVA: 0x0000AEA4 File Offset: 0x000090A4
		public CompressItemCollection FileList
		{
			get
			{
				CompressItemCollection compressItemCollection;
				if ((compressItemCollection = this.compressItemList) == null)
				{
					compressItemCollection = (this.compressItemList = new CompressItemCollection());
				}
				return compressItemCollection;
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x060002AB RID: 683 RVA: 0x0000AECC File Offset: 0x000090CC
		public RelayCommand<CompressItem> OpenInExplorer
		{
			get
			{
				RelayCommand<CompressItem> relayCommand;
				if ((relayCommand = this.openInExplorer) == null)
				{
					relayCommand = (this.openInExplorer = new RelayCommand<CompressItem>(delegate(CompressItem model)
					{
						string pdfresult = UtilsManager.getPDFResult(this.OutputPath, model.FilePath);
						if (LongPathFile.Exists(pdfresult))
						{
							UtilsManager.OpenFileInExplore(pdfresult, true);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x060002AC RID: 684 RVA: 0x0000AF00 File Offset: 0x00009100
		public RelayCommand SetLowCompress
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.setLowCompressMode) == null)
				{
					relayCommand = (this.setLowCompressMode = new RelayCommand(delegate
					{
						this.CompressMode = CompressMode.Low;
						this.NotifyCompressModeChanged();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x060002AD RID: 685 RVA: 0x0000AF34 File Offset: 0x00009134
		public RelayCommand SetMidCompress
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.setMidCompressMode) == null)
				{
					relayCommand = (this.setMidCompressMode = new RelayCommand(delegate
					{
						this.CompressMode = CompressMode.Medium;
						this.NotifyCompressModeChanged();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x060002AE RID: 686 RVA: 0x0000AF68 File Offset: 0x00009168
		public RelayCommand SetHighCompress
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.setHighCompressMode) == null)
				{
					relayCommand = (this.setHighCompressMode = new RelayCommand(delegate
					{
						this.CompressMode = CompressMode.High;
						this.NotifyCompressModeChanged();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x060002AF RID: 687 RVA: 0x0000AF99 File Offset: 0x00009199
		// (set) Token: 0x060002B0 RID: 688 RVA: 0x0000AFA1 File Offset: 0x000091A1
		public CompressItem SelectedItem
		{
			get
			{
				return this._selectedItem;
			}
			set
			{
				base.SetProperty<CompressItem>(ref this._selectedItem, value, "SelectedItem");
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060002B1 RID: 689 RVA: 0x0000AFB8 File Offset: 0x000091B8
		public RelayCommand SelectPath
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.selectPath) == null)
				{
					relayCommand = (this.selectPath = new RelayCommand(delegate
					{
						string text = ConvertManager.selectOutputFolder(this.OutputPath);
						if (!string.IsNullOrWhiteSpace(text))
						{
							this.OutputPath = text;
							ConfigManager.SetConvertPath(this.OutputPath);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x0000AFEC File Offset: 0x000091EC
		public RelayCommand UpdateItem
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.updateItem) == null)
				{
					relayCommand = (this.updateItem = new RelayCommand(delegate
					{
						this.NotifyAllMergeFilesSelectedChanged();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x060002B3 RID: 691 RVA: 0x0000B020 File Offset: 0x00009220
		public RelayCommand AddOneFile
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.addOneFile) == null)
				{
					relayCommand = (this.addOneFile = new RelayCommand(delegate
					{
						string[] array = ConvertManager.selectMultiFiles("Portable Document Format", ".pdf;*.pdf;");
						if (array != null && array.Length != 0)
						{
							foreach (string text in array)
							{
								this.AddOneFileToFileList(text, null);
							}
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x0000B054 File Offset: 0x00009254
		public RelayCommand<CompressItem> OpenWithEditor
		{
			get
			{
				RelayCommand<CompressItem> relayCommand;
				if ((relayCommand = this.openWithEditor) == null)
				{
					relayCommand = (this.openWithEditor = new RelayCommand<CompressItem>(delegate(CompressItem model)
					{
						string text = Path.Combine(this.OutputPath, model.FileName);
						if (!string.IsNullOrWhiteSpace(text) && LongPathFile.Exists(text))
						{
							UtilsManager.OpenFile(text);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x060002B5 RID: 693 RVA: 0x0000B088 File Offset: 0x00009288
		public RelayCommand<CompressItem> RemoveFromList
		{
			get
			{
				RelayCommand<CompressItem> relayCommand;
				if ((relayCommand = this.removeFromList) == null)
				{
					relayCommand = (this.removeFromList = new RelayCommand<CompressItem>(delegate(CompressItem model)
					{
						this.RemoveOneToPDFFileItem(model);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x060002B6 RID: 694 RVA: 0x0000B0BC File Offset: 0x000092BC
		public RelayCommand ClearFiles
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.clearFiles) == null)
				{
					relayCommand = (this.clearFiles = new RelayCommand(delegate
					{
						if (this.FileList.Count((CompressItem f) => f.IsFileSelected.GetValueOrDefault()) == 0)
						{
							ModernMessageBox.Show(Resources.MainWinOthersToPDFDeleteNoFile, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							return;
						}
						if (ModernMessageBox.Show(Resources.WinMergeSplitClearFileAskMsg, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.Yes)
						{
							this.ClearAllSelectedItems();
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x0000B0F0 File Offset: 0x000092F0
		public int AddOneFileToFileList(string fileName, string password = null)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(fileName) || this.FileList == null)
				{
					return 2;
				}
				LongPathFile longPathFile = fileName;
				fileName = longPathFile.FullPathWithoutPrefix;
				if (this.IsFileHasBeenAdded(longPathFile))
				{
					ModernMessageBox.Show(Resources.WinMergeSplitAddFileCheckMsg.Replace("XXX", fileName), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return 1;
				}
				bool? flag = ConToPDFUtils.CheckAccess(fileName);
				bool flag2 = false;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					flag = new FileOccupation(fileName).ShowDialog();
					flag2 = false;
					if ((flag.GetValueOrDefault() == flag2) & (flag != null))
					{
						return 0;
					}
				}
				if (password == null)
				{
					password = "";
				}
				CompressItem compressItem = new CompressItem(longPathFile, this.CompressMode, this);
				compressItem.Status = ToPDFItemStatus.Loaded;
				if (compressItem != null)
				{
					if (UtilsManager.IsnotSupportFile(longPathFile, UtilManager.PDFExtentions))
					{
						compressItem.Status = ToPDFItemStatus.Unsupport;
						compressItem.IsFileSelected = new bool?(false);
						compressItem.IsEnable = new bool?(false);
						this.FileList.Add(compressItem);
					}
					else if (ConToPDFUtils.CheckPassword(longPathFile, ref password, Application.Current.MainWindow))
					{
						compressItem.Password = password;
						this.FileList.Add(compressItem);
					}
					else
					{
						this.FileList.Add(compressItem);
						compressItem.IsFileSelected = new bool?(false);
						compressItem.IsEnable = new bool?(false);
						compressItem.Status = ToPDFItemStatus.LoadedFailed;
					}
					this.SelectModeChanged();
					if (this.FileList.Count == 1)
					{
						string text = Path.GetFileNameWithoutExtension(compressItem.FilePath);
						if (string.IsNullOrWhiteSpace(text))
						{
							text = "default";
						}
						text = UtilsManager.getValidFileName(this.OutputPath, text, ".pdf");
						if (!string.IsNullOrWhiteSpace(text))
						{
							this.OutputFilename = text;
						}
					}
				}
				this.NotifyAllMergeFilesSelectedChanged();
				return 0;
			}
			catch
			{
			}
			return 1;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0000B2E8 File Offset: 0x000094E8
		public void ClearAllSelectedItems()
		{
			try
			{
				if (this.FileList != null)
				{
					foreach (CompressItem compressItem in this.FileList.Where((CompressItem f) => f.IsFileSelected.GetValueOrDefault()).ToList<CompressItem>())
					{
						this.FileList.Remove(compressItem);
					}
					this.CompressMode = CompressMode.Medium;
					this.NotifyAllMergeFilesSelectedChanged();
				}
			}
			catch
			{
			}
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0000B394 File Offset: 0x00009594
		private void SelectAllMergeFiles(bool bSelectAll)
		{
			if (this.FileList == null || this.FileList.Count<CompressItem>() <= 0)
			{
				return;
			}
			foreach (CompressItem compressItem in this.FileList)
			{
				if (compressItem.Status != ToPDFItemStatus.Unsupport)
				{
					bool? isFileSelected = compressItem.IsFileSelected;
					if (!((isFileSelected.GetValueOrDefault() == bSelectAll) & (isFileSelected != null)) && compressItem.Status != ToPDFItemStatus.LoadedFailed)
					{
						compressItem.IsFileSelected = new bool?(bSelectAll);
					}
				}
			}
			this.NotifyAllMergeFilesSelectedChanged();
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000B434 File Offset: 0x00009634
		public void RemoveOneToPDFFileItem(CompressItem item)
		{
			try
			{
				if (item != null && this.FileList != null)
				{
					if (this.FileList.Contains(item))
					{
						this.FileList.Remove(item);
					}
					this.SelectModeChanged();
					this.NotifyAllMergeFilesSelectedChanged();
				}
			}
			catch
			{
			}
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0000B48C File Offset: 0x0000968C
		public void NotifyAllMergeFilesSelectedChanged()
		{
			base.OnPropertyChanged("IsAllMergeFilesSelected");
			base.OnPropertyChanged("FileList");
			if (this.UIStatus != WorkQueenState.Working)
			{
				this.UIStatus = WorkQueenState.Init;
			}
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0000B4B4 File Offset: 0x000096B4
		public void NotifyCompressModeChanged()
		{
			this.IsLock = true;
			foreach (CompressItem compressItem in this.FileList.Where((CompressItem f) => f.IsFileSelected.GetValueOrDefault()).ToList<CompressItem>())
			{
				compressItem.Compress_Mode = (int)this.CompressMode;
			}
			this.IsLock = false;
			if (this.UIStatus != WorkQueenState.Working)
			{
				this.UIStatus = WorkQueenState.Init;
			}
		}

		// Token: 0x060002BD RID: 701 RVA: 0x0000B554 File Offset: 0x00009754
		public async void ProcessingFiles()
		{
			GAManager.SendEvent("CompressPDF", "BtnClick", "Count", 1L);
			if (!IAPHelper.IsPaidUser && !ConfigManager.GetTest())
			{
				IAPHelper.ShowActivationWindow("Compress", "Compress");
			}
			else if (this.FileList.Count((CompressItem f) => f.IsFileSelected.GetValueOrDefault()) <= 0)
			{
				ModernMessageBox.Show(Resources.WinCompressAddFileTipText, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else if (string.IsNullOrWhiteSpace(this.OutputPath))
			{
				ModernMessageBox.Show(Resources.WinConvertSetOutputFolderText, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else
			{
				await Task.Run(TaskExceptionHelper.ExceptionBoundary(async delegate
				{
					CompressPDFUCViewModel.<>c__DisplayClass90_0 CS$<>8__locals1 = new CompressPDFUCViewModel.<>c__DisplayClass90_0();
					CS$<>8__locals1.<>4__this = this;
					this.UIStatus = WorkQueenState.Working;
					int selectCount = 0;
					CS$<>8__locals1.SuccCount = 0;
					List<Task> list = new List<Task>();
					foreach (CompressItem compressItem in this.FileList.Where((CompressItem f) => f.IsFileSelected.GetValueOrDefault()))
					{
						compressItem.Status = ToPDFItemStatus.Queuing;
					}
					using (IEnumerator<CompressItem> enumerator = this.FileList.Where((CompressItem f) => f.IsFileSelected.GetValueOrDefault()).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							CompressPDFUCViewModel.<>c__DisplayClass90_1 CS$<>8__locals2 = new CompressPDFUCViewModel.<>c__DisplayClass90_1();
							CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
							CS$<>8__locals2.it = enumerator.Current;
							int num = selectCount;
							selectCount = num + 1;
							if (list.Count((Task t) => t.Status != TaskStatus.RanToCompletion) >= 4)
							{
								Task.WaitAny(list.ToArray());
								list = list.Where((Task t) => t.Status != TaskStatus.RanToCompletion).ToList<Task>();
							}
							Task task = Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
							{
								CompressPDFUCViewModel.<>c__DisplayClass90_1.<<ProcessingFiles>b__6>d <<ProcessingFiles>b__6>d;
								<<ProcessingFiles>b__6>d.<>t__builder = AsyncTaskMethodBuilder.Create();
								<<ProcessingFiles>b__6>d.<>4__this = CS$<>8__locals2;
								<<ProcessingFiles>b__6>d.<>1__state = -1;
								<<ProcessingFiles>b__6>d.<>t__builder.Start<CompressPDFUCViewModel.<>c__DisplayClass90_1.<<ProcessingFiles>b__6>d>(ref <<ProcessingFiles>b__6>d);
								return <<ProcessingFiles>b__6>d.<>t__builder.Task;
							}));
							list.Add(task);
						}
					}
					await Task.WhenAll(list).ConfigureAwait(true);
					GC.Collect();
					if (this.ViewFileInExplore.GetValueOrDefault() && CS$<>8__locals1.SuccCount > 0)
					{
						UtilsManager.OpenFileInExplore(this.OutputFilename, true);
					}
					this.UIStatus = WorkQueenState.Succ;
					if (selectCount != CS$<>8__locals1.SuccCount)
					{
						GAManager.SendEvent("CompressPDF", "HasFailed", "Count", 1L);
						if (MessageBox.Show(Resources.FileCompressMsgCompressFailSupport, UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
						{
							Application.Current.Dispatcher.Invoke(delegate
							{
								FeedbackWindow feedbackWindow = new FeedbackWindow();
								if (feedbackWindow != null)
								{
									try
									{
										foreach (CompressItem compressItem2 in this.FileList)
										{
											if (compressItem2.IsFileSelected.GetValueOrDefault() && compressItem2.Status == ToPDFItemStatus.Fail)
											{
												feedbackWindow.flist.Add(compressItem2.FilePath);
											}
										}
									}
									catch
									{
									}
									feedbackWindow.source = "2";
									feedbackWindow.Owner = (MainWindow2)Application.Current.MainWindow;
									feedbackWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
									feedbackWindow.showAttachmentCB(true);
									feedbackWindow.ShowDialog();
								}
							});
						}
					}
					else
					{
						GAManager.SendEvent("CompressPDF", "AllSucc", "Count", 1L);
					}
				})).ConfigureAwait(false);
				this.SelectAllMergeFiles(false);
			}
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0000B58C File Offset: 0x0000978C
		private void CheckSucceedCount()
		{
			if (this.FileList.Count((CompressItem f) => f.Status != ToPDFItemStatus.Succ) == 0)
			{
				if (this.ViewFileInExplore.GetValueOrDefault())
				{
					UtilsManager.OpenFolderInExplore(this.OutputPath);
				}
				this.SelectAllMergeFiles(false);
			}
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0000B5E8 File Offset: 0x000097E8
		private bool IsFileHasBeenAdded(string fileName)
		{
			return !string.IsNullOrWhiteSpace(fileName) && this.FileList != null && this.FileList.Where((CompressItem f) => f.FilePath.Equals(fileName)).ToList<CompressItem>().Count<CompressItem>() > 0;
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x0000B640 File Offset: 0x00009840
		public string GetFileSize(string FileName)
		{
			LongPathFile longPathFile = FileName;
			if (!longPathFile.IsExists)
			{
				return "";
			}
			long length = longPathFile.FileInfo.Length;
			double num = 1024.0;
			if ((double)length < num)
			{
				return length.ToString() + "B";
			}
			if ((double)length < Math.Pow(num, 2.0))
			{
				return ((double)length / num).ToString("f2") + "K";
			}
			if ((double)length < Math.Pow(num, 3.0))
			{
				return ((double)length / Math.Pow(num, 2.0)).ToString("f2") + "M";
			}
			if ((double)length < Math.Pow(num, 4.0))
			{
				return ((double)length / Math.Pow(num, 3.0)).ToString("f2") + "G";
			}
			return ((double)length / Math.Pow(num, 4.0)).ToString("f2") + "T";
		}

		// Token: 0x04000184 RID: 388
		private RelayCommand updateItem;

		// Token: 0x04000185 RID: 389
		private RelayCommand addOneFile;

		// Token: 0x04000186 RID: 390
		private RelayCommand clearFiles;

		// Token: 0x04000187 RID: 391
		private RelayCommand selectPath;

		// Token: 0x04000188 RID: 392
		private RelayCommand setLowCompressMode;

		// Token: 0x04000189 RID: 393
		private RelayCommand setMidCompressMode;

		// Token: 0x0400018A RID: 394
		private RelayCommand setHighCompressMode;

		// Token: 0x0400018B RID: 395
		private RelayCommand beginWorks;

		// Token: 0x0400018C RID: 396
		private RelayCommand<CompressItem> openWithEditor;

		// Token: 0x0400018D RID: 397
		private RelayCommand<CompressItem> removeFromList;

		// Token: 0x0400018E RID: 398
		private RelayCommand<CompressItem> openInExplorer;

		// Token: 0x0400018F RID: 399
		private CompressItemCollection compressItemList;

		// Token: 0x04000190 RID: 400
		private CompressItem _selectedItem;

		// Token: 0x04000191 RID: 401
		private string _OutputPath;

		// Token: 0x04000192 RID: 402
		private string _OutputFilename;

		// Token: 0x04000193 RID: 403
		private string _OutputFileFullName;

		// Token: 0x04000194 RID: 404
		private bool? _ViewFileInExplore = new bool?(ConfigManager.GetConvertViewFileInExplore());

		// Token: 0x04000195 RID: 405
		private bool isSetAllLowCompress;

		// Token: 0x04000196 RID: 406
		private bool isSetAllMidCompress = true;

		// Token: 0x04000197 RID: 407
		private bool IsLock;

		// Token: 0x04000198 RID: 408
		private bool isSetAllHighCompress;

		// Token: 0x04000199 RID: 409
		private WorkQueenState _UIStatus;

		// Token: 0x0400019A RID: 410
		private CompressMode compressMode = CompressMode.Medium;
	}
}
