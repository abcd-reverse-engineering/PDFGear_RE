using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter
{
	// Token: 0x02000011 RID: 17
	public class FileSelectIsEnabled : IValueConverter
	{
		// Token: 0x06000081 RID: 129 RVA: 0x00002368 File Offset: 0x00000568
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			FileCovertStatus fileCovertStatus = (FileCovertStatus)value;
			if (fileCovertStatus == FileCovertStatus.ConvertUnsupport || fileCovertStatus == FileCovertStatus.ConvertLoadedFailed)
			{
				return false;
			}
			return true;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00002391 File Offset: 0x00000591
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
