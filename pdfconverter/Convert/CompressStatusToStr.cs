using System;
using System.Globalization;
using System.Windows.Data;
using pdfconverter.Models;
using pdfconverter.Properties;

namespace pdfconverter.Convert
{
	// Token: 0x0200008F RID: 143
	public class CompressStatusToStr : IValueConverter
	{
		// Token: 0x06000694 RID: 1684 RVA: 0x00017CB0 File Offset: 0x00015EB0
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ToPDFItemStatus toPDFItemStatus = (ToPDFItemStatus)value;
			string text = "";
			switch (toPDFItemStatus)
			{
			case ToPDFItemStatus.Init:
				text = Resources.FileCovertStatusInit;
				break;
			case ToPDFItemStatus.Loading:
				text = Resources.FileConvertStatusLoading;
				break;
			case ToPDFItemStatus.Loaded:
				text = Resources.FileCovertStatusLoaded;
				break;
			case ToPDFItemStatus.LoadedFailed:
				text = Resources.WinConvertLoadedFailed;
				break;
			case ToPDFItemStatus.Unsupport:
				text = Resources.FileCovertStatusUnsupported;
				break;
			case ToPDFItemStatus.Working:
				text = Resources.MainWinCompressCompressing;
				break;
			case ToPDFItemStatus.Fail:
				text = Resources.MainWinCompressCompresFaild;
				break;
			case ToPDFItemStatus.Succ:
				text = Resources.MainWinCompressCompresComplete;
				break;
			case ToPDFItemStatus.Queuing:
				text = Resources.MainWinCompressCompresQueue;
				break;
			}
			return text;
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x00017D3D File Offset: 0x00015F3D
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
