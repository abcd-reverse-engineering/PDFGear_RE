using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using pdfconverter.Models;

namespace pdfconverter.Utils.Converters
{
	// Token: 0x0200004C RID: 76
	public class ActionViewVisibilityConverter : IValueConverter
	{
		// Token: 0x06000543 RID: 1347 RVA: 0x00015A40 File Offset: 0x00013C40
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ActionMenuGroup actionMenuGroup = value as ActionMenuGroup;
			if (actionMenuGroup != null)
			{
				string text = parameter as string;
				if (text != null)
				{
					return (actionMenuGroup.Tag == text) ? Visibility.Visible : Visibility.Collapsed;
				}
			}
			return Visibility.Collapsed;
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x00015A7F File Offset: 0x00013C7F
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
