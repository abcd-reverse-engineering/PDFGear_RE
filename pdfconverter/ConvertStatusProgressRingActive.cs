using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter
{
	// Token: 0x0200000B RID: 11
	public class ConvertStatusProgressRingActive : IValueConverter
	{
		// Token: 0x0600006F RID: 111 RVA: 0x00002207 File Offset: 0x00000407
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((FileCovertStatus)value == FileCovertStatus.ConvertCoverting)
			{
				return true;
			}
			return false;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000221F File Offset: 0x0000041F
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
