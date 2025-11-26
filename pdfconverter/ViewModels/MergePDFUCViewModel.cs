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
	// Token: 0x02000034 RID: 52
	public class MergePDFUCViewModel : ObservableObject
	{
		// Token: 0x060003AC RID: 940 RVA: 0x0000EBE3 File Offset: 0x0000CDE3
		public MergePDFUCViewModel()
		{
			this.MergeInitializeEnv();
		}

		// Token: 0x060003AD RID: 941 RVA: 0x0000EBF4 File Offset: 0x0000CDF4
		private void MergeInitializeEnv()
		{
			this._mergePDFList = new MergeFileItemCollection();
			this._mergeViewFileInExplore = new bool?(ConfigManager.GetConvertViewFileInExplore());
			this._UIStatus = WorkQueenState.Init;
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
				this.MergeOutputPath = text;
			}
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060003AE RID: 942 RVA: 0x0000EC80 File Offset: 0x0000CE80
		public MergeFileItemCollection MergePDFList
		{
			get
			{
				return this._mergePDFList;
			}
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060003AF RID: 943 RVA: 0x0000EC88 File Offset: 0x0000CE88
		// (set) Token: 0x060003B0 RID: 944 RVA: 0x0000EC90 File Offset: 0x0000CE90
		public MergeFileItem SelectedMergeFileItem
		{
			get
			{
				return this._selectedMergeFileItem;
			}
			set
			{
				base.SetProperty<MergeFileItem>(ref this._selectedMergeFileItem, value, "SelectedMergeFileItem");
			}
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0000ECA8 File Offset: 0x0000CEA8
		private bool IsMergeFileHasBeenAdded(string fileName)
		{
			return !string.IsNullOrWhiteSpace(fileName) && this.MergePDFList != null && this.MergePDFList.Where((MergeFileItem f) => f.FilePath.Equals(fileName)).ToList<MergeFileItem>().Count<MergeFileItem>() > 0;
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0000ED00 File Offset: 0x0000CF00
		public int AddOneFileToMergeList(string fileName, string password = null)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(fileName) || this.MergePDFList == null)
				{
					return 2;
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
				LongPathFile fileFullName = fileName;
				if (this.MergePDFList.Where((MergeFileItem f) => fileFullName.Equals(f.FilePath)).ToList<MergeFileItem>().Count > 0)
				{
					password = this.MergePDFList.FirstOrDefault((MergeFileItem c) => fileFullName.Equals(c.FilePath)).Passwrod;
				}
				MergeFileItem mergeFileItem = new MergeFileItem(fileFullName);
				if (mergeFileItem != null)
				{
					if (UtilsManager.IsnotSupportFile(fileFullName, UtilManager.PDFExtentions))
					{
						mergeFileItem.IsFileSelected = new bool?(false);
						mergeFileItem.Status = MergeStatus.Unsupport;
						this.MergePDFList.Add(mergeFileItem);
					}
					else if (ConToPDFUtils.CheckPassword(fileFullName, ref password, Application.Current.MainWindow))
					{
						this.MergePDFList.Add(mergeFileItem);
						mergeFileItem.parseFile(password);
					}
					else
					{
						this.MergePDFList.Add(mergeFileItem);
						mergeFileItem.IsFileSelected = new bool?(false);
						mergeFileItem.Status = MergeStatus.LoadedFailed;
					}
					mergeFileItem.Passwrod = password;
					if (this.MergePDFList.Count == 1)
					{
						string text = Path.GetFileNameWithoutExtension(mergeFileItem.FilePath);
						if (string.IsNullOrWhiteSpace(text))
						{
							text = "default";
						}
						text += "_merge";
						text = UtilsManager.getValidFileName(this.MergeOutputPath, text, ".pdf");
						if (!string.IsNullOrWhiteSpace(text))
						{
							this.MergeOutputFilename = text;
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

		// Token: 0x060003B3 RID: 947 RVA: 0x0000EF00 File Offset: 0x0000D100
		public void RemoveOneMergeFileItem(MergeFileItem item)
		{
			try
			{
				if (item != null && this.MergePDFList != null)
				{
					if (this.MergePDFList.Contains(item))
					{
						this.MergePDFList.Remove(item);
					}
					this.NotifyAllMergeFilesSelectedChanged();
				}
			}
			catch
			{
			}
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x0000EF50 File Offset: 0x0000D150
		public void MoveUpOneMergeFileItem(MergeFileItem item)
		{
			try
			{
				if (item != null && this.MergePDFList != null)
				{
					if (this.MergePDFList.Contains(item))
					{
						this.SelectedMergeFileItem = item;
						int num = this.MergePDFList.IndexOf(item);
						int num2 = num - 1;
						if (num >= 0 && num2 >= 0)
						{
							this.MergePDFList.Move(num, num2);
							base.OnPropertyChanged("MergePDFList");
						}
					}
					this.NotifyAllMergeFilesSelectedChanged();
				}
			}
			catch
			{
			}
		}

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060003B5 RID: 949 RVA: 0x0000EFCC File Offset: 0x0000D1CC
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

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x0000F000 File Offset: 0x0000D200
		public RelayCommand AddOneFile
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.addOneFile) == null)
				{
					relayCommand = (this.addOneFile = new RelayCommand(delegate
					{
						string[] array = ConvertManager.selectMultiPDFFiles();
						if (array != null && array.Length != 0)
						{
							foreach (string text in array)
							{
								this.AddOneFileToMergeList(text, null);
							}
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x060003B7 RID: 951 RVA: 0x0000F034 File Offset: 0x0000D234
		public RelayCommand ClearFiles
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.clearFiles) == null)
				{
					relayCommand = (this.clearFiles = new RelayCommand(delegate
					{
						if (this.MergePDFList.Count((MergeFileItem f) => f.IsFileSelected.GetValueOrDefault()) == 0)
						{
							ModernMessageBox.Show(Resources.MainWinOthersToPDFDeleteNoFile, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							return;
						}
						if (ModernMessageBox.Show(Resources.WinMergeSplitClearFileAskMsg, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.Yes)
						{
							this.ClearAllSelectedMergeFileItems();
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x060003B8 RID: 952 RVA: 0x0000F068 File Offset: 0x0000D268
		public RelayCommand SelectPath
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.selectPath) == null)
				{
					relayCommand = (this.selectPath = new RelayCommand(delegate
					{
						string text = ConvertManager.selectOutputFolder(this.MergeOutputPath);
						if (!string.IsNullOrWhiteSpace(text))
						{
							this.MergeOutputPath = text;
							ConfigManager.SetConvertPath(this.MergeOutputPath);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x060003B9 RID: 953 RVA: 0x0000F09C File Offset: 0x0000D29C
		public RelayCommand BeginMerge
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.beginMerge) == null)
				{
					relayCommand = (this.beginMerge = new RelayCommand(delegate
					{
						this.DoMergePDFFiles();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x060003BA RID: 954 RVA: 0x0000F0D0 File Offset: 0x0000D2D0
		public RelayCommand<MergeFileItem> OpenInExplorer
		{
			get
			{
				RelayCommand<MergeFileItem> relayCommand;
				if ((relayCommand = this.openInExplorer) == null)
				{
					relayCommand = (this.openInExplorer = new RelayCommand<MergeFileItem>(delegate(MergeFileItem model)
					{
						if (!string.IsNullOrWhiteSpace(this.MergeOutputFileFullName) && LongPathFile.Exists(this.MergeOutputFileFullName))
						{
							UtilsManager.OpenFileInExplore(this.MergeOutputFileFullName, true);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x060003BB RID: 955 RVA: 0x0000F104 File Offset: 0x0000D304
		public RelayCommand<MergeFileItem> OpenWithEditor
		{
			get
			{
				RelayCommand<MergeFileItem> relayCommand;
				if ((relayCommand = this.openWithEditor) == null)
				{
					relayCommand = (this.openWithEditor = new RelayCommand<MergeFileItem>(delegate(MergeFileItem model)
					{
						if (!string.IsNullOrWhiteSpace(this.MergeOutputFileFullName) && LongPathFile.Exists(this.MergeOutputFileFullName))
						{
							UtilsManager.OpenFile(this.MergeOutputFileFullName);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x060003BC RID: 956 RVA: 0x0000F138 File Offset: 0x0000D338
		public RelayCommand<MergeFileItem> RemoveFromList
		{
			get
			{
				RelayCommand<MergeFileItem> relayCommand;
				if ((relayCommand = this.removeFromList) == null)
				{
					relayCommand = (this.removeFromList = new RelayCommand<MergeFileItem>(delegate(MergeFileItem model)
					{
						this.RemoveOneMergeFileItem(model);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x060003BD RID: 957 RVA: 0x0000F16C File Offset: 0x0000D36C
		public RelayCommand<MergeFileItem> MoveUpFile
		{
			get
			{
				RelayCommand<MergeFileItem> relayCommand;
				if ((relayCommand = this.moveUpOneFile) == null)
				{
					relayCommand = (this.moveUpOneFile = new RelayCommand<MergeFileItem>(delegate(MergeFileItem model)
					{
						this.MoveUpOneMergeFileItem(model);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x060003BE RID: 958 RVA: 0x0000F1A0 File Offset: 0x0000D3A0
		public RelayCommand<MergeFileItem> MoveDownFile
		{
			get
			{
				RelayCommand<MergeFileItem> relayCommand;
				if ((relayCommand = this.moveDownFile) == null)
				{
					relayCommand = (this.moveDownFile = new RelayCommand<MergeFileItem>(delegate(MergeFileItem model)
					{
						this.MoveDownOneMergeFileItem(model);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x060003BF RID: 959 RVA: 0x0000F1D4 File Offset: 0x0000D3D4
		public void changeMergeFileItem(MergeFileItem dropedFileItem, MergeFileItem oriFileItem)
		{
			try
			{
				if (dropedFileItem != null && this.MergePDFList != null && oriFileItem != null)
				{
					if (this.MergePDFList.Contains(dropedFileItem) && this.MergePDFList.Contains(oriFileItem))
					{
						this.SelectedMergeFileItem = oriFileItem;
						int num = this.MergePDFList.IndexOf(dropedFileItem);
						int num2 = this.MergePDFList.IndexOf(oriFileItem);
						if (num >= 0 && num2 < this.MergePDFList.Count)
						{
							this.MergePDFList.Move(num, num2);
							base.OnPropertyChanged("MergePDFList");
						}
					}
					this.NotifyAllMergeFilesSelectedChanged();
				}
			}
			catch
			{
			}
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0000F274 File Offset: 0x0000D474
		public void MoveDownOneMergeFileItem(MergeFileItem item)
		{
			try
			{
				if (item != null && this.MergePDFList != null)
				{
					if (this.MergePDFList.Contains(item))
					{
						this.SelectedMergeFileItem = item;
						int num = this.MergePDFList.IndexOf(item);
						int num2 = num + 1;
						if (num >= 0 && num2 < this.MergePDFList.Count)
						{
							this.MergePDFList.Move(num, num2);
							base.OnPropertyChanged("MergePDFList");
						}
					}
					this.NotifyAllMergeFilesSelectedChanged();
				}
			}
			catch
			{
			}
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0000F2FC File Offset: 0x0000D4FC
		public void ClearAllSelectedMergeFileItems()
		{
			try
			{
				if (this.MergePDFList != null)
				{
					foreach (MergeFileItem mergeFileItem in this.MergePDFList.Where((MergeFileItem f) => f.IsFileSelected.GetValueOrDefault()).ToList<MergeFileItem>())
					{
						this.MergePDFList.Remove(mergeFileItem);
					}
					this.NotifyAllMergeFilesSelectedChanged();
				}
			}
			catch
			{
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x060003C2 RID: 962 RVA: 0x0000F3A0 File Offset: 0x0000D5A0
		// (set) Token: 0x060003C3 RID: 963 RVA: 0x0000F420 File Offset: 0x0000D620
		public bool? IsAllMergeFilesSelected
		{
			get
			{
				if (this.MergePDFList == null || this.MergePDFList.Count<MergeFileItem>() <= 0)
				{
					return new bool?(false);
				}
				int num = this.MergePDFList.Count<MergeFileItem>();
				int num2 = this.MergePDFList.Count((MergeFileItem f) => f.IsFileSelected.GetValueOrDefault());
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

		// Token: 0x060003C4 RID: 964 RVA: 0x0000F450 File Offset: 0x0000D650
		private void SelectAllMergeFiles(bool bSelectAll)
		{
			if (this.MergePDFList == null || this.MergePDFList.Count<MergeFileItem>() <= 0)
			{
				return;
			}
			foreach (MergeFileItem mergeFileItem in this.MergePDFList)
			{
				if (mergeFileItem.Status != MergeStatus.Unsupport)
				{
					bool? isFileSelected = mergeFileItem.IsFileSelected;
					if (!((isFileSelected.GetValueOrDefault() == bSelectAll) & (isFileSelected != null)) && mergeFileItem.Status != MergeStatus.LoadedFailed)
					{
						mergeFileItem.IsFileSelected = new bool?(bSelectAll);
					}
				}
			}
			this.NotifyAllMergeFilesSelectedChanged();
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x0000F4F0 File Offset: 0x0000D6F0
		public void NotifyAllMergeFilesSelectedChanged()
		{
			base.OnPropertyChanged("IsAllMergeFilesSelected");
			base.OnPropertyChanged("MergePDFList");
			this.UIStatus = WorkQueenState.Init;
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x060003C6 RID: 966 RVA: 0x0000F50F File Offset: 0x0000D70F
		// (set) Token: 0x060003C7 RID: 967 RVA: 0x0000F517 File Offset: 0x0000D717
		public string MergeOutputFilename
		{
			get
			{
				return this._mergeOutputFilename;
			}
			set
			{
				this.UIStatus = WorkQueenState.Init;
				base.SetProperty<string>(ref this._mergeOutputFilename, value, "MergeOutputFilename");
			}
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x060003C8 RID: 968 RVA: 0x0000F533 File Offset: 0x0000D733
		// (set) Token: 0x060003C9 RID: 969 RVA: 0x0000F53B File Offset: 0x0000D73B
		public string MergeOutputPath
		{
			get
			{
				return this._mergeOutputPath;
			}
			set
			{
				this.UIStatus = WorkQueenState.Init;
				base.SetProperty<string>(ref this._mergeOutputPath, value, "MergeOutputPath");
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x060003CA RID: 970 RVA: 0x0000F557 File Offset: 0x0000D757
		// (set) Token: 0x060003CB RID: 971 RVA: 0x0000F55F File Offset: 0x0000D75F
		public string MergeOutputFileFullName
		{
			get
			{
				return this._mergeOutputFileFullName;
			}
			set
			{
				base.SetProperty<string>(ref this._mergeOutputFileFullName, value, "MergeOutputFileFullName");
			}
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x060003CC RID: 972 RVA: 0x0000F574 File Offset: 0x0000D774
		// (set) Token: 0x060003CD RID: 973 RVA: 0x0000F57C File Offset: 0x0000D77C
		public bool? MergeViewFileInExplore
		{
			get
			{
				return this._mergeViewFileInExplore;
			}
			set
			{
				base.SetProperty<bool?>(ref this._mergeViewFileInExplore, value, "MergeViewFileInExplore");
				ConfigManager.SetConvertViewFileInExplore(value.Value);
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x060003CE RID: 974 RVA: 0x0000F59D File Offset: 0x0000D79D
		// (set) Token: 0x060003CF RID: 975 RVA: 0x0000F5A5 File Offset: 0x0000D7A5
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
					this.MergeOutputFileFullName = "";
				}
			}
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x0000F5D0 File Offset: 0x0000D7D0
		public async void DoMergePDFFiles()
		{
			MergePDFUCViewModel.<>c__DisplayClass71_0 CS$<>8__locals1 = new MergePDFUCViewModel.<>c__DisplayClass71_0();
			CS$<>8__locals1.<>4__this = this;
			GAManager.SendEvent("MergePDF", "BtnClick", "Count", 1L);
			if (!IAPHelper.IsPaidUser && !ConfigManager.GetTest())
			{
				IAPHelper.ShowActivationWindow("MergePDF", ".convert");
			}
			else if (this.MergePDFList.Where((MergeFileItem f) => f.IsFileSelected.GetValueOrDefault()).Count<MergeFileItem>() <= 1)
			{
				ModernMessageBox.Show(Resources.WinMergeSplitMergeFileCheckEmptyFileMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else if (string.IsNullOrWhiteSpace(this.MergeOutputPath) || string.IsNullOrWhiteSpace(this.MergeOutputFilename))
			{
				ModernMessageBox.Show(Resources.WinMergeSplitMergeFileCheckEmptyFilenameMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else
			{
				if (!Directory.Exists(this.MergeOutputPath))
				{
					Directory.CreateDirectory(this.MergeOutputPath);
				}
				string text = this.MergeOutputFilename;
				if (!this.MergeOutputFilename.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
				{
					text += ".pdf";
				}
				CS$<>8__locals1.outputFile = Path.Combine(this.MergeOutputPath, text);
				if (File.Exists(CS$<>8__locals1.outputFile))
				{
					ModernMessageBox.Show(Resources.WinMergeSplitMergeFileCheckExistFilenameMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else
				{
					CS$<>8__locals1.flist = new List<PdfiumPdfRange>();
					foreach (MergeFileItem mergeFileItem in this.MergePDFList.Where((MergeFileItem f) => f.IsFileSelected.GetValueOrDefault()))
					{
						LongPathFile longPathFile = mergeFileItem.FilePath;
						if (longPathFile.IsExists)
						{
							mergeFileItem.Status = MergeStatus.Merging;
							PdfiumPdfRange pdfiumPdfRange = new PdfiumPdfRange(longPathFile, mergeFileItem.PageFrom - 1, mergeFileItem.PageTo - 1, mergeFileItem.Passwrod);
							CS$<>8__locals1.flist.Add(pdfiumPdfRange);
						}
					}
					await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
					{
						MergePDFUCViewModel.<>c__DisplayClass71_0.<<DoMergePDFFiles>b__1>d <<DoMergePDFFiles>b__1>d;
						<<DoMergePDFFiles>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<DoMergePDFFiles>b__1>d.<>4__this = CS$<>8__locals1;
						<<DoMergePDFFiles>b__1>d.<>1__state = -1;
						<<DoMergePDFFiles>b__1>d.<>t__builder.Start<MergePDFUCViewModel.<>c__DisplayClass71_0.<<DoMergePDFFiles>b__1>d>(ref <<DoMergePDFFiles>b__1>d);
						return <<DoMergePDFFiles>b__1>d.<>t__builder.Task;
					})).ConfigureAwait(false);
				}
			}
		}

		// Token: 0x04000204 RID: 516
		private MergeFileItemCollection _mergePDFList;

		// Token: 0x04000205 RID: 517
		private MergeFileItem _selectedMergeFileItem;

		// Token: 0x04000206 RID: 518
		private string _mergeOutputPath;

		// Token: 0x04000207 RID: 519
		private string _mergeOutputFilename;

		// Token: 0x04000208 RID: 520
		private string _mergeOutputFileFullName;

		// Token: 0x04000209 RID: 521
		private bool? _mergeViewFileInExplore;

		// Token: 0x0400020A RID: 522
		private WorkQueenState _UIStatus;

		// Token: 0x0400020B RID: 523
		private RelayCommand addOneFile;

		// Token: 0x0400020C RID: 524
		private RelayCommand clearFiles;

		// Token: 0x0400020D RID: 525
		private RelayCommand selectPath;

		// Token: 0x0400020E RID: 526
		private RelayCommand beginMerge;

		// Token: 0x0400020F RID: 527
		private RelayCommand updateItem;

		// Token: 0x04000210 RID: 528
		private RelayCommand<MergeFileItem> moveUpOneFile;

		// Token: 0x04000211 RID: 529
		private RelayCommand<MergeFileItem> moveDownFile;

		// Token: 0x04000212 RID: 530
		private RelayCommand<MergeFileItem> openInExplorer;

		// Token: 0x04000213 RID: 531
		private RelayCommand<MergeFileItem> openWithEditor;

		// Token: 0x04000214 RID: 532
		private RelayCommand<MergeFileItem> removeFromList;
	}
}
