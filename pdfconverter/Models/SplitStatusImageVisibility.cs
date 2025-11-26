using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x0200007C RID: 124
	public class SplitStatusImageVisibility : IValueConverter
	{
		// Token: 0x06000604 RID: 1540 RVA: 0x00016BAC File Offset: 0x00014DAC
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SplitStatus splitStatus = (SplitStatus)value;
			if (splitStatus == SplitStatus.Init || splitStatus == SplitStatus.Loading || splitStatus == SplitStatus.Loaded || splitStatus == SplitStatus.Spliting)
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x00016BDC File Offset: 0x00014DDC
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
