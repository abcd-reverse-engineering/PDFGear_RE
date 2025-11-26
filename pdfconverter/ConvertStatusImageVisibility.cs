using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter
{
	// Token: 0x0200000C RID: 12
	public class ConvertStatusImageVisibility : IValueConverter
	{
		// Token: 0x06000072 RID: 114 RVA: 0x0000222E File Offset: 0x0000042E
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((FileCovertStatus)value == FileCovertStatus.ConvertCoverting)
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00002246 File Offset: 0x00000446
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
