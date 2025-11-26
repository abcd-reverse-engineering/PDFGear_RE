using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000114 RID: 276
	public class StringStrokeConverter : IValueConverter
	{
		// Token: 0x06000CC9 RID: 3273 RVA: 0x0004153C File Offset: 0x0003F73C
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
				if (text.StartsWith("#"))
				{
					if (text.Length == 9 && text.StartsWith("#00"))
					{
						return parameter.ToString().Equals("Stroke") ? Brushes.Black : Brushes.Red;
					}
					if (text.Equals("#FFFFFFFF", StringComparison.OrdinalIgnoreCase))
					{
						return parameter.ToString().Equals("Stroke") ? Brushes.Black : Brushes.Transparent;
					}
				}
				obj = Brushes.Transparent;
			}
			catch (Exception)
			{
				obj = Brushes.Transparent;
			}
			return obj;
		}

		// Token: 0x06000CCA RID: 3274 RVA: 0x000415F0 File Offset: 0x0003F7F0
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
