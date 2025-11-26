using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter.Utils.Converters
{
	// Token: 0x0200004D RID: 77
	public class BooleanToVisibilityConverter : IValueConverter
	{
		// Token: 0x06000546 RID: 1350 RVA: 0x00015A8E File Offset: 0x00013C8E
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x00015AA1 File Offset: 0x00013CA1
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Visibility)value == Visibility.Visible;
		}
	}
}
