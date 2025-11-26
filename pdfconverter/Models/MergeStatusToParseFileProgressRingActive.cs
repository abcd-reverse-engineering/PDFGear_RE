using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x0200005F RID: 95
	public class MergeStatusToParseFileProgressRingActive : IValueConverter
	{
		// Token: 0x060005AC RID: 1452 RVA: 0x0001644C File Offset: 0x0001464C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			MergeStatus mergeStatus = (MergeStatus)value;
			if (mergeStatus == MergeStatus.Init || mergeStatus == MergeStatus.Loading)
			{
				return true;
			}
			return false;
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x00016474 File Offset: 0x00014674
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
