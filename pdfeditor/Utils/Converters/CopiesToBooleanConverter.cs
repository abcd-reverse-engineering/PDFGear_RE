using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000ED RID: 237
	public class CopiesToBooleanConverter : IValueConverter
	{
		// Token: 0x06000C54 RID: 3156 RVA: 0x000408E8 File Offset: 0x0003EAE8
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int num;
			int.TryParse(value.ToString(), out num);
			return num > 1;
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x0004090C File Offset: 0x0003EB0C
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
