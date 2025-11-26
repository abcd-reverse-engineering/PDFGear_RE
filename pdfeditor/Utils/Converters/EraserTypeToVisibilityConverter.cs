using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using pdfeditor.Models.Menus.ToolbarSettings;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000F3 RID: 243
	public class EraserTypeToVisibilityConverter : IValueConverter
	{
		// Token: 0x06000C66 RID: 3174 RVA: 0x00040A5D File Offset: 0x0003EC5D
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((ToolbarSettingInkEraserModel.EraserType)value == ToolbarSettingInkEraserModel.EraserType.Partial) ? Visibility.Visible : Visibility.Collapsed;
		}

		// Token: 0x06000C67 RID: 3175 RVA: 0x00040A71 File Offset: 0x0003EC71
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((Visibility)value == Visibility.Visible) ? ToolbarSettingInkEraserModel.EraserType.Partial : ToolbarSettingInkEraserModel.EraserType.Whole;
		}
	}
}
