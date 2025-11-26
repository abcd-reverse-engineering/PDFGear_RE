using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000E7 RID: 231
	public class BooleanToOpacityConverter : IValueConverter
	{
		// Token: 0x06000C42 RID: 3138 RVA: 0x000404E1 File Offset: 0x0003E6E1
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
			{
				return 1.0;
			}
			return 0.0;
		}

		// Token: 0x06000C43 RID: 3139 RVA: 0x00040510 File Offset: 0x0003E710
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				return global::System.Convert.ToDouble(value, CultureInfo.InvariantCulture) > 0.0;
			}
			catch
			{
			}
			return false;
		}
	}
}
