using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x02000081 RID: 129
	public class SplitPDFUIStatusToUIElementEnabled : IValueConverter
	{
		// Token: 0x06000624 RID: 1572 RVA: 0x00016FB8 File Offset: 0x000151B8
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((SplitPDFUIStatus)value == SplitPDFUIStatus.Spliting)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x00016FD0 File Offset: 0x000151D0
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
