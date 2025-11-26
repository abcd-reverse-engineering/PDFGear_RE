using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000FE RID: 254
	internal class LogicalOrConverter : IMultiValueConverter
	{
		// Token: 0x06000C87 RID: 3207 RVA: 0x00040C9C File Offset: 0x0003EE9C
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			foreach (object obj in values)
			{
				bool flag;
				bool flag2;
				if (obj is bool)
				{
					flag = (bool)obj;
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
				if (flag2 && flag)
				{
					return Visibility.Visible;
				}
			}
			return Visibility.Collapsed;
		}

		// Token: 0x06000C88 RID: 3208 RVA: 0x00040CE1 File Offset: 0x0003EEE1
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
