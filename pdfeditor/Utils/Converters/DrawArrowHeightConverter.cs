using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000EF RID: 239
	internal class DrawArrowHeightConverter : IValueConverter
	{
		// Token: 0x06000C5A RID: 3162 RVA: 0x00040953 File Offset: 0x0003EB53
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (double)value / 2.0;
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x0004096A File Offset: 0x0003EB6A
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
