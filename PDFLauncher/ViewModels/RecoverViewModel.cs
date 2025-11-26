using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CommonLib.Common;
using CommonLib.Config.ConfigModels;
using PDFLauncher.Models;

namespace PDFLauncher.ViewModels
{
	// Token: 0x02000014 RID: 20
	public class RecoverViewModel : BindableBase
	{
		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x0600015B RID: 347 RVA: 0x00005E59 File Offset: 0x00004059
		// (set) Token: 0x0600015C RID: 348 RVA: 0x00005E61 File Offset: 0x00004061
		public ObservableCollection<RecoverFileItem> RecoverFileList { get; private set; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x0600015D RID: 349 RVA: 0x00005E6A File Offset: 0x0000406A
		// (set) Token: 0x0600015E RID: 350 RVA: 0x00005E72 File Offset: 0x00004072
		public RecoverFileItem SelectedRecoverFileItem
		{
			get
			{
				return this.selectedRecoverFileItem;
			}
			set
			{
				base.SetProperty<RecoverFileItem>(ref this.selectedRecoverFileItem, value, "SelectedRecoverFileItem");
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x0600015F RID: 351 RVA: 0x00005E87 File Offset: 0x00004087
		// (set) Token: 0x06000160 RID: 352 RVA: 0x00005E8F File Offset: 0x0000408F
		public string ReoverOutputPath
		{
			get
			{
				return this.recoverOutputPath;
			}
			set
			{
				base.SetProperty<string>(ref this.recoverOutputPath, value, "ReoverOutputPath");
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000161 RID: 353 RVA: 0x00005EA4 File Offset: 0x000040A4
		// (set) Token: 0x06000162 RID: 354 RVA: 0x00005EAC File Offset: 0x000040AC
		public int SelectedRecoveringCount
		{
			get
			{
				return this.selectedRecoveringCount;
			}
			set
			{
				base.SetProperty<int>(ref this.selectedRecoveringCount, value, "SelectedRecoveringCount");
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000163 RID: 355 RVA: 0x00005EC4 File Offset: 0x000040C4
		// (set) Token: 0x06000164 RID: 356 RVA: 0x00005F44 File Offset: 0x00004144
		public bool? IsAllRecoverFileSelected
		{
			get
			{
				if (this.RecoverFileList == null || this.RecoverFileList.Count <= 0)
				{
					return new bool?(false);
				}
				int count = this.RecoverFileList.Count;
				int num = this.RecoverFileList.Count((RecoverFileItem r) => r.IsFileSelected.GetValueOrDefault());
				if (num <= 0)
				{
					return new bool?(false);
				}
				if (count == num)
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
				this.SelectAllRecoverFiles(flag.Value);
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00005F74 File Offset: 0x00004174
		public void SelectAllRecoverFiles(bool bSelectAll)
		{
			if (this.RecoverFileList == null || this.RecoverFileList.Count<RecoverFileItem>() <= 0)
			{
				return;
			}
			foreach (RecoverFileItem recoverFileItem in this.RecoverFileList)
			{
				bool? isFileSelected = recoverFileItem.IsFileSelected;
				if (!((isFileSelected.GetValueOrDefault() == bSelectAll) & (isFileSelected != null)))
				{
					recoverFileItem.IsFileSelected = new bool?(bSelectAll);
				}
				if (recoverFileItem.Status == RecoverStatus.Converted)
				{
					recoverFileItem.IsFileSelected = new bool?(false);
				}
			}
			this.NotifyAllRecoverFilesSelectedChanged();
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00006018 File Offset: 0x00004218
		public void NotifyAllRecoverFilesSelectedChanged()
		{
			base.OnPropertyChanged("IsAllRecoverFileSelected");
			base.OnPropertyChanged("RecoverFileList");
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00006030 File Offset: 0x00004230
		public RecoverViewModel()
		{
			this.RecoverFileList = new ObservableCollection<RecoverFileItem>();
			this.RecoverFileList.CollectionChanged += this.RecoverFileList_CollectionChanged;
			this.ReoverOutputPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\PDFgear";
			this.LoadRecoverFiles();
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00006084 File Offset: 0x00004284
		private void RecoverFileList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (object obj in e.NewItems)
				{
					((RecoverFileItem)obj).PropertyChanged += this.RecoveFileItem_PropertyChanged;
				}
			}
			if (e.OldItems != null)
			{
				foreach (object obj2 in e.OldItems)
				{
					((RecoverFileItem)obj2).PropertyChanged -= this.RecoveFileItem_PropertyChanged;
				}
			}
			this.SelectedRecoveringCount = this.RecoverFileList.ToList<RecoverFileItem>().Count((RecoverFileItem r) => r.IsFileSelected.GetValueOrDefault() && r.Status == RecoverStatus.Prepare);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000617C File Offset: 0x0000437C
		private void RecoveFileItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsFileSelected" || e.PropertyName == "Status")
			{
				this.SelectedRecoveringCount = this.RecoverFileList.ToList<RecoverFileItem>().Count((RecoverFileItem r) => r.IsFileSelected.GetValueOrDefault() && r.Status == RecoverStatus.Prepare);
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x000061E4 File Offset: 0x000043E4
		private void LoadRecoverFiles()
		{
			this.RecoverFileList.Clear();
			if (Directory.Exists(RecoverViewModel.AutoSaveDir))
			{
				FileInfo[] files = new DirectoryInfo(RecoverViewModel.AutoSaveDir).GetFiles("*.data");
				if (files.Length == 0)
				{
					return;
				}
				foreach (FileInfo fileInfo in files)
				{
					int num = fileInfo.Name.LastIndexOf("_") + 1;
					int num2 = fileInfo.Name.LastIndexOf(".") - num;
					string text = fileInfo.Name.Substring(num, num2);
					if (!this.GetAutoSaveFileByUnUsing(fileInfo.Name))
					{
						AutoSaveFileModel result = ConfigManager.GetAutoSaveFileModelAsync(default(CancellationToken), text).GetAwaiter().GetResult();
						if (result != null)
						{
							this.RecoverFileList.Add(new RecoverFileItem
							{
								FileName = result.FileName,
								LastTime = result.LastSaveTime,
								Status = RecoverStatus.Prepare,
								FileGuid = result.Guid,
								IsFileSelected = new bool?(true),
								RecoverDir = "",
								SourceFullFileName = result.TempFileName,
								EditorSourceFullFileName = result.SoruceFileFullName,
								DisplayName = result.SoruceFileFullName
							});
						}
					}
				}
			}
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000632C File Offset: 0x0000452C
		public bool GetAutoSaveFileByUnUsing(string fileName)
		{
			List<AutoSaveFileModel> result = ConfigManager.GetAutoSaveFilesAsync(default(CancellationToken)).GetAwaiter().GetResult();
			List<Process> pdfeditorProcesss = RecoverViewModel.GetPdfeditorProcesss();
			if (result != null && pdfeditorProcesss != null)
			{
				for (int i = 0; i < result.Count; i++)
				{
					AutoSaveFileModel configFile = result[i];
					if (pdfeditorProcesss.Find((Process p) => p.Id == configFile.CreatePid) != null && fileName.Contains(configFile.Guid))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x000063B8 File Offset: 0x000045B8
		public static List<Process> GetPdfeditorProcesss()
		{
			return Process.GetProcessesByName("pdfeditor").ToList<Process>();
		}

		// Token: 0x040000B6 RID: 182
		public static string AutoSaveDir = Path.Combine(AppDataHelper.LocalFolder, "BackUp");

		// Token: 0x040000B8 RID: 184
		private RecoverFileItem selectedRecoverFileItem;

		// Token: 0x040000B9 RID: 185
		private string recoverOutputPath;

		// Token: 0x040000BA RID: 186
		private int selectedRecoveringCount;
	}
}
