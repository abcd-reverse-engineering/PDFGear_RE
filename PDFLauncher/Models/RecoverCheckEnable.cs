using System;
using System.Globalization;
using System.Windows.Data;

namespace PDFLauncher.Models
{
	// Token: 0x0200001E RID: 30
	public class RecoverCheckEnable : IValueConverter
	{
		// Token: 0x060001A5 RID: 421 RVA: 0x00006AE6 File Offset: 0x00004CE6
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((RecoverStatus)value == RecoverStatus.Prepare)
			{
				return true;
			}
			return false;
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00006AFD File Offset: 0x00004CFD
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
