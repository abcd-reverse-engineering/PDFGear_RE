using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x0200007B RID: 123
	public class SplitStatusProgressRingActive : IValueConverter
	{
		// Token: 0x06000601 RID: 1537 RVA: 0x00016B82 File Offset: 0x00014D82
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((SplitStatus)value == SplitStatus.Spliting)
			{
				return true;
			}
			return false;
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x00016B9A File Offset: 0x00014D9A
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
