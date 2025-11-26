using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter.Utils.Converters
{
	// Token: 0x0200004E RID: 78
	public class BooleanToVisibilityReverseConverter : IValueConverter
	{
		// Token: 0x06000549 RID: 1353 RVA: 0x00015ABC File Offset: 0x00013CBC
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
			{
				return Visibility.Collapsed;
			}
			if (value is bool && !(bool)value)
			{
				return Visibility.Visible;
			}
			if (parameter is Visibility)
			{
				Visibility visibility = (Visibility)parameter;
				return visibility;
			}
			return Visibility.Visible;
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x00015B14 File Offset: 0x00013D14
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Visibility)
			{
				Visibility visibility = (Visibility)value;
				return visibility > Visibility.Visible;
			}
			return true;
		}
	}
}
