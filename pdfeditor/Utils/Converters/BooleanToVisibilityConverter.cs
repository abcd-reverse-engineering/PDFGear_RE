using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000EA RID: 234
	public class BooleanToVisibilityConverter : IValueConverter
	{
		// Token: 0x06000C4B RID: 3147 RVA: 0x00040634 File Offset: 0x0003E834
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x06000C4C RID: 3148 RVA: 0x00040647 File Offset: 0x0003E847
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Visibility)value == Visibility.Visible;
		}
	}
}
