using System;
using System.Globalization;
using System.Windows.Data;
using pdfconverter.Properties;

namespace pdfconverter.Models
{
	// Token: 0x0200005E RID: 94
	public class MergeStatusToStr : IValueConverter
	{
		// Token: 0x060005A9 RID: 1449 RVA: 0x000163BC File Offset: 0x000145BC
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			MergeStatus mergeStatus = (MergeStatus)value;
			string text = "";
			switch (mergeStatus)
			{
			case MergeStatus.Init:
				text = Resources.FileCovertStatusInit;
				break;
			case MergeStatus.Loading:
				text = Resources.FileConvertStatusLoading;
				break;
			case MergeStatus.Loaded:
				text = Resources.FileCovertStatusLoaded;
				break;
			case MergeStatus.LoadedFailed:
				text = Resources.WinConvertLoadedFailed;
				break;
			case MergeStatus.Unsupport:
				text = Resources.FileCovertStatusUnsupported;
				break;
			case MergeStatus.Merging:
				text = Resources.WinMergeSplitMergeStatusMerging;
				break;
			case MergeStatus.Fail:
				text = Resources.WinMergeSplitMergeStatusFail;
				break;
			case MergeStatus.Succ:
				text = Resources.WinMergeSplitMergeStatusSucc;
				break;
			}
			return text;
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x0001643D File Offset: 0x0001463D
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
