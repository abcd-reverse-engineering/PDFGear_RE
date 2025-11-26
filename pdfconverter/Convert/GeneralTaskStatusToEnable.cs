using System;
using System.Globalization;
using System.Windows.Data;
using pdfconverter.Models;

namespace pdfconverter.Convert
{
	// Token: 0x02000093 RID: 147
	public class GeneralTaskStatusToEnable : IValueConverter
	{
		// Token: 0x060006A0 RID: 1696 RVA: 0x00017E4C File Offset: 0x0001604C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((WorkQueenState)value == WorkQueenState.Working)
			{
				return false;
			}
			return true;
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x00017E64 File Offset: 0x00016064
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
