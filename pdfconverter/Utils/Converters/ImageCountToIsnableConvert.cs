using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Utils.Converters
{
	// Token: 0x0200004F RID: 79
	internal class ImageCountToIsnableConvert : IValueConverter
	{
		// Token: 0x0600054C RID: 1356 RVA: 0x00015B48 File Offset: 0x00013D48
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (int)value > 0;
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x00015B58 File Offset: 0x00013D58
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
