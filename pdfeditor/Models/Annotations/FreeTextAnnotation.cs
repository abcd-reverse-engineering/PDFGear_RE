using System;
using System.Collections.Generic;
using System.Linq;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Models.Menus;
using pdfeditor.ViewModels;
using PDFKit.Utils.PdfRichTextStrings;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001AA RID: 426
	public class FreeTextAnnotation : BaseMarkupAnnotation
	{
		// Token: 0x17000985 RID: 2437
		// (get) Token: 0x06001823 RID: 6179 RVA: 0x0005C22B File Offset: 0x0005A42B
		// (set) Token: 0x06001824 RID: 6180 RVA: 0x0005C233 File Offset: 0x0005A433
		public string DefaultAppearance { get; protected set; }

		// Token: 0x17000986 RID: 2438
		// (get) Token: 0x06001825 RID: 6181 RVA: 0x0005C23C File Offset: 0x0005A43C
		// (set) Token: 0x06001826 RID: 6182 RVA: 0x0005C244 File Offset: 0x0005A444
		public JustifyTypes TextAlignment { get; protected set; }

		// Token: 0x17000987 RID: 2439
		// (get) Token: 0x06001827 RID: 6183 RVA: 0x0005C24D File Offset: 0x0005A44D
		// (set) Token: 0x06001828 RID: 6184 RVA: 0x0005C255 File Offset: 0x0005A455
		public string DefaultStyle { get; protected set; }

		// Token: 0x17000988 RID: 2440
		// (get) Token: 0x06001829 RID: 6185 RVA: 0x0005C25E File Offset: 0x0005A45E
		// (set) Token: 0x0600182A RID: 6186 RVA: 0x0005C266 File Offset: 0x0005A466
		public IReadOnlyList<FS_POINTF> CalloutLine { get; protected set; }

		// Token: 0x17000989 RID: 2441
		// (get) Token: 0x0600182B RID: 6187 RVA: 0x0005C26F File Offset: 0x0005A46F
		// (set) Token: 0x0600182C RID: 6188 RVA: 0x0005C277 File Offset: 0x0005A477
		public IReadOnlyList<LineEndingStyles> LineEnding { get; protected set; }

		// Token: 0x1700098A RID: 2442
		// (get) Token: 0x0600182D RID: 6189 RVA: 0x0005C280 File Offset: 0x0005A480
		// (set) Token: 0x0600182E RID: 6190 RVA: 0x0005C288 File Offset: 0x0005A488
		public AnnotationIntent Intent { get; protected set; }

		// Token: 0x1700098B RID: 2443
		// (get) Token: 0x0600182F RID: 6191 RVA: 0x0005C291 File Offset: 0x0005A491
		// (set) Token: 0x06001830 RID: 6192 RVA: 0x0005C299 File Offset: 0x0005A499
		public PdfBorderEffectModel BorderEffect { get; protected set; }

		// Token: 0x1700098C RID: 2444
		// (get) Token: 0x06001831 RID: 6193 RVA: 0x0005C2A2 File Offset: 0x0005A4A2
		// (set) Token: 0x06001832 RID: 6194 RVA: 0x0005C2AA File Offset: 0x0005A4AA
		public PdfBorderStyleModel BorderStyle { get; protected set; }

		// Token: 0x1700098D RID: 2445
		// (get) Token: 0x06001833 RID: 6195 RVA: 0x0005C2B3 File Offset: 0x0005A4B3
		// (set) Token: 0x06001834 RID: 6196 RVA: 0x0005C2BB File Offset: 0x0005A4BB
		public IReadOnlyList<float> InnerRectangle { get; protected set; }

		// Token: 0x06001835 RID: 6197 RVA: 0x0005C2C4 File Offset: 0x0005A4C4
		protected override void Init(PdfAnnotation pdfAnnotation)
		{
			base.Init(pdfAnnotation);
			PdfFreeTextAnnotation annot = pdfAnnotation as PdfFreeTextAnnotation;
			if (annot != null)
			{
				this.CalloutLine = BaseAnnotation.ReturnArrayOrEmpty<FS_POINTF>(delegate
				{
					PdfLinePointCollection<PdfFreeTextAnnotation> calloutLine = annot.CalloutLine;
					if (calloutLine == null)
					{
						return null;
					}
					return calloutLine.ToArray<FS_POINTF>();
				});
				this.LineEnding = BaseAnnotation.ReturnArrayOrEmpty<LineEndingStyles>(delegate
				{
					PdfLineEndingCollection lineEnding = annot.LineEnding;
					if (lineEnding == null)
					{
						return null;
					}
					return lineEnding.ToArray<LineEndingStyles>();
				});
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
				this.DefaultAppearance = BaseAnnotation.ReturnValueOrDefault<string>(() => annot.DefaultAppearance);
				this.TextAlignment = BaseAnnotation.ReturnValueOrDefault<JustifyTypes>(() => annot.TextAlignment);
				this.DefaultStyle = BaseAnnotation.ReturnValueOrDefault<string>(() => annot.DefaultStyle);
				this.Intent = BaseAnnotation.ReturnValueOrDefault<AnnotationIntent>(() => annot.Intent);
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

		// Token: 0x06001836 RID: 6198 RVA: 0x0005C3C4 File Offset: 0x0005A5C4
		protected override void ApplyCore(PdfAnnotation annot)
		{
			base.ApplyCore(annot);
			PdfFreeTextAnnotation pdfFreeTextAnnotation = annot as PdfFreeTextAnnotation;
			if (pdfFreeTextAnnotation != null)
			{
				if (pdfFreeTextAnnotation.CalloutLine == null)
				{
					pdfFreeTextAnnotation.CalloutLine = new PdfLinePointCollection<PdfFreeTextAnnotation>();
				}
				else
				{
					pdfFreeTextAnnotation.CalloutLine.Clear();
				}
				foreach (FS_POINTF fs_POINTF in this.CalloutLine)
				{
					pdfFreeTextAnnotation.CalloutLine.Add(fs_POINTF);
				}
				if (pdfFreeTextAnnotation.LineEnding == null)
				{
					pdfFreeTextAnnotation.LineEnding = new PdfLineEndingCollection(LineEndingStyles.None, LineEndingStyles.None);
					pdfFreeTextAnnotation.LineEnding.Clear();
				}
				else
				{
					pdfFreeTextAnnotation.LineEnding.Clear();
				}
				foreach (LineEndingStyles lineEndingStyles in this.LineEnding)
				{
					pdfFreeTextAnnotation.LineEnding.Add(lineEndingStyles);
				}
				if (this.BorderEffect == null)
				{
					pdfFreeTextAnnotation.BorderEffect = null;
				}
				else
				{
					if (pdfFreeTextAnnotation.BorderEffect == null)
					{
						pdfFreeTextAnnotation.BorderEffect = new PdfBorderEffect();
					}
					pdfFreeTextAnnotation.BorderEffect.Effect = this.BorderEffect.Effect;
					pdfFreeTextAnnotation.BorderEffect.Intensity = this.BorderEffect.Intensity;
				}
				if (this.BorderStyle == null)
				{
					pdfFreeTextAnnotation.BorderStyle = null;
				}
				else
				{
					if (pdfFreeTextAnnotation.BorderStyle == null)
					{
						pdfFreeTextAnnotation.BorderStyle = new PdfBorderStyle();
					}
					pdfFreeTextAnnotation.BorderStyle.Width = this.BorderStyle.Width;
					pdfFreeTextAnnotation.BorderStyle.DashPattern = this.BorderStyle.DashPattern.ToArray<float>();
					pdfFreeTextAnnotation.BorderStyle.Style = this.BorderStyle.Style;
				}
				pdfFreeTextAnnotation.DefaultAppearance = this.DefaultAppearance;
				pdfFreeTextAnnotation.TextAlignment = this.TextAlignment;
				pdfFreeTextAnnotation.DefaultStyle = this.DefaultStyle;
				pdfFreeTextAnnotation.Intent = this.Intent;
				PdfFreeTextAnnotation pdfFreeTextAnnotation2 = pdfFreeTextAnnotation;
				IReadOnlyList<float> innerRectangle = this.InnerRectangle;
				pdfFreeTextAnnotation2.InnerRectangle = ((innerRectangle != null) ? innerRectangle.ToArray<float>() : null);
				pdfFreeTextAnnotation.Rectangle = base.Rectangle;
			}
		}

		// Token: 0x06001837 RID: 6199 RVA: 0x0005C5D8 File Offset: 0x0005A7D8
		protected override bool EqualsCore(BaseAnnotation other)
		{
			if (base.EqualsCore(other))
			{
				FreeTextAnnotation freeTextAnnotation = other as FreeTextAnnotation;
				if (freeTextAnnotation != null && freeTextAnnotation.DefaultAppearance == this.DefaultAppearance && freeTextAnnotation.TextAlignment == this.TextAlignment && freeTextAnnotation.DefaultStyle == this.DefaultStyle && freeTextAnnotation.Intent == this.Intent && freeTextAnnotation.Rectangle == base.Rectangle && EqualityComparer<PdfBorderEffectModel>.Default.Equals(freeTextAnnotation.BorderEffect, this.BorderEffect) && EqualityComparer<PdfBorderStyleModel>.Default.Equals(freeTextAnnotation.BorderStyle, this.BorderStyle) && BaseAnnotation.CollectionEqual<FS_POINTF>(freeTextAnnotation.CalloutLine, this.CalloutLine) && BaseAnnotation.CollectionEqual<LineEndingStyles>(freeTextAnnotation.LineEnding, this.LineEnding))
				{
					return BaseAnnotation.CollectionEqual<float>(freeTextAnnotation.InnerRectangle, this.InnerRectangle);
				}
			}
			return false;
		}

		// Token: 0x06001838 RID: 6200 RVA: 0x0005C6C8 File Offset: 0x0005A8C8
		public override object GetValue(AnnotationMode mode, ContextMenuItemType type)
		{
			if (mode == AnnotationMode.TextBox || mode == AnnotationMode.Text)
			{
				switch (type)
				{
				case ContextMenuItemType.StrokeColor:
				{
					PdfDefaultAppearance pdfDefaultAppearance;
					if (PdfDefaultAppearance.TryParse(this.DefaultAppearance, out pdfDefaultAppearance))
					{
						return pdfDefaultAppearance.StrokeColor;
					}
					goto IL_013A;
				}
				case ContextMenuItemType.FillColor:
					return base.Color;
				case ContextMenuItemType.StrokeThickness:
				{
					PdfBorderStyleModel borderStyle = this.BorderStyle;
					return (borderStyle != null) ? borderStyle.Width : 1f;
				}
				case ContextMenuItemType.FontSize:
				{
					PdfDefaultAppearance pdfDefaultAppearance2;
					if (PdfDefaultAppearance.TryParse(this.DefaultAppearance, out pdfDefaultAppearance2))
					{
						return pdfDefaultAppearance2.FontSize;
					}
					goto IL_013A;
				}
				case ContextMenuItemType.FontColor:
				{
					FS_COLOR? fs_COLOR = null;
					PdfDefaultAppearance pdfDefaultAppearance3;
					if (PdfDefaultAppearance.TryParse(this.DefaultAppearance, out pdfDefaultAppearance3))
					{
						fs_COLOR = new FS_COLOR?(pdfDefaultAppearance3.FillColor);
					}
					if (fs_COLOR == null)
					{
						goto IL_013A;
					}
					if (fs_COLOR.Value.A < 250)
					{
						return new FS_COLOR(255, 255, 255, 255);
					}
					return new FS_COLOR(255, fs_COLOR.Value.R, fs_COLOR.Value.G, fs_COLOR.Value.B);
				}
				}
				return null;
			}
			IL_013A:
			return base.GetValue(mode, type);
		}
	}
}
