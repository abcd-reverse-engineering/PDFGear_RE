using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace pdfconverter.Models
{
	// Token: 0x0200007D RID: 125
	public class SplitStatusImage : IValueConverter
	{
		// Token: 0x06000607 RID: 1543 RVA: 0x00016BEC File Offset: 0x00014DEC
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SplitStatus splitStatus = (SplitStatus)value;
			string text = "";
			switch (splitStatus)
			{
			case SplitStatus.Init:
			case SplitStatus.Loading:
			case SplitStatus.Loaded:
			case SplitStatus.Spliting:
				text = "";
				break;
			case SplitStatus.LoadedFailed:
				text = "pack://application:,,,/images/warning.png";
				break;
			case SplitStatus.Unsupport:
				text = "pack://application:,,,/images/warning.png";
				break;
			case SplitStatus.Fail:
				text = "pack://application:,,,/images/warning.png";
				break;
			case SplitStatus.Succ:
				text = "pack://application:,,,/images/converted.png";
				break;
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				return null;
			}
			return new BitmapImage(new Uri(text));
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x00016C69 File Offset: 0x00014E69
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
