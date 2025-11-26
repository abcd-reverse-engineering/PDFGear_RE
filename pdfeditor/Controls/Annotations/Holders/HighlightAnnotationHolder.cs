using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002B5 RID: 693
	public class HighlightAnnotationHolder : TextMarkupAnnotationHolder<PdfHighlightAnnotation>
	{
		// Token: 0x06002816 RID: 10262 RVA: 0x000BC0A4 File Offset: 0x000BA2A4
		public HighlightAnnotationHolder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
		}

		// Token: 0x06002817 RID: 10263 RVA: 0x000BC0B0 File Offset: 0x000BA2B0
		public override global::System.Collections.Generic.IReadOnlyList<PdfHighlightAnnotation> CreateAnnotation(PdfDocument document, SelectInfo selectInfo)
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			TextInfo[] array = PdfTextMarkupAnnotationUtils.GetTextInfos(document, selectInfo, false).ToArray<TextInfo>();
			if (array.Length != 0)
			{
				Color color = (Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.HighlightStroke);
				FS_COLOR fs_COLOR = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
				List<PdfHighlightAnnotation> list = new List<PdfHighlightAnnotation>(array.Length);
				foreach (TextInfo textInfo in array)
				{
					PdfQuadPointsCollection pdfQuadPointsCollection = new PdfQuadPointsCollection();
					foreach (FS_RECTF fs_RECTF in PdfTextMarkupAnnotationUtils.GetNormalizedRects(base.AnnotationCanvas.PdfViewer, textInfo, true, true))
					{
						FS_QUADPOINTSF fs_QUADPOINTSF = fs_RECTF.ToQuadPoints();
						pdfQuadPointsCollection.Add(fs_QUADPOINTSF);
					}
					if (pdfQuadPointsCollection.Count > 0)
					{
						PdfPage pdfPage = document.Pages[textInfo.PageIndex];
						PdfHighlightAnnotation pdfHighlightAnnotation = new PdfHighlightAnnotation(pdfPage);
						pdfHighlightAnnotation.Color = fs_COLOR;
						pdfHighlightAnnotation.QuadPoints = pdfQuadPointsCollection;
						pdfHighlightAnnotation.Text = AnnotationAuthorUtil.GetAuthorName();
						pdfHighlightAnnotation.Flags |= AnnotationFlags.Print;
						if (pdfPage.Annots == null)
						{
							pdfPage.CreateAnnotations();
						}
						if (pdfPage.Annots != null)
						{
							pdfPage.Annots.Add(pdfHighlightAnnotation);
							pdfHighlightAnnotation.RegenerateAppearancesWithoutRound();
							list.Add(pdfHighlightAnnotation);
						}
					}
				}
				return list;
			}
			return null;
		}

		// Token: 0x06002818 RID: 10264 RVA: 0x000BC23C File Offset: 0x000BA43C
		protected override bool CheckPointMoved(FS_POINTF point1, FS_POINTF point2)
		{
			return Math.Abs(point1.X - point2.X) > 5f || Math.Abs(point1.Y - point2.Y) > 5f;
		}

		// Token: 0x06002819 RID: 10265 RVA: 0x000BC278 File Offset: 0x000BA478
		public override bool OnPropertyChanged(string propertyName)
		{
			PdfHighlightAnnotation pdfHighlightAnnotation = base.SelectedAnnotation as PdfHighlightAnnotation;
			if (pdfHighlightAnnotation != null && propertyName == "HighlightStroke")
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				using (requiredService.OperationManager.TraceAnnotationChange(pdfHighlightAnnotation.Page, ""))
				{
					FS_COLOR fs_COLOR = ((Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.HighlightStroke)).ToPdfColor();
					pdfHighlightAnnotation.Color = fs_COLOR;
				}
				pdfHighlightAnnotation.TryRedrawAnnotation(false);
			}
			return false;
		}
	}
}
