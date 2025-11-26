using System;
using System.Windows;
using Patagames.Pdf.Net;

namespace pdfeditor.Utils
{
	// Token: 0x0200008A RID: 138
	public class PageImageUtils
	{
		// Token: 0x06000930 RID: 2352 RVA: 0x0002D940 File Offset: 0x0002BB40
		public static bool ImageTestHitTest(PdfPage page, Point point, out int Index)
		{
			int num = -1;
			Index = -1;
			foreach (PdfPageObject pdfPageObject in page.PageObjects)
			{
				num++;
				PdfImageObject pdfImageObject = pdfPageObject as PdfImageObject;
				if (pdfImageObject != null && point.X >= (double)(pdfImageObject.BoundingBox.left - 5f) && point.X <= (double)(pdfImageObject.BoundingBox.right + 5f) && point.Y >= (double)(pdfImageObject.BoundingBox.bottom - 5f) && point.Y <= (double)(pdfImageObject.BoundingBox.top + 5f))
				{
					Index = num;
				}
			}
			return Index != -1;
		}
	}
}
