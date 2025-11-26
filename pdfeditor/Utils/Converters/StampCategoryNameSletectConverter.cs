using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x0200010D RID: 269
	internal class StampCategoryNameSletectConverter : IValueConverter
	{
		// Token: 0x06000CB4 RID: 3252 RVA: 0x000413B8 File Offset: 0x0003F5B8
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((int)value == 0) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x000413CB File Offset: 0x0003F5CB
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
