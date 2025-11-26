using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter.Convert
{
	// Token: 0x02000097 RID: 151
	public class IsOutputInOneFileToMoveBtnVisibilityConvert : IValueConverter
	{
		// Token: 0x060006AC RID: 1708 RVA: 0x00017FD3 File Offset: 0x000161D3
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)value)
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x00017FEA File Offset: 0x000161EA
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
