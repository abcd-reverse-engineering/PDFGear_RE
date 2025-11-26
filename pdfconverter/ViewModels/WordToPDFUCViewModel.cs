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
	// Token: 0x02000039 RID: 57
	public class WordToPDFUCViewModel : ObservableObject
	{
		// Token: 0x0600048E RID: 1166 RVA: 0x00012414 File Offset: 0x00010614
		public WordToPDFUCViewModel()
		{
			this.InitializeEnv();
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x00012440 File Offset: 0x00010640
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
			if (App.convertType.Equals(ConvToPDFType.WordToPDF) && App.selectedFile != null && App.selectedFile.Length != 0)
			{
				foreach (string text2 in App.selectedFile)
				{
					this.AddOneFileToFileList(text2);
				}
			}
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000490 RID: 1168 RVA: 0x000124EC File Offset: 0x000106EC
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

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000491 RID: 1169 RVA: 0x00012511 File Offset: 0x00010711
		// (set) Token: 0x06000492 RID: 1170 RVA: 0x00012519 File Offset: 0x00010719
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

		// Token: 0x06000493 RID: 1171 RVA: 0x00012530 File Offset: 0x00010730
		private bool IsFileHasBeenAdded(string fileName)
		{
			return !string.IsNullOrWhiteSpace(fileName) && this.FileList != null && this.FileList.Where((ToPDFFileItem f) => f.FilePath.Equals(fileName)).ToList<ToPDFFileItem>().Count<ToPDFFileItem>() > 0;
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x00012588 File Offset: 0x00010788
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
				ToPDFFileItem toPDFFileItem = new ToPDFFileItem(longPathFile, ConvToPDFType.WordToPDF);
				if (toPDFFileItem != null)
				{
					string text = "";
					if (UtilsManager.IsnotSupportFile(longPathFile, UtilManager.WordExtentions))
					{
						toPDFFileItem.Status = ToPDFItemStatus.Unsupport;
						toPDFFileItem.IsFileSelected = new bool?(false);
						toPDFFileItem.IsEnable = new bool?(false);
						this.FileList.Add(toPDFFileItem);
					}
					else if (ConToPDFUtils.CheckWordPassword(longPathFile, out text, Application.Current.MainWindow))
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

		// Token: 0x06000495 RID: 1173 RVA: 0x00012774 File Offset: 0x00010974
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

		// Token: 0x06000496 RID: 1174 RVA: 0x000127C4 File Offset: 0x000109C4
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

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06000497 RID: 1175 RVA: 0x00012840 File Offset: 0x00010A40
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

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06000498 RID: 1176 RVA: 0x00012874 File Offset: 0x00010A74
		public RelayCommand AddOneFile
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.addOneFile) == null)
				{
					relayCommand = (this.addOneFile = new RelayCommand(delegate
					{
						string[] array = ConvertManager.selectMultiFiles("Word document", ".docx;*.doc;");
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

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x06000499 RID: 1177 RVA: 0x000128A8 File Offset: 0x00010AA8
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

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x0600049A RID: 1178 RVA: 0x000128DC File Offset: 0x00010ADC
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

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x0600049B RID: 1179 RVA: 0x00012910 File Offset: 0x00010B10
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

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x0600049C RID: 1180 RVA: 0x00012944 File Offset: 0x00010B44
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

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x0600049D RID: 1181 RVA: 0x00012978 File Offset: 0x00010B78
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

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x0600049E RID: 1182 RVA: 0x000129AC File Offset: 0x00010BAC
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

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x0600049F RID: 1183 RVA: 0x000129E0 File Offset: 0x00010BE0
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

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x060004A0 RID: 1184 RVA: 0x00012A14 File Offset: 0x00010C14
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

		// Token: 0x060004A1 RID: 1185 RVA: 0x00012A48 File Offset: 0x00010C48
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

		// Token: 0x060004A2 RID: 1186 RVA: 0x00012AD0 File Offset: 0x00010CD0
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

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x060004A3 RID: 1187 RVA: 0x00012B74 File Offset: 0x00010D74
		// (set) Token: 0x060004A4 RID: 1188 RVA: 0x00012BF4 File Offset: 0x00010DF4
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

		// Token: 0x060004A5 RID: 1189 RVA: 0x00012C24 File Offset: 0x00010E24
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

		// Token: 0x060004A6 RID: 1190 RVA: 0x00012CC4 File Offset: 0x00010EC4
		public void NotifyAllMergeFilesSelectedChanged()
		{
			base.OnPropertyChanged("IsAllMergeFilesSelected");
			base.OnPropertyChanged("FileList");
			this.UIStatus = WorkQueenState.Init;
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x060004A7 RID: 1191 RVA: 0x00012CE3 File Offset: 0x00010EE3
		// (set) Token: 0x060004A8 RID: 1192 RVA: 0x00012CEB File Offset: 0x00010EEB
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

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x060004A9 RID: 1193 RVA: 0x00012D00 File Offset: 0x00010F00
		// (set) Token: 0x060004AA RID: 1194 RVA: 0x00012D08 File Offset: 0x00010F08
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

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x060004AB RID: 1195 RVA: 0x00012D24 File Offset: 0x00010F24
		// (set) Token: 0x060004AC RID: 1196 RVA: 0x00012D2C File Offset: 0x00010F2C
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

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x060004AD RID: 1197 RVA: 0x00012D41 File Offset: 0x00010F41
		// (set) Token: 0x060004AE RID: 1198 RVA: 0x00012D49 File Offset: 0x00010F49
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

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x060004AF RID: 1199 RVA: 0x00012D6A File Offset: 0x00010F6A
		// (set) Token: 0x060004B0 RID: 1200 RVA: 0x00012D72 File Offset: 0x00010F72
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

		// Token: 0x060004B1 RID: 1201 RVA: 0x00012D9C File Offset: 0x00010F9C
		public async void ProcessingFiles()
		{
			GAManager.SendEvent("WordToPDF", "BtnClick", "Count", 1L);
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
					WordToPDFUCViewModel.<>c__DisplayClass72_0 CS$<>8__locals1 = new WordToPDFUCViewModel.<>c__DisplayClass72_0();
					CS$<>8__locals1.<>4__this = this;
					this.UIStatus = WorkQueenState.Working;
					int selectCount = 0;
					CS$<>8__locals1.SuccCount = 0;
					List<Task> list = new List<Task>();
					using (IEnumerator<ToPDFFileItem> enumerator = this.FileList.Where((ToPDFFileItem f) => f.IsFileSelected.GetValueOrDefault()).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							WordToPDFUCViewModel.<>c__DisplayClass72_1 CS$<>8__locals2 = new WordToPDFUCViewModel.<>c__DisplayClass72_1();
							CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
							CS$<>8__locals2.it = enumerator.Current;
							int num = selectCount;
							selectCount = num + 1;
							Task task = Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
							{
								WordToPDFUCViewModel.<>c__DisplayClass72_1.<<ProcessingFiles>b__3>d <<ProcessingFiles>b__3>d;
								<<ProcessingFiles>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
								<<ProcessingFiles>b__3>d.<>4__this = CS$<>8__locals2;
								<<ProcessingFiles>b__3>d.<>1__state = -1;
								<<ProcessingFiles>b__3>d.<>t__builder.Start<WordToPDFUCViewModel.<>c__DisplayClass72_1.<<ProcessingFiles>b__3>d>(ref <<ProcessingFiles>b__3>d);
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
						GAManager.SendEvent("WordToPDF", "HasFailed", "Count", 1L);
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
						GAManager.SendEvent("WordToPDF", "AllSucc", "Count", 1L);
					}
				})).ConfigureAwait(false);
				this.SelectAllMergeFiles(false);
			}
		}

		// Token: 0x04000254 RID: 596
		private readonly string wordExtention = ".docx;.doc;";

		// Token: 0x04000255 RID: 597
		private ToPDFFileItemCollection _toPDFItemList;

		// Token: 0x04000256 RID: 598
		private ToPDFFileItem _selectedItem;

		// Token: 0x04000257 RID: 599
		private string _OutputPath;

		// Token: 0x04000258 RID: 600
		private string _OutputFilename;

		// Token: 0x04000259 RID: 601
		private string _OutputFileFullName;

		// Token: 0x0400025A RID: 602
		private bool? _ViewFileInExplore = new bool?(ConfigManager.GetConvertViewFileInExplore());

		// Token: 0x0400025B RID: 603
		private WorkQueenState _UIStatus;

		// Token: 0x0400025C RID: 604
		private RelayCommand addOneFile;

		// Token: 0x0400025D RID: 605
		private RelayCommand clearFiles;

		// Token: 0x0400025E RID: 606
		private RelayCommand selectPath;

		// Token: 0x0400025F RID: 607
		private RelayCommand beginWorks;

		// Token: 0x04000260 RID: 608
		private RelayCommand updateItem;

		// Token: 0x04000261 RID: 609
		private RelayCommand<ToPDFFileItem> moveUpOneFile;

		// Token: 0x04000262 RID: 610
		private RelayCommand<ToPDFFileItem> moveDownFile;

		// Token: 0x04000263 RID: 611
		private RelayCommand<ToPDFFileItem> openInExplorer;

		// Token: 0x04000264 RID: 612
		private RelayCommand<ToPDFFileItem> openFileReader;

		// Token: 0x04000265 RID: 613
		private RelayCommand<ToPDFFileItem> openWithEditor;

		// Token: 0x04000266 RID: 614
		private RelayCommand<ToPDFFileItem> removeFromList;
	}
}
