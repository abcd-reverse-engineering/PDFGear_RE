using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000109 RID: 265
	public class SelectedCountVisibilityConverter : IValueConverter
	{
		// Token: 0x06000CA8 RID: 3240 RVA: 0x0004122C File Offset: 0x0003F42C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int)
			{
				int num = (int)value;
				return (num <= 0) ? Visibility.Collapsed : Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x06000CA9 RID: 3241 RVA: 0x0004125C File Offset: 0x0003F45C
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
