using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000101 RID: 257
	internal class NumberIsEqualConverter : IValueConverter
	{
		// Token: 0x06000C90 RID: 3216 RVA: 0x00041064 File Offset: 0x0003F264
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int)
			{
				int num = (int)value;
				string text = parameter as string;
				int num2;
				if (text != null && int.TryParse(text, out num2))
				{
					return num == num2;
				}
			}
			return false;
		}

		// Token: 0x06000C91 RID: 3217 RVA: 0x000410A4 File Offset: 0x0003F2A4
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
