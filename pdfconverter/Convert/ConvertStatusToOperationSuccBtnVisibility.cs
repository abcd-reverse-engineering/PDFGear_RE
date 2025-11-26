using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using pdfconverter.Models;

namespace pdfconverter.Convert
{
	// Token: 0x02000091 RID: 145
	public class ConvertStatusToOperationSuccBtnVisibility : IValueConverter
	{
		// Token: 0x0600069A RID: 1690 RVA: 0x00017D88 File Offset: 0x00015F88
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((ToPDFItemStatus)value == ToPDFItemStatus.Succ)
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x00017DA0 File Offset: 0x00015FA0
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
