using System;
using System.Globalization;
using System.Windows.Data;

namespace pdfeditor.Controls
{
	// Token: 0x020001D6 RID: 470
	public class TransitionAnimationConverter : IMultiValueConverter
	{
		// Token: 0x06001A8F RID: 6799 RVA: 0x0006A810 File Offset: 0x00068A10
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length == 2)
			{
				try
				{
					double num = global::System.Convert.ToDouble(values[0], culture);
					return global::System.Convert.ToDouble(values[1], culture) * num;
				}
				catch
				{
				}
			}
			return 0.0;
		}

		// Token: 0x06001A90 RID: 6800 RVA: 0x0006A868 File Offset: 0x00068A68
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
