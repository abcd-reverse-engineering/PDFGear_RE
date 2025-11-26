using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x0200010A RID: 266
	internal class SerarchDelVisibitlityConverter : IValueConverter
	{
		// Token: 0x06000CAB RID: 3243 RVA: 0x0004126B File Offset: 0x0003F46B
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return Visibility.Collapsed;
			}
			return (((string)value).Trim().Length > 0) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x06000CAC RID: 3244 RVA: 0x00041293 File Offset: 0x0003F493
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
