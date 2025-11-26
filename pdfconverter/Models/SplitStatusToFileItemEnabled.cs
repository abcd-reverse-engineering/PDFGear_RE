using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x02000078 RID: 120
	public class SplitStatusToFileItemEnabled : IValueConverter
	{
		// Token: 0x060005F8 RID: 1528 RVA: 0x00016AE4 File Offset: 0x00014CE4
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SplitStatus splitStatus = (SplitStatus)value;
			if (splitStatus == SplitStatus.Unsupport || splitStatus == SplitStatus.Spliting || splitStatus == SplitStatus.LoadedFailed)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x00016B11 File Offset: 0x00014D11
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
