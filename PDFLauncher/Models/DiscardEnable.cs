using System;
using System.Globalization;
using System.Windows.Data;

namespace PDFLauncher.Models
{
	// Token: 0x0200001D RID: 29
	public class DiscardEnable : IValueConverter
	{
		// Token: 0x060001A2 RID: 418 RVA: 0x00006AC2 File Offset: 0x00004CC2
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return int.Parse(value.ToString()) > 0;
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00006AD7 File Offset: 0x00004CD7
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
