using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000115 RID: 277
	public class StringToBooleanConverter : IValueConverter
	{
		// Token: 0x06000CCC RID: 3276 RVA: 0x000415FF File Offset: 0x0003F7FF
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null && !string.IsNullOrEmpty(value.ToString()))
			{
				return true;
			}
			return false;
		}

		// Token: 0x06000CCD RID: 3277 RVA: 0x0004161E File Offset: 0x0003F81E
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
