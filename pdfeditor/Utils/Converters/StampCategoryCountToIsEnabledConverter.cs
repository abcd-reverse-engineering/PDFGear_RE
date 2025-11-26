using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x0200010C RID: 268
	internal class StampCategoryCountToIsEnabledConverter : IValueConverter
	{
		// Token: 0x06000CB1 RID: 3249 RVA: 0x00041399 File Offset: 0x0003F599
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (int)value < 5;
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x000413A9 File Offset: 0x0003F5A9
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
