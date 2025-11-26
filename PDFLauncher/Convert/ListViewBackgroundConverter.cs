using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PDFLauncher.Convert
{
	// Token: 0x0200002A RID: 42
	public class ListViewBackgroundConverter : IValueConverter
	{
		// Token: 0x060001FD RID: 509 RVA: 0x00007750 File Offset: 0x00005950
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ListViewItem listViewItem = (ListViewItem)value;
			bool flag = (ItemsControl.ItemsControlFromItemContainer(listViewItem) as ListView).ItemContainerGenerator.IndexFromContainer(listViewItem) != 0;
			BrushConverter brushConverter = new BrushConverter();
			if ((flag ? 1 : 0) % 2 == 0)
			{
				return (Brush)brushConverter.ConvertFromString("#FFFFFF");
			}
			return (Brush)brushConverter.ConvertFromString("#F5F5F5");
		}

		// Token: 0x060001FE RID: 510 RVA: 0x000077A5 File Offset: 0x000059A5
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((Brush)value).ToString();
		}
	}
}
