using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x02000064 RID: 100
	public class MergeStatusImageVisibility : IValueConverter
	{
		// Token: 0x060005BB RID: 1467 RVA: 0x00016564 File Offset: 0x00014764
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			MergeStatus mergeStatus = (MergeStatus)value;
			if (mergeStatus == MergeStatus.Init || mergeStatus == MergeStatus.Loading || mergeStatus == MergeStatus.Loaded || mergeStatus == MergeStatus.Merging)
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		// Token: 0x060005BC RID: 1468 RVA: 0x00016594 File Offset: 0x00014794
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
