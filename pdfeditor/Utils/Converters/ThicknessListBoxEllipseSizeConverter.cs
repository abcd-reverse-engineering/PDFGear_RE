using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using pdfeditor.Controls.Screenshots;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000119 RID: 281
	internal class ThicknessListBoxEllipseSizeConverter : IValueConverter
	{
		// Token: 0x06000CD8 RID: 3288 RVA: 0x00041750 File Offset: 0x0003F950
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double num = (double)value;
			int num2 = Array.IndexOf<double>(DrawSettingConstants.Thicknesses, num);
			if (num2 < 0 || num2 > DrawSettingConstants.ThicknessListBoxEllipseSize.Count<double>() - 1)
			{
				num2 = 0;
			}
			return DrawSettingConstants.ThicknessListBoxEllipseSize[num2];
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x00041794 File Offset: 0x0003F994
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double num = (double)value;
			int num2 = Array.IndexOf<double>(DrawSettingConstants.ThicknessListBoxEllipseSize, num);
			if (num2 < 0 || num2 > DrawSettingConstants.Thicknesses.Count<double>() - 1)
			{
				num2 = 0;
			}
			return DrawSettingConstants.Thicknesses[num2];
		}
	}
}
