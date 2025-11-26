using System;
using System.Globalization;
using System.Windows.Data;
using pdfconverter.Models;
using pdfconverter.Properties;

namespace pdfconverter.Convert
{
	// Token: 0x02000092 RID: 146
	public class ConvertStatusToStr : IValueConverter
	{
		// Token: 0x0600069D RID: 1693 RVA: 0x00017DB0 File Offset: 0x00015FB0
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
				text = Resources.FileConvertStatusConverting;
				break;
			case ToPDFItemStatus.Fail:
				text = Resources.FileConvertStatusConvertFail;
				break;
			case ToPDFItemStatus.Succ:
				text = Resources.FileConvertStatusConvertSucc;
				break;
			case ToPDFItemStatus.Queuing:
				text = "Queue";
				break;
			}
			return text;
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x00017E3D File Offset: 0x0001603D
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
