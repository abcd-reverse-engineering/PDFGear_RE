using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x0200011C RID: 284
	public class TwoValuesIsEqualConverter : IMultiValueConverter
	{
		// Token: 0x06000CE1 RID: 3297 RVA: 0x00041891 File Offset: 0x0003FA91
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length < 2)
			{
				return false;
			}
			if (object.Equals(values[0], values[1]))
			{
				return true;
			}
			return false;
		}

		// Token: 0x06000CE2 RID: 3298 RVA: 0x000418BA File Offset: 0x0003FABA
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
