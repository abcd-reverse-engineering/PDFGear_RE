using System;
using System.Windows;

namespace PDFLauncher.Utils
{
	// Token: 0x02000018 RID: 24
	public static class AppManager
	{
		// Token: 0x06000178 RID: 376 RVA: 0x00006678 File Offset: 0x00004878
		public static void ShowRecoverWindows()
		{
			RecoverWindow recoverWindow = new RecoverWindow();
			if (recoverWindow.VM.RecoverFileList.Count == 0)
			{
				return;
			}
			recoverWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			recoverWindow.ShowDialog();
		}
	}
}
