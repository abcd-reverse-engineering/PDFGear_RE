using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000E9 RID: 233
	public class BooleanToVisibilityReverseConverter : IValueConverter
	{
		// Token: 0x06000C48 RID: 3144 RVA: 0x000405E0 File Offset: 0x0003E7E0
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		// Token: 0x06000C49 RID: 3145 RVA: 0x00040600 File Offset: 0x0003E800
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
