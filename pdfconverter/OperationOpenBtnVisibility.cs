using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter
{
	// Token: 0x0200000F RID: 15
	public class OperationOpenBtnVisibility : IValueConverter
	{
		// Token: 0x0600007B RID: 123 RVA: 0x00002308 File Offset: 0x00000508
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((FileCovertStatus)value == FileCovertStatus.ConvertSucc)
			{
				return Visibility.Visible;
			}
			return Visibility.Hidden;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00002320 File Offset: 0x00000520
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
