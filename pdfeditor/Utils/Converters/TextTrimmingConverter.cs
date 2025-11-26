using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000118 RID: 280
	internal class TextTrimmingConverter : IValueConverter
	{
		// Token: 0x06000CD5 RID: 3285 RVA: 0x00041704 File Offset: 0x0003F904
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string text = value as string;
			if (text != null && !string.IsNullOrEmpty(text))
			{
				return text.TrimEnd(new char[] { ' ', ':', '：' });
			}
			return string.Empty;
		}

		// Token: 0x06000CD6 RID: 3286 RVA: 0x00041740 File Offset: 0x0003F940
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
