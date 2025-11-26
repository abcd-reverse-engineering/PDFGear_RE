using System;
using System.Diagnostics;
using System.Windows;
using CommonLib.Common;
using CommonLib.Views;

namespace pdfeditor.Utils
{
	// Token: 0x020000A6 RID: 166
	internal class SupportUtils
	{
		// Token: 0x06000A55 RID: 2645 RVA: 0x00034EC4 File Offset: 0x000330C4
		public static void ShowFeedbackWindow(string file = "")
		{
			FeedbackWindow feedbackWindow = new FeedbackWindow();
			feedbackWindow.HideFaq();
			feedbackWindow.Owner = Application.Current.MainWindow;
			feedbackWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			if (!string.IsNullOrWhiteSpace(file))
			{
				feedbackWindow.flist.Add(file);
				feedbackWindow.showAttachmentCB(true);
			}
			feedbackWindow.ShowDialog();
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x00034F18 File Offset: 0x00033118
		public static void SendEmailFeedback()
		{
			try
			{
				string text = "pdfreadersustain@outlook.com";
				string text2 = string.Concat(new string[]
				{
					"subject=[",
					UtilManager.GetProductName(),
					"] [",
					UtilManager.GetAppVersion(),
					"] Support"
				});
				string text3 = "mailto:" + text + "?" + text2;
				Process.Start(new ProcessStartInfo
				{
					FileName = text3,
					UseShellExecute = true,
					CreateNoWindow = true
				});
			}
			catch (Exception)
			{
			}
		}
	}
}
