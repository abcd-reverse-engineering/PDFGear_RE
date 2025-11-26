using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x020000FC RID: 252
	public class IsCollatedToImageSourceConverterr : IValueConverter
	{
		// Token: 0x06000C81 RID: 3201 RVA: 0x00040BE0 File Offset: 0x0003EDE0
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)
			{
				return null;
			}
			if (!(value is bool) || !(bool)value)
			{
				return new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Printer/NotCollated.png"));
			}
			return new BitmapImage(new Uri("pack://application:,,,/Style/Resources/Printer/PageCollated.png"));
		}

		// Token: 0x06000C82 RID: 3202 RVA: 0x00040C3E File Offset: 0x0003EE3E
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
