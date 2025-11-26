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

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002BC RID: 700
	public class StrikeoutAnnotationHoder : TextMarkupAnnotationHolder<PdfStrikeoutAnnotation>
	{
		// Token: 0x0600285C RID: 10332 RVA: 0x000BDC5B File Offset: 0x000BBE5B
		public StrikeoutAnnotationHoder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
		}

		// Token: 0x0600285D RID: 10333 RVA: 0x000BDC64 File Offset: 0x000BBE64
		public override global::System.Collections.Generic.IReadOnlyList<PdfStrikeoutAnnotation> CreateAnnotation(PdfDocument document, SelectInfo selectInfo)
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			TextInfo[] array = PdfTextMarkupAnnotationUtils.GetTextInfos(document, selectInfo, false).ToArray<TextInfo>();
			if (array.Length != 0)
			{
				Color color = (Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.StrikeStroke);
				FS_COLOR fs_COLOR = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
				List<PdfStrikeoutAnnotation> list = new List<PdfStrikeoutAnnotation>(array.Length);
				foreach (TextInfo textInfo in array)
				{
					PdfQuadPointsCollection pdfQuadPointsCollection = new PdfQuadPointsCollection();
					foreach (FS_RECTF fs_RECTF in PdfTextMarkupAnnotationUtils.GetNormalizedRects(base.AnnotationCanvas.PdfViewer, textInfo, false, false))
					{
						FS_QUADPOINTSF fs_QUADPOINTSF = fs_RECTF.ToQuadPoints(0f, 2f);
						pdfQuadPointsCollection.Add(fs_QUADPOINTSF);
					}
					if (pdfQuadPointsCollection.Count > 0)
					{
						PdfPage pdfPage = document.Pages[textInfo.PageIndex];
						PdfStrikeoutAnnotation pdfStrikeoutAnnotation = new PdfStrikeoutAnnotation(pdfPage);
						pdfStrikeoutAnnotation.Color = fs_COLOR;
						pdfStrikeoutAnnotation.QuadPoints = pdfQuadPointsCollection;
						pdfStrikeoutAnnotation.Text = AnnotationAuthorUtil.GetAuthorName();
						pdfStrikeoutAnnotation.Flags |= AnnotationFlags.Print;
						if (pdfPage.Annots == null)
						{
							pdfPage.CreateAnnotations();
						}
						if (pdfPage.Annots != null)
						{
							pdfPage.Annots.Add(pdfStrikeoutAnnotation);
							pdfStrikeoutAnnotation.RegenerateAppearances();
							list.Add(pdfStrikeoutAnnotation);
						}
					}
				}
				return list;
			}
			return null;
		}

		// Token: 0x0600285E RID: 10334 RVA: 0x000BDDFC File Offset: 0x000BBFFC
		public override bool OnPropertyChanged(string propertyName)
		{
			PdfStrikeoutAnnotation pdfStrikeoutAnnotation = base.SelectedAnnotation as PdfStrikeoutAnnotation;
			if (pdfStrikeoutAnnotation != null && propertyName == "StrikeStroke")
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				using (requiredService.OperationManager.TraceAnnotationChange(pdfStrikeoutAnnotation.Page, ""))
				{
					FS_COLOR fs_COLOR = ((Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.StrikeStroke)).ToPdfColor();
					pdfStrikeoutAnnotation.Color = fs_COLOR;
				}
				pdfStrikeoutAnnotation.TryRedrawAnnotation(false);
			}
			return false;
		}
	}
}
