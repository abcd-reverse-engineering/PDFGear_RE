using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x02000067 RID: 103
	public class MergeFileItemMoveUpBtnIsEnabledConverter : IMultiValueConverter
	{
		// Token: 0x060005D1 RID: 1489 RVA: 0x000167DC File Offset: 0x000149DC
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				MergeFileItem mergeFileItem = values[0] as MergeFileItem;
				if (mergeFileItem != null)
				{
					MergeFileItemCollection mergeFileItemCollection = values[1] as MergeFileItemCollection;
					if (mergeFileItemCollection != null)
					{
						if (mergeFileItemCollection.IndexOf(mergeFileItem) == 0)
						{
							return false;
						}
						return true;
					}
				}
			}
			catch
			{
			}
			return true;
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x0001683C File Offset: 0x00014A3C
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
