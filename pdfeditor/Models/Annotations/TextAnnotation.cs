using System;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net.Annotations;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001AB RID: 427
	public class TextAnnotation : BaseMarkupAnnotation
	{
		// Token: 0x1700098E RID: 2446
		// (get) Token: 0x0600183A RID: 6202 RVA: 0x0005C81F File Offset: 0x0005AA1F
		// (set) Token: 0x0600183B RID: 6203 RVA: 0x0005C827 File Offset: 0x0005AA27
		public bool IsOpen { get; protected set; }

		// Token: 0x1700098F RID: 2447
		// (get) Token: 0x0600183C RID: 6204 RVA: 0x0005C830 File Offset: 0x0005AA30
		// (set) Token: 0x0600183D RID: 6205 RVA: 0x0005C838 File Offset: 0x0005AA38
		public IconNames StandardIconName { get; protected set; }

		// Token: 0x17000990 RID: 2448
		// (get) Token: 0x0600183E RID: 6206 RVA: 0x0005C841 File Offset: 0x0005AA41
		// (set) Token: 0x0600183F RID: 6207 RVA: 0x0005C849 File Offset: 0x0005AA49
		public string ExtendedIconName { get; protected set; }

		// Token: 0x17000991 RID: 2449
		// (get) Token: 0x06001840 RID: 6208 RVA: 0x0005C852 File Offset: 0x0005AA52
		// (set) Token: 0x06001841 RID: 6209 RVA: 0x0005C85A File Offset: 0x0005AA5A
		public StateModels StateModel { get; protected set; }

		// Token: 0x17000992 RID: 2450
		// (get) Token: 0x06001842 RID: 6210 RVA: 0x0005C863 File Offset: 0x0005AA63
		// (set) Token: 0x06001843 RID: 6211 RVA: 0x0005C86B File Offset: 0x0005AA6B
		public AnnotationStates State { get; protected set; }

		// Token: 0x06001844 RID: 6212 RVA: 0x0005C874 File Offset: 0x0005AA74
		protected override void ApplyCore(PdfAnnotation annot)
		{
			base.ApplyCore(annot);
			PdfTextAnnotation pdfTextAnnotation = annot as PdfTextAnnotation;
			if (pdfTextAnnotation != null)
			{
				pdfTextAnnotation.IsOpen = this.IsOpen;
				pdfTextAnnotation.StandardIconName = this.StandardIconName;
				pdfTextAnnotation.ExtendedIconName = this.ExtendedIconName;
				pdfTextAnnotation.StateModel = this.StateModel;
				pdfTextAnnotation.State = this.State;
				pdfTextAnnotation.Rectangle = base.Rectangle;
			}
		}

		// Token: 0x06001845 RID: 6213 RVA: 0x0005C8DC File Offset: 0x0005AADC
		protected override void Init(PdfAnnotation pdfAnnotation)
		{
			base.Init(pdfAnnotation);
			PdfTextAnnotation text = pdfAnnotation as PdfTextAnnotation;
			if (text != null)
			{
				this.IsOpen = BaseAnnotation.ReturnValueOrDefault<bool>(() => text.IsOpen);
				this.StandardIconName = BaseAnnotation.ReturnValueOrDefault<IconNames>(() => text.StandardIconName);
				this.ExtendedIconName = BaseAnnotation.ReturnValueOrDefault<string>(() => text.ExtendedIconName);
				this.StateModel = BaseAnnotation.ReturnValueOrDefault<StateModels>(() => text.StateModel);
				this.State = BaseAnnotation.ReturnValueOrDefault<AnnotationStates>(() => text.State);
			}
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x0005C980 File Offset: 0x0005AB80
		protected override bool EqualsCore(BaseAnnotation other)
		{
			if (base.EqualsCore(other))
			{
				TextAnnotation textAnnotation = other as TextAnnotation;
				if (textAnnotation != null && textAnnotation.IsOpen == this.IsOpen && textAnnotation.StandardIconName == this.StandardIconName && textAnnotation.ExtendedIconName == this.ExtendedIconName && textAnnotation.StateModel == this.StateModel && textAnnotation.State == this.State)
				{
					return textAnnotation.Rectangle == base.Rectangle;
				}
			}
			return false;
		}
	}
}
