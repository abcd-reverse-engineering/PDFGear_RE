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
	// Token: 0x02000038 RID: 56
	public class TXTToPDFUCViewModel : ObservableObject
	{
		// Token: 0x0600045E RID: 1118 RVA: 0x00011850 File Offset: 0x0000FA50
		public TXTToPDFUCViewModel()
		{
			this.InitializeEnv();
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x00011870 File Offset: 0x0000FA70
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
			if (App.convertType.Equals(ConvToPDFType.TxtToPDF) && App.selectedFile != null && App.selectedFile.Length != 0)
			{
				foreach (string text2 in App.selectedFile)
				{
					this.AddOneFileToFileList(text2);
				}
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000460 RID: 1120 RVA: 0x0001191C File Offset: 0x0000FB1C
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

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000461 RID: 1121 RVA: 0x00011941 File Offset: 0x0000FB41
		// (set) Token: 0x06000462 RID: 1122 RVA: 0x00011949 File Offset: 0x0000FB49
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

		// Token: 0x06000463 RID: 1123 RVA: 0x00011960 File Offset: 0x0000FB60
		private bool IsFileHasBeenAdded(string fileName)
		{
			return !string.IsNullOrWhiteSpace(fileName) && this.FileList != null && this.FileList.Where((ToPDFFileItem f) => f.FilePath.Equals(fileName)).ToList<ToPDFFileItem>().Count<ToPDFFileItem>() > 0;
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x000119B8 File Offset: 0x0000FBB8
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
				ToPDFFileItem toPDFFileItem = new ToPDFFileItem(longPathFile, ConvToPDFType.TxtToPDF);
				if (toPDFFileItem != null)
				{
					if (UtilsManager.IsnotSupportFile(longPathFile, UtilManager.TxtExtentions))
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

		// Token: 0x06000465 RID: 1125 RVA: 0x00011B44 File Offset: 0x0000FD44
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

		// Token: 0x06000466 RID: 1126 RVA: 0x00011B94 File Offset: 0x0000FD94
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

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000467 RID: 1127 RVA: 0x00011C10 File Offset: 0x0000FE10
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

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06000468 RID: 1128 RVA: 0x00011C44 File Offset: 0x0000FE44
		public RelayCommand AddOneFile
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.addOneFile) == null)
				{
					relayCommand = (this.addOneFile = new RelayCommand(delegate
					{
						string[] array = ConvertManager.selectMultiFiles("Text Format", UtilManager.TxtExtention);
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

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06000469 RID: 1129 RVA: 0x00011C78 File Offset: 0x0000FE78
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

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x0600046A RID: 1130 RVA: 0x00011CAC File Offset: 0x0000FEAC
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

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x0600046B RID: 1131 RVA: 0x00011CE0 File Offset: 0x0000FEE0
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

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x0600046C RID: 1132 RVA: 0x00011D14 File Offset: 0x0000FF14
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

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x0600046D RID: 1133 RVA: 0x00011D48 File Offset: 0x0000FF48
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

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x0600046E RID: 1134 RVA: 0x00011D7C File Offset: 0x0000FF7C
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

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x0600046F RID: 1135 RVA: 0x00011DB0 File Offset: 0x0000FFB0
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

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06000470 RID: 1136 RVA: 0x00011DE4 File Offset: 0x0000FFE4
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

		// Token: 0x06000471 RID: 1137 RVA: 0x00011E18 File Offset: 0x00010018
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

		// Token: 0x06000472 RID: 1138 RVA: 0x00011EA0 File Offset: 0x000100A0
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

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06000473 RID: 1139 RVA: 0x00011F44 File Offset: 0x00010144
		// (set) Token: 0x06000474 RID: 1140 RVA: 0x00011FC4 File Offset: 0x000101C4
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

		// Token: 0x06000475 RID: 1141 RVA: 0x00011FF4 File Offset: 0x000101F4
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

		// Token: 0x06000476 RID: 1142 RVA: 0x00012094 File Offset: 0x00010294
		public void NotifyAllMergeFilesSelectedChanged()
		{
			base.OnPropertyChanged("IsAllMergeFilesSelected");
			base.OnPropertyChanged("FileList");
			this.UIStatus = WorkQueenState.Init;
		}

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000477 RID: 1143 RVA: 0x000120B3 File Offset: 0x000102B3
		// (set) Token: 0x06000478 RID: 1144 RVA: 0x000120BB File Offset: 0x000102BB
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

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000479 RID: 1145 RVA: 0x000120D0 File Offset: 0x000102D0
		// (set) Token: 0x0600047A RID: 1146 RVA: 0x000120D8 File Offset: 0x000102D8
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

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x0600047B RID: 1147 RVA: 0x000120F4 File Offset: 0x000102F4
		// (set) Token: 0x0600047C RID: 1148 RVA: 0x000120FC File Offset: 0x000102FC
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

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x0600047D RID: 1149 RVA: 0x00012111 File Offset: 0x00010311
		// (set) Token: 0x0600047E RID: 1150 RVA: 0x00012119 File Offset: 0x00010319
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

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x0600047F RID: 1151 RVA: 0x0001213A File Offset: 0x0001033A
		// (set) Token: 0x06000480 RID: 1152 RVA: 0x00012142 File Offset: 0x00010342
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

		// Token: 0x06000481 RID: 1153 RVA: 0x0001216C File Offset: 0x0001036C
		public async void ProcessingFiles()
		{
			GAManager.SendEvent("TxtToPDF", "BtnClick", "Count", 1L);
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
					TXTToPDFUCViewModel.<>c__DisplayClass70_0 CS$<>8__locals1 = new TXTToPDFUCViewModel.<>c__DisplayClass70_0();
					CS$<>8__locals1.<>4__this = this;
					this.UIStatus = WorkQueenState.Working;
					int selectCount = 0;
					CS$<>8__locals1.SuccCount = 0;
					List<Task> list = new List<Task>();
					using (IEnumerator<ToPDFFileItem> enumerator = this.FileList.Where((ToPDFFileItem f) => f.IsFileSelected.GetValueOrDefault()).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TXTToPDFUCViewModel.<>c__DisplayClass70_1 CS$<>8__locals2 = new TXTToPDFUCViewModel.<>c__DisplayClass70_1();
							CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
							CS$<>8__locals2.it = enumerator.Current;
							int num = selectCount;
							selectCount = num + 1;
							Task task = Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
							{
								TXTToPDFUCViewModel.<>c__DisplayClass70_1.<<ProcessingFiles>b__3>d <<ProcessingFiles>b__3>d;
								<<ProcessingFiles>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
								<<ProcessingFiles>b__3>d.<>4__this = CS$<>8__locals2;
								<<ProcessingFiles>b__3>d.<>1__state = -1;
								<<ProcessingFiles>b__3>d.<>t__builder.Start<TXTToPDFUCViewModel.<>c__DisplayClass70_1.<<ProcessingFiles>b__3>d>(ref <<ProcessingFiles>b__3>d);
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
						GAManager.SendEvent("TxtToPDF", "HasFailed", "Count", 1L);
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
						GAManager.SendEvent("TxtToPDF", "AllSucc", "Count", 1L);
					}
				})).ConfigureAwait(false);
				this.SelectAllMergeFiles(false);
			}
		}

		// Token: 0x04000243 RID: 579
		private ToPDFFileItemCollection _toPDFItemList;

		// Token: 0x04000244 RID: 580
		private ToPDFFileItem _selectedItem;

		// Token: 0x04000245 RID: 581
		private string _OutputPath;

		// Token: 0x04000246 RID: 582
		private string _OutputFilename;

		// Token: 0x04000247 RID: 583
		private string _OutputFileFullName;

		// Token: 0x04000248 RID: 584
		private bool? _ViewFileInExplore = new bool?(ConfigManager.GetConvertViewFileInExplore());

		// Token: 0x04000249 RID: 585
		private WorkQueenState _UIStatus;

		// Token: 0x0400024A RID: 586
		private RelayCommand addOneFile;

		// Token: 0x0400024B RID: 587
		private RelayCommand clearFiles;

		// Token: 0x0400024C RID: 588
		private RelayCommand selectPath;

		// Token: 0x0400024D RID: 589
		private RelayCommand beginWorks;

		// Token: 0x0400024E RID: 590
		private RelayCommand updateItem;

		// Token: 0x0400024F RID: 591
		private RelayCommand<ToPDFFileItem> moveUpOneFile;

		// Token: 0x04000250 RID: 592
		private RelayCommand<ToPDFFileItem> moveDownFile;

		// Token: 0x04000251 RID: 593
		private RelayCommand<ToPDFFileItem> openInExplorer;

		// Token: 0x04000252 RID: 594
		private RelayCommand<ToPDFFileItem> openWithEditor;

		// Token: 0x04000253 RID: 595
		private RelayCommand<ToPDFFileItem> removeFromList;
	}
}
