using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter
{
	// Token: 0x0200000A RID: 10
	public class PageCountVisibility : IValueConverter
	{
		// Token: 0x0600006C RID: 108 RVA: 0x000021D0 File Offset: 0x000003D0
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			FileCovertStatus fileCovertStatus = (FileCovertStatus)value;
			if (fileCovertStatus == FileCovertStatus.ConvertInit || fileCovertStatus == FileCovertStatus.ConvertLoading)
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000021F8 File Offset: 0x000003F8
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
