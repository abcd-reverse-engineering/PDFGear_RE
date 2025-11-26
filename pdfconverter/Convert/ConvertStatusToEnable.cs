using System;
using System.Globalization;
using System.Windows.Data;
using pdfconverter.Models;

namespace pdfconverter.Convert
{
	// Token: 0x02000090 RID: 144
	public class ConvertStatusToEnable : IValueConverter
	{
		// Token: 0x06000697 RID: 1687 RVA: 0x00017D4C File Offset: 0x00015F4C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ToPDFItemStatus toPDFItemStatus = (ToPDFItemStatus)value;
			if (toPDFItemStatus == ToPDFItemStatus.Unsupport || toPDFItemStatus == ToPDFItemStatus.Working || toPDFItemStatus == ToPDFItemStatus.LoadedFailed)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x00017D79 File Offset: 0x00015F79
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
