using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000EE RID: 238
	public class CopiesToVisibilityConverter : IValueConverter
	{
		// Token: 0x06000C57 RID: 3159 RVA: 0x0004091C File Offset: 0x0003EB1C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int num;
			int.TryParse(value.ToString(), out num);
			return (num > 1) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x06000C58 RID: 3160 RVA: 0x00040944 File Offset: 0x0003EB44
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
