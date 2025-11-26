using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000F9 RID: 249
	internal class IntToBooleanConverter : IValueConverter
	{
		// Token: 0x06000C78 RID: 3192 RVA: 0x00040B47 File Offset: 0x0003ED47
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((int)value > 0)
			{
				return true;
			}
			return false;
		}

		// Token: 0x06000C79 RID: 3193 RVA: 0x00040B5F File Offset: 0x0003ED5F
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
