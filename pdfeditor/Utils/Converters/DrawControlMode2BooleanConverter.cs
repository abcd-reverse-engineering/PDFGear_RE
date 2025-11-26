using System;
using System.Globalization;
using System.Windows.Data;
using pdfeditor.Controls.Screenshots;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000F0 RID: 240
	internal class DrawControlMode2BooleanConverter : IValueConverter
	{
		// Token: 0x06000C5D RID: 3165 RVA: 0x00040979 File Offset: 0x0003EB79
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.Format("{0}", value) == string.Format("{0}", parameter);
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x0004099C File Offset: 0x0003EB9C
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag;
			bool flag2;
			if (value is bool)
			{
				flag = (bool)value;
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			DrawControlMode drawControlMode;
			if (flag2 && flag)
			{
				Enum.TryParse<DrawControlMode>((parameter != null) ? parameter.ToString() : null, true, out drawControlMode);
			}
			else
			{
				drawControlMode = DrawControlMode.None;
			}
			return drawControlMode;
		}
	}
}
