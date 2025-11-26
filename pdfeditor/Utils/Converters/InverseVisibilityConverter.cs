using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000FB RID: 251
	internal class InverseVisibilityConverter : IValueConverter
	{
		// Token: 0x06000C7E RID: 3198 RVA: 0x00040B96 File Offset: 0x0003ED96
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return Visibility.Visible;
			}
			if ((Visibility)value != Visibility.Visible)
			{
				return Visibility.Visible;
			}
			if (parameter != null && parameter.GetType() == typeof(Visibility))
			{
				return parameter;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x00040BD3 File Offset: 0x0003EDD3
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
