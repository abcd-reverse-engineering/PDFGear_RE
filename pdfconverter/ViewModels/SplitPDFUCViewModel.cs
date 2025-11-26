using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
	// Token: 0x02000037 RID: 55
	public class SplitPDFUCViewModel : ObservableObject
	{
		// Token: 0x0600043B RID: 1083 RVA: 0x00010F3C File Offset: 0x0000F13C
		public SplitPDFUCViewModel()
		{
			try
			{
				this.SplitInitializeEnv();
			}
			catch
			{
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x00010F6C File Offset: 0x0000F16C
		public RelayCommand UpdateItem
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.updateItem) == null)
				{
					relayCommand = (this.updateItem = new RelayCommand(delegate
					{
						this.NotifyAllSplitFilesSelectedChanged();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x0600043D RID: 1085 RVA: 0x00010FA0 File Offset: 0x0000F1A0
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
								this.AddOneFileToSplitList(text, null);
							}
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x0600043E RID: 1086 RVA: 0x00010FD4 File Offset: 0x0000F1D4
		public RelayCommand ClearFiles
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.clearFiles) == null)
				{
					relayCommand = (this.clearFiles = new RelayCommand(delegate
					{
						if (this.SplitPDFList.Count((SplitFileItem f) => f.IsFileSelected.GetValueOrDefault()) == 0)
						{
							ModernMessageBox.Show(Resources.MainWinOthersToPDFDeleteNoFile, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							return;
						}
						if (ModernMessageBox.Show(Resources.WinMergeSplitClearFileAskMsg, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.Yes)
						{
							this.ClearAllSelectedSplitFileItems();
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x0600043F RID: 1087 RVA: 0x00011008 File Offset: 0x0000F208
		public RelayCommand SelectPath
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.selectPath) == null)
				{
					relayCommand = (this.selectPath = new RelayCommand(delegate
					{
						string text = ConvertManager.selectOutputFolder(this.SplitOutputPath);
						if (!string.IsNullOrWhiteSpace(text))
						{
							this.SplitOutputPath = text;
							ConfigManager.SetConvertPath(this.SplitOutputPath);
						}
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x0001103C File Offset: 0x0000F23C
		public RelayCommand BeginSplit
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.beginSplit) == null)
				{
					relayCommand = (this.beginSplit = new RelayCommand(delegate
					{
						this.DoSplitPDFFiles();
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06000441 RID: 1089 RVA: 0x00011070 File Offset: 0x0000F270
		public RelayCommand<SplitFileItem> OpenInExplorer
		{
			get
			{
				RelayCommand<SplitFileItem> relayCommand;
				if ((relayCommand = this.openInExplorer) == null)
				{
					relayCommand = (this.openInExplorer = new RelayCommand<SplitFileItem>(delegate(SplitFileItem model)
					{
						UtilsManager.OpenFolderInExplore(model.OutputPath);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x000110B4 File Offset: 0x0000F2B4
		public RelayCommand<SplitFileItem> RemoveFromList
		{
			get
			{
				RelayCommand<SplitFileItem> relayCommand;
				if ((relayCommand = this.removeFromList) == null)
				{
					relayCommand = (this.removeFromList = new RelayCommand<SplitFileItem>(delegate(SplitFileItem model)
					{
						this.RemoveOneSplitFileItem(model);
					}));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000443 RID: 1091 RVA: 0x000110E8 File Offset: 0x0000F2E8
		private void SplitInitializeEnv()
		{
			this._splitPDFList = new SplitFileItemCollection();
			this._splitViewFileInExplore = new bool?(ConfigManager.GetConvertViewFileInExplore());
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
				this.SplitOutputPath = text;
			}
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06000444 RID: 1092 RVA: 0x0001116C File Offset: 0x0000F36C
		public SplitFileItemCollection SplitPDFList
		{
			get
			{
				return this._splitPDFList;
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06000445 RID: 1093 RVA: 0x00011174 File Offset: 0x0000F374
		// (set) Token: 0x06000446 RID: 1094 RVA: 0x0001117C File Offset: 0x0000F37C
		public SplitFileItem SelectedSplitFileItem
		{
			get
			{
				return this._selectedSplitFileItem;
			}
			set
			{
				base.SetProperty<SplitFileItem>(ref this._selectedSplitFileItem, value, "SelectedSplitFileItem");
			}
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x00011194 File Offset: 0x0000F394
		private bool IsSplitFileHasBeenAdded(string fileName)
		{
			return !string.IsNullOrWhiteSpace(fileName) && this.SplitPDFList != null && this.SplitPDFList.Where((SplitFileItem f) => f.FilePath.Equals(fileName)).ToList<SplitFileItem>().Count<SplitFileItem>() > 0;
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x000111EC File Offset: 0x0000F3EC
		public int AddOneFileToSplitList(string fileName, string password = null)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(fileName) || this.SplitPDFList == null)
				{
					return 2;
				}
				LongPathFile longPathFile = fileName;
				fileName = longPathFile.FullPathWithoutPrefix;
				if (this.IsSplitFileHasBeenAdded(longPathFile))
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
				if (password == null)
				{
					password = "";
				}
				SplitFileItem splitFileItem = new SplitFileItem(longPathFile);
				if (splitFileItem != null)
				{
					if (UtilsManager.IsnotSupportFile(longPathFile, UtilManager.PDFExtentions))
					{
						splitFileItem.IsFileSelected = new bool?(false);
						splitFileItem.Status = SplitStatus.Unsupport;
						this.SplitPDFList.Add(splitFileItem);
					}
					else if (ConToPDFUtils.CheckPassword(longPathFile, ref password, Application.Current.MainWindow))
					{
						this.SplitPDFList.Add(splitFileItem);
						splitFileItem.parseFile(password);
					}
					else
					{
						splitFileItem.IsFileSelected = new bool?(false);
						splitFileItem.Status = SplitStatus.LoadedFailed;
						this.SplitPDFList.Add(splitFileItem);
					}
				}
				this.NotifyAllSplitFilesSelectedChanged();
				return 0;
			}
			catch
			{
			}
			return 1;
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x0001136C File Offset: 0x0000F56C
		private void RemoveOneSplitFileItem(SplitFileItem item)
		{
			try
			{
				if (item != null && this.SplitPDFList != null)
				{
					if (this.SplitPDFList.Contains(item))
					{
						this.SplitPDFList.Remove(item);
					}
					this.NotifyAllSplitFilesSelectedChanged();
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x000113BC File Offset: 0x0000F5BC
		private void ClearAllSelectedSplitFileItems()
		{
			try
			{
				if (this.SplitPDFList != null)
				{
					foreach (SplitFileItem splitFileItem in this.SplitPDFList.Where((SplitFileItem f) => f.IsFileSelected.GetValueOrDefault()).ToList<SplitFileItem>())
					{
						this.SplitPDFList.Remove(splitFileItem);
					}
					this.NotifyAllSplitFilesSelectedChanged();
				}
			}
			catch
			{
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x0600044B RID: 1099 RVA: 0x00011460 File Offset: 0x0000F660
		// (set) Token: 0x0600044C RID: 1100 RVA: 0x000114E0 File Offset: 0x0000F6E0
		public bool? IsAllSplitFilesSelected
		{
			get
			{
				if (this.SplitPDFList == null || this.SplitPDFList.Count<SplitFileItem>() <= 0)
				{
					return new bool?(false);
				}
				int num = this.SplitPDFList.Count<SplitFileItem>();
				int num2 = this.SplitPDFList.Count((SplitFileItem f) => f.IsFileSelected.GetValueOrDefault());
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
				this.SelectAllSplitFiles(flag.Value);
			}
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x00011510 File Offset: 0x0000F710
		private void SelectAllSplitFiles(bool bSelectAll)
		{
			if (this.SplitPDFList == null || this.SplitPDFList.Count<SplitFileItem>() <= 0)
			{
				return;
			}
			foreach (SplitFileItem splitFileItem in this.SplitPDFList)
			{
				if (splitFileItem.Status != SplitStatus.Unsupport)
				{
					bool? isFileSelected = splitFileItem.IsFileSelected;
					if (!((isFileSelected.GetValueOrDefault() == bSelectAll) & (isFileSelected != null)) && splitFileItem.Status != SplitStatus.LoadedFailed)
					{
						splitFileItem.IsFileSelected = new bool?(bSelectAll);
					}
				}
			}
			this.NotifyAllSplitFilesSelectedChanged();
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x000115B0 File Offset: 0x0000F7B0
		public void NotifyAllSplitFilesSelectedChanged()
		{
			base.OnPropertyChanged("IsAllSplitFilesSelected");
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x0600044F RID: 1103 RVA: 0x000115BD File Offset: 0x0000F7BD
		// (set) Token: 0x06000450 RID: 1104 RVA: 0x000115C5 File Offset: 0x0000F7C5
		public string SplitOutputPath
		{
			get
			{
				return this._splitOutputPath;
			}
			set
			{
				base.SetProperty<string>(ref this._splitOutputPath, value, "SplitOutputPath");
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000451 RID: 1105 RVA: 0x000115DA File Offset: 0x0000F7DA
		// (set) Token: 0x06000452 RID: 1106 RVA: 0x000115E2 File Offset: 0x0000F7E2
		public bool? SplitViewFileInExplore
		{
			get
			{
				return this._splitViewFileInExplore;
			}
			set
			{
				base.SetProperty<bool?>(ref this._splitViewFileInExplore, value, "SplitViewFileInExplore");
				ConfigManager.SetConvertViewFileInExplore(value.Value);
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000453 RID: 1107 RVA: 0x00011603 File Offset: 0x0000F803
		// (set) Token: 0x06000454 RID: 1108 RVA: 0x0001160B File Offset: 0x0000F80B
		public SplitPDFUIStatus SplitUIStatus
		{
			get
			{
				return this._splitUIStatus;
			}
			set
			{
				base.SetProperty<SplitPDFUIStatus>(ref this._splitUIStatus, value, "SplitUIStatus");
			}
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x00011620 File Offset: 0x0000F820
		public async void DoSplitPDFFiles()
		{
			GAManager.SendEvent("SplitPDF", "BtnClick", "Count", 1L);
			if (!IAPHelper.IsPaidUser && !ConfigManager.GetTest())
			{
				IAPHelper.ShowActivationWindow("SplitPDF", ".convert");
			}
			else if (this.SplitPDFList.Where((SplitFileItem f) => f.IsFileSelected.GetValueOrDefault()).Count<SplitFileItem>() <= 0)
			{
				ModernMessageBox.Show(Resources.WinMergeSplitSplitFileCheckEmptyFileMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else if (string.IsNullOrWhiteSpace(this.SplitOutputPath))
			{
				ModernMessageBox.Show(Resources.WinConvertSetOutputFolderText, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else
			{
				foreach (SplitFileItem splitFileItem in this.SplitPDFList.Where((SplitFileItem f) => f.IsFileSelected.GetValueOrDefault()))
				{
					if (string.IsNullOrWhiteSpace(splitFileItem.PageSplitModeStr))
					{
						this.SelectedSplitFileItem = splitFileItem;
						ModernMessageBox.Show(Resources.WinMergeSplitSplitFileCheckModeMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						return;
					}
					bool flag = false;
					if (splitFileItem.PageSplitMode == 0)
					{
						int[][] array;
						int num;
						if (!PageRangeHelper.TryParsePageRange2(splitFileItem.PageSplitModeStr, out array, out num))
						{
							ModernMessageBox.Show(Resources.WinMergeSplitSplitFileCheckpageRangeMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							return;
						}
					}
					else if (splitFileItem.PageSplitMode == 1)
					{
						if (new Regex("^\\d+$").IsMatch(splitFileItem.PageSplitModeStr))
						{
							int num2 = Convert.ToInt32(splitFileItem.PageSplitModeStr);
							if (num2 <= 0 || num2 >= splitFileItem.PageCount)
							{
								flag = true;
							}
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						this.SelectedSplitFileItem = splitFileItem;
						ModernMessageBox.Show(Resources.WinMergeSplitSplitFileCheckpageRangeMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
						return;
					}
				}
				if (!Directory.Exists(this.SplitOutputPath))
				{
					Directory.CreateDirectory(this.SplitOutputPath);
				}
				await Task.Run(TaskExceptionHelper.ExceptionBoundary(async delegate
				{
					SplitPDFUCViewModel.<>c__DisplayClass51_0 CS$<>8__locals1 = new SplitPDFUCViewModel.<>c__DisplayClass51_0();
					CS$<>8__locals1.<>4__this = this;
					this.SplitUIStatus = SplitPDFUIStatus.Spliting;
					int selectCount = 0;
					CS$<>8__locals1.SuccCount = 0;
					List<Task> list = new List<Task>();
					using (IEnumerator<SplitFileItem> enumerator2 = this.SplitPDFList.Where((SplitFileItem f) => f.IsFileSelected.GetValueOrDefault()).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							SplitPDFUCViewModel.<>c__DisplayClass51_1 CS$<>8__locals2 = new SplitPDFUCViewModel.<>c__DisplayClass51_1();
							CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
							CS$<>8__locals2.it = enumerator2.Current;
							int num3 = selectCount;
							selectCount = num3 + 1;
							Task task = Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
							{
								SplitPDFUCViewModel.<>c__DisplayClass51_1.<<DoSplitPDFFiles>b__4>d <<DoSplitPDFFiles>b__4>d;
								<<DoSplitPDFFiles>b__4>d.<>t__builder = AsyncTaskMethodBuilder.Create();
								<<DoSplitPDFFiles>b__4>d.<>4__this = CS$<>8__locals2;
								<<DoSplitPDFFiles>b__4>d.<>1__state = -1;
								<<DoSplitPDFFiles>b__4>d.<>t__builder.Start<SplitPDFUCViewModel.<>c__DisplayClass51_1.<<DoSplitPDFFiles>b__4>d>(ref <<DoSplitPDFFiles>b__4>d);
								return <<DoSplitPDFFiles>b__4>d.<>t__builder.Task;
							}));
							list.Add(task);
						}
					}
					await Task.WhenAll(list).ConfigureAwait(true);
					this.SplitUIStatus = SplitPDFUIStatus.Complete;
					this.NotifyAllSplitFilesSelectedChanged();
					if (selectCount != CS$<>8__locals1.SuccCount)
					{
						GAManager.SendEvent("SplitPDF", "HasFailed", "Count", 1L);
						if (MessageBox.Show(Resources.WinMergeSplitSplitFileFailMsg, UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
						{
							Application.Current.Dispatcher.Invoke(delegate
							{
								FeedbackWindow feedbackWindow = new FeedbackWindow();
								if (feedbackWindow != null)
								{
									try
									{
										foreach (SplitFileItem splitFileItem2 in this.SplitPDFList)
										{
											if (splitFileItem2.IsFileSelected.GetValueOrDefault() && splitFileItem2.Status == SplitStatus.Fail)
											{
												feedbackWindow.flist.Add(splitFileItem2.FilePath);
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
						GAManager.SendEvent("SplitPDF", "AllSucc", "Count", 1L);
					}
				})).ConfigureAwait(false);
			}
		}

		// Token: 0x04000237 RID: 567
		private SplitFileItemCollection _splitPDFList;

		// Token: 0x04000238 RID: 568
		private SplitFileItem _selectedSplitFileItem;

		// Token: 0x04000239 RID: 569
		private string _splitOutputPath;

		// Token: 0x0400023A RID: 570
		private bool? _splitViewFileInExplore;

		// Token: 0x0400023B RID: 571
		private SplitPDFUIStatus _splitUIStatus;

		// Token: 0x0400023C RID: 572
		private RelayCommand addOneFile;

		// Token: 0x0400023D RID: 573
		private RelayCommand clearFiles;

		// Token: 0x0400023E RID: 574
		private RelayCommand selectPath;

		// Token: 0x0400023F RID: 575
		private RelayCommand beginSplit;

		// Token: 0x04000240 RID: 576
		private RelayCommand<SplitFileItem> openInExplorer;

		// Token: 0x04000241 RID: 577
		private RelayCommand<SplitFileItem> removeFromList;

		// Token: 0x04000242 RID: 578
		private RelayCommand updateItem;
	}
}
