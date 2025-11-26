using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Win32;
using PDFLauncher.Models;
using PDFLauncher.Properties;
using PDFLauncher.ViewModels;

namespace PDFLauncher.Utils
{
	// Token: 0x02000019 RID: 25
	public static class FileManager
	{
		// Token: 0x06000179 RID: 377 RVA: 0x000066AC File Offset: 0x000048AC
		public static string SelectFileForOpen()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = false;
			openFileDialog.Filter = AppManager.GetOpenFileFilter();
			if (openFileDialog.ShowDialog().GetValueOrDefault())
			{
				return openFileDialog.FileName;
			}
			return null;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x000066EC File Offset: 0x000048EC
		public static string SelectPDFFileForOpen()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = false;
			openFileDialog.Filter = "PDF documents|*.pdf";
			if (openFileDialog.ShowDialog().GetValueOrDefault())
			{
				return openFileDialog.FileName;
			}
			return null;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000672C File Offset: 0x0000492C
		public static bool OpenOneFile(string file, string action = null)
		{
			if (string.IsNullOrWhiteSpace(file))
			{
				return false;
			}
			LongPathFile longPathFile = file;
			if (!longPathFile.IsExists)
			{
				ModernMessageBox.Show(Resources.OpenFileNoExistMsg + file.FullPathWithoutPrefix, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				OpenHistoryModel openHistoryModel = requiredService.HistoryModels.FirstOrDefault((OpenHistoryModel t) => t.FilePath == file);
				if (openHistoryModel != null)
				{
					requiredService.RemoveOne.Execute(openHistoryModel);
				}
				return false;
			}
			if (!AppManager.IsSupportFileType(file))
			{
				MessageBox.Show(Resources.OpenFileNoSupporttypeMsg + file, UtilManager.GetProductName());
				return false;
			}
			if (new FileInfo(file).Extension.ToLower().Equals(".pdf"))
			{
				AppManager.OpenEditor(longPathFile, action);
			}
			return true;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00006824 File Offset: 0x00004A24
		public static void OpenFolderInExplore(string folder)
		{
			if (string.IsNullOrWhiteSpace(folder))
			{
				return;
			}
			try
			{
				char[] array = new char[] { '\\', '/', ' ' };
				string text = folder.TrimEnd(array);
				Process.Start("explorer.exe", text);
			}
			catch
			{
			}
		}
	}
}
