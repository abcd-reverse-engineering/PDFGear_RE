using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x02000063 RID: 99
	public class MergeStatusProgressRingActive : IValueConverter
	{
		// Token: 0x060005B8 RID: 1464 RVA: 0x0001653B File Offset: 0x0001473B
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((MergeStatus)value == MergeStatus.Merging)
			{
				return true;
			}
			return false;
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x00016553 File Offset: 0x00014753
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
