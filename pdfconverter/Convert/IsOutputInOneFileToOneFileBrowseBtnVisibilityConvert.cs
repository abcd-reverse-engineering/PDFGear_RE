using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using pdfconverter.Models;

namespace pdfconverter.Convert
{
	// Token: 0x02000098 RID: 152
	internal class IsOutputInOneFileToOneFileBrowseBtnVisibilityConvert : IMultiValueConverter
	{
		// Token: 0x060006AF RID: 1711 RVA: 0x00017FFC File Offset: 0x000161FC
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			object obj;
			try
			{
				if ((bool)values[1] && (WorkQueenState)values[0] == WorkQueenState.Succ)
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

		// Token: 0x060006B0 RID: 1712 RVA: 0x0001804C File Offset: 0x0001624C
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
