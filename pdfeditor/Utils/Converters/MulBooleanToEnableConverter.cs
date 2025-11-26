using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000100 RID: 256
	public class MulBooleanToEnableConverter : IMultiValueConverter
	{
		// Token: 0x06000C8D RID: 3213 RVA: 0x00041020 File Offset: 0x0003F220
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			for (int i = 0; i < values.Length; i++)
			{
				if ((bool)values[i])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000C8E RID: 3214 RVA: 0x00041054 File Offset: 0x0003F254
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
