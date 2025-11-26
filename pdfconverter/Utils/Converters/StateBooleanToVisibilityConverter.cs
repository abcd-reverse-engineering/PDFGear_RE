using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter.Utils.Converters
{
	// Token: 0x02000053 RID: 83
	public class StateBooleanToVisibilityConverter : IValueConverter
	{
		// Token: 0x06000558 RID: 1368 RVA: 0x00015C14 File Offset: 0x00013E14
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
			{
				return Visibility.Visible;
			}
			if (value is bool && !(bool)value)
			{
				return Visibility.Collapsed;
			}
			if (parameter is Visibility)
			{
				Visibility visibility = (Visibility)parameter;
				return visibility;
			}
			return Visibility.Visible;
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x00015C6B File Offset: 0x00013E6B
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Visibility)value == Visibility.Visible;
		}
	}
}
