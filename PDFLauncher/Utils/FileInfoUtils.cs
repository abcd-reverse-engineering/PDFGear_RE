using System;
using System.IO;
using CommonLib.Common;

namespace PDFLauncher.Utils
{
	// Token: 0x02000015 RID: 21
	public static class FileInfoUtils
	{
		// Token: 0x0600016E RID: 366 RVA: 0x000063E0 File Offset: 0x000045E0
		public static string GetLastOpenTime(string path)
		{
			string text;
			if (!LongPathFile.Exists(path, out text))
			{
				return "";
			}
			return File.GetLastAccessTime(text).ToString("yyyy-MM-dd HH:mm:ss");
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00006410 File Offset: 0x00004610
		public static string GetFileName(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return "";
			}
			return new FileInfo(path).Name;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000642C File Offset: 0x0000462C
		public static string GetFileSize(string FileName)
		{
			LongPathFile longPathFile = FileName;
			if (!longPathFile.IsExists)
			{
				return "";
			}
			long length = longPathFile.FileInfo.Length;
			double num = 1024.0;
			if (length <= 0L)
			{
				return "0 KB";
			}
			if ((double)length < num)
			{
				return "1 KB";
			}
			if ((double)length < Math.Pow(num, 2.0))
			{
				return ((double)length / num).ToString("f2") + " KB";
			}
			if ((double)length < Math.Pow(num, 3.0))
			{
				return ((double)length / Math.Pow(num, 2.0)).ToString("f2") + " MB";
			}
			if ((double)length < Math.Pow(num, 4.0))
			{
				return ((double)length / Math.Pow(num, 3.0)).ToString("f2") + " GB";
			}
			return ((double)length / Math.Pow(num, 4.0)).ToString("f2") + " TB";
		}
	}
}
