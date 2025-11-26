using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000E4 RID: 228
	internal class AppSettingsBlankSizeSelectedToVisibilityConverter : IValueConverter
	{
		// Token: 0x06000C39 RID: 3129 RVA: 0x00040409 File Offset: 0x0003E609
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((int)value != 5) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x06000C3A RID: 3130 RVA: 0x0004041D File Offset: 0x0003E61D
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
