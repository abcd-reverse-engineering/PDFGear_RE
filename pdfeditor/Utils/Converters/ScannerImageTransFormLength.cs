using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000106 RID: 262
	internal class ScannerImageTransFormLength : IValueConverter
	{
		// Token: 0x06000C9F RID: 3231 RVA: 0x000411B0 File Offset: 0x0003F3B0
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (double)value / 2.0;
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x000411C7 File Offset: 0x0003F3C7
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
