using System;
using System.Runtime.InteropServices;

namespace pdfeditor.Utils.Email
{
	// Token: 0x020000D5 RID: 213
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal class MapiRecipDescW
	{
		// Token: 0x06000BCC RID: 3020 RVA: 0x0003E644 File Offset: 0x0003C844
		public MapiRecipDescW(MapiRecipDesc recipDesc)
		{
			this.reserved = recipDesc.reserved;
			this.recipClass = recipDesc.recipClass;
			this.name = recipDesc.name;
			this.address = recipDesc.address;
			this.eIDSize = recipDesc.eIDSize;
			this.entryID = recipDesc.entryID;
		}

		// Token: 0x04000562 RID: 1378
		public int reserved;

		// Token: 0x04000563 RID: 1379
		public int recipClass;

		// Token: 0x04000564 RID: 1380
		public string name;

		// Token: 0x04000565 RID: 1381
		public string address;

		// Token: 0x04000566 RID: 1382
		public int eIDSize;

		// Token: 0x04000567 RID: 1383
		public IntPtr entryID;
	}
}
