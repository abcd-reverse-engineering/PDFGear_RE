using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000E6 RID: 230
	public class BooleanReverseConverter : IValueConverter
	{
		// Token: 0x06000C3F RID: 3135 RVA: 0x000404B9 File Offset: 0x0003E6B9
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}

		// Token: 0x06000C40 RID: 3136 RVA: 0x000404C9 File Offset: 0x0003E6C9
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool)value;
		}
	}
}
