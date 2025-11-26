using System;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;

namespace pdfeditor.Models.PageContents
{
	// Token: 0x02000152 RID: 338
	public class PageTextObject : PageBaseObject
	{
		// Token: 0x17000826 RID: 2086
		// (get) Token: 0x06001433 RID: 5171 RVA: 0x00050709 File Offset: 0x0004E909
		public override PageObjectTypes ObjectType
		{
			get
			{
				return PageObjectTypes.PDFPAGE_TEXT;
			}
		}

		// Token: 0x17000827 RID: 2087
		// (get) Token: 0x06001434 RID: 5172 RVA: 0x0005070C File Offset: 0x0004E90C
		// (set) Token: 0x06001435 RID: 5173 RVA: 0x00050714 File Offset: 0x0004E914
		public float CharSpacing { get; protected set; }

		// Token: 0x17000828 RID: 2088
		// (get) Token: 0x06001436 RID: 5174 RVA: 0x0005071D File Offset: 0x0004E91D
		// (set) Token: 0x06001437 RID: 5175 RVA: 0x00050725 File Offset: 0x0004E925
		public float WordSpacing { get; protected set; }

		// Token: 0x17000829 RID: 2089
		// (get) Token: 0x06001438 RID: 5176 RVA: 0x0005072E File Offset: 0x0004E92E
		// (set) Token: 0x06001439 RID: 5177 RVA: 0x00050736 File Offset: 0x0004E936
		public PdfFont Font { get; protected set; }

		// Token: 0x1700082A RID: 2090
		// (get) Token: 0x0600143A RID: 5178 RVA: 0x0005073F File Offset: 0x0004E93F
		// (set) Token: 0x0600143B RID: 5179 RVA: 0x00050747 File Offset: 0x0004E947
		public float FontSize { get; protected set; }

		// Token: 0x1700082B RID: 2091
		// (get) Token: 0x0600143C RID: 5180 RVA: 0x00050750 File Offset: 0x0004E950
		// (set) Token: 0x0600143D RID: 5181 RVA: 0x00050758 File Offset: 0x0004E958
		public TextRenderingModes RenderMode { get; protected set; }

		// Token: 0x1700082C RID: 2092
		// (get) Token: 0x0600143E RID: 5182 RVA: 0x00050761 File Offset: 0x0004E961
		// (set) Token: 0x0600143F RID: 5183 RVA: 0x00050769 File Offset: 0x0004E969
		public FS_POINTF LocationWithoutTransform { get; protected set; }

		// Token: 0x1700082D RID: 2093
		// (get) Token: 0x06001440 RID: 5184 RVA: 0x00050772 File Offset: 0x0004E972
		// (set) Token: 0x06001441 RID: 5185 RVA: 0x0005077A File Offset: 0x0004E97A
		public bool TextKnockout { get; protected set; }

		// Token: 0x1700082E RID: 2094
		// (get) Token: 0x06001442 RID: 5186 RVA: 0x00050783 File Offset: 0x0004E983
		// (set) Token: 0x06001443 RID: 5187 RVA: 0x0005078B File Offset: 0x0004E98B
		public string TextUnicode { get; protected set; }

		// Token: 0x1700082F RID: 2095
		// (get) Token: 0x06001444 RID: 5188 RVA: 0x00050794 File Offset: 0x0004E994
		// (set) Token: 0x06001445 RID: 5189 RVA: 0x0005079C File Offset: 0x0004E99C
		public int[] CharCodes { get; protected set; }

		// Token: 0x17000830 RID: 2096
		// (get) Token: 0x06001446 RID: 5190 RVA: 0x000507A5 File Offset: 0x0004E9A5
		// (set) Token: 0x06001447 RID: 5191 RVA: 0x000507AD File Offset: 0x0004E9AD
		public float[] Kernings { get; protected set; }

		// Token: 0x06001448 RID: 5192 RVA: 0x000507B8 File Offset: 0x0004E9B8
		protected override void ApplyCore(PdfPageObject pageObject)
		{
			base.ApplyCore(pageObject);
			PdfTextObject pdfTextObject = pageObject as PdfTextObject;
			if (pdfTextObject != null)
			{
				pdfTextObject.CharSpacing = this.CharSpacing;
				pdfTextObject.WordSpacing = this.WordSpacing;
				pdfTextObject.Font = this.Font;
				pdfTextObject.FontSize = this.FontSize;
				pdfTextObject.RenderMode = this.RenderMode;
				pdfTextObject.TextKnockout = this.TextKnockout;
				Pdfium.FPDFTextObj_SetPosition(pageObject.Handle, this.LocationWithoutTransform.X, this.LocationWithoutTransform.Y);
				Pdfium.FPDFTextObj_SetText(pdfTextObject.Handle, this.CharCodes.Length, this.CharCodes, this.Kernings);
			}
		}

		// Token: 0x06001449 RID: 5193 RVA: 0x00050868 File Offset: 0x0004EA68
		protected override void Init(PdfPageObject pageObject)
		{
			base.Init(pageObject);
			PdfTextObject textObject = pageObject as PdfTextObject;
			if (textObject != null)
			{
				this.CharSpacing = PageBaseObject.ReturnValueOrDefault<float>(() => textObject.CharSpacing);
				this.WordSpacing = PageBaseObject.ReturnValueOrDefault<float>(() => textObject.WordSpacing);
				this.Font = PageBaseObject.ReturnValueOrDefault<PdfFont>(() => textObject.Font);
				this.FontSize = PageBaseObject.ReturnValueOrDefault<float>(() => textObject.FontSize);
				this.RenderMode = PageBaseObject.ReturnValueOrDefault<TextRenderingModes>(() => textObject.RenderMode);
				this.LocationWithoutTransform = PageBaseObject.ReturnValueOrDefault<FS_POINTF>(delegate
				{
					float num;
					float num2;
					Pdfium.FPDFTextObj_GetPos(pageObject.Handle, out num, out num2);
					return new FS_POINTF(num, num2);
				});
				this.TextKnockout = PageBaseObject.ReturnValueOrDefault<bool>(() => textObject.TextKnockout);
				this.TextUnicode = PageBaseObject.ReturnValueOrDefault<string>(() => textObject.TextUnicode);
				int charsCount = textObject.CharsCount;
				int[] array = new int[charsCount];
				float[] array2 = new float[charsCount];
				for (int i = 0; i < charsCount; i++)
				{
					Pdfium.FPDFTextObj_GetCharInfo(textObject.Handle, i, out array[i], out array2[i]);
				}
				this.CharCodes = array;
				this.Kernings = array2;
			}
		}

		// Token: 0x0600144A RID: 5194 RVA: 0x000509BC File Offset: 0x0004EBBC
		protected override bool EqualsCore(PageBaseObject other)
		{
			if (base.EqualsCore(other))
			{
				PageTextObject pageTextObject = other as PageTextObject;
				if (pageTextObject != null && this.CharSpacing == pageTextObject.CharSpacing && this.WordSpacing == pageTextObject.WordSpacing && this.Font == pageTextObject.Font && this.FontSize == pageTextObject.FontSize && this.RenderMode == pageTextObject.RenderMode && this.LocationWithoutTransform == pageTextObject.LocationWithoutTransform && this.TextKnockout == pageTextObject.TextKnockout)
				{
					return this.TextUnicode == pageTextObject.TextUnicode;
				}
			}
			return false;
		}
	}
}
