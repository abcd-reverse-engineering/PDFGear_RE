using System;
using Patagames.Pdf.Enums;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001A1 RID: 417
	public class PdfBorderEffectModel : IEquatable<PdfBorderEffectModel>
	{
		// Token: 0x060017DE RID: 6110 RVA: 0x0005B5E8 File Offset: 0x000597E8
		public PdfBorderEffectModel(BorderEffects effect, int intensity)
		{
			this.Effect = effect;
			this.Intensity = intensity;
		}

		// Token: 0x1700096E RID: 2414
		// (get) Token: 0x060017DF RID: 6111 RVA: 0x0005B5FE File Offset: 0x000597FE
		public BorderEffects Effect { get; }

		// Token: 0x1700096F RID: 2415
		// (get) Token: 0x060017E0 RID: 6112 RVA: 0x0005B606 File Offset: 0x00059806
		public int Intensity { get; }

		// Token: 0x060017E1 RID: 6113 RVA: 0x0005B60E File Offset: 0x0005980E
		public bool Equals(PdfBorderEffectModel other)
		{
			return other != null && this.Effect == other.Effect && this.Intensity == other.Intensity;
		}
	}
}
