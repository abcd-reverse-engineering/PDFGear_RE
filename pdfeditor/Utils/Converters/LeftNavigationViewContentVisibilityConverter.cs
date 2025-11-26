using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using pdfeditor.Models.LeftNavigations;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000FD RID: 253
	public class LeftNavigationViewContentVisibilityConverter : IValueConverter
	{
		// Token: 0x06000C84 RID: 3204 RVA: 0x00040C50 File Offset: 0x0003EE50
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			NavigationModel navigationModel = value as NavigationModel;
			if (navigationModel != null)
			{
				return (navigationModel.Name == parameter as string) ? Visibility.Visible : Visibility.Collapsed;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x00040C8A File Offset: 0x0003EE8A
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
