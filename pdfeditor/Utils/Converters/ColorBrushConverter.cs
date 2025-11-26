using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000EC RID: 236
	public class ColorBrushConverter : IValueConverter
	{
		// Token: 0x06000C51 RID: 3153 RVA: 0x00040898 File Offset: 0x0003EA98
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Brush brush = value as Brush;
			if (brush != null)
			{
				return brush;
			}
			string text = value as string;
			if (text != null)
			{
				return ColorConverter.ConvertFromString(text);
			}
			if (value is Color)
			{
				Color color = (Color)value;
				return new SolidColorBrush(color);
			}
			return null;
		}

		// Token: 0x06000C52 RID: 3154 RVA: 0x000408D9 File Offset: 0x0003EAD9
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
