using System;
using System.Drawing.Printing;

namespace pdfeditor.Controls.Printer
{
	// Token: 0x0200022C RID: 556
	public class PaperSizeInfo
	{
		// Token: 0x17000AAD RID: 2733
		// (get) Token: 0x06001F2F RID: 7983 RVA: 0x0008C4D8 File Offset: 0x0008A6D8
		// (set) Token: 0x06001F30 RID: 7984 RVA: 0x0008C4E0 File Offset: 0x0008A6E0
		public string FriendlyName { get; set; }

		// Token: 0x17000AAE RID: 2734
		// (get) Token: 0x06001F31 RID: 7985 RVA: 0x0008C4E9 File Offset: 0x0008A6E9
		// (set) Token: 0x06001F32 RID: 7986 RVA: 0x0008C4F1 File Offset: 0x0008A6F1
		public PaperSize PaperSize { get; set; }

		// Token: 0x06001F33 RID: 7987 RVA: 0x0008C4FC File Offset: 0x0008A6FC
		public override string ToString()
		{
			if (this.PaperSize != null)
			{
				return string.Format("{0} ({1}*{2}mm)", this.FriendlyName, Convert.ToInt32((double)this.PaperSize.Width / 3.9370078740157481), Convert.ToInt32((double)this.PaperSize.Height / 3.9370078740157481));
			}
			return this.FriendlyName;
		}
	}
}
