using System;
using System.Linq;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using PDFKit.Utils;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001A5 RID: 421
	public class StampAnnotation : BaseMarkupAnnotation
	{
		// Token: 0x17000977 RID: 2423
		// (get) Token: 0x060017F5 RID: 6133 RVA: 0x0005B8AD File Offset: 0x00059AAD
		// (set) Token: 0x060017F6 RID: 6134 RVA: 0x0005B8B5 File Offset: 0x00059AB5
		public string ExtendedIconName { get; protected set; }

		// Token: 0x17000978 RID: 2424
		// (get) Token: 0x060017F7 RID: 6135 RVA: 0x0005B8BE File Offset: 0x00059ABE
		// (set) Token: 0x060017F8 RID: 6136 RVA: 0x0005B8C6 File Offset: 0x00059AC6
		public FS_MATRIX ImageMatrix { get; protected set; }

		// Token: 0x17000979 RID: 2425
		// (get) Token: 0x060017F9 RID: 6137 RVA: 0x0005B8CF File Offset: 0x00059ACF
		// (set) Token: 0x060017FA RID: 6138 RVA: 0x0005B8D7 File Offset: 0x00059AD7
		public new string Contents { get; protected set; }

		// Token: 0x1700097A RID: 2426
		// (get) Token: 0x060017FB RID: 6139 RVA: 0x0005B8E0 File Offset: 0x00059AE0
		// (set) Token: 0x060017FC RID: 6140 RVA: 0x0005B8E8 File Offset: 0x00059AE8
		public new FS_COLOR Color { get; protected set; }

		// Token: 0x1700097B RID: 2427
		// (get) Token: 0x060017FD RID: 6141 RVA: 0x0005B8F1 File Offset: 0x00059AF1
		// (set) Token: 0x060017FE RID: 6142 RVA: 0x0005B8F9 File Offset: 0x00059AF9
		public PdfImageObjectModel ImageObject { get; protected set; }

		// Token: 0x1700097C RID: 2428
		// (get) Token: 0x060017FF RID: 6143 RVA: 0x0005B902 File Offset: 0x00059B02
		// (set) Token: 0x06001800 RID: 6144 RVA: 0x0005B90A File Offset: 0x00059B0A
		public bool IsRemoveBg { get; protected set; }

		// Token: 0x1700097D RID: 2429
		// (get) Token: 0x06001801 RID: 6145 RVA: 0x0005B913 File Offset: 0x00059B13
		// (set) Token: 0x06001802 RID: 6146 RVA: 0x0005B91B File Offset: 0x00059B1B
		public string ApplyId { get; protected set; }

		// Token: 0x1700097E RID: 2430
		// (get) Token: 0x06001803 RID: 6147 RVA: 0x0005B924 File Offset: 0x00059B24
		// (set) Token: 0x06001804 RID: 6148 RVA: 0x0005B92C File Offset: 0x00059B2C
		public int[] ApplyPageIndexs { get; protected set; }

		// Token: 0x1700097F RID: 2431
		// (get) Token: 0x06001805 RID: 6149 RVA: 0x0005B935 File Offset: 0x00059B35
		// (set) Token: 0x06001806 RID: 6150 RVA: 0x0005B93D File Offset: 0x00059B3D
		public int[] ImageSource { get; protected set; }

		// Token: 0x17000980 RID: 2432
		// (get) Token: 0x06001807 RID: 6151 RVA: 0x0005B946 File Offset: 0x00059B46
		// (set) Token: 0x06001808 RID: 6152 RVA: 0x0005B94E File Offset: 0x00059B4E
		public PdfTypeDictionary PDFXExtend { get; protected set; }

		// Token: 0x17000981 RID: 2433
		// (get) Token: 0x06001809 RID: 6153 RVA: 0x0005B957 File Offset: 0x00059B57
		// (set) Token: 0x0600180A RID: 6154 RVA: 0x0005B95F File Offset: 0x00059B5F
		public string ImgSource { get; protected set; }

		// Token: 0x0600180B RID: 6155 RVA: 0x0005B968 File Offset: 0x00059B68
		protected override void Init(PdfAnnotation pdfAnnotation)
		{
			base.Init(pdfAnnotation);
			PdfStampAnnotation annot = pdfAnnotation as PdfStampAnnotation;
			if (annot != null)
			{
				PdfPageObjectsCollection normalAppearance = annot.NormalAppearance;
				PdfImageObject pdfImageObject = ((normalAppearance != null) ? normalAppearance.OfType<PdfImageObject>().FirstOrDefault<PdfImageObject>() : null);
				this.IsRemoveBg = annot.Dictionary.ContainsKey("IsRemoveBg") > false;
				if (annot.Dictionary.ContainsKey("ApplyId") && annot.Dictionary["ApplyId"].Is<PdfTypeString>())
				{
					this.ApplyId = annot.Dictionary["ApplyId"].As<PdfTypeString>(true).UnicodeString.Trim();
				}
				if (annot.Dictionary.ContainsKey("ApplyRange") && annot.Dictionary["ApplyRange"].Is<PdfTypeArray>())
				{
					PdfTypeBase[] array = annot.Dictionary["ApplyRange"].As<PdfTypeArray>(true).ToArray<PdfTypeBase>();
					int[] array2 = new int[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = (array[i] as PdfTypeNumber).IntValue;
					}
					this.ApplyPageIndexs = array2;
				}
				if (((pdfImageObject != null) ? pdfImageObject.Bitmap : null) != null)
				{
					this.ImageObject = new PdfImageObjectModel(pdfImageObject);
					this.ImageMatrix = pdfImageObject.Matrix;
				}
				this.ExtendedIconName = BaseAnnotation.ReturnValueOrDefault<string>(() => annot.ExtendedIconName);
				this.Contents = BaseAnnotation.ReturnValueOrDefault<string>(() => annot.Contents);
				this.Color = BaseAnnotation.ReturnValueOrDefault<FS_COLOR>(() => annot.Color);
				base.Rectangle = BaseAnnotation.ReturnValueOrDefault<FS_RECTF>(() => annot.GetRECT());
				this.PDFXExtend = BaseAnnotation.ReturnValueOrDefault<PdfTypeDictionary>(delegate
				{
					PdfTypeBase pdfTypeBase;
					if (annot.Dictionary.TryGetValue("PDFXExtend", out pdfTypeBase) && pdfTypeBase != null && pdfTypeBase.Is<PdfTypeDictionary>())
					{
						return (PdfTypeDictionary)PageDisposeHelper.DeepClone(pdfTypeBase.As<PdfTypeDictionary>(true));
					}
					return null;
				});
				if (annot.Dictionary.ContainsKey("ImgSource") && annot.Dictionary["ImgSource"].Is<PdfTypeString>())
				{
					this.ImgSource = annot.Dictionary["ImgSource"].As<PdfTypeString>(true).UnicodeString.Trim();
				}
			}
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x0005BBB4 File Offset: 0x00059DB4
		protected override void ApplyCore(PdfAnnotation annot)
		{
			base.ApplyCore(annot);
			PdfStampAnnotation pdfStampAnnotation = annot as PdfStampAnnotation;
			if (pdfStampAnnotation != null)
			{
				pdfStampAnnotation.ExtendedIconName = this.ExtendedIconName;
				if (!string.IsNullOrEmpty(this.ApplyId))
				{
					pdfStampAnnotation.Dictionary["ApplyId"] = PdfTypeString.Create(this.ApplyId, false, false);
				}
				if (this.ApplyPageIndexs != null && this.ApplyPageIndexs.Length != 0)
				{
					PdfTypeArray pdfTypeArray = PdfTypeArray.Create();
					for (int i = 0; i < this.ApplyPageIndexs.Length; i++)
					{
						pdfTypeArray.Add(PdfTypeNumber.Create(this.ApplyPageIndexs[i]));
					}
					pdfStampAnnotation.Dictionary["ApplyRange"] = pdfTypeArray;
				}
				if ((pdfStampAnnotation.NormalAppearance == null || pdfStampAnnotation.NormalAppearance.Count == 0) && this.ImageObject != null)
				{
					pdfStampAnnotation.CreateEmptyAppearance(AppearanceStreamModes.Normal);
					PdfImageObject pdfImageObject = PdfImageObject.Create(annot.Page.Document, this.ImageObject.Bitmap.Clone(null), annot.GetRECT().left, annot.GetRECT().top);
					pdfImageObject.Matrix = this.ImageMatrix;
					if (this.IsRemoveBg)
					{
						pdfImageObject.BlendMode = BlendTypes.FXDIB_BLEND_MULTIPLY;
						pdfStampAnnotation.Dictionary["IsRemoveBg"] = PdfTypeBoolean.Create(true);
					}
					pdfStampAnnotation.NormalAppearance.Add(pdfImageObject);
					if (this.ImageObject.SoftMask != null)
					{
						PdfIndirectList pdfIndirectList = PdfIndirectList.FromPdfDocument(annot.Page.Document);
						int num = pdfIndirectList.Add(this.ImageObject.SoftMask.Clone(false));
						if (pdfImageObject.Stream == null)
						{
							Pdfium.FPDFImageObj_GenerateStream(pdfImageObject.Handle, annot.Page.Handle);
						}
						pdfImageObject.Stream.Dictionary.SetIndirectAt("SMask", pdfIndirectList, num);
					}
					pdfStampAnnotation.GenerateAppearance(AppearanceStreamModes.Normal);
				}
				else
				{
					pdfStampAnnotation.Contents = this.ExtendedIconName;
					pdfStampAnnotation.Color = this.Color;
				}
				if (this.PDFXExtend != null)
				{
					pdfStampAnnotation.Dictionary["PDFXExtend"] = PageDisposeHelper.DeepClone(this.PDFXExtend);
				}
				if (this.ImgSource != null)
				{
					pdfStampAnnotation.Dictionary["ImgSource"] = PdfTypeString.Create(this.ImgSource, true, false);
				}
				pdfStampAnnotation.Rectangle = base.Rectangle;
			}
		}

		// Token: 0x0600180D RID: 6157 RVA: 0x0005BDE0 File Offset: 0x00059FE0
		protected override bool EqualsCore(BaseAnnotation other)
		{
			if (base.EqualsCore(other))
			{
				StampAnnotation stampAnnotation = other as StampAnnotation;
				if (stampAnnotation != null && other.Color == this.Color && other.Contents == this.Contents && other.Rectangle == base.Rectangle)
				{
					if (this.PDFXExtend == null && stampAnnotation.PDFXExtend == null)
					{
						return true;
					}
					if (this.PDFXExtend == null || stampAnnotation.PDFXExtend == null)
					{
						return false;
					}
					PdfTypeBase pdfTypeBase;
					PdfTypeBase pdfTypeBase2;
					if (this.PDFXExtend.TryGetValue("Type", out pdfTypeBase) && pdfTypeBase.Is<PdfTypeName>() && stampAnnotation.PDFXExtend.TryGetValue("Type", out pdfTypeBase2) && pdfTypeBase2.Is<PdfTypeName>())
					{
						string value = pdfTypeBase.As<PdfTypeName>(true).Value;
						PdfTypeBase pdfTypeBase3;
						PdfTypeBase pdfTypeBase4;
						return !(value != pdfTypeBase2.As<PdfTypeName>(true).Value) && (!(value == "Stamp") || !this.PDFXExtend.TryGetValue("Content", out pdfTypeBase3) || !pdfTypeBase3.Is<PdfTypeString>() || !stampAnnotation.PDFXExtend.TryGetValue("Content", out pdfTypeBase4) || !pdfTypeBase4.Is<PdfTypeString>() || !(pdfTypeBase3.As<PdfTypeString>(true).UnicodeString != pdfTypeBase4.As<PdfTypeString>(true).UnicodeString));
					}
					return false;
				}
			}
			return false;
		}
	}
}
