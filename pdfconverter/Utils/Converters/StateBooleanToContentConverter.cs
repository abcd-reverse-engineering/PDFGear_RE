using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Utils.Converters
{
	// Token: 0x02000051 RID: 81
	public class StateBooleanToContentConverter : IValueConverter
	{
		// Token: 0x06000552 RID: 1362 RVA: 0x00015B8D File Offset: 0x00013D8D
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
			{
				return "Success";
			}
			if (value is bool && !(bool)value)
			{
				return "Fail";
			}
			return "";
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x00015BC0 File Offset: 0x00013DC0
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
