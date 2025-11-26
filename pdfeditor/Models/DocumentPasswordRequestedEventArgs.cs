using System;

namespace pdfeditor.Models
{
	// Token: 0x02000128 RID: 296
	public class DocumentPasswordRequestedEventArgs : EventArgs
	{
		// Token: 0x17000781 RID: 1921
		// (get) Token: 0x06001236 RID: 4662 RVA: 0x0004A998 File Offset: 0x00048B98
		// (set) Token: 0x06001237 RID: 4663 RVA: 0x0004A9A0 File Offset: 0x00048BA0
		public string Password { get; set; }

		// Token: 0x17000782 RID: 1922
		// (get) Token: 0x06001238 RID: 4664 RVA: 0x0004A9A9 File Offset: 0x00048BA9
		// (set) Token: 0x06001239 RID: 4665 RVA: 0x0004A9B1 File Offset: 0x00048BB1
		public bool Cancel { get; set; }

		// Token: 0x17000783 RID: 1923
		// (get) Token: 0x0600123A RID: 4666 RVA: 0x0004A9BA File Offset: 0x00048BBA
		// (set) Token: 0x0600123B RID: 4667 RVA: 0x0004A9C2 File Offset: 0x00048BC2
		public string FileName { get; set; }
	}
}
