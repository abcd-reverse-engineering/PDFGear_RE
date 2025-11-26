using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000104 RID: 260
	internal class ScannerCountToIsnableConvert : IValueConverter
	{
		// Token: 0x06000C99 RID: 3225 RVA: 0x0004116E File Offset: 0x0003F36E
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (int)value > 0;
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x0004117E File Offset: 0x0003F37E
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
