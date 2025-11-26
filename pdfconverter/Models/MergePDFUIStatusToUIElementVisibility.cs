using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x0200006D RID: 109
	public class MergePDFUIStatusToUIElementVisibility : IValueConverter
	{
		// Token: 0x060005D9 RID: 1497 RVA: 0x000168D7 File Offset: 0x00014AD7
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((MergePDFUIStatus)value == MergePDFUIStatus.Succ)
			{
				return Visibility.Visible;
			}
			return Visibility.Collapsed;
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x000168EF File Offset: 0x00014AEF
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
