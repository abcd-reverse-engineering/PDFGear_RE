using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Convert
{
	// Token: 0x0200008E RID: 142
	public class CompressedSizeConvert : IMultiValueConverter
	{
		// Token: 0x06000691 RID: 1681 RVA: 0x00017C54 File Offset: 0x00015E54
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			string text = "";
			try
			{
				if (int.Parse(values[1].ToString()) == 8)
				{
					text = "";
				}
				else
				{
					text = values[0].ToString();
				}
			}
			catch
			{
			}
			return text;
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x00017CA0 File Offset: 0x00015EA0
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
