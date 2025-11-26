using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000E5 RID: 229
	public class BatchPrintStatusConverter : IValueConverter
	{
		// Token: 0x06000C3C RID: 3132 RVA: 0x0004042C File Offset: 0x0003E62C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int num = (int)value;
			string text = (string)parameter;
			if (text == "Printing")
			{
				if (num == 1)
				{
					return Visibility.Visible;
				}
				return Visibility.Collapsed;
			}
			else if (text == "Done")
			{
				if (num == 2)
				{
					return Visibility.Visible;
				}
				return Visibility.Collapsed;
			}
			else
			{
				if (!(text == "Fail"))
				{
					return Visibility.Collapsed;
				}
				if (num == 3)
				{
					return Visibility.Visible;
				}
				return Visibility.Collapsed;
			}
		}

		// Token: 0x06000C3D RID: 3133 RVA: 0x000404AA File Offset: 0x0003E6AA
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
