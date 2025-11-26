using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using pdfconverter.Models;

namespace pdfconverter.Convert
{
	// Token: 0x02000096 RID: 150
	public class IsOutputInOneFileToBrowseBtnVisibilityConvert : IMultiValueConverter
	{
		// Token: 0x060006A9 RID: 1705 RVA: 0x00017F60 File Offset: 0x00016160
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			object obj;
			try
			{
				if ((bool)values[1])
				{
					obj = Visibility.Collapsed;
				}
				else if ((ToPDFItemStatus)values[0] == ToPDFItemStatus.Succ && (WorkQueenState)values[2] == WorkQueenState.Succ)
				{
					obj = Visibility.Visible;
				}
				else
				{
					obj = Visibility.Collapsed;
				}
			}
			catch
			{
				obj = Visibility.Collapsed;
			}
			return obj;
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x00017FC4 File Offset: 0x000161C4
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
