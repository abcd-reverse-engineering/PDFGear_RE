using System;
using System.Collections.Generic;
using CommonLib.Common;

namespace pdfconverter.Models
{
	// Token: 0x0200005D RID: 93
	public static class MapOnlineConverttype
	{
		// Token: 0x060005A6 RID: 1446 RVA: 0x000162BF File Offset: 0x000144BF
		public static string GetOnlineConvertStr(ConvFromPDFType id)
		{
			if (MapOnlineConverttype.OnlineConvertType.ContainsKey(id))
			{
				return MapOnlineConverttype.OnlineConvertType[id];
			}
			return string.Empty;
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x000162DF File Offset: 0x000144DF
		public static string GetOnlineExtensionStr(string convertype)
		{
			if (MapOnlineConverttype.targetInfoDic.ContainsKey(convertype))
			{
				return MapOnlineConverttype.targetInfoDic[convertype];
			}
			return string.Empty;
		}

		// Token: 0x040002AB RID: 683
		private static readonly Dictionary<ConvFromPDFType, string> OnlineConvertType = new Dictionary<ConvFromPDFType, string>
		{
			{
				ConvFromPDFType.PDFToWord,
				"pdf2docx"
			},
			{
				ConvFromPDFType.PDFToExcel,
				"pdf2xlsx"
			},
			{
				ConvFromPDFType.PDFToPng,
				"pdf2png"
			},
			{
				ConvFromPDFType.PDFToJpg,
				"pdf2jpeg"
			},
			{
				ConvFromPDFType.PDFToRTF,
				"pdf2rtf"
			},
			{
				ConvFromPDFType.PDFToPPT,
				"pdf2pptx"
			}
		};

		// Token: 0x040002AC RID: 684
		private static readonly Dictionary<string, string> targetInfoDic = new Dictionary<string, string>
		{
			{ "pdf2docx", ".docx" },
			{ "pdf2xlsx", ".xlsx" },
			{ "pdf2png", ".zip" },
			{ "pdf2jpeg", ".zip" },
			{ "pdf2rtf", ".rtf" }
		};
	}
}
