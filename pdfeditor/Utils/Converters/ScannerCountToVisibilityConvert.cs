using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000105 RID: 261
	internal class ScannerCountToVisibilityConvert : IValueConverter
	{
		// Token: 0x06000C9C RID: 3228 RVA: 0x0004118D File Offset: 0x0003F38D
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((int)value > 0) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x000411A1 File Offset: 0x0003F3A1
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
