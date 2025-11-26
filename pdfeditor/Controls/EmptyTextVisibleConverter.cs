using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfeditor.Controls
{
	// Token: 0x020001B5 RID: 437
	public class EmptyTextVisibleConverter : IValueConverter
	{
		// Token: 0x060018EB RID: 6379 RVA: 0x00060578 File Offset: 0x0005E778
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string text = value as string;
			if (text != null && !string.IsNullOrEmpty(text))
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x060018EC RID: 6380 RVA: 0x000605A4 File Offset: 0x0005E7A4
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
