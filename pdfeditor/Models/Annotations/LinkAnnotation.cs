using System;
using System.Collections.Generic;
using System.Linq;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Utils;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001A0 RID: 416
	public class LinkAnnotation : BaseAnnotation
	{
		// Token: 0x1700096A RID: 2410
		// (get) Token: 0x060017D2 RID: 6098 RVA: 0x0005B3DA File Offset: 0x000595DA
		// (set) Token: 0x060017D3 RID: 6099 RVA: 0x0005B3E2 File Offset: 0x000595E2
		public PdfLink Link { get; set; }

		// Token: 0x1700096B RID: 2411
		// (get) Token: 0x060017D4 RID: 6100 RVA: 0x0005B3EB File Offset: 0x000595EB
		// (set) Token: 0x060017D5 RID: 6101 RVA: 0x0005B3F3 File Offset: 0x000595F3
		public PdfAction PdfAction { get; set; }

		// Token: 0x1700096C RID: 2412
		// (get) Token: 0x060017D6 RID: 6102 RVA: 0x0005B3FC File Offset: 0x000595FC
		// (set) Token: 0x060017D7 RID: 6103 RVA: 0x0005B404 File Offset: 0x00059604
		public PdfTypeBase PdfTypedictionary { get; set; }

		// Token: 0x1700096D RID: 2413
		// (get) Token: 0x060017D8 RID: 6104 RVA: 0x0005B40D File Offset: 0x0005960D
		// (set) Token: 0x060017D9 RID: 6105 RVA: 0x0005B415 File Offset: 0x00059615
		public PdfBorderStyleModel PdfBorderStyle { get; set; }

		// Token: 0x060017DA RID: 6106 RVA: 0x0005B420 File Offset: 0x00059620
		protected override bool EqualsCore(BaseAnnotation other)
		{
			if (base.EqualsCore(other))
			{
				LinkAnnotation linkAnnotation = other as LinkAnnotation;
				if (linkAnnotation != null && linkAnnotation.Rectangle == base.Rectangle && linkAnnotation.Link == this.Link && linkAnnotation.PdfAction == this.PdfAction && EqualityComparer<PdfBorderStyleModel>.Default.Equals(linkAnnotation.PdfBorderStyle, this.PdfBorderStyle))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060017DB RID: 6107 RVA: 0x0005B48C File Offset: 0x0005968C
		protected override void Init(PdfAnnotation pdfAnnotation)
		{
			base.Init(pdfAnnotation);
			PdfLinkAnnotation link = pdfAnnotation as PdfLinkAnnotation;
			if (link != null)
			{
				this.PdfAction = BaseAnnotation.ReturnValueOrDefault<PdfAction>(() => link.Link.Action);
				this.PdfBorderStyle = BaseAnnotation.ReturnValueOrDefault<PdfBorderStyleModel>(delegate
				{
					PdfBorderStyle borderStyle = link.GetBorderStyle();
					if (borderStyle != null)
					{
						return new PdfBorderStyleModel(borderStyle.Width, borderStyle.Style, borderStyle.DashPattern);
					}
					return null;
				});
			}
		}

		// Token: 0x060017DC RID: 6108 RVA: 0x0005B4E8 File Offset: 0x000596E8
		protected override void ApplyCore(PdfAnnotation annot)
		{
			base.ApplyCore(annot);
			PdfLinkAnnotation pdfLinkAnnotation = annot as PdfLinkAnnotation;
			if (pdfLinkAnnotation != null)
			{
				pdfLinkAnnotation = (PdfLinkAnnotation)annot;
				pdfLinkAnnotation.Rectangle = base.Rectangle;
				try
				{
					pdfLinkAnnotation.Link.Action = this.PdfAction;
				}
				catch
				{
				}
				PdfBorderStyle pdfBorderStyle = new PdfBorderStyle();
				if (pdfLinkAnnotation.Link.Dictionary.ContainsKey("BS"))
				{
					pdfBorderStyle = pdfLinkAnnotation.GetBorderStyle();
				}
				if (this.PdfBorderStyle != null)
				{
					pdfBorderStyle.Width = this.PdfBorderStyle.Width;
					pdfBorderStyle.DashPattern = this.PdfBorderStyle.DashPattern.ToArray<float>();
					pdfBorderStyle.Style = this.PdfBorderStyle.Style;
				}
				else
				{
					pdfBorderStyle.Width = 1f;
					pdfBorderStyle.DashPattern = new float[] { 2f, 4f };
					pdfBorderStyle.Style = BorderStyles.Solid;
				}
				pdfLinkAnnotation.SetBorderStyle(pdfBorderStyle);
			}
		}
	}
}
