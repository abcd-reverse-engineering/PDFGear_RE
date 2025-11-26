using System;

namespace pdfeditor.Utils.DigitalSignatures
{
	// Token: 0x020000DE RID: 222
	public class DigitalSignatureFormattedItem
	{
		// Token: 0x06000BFD RID: 3069 RVA: 0x0003F452 File Offset: 0x0003D652
		public DigitalSignatureFormattedItem(string key, string text)
		{
			this.Key = key;
			this.Text = text;
		}

		// Token: 0x170002A1 RID: 673
		// (get) Token: 0x06000BFE RID: 3070 RVA: 0x0003F468 File Offset: 0x0003D668
		public string Key { get; }

		// Token: 0x170002A2 RID: 674
		// (get) Token: 0x06000BFF RID: 3071 RVA: 0x0003F470 File Offset: 0x0003D670
		public string Text { get; }
	}
}
