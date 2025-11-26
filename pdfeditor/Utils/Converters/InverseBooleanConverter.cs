using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000FA RID: 250
	public class InverseBooleanConverter : IValueConverter
	{
		// Token: 0x06000C7B RID: 3195 RVA: 0x00040B6E File Offset: 0x0003ED6E
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x00040B7E File Offset: 0x0003ED7E
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}
	}
}
