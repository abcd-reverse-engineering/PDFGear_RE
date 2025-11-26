using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Utils.Converters
{
	// Token: 0x02000052 RID: 82
	public class StateBooleanToIsnabledConverter : IValueConverter
	{
		// Token: 0x06000555 RID: 1365 RVA: 0x00015BCF File Offset: 0x00013DCF
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
			{
				return true;
			}
			if (value is bool && !(bool)value)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06000556 RID: 1366 RVA: 0x00015C05 File Offset: 0x00013E05
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
