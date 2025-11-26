using System;

namespace pdfconverter.Models.Image
{
	// Token: 0x02000087 RID: 135
	internal class AdjustSettings
	{
		// Token: 0x1700023D RID: 573
		// (get) Token: 0x06000676 RID: 1654 RVA: 0x00017A25 File Offset: 0x00015C25
		// (set) Token: 0x06000677 RID: 1655 RVA: 0x00017A2D File Offset: 0x00015C2D
		public ToimagePage OriginalPage { get; set; }

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x06000678 RID: 1656 RVA: 0x00017A36 File Offset: 0x00015C36
		// (set) Token: 0x06000679 RID: 1657 RVA: 0x00017A3E File Offset: 0x00015C3E
		public ToimagePage Page { get; set; }

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x0600067A RID: 1658 RVA: 0x00017A47 File Offset: 0x00015C47
		// (set) Token: 0x0600067B RID: 1659 RVA: 0x00017A4F File Offset: 0x00015C4F
		public AdjustType AdjustType { get; set; }

		// Token: 0x0600067C RID: 1660 RVA: 0x00017A58 File Offset: 0x00015C58
		public bool IsSame(AdjustSettings settings)
		{
			if (settings == null)
			{
				return false;
			}
			if (this.Page == null || settings.Page == null)
			{
				return false;
			}
			if (this.AdjustType != settings.AdjustType)
			{
				return false;
			}
			bool flag = this.Page.Brightness == settings.Page.Brightness;
			bool flag2 = this.Page.Contrast == settings.Page.Contrast;
			bool flag3 = this.Page.Saturation == settings.Page.Saturation;
			if (this.AdjustType == AdjustType.Brightness)
			{
				return flag2 && flag3;
			}
			if (this.AdjustType == AdjustType.Contrast)
			{
				return flag && flag3;
			}
			return this.AdjustType == AdjustType.Saturation && (flag && flag2);
		}
	}
}
