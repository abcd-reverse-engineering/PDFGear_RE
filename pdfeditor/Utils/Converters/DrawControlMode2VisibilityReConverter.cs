using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000F2 RID: 242
	internal class DrawControlMode2VisibilityReConverter : IValueConverter
	{
		// Token: 0x06000C63 RID: 3171 RVA: 0x00040A22 File Offset: 0x0003EC22
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (string.Format("{0}", value) != string.Format("{0}", parameter))
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x06000C64 RID: 3172 RVA: 0x00040A4E File Offset: 0x0003EC4E
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
