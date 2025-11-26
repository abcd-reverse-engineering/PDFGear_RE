using System;
using System.Globalization;
using System.Windows.Data;
using pdfeditor.Models.Menus.ToolbarSettings;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000F4 RID: 244
	public class EraserTypeToBoolConverter : IValueConverter
	{
		// Token: 0x06000C69 RID: 3177 RVA: 0x00040A8C File Offset: 0x0003EC8C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (ToolbarSettingInkEraserModel.EraserType)value == ToolbarSettingInkEraserModel.EraserType.Whole;
		}

		// Token: 0x06000C6A RID: 3178 RVA: 0x00040A9C File Offset: 0x0003EC9C
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ToolbarSettingInkEraserModel.EraserType.Whole;
		}
	}
}
