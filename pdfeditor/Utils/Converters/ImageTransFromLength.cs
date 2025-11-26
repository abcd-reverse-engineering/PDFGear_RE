using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000F7 RID: 247
	public class ImageTransFromLength : IValueConverter
	{
		// Token: 0x06000C72 RID: 3186 RVA: 0x00040AEF File Offset: 0x0003ECEF
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (double)value / 2.0;
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x00040B06 File Offset: 0x0003ED06
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
