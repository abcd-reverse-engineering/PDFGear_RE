using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using pdfconverter.Models;
using pdfconverter.Properties;
using pdfconverter.Utils;
using pdfconverter.Views;

namespace pdfconverter.ViewModels
{
	// Token: 0x02000030 RID: 48
	public class ImageToPDFUCViewModel : ObservableObject
	{
		// Token: 0x06000300 RID: 768 RVA: 0x0000C648 File Offset: 0x0000A848
		public ImageToPDFUCViewModel()
		{
			this.InitializeEnv();
		}

		// Token: 0x06000301 RID: 769 RVA: 0x0000C688 File Offset: 0x0000A888
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
			if (App.convertType.Equals(ConvToPDFType.ImageToPDF) && App.selectedFile != null && App.selectedFile.Length != 0)
			{
				foreach (string text2 in App.selectedFile)
				{
					this.AddOneFileToFileList(text2);
				}
			}
			if (this.pageSizeItems.Count == 0)
			{
				this.pageSizeItems.Add(new PageSizeItem(Resources.MainWinImageToPDFPageSizeMatchSource, pdfconverter.Models.PDFPageSize.MatchSource));
				this.pageSizeItems.Add(new PageSizeItem(Resources.MainWinImageToPDFPageSizeA4Portrait, pdfconverter.Models.PDFPageSize.A4_Portrait));
				this.pageSizeItems.Add(new PageSizeItem(Resources.MainWinImageToPDFPageSizeA4Landscape, pdfconverter.Models.PDFPageSize.A4_Landscape));
				this.pageSizeItems.Add(new PageSizeItem(Resources.MainWinImageToPDFPageSizeA3Portrait, pdfconverter.Models.PDFPageSize.A3_Portrait));
				this.pageSizeItems.Add(new PageSizeItem(Resources.MainWinImageToPDFPageSizeA3Landscape, pdfconverter.Models.PDFPageSize.A3_Landscape));
			}
			if (this.contentMargins.Count == 0)
			{
				this.contentMargins.Add(new PageMarginItem(Resources.MainWinImageToPDFPageMarginsNoMargin, pdfconverter.Models.ContentMargin.NoMargin));
				this.contentMargins.Add(new PageMarginItem(Resources.MainWinImageToPDFPageMarginsSmallMargin, pdfconverter.Models.ContentMargin.SmallMargin));
				this.contentMargins.Add(new PageMarginItem(Resources.MainWinImageToPDFPageMarginsBigMargin, pdfconverter.Models.ContentMargin.BigMargin));
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000302 RID: 770 RVA: 0x0000C7FC File Offset: 0x0000A9FC
		public List<PageSizeItem> PageSizeItems
		{
			get
			{
				return this.pageSizeItems;
			}
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000303 RID: 771 RVA: 0x0000C804 File Offset: 0x0000AA04
		public List<PageMarginItem> PageMarginItems
		{
			get
			{
				return this.contentMargins;
			}
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000304 RID: 772 RVA: 0x0000C80C File Offset: 0x0000AA0C
		public ToPDFFileItemCollection FileList
		{
			get
			{
				ToPDFFileItemCollection toPDFFileItemCollection;
				if ((toPDFFileItemCollection = this._toPDFItemList) == null)
				{
					toPDFFileItemCollection = (this._toPDFItemList = new ToPDFFileItemCollection());
				}
				return toPDFFileItemCollection;
			}
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x06000305 RID: 773 RVA: 0x0000C831 File Offset: 0x0000AA31
		// (set) Token: 0x06000306 RID: 774 RVA: 0x0000C839 File Offset: 0x0000AA39
		public ToPDFFileItem SelectedItem
		{
			get
			{
				return this._selectedItem;
			}
			set
			{
				base.SetProperty<ToPDFFileItem>(ref this._selectedItem, value, "SelectedItem");
			}
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0000C850 File Offset: 0x0000AA50
		private bool IsFileHasBeenAdded(string fileName)
		{
			return !string.IsNullOrWhiteSpace(fileName) && this.FileList != null && this.FileList.Where((ToPDFFileItem f) => f.FilePath.Equals(fileName)).ToList<ToPDFFileItem>().Count<ToPDFFileItem>() > 0;
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0000C8A8 File Offset: 0x0000AAA8
		public int AddOneFileToFileList(string fileName)
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
				bool? flag = ConToPDFUtils.CheckAccess(longPathFile);
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
				ToPDFFileItem toPDFFileItem = new ToPDFFileItem(longPathFile, ConvToPDFType.ImageToPDF);
				if (toPDFFileItem != null)
				{
					if (UtilsManager.IsnotSupportFile(longPathFile, UtilManager.ImageExtentions))
					{
						toPDFFileItem.Status = ToPDFItemStatus.Unsupport;
						toPDFFileItem.IsEnable = new bool?(false);
						toPDFFileItem.IsFileSelected = new bool?(false);
					}
					this.FileList.Add(toPDFFileItem);
					toPDFFileItem.ParseFile();
					if (this.FileList.Count == 1)
					{
						string text = Path.GetFileNameWithoutExtension(toPDFFileItem.FilePath);
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

		// Token: 0x06000309 RID: 777 RVA: 0x0000CA34 File Offset: 0x0000AC34
		public void RemoveOneToPDFFileItem(ToPDFFileItem item)
		{
			try
			{
				if (item != null && this.FileList != null)
				{
					if (this.FileList.Contains(item))
					{
						this.FileList.Remove(item);
					}
					this.NotifyAllMergeFilesSelectedChanged();
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600030A RID: 778 RVA: 0x0000CA84 File Offset: 0x0000AC84
		public void MoveUpOneToPDFFileItem(ToPDFFileItem item)
		{
			try
			{
				if (item != null && this.FileList != null)
				{
					if (this.FileList.Contains(item))
					{
						this.SelectedItem = item;
						int num = this.FileList.IndexOf(item);
						int num2 = num - 1;
						if (num >= 0 && num2 >= 0)
						{
							this.FileList.Move(num, num2);
							base.OnPropertyChanged("FileList");
						}
					}
					this.NotifyAllMergeFilesSelectedChanged();
				}
			}
			catch
			{
			}
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x0600030B RID: 779 RVA: 0x0000CB00 File Offset: 0x0000AD00
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

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x0600030C RID: 780 RVA: 0x0000CB34 File Offset: 0x0000AD34
		public RelayCommand AddOneFile
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.addOneFile) == null)
				{
					relayCommand = (this.addOneFile = new RelayCommand(delegate
					{
						string[] array = ConvertManager.selectMultiFiles("Image Format", UtilManager.ImageExtention);
						if (array != null && array.Length != 0)
						{
							foreach (string text in array)
							{
								this.AddOneFileToFileList(text);
							}
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x0600030D RID: 781 RVA: 0x0000CB68 File Offset: 0x0000AD68
		public RelayCommand ClearFiles
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.clearFiles) == null)
				{
					relayCommand = (this.clearFiles = new RelayCommand(delegate
					{
						if (this.FileList.Count((ToPDFFileItem f) => f.IsFileSelected.GetValueOrDefault()) == 0)
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

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x0600030E RID: 782 RVA: 0x0000CB9C File Offset: 0x0000AD9C
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

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x0600030F RID: 783 RVA: 0x0000CBD0 File Offset: 0x0000ADD0
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

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000310 RID: 784 RVA: 0x0000CC04 File Offset: 0x0000AE04
		public RelayCommand<ToPDFFileItem> OpenInExplorer
		{
			get
			{
				RelayCommand<ToPDFFileItem> relayCommand;
				if ((relayCommand = this.openInExplorer) == null)
				{
					relayCommand = (this.openInExplorer = new RelayCommand<ToPDFFileItem>(delegate(ToPDFFileItem model)
					{
						string pdffilePath = this.GetPDFFilePath(this.OutputPath, model.FilePath, this._OutputInOneFile.Value);
						if (LongPathFile.Exists(pdffilePath))
						{
							UtilsManager.OpenFileInExplore(pdffilePath, true);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000311 RID: 785 RVA: 0x0000CC38 File Offset: 0x0000AE38
		public RelayCommand OpenOneFileInExplorer
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.openOneFileInExplorer) == null)
				{
					relayCommand = (this.openOneFileInExplorer = new RelayCommand(delegate
					{
						string text = Path.Combine(this.OutputPath, this._MergeFileName);
						if (LongPathFile.Exists(text))
						{
							UtilsManager.OpenFileInExplore(text, true);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000312 RID: 786 RVA: 0x0000CC6C File Offset: 0x0000AE6C
		public RelayCommand<ToPDFFileItem> OpenWithEditor
		{
			get
			{
				RelayCommand<ToPDFFileItem> relayCommand;
				if ((relayCommand = this.openWithEditor) == null)
				{
					relayCommand = (this.openWithEditor = new RelayCommand<ToPDFFileItem>(delegate(ToPDFFileItem model)
					{
						string pdffilePath = this.GetPDFFilePath(this.OutputPath, model.FilePath, this._OutputInOneFile.Value);
						if (LongPathFile.Exists(pdffilePath))
						{
							UtilsManager.OpenFile(pdffilePath);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000313 RID: 787 RVA: 0x0000CCA0 File Offset: 0x0000AEA0
		public RelayCommand OpenWithOneFileEditor
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.openWithOneFileEditor) == null)
				{
					relayCommand = (this.openWithOneFileEditor = new RelayCommand(delegate
					{
						string text = Path.Combine(this.OutputPath, this._MergeFileName);
						if (LongPathFile.Exists(text))
						{
							UtilsManager.OpenFile(text);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000314 RID: 788 RVA: 0x0000CCD4 File Offset: 0x0000AED4
		public RelayCommand<ToPDFFileItem> RemoveFromList
		{
			get
			{
				RelayCommand<ToPDFFileItem> relayCommand;
				if ((relayCommand = this.removeFromList) == null)
				{
					relayCommand = (this.removeFromList = new RelayCommand<ToPDFFileItem>(delegate(ToPDFFileItem model)
					{
						this.RemoveOneToPDFFileItem(model);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000315 RID: 789 RVA: 0x0000CD08 File Offset: 0x0000AF08
		public RelayCommand<ToPDFFileItem> MoveUpFile
		{
			get
			{
				RelayCommand<ToPDFFileItem> relayCommand;
				if ((relayCommand = this.moveUpOneFile) == null)
				{
					relayCommand = (this.moveUpOneFile = new RelayCommand<ToPDFFileItem>(delegate(ToPDFFileItem model)
					{
						this.MoveUpOneToPDFFileItem(model);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000316 RID: 790 RVA: 0x0000CD3C File Offset: 0x0000AF3C
		public RelayCommand<ToPDFFileItem> MoveDownFile
		{
			get
			{
				RelayCommand<ToPDFFileItem> relayCommand;
				if ((relayCommand = this.moveDownFile) == null)
				{
					relayCommand = (this.moveDownFile = new RelayCommand<ToPDFFileItem>(delegate(ToPDFFileItem model)
					{
						this.MoveDownOneToPDFFileItem(model);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0000CD70 File Offset: 0x0000AF70
		public void MoveDownOneToPDFFileItem(ToPDFFileItem item)
		{
			try
			{
				if (item != null && this.FileList != null)
				{
					if (this.FileList.Contains(item))
					{
						this.SelectedItem = item;
						int num = this.FileList.IndexOf(item);
						int num2 = num + 1;
						if (num >= 0 && num2 < this.FileList.Count)
						{
							this.FileList.Move(num, num2);
							base.OnPropertyChanged("FileList");
						}
					}
					this.NotifyAllMergeFilesSelectedChanged();
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0000CDF8 File Offset: 0x0000AFF8
		public void ClearAllSelectedItems()
		{
			try
			{
				if (this.FileList != null)
				{
					foreach (ToPDFFileItem toPDFFileItem in this.FileList.Where((ToPDFFileItem f) => f.IsFileSelected.GetValueOrDefault()).ToList<ToPDFFileItem>())
					{
						this.FileList.Remove(toPDFFileItem);
					}
					this.NotifyAllMergeFilesSelectedChanged();
				}
			}
			catch
			{
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000319 RID: 793 RVA: 0x0000CE9C File Offset: 0x0000B09C
		// (set) Token: 0x0600031A RID: 794 RVA: 0x0000CF1C File Offset: 0x0000B11C
		public bool? IsAllMergeFilesSelected
		{
			get
			{
				if (this.FileList == null || this.FileList.Count<ToPDFFileItem>() <= 0)
				{
					return new bool?(false);
				}
				int num = this.FileList.Count<ToPDFFileItem>();
				int num2 = this.FileList.Count((ToPDFFileItem f) => f.IsFileSelected.GetValueOrDefault());
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

		// Token: 0x0600031B RID: 795 RVA: 0x0000CF4C File Offset: 0x0000B14C
		private void SelectAllMergeFiles(bool bSelectAll)
		{
			if (this.FileList == null || this.FileList.Count<ToPDFFileItem>() <= 0)
			{
				return;
			}
			foreach (ToPDFFileItem toPDFFileItem in this.FileList)
			{
				if (toPDFFileItem.Status != ToPDFItemStatus.Unsupport && toPDFFileItem.Status != ToPDFItemStatus.LoadedFailed)
				{
					bool? isFileSelected = toPDFFileItem.IsFileSelected;
					if (!((isFileSelected.GetValueOrDefault() == bSelectAll) & (isFileSelected != null)))
					{
						toPDFFileItem.IsFileSelected = new bool?(bSelectAll);
					}
				}
			}
			this.NotifyAllMergeFilesSelectedChanged();
		}

		// Token: 0x0600031C RID: 796 RVA: 0x0000CFEC File Offset: 0x0000B1EC
		public void NotifyAllMergeFilesSelectedChanged()
		{
			base.OnPropertyChanged("IsAllMergeFilesSelected");
			base.OnPropertyChanged("FileList");
			this.UIStatus = WorkQueenState.Init;
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600031D RID: 797 RVA: 0x0000D00B File Offset: 0x0000B20B
		// (set) Token: 0x0600031E RID: 798 RVA: 0x0000D013 File Offset: 0x0000B213
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

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x0600031F RID: 799 RVA: 0x0000D028 File Offset: 0x0000B228
		// (set) Token: 0x06000320 RID: 800 RVA: 0x0000D030 File Offset: 0x0000B230
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

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06000321 RID: 801 RVA: 0x0000D04C File Offset: 0x0000B24C
		// (set) Token: 0x06000322 RID: 802 RVA: 0x0000D054 File Offset: 0x0000B254
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

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000323 RID: 803 RVA: 0x0000D069 File Offset: 0x0000B269
		// (set) Token: 0x06000324 RID: 804 RVA: 0x0000D071 File Offset: 0x0000B271
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

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000325 RID: 805 RVA: 0x0000D092 File Offset: 0x0000B292
		// (set) Token: 0x06000326 RID: 806 RVA: 0x0000D09C File Offset: 0x0000B29C
		public bool? OurputInOneFile
		{
			get
			{
				return this._OutputInOneFile;
			}
			set
			{
				this.UIStatus = WorkQueenState.Init;
				if (value.GetValueOrDefault())
				{
					foreach (ToPDFFileItem toPDFFileItem in this.FileList)
					{
						toPDFFileItem.Status = ToPDFItemStatus.Loaded;
					}
				}
				base.SetProperty<bool?>(ref this._OutputInOneFile, value, "OurputInOneFile");
			}
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000327 RID: 807 RVA: 0x0000D10C File Offset: 0x0000B30C
		// (set) Token: 0x06000328 RID: 808 RVA: 0x0000D114 File Offset: 0x0000B314
		public PageSizeItem PDFPageSize
		{
			get
			{
				return this.pageSize;
			}
			set
			{
				base.SetProperty<PageSizeItem>(ref this.pageSize, value, "PDFPageSize");
			}
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x06000329 RID: 809 RVA: 0x0000D129 File Offset: 0x0000B329
		// (set) Token: 0x0600032A RID: 810 RVA: 0x0000D131 File Offset: 0x0000B331
		public PageMarginItem ContentMargin
		{
			get
			{
				return this.contentMargin;
			}
			set
			{
				base.SetProperty<PageMarginItem>(ref this.contentMargin, value, "ContentMargin");
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x0600032B RID: 811 RVA: 0x0000D146 File Offset: 0x0000B346
		// (set) Token: 0x0600032C RID: 812 RVA: 0x0000D14E File Offset: 0x0000B34E
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

		// Token: 0x0600032D RID: 813 RVA: 0x0000D178 File Offset: 0x0000B378
		public async void ProcessingFiles()
		{
			ImageToPDFUCViewModel.<>c__DisplayClass95_0 CS$<>8__locals1 = new ImageToPDFUCViewModel.<>c__DisplayClass95_0();
			CS$<>8__locals1.<>4__this = this;
			GAManager.SendEvent("ImageToPDF", "BtnClick", "Count", 1L);
			if (this.FileList.Where((ToPDFFileItem f) => f.IsFileSelected.GetValueOrDefault()).Count<ToPDFFileItem>() <= 0)
			{
				ModernMessageBox.Show(Resources.WinConvertAddFileTipText, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else if (string.IsNullOrWhiteSpace(this.OutputPath))
			{
				ModernMessageBox.Show(Resources.WinConvertSetOutputFolderText, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else
			{
				UtilsManager.getPDFFileName(this.OutputPath, this.FileList.Where((ToPDFFileItem f) => f.IsFileSelected.GetValueOrDefault()).First<ToPDFFileItem>().FilePath);
				CS$<>8__locals1.flist = this.FileList.Where((ToPDFFileItem f) => f.IsFileSelected.GetValueOrDefault()).ToList<ToPDFFileItem>();
				foreach (ToPDFFileItem toPDFFileItem in CS$<>8__locals1.flist)
				{
					toPDFFileItem.Status = ToPDFItemStatus.Working;
				}
				CS$<>8__locals1.mergeRet = false;
				await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
				{
					ImageToPDFUCViewModel.<>c__DisplayClass95_0.<<ProcessingFiles>b__3>d <<ProcessingFiles>b__3>d;
					<<ProcessingFiles>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<ProcessingFiles>b__3>d.<>4__this = CS$<>8__locals1;
					<<ProcessingFiles>b__3>d.<>1__state = -1;
					<<ProcessingFiles>b__3>d.<>t__builder.Start<ImageToPDFUCViewModel.<>c__DisplayClass95_0.<<ProcessingFiles>b__3>d>(ref <<ProcessingFiles>b__3>d);
					return <<ProcessingFiles>b__3>d.<>t__builder.Task;
				})).ConfigureAwait(false);
				this.SelectAllMergeFiles(false);
				this.UIStatus = (CS$<>8__locals1.mergeRet ? WorkQueenState.Succ : WorkQueenState.Fail);
			}
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0000D1B0 File Offset: 0x0000B3B0
		private string GetValidOutFileName(List<ToPDFFileItem> list, string parentFolder, ToPDFFileItem item)
		{
			if (!string.IsNullOrWhiteSpace(item.FilePath))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item.FilePath);
				string text = fileNameWithoutExtension;
				for (int i = 2; i < 100; i++)
				{
					string fname_full = Path.Combine(parentFolder, text + ".pdf");
					if (list.Count((ToPDFFileItem f) => f.OutputPath == fname_full) == 0)
					{
						return fname_full;
					}
					text = string.Format("{0}_{1:0}", fileNameWithoutExtension, i);
				}
			}
			return "";
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0000D234 File Offset: 0x0000B434
		private string GetMergeFileName(string outFolder, string source)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(source);
			int num = 0;
			string text = Path.Combine(outFolder, fileNameWithoutExtension + ".pdf");
			while (File.Exists(text))
			{
				text = Path.Combine(outFolder, string.Format("{0}_{1}.pdf", fileNameWithoutExtension, num));
				num++;
			}
			return text;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0000D284 File Offset: 0x0000B484
		private string GetValidOutFolder(string root, string firstFileName)
		{
			string text = Path.GetFileNameWithoutExtension(firstFileName).Trim();
			if (string.IsNullOrWhiteSpace(text))
			{
				text = "PDF Files";
			}
			string text2 = Path.Combine(root, text);
			int num = 1;
			while (Directory.Exists(text2))
			{
				text2 = Path.Combine(root, text + " " + num.ToString());
				num++;
			}
			return text2;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0000D2DD File Offset: 0x0000B4DD
		private string GetPDFFilePath(string outFolder, string FileName, bool IsOutputInOneFile)
		{
			if (!IsOutputInOneFile)
			{
				return Path.Combine(outFolder, this.OutputFilename, Path.GetFileNameWithoutExtension(FileName) + ".pdf");
			}
			return Path.Combine(outFolder, this._OutputFileFullName);
		}

		// Token: 0x040001AD RID: 429
		private ToPDFFileItemCollection _toPDFItemList;

		// Token: 0x040001AE RID: 430
		private List<PageSizeItem> pageSizeItems = new List<PageSizeItem>();

		// Token: 0x040001AF RID: 431
		private List<PageMarginItem> contentMargins = new List<PageMarginItem>();

		// Token: 0x040001B0 RID: 432
		private ToPDFFileItem _selectedItem;

		// Token: 0x040001B1 RID: 433
		private string _OutputPath;

		// Token: 0x040001B2 RID: 434
		private string _OutputFilename;

		// Token: 0x040001B3 RID: 435
		private string _OutputFileFullName;

		// Token: 0x040001B4 RID: 436
		private string _MergeFileName;

		// Token: 0x040001B5 RID: 437
		private bool? _ViewFileInExplore = new bool?(ConfigManager.GetConvertViewFileInExplore());

		// Token: 0x040001B6 RID: 438
		private bool? _OutputInOneFile = new bool?(true);

		// Token: 0x040001B7 RID: 439
		private PageSizeItem pageSize;

		// Token: 0x040001B8 RID: 440
		private PageMarginItem contentMargin;

		// Token: 0x040001B9 RID: 441
		private WorkQueenState _UIStatus;

		// Token: 0x040001BA RID: 442
		private RelayCommand addOneFile;

		// Token: 0x040001BB RID: 443
		private RelayCommand clearFiles;

		// Token: 0x040001BC RID: 444
		private RelayCommand selectPath;

		// Token: 0x040001BD RID: 445
		private RelayCommand beginWorks;

		// Token: 0x040001BE RID: 446
		private RelayCommand updateItem;

		// Token: 0x040001BF RID: 447
		private RelayCommand<ToPDFFileItem> moveUpOneFile;

		// Token: 0x040001C0 RID: 448
		private RelayCommand<ToPDFFileItem> moveDownFile;

		// Token: 0x040001C1 RID: 449
		private RelayCommand<ToPDFFileItem> openInExplorer;

		// Token: 0x040001C2 RID: 450
		private RelayCommand<ToPDFFileItem> openWithEditor;

		// Token: 0x040001C3 RID: 451
		private RelayCommand openOneFileInExplorer;

		// Token: 0x040001C4 RID: 452
		private RelayCommand openWithOneFileEditor;

		// Token: 0x040001C5 RID: 453
		private RelayCommand<ToPDFFileItem> removeFromList;
	}
}
