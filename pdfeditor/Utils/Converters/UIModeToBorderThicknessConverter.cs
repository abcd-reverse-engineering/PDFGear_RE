using System;
using System.Globalization;
using System.Windows.Data;
using CommonLib.AppTheme;
using CommonLib.Common;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x0200011D RID: 285
	internal class UIModeToBorderThicknessConverter : IValueConverter
	{
		// Token: 0x06000CE4 RID: 3300 RVA: 0x000418C8 File Offset: 0x0003FAC8
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			object obj;
			try
			{
				string currentAppTheme = ConfigManager.GetCurrentAppTheme();
				if (currentAppTheme == "Auto")
				{
					if (new SystemThemeListener().ActualAppTheme == SystemThemeListener.ActualTheme.Light)
					{
						obj = 1;
					}
					else
					{
						obj = 0.5;
					}
				}
				else if (currentAppTheme == "Light")
				{
					obj = 1;
				}
				else
				{
					obj = 0.5;
				}
			}
			catch
			{
				obj = 1;
			}
			return obj;
		}

		// Token: 0x06000CE5 RID: 3301 RVA: 0x00041954 File Offset: 0x0003FB54
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
