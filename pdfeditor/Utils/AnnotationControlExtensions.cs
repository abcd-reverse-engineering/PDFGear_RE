using System;
using System.Windows;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Controls;
using pdfeditor.Controls.Annotations;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Models.Annotations;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Utils
{
	// Token: 0x02000070 RID: 112
	public static class AnnotationControlExtensions
	{
		// Token: 0x06000889 RID: 2185 RVA: 0x00028F4C File Offset: 0x0002714C
		public static Point? PageToDevice(this IAnnotationControl annotControl, Point point)
		{
			PdfViewer pdfViewer;
			if (annotControl == null)
			{
				pdfViewer = null;
			}
			else
			{
				AnnotationCanvas parentCanvas = annotControl.ParentCanvas;
				pdfViewer = ((parentCanvas != null) ? parentCanvas.PdfViewer : null);
			}
			PdfViewer pdfViewer2 = pdfViewer;
			PdfPage pdfPage;
			if (annotControl == null)
			{
				pdfPage = null;
			}
			else
			{
				PdfAnnotation annotation = annotControl.Annotation;
				pdfPage = ((annotation != null) ? annotation.Page : null);
			}
			PdfPage pdfPage2 = pdfPage;
			if (pdfViewer2 == null || pdfPage2 == null)
			{
				return null;
			}
			Point point2;
			if (pdfViewer2.TryGetClientPoint(pdfPage2.PageIndex, point, out point2))
			{
				return new Point?(point2);
			}
			return null;
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x00028FBC File Offset: 0x000271BC
		public static Point? PageToDevice(this IAnnotationControl annotControl, double x, double y)
		{
			return annotControl.PageToDevice(new Point(x, y));
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x00028FCC File Offset: 0x000271CC
		public static Point? DeviceToPage(this IAnnotationControl annotControl, Point point)
		{
			PdfViewer pdfViewer;
			if (annotControl == null)
			{
				pdfViewer = null;
			}
			else
			{
				AnnotationCanvas parentCanvas = annotControl.ParentCanvas;
				pdfViewer = ((parentCanvas != null) ? parentCanvas.PdfViewer : null);
			}
			PdfViewer pdfViewer2 = pdfViewer;
			PdfPage pdfPage;
			if (annotControl == null)
			{
				pdfPage = null;
			}
			else
			{
				PdfAnnotation annotation = annotControl.Annotation;
				pdfPage = ((annotation != null) ? annotation.Page : null);
			}
			PdfPage pdfPage2 = pdfPage;
			if (pdfViewer2 == null || pdfPage2 == null)
			{
				return null;
			}
			Point point2;
			if (pdfViewer2.TryGetPagePoint(pdfPage2.PageIndex, point, out point2))
			{
				return new Point?(point2);
			}
			return null;
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x0002903C File Offset: 0x0002723C
		public static Point? DeviceToPage(this IAnnotationControl annotControl, double x, double y)
		{
			return annotControl.DeviceToPage(new Point(x, y));
		}

		// Token: 0x0600088D RID: 2189 RVA: 0x0002904C File Offset: 0x0002724C
		public static void TryRedrawAnnotation(this PdfMarkupAnnotation annot, bool removeEditor = false)
		{
			if (((annot != null) ? annot.Page : null) != null)
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(annot.Page.Document);
				PdfViewer pdfViewer = ((pdfControl != null) ? pdfControl.Viewer : null);
				if (removeEditor && pdfViewer != null)
				{
					AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(pdfViewer);
					int? num;
					if (annotationHolderManager == null)
					{
						num = null;
					}
					else
					{
						IAnnotationHolder currentHolder = annotationHolderManager.CurrentHolder;
						if (currentHolder == null)
						{
							num = null;
						}
						else
						{
							PdfPage currentPage = currentHolder.CurrentPage;
							num = ((currentPage != null) ? new int?(currentPage.PageIndex) : null);
						}
					}
					int? num2 = num;
					int pageIndex = annot.Page.PageIndex;
					if ((num2.GetValueOrDefault() == pageIndex) & (num2 != null))
					{
						annotationHolderManager.CurrentHolder.Cancel();
					}
				}
				lock (annot)
				{
					if (!AnnotationControlExtensions.TryRedrawAnnotationCore(annot, pdfViewer))
					{
						try
						{
							AnnotationFactory.Create(annot).Apply(annot);
							AnnotationControlExtensions.TryRedrawAnnotationCore(annot, pdfViewer);
						}
						catch
						{
						}
					}
				}
			}
		}

		// Token: 0x0600088E RID: 2190 RVA: 0x00029160 File Offset: 0x00027360
		private static bool TryRedrawAnnotationCore(PdfMarkupAnnotation annot, PdfViewer viewer)
		{
			try
			{
				annot.Dispose();
				PdfHighlightAnnotation pdfHighlightAnnotation = annot as PdfHighlightAnnotation;
				if (pdfHighlightAnnotation != null)
				{
					pdfHighlightAnnotation.RegenerateAppearancesWithoutRound();
				}
				else
				{
					PdfFreeTextAnnotation pdfFreeTextAnnotation = annot as PdfFreeTextAnnotation;
					if (pdfFreeTextAnnotation != null)
					{
						pdfFreeTextAnnotation.RegenerateAppearancesWithRichText();
					}
					else
					{
						PdfStampAnnotation pdfStampAnnotation = annot as PdfStampAnnotation;
						if (pdfStampAnnotation != null)
						{
							pdfStampAnnotation.RegenerateAppearancesAdvance();
						}
						else
						{
							PdfTextAnnotation pdfTextAnnotation = annot as PdfTextAnnotation;
							if (pdfTextAnnotation != null)
							{
								pdfTextAnnotation.RegenerateAppearancesAdvance();
							}
							else
							{
								annot.RegenerateAppearances();
							}
						}
					}
				}
				if (!(annot is PdfFreeTextAnnotation) && viewer != null)
				{
					viewer.InvalidateVisual();
				}
				return true;
			}
			catch
			{
			}
			return false;
		}
	}
}
