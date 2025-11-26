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
	// Token: 0x02000035 RID: 53
	public class PPTToPDFUCViewModel : ObservableObject
	{
		// Token: 0x060003DB RID: 987 RVA: 0x0000F759 File Offset: 0x0000D959
		public PPTToPDFUCViewModel()
		{
			this.InitializeEnv();
		}

		// Token: 0x060003DC RID: 988 RVA: 0x0000F778 File Offset: 0x0000D978
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
			if (App.convertType.Equals(ConvToPDFType.PPTToPDF) && App.selectedFile != null && App.selectedFile.Length != 0)
			{
				foreach (string text2 in App.selectedFile)
				{
					this.AddOneFileToFileList(text2);
				}
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x060003DD RID: 989 RVA: 0x0000F824 File Offset: 0x0000DA24
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

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x060003DE RID: 990 RVA: 0x0000F849 File Offset: 0x0000DA49
		// (set) Token: 0x060003DF RID: 991 RVA: 0x0000F851 File Offset: 0x0000DA51
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

		// Token: 0x060003E0 RID: 992 RVA: 0x0000F868 File Offset: 0x0000DA68
		private bool IsFileHasBeenAdded(string fileName)
		{
			return !string.IsNullOrWhiteSpace(fileName) && this.FileList != null && this.FileList.Where((ToPDFFileItem f) => f.FilePath.Equals(fileName)).ToList<ToPDFFileItem>().Count<ToPDFFileItem>() > 0;
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x0000F8C0 File Offset: 0x0000DAC0
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
					MessageBox.Show(Resources.WinMergeSplitAddFileCheckMsg.Replace("XXX", fileName), UtilManager.GetProductName());
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
				ToPDFFileItem toPDFFileItem = new ToPDFFileItem(longPathFile, ConvToPDFType.PPTToPDF);
				if (toPDFFileItem != null)
				{
					string text = "";
					if (UtilsManager.IsnotSupportFile(longPathFile, UtilManager.PPTExtentions))
					{
						toPDFFileItem.Status = ToPDFItemStatus.Unsupport;
						toPDFFileItem.IsFileSelected = new bool?(false);
						toPDFFileItem.IsEnable = new bool?(false);
						this.FileList.Add(toPDFFileItem);
					}
					else if (ConToPDFUtils.CheckPPTPassword(longPathFile, out text, Application.Current.MainWindow))
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

		// Token: 0x060003E2 RID: 994 RVA: 0x0000FAA8 File Offset: 0x0000DCA8
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

		// Token: 0x060003E3 RID: 995 RVA: 0x0000FAF8 File Offset: 0x0000DCF8
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

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x060003E4 RID: 996 RVA: 0x0000FB74 File Offset: 0x0000DD74
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

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060003E5 RID: 997 RVA: 0x0000FBA8 File Offset: 0x0000DDA8
		public RelayCommand AddOneFile
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.addOneFile) == null)
				{
					relayCommand = (this.addOneFile = new RelayCommand(delegate
					{
						string[] array = ConvertManager.selectMultiFiles("Microsoft Office PowerPoint", UtilManager.PPTExtention);
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

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x060003E6 RID: 998 RVA: 0x0000FBDC File Offset: 0x0000DDDC
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

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x060003E7 RID: 999 RVA: 0x0000FC10 File Offset: 0x0000DE10
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

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x060003E8 RID: 1000 RVA: 0x0000FC44 File Offset: 0x0000DE44
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

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x060003E9 RID: 1001 RVA: 0x0000FC78 File Offset: 0x0000DE78
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

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x060003EA RID: 1002 RVA: 0x0000FCAC File Offset: 0x0000DEAC
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

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x060003EB RID: 1003 RVA: 0x0000FCE0 File Offset: 0x0000DEE0
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

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x060003EC RID: 1004 RVA: 0x0000FD14 File Offset: 0x0000DF14
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

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x060003ED RID: 1005 RVA: 0x0000FD48 File Offset: 0x0000DF48
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

		// Token: 0x060003EE RID: 1006 RVA: 0x0000FD7C File Offset: 0x0000DF7C
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

		// Token: 0x060003EF RID: 1007 RVA: 0x0000FE04 File Offset: 0x0000E004
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

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x060003F0 RID: 1008 RVA: 0x0000FEA8 File Offset: 0x0000E0A8
		// (set) Token: 0x060003F1 RID: 1009 RVA: 0x0000FF28 File Offset: 0x0000E128
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

		// Token: 0x060003F2 RID: 1010 RVA: 0x0000FF58 File Offset: 0x0000E158
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

		// Token: 0x060003F3 RID: 1011 RVA: 0x0000FFF8 File Offset: 0x0000E1F8
		public void NotifyAllMergeFilesSelectedChanged()
		{
			base.OnPropertyChanged("IsAllMergeFilesSelected");
			base.OnPropertyChanged("FileList");
			this.UIStatus = WorkQueenState.Init;
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x060003F4 RID: 1012 RVA: 0x00010017 File Offset: 0x0000E217
		// (set) Token: 0x060003F5 RID: 1013 RVA: 0x0001001F File Offset: 0x0000E21F
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

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x060003F6 RID: 1014 RVA: 0x00010034 File Offset: 0x0000E234
		// (set) Token: 0x060003F7 RID: 1015 RVA: 0x0001003C File Offset: 0x0000E23C
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

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x060003F8 RID: 1016 RVA: 0x00010058 File Offset: 0x0000E258
		// (set) Token: 0x060003F9 RID: 1017 RVA: 0x00010060 File Offset: 0x0000E260
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

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x060003FA RID: 1018 RVA: 0x00010075 File Offset: 0x0000E275
		// (set) Token: 0x060003FB RID: 1019 RVA: 0x0001007D File Offset: 0x0000E27D
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

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x060003FC RID: 1020 RVA: 0x0001009E File Offset: 0x0000E29E
		// (set) Token: 0x060003FD RID: 1021 RVA: 0x000100A6 File Offset: 0x0000E2A6
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

		// Token: 0x060003FE RID: 1022 RVA: 0x000100D0 File Offset: 0x0000E2D0
		public async void ProcessingFiles()
		{
			GAManager.SendEvent("PPTToPDF", "BtnClick", "Count", 1L);
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
					PPTToPDFUCViewModel.<>c__DisplayClass70_0 CS$<>8__locals1 = new PPTToPDFUCViewModel.<>c__DisplayClass70_0();
					CS$<>8__locals1.<>4__this = this;
					this.UIStatus = WorkQueenState.Working;
					int selectCount = 0;
					CS$<>8__locals1.SuccCount = 0;
					List<Task> list = new List<Task>();
					using (IEnumerator<ToPDFFileItem> enumerator = this.FileList.Where((ToPDFFileItem f) => f.IsFileSelected.GetValueOrDefault()).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							PPTToPDFUCViewModel.<>c__DisplayClass70_1 CS$<>8__locals2 = new PPTToPDFUCViewModel.<>c__DisplayClass70_1();
							CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
							CS$<>8__locals2.it = enumerator.Current;
							int num = selectCount;
							selectCount = num + 1;
							Task task = Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
							{
								PPTToPDFUCViewModel.<>c__DisplayClass70_1.<<ProcessingFiles>b__3>d <<ProcessingFiles>b__3>d;
								<<ProcessingFiles>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
								<<ProcessingFiles>b__3>d.<>4__this = CS$<>8__locals2;
								<<ProcessingFiles>b__3>d.<>1__state = -1;
								<<ProcessingFiles>b__3>d.<>t__builder.Start<PPTToPDFUCViewModel.<>c__DisplayClass70_1.<<ProcessingFiles>b__3>d>(ref <<ProcessingFiles>b__3>d);
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
						GAManager.SendEvent("PPTToPDF", "HasFailed", "Count", 1L);
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
						GAManager.SendEvent("PPTToPDF", "AllSucc", "Count", 1L);
					}
				})).ConfigureAwait(false);
				this.SelectAllMergeFiles(false);
			}
		}

		// Token: 0x04000215 RID: 533
		private ToPDFFileItemCollection _toPDFItemList;

		// Token: 0x04000216 RID: 534
		private ToPDFFileItem _selectedItem;

		// Token: 0x04000217 RID: 535
		private string _OutputPath;

		// Token: 0x04000218 RID: 536
		private string _OutputFilename;

		// Token: 0x04000219 RID: 537
		private string _OutputFileFullName;

		// Token: 0x0400021A RID: 538
		private bool? _ViewFileInExplore = new bool?(ConfigManager.GetConvertViewFileInExplore());

		// Token: 0x0400021B RID: 539
		private WorkQueenState _UIStatus;

		// Token: 0x0400021C RID: 540
		private RelayCommand addOneFile;

		// Token: 0x0400021D RID: 541
		private RelayCommand clearFiles;

		// Token: 0x0400021E RID: 542
		private RelayCommand selectPath;

		// Token: 0x0400021F RID: 543
		private RelayCommand beginWorks;

		// Token: 0x04000220 RID: 544
		private RelayCommand<ToPDFFileItem> moveUpOneFile;

		// Token: 0x04000221 RID: 545
		private RelayCommand<ToPDFFileItem> moveDownFile;

		// Token: 0x04000222 RID: 546
		private RelayCommand<ToPDFFileItem> openInExplorer;

		// Token: 0x04000223 RID: 547
		private RelayCommand<ToPDFFileItem> openWithEditor;

		// Token: 0x04000224 RID: 548
		private RelayCommand<ToPDFFileItem> removeFromList;

		// Token: 0x04000225 RID: 549
		private RelayCommand updateItem;
	}
}
