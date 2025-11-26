using System;
using System.Collections.Generic;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;

namespace pdfeditor.Utils
{
	// Token: 0x0200009B RID: 155
	public static class PopupAnnotationWrapperExtensions
	{
		// Token: 0x060009F4 RID: 2548 RVA: 0x000327A4 File Offset: 0x000309A4
		public static void SetIsOpen(PdfPopupAnnotation annot, bool value)
		{
			bool flag;
			if (annot == null)
			{
				flag = null != null;
			}
			else
			{
				PdfPage page = annot.Page;
				flag = ((page != null) ? page.Document : null) != null;
			}
			if (!flag)
			{
				throw new ArgumentNullException("annot");
			}
			PdfAnnotation parent = annot.Parent;
			string text = ((parent != null) ? parent.Name : null);
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = string.Format("{0:X2}_{1}_{2}", annot.Page.Document.Handle.ToInt64(), annot.Page.PageIndex, text);
				PopupAnnotationWrapperExtensions.popupIsOpenCache[text2] = value;
			}
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x00032838 File Offset: 0x00030A38
		public static bool GetIsOpen(PdfPopupAnnotation annot)
		{
			bool flag;
			if (annot == null)
			{
				flag = null != null;
			}
			else
			{
				PdfPage page = annot.Page;
				flag = ((page != null) ? page.Document : null) != null;
			}
			if (!flag)
			{
				throw new ArgumentNullException("annot");
			}
			PdfAnnotation parent = annot.Parent;
			string text = ((parent != null) ? parent.Name : null);
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = string.Format("{0:X2}_{1}_{2}", annot.Page.Document.Handle.ToInt64(), annot.Page.PageIndex, text);
				bool flag2;
				if (PopupAnnotationWrapperExtensions.popupIsOpenCache.TryGetValue(text2, out flag2))
				{
					return flag2;
				}
			}
			return annot.IsOpen;
		}

		// Token: 0x04000484 RID: 1156
		private static Dictionary<string, bool> popupIsOpenCache = new Dictionary<string, bool>();
	}
}
