using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002A0 RID: 672
	public class PopupControlBooleanToVisibilityConverter : IValueConverter
	{
		// Token: 0x060026CB RID: 9931 RVA: 0x000B7B69 File Offset: 0x000B5D69
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x060026CC RID: 9932 RVA: 0x000B7B7C File Offset: 0x000B5D7C
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Visibility)value == Visibility.Visible;
		}
	}
}
