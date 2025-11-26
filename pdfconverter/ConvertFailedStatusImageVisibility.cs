using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter
{
	// Token: 0x0200000D RID: 13
	public class ConvertFailedStatusImageVisibility : IValueConverter
	{
		// Token: 0x06000075 RID: 117 RVA: 0x00002255 File Offset: 0x00000455
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((FileCovertStatus)value == FileCovertStatus.ConvertFail)
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000226D File Offset: 0x0000046D
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
