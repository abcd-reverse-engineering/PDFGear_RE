using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter
{
	// Token: 0x02000010 RID: 16
	public class OperationDeleteBtnVisibility : IValueConverter
	{
		// Token: 0x0600007E RID: 126 RVA: 0x00002330 File Offset: 0x00000530
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			FileCovertStatus fileCovertStatus = (FileCovertStatus)value;
			if (fileCovertStatus == FileCovertStatus.ConvertLoading || fileCovertStatus == FileCovertStatus.ConvertCoverting)
			{
				return Visibility.Hidden;
			}
			return Visibility.Visible;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00002359 File Offset: 0x00000559
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
