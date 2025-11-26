using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000112 RID: 274
	public class StringBrushConverter : IValueConverter
	{
		// Token: 0x06000CC3 RID: 3267 RVA: 0x00041490 File Offset: 0x0003F690
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return Brushes.Transparent;
			}
			string text = value.ToString();
			object obj;
			try
			{
				obj = (text.StartsWith("#") ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(text)) : Brushes.Transparent);
			}
			catch (Exception)
			{
				obj = Brushes.Transparent;
			}
			return obj;
		}

		// Token: 0x06000CC4 RID: 3268 RVA: 0x000414F0 File Offset: 0x0003F6F0
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
