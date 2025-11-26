using System;
using System.Windows;
using System.Windows.Threading;

namespace pdfconverter.Utils.Image
{
	// Token: 0x02000049 RID: 73
	internal class ImageDispatcherHelper
	{
		// Token: 0x06000530 RID: 1328 RVA: 0x00015504 File Offset: 0x00013704
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
