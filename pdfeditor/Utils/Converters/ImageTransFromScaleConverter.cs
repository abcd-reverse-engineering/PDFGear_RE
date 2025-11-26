using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000F8 RID: 248
	internal class ImageTransFromScaleConverter : IValueConverter
	{
		// Token: 0x06000C75 RID: 3189 RVA: 0x00040B15 File Offset: 0x0003ED15
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((bool)value) ? 0.7 : 1.0;
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x00040B38 File Offset: 0x0003ED38
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
