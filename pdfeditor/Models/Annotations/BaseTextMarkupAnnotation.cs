using System;
using System.Collections.Generic;
using System.Linq;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001A8 RID: 424
	public abstract class BaseTextMarkupAnnotation : BaseMarkupAnnotation
	{
		// Token: 0x17000984 RID: 2436
		// (get) Token: 0x0600181A RID: 6170 RVA: 0x0005C0D2 File Offset: 0x0005A2D2
		// (set) Token: 0x0600181B RID: 6171 RVA: 0x0005C0DA File Offset: 0x0005A2DA
		public global::System.Collections.Generic.IReadOnlyList<FS_QUADPOINTSF> QuadPoints { get; protected set; }

		// Token: 0x0600181C RID: 6172 RVA: 0x0005C0E4 File Offset: 0x0005A2E4
		protected override bool EqualsCore(BaseAnnotation other)
		{
			if (base.EqualsCore(other))
			{
				BaseTextMarkupAnnotation baseTextMarkupAnnotation = other as BaseTextMarkupAnnotation;
				if (baseTextMarkupAnnotation != null)
				{
					return BaseAnnotation.CollectionEqual<FS_QUADPOINTSF>(this.QuadPoints, baseTextMarkupAnnotation.QuadPoints);
				}
			}
			return false;
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x0005C118 File Offset: 0x0005A318
		protected override void ApplyCore(PdfAnnotation annot)
		{
			base.ApplyCore(annot);
			PdfTextMarkupAnnotation pdfTextMarkupAnnotation = annot as PdfTextMarkupAnnotation;
			if (pdfTextMarkupAnnotation != null)
			{
				if (pdfTextMarkupAnnotation.QuadPoints == null)
				{
					pdfTextMarkupAnnotation.QuadPoints = new PdfQuadPointsCollection();
				}
				else
				{
					pdfTextMarkupAnnotation.QuadPoints.Clear();
				}
				if (this.QuadPoints != null && this.QuadPoints.Count > 0)
				{
					for (int i = 0; i < this.QuadPoints.Count; i++)
					{
						pdfTextMarkupAnnotation.QuadPoints.Add(this.QuadPoints[i]);
					}
				}
			}
		}

		// Token: 0x0600181E RID: 6174 RVA: 0x0005C19C File Offset: 0x0005A39C
		protected override void Init(PdfAnnotation pdfAnnotation)
		{
			base.Init(pdfAnnotation);
			PdfTextMarkupAnnotation textMarkup = pdfAnnotation as PdfTextMarkupAnnotation;
			if (textMarkup != null)
			{
				this.QuadPoints = BaseAnnotation.ReturnArrayOrEmpty<FS_QUADPOINTSF>(delegate
				{
					PdfQuadPointsCollection quadPoints = textMarkup.QuadPoints;
					if (quadPoints == null)
					{
						return null;
					}
					return quadPoints.ToArray<FS_QUADPOINTSF>();
				});
			}
		}
	}
}
