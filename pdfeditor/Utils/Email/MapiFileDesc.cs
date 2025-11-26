using System;
using System.Runtime.InteropServices;

namespace pdfeditor.Utils.Email
{
	// Token: 0x020000D2 RID: 210
	[StructLayout(LayoutKind.Sequential)]
	public class MapiFileDesc
	{
		// Token: 0x04000550 RID: 1360
		public int reserved;

		// Token: 0x04000551 RID: 1361
		public int flags;

		// Token: 0x04000552 RID: 1362
		public int position;

		// Token: 0x04000553 RID: 1363
		public string path;

		// Token: 0x04000554 RID: 1364
		public string name;

		// Token: 0x04000555 RID: 1365
		public IntPtr type;
	}
}
