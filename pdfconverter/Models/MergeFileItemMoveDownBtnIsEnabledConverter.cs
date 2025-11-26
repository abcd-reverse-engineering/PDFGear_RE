using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfconverter.Models
{
	// Token: 0x02000068 RID: 104
	public class MergeFileItemMoveDownBtnIsEnabledConverter : IMultiValueConverter
	{
		// Token: 0x060005D4 RID: 1492 RVA: 0x0001684C File Offset: 0x00014A4C
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
						int num = mergeFileItemCollection.IndexOf(mergeFileItem);
						int num2 = mergeFileItemCollection.Count - 1;
						if (num == num2)
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

		// Token: 0x060005D5 RID: 1493 RVA: 0x000168B8 File Offset: 0x00014AB8
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
