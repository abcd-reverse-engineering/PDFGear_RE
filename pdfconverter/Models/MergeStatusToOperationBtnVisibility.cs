using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x02000062 RID: 98
	public class MergeStatusToOperationBtnVisibility : IValueConverter
	{
		// Token: 0x060005B5 RID: 1461 RVA: 0x00016500 File Offset: 0x00014700
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			MergeStatus mergeStatus = (MergeStatus)value;
			if (mergeStatus == MergeStatus.Init || mergeStatus == MergeStatus.Loading || mergeStatus == MergeStatus.Merging)
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x0001652C File Offset: 0x0001472C
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
