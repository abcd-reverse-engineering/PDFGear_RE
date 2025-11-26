using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x02000061 RID: 97
	public class MergeStatusToFileItemEnabled : IValueConverter
	{
		// Token: 0x060005B2 RID: 1458 RVA: 0x000164C4 File Offset: 0x000146C4
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			MergeStatus mergeStatus = (MergeStatus)value;
			if (mergeStatus == MergeStatus.Unsupport || mergeStatus == MergeStatus.Merging || mergeStatus == MergeStatus.LoadedFailed)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x000164F1 File Offset: 0x000146F1
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
