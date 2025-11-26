using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000108 RID: 264
	public class SearchResultVisibilityConverter : IValueConverter
	{
		// Token: 0x06000CA5 RID: 3237 RVA: 0x00041207 File Offset: 0x0003F407
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((int)value > 0) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x06000CA6 RID: 3238 RVA: 0x0004121B File Offset: 0x0003F41B
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
