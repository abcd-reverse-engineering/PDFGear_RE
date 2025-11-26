using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x0200010F RID: 271
	internal class StampContentToIsEnabledConverter : IValueConverter
	{
		// Token: 0x06000CBA RID: 3258 RVA: 0x000413FD File Offset: 0x0003F5FD
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (int)value == 0;
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x0004140D File Offset: 0x0003F60D
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
