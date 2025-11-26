using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x0200011B RID: 283
	public class ToolbarSettingViewerPaddingConverter : IValueConverter
	{
		// Token: 0x06000CDE RID: 3294 RVA: 0x00041830 File Offset: 0x0003FA30
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return new Thickness(10.0);
			}
			return new Thickness(10.0, 50.0, 10.0, 10.0);
		}

		// Token: 0x06000CDF RID: 3295 RVA: 0x00041882 File Offset: 0x0003FA82
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
