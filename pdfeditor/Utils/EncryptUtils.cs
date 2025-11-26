using System;
using Patagames.Pdf;
using Patagames.Pdf.Net;

namespace pdfeditor.Utils
{
	// Token: 0x0200007A RID: 122
	public static class EncryptUtils
	{
		// Token: 0x060008D4 RID: 2260 RVA: 0x0002BFBC File Offset: 0x0002A1BC
		public static bool VerifyOwerpassword(string pdfpath, string password)
		{
			PdfDocument pdfDocument = null;
			bool flag;
			try
			{
				pdfDocument = PdfDocument.Load(pdfpath, null, password);
				if (Pdfium.FPDF_IsOwnerPasswordIsUsed(pdfDocument.Handle))
				{
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			catch
			{
				flag = false;
			}
			finally
			{
				if (pdfDocument != null && pdfDocument != null)
				{
					pdfDocument.Dispose();
				}
				pdfDocument = null;
			}
			return flag;
		}
	}
}
