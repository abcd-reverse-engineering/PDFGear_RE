using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x02000077 RID: 119
	public class SplitStatusToPageCountVisibility : IValueConverter
	{
		// Token: 0x060005F5 RID: 1525 RVA: 0x00016AA4 File Offset: 0x00014CA4
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SplitStatus splitStatus = (SplitStatus)value;
			if (splitStatus == SplitStatus.Init || splitStatus == SplitStatus.Loading || splitStatus == SplitStatus.Unsupport || splitStatus == SplitStatus.LoadedFailed)
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x00016AD4 File Offset: 0x00014CD4
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
