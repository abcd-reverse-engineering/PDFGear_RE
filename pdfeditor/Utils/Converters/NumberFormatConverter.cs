using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000107 RID: 263
	internal class NumberFormatConverter : IValueConverter
	{
		// Token: 0x06000CA2 RID: 3234 RVA: 0x000411D8 File Offset: 0x0003F3D8
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return global::System.Convert.ToDouble(value).ToString("#####%");
		}

		// Token: 0x06000CA3 RID: 3235 RVA: 0x000411F8 File Offset: 0x0003F3F8
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
