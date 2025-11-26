using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using pdfeditor.Models.Scan;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000103 RID: 259
	internal class ResolutionTrimConverter : IValueConverter
	{
		// Token: 0x06000C96 RID: 3222 RVA: 0x00041124 File Offset: 0x0003F324
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is KeyValuePair<ScanResolution, string>)
			{
				KeyValuePair<ScanResolution, string> keyValuePair = (KeyValuePair<ScanResolution, string>)value;
				return new KeyValuePair<ScanResolution, string>(keyValuePair.Key, keyValuePair.Value.Trim());
			}
			return value;
		}

		// Token: 0x06000C97 RID: 3223 RVA: 0x0004115F File Offset: 0x0003F35F
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
