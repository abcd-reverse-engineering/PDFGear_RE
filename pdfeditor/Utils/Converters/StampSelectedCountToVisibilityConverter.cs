using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000111 RID: 273
	internal class StampSelectedCountToVisibilityConverter : IValueConverter
	{
		// Token: 0x06000CC0 RID: 3264 RVA: 0x00041450 File Offset: 0x0003F650
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int)
			{
				int num = (int)value;
				return (num <= 0) ? Visibility.Visible : Visibility.Collapsed;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x06000CC1 RID: 3265 RVA: 0x00041480 File Offset: 0x0003F680
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
