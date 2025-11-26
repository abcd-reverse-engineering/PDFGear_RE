using System;
using System.Globalization;
using System.Windows.Data;
using pdfconverter.Properties;

namespace pdfconverter.Models
{
	// Token: 0x02000075 RID: 117
	public class SplitStatusToStr : IValueConverter
	{
		// Token: 0x060005EF RID: 1519 RVA: 0x000169DC File Offset: 0x00014BDC
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SplitStatus splitStatus = (SplitStatus)value;
			string text = "";
			switch (splitStatus)
			{
			case SplitStatus.Init:
				text = Resources.FileCovertStatusInit;
				break;
			case SplitStatus.Loading:
				text = Resources.FileConvertStatusLoading;
				break;
			case SplitStatus.Loaded:
				text = Resources.FileCovertStatusLoaded;
				break;
			case SplitStatus.LoadedFailed:
				text = Resources.WinConvertLoadedFailed;
				break;
			case SplitStatus.Unsupport:
				text = Resources.FileCovertStatusUnsupported;
				break;
			case SplitStatus.Spliting:
				text = Resources.WinMergeSplitSplitStatusSplitting;
				break;
			case SplitStatus.Fail:
				text = Resources.WinMergeSplitSplitStatusFail;
				break;
			case SplitStatus.Succ:
				text = Resources.WinMergeSplitSplitStatusSucc;
				break;
			}
			return text;
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x00016A5D File Offset: 0x00014C5D
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
