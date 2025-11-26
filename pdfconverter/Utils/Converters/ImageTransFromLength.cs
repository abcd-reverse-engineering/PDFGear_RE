using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Utils.Converters
{
	// Token: 0x02000050 RID: 80
	internal class ImageTransFromLength : IValueConverter
	{
		// Token: 0x0600054F RID: 1359 RVA: 0x00015B67 File Offset: 0x00013D67
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (double)value / 2.0;
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x00015B7E File Offset: 0x00013D7E
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
