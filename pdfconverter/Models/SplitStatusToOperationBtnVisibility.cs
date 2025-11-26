using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x02000079 RID: 121
	public class SplitStatusToOperationBtnVisibility : IValueConverter
	{
		// Token: 0x060005FB RID: 1531 RVA: 0x00016B20 File Offset: 0x00014D20
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SplitStatus splitStatus = (SplitStatus)value;
			if (splitStatus == SplitStatus.Init || splitStatus == SplitStatus.Loading || splitStatus == SplitStatus.Spliting)
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x00016B4C File Offset: 0x00014D4C
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
