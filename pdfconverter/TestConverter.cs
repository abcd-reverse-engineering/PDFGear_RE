using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter
{
	// Token: 0x0200001B RID: 27
	public class TestConverter : IMultiValueConverter
	{
		// Token: 0x060000C5 RID: 197 RVA: 0x00003782 File Offset: 0x00001982
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			return (values.Length >= 2 && object.Equals(values[0], values[1])) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x000037A0 File Offset: 0x000019A0
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
