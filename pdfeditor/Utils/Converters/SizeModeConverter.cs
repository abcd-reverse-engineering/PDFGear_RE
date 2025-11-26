using System;
using System.Globalization;
using System.Windows.Data;
using pdfeditor.Utils.Enums;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x0200010B RID: 267
	public class SizeModeConverter : IValueConverter
	{
		// Token: 0x06000CAE RID: 3246 RVA: 0x000412A4 File Offset: 0x0003F4A4
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SizeModesWrap sizeModesWrap = (SizeModesWrap)value;
			string text = (string)parameter;
			if (sizeModesWrap == SizeModesWrap.ZoomActualSize && text == "ActualSize")
			{
				return true;
			}
			if (sizeModesWrap == SizeModesWrap.FitToWidth && text == "FitToWidth")
			{
				return true;
			}
			if (sizeModesWrap == SizeModesWrap.FitToHeight && text == "FitToHeight")
			{
				return true;
			}
			if (sizeModesWrap == SizeModesWrap.FitToSize && text == "FitToSize")
			{
				return true;
			}
			return false;
		}

		// Token: 0x06000CAF RID: 3247 RVA: 0x00041324 File Offset: 0x0003F524
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag = (bool)value;
			string text = (string)parameter;
			if (flag)
			{
				if (text == "ActualSize")
				{
					return SizeModesWrap.ZoomActualSize;
				}
				if (text == "FitToWidth")
				{
					return SizeModesWrap.FitToWidth;
				}
				if (text == "FitToHeight")
				{
					return SizeModesWrap.FitToHeight;
				}
				if (text == "FitToSize")
				{
					return SizeModesWrap.FitToSize;
				}
			}
			return null;
		}
	}
}
