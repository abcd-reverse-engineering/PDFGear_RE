using System;
using System.Globalization;
using System.Windows.Data;

namespace PDFLauncher.Models
{
	// Token: 0x02000020 RID: 32
	public class RecoverBtnEnable : IMultiValueConverter
	{
		// Token: 0x060001AB RID: 427 RVA: 0x00006B34 File Offset: 0x00004D34
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null || values.Length == 0)
			{
				throw new ArgumentNullException("values can not be null");
			}
			string text = values[0].ToString().Trim();
			int num = int.Parse(values[1].ToString());
			if (text.Length > 0 && num > 0)
			{
				return true;
			}
			return false;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00006B87 File Offset: 0x00004D87
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
