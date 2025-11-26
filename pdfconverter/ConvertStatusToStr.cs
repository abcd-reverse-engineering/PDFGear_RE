using System;
using System.Globalization;
using System.Windows.Data;
using pdfconverter.Properties;

namespace pdfconverter
{
	// Token: 0x02000008 RID: 8
	public class ConvertStatusToStr : IValueConverter
	{
		// Token: 0x06000066 RID: 102 RVA: 0x00002108 File Offset: 0x00000308
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			FileCovertStatus fileCovertStatus = (FileCovertStatus)value;
			string text = "";
			switch (fileCovertStatus)
			{
			case FileCovertStatus.ConvertInit:
				text = Resources.FileCovertStatusInit;
				break;
			case FileCovertStatus.ConvertLoading:
				text = Resources.FileConvertStatusLoading;
				break;
			case FileCovertStatus.ConvertLoaded:
				text = Resources.FileCovertStatusLoaded;
				break;
			case FileCovertStatus.ConvertLoadedFailed:
				text = Resources.WinConvertLoadedFailed;
				break;
			case FileCovertStatus.ConvertUnsupport:
				text = Resources.FileCovertStatusUnsupported;
				break;
			case FileCovertStatus.ConvertCoverting:
				text = Resources.FileConvertStatusConverting;
				break;
			case FileCovertStatus.ConvertFail:
				text = Resources.FileConvertStatusConvertFail;
				break;
			case FileCovertStatus.ConvertSucc:
				text = Resources.FileConvertStatusConvertSucc;
				break;
			}
			return text;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00002189 File Offset: 0x00000389
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
