using System;
using System.Globalization;
using System.Windows.Data;
using pdfconverter.Models;

namespace pdfconverter.Convert
{
	// Token: 0x02000094 RID: 148
	public class ImageToPDFMoveDownBtnIsEnableConverter : IMultiValueConverter
	{
		// Token: 0x060006A3 RID: 1699 RVA: 0x00017E74 File Offset: 0x00016074
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				ToPDFFileItem toPDFFileItem = values[0] as ToPDFFileItem;
				if (toPDFFileItem != null)
				{
					ToPDFFileItemCollection toPDFFileItemCollection = values[1] as ToPDFFileItemCollection;
					if (toPDFFileItemCollection != null)
					{
						int num = toPDFFileItemCollection.IndexOf(toPDFFileItem);
						int num2 = toPDFFileItemCollection.Count - 1;
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

		// Token: 0x060006A4 RID: 1700 RVA: 0x00017EE0 File Offset: 0x000160E0
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
