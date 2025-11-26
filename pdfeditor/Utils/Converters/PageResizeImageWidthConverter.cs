using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000102 RID: 258
	internal class PageResizeImageWidthConverter : IValueConverter
	{
		// Token: 0x06000C93 RID: 3219 RVA: 0x000410B4 File Offset: 0x0003F2B4
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			object obj;
			try
			{
				if (value == null || value.ToString() == "0")
				{
					obj = 250;
				}
				else
				{
					obj = (int)value - 10;
				}
			}
			catch
			{
				obj = 250;
			}
			return obj;
		}

		// Token: 0x06000C94 RID: 3220 RVA: 0x00041114 File Offset: 0x0003F314
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
