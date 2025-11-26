using System;
using System.Windows;
using Patagames.Pdf.Net;

namespace pdfeditor.Utils
{
	// Token: 0x0200007E RID: 126
	internal static class LaunchUtils
	{
		// Token: 0x060008E0 RID: 2272 RVA: 0x0002C55C File Offset: 0x0002A75C
		public static void Initialize(StartupEventArgs e)
		{
			int num = Array.IndexOf<string>(e.Args, "-action");
			if (num != -1 && e.Args.Length > num + 1)
			{
				LaunchUtils.action = e.Args[num + 1];
			}
		}

		// Token: 0x060008E1 RID: 2273 RVA: 0x0002C59C File Offset: 0x0002A79C
		public static void OnDocumentLoaded(PdfDocument document)
		{
			if (!string.IsNullOrEmpty(LaunchUtils.action))
			{
				string text = LaunchUtils.action;
				LaunchUtils.action = null;
				LaunchActionInvokedEventHandler launchActionInvoked = LaunchUtils.LaunchActionInvoked;
				if (launchActionInvoked == null)
				{
					return;
				}
				launchActionInvoked(document, new LaunchActionInvokedEventArgs(text));
			}
		}

		// Token: 0x060008E2 RID: 2274 RVA: 0x0002C5D8 File Offset: 0x0002A7D8
		public static void DoLaunchAction()
		{
			if (LaunchUtils.action == "new:CreatedBlankFile" || LaunchUtils.action == "new:CreatedFileFromScanner")
			{
				string text = LaunchUtils.action;
				LaunchUtils.action = null;
				LaunchActionInvokedEventHandler launchActionInvoked = LaunchUtils.LaunchActionInvoked;
				if (launchActionInvoked == null)
				{
					return;
				}
				launchActionInvoked(null, new LaunchActionInvokedEventArgs(text));
			}
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060008E3 RID: 2275 RVA: 0x0002C62C File Offset: 0x0002A82C
		// (remove) Token: 0x060008E4 RID: 2276 RVA: 0x0002C660 File Offset: 0x0002A860
		public static event LaunchActionInvokedEventHandler LaunchActionInvoked;

		// Token: 0x04000452 RID: 1106
		private static string action;
	}
}
