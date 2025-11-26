using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x02000060 RID: 96
	public class MergeStatusToPageCountVisibility : IValueConverter
	{
		// Token: 0x060005AF RID: 1455 RVA: 0x00016484 File Offset: 0x00014684
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			MergeStatus mergeStatus = (MergeStatus)value;
			if (mergeStatus == MergeStatus.Init || mergeStatus == MergeStatus.Loading || mergeStatus == MergeStatus.Unsupport || mergeStatus == MergeStatus.LoadedFailed)
			{
				return Visibility.Collapsed;
			}
			return Visibility.Visible;
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x000164B4 File Offset: 0x000146B4
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
