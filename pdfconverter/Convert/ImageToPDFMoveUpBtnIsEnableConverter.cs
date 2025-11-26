using System;
using System.Globalization;
using System.Windows.Data;
using pdfconverter.Models;

namespace pdfconverter.Convert
{
	// Token: 0x02000095 RID: 149
	public class ImageToPDFMoveUpBtnIsEnableConverter : IMultiValueConverter
	{
		// Token: 0x060006A6 RID: 1702 RVA: 0x00017EF0 File Offset: 0x000160F0
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
						if (toPDFFileItemCollection.IndexOf(toPDFFileItem) == 0)
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

		// Token: 0x060006A7 RID: 1703 RVA: 0x00017F50 File Offset: 0x00016150
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
