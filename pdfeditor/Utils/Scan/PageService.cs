using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Common;
using pdfeditor.Models.Scan;
using pdfeditor.Properties;

namespace pdfeditor.Utils.Scan
{
	// Token: 0x020000B6 RID: 182
	internal class PageService
	{
		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000AFE RID: 2814 RVA: 0x00038D6C File Offset: 0x00036F6C
		// (remove) Token: 0x06000AFF RID: 2815 RVA: 0x00038DA0 File Offset: 0x00036FA0
		public static event Action<ScannedPage> PageAdded;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000B00 RID: 2816 RVA: 0x00038DD4 File Offset: 0x00036FD4
		// (remove) Token: 0x06000B01 RID: 2817 RVA: 0x00038E08 File Offset: 0x00037008
		public static event Action<double, string> ImportProgressChanged;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000B02 RID: 2818 RVA: 0x00038E3C File Offset: 0x0003703C
		// (remove) Token: 0x06000B03 RID: 2819 RVA: 0x00038E70 File Offset: 0x00037070
		public static event Action<double, string> ExportProgressChanged;

		// Token: 0x06000B04 RID: 2820 RVA: 0x00038EA3 File Offset: 0x000370A3
		public static void OnPageAdded(ScannedPage page)
		{
			Action<ScannedPage> pageAdded = PageService.PageAdded;
			if (pageAdded == null)
			{
				return;
			}
			pageAdded(page);
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x00038EB8 File Offset: 0x000370B8
		public static Task Import(IEnumerable<string> filePaths, CancellationToken token)
		{
			if (filePaths == null || !filePaths.Any<string>())
			{
				return Task.CompletedTask;
			}
			List<FileInfo> imageFiles = new List<FileInfo>();
			foreach (string text in filePaths)
			{
				FileInfo fileInfo = text.FileInfo;
				if (fileInfo.Exists && fileInfo.Length != 0L && fileInfo.Extension.Length != 0)
				{
					string text2 = fileInfo.Extension.ToLower();
					if (PageService.ImageExtensions.Contains(text2))
					{
						imageFiles.Add(fileInfo);
					}
				}
			}
			if (imageFiles.Any<FileInfo>())
			{
				PageService.OnImportProgressChanged(imageFiles.First<FileInfo>().FullName);
				return Task.Run(delegate
				{
					try
					{
						int count = imageFiles.Count;
						int num = 0;
						using (List<FileInfo>.Enumerator enumerator2 = imageFiles.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								FileInfo file = enumerator2.Current;
								try
								{
									PageService.OnImportProgressChanged((double)count, (double)(++num), file.FullName);
									Bitmap bitmap = Image.FromFile(file.FullName) as Bitmap;
									if (bitmap != null && bitmap.Width > 0 && bitmap.Height > 0)
									{
										PageService.OnPageAdded(new ScannedPage(bitmap, file.Extension));
									}
								}
								catch (Exception)
								{
									ScannerDispatcherHelper.Invoke(delegate
									{
										ModernMessageBox.Show(Resources.ScannerWinAddFailed.Replace("XXX", file.Name), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
									});
								}
								if (token.IsCancellationRequested)
								{
									break;
								}
							}
						}
					}
					catch (TaskCanceledException)
					{
					}
					catch (Exception)
					{
					}
				});
			}
			return Task.CompletedTask;
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x00038FB4 File Offset: 0x000371B4
		private static void OnImportProgressChanged(double total, double count, string fileName)
		{
			Action<double, string> importProgressChanged = PageService.ImportProgressChanged;
			if (importProgressChanged == null)
			{
				return;
			}
			importProgressChanged(Math.Floor(count / total * 100.0), fileName);
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x00038FD8 File Offset: 0x000371D8
		private static void OnImportProgressChanged(string fileName)
		{
			Action<double, string> importProgressChanged = PageService.ImportProgressChanged;
			if (importProgressChanged == null)
			{
				return;
			}
			importProgressChanged(0.0, fileName);
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x00038FF3 File Offset: 0x000371F3
		private static void OnExportProgressChanged(double total, double count, string fileName)
		{
			Action<double, string> exportProgressChanged = PageService.ExportProgressChanged;
			if (exportProgressChanged == null)
			{
				return;
			}
			exportProgressChanged(Math.Floor(count / total * 100.0), fileName);
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x00039018 File Offset: 0x00037218
		private static string GetNextFileName(string folder, string fileNameFormat, ref int index)
		{
			string text;
			for (;;)
			{
				text = Path.Combine(folder, string.Format(fileNameFormat, index));
				if (!File.Exists(text))
				{
					break;
				}
				index++;
			}
			return text;
		}

		// Token: 0x040004C8 RID: 1224
		public static readonly string[] ImageExtensions = new string[] { ".bmp", ".emf", ".exif", ".gif", ".jpg", ".jpeg", ".png", ".tiff", ".tif" };
	}
}
