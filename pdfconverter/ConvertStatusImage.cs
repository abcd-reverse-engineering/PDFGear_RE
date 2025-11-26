using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace pdfconverter
{
	// Token: 0x0200000E RID: 14
	public class ConvertStatusImage : IValueConverter
	{
		// Token: 0x06000078 RID: 120 RVA: 0x0000227C File Offset: 0x0000047C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			FileCovertStatus fileCovertStatus = (FileCovertStatus)value;
			string text = "";
			switch (fileCovertStatus)
			{
			case FileCovertStatus.ConvertLoaded:
				text = "";
				break;
			case FileCovertStatus.ConvertLoadedFailed:
				text = "pack://application:,,,/images/warning.png";
				break;
			case FileCovertStatus.ConvertUnsupport:
				text = "pack://application:,,,/images/warning.png";
				break;
			case FileCovertStatus.ConvertFail:
				text = "pack://application:,,,/images/warning.png";
				break;
			case FileCovertStatus.ConvertSucc:
				text = "pack://application:,,,/images/converted.png";
				break;
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				return null;
			}
			return new BitmapImage(new Uri(text));
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000022F9 File Offset: 0x000004F9
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
