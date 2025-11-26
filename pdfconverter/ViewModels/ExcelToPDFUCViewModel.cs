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
	// Token: 0x0200002F RID: 47
	public class ExcelToPDFUCViewModel : ObservableObject
	{
		// Token: 0x060002CE RID: 718 RVA: 0x0000BA00 File Offset: 0x00009C00
		public ExcelToPDFUCViewModel()
		{
			this.InitializeEnv();
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0000BA28 File Offset: 0x00009C28
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
			if (App.convertType.Equals(ConvToPDFType.ExcelToPDF) && App.selectedFile != null && App.selectedFile.Length != 0)
			{
				foreach (string text2 in App.selectedFile)
				{
					this.AddOneFileToFileList(text2);
				}
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060002D0 RID: 720 RVA: 0x0000BAD4 File Offset: 0x00009CD4
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

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060002D1 RID: 721 RVA: 0x0000BAF9 File Offset: 0x00009CF9
		// (set) Token: 0x060002D2 RID: 722 RVA: 0x0000BB01 File Offset: 0x00009D01
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

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x0000BB16 File Offset: 0x00009D16
		// (set) Token: 0x060002D4 RID: 724 RVA: 0x0000BB1E File Offset: 0x00009D1E
		public bool FillAllColinOnePage
		{
			get
			{
				return this.fillAllColinOnePage;
			}
			set
			{
				base.SetProperty<bool>(ref this.fillAllColinOnePage, value, "FillAllColinOnePage");
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0000BB34 File Offset: 0x00009D34
		private bool IsFileHasBeenAdded(string fileName)
		{
			return !string.IsNullOrWhiteSpace(fileName) && this.FileList != null && this.FileList.Where((ToPDFFileItem f) => f.FilePath.Equals(fileName)).ToList<ToPDFFileItem>().Count<ToPDFFileItem>() > 0;
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0000BB8C File Offset: 0x00009D8C
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
				ToPDFFileItem toPDFFileItem = new ToPDFFileItem(longPathFile, ConvToPDFType.ExcelToPDF);
				if (toPDFFileItem != null)
				{
					string text = "";
					if (UtilsManager.IsnotSupportFile(longPathFile, UtilManager.ExcelExtentions))
					{
						toPDFFileItem.Status = ToPDFItemStatus.Unsupport;
						toPDFFileItem.IsFileSelected = new bool?(false);
						toPDFFileItem.IsEnable = new bool?(false);
						this.FileList.Add(toPDFFileItem);
					}
					else if (ConToPDFUtils.CheckExcelPassword(longPathFile, out text, Application.Current.MainWindow))
					{
						toPDFFileItem.Password = text;
						this.FileList.Add(toPDFFileItem);
						toPDFFileItem.ParseFile();
					}
					else
					{
						toPDFFileItem.Status = ToPDFItemStatus.LoadedFailed;
						toPDFFileItem.IsFileSelected = new bool?(false);
						toPDFFileItem.IsEnable = new bool?(false);
						this.FileList.Add(toPDFFileItem);
					}
					if (this.FileList.Count == 1)
					{
						string text2 = Path.GetFileNameWithoutExtension(toPDFFileItem.FilePath);
						if (string.IsNullOrWhiteSpace(text2))
						{
							text2 = "default";
						}
						text2 = UtilsManager.getValidFileName(this.OutputPath, text2, ".pdf");
						if (!string.IsNullOrWhiteSpace(text2))
						{
							this.OutputFilename = text2;
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

		// Token: 0x060002D7 RID: 727 RVA: 0x0000BD78 File Offset: 0x00009F78
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

		// Token: 0x060002D8 RID: 728 RVA: 0x0000BDC8 File Offset: 0x00009FC8
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

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x060002D9 RID: 729 RVA: 0x0000BE44 File Offset: 0x0000A044
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

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x060002DA RID: 730 RVA: 0x0000BE78 File Offset: 0x0000A078
		public RelayCommand AddOneFile
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.addOneFile) == null)
				{
					relayCommand = (this.addOneFile = new RelayCommand(delegate
					{
						string[] array = ConvertManager.selectMultiFiles("Excel Work Sheet", UtilManager.ExcelExtention);
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

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060002DB RID: 731 RVA: 0x0000BEAC File Offset: 0x0000A0AC
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

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x060002DC RID: 732 RVA: 0x0000BEE0 File Offset: 0x0000A0E0
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

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x060002DD RID: 733 RVA: 0x0000BF14 File Offset: 0x0000A114
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

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x060002DE RID: 734 RVA: 0x0000BF48 File Offset: 0x0000A148
		public RelayCommand<ToPDFFileItem> OpenInExplorer
		{
			get
			{
				RelayCommand<ToPDFFileItem> relayCommand;
				if ((relayCommand = this.openInExplorer) == null)
				{
					relayCommand = (this.openInExplorer = new RelayCommand<ToPDFFileItem>(delegate(ToPDFFileItem model)
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

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060002DF RID: 735 RVA: 0x0000BF7C File Offset: 0x0000A17C
		public RelayCommand<ToPDFFileItem> OpenWithEditor
		{
			get
			{
				RelayCommand<ToPDFFileItem> relayCommand;
				if ((relayCommand = this.openWithEditor) == null)
				{
					relayCommand = (this.openWithEditor = new RelayCommand<ToPDFFileItem>(delegate(ToPDFFileItem model)
					{
						string pdfresult = UtilsManager.getPDFResult(this.OutputPath, model.FilePath);
						if (LongPathFile.Exists(pdfresult))
						{
							UtilsManager.OpenFile(pdfresult);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x0000BFB0 File Offset: 0x0000A1B0
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

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x060002E1 RID: 737 RVA: 0x0000BFE4 File Offset: 0x0000A1E4
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

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x0000C018 File Offset: 0x0000A218
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

		// Token: 0x060002E3 RID: 739 RVA: 0x0000C04C File Offset: 0x0000A24C
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

		// Token: 0x060002E4 RID: 740 RVA: 0x0000C0D4 File Offset: 0x0000A2D4
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

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x060002E5 RID: 741 RVA: 0x0000C178 File Offset: 0x0000A378
		// (set) Token: 0x060002E6 RID: 742 RVA: 0x0000C1F8 File Offset: 0x0000A3F8
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

		// Token: 0x060002E7 RID: 743 RVA: 0x0000C228 File Offset: 0x0000A428
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

		// Token: 0x060002E8 RID: 744 RVA: 0x0000C2C8 File Offset: 0x0000A4C8
		public void NotifyAllMergeFilesSelectedChanged()
		{
			base.OnPropertyChanged("IsAllMergeFilesSelected");
			base.OnPropertyChanged("FileList");
			this.UIStatus = WorkQueenState.Init;
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x060002E9 RID: 745 RVA: 0x0000C2E7 File Offset: 0x0000A4E7
		// (set) Token: 0x060002EA RID: 746 RVA: 0x0000C2EF File Offset: 0x0000A4EF
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

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x060002EB RID: 747 RVA: 0x0000C304 File Offset: 0x0000A504
		// (set) Token: 0x060002EC RID: 748 RVA: 0x0000C30C File Offset: 0x0000A50C
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

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060002ED RID: 749 RVA: 0x0000C328 File Offset: 0x0000A528
		// (set) Token: 0x060002EE RID: 750 RVA: 0x0000C330 File Offset: 0x0000A530
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

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060002EF RID: 751 RVA: 0x0000C345 File Offset: 0x0000A545
		// (set) Token: 0x060002F0 RID: 752 RVA: 0x0000C34D File Offset: 0x0000A54D
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

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x060002F1 RID: 753 RVA: 0x0000C36E File Offset: 0x0000A56E
		// (set) Token: 0x060002F2 RID: 754 RVA: 0x0000C376 File Offset: 0x0000A576
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

		// Token: 0x060002F3 RID: 755 RVA: 0x0000C3A0 File Offset: 0x0000A5A0
		public async void ProcessingFiles()
		{
			GAManager.SendEvent("ExcelToPDF", "BtnClick", "Count", 1L);
			if (this.FileList.Count((ToPDFFileItem f) => f.IsFileSelected.GetValueOrDefault()) <= 0)
			{
				ModernMessageBox.Show(Resources.WinConvertAddFileTipText, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else if (string.IsNullOrWhiteSpace(this.OutputPath))
			{
				ModernMessageBox.Show(Resources.WinConvertSetOutputFolderText, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else
			{
				await Task.Run(TaskExceptionHelper.ExceptionBoundary(async delegate
				{
					ExcelToPDFUCViewModel.<>c__DisplayClass74_0 CS$<>8__locals1 = new ExcelToPDFUCViewModel.<>c__DisplayClass74_0();
					CS$<>8__locals1.<>4__this = this;
					this.UIStatus = WorkQueenState.Working;
					int selectCount = 0;
					CS$<>8__locals1.SuccCount = 0;
					List<Task> list = new List<Task>();
					using (IEnumerator<ToPDFFileItem> enumerator = this.FileList.Where((ToPDFFileItem f) => f.IsFileSelected.GetValueOrDefault()).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ExcelToPDFUCViewModel.<>c__DisplayClass74_1 CS$<>8__locals2 = new ExcelToPDFUCViewModel.<>c__DisplayClass74_1();
							CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
							CS$<>8__locals2.it = enumerator.Current;
							int num = selectCount;
							selectCount = num + 1;
							Task task = Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
							{
								ExcelToPDFUCViewModel.<>c__DisplayClass74_1.<<ProcessingFiles>b__3>d <<ProcessingFiles>b__3>d;
								<<ProcessingFiles>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
								<<ProcessingFiles>b__3>d.<>4__this = CS$<>8__locals2;
								<<ProcessingFiles>b__3>d.<>1__state = -1;
								<<ProcessingFiles>b__3>d.<>t__builder.Start<ExcelToPDFUCViewModel.<>c__DisplayClass74_1.<<ProcessingFiles>b__3>d>(ref <<ProcessingFiles>b__3>d);
								return <<ProcessingFiles>b__3>d.<>t__builder.Task;
							}));
							list.Add(task);
						}
					}
					await Task.WhenAll(list).ConfigureAwait(true);
					this.UIStatus = WorkQueenState.Succ;
					if (this.ViewFileInExplore.GetValueOrDefault() && CS$<>8__locals1.SuccCount > 0)
					{
						UtilsManager.OpenFileInExplore(this.OutputFilename, true);
					}
					if (selectCount != CS$<>8__locals1.SuccCount)
					{
						GAManager.SendEvent("ExcelToPDF", "HasFailed", "Count", 1L);
						if (MessageBox.Show(Resources.FileConvertMsgConvertFailSupport, UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
						{
							Application.Current.Dispatcher.Invoke(delegate
							{
								FeedbackWindow feedbackWindow = new FeedbackWindow();
								if (feedbackWindow != null)
								{
									try
									{
										foreach (ToPDFFileItem toPDFFileItem in this.FileList)
										{
											if (toPDFFileItem.IsFileSelected.GetValueOrDefault() && toPDFFileItem.Status == ToPDFItemStatus.Fail)
											{
												feedbackWindow.flist.Add(toPDFFileItem.FilePath);
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
						GAManager.SendEvent("ExcelToPDF", "AllSucc", "Count", 1L);
					}
				})).ConfigureAwait(false);
				this.SelectAllMergeFiles(false);
			}
		}

		// Token: 0x0400019B RID: 411
		private ToPDFFileItemCollection _toPDFItemList;

		// Token: 0x0400019C RID: 412
		private ToPDFFileItem _selectedItem;

		// Token: 0x0400019D RID: 413
		private string _OutputPath;

		// Token: 0x0400019E RID: 414
		private string _OutputFilename;

		// Token: 0x0400019F RID: 415
		private string _OutputFileFullName;

		// Token: 0x040001A0 RID: 416
		private bool? _ViewFileInExplore = new bool?(ConfigManager.GetConvertViewFileInExplore());

		// Token: 0x040001A1 RID: 417
		private bool fillAllColinOnePage = true;

		// Token: 0x040001A2 RID: 418
		private WorkQueenState _UIStatus;

		// Token: 0x040001A3 RID: 419
		private RelayCommand addOneFile;

		// Token: 0x040001A4 RID: 420
		private RelayCommand clearFiles;

		// Token: 0x040001A5 RID: 421
		private RelayCommand selectPath;

		// Token: 0x040001A6 RID: 422
		private RelayCommand beginWorks;

		// Token: 0x040001A7 RID: 423
		private RelayCommand updateItem;

		// Token: 0x040001A8 RID: 424
		private RelayCommand<ToPDFFileItem> moveUpOneFile;

		// Token: 0x040001A9 RID: 425
		private RelayCommand<ToPDFFileItem> moveDownFile;

		// Token: 0x040001AA RID: 426
		private RelayCommand<ToPDFFileItem> openInExplorer;

		// Token: 0x040001AB RID: 427
		private RelayCommand<ToPDFFileItem> openWithEditor;

		// Token: 0x040001AC RID: 428
		private RelayCommand<ToPDFFileItem> removeFromList;
	}
}
