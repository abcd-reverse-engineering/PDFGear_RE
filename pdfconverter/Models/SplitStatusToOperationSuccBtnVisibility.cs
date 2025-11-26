using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x0200007A RID: 122
	public class SplitStatusToOperationSuccBtnVisibility : IValueConverter
	{
		// Token: 0x060005FE RID: 1534 RVA: 0x00016B5B File Offset: 0x00014D5B
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((SplitStatus)value == SplitStatus.Succ)
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x00016B73 File Offset: 0x00014D73
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
