using System;
using System.Collections.Generic;
using System.Linq;
using Patagames.Pdf;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x0200019C RID: 412
	public class BaseFigureAnnotation : BaseMarkupAnnotation
	{
		// Token: 0x17000959 RID: 2393
		// (get) Token: 0x0600179F RID: 6047 RVA: 0x0005A99E File Offset: 0x00058B9E
		// (set) Token: 0x060017A0 RID: 6048 RVA: 0x0005A9A6 File Offset: 0x00058BA6
		public PdfBorderEffectModel BorderEffect { get; protected set; }

		// Token: 0x1700095A RID: 2394
		// (get) Token: 0x060017A1 RID: 6049 RVA: 0x0005A9AF File Offset: 0x00058BAF
		// (set) Token: 0x060017A2 RID: 6050 RVA: 0x0005A9B7 File Offset: 0x00058BB7
		public FS_COLOR InteriorColor { get; protected set; }

		// Token: 0x1700095B RID: 2395
		// (get) Token: 0x060017A3 RID: 6051 RVA: 0x0005A9C0 File Offset: 0x00058BC0
		// (set) Token: 0x060017A4 RID: 6052 RVA: 0x0005A9C8 File Offset: 0x00058BC8
		public PdfBorderStyleModel BorderStyle { get; protected set; }

		// Token: 0x1700095C RID: 2396
		// (get) Token: 0x060017A5 RID: 6053 RVA: 0x0005A9D1 File Offset: 0x00058BD1
		// (set) Token: 0x060017A6 RID: 6054 RVA: 0x0005A9D9 File Offset: 0x00058BD9
		public IReadOnlyList<float> InnerRectangle { get; protected set; }

		// Token: 0x060017A7 RID: 6055 RVA: 0x0005A9E4 File Offset: 0x00058BE4
		protected override void Init(PdfAnnotation pdfAnnotation)
		{
			base.Init(pdfAnnotation);
			PdfFigureAnnotation annot = pdfAnnotation as PdfFigureAnnotation;
			if (annot != null)
			{
				this.BorderEffect = BaseAnnotation.ReturnValueOrDefault<PdfBorderEffectModel>(delegate
				{
					if (!(annot.BorderEffect != null))
					{
						return null;
					}
					return new PdfBorderEffectModel(annot.BorderEffect.Effect, annot.BorderEffect.Intensity);
				});
				this.BorderStyle = BaseAnnotation.ReturnValueOrDefault<PdfBorderStyleModel>(delegate
				{
					if (!(annot.BorderStyle != null))
					{
						return null;
					}
					return new PdfBorderStyleModel(annot.BorderStyle.Width, annot.BorderStyle.Style, annot.BorderStyle.DashPattern);
				});
				this.InteriorColor = BaseAnnotation.ReturnValueOrDefault<FS_COLOR>(() => annot.InteriorColor);
				this.InnerRectangle = BaseAnnotation.ReturnArrayOrEmpty<float>(delegate
				{
					float[] innerRectangle = annot.InnerRectangle;
					if (innerRectangle == null)
					{
						return null;
					}
					return innerRectangle.ToArray<float>();
				});
			}
		}

		// Token: 0x060017A8 RID: 6056 RVA: 0x0005AA70 File Offset: 0x00058C70
		protected override void ApplyCore(PdfAnnotation annot)
		{
			base.ApplyCore(annot);
			PdfFigureAnnotation pdfFigureAnnotation = annot as PdfFigureAnnotation;
			if (pdfFigureAnnotation != null)
			{
				if (this.BorderEffect == null)
				{
					pdfFigureAnnotation.BorderEffect = null;
				}
				else
				{
					if (pdfFigureAnnotation.BorderEffect == null)
					{
						pdfFigureAnnotation.BorderEffect = new PdfBorderEffect();
					}
					pdfFigureAnnotation.BorderEffect.Effect = this.BorderEffect.Effect;
					pdfFigureAnnotation.BorderEffect.Intensity = this.BorderEffect.Intensity;
				}
				if (this.BorderStyle == null)
				{
					pdfFigureAnnotation.BorderStyle = null;
				}
				else
				{
					if (pdfFigureAnnotation.BorderStyle == null)
					{
						pdfFigureAnnotation.BorderStyle = new PdfBorderStyle();
					}
					pdfFigureAnnotation.BorderStyle.Width = this.BorderStyle.Width;
					pdfFigureAnnotation.BorderStyle.DashPattern = this.BorderStyle.DashPattern.ToArray<float>();
					pdfFigureAnnotation.BorderStyle.Style = this.BorderStyle.Style;
				}
				pdfFigureAnnotation.InteriorColor = this.InteriorColor;
				PdfFigureAnnotation pdfFigureAnnotation2 = pdfFigureAnnotation;
				IReadOnlyList<float> innerRectangle = this.InnerRectangle;
				pdfFigureAnnotation2.InnerRectangle = ((innerRectangle != null) ? innerRectangle.ToArray<float>() : null);
				pdfFigureAnnotation.Rectangle = base.Rectangle;
			}
		}

		// Token: 0x060017A9 RID: 6057 RVA: 0x0005AB88 File Offset: 0x00058D88
		protected override bool EqualsCore(BaseAnnotation other)
		{
			if (base.EqualsCore(other))
			{
				BaseFigureAnnotation baseFigureAnnotation = other as BaseFigureAnnotation;
				if (baseFigureAnnotation != null && baseFigureAnnotation.InteriorColor == this.InteriorColor && baseFigureAnnotation.Rectangle == base.Rectangle && EqualityComparer<PdfBorderEffectModel>.Default.Equals(baseFigureAnnotation.BorderEffect, this.BorderEffect) && EqualityComparer<PdfBorderStyleModel>.Default.Equals(baseFigureAnnotation.BorderStyle, this.BorderStyle))
				{
					return BaseAnnotation.CollectionEqual<float>(baseFigureAnnotation.InnerRectangle, this.InnerRectangle);
				}
			}
			return false;
		}
	}
}
