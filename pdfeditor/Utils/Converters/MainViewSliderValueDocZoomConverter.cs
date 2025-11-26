using System;
using System.Globalization;
using System.Windows.Data;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using pdfeditor.Models;
using pdfeditor.ViewModels;
using PDFKit;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000FF RID: 255
	internal class MainViewSliderValueDocZoomConverter : IValueConverter
	{
		// Token: 0x06000C8A RID: 3210 RVA: 0x00040CF0 File Offset: 0x0003EEF0
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double num = global::System.Convert.ToDouble(value);
			if (num < 1.0)
			{
				return num * 50.0;
			}
			if (num <= 1.0)
			{
				return 50;
			}
			if (num <= 2.0)
			{
				return 25.0 * (num - 1.0) + 50.0;
			}
			if (num >= 64.0)
			{
				return 100;
			}
			return 0.40322580645161288 * (num - 2.0) + 75.0;
		}

		// Token: 0x06000C8B RID: 3211 RVA: 0x00040DA0 File Offset: 0x0003EFA0
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			requiredService.ViewToolbar.DocSizeMode = SizeModes.Zoom;
			double num = global::System.Convert.ToDouble(value);
			if (num == 0.0)
			{
				DocumentWrapper documentWrapper = requiredService.DocumentWrapper;
				ConfigManager.SetPageSizeZoomModelAsync((documentWrapper != null) ? documentWrapper.DocumentPath : null, requiredService.ViewToolbar.DocSizeModeWrap.ToString(), 0.01f);
				return 0.01;
			}
			if (num < 50.0)
			{
				DocumentWrapper documentWrapper2 = requiredService.DocumentWrapper;
				ConfigManager.SetPageSizeZoomModelAsync((documentWrapper2 != null) ? documentWrapper2.DocumentPath : null, requiredService.ViewToolbar.DocSizeModeWrap.ToString(), (float)num / 50f);
				return num / 50.0;
			}
			if (num <= 50.0)
			{
				DocumentWrapper documentWrapper3 = requiredService.DocumentWrapper;
				ConfigManager.SetPageSizeZoomModelAsync((documentWrapper3 != null) ? documentWrapper3.DocumentPath : null, requiredService.ViewToolbar.DocSizeModeWrap.ToString(), 1f);
				return 1;
			}
			if (num <= 75.0)
			{
				DocumentWrapper documentWrapper4 = requiredService.DocumentWrapper;
				ConfigManager.SetPageSizeZoomModelAsync((documentWrapper4 != null) ? documentWrapper4.DocumentPath : null, requiredService.ViewToolbar.DocSizeModeWrap.ToString(), (float)(1.0 + (num - 50.0) / 25.0));
				return 1.0 + (num - 50.0) / 25.0;
			}
			if (num >= 6400.0)
			{
				DocumentWrapper documentWrapper5 = requiredService.DocumentWrapper;
				ConfigManager.SetPageSizeZoomModelAsync((documentWrapper5 != null) ? documentWrapper5.DocumentPath : null, requiredService.ViewToolbar.DocSizeModeWrap.ToString(), 64f);
				return 64;
			}
			DocumentWrapper documentWrapper6 = requiredService.DocumentWrapper;
			ConfigManager.SetPageSizeZoomModelAsync((documentWrapper6 != null) ? documentWrapper6.DocumentPath : null, requiredService.ViewToolbar.DocSizeModeWrap.ToString(), (float)(2.0 + (num - 75.0) * 62.0 / 25.0));
			return 2.0 + (num - 75.0) * 62.0 / 25.0;
		}
	}
}
