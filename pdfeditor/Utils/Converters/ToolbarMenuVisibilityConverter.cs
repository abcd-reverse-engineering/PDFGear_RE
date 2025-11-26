using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using pdfeditor.Models.Menus;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x0200011A RID: 282
	public class ToolbarMenuVisibilityConverter : IValueConverter
	{
		// Token: 0x06000CDB RID: 3291 RVA: 0x000417E0 File Offset: 0x0003F9E0
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			MainMenuGroup mainMenuGroup = value as MainMenuGroup;
			if (mainMenuGroup != null)
			{
				string text = parameter as string;
				if (text != null)
				{
					return (mainMenuGroup.Tag == text) ? Visibility.Visible : Visibility.Collapsed;
				}
			}
			return Visibility.Collapsed;
		}

		// Token: 0x06000CDC RID: 3292 RVA: 0x0004181F File Offset: 0x0003FA1F
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
