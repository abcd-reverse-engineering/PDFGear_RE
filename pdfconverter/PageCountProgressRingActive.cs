using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter
{
	// Token: 0x02000009 RID: 9
	public class PageCountProgressRingActive : IValueConverter
	{
		// Token: 0x06000069 RID: 105 RVA: 0x00002198 File Offset: 0x00000398
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			FileCovertStatus fileCovertStatus = (FileCovertStatus)value;
			if (fileCovertStatus == FileCovertStatus.ConvertInit || fileCovertStatus == FileCovertStatus.ConvertLoading)
			{
				return true;
			}
			return false;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000021C0 File Offset: 0x000003C0
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
