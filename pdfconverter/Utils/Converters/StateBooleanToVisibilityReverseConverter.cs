using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter.Utils.Converters
{
	// Token: 0x02000054 RID: 84
	public class StateBooleanToVisibilityReverseConverter : IValueConverter
	{
		// Token: 0x0600055B RID: 1371 RVA: 0x00015C84 File Offset: 0x00013E84
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
			{
				return Visibility.Visible;
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
			return Visibility.Collapsed;
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x00015CDB File Offset: 0x00013EDB
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Visibility)value == Visibility.Visible;
		}
	}
}
