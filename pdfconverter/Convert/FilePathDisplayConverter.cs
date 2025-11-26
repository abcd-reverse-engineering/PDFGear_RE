using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Convert
{
	// Token: 0x02000099 RID: 153
	public class FilePathDisplayConverter : IValueConverter
	{
		// Token: 0x060006B2 RID: 1714 RVA: 0x0001805C File Offset: 0x0001625C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string text = value as string;
			if (text != null)
			{
				return text.FullPathWithoutPrefix ?? "";
			}
			return "";
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x00018090 File Offset: 0x00016290
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
