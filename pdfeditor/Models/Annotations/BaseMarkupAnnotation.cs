using System;
using Newtonsoft.Json;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using PDFKit.Utils;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x0200019B RID: 411
	public abstract class BaseMarkupAnnotation : BaseAnnotation
	{
		// Token: 0x1700094F RID: 2383
		// (get) Token: 0x06001788 RID: 6024 RVA: 0x0005A6E8 File Offset: 0x000588E8
		// (set) Token: 0x06001789 RID: 6025 RVA: 0x0005A6F0 File Offset: 0x000588F0
		public string Text { get; protected set; }

		// Token: 0x17000950 RID: 2384
		// (get) Token: 0x0600178A RID: 6026 RVA: 0x0005A6F9 File Offset: 0x000588F9
		// (set) Token: 0x0600178B RID: 6027 RVA: 0x0005A701 File Offset: 0x00058901
		[JsonIgnore]
		public PopupAnnotation Popup { get; set; }

		// Token: 0x17000951 RID: 2385
		// (get) Token: 0x0600178C RID: 6028 RVA: 0x0005A70A File Offset: 0x0005890A
		// (set) Token: 0x0600178D RID: 6029 RVA: 0x0005A712 File Offset: 0x00058912
		public float Opacity { get; protected set; }

		// Token: 0x17000952 RID: 2386
		// (get) Token: 0x0600178E RID: 6030 RVA: 0x0005A71B File Offset: 0x0005891B
		// (set) Token: 0x0600178F RID: 6031 RVA: 0x0005A723 File Offset: 0x00058923
		public string RichText { get; protected set; }

		// Token: 0x17000953 RID: 2387
		// (get) Token: 0x06001790 RID: 6032 RVA: 0x0005A72C File Offset: 0x0005892C
		// (set) Token: 0x06001791 RID: 6033 RVA: 0x0005A734 File Offset: 0x00058934
		public string CreationDate { get; protected set; }

		// Token: 0x17000954 RID: 2388
		// (get) Token: 0x06001792 RID: 6034 RVA: 0x0005A73D File Offset: 0x0005893D
		// (set) Token: 0x06001793 RID: 6035 RVA: 0x0005A745 File Offset: 0x00058945
		[JsonIgnore]
		public BaseAnnotation RelationshipAnnotation { get; set; }

		// Token: 0x17000955 RID: 2389
		// (get) Token: 0x06001794 RID: 6036 RVA: 0x0005A74E File Offset: 0x0005894E
		// (set) Token: 0x06001795 RID: 6037 RVA: 0x0005A756 File Offset: 0x00058956
		public string Subject { get; protected set; }

		// Token: 0x17000956 RID: 2390
		// (get) Token: 0x06001796 RID: 6038 RVA: 0x0005A75F File Offset: 0x0005895F
		// (set) Token: 0x06001797 RID: 6039 RVA: 0x0005A767 File Offset: 0x00058967
		public RelationTypes Relationship { get; protected set; }

		// Token: 0x17000957 RID: 2391
		// (get) Token: 0x06001798 RID: 6040 RVA: 0x0005A770 File Offset: 0x00058970
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? PopupAnnotationIndex
		{
			get
			{
				PopupAnnotation popup = this.Popup;
				if (popup == null)
				{
					return null;
				}
				return new int?(popup.AnnotIndex);
			}
		}

		// Token: 0x17000958 RID: 2392
		// (get) Token: 0x06001799 RID: 6041 RVA: 0x0005A79B File Offset: 0x0005899B
		// (set) Token: 0x0600179A RID: 6042 RVA: 0x0005A7A3 File Offset: 0x000589A3
		public float Rotate { get; protected set; }

		// Token: 0x0600179B RID: 6043 RVA: 0x0005A7AC File Offset: 0x000589AC
		protected override void Init(PdfAnnotation pdfAnnotation)
		{
			base.Init(pdfAnnotation);
			PdfMarkupAnnotation annot = pdfAnnotation as PdfMarkupAnnotation;
			if (annot != null)
			{
				this.Text = BaseAnnotation.ReturnValueOrDefault<string>(() => annot.Text);
				this.Opacity = BaseAnnotation.ReturnValueOrDefault<float>(() => annot.Opacity);
				this.RichText = BaseAnnotation.ReturnValueOrDefault<string>(() => annot.RichText);
				this.CreationDate = BaseAnnotation.ReturnValueOrDefault<string>(() => annot.CreationDate);
				this.Subject = BaseAnnotation.ReturnValueOrDefault<string>(() => annot.Subject);
				this.Relationship = BaseAnnotation.ReturnValueOrDefault<RelationTypes>(() => annot.Relationship);
				this.Rotate = BaseAnnotation.ReturnValueOrDefault<float>(() => annot.GetRotate());
			}
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x0005A880 File Offset: 0x00058A80
		protected override void ApplyCore(PdfAnnotation annot)
		{
			base.ApplyCore(annot);
			PdfMarkupAnnotation pdfMarkupAnnotation = annot as PdfMarkupAnnotation;
			if (pdfMarkupAnnotation != null)
			{
				pdfMarkupAnnotation.Text = this.Text;
				pdfMarkupAnnotation.Opacity = this.Opacity;
				pdfMarkupAnnotation.RichText = this.RichText;
				pdfMarkupAnnotation.CreationDate = this.CreationDate;
				pdfMarkupAnnotation.Subject = this.Subject;
				pdfMarkupAnnotation.Relationship = this.Relationship;
				if (pdfMarkupAnnotation.Dictionary != null)
				{
					pdfMarkupAnnotation.Dictionary["Rotate"] = PdfTypeNumber.Create(this.Rotate);
				}
			}
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x0005A90C File Offset: 0x00058B0C
		protected override bool EqualsCore(BaseAnnotation other)
		{
			if (base.EqualsCore(other))
			{
				BaseMarkupAnnotation baseMarkupAnnotation = other as BaseMarkupAnnotation;
				if (baseMarkupAnnotation != null && baseMarkupAnnotation.Text == this.Text && baseMarkupAnnotation.Opacity == this.Opacity && baseMarkupAnnotation.RichText == this.RichText && baseMarkupAnnotation.CreationDate == this.CreationDate && baseMarkupAnnotation.Subject == this.Subject)
				{
					return baseMarkupAnnotation.Relationship == this.Relationship;
				}
			}
			return false;
		}
	}
}
