using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Models.Annotations;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Utils
{
	// Token: 0x02000083 RID: 131
	public static class LinkOperationManagerExtensions
	{
		// Token: 0x06000900 RID: 2304 RVA: 0x0002CBE8 File Offset: 0x0002ADE8
		public static async void LinkDeleteAllUndo(Dictionary<int, List<BaseAnnotation>> LinkDic, PdfDocument pdfDocument)
		{
			int startPage = 0;
			int pageCount = pdfDocument.Pages.Count;
			new Dictionary<int, List<BaseAnnotation>>();
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(pdfDocument);
			global::System.ValueTuple<int, int> valueTuple = ((pdfControl != null) ? pdfControl.GetVisiblePageRange() : new global::System.ValueTuple<int, int>(-1, -1));
			int viewportStartPage = valueTuple.Item1;
			int viewportEndPage = valueTuple.Item2;
			for (int i = 0; i < LinkDic.Count; i++)
			{
				PdfPage page = null;
				IntPtr pageHandle = IntPtr.Zero;
				try
				{
					if (i >= viewportStartPage && i <= viewportEndPage)
					{
						page = pdfDocument.Pages[i];
					}
					else
					{
						pageHandle = Pdfium.FPDF_LoadPage(pdfDocument.Handle, i);
						if (pageHandle != IntPtr.Zero)
						{
							page = PdfPage.FromHandle(pdfDocument, pageHandle, i, true);
						}
					}
					int[] array = LinkDic.Keys.ToArray<int>();
					BaseAnnotation[] array2 = LinkDic[array[i]].ToArray();
					if (array[i] >= startPage && array[i] < pageCount)
					{
						page = pdfDocument.Pages[array[i]];
						foreach (BaseAnnotation baseAnnotation in array2)
						{
							PdfAnnotation pdfAnnotation = AnnotationFactory.Create(page, baseAnnotation);
							if (pdfAnnotation is PdfLinkAnnotation)
							{
								page.Annots.Add(pdfAnnotation);
								await page.TryRedrawPageAsync(default(CancellationToken));
							}
						}
						BaseAnnotation[] array3 = null;
					}
				}
				finally
				{
					if (pageHandle != IntPtr.Zero)
					{
						PageDisposeHelper.DisposePage(page);
						Pdfium.FPDF_ClosePage(pageHandle);
					}
				}
				page = null;
			}
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x0002CC28 File Offset: 0x0002AE28
		public static Dictionary<int, List<BaseAnnotation>> LinkDeleteAllRedo(PdfDocument pdfDocument)
		{
			int num = 0;
			int count = pdfDocument.Pages.Count;
			Dictionary<int, List<BaseAnnotation>> dictionary = new Dictionary<int, List<BaseAnnotation>>();
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(pdfDocument);
			global::System.ValueTuple<int, int> valueTuple = ((pdfControl != null) ? pdfControl.GetVisiblePageRange() : new global::System.ValueTuple<int, int>(-1, -1));
			int item = valueTuple.Item1;
			int item2 = valueTuple.Item2;
			for (int i = num; i < num + count; i++)
			{
				PdfPage pdfPage = null;
				IntPtr intPtr = IntPtr.Zero;
				try
				{
					if (i >= item && i <= item2)
					{
						pdfPage = pdfDocument.Pages[i];
					}
					else
					{
						intPtr = Pdfium.FPDF_LoadPage(pdfDocument.Handle, i);
						if (intPtr != IntPtr.Zero)
						{
							pdfPage = PdfPage.FromHandle(pdfDocument, intPtr, i, true);
						}
					}
					if (((pdfPage != null) ? pdfPage.Annots : null) != null && pdfPage.Annots.Count > 0)
					{
						for (int j = pdfPage.Annots.Count - 1; j >= 0; j--)
						{
							if (pdfPage.Annots[j] is PdfLinkAnnotation)
							{
								BaseAnnotation baseAnnotation = AnnotationFactory.Create(pdfPage.Annots[j]);
								List<BaseAnnotation> list;
								if (!dictionary.TryGetValue(i, out list))
								{
									list = new List<BaseAnnotation>();
									dictionary[i] = list;
								}
								list.Add(baseAnnotation);
								pdfPage.Annots.RemoveAt(j);
							}
						}
					}
				}
				finally
				{
					if (intPtr != IntPtr.Zero)
					{
						PageDisposeHelper.DisposePage(pdfPage);
						Pdfium.FPDF_ClosePage(intPtr);
					}
				}
			}
			return dictionary;
		}
	}
}
