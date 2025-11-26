using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PDFLauncher.Models
{
	// Token: 0x02000022 RID: 34
	public class RecoverStatusImage : IValueConverter
	{
		// Token: 0x060001B1 RID: 433 RVA: 0x00006BE0 File Offset: 0x00004DE0
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			RecoverStatus recoverStatus = (RecoverStatus)value;
			string text = "";
			if (recoverStatus == RecoverStatus.Recovering)
			{
				text = "pack://application:,,,/images/convering.png";
			}
			if (recoverStatus == RecoverStatus.Converted)
			{
				text = "pack://application:,,,/images/converted.png";
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				return null;
			}
			return new BitmapImage(new Uri(text));
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00006C21 File Offset: 0x00004E21
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
