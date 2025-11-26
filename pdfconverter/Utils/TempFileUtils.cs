using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommonLib.Common;

namespace pdfconverter.Utils
{
	// Token: 0x02000044 RID: 68
	public static class TempFileUtils
	{
		// Token: 0x06000518 RID: 1304 RVA: 0x00015054 File Offset: 0x00013254
		public static async Task<string> SplitFileInRangeAsync(string Filepath, int from, int to)
		{
			if (!Directory.Exists(TempFileUtils.TempFolder))
			{
				Directory.CreateDirectory(TempFileUtils.TempFolder);
			}
			string text = string.Format("{0}-{1}", from, to);
			CancellationToken cancellationToken;
			PdfiumNetHelper.SplitResult splitResult = await PdfiumNetHelper.SplitByRangeAsync(Filepath, null, TempFileUtils.TempFolder, text, null, cancellationToken);
			string text3;
			if (splitResult.Success)
			{
				string text2 = splitResult.OutputFiles.FirstOrDefault<string>();
				text3 = Path.Combine(TempFileUtils.TempFolder, text2);
			}
			else
			{
				text3 = "";
			}
			return text3;
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x000150A8 File Offset: 0x000132A8
		public static string GetTempFileName(string Filepath, int from, int to)
		{
			if (!Directory.Exists(TempFileUtils.TempFolder))
			{
				Directory.CreateDirectory(TempFileUtils.TempFolder);
			}
			string text = string.Format("{0}-{1}", from, to);
			string text2 = Path.Combine(TempFileUtils.TempFolder, Path.GetFileNameWithoutExtension(Filepath).Trim() + " [" + text + "].pdf");
			return Path.Combine(TempFileUtils.TempFolder, text2);
		}

		// Token: 0x04000278 RID: 632
		private static readonly string TempFolder = Path.Combine(AppDataHelper.TemporaryFolder, "ConvertOnline");
	}
}
