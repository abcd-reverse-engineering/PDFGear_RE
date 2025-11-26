using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x02000076 RID: 118
	public class SplitStatusToParseFileProgressRingActive : IValueConverter
	{
		// Token: 0x060005F2 RID: 1522 RVA: 0x00016A6C File Offset: 0x00014C6C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SplitStatus splitStatus = (SplitStatus)value;
			if (splitStatus == SplitStatus.Init || splitStatus == SplitStatus.Loading)
			{
				return true;
			}
			return false;
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x00016A94 File Offset: 0x00014C94
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
