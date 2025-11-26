using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace pdfconverter.Models
{
	// Token: 0x02000065 RID: 101
	public class MergeStatusImage : IValueConverter
	{
		// Token: 0x060005BE RID: 1470 RVA: 0x000165A4 File Offset: 0x000147A4
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			MergeStatus mergeStatus = (MergeStatus)value;
			string text = "";
			switch (mergeStatus)
			{
			case MergeStatus.Init:
			case MergeStatus.Loading:
			case MergeStatus.Loaded:
			case MergeStatus.Merging:
				text = "";
				break;
			case MergeStatus.LoadedFailed:
				text = "pack://application:,,,/images/warning.png";
				break;
			case MergeStatus.Unsupport:
				text = "pack://application:,,,/images/warning.png";
				break;
			case MergeStatus.Fail:
				text = "pack://application:,,,/images/warning.png";
				break;
			case MergeStatus.Succ:
				text = "pack://application:,,,/images/converted.png";
				break;
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				return null;
			}
			return new BitmapImage(new Uri(text));
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x00016621 File Offset: 0x00014821
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
