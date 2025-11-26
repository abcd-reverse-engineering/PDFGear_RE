using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CommonLib.Common;
using pdfconverter.Models;
using PDFKit.GenerateImagePdf;

namespace pdfconverter.Utils
{
	// Token: 0x02000041 RID: 65
	internal class PageService
	{
		// Token: 0x14000005 RID: 5
		// (add) Token: 0x060004FF RID: 1279 RVA: 0x00014850 File Offset: 0x00012A50
		// (remove) Token: 0x06000500 RID: 1280 RVA: 0x00014884 File Offset: 0x00012A84
		public static event Action<ToimagePage> PageAdded;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000501 RID: 1281 RVA: 0x000148B8 File Offset: 0x00012AB8
		// (remove) Token: 0x06000502 RID: 1282 RVA: 0x000148EC File Offset: 0x00012AEC
		public static event Action<double, string> ImportProgressChanged;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000503 RID: 1283 RVA: 0x00014920 File Offset: 0x00012B20
		// (remove) Token: 0x06000504 RID: 1284 RVA: 0x00014954 File Offset: 0x00012B54
		public static event Action<double, string> ExportProgressChanged;

		// Token: 0x06000505 RID: 1285 RVA: 0x00014987 File Offset: 0x00012B87
		public static void OnPageAdded(ToimagePage page)
		{
			Action<ToimagePage> pageAdded = PageService.PageAdded;
			if (pageAdded == null)
			{
				return;
			}
			pageAdded(page);
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x0001499C File Offset: 0x00012B9C
		private static void importimage(IEnumerable<string> filePaths)
		{
			List<FileInfo> list = new List<FileInfo>();
			foreach (string text in filePaths)
			{
				FileInfo fileInfo = new FileInfo(text);
				if (fileInfo.Exists && fileInfo.Length != 0L && fileInfo.Extension.Length != 0)
				{
					string text2 = fileInfo.Extension.ToLower();
					if (PageService.ImageExtensions.Contains(text2))
					{
						list.Add(fileInfo);
					}
				}
			}
			new ImagePdfGenerator();
			foreach (FileInfo fileInfo2 in list)
			{
				new ImagePdfGenerateItem(BitmapImageSource.CreateFromSystemDrawingBitmap(Image.FromFile(fileInfo2.FullName) as Bitmap, false));
			}
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x00014A7C File Offset: 0x00012C7C
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
						foreach (FileInfo fileInfo2 in imageFiles)
						{
							try
							{
								PageService.OnImportProgressChanged((double)count, (double)(++num), fileInfo2.FullName);
								Bitmap bitmap = Image.FromFile(fileInfo2.FullName) as Bitmap;
								if (bitmap != null && bitmap.Width > 0 && bitmap.Height > 0)
								{
									PageService.OnPageAdded(new ToimagePage(bitmap, fileInfo2.Name, fileInfo2.Extension));
								}
							}
							catch (Exception ex)
							{
								try
								{
									Bitmap bitmap2 = PageService.TryLoadBitmap(fileInfo2.FullName);
									if (bitmap2 != null && bitmap2.Width > 0 && bitmap2.Height > 0)
									{
										PageService.OnPageAdded(new ToimagePage(bitmap2, fileInfo2.Name, fileInfo2.Extension));
									}
									else
									{
										PageService.OnPageAdded(new ToimagePage(null, fileInfo2.Name, fileInfo2.Extension));
									}
								}
								catch
								{
									PageService.OnPageAdded(new ToimagePage(null, fileInfo2.Name, fileInfo2.Extension));
								}
								Log.WriteLog(ex.ToString());
							}
							if (token.IsCancellationRequested)
							{
								break;
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

		// Token: 0x06000508 RID: 1288 RVA: 0x00014B78 File Offset: 0x00012D78
		public static Bitmap TryLoadBitmap(string path)
		{
			try
			{
				LongPathFile longPathFile = path;
				if (!longPathFile.IsExists)
				{
					return null;
				}
				using (FileStream fileStream = new FileStream(longPathFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						BitmapDecoder bitmapDecoder = BitmapDecoder.Create(fileStream, BitmapCreateOptions.None, BitmapCacheOption.None);
						new PngBitmapEncoder
						{
							Frames = { bitmapDecoder.Frames[0] }
						}.Save(memoryStream);
						memoryStream.Position = 0L;
						using (Bitmap bitmap = new Bitmap(memoryStream))
						{
							return new Bitmap(bitmap);
						}
					}
				}
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x00014C54 File Offset: 0x00012E54
		private static void OnImportProgressChanged(double total, double count, string fileName)
		{
			Action<double, string> importProgressChanged = PageService.ImportProgressChanged;
			if (importProgressChanged == null)
			{
				return;
			}
			importProgressChanged(Math.Floor(count / total * 100.0), fileName);
		}

		// Token: 0x0600050A RID: 1290 RVA: 0x00014C78 File Offset: 0x00012E78
		private static void OnImportProgressChanged(string fileName)
		{
			Action<double, string> importProgressChanged = PageService.ImportProgressChanged;
			if (importProgressChanged == null)
			{
				return;
			}
			importProgressChanged(0.0, fileName);
		}

		// Token: 0x0600050B RID: 1291 RVA: 0x00014C93 File Offset: 0x00012E93
		private static void OnExportProgressChanged(double total, double count, string fileName)
		{
			Action<double, string> exportProgressChanged = PageService.ExportProgressChanged;
			if (exportProgressChanged == null)
			{
				return;
			}
			exportProgressChanged(Math.Floor(count / total * 100.0), fileName);
		}

		// Token: 0x0600050C RID: 1292 RVA: 0x00014CB8 File Offset: 0x00012EB8
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

		// Token: 0x04000277 RID: 631
		public static readonly string[] ImageExtensions = new string[] { ".bmp", ".emf", ".exif", ".gif", ".jpg", ".jpeg", ".png", ".tiff", ".tif" };
	}
}
