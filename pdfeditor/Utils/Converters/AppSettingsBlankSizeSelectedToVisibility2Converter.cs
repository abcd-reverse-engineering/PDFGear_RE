using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000E3 RID: 227
	internal class AppSettingsBlankSizeSelectedToVisibility2Converter : IValueConverter
	{
		// Token: 0x06000C36 RID: 3126 RVA: 0x000403E6 File Offset: 0x0003E5E6
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((int)value != 5) ? Visibility.Collapsed : Visibility.Visible;
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x000403FA File Offset: 0x0003E5FA
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
