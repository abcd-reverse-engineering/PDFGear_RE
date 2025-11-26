using System;
using System.Runtime.InteropServices;

namespace pdfeditor.Utils.Email
{
	// Token: 0x020000D4 RID: 212
	[StructLayout(LayoutKind.Sequential)]
	public class MapiRecipDesc
	{
		// Token: 0x0400055C RID: 1372
		public int reserved;

		// Token: 0x0400055D RID: 1373
		public int recipClass;

		// Token: 0x0400055E RID: 1374
		public string name;

		// Token: 0x0400055F RID: 1375
		public string address;

		// Token: 0x04000560 RID: 1376
		public int eIDSize;

		// Token: 0x04000561 RID: 1377
		public IntPtr entryID;
	}
}
