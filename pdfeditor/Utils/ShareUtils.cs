using System;
using System.Threading;
using System.Threading.Tasks;
using CommonLib.Common;
using pdfeditor.Utils.Email;

namespace pdfeditor.Utils
{
	// Token: 0x020000A3 RID: 163
	public static class ShareUtils
	{
		// Token: 0x06000A22 RID: 2594 RVA: 0x000337D8 File Offset: 0x000319D8
		public static async Task ShowInFolderAsync(string file)
		{
			try
			{
				GAManager.SendEvent("FileShare", "ShareInFolder", "Count", 1L);
				LongPathFile longPathFile = file;
				if (longPathFile.IsExists)
				{
					await longPathFile.ShowInExplorerAsync(default(CancellationToken));
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x0003381C File Offset: 0x00031A1C
		public static async Task WindowsShareAsync(string file)
		{
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x00033858 File Offset: 0x00031A58
		public static async Task SendMailAsync(string file)
		{
			GAManager.SendEvent("FileShare", "SendMail", "Count", 1L);
			LongPathFile fullPath = file;
			if (fullPath.IsExists)
			{
				await Task.Run(delegate
				{
					try
					{
						new EmailMessage
						{
							Subject = (fullPath.FileInfo.Name ?? ""),
							AttachmentFilePath = { fullPath },
							Body = "Here is a document for you, please see the attachment for details.\n \nShared from PDFgear:\nhttps://www.pdfgear.com/"
						}.Send();
					}
					catch
					{
					}
				});
			}
		}
	}
}
