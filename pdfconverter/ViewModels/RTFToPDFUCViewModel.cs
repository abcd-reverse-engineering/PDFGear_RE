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
	// Token: 0x02000036 RID: 54
	public class RTFToPDFUCViewModel : ObservableObject
	{
		// Token: 0x0600040B RID: 1035 RVA: 0x00010378 File Offset: 0x0000E578
		public RTFToPDFUCViewModel()
		{
			this.InitializeEnv();
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x00010398 File Offset: 0x0000E598
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
			if (App.convertType.Equals(ConvToPDFType.RtfToPDF) && App.selectedFile != null && App.selectedFile.Length != 0)
			{
				foreach (string text2 in App.selectedFile)
				{
					this.AddOneFileToFileList(text2);
				}
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x0600040D RID: 1037 RVA: 0x00010444 File Offset: 0x0000E644
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

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x0600040E RID: 1038 RVA: 0x00010469 File Offset: 0x0000E669
		// (set) Token: 0x0600040F RID: 1039 RVA: 0x00010471 File Offset: 0x0000E671
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

		// Token: 0x06000410 RID: 1040 RVA: 0x00010488 File Offset: 0x0000E688
		private bool IsFileHasBeenAdded(string fileName)
		{
			return !string.IsNullOrWhiteSpace(fileName) && this.FileList != null && this.FileList.Where((ToPDFFileItem f) => f.FilePath.Equals(fileName)).ToList<ToPDFFileItem>().Count<ToPDFFileItem>() > 0;
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x000104E0 File Offset: 0x0000E6E0
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
				ToPDFFileItem toPDFFileItem = new ToPDFFileItem(longPathFile, ConvToPDFType.RtfToPDF);
				if (toPDFFileItem != null)
				{
					if (UtilsManager.IsnotSupportFile(longPathFile, UtilManager.RtfExtentions))
					{
						toPDFFileItem.Status = ToPDFItemStatus.Unsupport;
						toPDFFileItem.IsFileSelected = new bool?(false);
						toPDFFileItem.IsEnable = new bool?(false);
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

		// Token: 0x06000412 RID: 1042 RVA: 0x0001066C File Offset: 0x0000E86C
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

		// Token: 0x06000413 RID: 1043 RVA: 0x000106BC File Offset: 0x0000E8BC
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

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06000414 RID: 1044 RVA: 0x00010738 File Offset: 0x0000E938
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

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000415 RID: 1045 RVA: 0x0001076C File Offset: 0x0000E96C
		public RelayCommand AddOneFile
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.addOneFile) == null)
				{
					relayCommand = (this.addOneFile = new RelayCommand(delegate
					{
						string[] array = ConvertManager.selectMultiFiles("Rich Test Format", UtilManager.RtfExtention);
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

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x06000416 RID: 1046 RVA: 0x000107A0 File Offset: 0x0000E9A0
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

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x06000417 RID: 1047 RVA: 0x000107D4 File Offset: 0x0000E9D4
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

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x06000418 RID: 1048 RVA: 0x00010808 File Offset: 0x0000EA08
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

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06000419 RID: 1049 RVA: 0x0001083C File Offset: 0x0000EA3C
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

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x0600041A RID: 1050 RVA: 0x00010870 File Offset: 0x0000EA70
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

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x0600041B RID: 1051 RVA: 0x000108A4 File Offset: 0x0000EAA4
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

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x0600041C RID: 1052 RVA: 0x000108D8 File Offset: 0x0000EAD8
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

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x0600041D RID: 1053 RVA: 0x0001090C File Offset: 0x0000EB0C
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

		// Token: 0x0600041E RID: 1054 RVA: 0x00010940 File Offset: 0x0000EB40
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

		// Token: 0x0600041F RID: 1055 RVA: 0x000109C8 File Offset: 0x0000EBC8
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

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06000420 RID: 1056 RVA: 0x00010A6C File Offset: 0x0000EC6C
		// (set) Token: 0x06000421 RID: 1057 RVA: 0x00010AEC File Offset: 0x0000ECEC
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

		// Token: 0x06000422 RID: 1058 RVA: 0x00010B1C File Offset: 0x0000ED1C
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

		// Token: 0x06000423 RID: 1059 RVA: 0x00010BBC File Offset: 0x0000EDBC
		public void NotifyAllMergeFilesSelectedChanged()
		{
			base.OnPropertyChanged("IsAllMergeFilesSelected");
			base.OnPropertyChanged("FileList");
			this.UIStatus = WorkQueenState.Init;
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06000424 RID: 1060 RVA: 0x00010BDB File Offset: 0x0000EDDB
		// (set) Token: 0x06000425 RID: 1061 RVA: 0x00010BE3 File Offset: 0x0000EDE3
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

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06000426 RID: 1062 RVA: 0x00010BF8 File Offset: 0x0000EDF8
		// (set) Token: 0x06000427 RID: 1063 RVA: 0x00010C00 File Offset: 0x0000EE00
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

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06000428 RID: 1064 RVA: 0x00010C1C File Offset: 0x0000EE1C
		// (set) Token: 0x06000429 RID: 1065 RVA: 0x00010C24 File Offset: 0x0000EE24
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

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x0600042A RID: 1066 RVA: 0x00010C39 File Offset: 0x0000EE39
		// (set) Token: 0x0600042B RID: 1067 RVA: 0x00010C41 File Offset: 0x0000EE41
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

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x0600042C RID: 1068 RVA: 0x00010C62 File Offset: 0x0000EE62
		// (set) Token: 0x0600042D RID: 1069 RVA: 0x00010C6A File Offset: 0x0000EE6A
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

		// Token: 0x0600042E RID: 1070 RVA: 0x00010C94 File Offset: 0x0000EE94
		public async void ProcessingFiles()
		{
			GAManager.SendEvent("RTFToPDF", "BtnClick", "Count", 1L);
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
					RTFToPDFUCViewModel.<>c__DisplayClass70_0 CS$<>8__locals1 = new RTFToPDFUCViewModel.<>c__DisplayClass70_0();
					CS$<>8__locals1.<>4__this = this;
					this.UIStatus = WorkQueenState.Working;
					int selectCount = 0;
					CS$<>8__locals1.SuccCount = 0;
					List<Task> list = new List<Task>();
					using (IEnumerator<ToPDFFileItem> enumerator = this.FileList.Where((ToPDFFileItem f) => f.IsFileSelected.GetValueOrDefault()).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							RTFToPDFUCViewModel.<>c__DisplayClass70_1 CS$<>8__locals2 = new RTFToPDFUCViewModel.<>c__DisplayClass70_1();
							CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
							CS$<>8__locals2.it = enumerator.Current;
							int num = selectCount;
							selectCount = num + 1;
							Task task = Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
							{
								RTFToPDFUCViewModel.<>c__DisplayClass70_1.<<ProcessingFiles>b__3>d <<ProcessingFiles>b__3>d;
								<<ProcessingFiles>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
								<<ProcessingFiles>b__3>d.<>4__this = CS$<>8__locals2;
								<<ProcessingFiles>b__3>d.<>1__state = -1;
								<<ProcessingFiles>b__3>d.<>t__builder.Start<RTFToPDFUCViewModel.<>c__DisplayClass70_1.<<ProcessingFiles>b__3>d>(ref <<ProcessingFiles>b__3>d);
								return <<ProcessingFiles>b__3>d.<>t__builder.Task;
							}));
							list.Add(task);
						}
					}
					await Task.WhenAll(list).ConfigureAwait(true);
					if (this.ViewFileInExplore.GetValueOrDefault() && CS$<>8__locals1.SuccCount > 0)
					{
						UtilsManager.OpenFileInExplore(this.OutputFilename, true);
					}
					this.UIStatus = WorkQueenState.Succ;
					if (selectCount != CS$<>8__locals1.SuccCount)
					{
						GAManager.SendEvent("RTFToPDF", "HasFailed", "Count", 1L);
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
						GAManager.SendEvent("RTFToPDF", "AllSucc", "Count", 1L);
					}
				})).ConfigureAwait(false);
				this.SelectAllMergeFiles(false);
			}
		}

		// Token: 0x04000226 RID: 550
		private ToPDFFileItemCollection _toPDFItemList;

		// Token: 0x04000227 RID: 551
		private ToPDFFileItem _selectedItem;

		// Token: 0x04000228 RID: 552
		private string _OutputPath;

		// Token: 0x04000229 RID: 553
		private string _OutputFilename;

		// Token: 0x0400022A RID: 554
		private string _OutputFileFullName;

		// Token: 0x0400022B RID: 555
		private bool? _ViewFileInExplore = new bool?(ConfigManager.GetConvertViewFileInExplore());

		// Token: 0x0400022C RID: 556
		private WorkQueenState _UIStatus;

		// Token: 0x0400022D RID: 557
		private RelayCommand addOneFile;

		// Token: 0x0400022E RID: 558
		private RelayCommand clearFiles;

		// Token: 0x0400022F RID: 559
		private RelayCommand selectPath;

		// Token: 0x04000230 RID: 560
		private RelayCommand beginWorks;

		// Token: 0x04000231 RID: 561
		private RelayCommand updateItem;

		// Token: 0x04000232 RID: 562
		private RelayCommand<ToPDFFileItem> moveUpOneFile;

		// Token: 0x04000233 RID: 563
		private RelayCommand<ToPDFFileItem> moveDownFile;

		// Token: 0x04000234 RID: 564
		private RelayCommand<ToPDFFileItem> openInExplorer;

		// Token: 0x04000235 RID: 565
		private RelayCommand<ToPDFFileItem> openWithEditor;

		// Token: 0x04000236 RID: 566
		private RelayCommand<ToPDFFileItem> removeFromList;
	}
}
