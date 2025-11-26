using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000E8 RID: 232
	public class BooleanToOpacityReverseConverter : IValueConverter
	{
		// Token: 0x06000C45 RID: 3141 RVA: 0x00040560 File Offset: 0x0003E760
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && (bool)value)
			{
				return 0.0;
			}
			return 1.0;
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x00040590 File Offset: 0x0003E790
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				return global::System.Convert.ToDouble(value, CultureInfo.InvariantCulture) == 0.0;
			}
			catch
			{
			}
			return true;
		}
	}
}
