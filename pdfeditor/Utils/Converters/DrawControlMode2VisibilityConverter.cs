using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000F1 RID: 241
	internal class DrawControlMode2VisibilityConverter : IValueConverter
	{
		// Token: 0x06000C60 RID: 3168 RVA: 0x000409E7 File Offset: 0x0003EBE7
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (string.Format("{0}", value) == string.Format("{0}", parameter))
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x00040A13 File Offset: 0x0003EC13
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
