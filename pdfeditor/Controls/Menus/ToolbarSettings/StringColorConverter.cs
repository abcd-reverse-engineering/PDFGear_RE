using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace pdfeditor.Controls.Menus.ToolbarSettings
{
	// Token: 0x0200027A RID: 634
	public class StringColorConverter : IValueConverter
	{
		// Token: 0x060024AB RID: 9387 RVA: 0x000AA294 File Offset: 0x000A8494
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string text = value as string;
			if (text != null)
			{
				try
				{
					return (Color)ColorConverter.ConvertFromString(text);
				}
				catch
				{
				}
			}
			return Colors.Black;
		}

		// Token: 0x060024AC RID: 9388 RVA: 0x000AA2E0 File Offset: 0x000A84E0
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
