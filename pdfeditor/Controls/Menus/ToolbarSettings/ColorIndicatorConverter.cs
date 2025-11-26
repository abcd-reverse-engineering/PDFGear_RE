using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace pdfeditor.Controls.Menus.ToolbarSettings
{
	// Token: 0x0200027B RID: 635
	public class ColorIndicatorConverter : IValueConverter
	{
		// Token: 0x060024AE RID: 9390 RVA: 0x000AA2F0 File Offset: 0x000A84F0
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is Color))
			{
				return null;
			}
			Color color = (Color)value;
			if ((double)(((float)color.R * 0.2126f + (float)color.G * 0.7152f + (float)color.B * 0.0722f) / 255f) < 0.65)
			{
				return new SolidColorBrush(Colors.White);
			}
			return new SolidColorBrush(Colors.Black);
		}

		// Token: 0x060024AF RID: 9391 RVA: 0x000AA361 File Offset: 0x000A8561
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
