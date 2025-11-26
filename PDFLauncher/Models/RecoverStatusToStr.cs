using System;
using System.Globalization;
using System.Windows.Data;
using PDFLauncher.Properties;

namespace PDFLauncher.Models
{
	// Token: 0x02000021 RID: 33
	public class RecoverStatusToStr : IValueConverter
	{
		// Token: 0x060001AE RID: 430 RVA: 0x00006B98 File Offset: 0x00004D98
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			RecoverStatus recoverStatus = (RecoverStatus)value;
			string text = "";
			if (recoverStatus == RecoverStatus.Prepare)
			{
				text = "";
			}
			if (recoverStatus == RecoverStatus.Converted)
			{
				text = Resources.WinRecoverSucceedContent;
			}
			if (recoverStatus == RecoverStatus.Recovering)
			{
				text = Resources.WinRecoveringContent;
			}
			return text;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00006BCE File Offset: 0x00004DCE
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
