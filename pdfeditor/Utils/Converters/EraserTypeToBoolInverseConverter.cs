using System;
using System.Globalization;
using System.Windows.Data;
using pdfeditor.Models.Menus.ToolbarSettings;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000F5 RID: 245
	public class EraserTypeToBoolInverseConverter : IValueConverter
	{
		// Token: 0x06000C6C RID: 3180 RVA: 0x00040AAC File Offset: 0x0003ECAC
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (ToolbarSettingInkEraserModel.EraserType)value == ToolbarSettingInkEraserModel.EraserType.Partial;
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x00040ABC File Offset: 0x0003ECBC
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ToolbarSettingInkEraserModel.EraserType.Partial;
		}
	}
}
