using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000110 RID: 272
	public class StampContentToVisibilityConverter : IValueConverter
	{
		// Token: 0x06000CBD RID: 3261 RVA: 0x0004141C File Offset: 0x0003F61C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int num = (int)value;
			return (num == 0 || num == 2) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x06000CBE RID: 3262 RVA: 0x00041440 File Offset: 0x0003F640
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
