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
	// Token: 0x020002BF RID: 703
	public class UnderlineAnnotationHoder : TextMarkupAnnotationHolder<PdfUnderlineAnnotation>
	{
		// Token: 0x06002874 RID: 10356 RVA: 0x000BE390 File Offset: 0x000BC590
		public UnderlineAnnotationHoder(AnnotationCanvas annotationCanvas)
			: base(annotationCanvas)
		{
		}

		// Token: 0x06002875 RID: 10357 RVA: 0x000BE39C File Offset: 0x000BC59C
		public override global::System.Collections.Generic.IReadOnlyList<PdfUnderlineAnnotation> CreateAnnotation(PdfDocument document, SelectInfo selectInfo)
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			TextInfo[] array = PdfTextMarkupAnnotationUtils.GetTextInfos(document, selectInfo, false).ToArray<TextInfo>();
			if (array.Length != 0)
			{
				Color color = (Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.UnderlineStroke);
				FS_COLOR fs_COLOR = new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
				List<PdfUnderlineAnnotation> list = new List<PdfUnderlineAnnotation>(array.Length);
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
						PdfUnderlineAnnotation pdfUnderlineAnnotation = new PdfUnderlineAnnotation(pdfPage);
						pdfUnderlineAnnotation.Color = fs_COLOR;
						pdfUnderlineAnnotation.Text = AnnotationAuthorUtil.GetAuthorName();
						pdfUnderlineAnnotation.QuadPoints = pdfQuadPointsCollection;
						pdfUnderlineAnnotation.Flags |= AnnotationFlags.Print;
						if (pdfPage.Annots == null)
						{
							pdfPage.CreateAnnotations();
						}
						if (pdfPage.Annots != null)
						{
							pdfPage.Annots.Add(pdfUnderlineAnnotation);
							pdfUnderlineAnnotation.RegenerateAppearances();
							list.Add(pdfUnderlineAnnotation);
						}
					}
				}
				return list;
			}
			return null;
		}

		// Token: 0x06002876 RID: 10358 RVA: 0x000BE534 File Offset: 0x000BC734
		public override bool OnPropertyChanged(string propertyName)
		{
			PdfUnderlineAnnotation pdfUnderlineAnnotation = base.SelectedAnnotation as PdfUnderlineAnnotation;
			if (pdfUnderlineAnnotation != null && propertyName == "UnderlineStroke")
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				using (requiredService.OperationManager.TraceAnnotationChange(pdfUnderlineAnnotation.Page, ""))
				{
					FS_COLOR fs_COLOR = ((Color)ColorConverter.ConvertFromString(requiredService.AnnotationToolbar.AnnotationMenuPropertyAccessor.UnderlineStroke)).ToPdfColor();
					pdfUnderlineAnnotation.Color = fs_COLOR;
				}
				pdfUnderlineAnnotation.TryRedrawAnnotation(false);
			}
			return false;
		}
	}
}
