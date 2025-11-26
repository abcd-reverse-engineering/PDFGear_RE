using System;
using Patagames.Pdf.Net;

namespace pdfeditor.Models.PageContents
{
	// Token: 0x0200014F RID: 335
	public static class PageContentFactory
	{
		// Token: 0x0600141A RID: 5146 RVA: 0x0005022C File Offset: 0x0004E42C
		public static PageBaseObject Create(PdfPageObject pageObject)
		{
			if (pageObject == null)
			{
				return null;
			}
			PageBaseObject pageBaseObject = null;
			if (pageObject is PdfTextObject)
			{
				pageBaseObject = new PageTextObject();
			}
			if (pageBaseObject != null)
			{
				PageBaseObject.InitModelProperties(pageObject, pageBaseObject);
			}
			return pageBaseObject;
		}
	}
}
