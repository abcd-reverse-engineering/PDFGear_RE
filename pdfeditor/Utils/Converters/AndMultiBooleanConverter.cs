using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000E2 RID: 226
	internal class AndMultiBooleanConverter : IMultiValueConverter
	{
		// Token: 0x06000C33 RID: 3123 RVA: 0x00040388 File Offset: 0x0003E588
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag = false;
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i].GetType() == typeof(bool))
				{
					bool flag2 = (bool)values[i];
					if (i == 0)
					{
						flag = flag2;
					}
					else
					{
						flag = flag && flag2;
					}
				}
				if (!flag)
				{
					break;
				}
			}
			return flag;
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x000403DB File Offset: 0x0003E5DB
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
