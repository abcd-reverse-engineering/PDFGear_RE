using System;
using System.Runtime.InteropServices;

namespace pdfeditor.Utils.Email
{
	// Token: 0x020000D3 RID: 211
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal class MapiFileDescW
	{
		// Token: 0x06000BCA RID: 3018 RVA: 0x0003E5E0 File Offset: 0x0003C7E0
		public MapiFileDescW(MapiFileDesc fileDesc)
		{
			this.reserved = fileDesc.reserved;
			this.flags = fileDesc.flags;
			this.position = fileDesc.position;
			this.path = fileDesc.path;
			this.name = fileDesc.name;
			this.type = fileDesc.type;
		}

		// Token: 0x04000556 RID: 1366
		public int reserved;

		// Token: 0x04000557 RID: 1367
		public int flags;

		// Token: 0x04000558 RID: 1368
		public int position;

		// Token: 0x04000559 RID: 1369
		public string path;

		// Token: 0x0400055A RID: 1370
		public string name;

		// Token: 0x0400055B RID: 1371
		public IntPtr type;
	}
}
