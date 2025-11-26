using System;
using System.Globalization;
using System.Windows.Data;
using pdfeditor.Utils.Enums;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000117 RID: 279
	public class SubViewModeContinuousConverter : IValueConverter
	{
		// Token: 0x06000CD2 RID: 3282 RVA: 0x000416CD File Offset: 0x0003F8CD
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((SubViewModeContinuous)value == SubViewModeContinuous.Verticalcontinuous)
			{
				return true;
			}
			return false;
		}

		// Token: 0x06000CD3 RID: 3283 RVA: 0x000416E5 File Offset: 0x0003F8E5
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)value)
			{
				return SubViewModeContinuous.Verticalcontinuous;
			}
			return SubViewModeContinuous.Discontinuous;
		}
	}
}
