using System;

namespace pdfeditor.Models.Scan
{
	// Token: 0x0200013B RID: 315
	public class AdjustSettings
	{
		// Token: 0x170007BE RID: 1982
		// (get) Token: 0x0600132C RID: 4908 RVA: 0x0004E604 File Offset: 0x0004C804
		// (set) Token: 0x0600132D RID: 4909 RVA: 0x0004E60C File Offset: 0x0004C80C
		public ScannedPage OriginalPage { get; set; }

		// Token: 0x170007BF RID: 1983
		// (get) Token: 0x0600132E RID: 4910 RVA: 0x0004E615 File Offset: 0x0004C815
		// (set) Token: 0x0600132F RID: 4911 RVA: 0x0004E61D File Offset: 0x0004C81D
		public ScannedPage Page { get; set; }

		// Token: 0x170007C0 RID: 1984
		// (get) Token: 0x06001330 RID: 4912 RVA: 0x0004E626 File Offset: 0x0004C826
		// (set) Token: 0x06001331 RID: 4913 RVA: 0x0004E62E File Offset: 0x0004C82E
		public AdjustType AdjustType { get; set; }

		// Token: 0x06001332 RID: 4914 RVA: 0x0004E638 File Offset: 0x0004C838
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
