using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using PDFLauncher.CustomControl;
using PDFLauncher.Models;
using PDFLauncher.Utils;
using PDFLauncher.ViewModels;

namespace PDFLauncher
{
	// Token: 0x0200000B RID: 11
	public partial class RecoverWindow : Window
	{
		// Token: 0x06000025 RID: 37 RVA: 0x0000244C File Offset: 0x0000064C
		public RecoverWindow()
		{
			base.DataContext = Ioc.Default.GetRequiredService<RecoverViewModel>();
			this.InitializeComponent();
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000026 RID: 38 RVA: 0x0000246A File Offset: 0x0000066A
		public RecoverViewModel VM
		{
			get
			{
				return base.DataContext as RecoverViewModel;
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002477 File Offset: 0x00000677
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("RecoverWindow", "Show", "Count", 1L);
		}

		// Token: 0x06000028 RID: 40 RVA: 0x0000248F File Offset: 0x0000068F
		private void RecoverView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002494 File Offset: 0x00000694
		private async void StartBtn_Click(object sender, RoutedEventArgs e)
		{
			List<RecoverFileItem> list = this.VM.RecoverFileList.ToList<RecoverFileItem>().FindAll((RecoverFileItem r) => r.IsFileSelected.GetValueOrDefault());
			string reoverOutputPath = this.VM.ReoverOutputPath;
			if (!reoverOutputPath.IsExists)
			{
				reoverOutputPath.DirectoryInfo.Create();
			}
			FileInfo[] files = new DirectoryInfo(RecoverViewModel.AutoSaveDir).GetFiles();
			GAManager.SendEvent("RecoverWindow", "Start", "Count", 1L);
			for (int i = 0; i < list.Count; i++)
			{
				RecoverFileItem recoverItem = list[i];
				recoverItem.Status = RecoverStatus.Recovering;
				FileInfo fileInfo = files.ToList<FileInfo>().Find((FileInfo x) => x.FullName.Equals(recoverItem.SourceFullFileName, StringComparison.OrdinalIgnoreCase));
				if (fileInfo != null)
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(recoverItem.FileName);
					string text = ".pdf";
					string text3;
					do
					{
						string text2 = fileNameWithoutExtension + " - " + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + text;
						text3 = Path.Combine(reoverOutputPath, text2);
					}
					while (File.Exists(text3));
					try
					{
						recoverItem.RecoverFullFileName = text3;
						File.Copy(fileInfo, recoverItem.RecoverFullFileName);
						fileInfo.Delete();
					}
					catch (PathTooLongException)
					{
						recoverItem.RecoverFullFileName = Path.Combine(reoverOutputPath, Guid.NewGuid().ToString("N").ToLower() + ".pdf");
						File.Copy(fileInfo, recoverItem.RecoverFullFileName);
						fileInfo.Delete();
					}
					catch (DirectoryNotFoundException)
					{
						recoverItem.RecoverFullFileName = Path.Combine(reoverOutputPath, Guid.NewGuid().ToString("N").ToLower() + ".pdf");
						File.Copy(fileInfo, recoverItem.RecoverFullFileName);
						fileInfo.Delete();
					}
					catch (Exception ex)
					{
						GAManager.SendEvent("Exception", "Recover", ex.GetType().Name + ", " + ex.Message, 1L);
					}
					ConfigManager.DelAutoSaveFileAsync(recoverItem.FileGuid, null);
					recoverItem.Status = RecoverStatus.Converted;
					recoverItem.IsFileSelected = new bool?(false);
					recoverItem.RecoverDir = reoverOutputPath;
					recoverItem.DisplayName = recoverItem.RecoverFullFileName;
				}
				else
				{
					recoverItem.Status = RecoverStatus.Prepare;
					recoverItem.IsFileSelected = new bool?(false);
				}
			}
			if (list.Where((RecoverFileItem x) => x.Status == RecoverStatus.Converted).Count<RecoverFileItem>() > 0)
			{
				await ExplorerUtils.OpenFolderAsync(reoverOutputPath, (from x in list
					where x.Status == RecoverStatus.Converted
					select x.RecoverFullFileName).ToArray<string>(), default(CancellationToken));
			}
			if (this.VM.RecoverFileList.Count((RecoverFileItem x) => x.Status == RecoverStatus.Converted) == this.VM.RecoverFileList.Count)
			{
				base.Close();
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000024CC File Offset: 0x000006CC
		private void DiscardBtn_Click(object sender, RoutedEventArgs e)
		{
			Discardconfirm discardconfirm = new Discardconfirm();
			discardconfirm.ShowDialog();
			if (discardconfirm.DialogResult.GetValueOrDefault())
			{
				GAManager.SendEvent("RecoverWindow", "Discard", "Count", 1L);
				List<RecoverFileItem> list = this.VM.RecoverFileList.ToList<RecoverFileItem>().FindAll((RecoverFileItem r) => r.IsFileSelected.GetValueOrDefault());
				FileInfo[] files = new DirectoryInfo(RecoverViewModel.AutoSaveDir).GetFiles();
				for (int i = 0; i < files.Length; i++)
				{
					FileInfo f = files[i];
					RecoverFileItem recoverFileItem = list.Find((RecoverFileItem x) => x.SourceFullFileName.Equals(f.FullName, StringComparison.OrdinalIgnoreCase));
					if (recoverFileItem != null)
					{
						f.Delete();
						ConfigManager.DelAutoSaveFileAsync(recoverFileItem.FileGuid, null);
						this.VM.RecoverFileList.Remove(recoverFileItem);
					}
				}
				if (this.VM.RecoverFileList.Count((RecoverFileItem x) => x.Status == RecoverStatus.Converted) == this.VM.RecoverFileList.Count)
				{
					base.Close();
				}
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002607 File Offset: 0x00000807
		private void recoverFileItemCB_Checked(object sender, RoutedEventArgs e)
		{
			this.VM.NotifyAllRecoverFilesSelectedChanged();
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002614 File Offset: 0x00000814
		private void recoverFileItemCB_Unchecked(object sender, RoutedEventArgs e)
		{
			this.VM.NotifyAllRecoverFilesSelectedChanged();
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002624 File Offset: 0x00000824
		private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Button button = sender as Button;
				if (button != null)
				{
					object tag = button.Tag;
					if (tag != null && !string.IsNullOrEmpty(tag.ToString()))
					{
						FileManager.OpenOneFile(Path.Combine(this.VM.ReoverOutputPath, tag.ToString()), null);
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002684 File Offset: 0x00000884
		private async void OpenDirBtn_Click(object sender, RoutedEventArgs e)
		{
			object tag = ((Button)sender).Tag;
			if (tag != null && !string.IsNullOrEmpty(tag.ToString()))
			{
				await ExplorerUtils.SelectItemInExplorerAsync(tag.ToString(), default(CancellationToken));
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000279C File Offset: 0x0000099C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 3:
				((CheckBox)target).Checked += this.recoverFileItemCB_Checked;
				((CheckBox)target).Unchecked += this.recoverFileItemCB_Unchecked;
				return;
			case 4:
				((Button)target).Click += this.OpenFileBtn_Click;
				return;
			case 5:
				((Button)target).Click += this.OpenDirBtn_Click;
				return;
			default:
				return;
			}
		}
	}
}
