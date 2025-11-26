using System;
using System.Globalization;
using System.Windows.Data;
using pdfeditor.Utils.Enums;

namespace pdfeditor.Utils.Converters
{
	// Token: 0x02000116 RID: 278
	public class SubViewModePageConverter : IValueConverter
	{
		// Token: 0x06000CCF RID: 3279 RVA: 0x00041630 File Offset: 0x0003F830
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SubViewModePage subViewModePage = (SubViewModePage)value;
			string text = (string)parameter;
			if (subViewModePage == SubViewModePage.SinglePage && text == "SingalPage")
			{
				return true;
			}
			if (subViewModePage == SubViewModePage.DoublePages && text == "DoublePages")
			{
				return true;
			}
			return false;
		}

		// Token: 0x06000CD0 RID: 3280 RVA: 0x00041680 File Offset: 0x0003F880
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool flag = (bool)value;
			string text = (string)parameter;
			if (flag)
			{
				if (text == "SingalPage")
				{
					return SubViewModePage.SinglePage;
				}
				if (text == "DoublePages")
				{
					return SubViewModePage.DoublePages;
				}
			}
			return null;
		}
	}
}
