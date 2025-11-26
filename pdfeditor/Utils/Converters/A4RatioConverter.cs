using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000E1 RID: 225
	internal class A4RatioConverter : IValueConverter
	{
		// Token: 0x06000C30 RID: 3120 RVA: 0x00040344 File Offset: 0x0003E544
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is double)
			{
				double num = (double)value;
				return num * 1.414;
			}
			return 0;
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x00040377 File Offset: 0x0003E577
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
