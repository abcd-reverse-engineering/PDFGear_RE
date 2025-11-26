using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000113 RID: 275
	public class StringNotEmptyToVisibilityConverter : IValueConverter
	{
		// Token: 0x06000CC6 RID: 3270 RVA: 0x00041500 File Offset: 0x0003F700
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string text = value as string;
			if (text != null && !string.IsNullOrWhiteSpace(text))
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x06000CC7 RID: 3271 RVA: 0x0004152C File Offset: 0x0003F72C
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
