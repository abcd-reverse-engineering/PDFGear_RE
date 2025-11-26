using System;
using Newtonsoft.Json;
using Patagames.Pdf.Net.Annotations;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001A3 RID: 419
	public class PopupAnnotation : BaseAnnotation
	{
		// Token: 0x17000973 RID: 2419
		// (get) Token: 0x060017E7 RID: 6119 RVA: 0x0005B6CD File Offset: 0x000598CD
		// (set) Token: 0x060017E8 RID: 6120 RVA: 0x0005B6D5 File Offset: 0x000598D5
		[JsonIgnore]
		public BaseAnnotation Parent { get; set; }

		// Token: 0x17000974 RID: 2420
		// (get) Token: 0x060017E9 RID: 6121 RVA: 0x0005B6DE File Offset: 0x000598DE
		// (set) Token: 0x060017EA RID: 6122 RVA: 0x0005B6E6 File Offset: 0x000598E6
		public bool IsOpen { get; protected set; }

		// Token: 0x17000975 RID: 2421
		// (get) Token: 0x060017EB RID: 6123 RVA: 0x0005B6EF File Offset: 0x000598EF
		// (set) Token: 0x060017EC RID: 6124 RVA: 0x0005B6F7 File Offset: 0x000598F7
		public string Text { get; protected set; }

		// Token: 0x17000976 RID: 2422
		// (get) Token: 0x060017ED RID: 6125 RVA: 0x0005B700 File Offset: 0x00059900
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? ParentAnnotationIndex
		{
			get
			{
				BaseAnnotation parent = this.Parent;
				if (parent == null)
				{
					return null;
				}
				return new int?(parent.AnnotIndex);
			}
		}

		// Token: 0x060017EE RID: 6126 RVA: 0x0005B72C File Offset: 0x0005992C
		protected override void Init(PdfAnnotation pdfAnnotation)
		{
			base.Init(pdfAnnotation);
			PdfPopupAnnotation annot = pdfAnnotation as PdfPopupAnnotation;
			if (annot != null)
			{
				this.IsOpen = BaseAnnotation.ReturnValueOrDefault<bool>(() => annot.IsOpen);
				this.Text = BaseAnnotation.ReturnValueOrDefault<string>(() => annot.Text);
			}
		}

		// Token: 0x060017EF RID: 6127 RVA: 0x0005B788 File Offset: 0x00059988
		protected override void ApplyCore(PdfAnnotation annot)
		{
			base.ApplyCore(annot);
			PdfPopupAnnotation pdfPopupAnnotation = annot as PdfPopupAnnotation;
			if (pdfPopupAnnotation != null)
			{
				pdfPopupAnnotation.IsOpen = this.IsOpen;
				pdfPopupAnnotation.Text = this.Text;
				pdfPopupAnnotation.Rectangle = base.Rectangle;
			}
		}

		// Token: 0x060017F0 RID: 6128 RVA: 0x0005B7CC File Offset: 0x000599CC
		protected override bool EqualsCore(BaseAnnotation other)
		{
			if (base.EqualsCore(other))
			{
				PopupAnnotation popupAnnotation = other as PopupAnnotation;
				if (popupAnnotation != null && popupAnnotation.IsOpen == this.IsOpen && popupAnnotation.Text == this.Text)
				{
					return popupAnnotation.Rectangle == base.Rectangle;
				}
			}
			return false;
		}
	}
}
