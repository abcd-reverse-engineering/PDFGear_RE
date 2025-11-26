using System;
using System.Drawing;
using pdfconverter.Models;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

namespace pdfconverter.Utils
{
	// Token: 0x02000042 RID: 66
	public static class ParaConvert
	{
		// Token: 0x0600050F RID: 1295 RVA: 0x00014D58 File Offset: 0x00012F58
		public static SizeF GetPdfPagesize(PageSizeItem size)
		{
			switch (size.PDFPageSize)
			{
			case PDFPageSize.MatchSource:
				return new SizeF(0f, 0f);
			case PDFPageSize.A4_Portrait:
				return PdfPageSize.A4;
			case PDFPageSize.A4_Landscape:
				return new SizeF(PdfPageSize.A4.Height, PdfPageSize.A4.Width);
			case PDFPageSize.A3_Portrait:
				return PdfPageSize.A3;
			case PDFPageSize.A3_Landscape:
				return new SizeF(PdfPageSize.A3.Height, PdfPageSize.A3.Width);
			default:
				return PdfPageSize.A4;
			}
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x00014DEC File Offset: 0x00012FEC
		public static PdfPageOrientation GetPdfeOrientation(PageSizeItem size)
		{
			switch (size.PDFPageSize)
			{
			case PDFPageSize.A4_Portrait:
			case PDFPageSize.A3_Portrait:
				return PdfPageOrientation.Portrait;
			case PDFPageSize.A4_Landscape:
			case PDFPageSize.A3_Landscape:
				return PdfPageOrientation.Landscape;
			default:
				return PdfPageOrientation.Portrait;
			}
		}

		// Token: 0x06000511 RID: 1297 RVA: 0x00014E20 File Offset: 0x00013020
		public static PdfMargins GetMargins(PageMarginItem size)
		{
			PdfMargins pdfMargins = new PdfMargins();
			switch (size.PDFPageSize)
			{
			case ContentMargin.NoMargin:
				pdfMargins.All = 0f;
				return pdfMargins;
			case ContentMargin.BigMargin:
				pdfMargins.Left = ParaConvert.GetPixValue(3.18);
				pdfMargins.Right = ParaConvert.GetPixValue(3.18);
				pdfMargins.Top = ParaConvert.GetPixValue(2.54);
				pdfMargins.Bottom = ParaConvert.GetPixValue(2.54);
				return pdfMargins;
			case ContentMargin.SmallMargin:
				pdfMargins.All = ParaConvert.GetPixValue(1.27);
				return pdfMargins;
			default:
				return pdfMargins;
			}
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x00014EC4 File Offset: 0x000130C4
		private static float GetPixValue(double CM)
		{
			return (float)(CM * 72.0 / 2.54);
		}
	}
}
