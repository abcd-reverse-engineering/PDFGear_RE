using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000F6 RID: 246
	internal class ImageTransFromAngleConverter : IValueConverter
	{
		// Token: 0x06000C6F RID: 3183 RVA: 0x00040ACC File Offset: 0x0003ECCC
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((bool)value) ? (-90) : 0;
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x00040AE0 File Offset: 0x0003ECE0
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
