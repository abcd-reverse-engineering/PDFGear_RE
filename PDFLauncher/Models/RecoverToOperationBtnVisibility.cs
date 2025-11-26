using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PDFLauncher.Models
{
	// Token: 0x0200001F RID: 31
	public class RecoverToOperationBtnVisibility : IValueConverter
	{
		// Token: 0x060001A8 RID: 424 RVA: 0x00006B0C File Offset: 0x00004D0C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((RecoverStatus)value == RecoverStatus.Converted)
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00006B24 File Offset: 0x00004D24
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
