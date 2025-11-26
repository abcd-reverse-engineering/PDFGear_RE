using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x0200010E RID: 270
	public class StampContent2ToVisibilityConverter : IValueConverter
	{
		// Token: 0x06000CB7 RID: 3255 RVA: 0x000413DA File Offset: 0x0003F5DA
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((int)value == 1) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x06000CB8 RID: 3256 RVA: 0x000413EE File Offset: 0x0003F5EE
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
