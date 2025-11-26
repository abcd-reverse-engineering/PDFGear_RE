using System;
using System.Windows;
using System.Windows.Threading;

namespace pdfeditor.Utils.Scan
{
	// Token: 0x020000B8 RID: 184
	public class ScannerDispatcherHelper
	{
		// Token: 0x06000B0E RID: 2830 RVA: 0x00039124 File Offset: 0x00037324
		public static void Invoke(Action callback)
		{
			Application application = Application.Current;
			Dispatcher dispatcher = ((application != null) ? application.Dispatcher : null);
			if (dispatcher == null)
			{
				return;
			}
			if (dispatcher.CheckAccess())
			{
				callback();
				return;
			}
			dispatcher.Invoke(callback);
		}
	}
}
