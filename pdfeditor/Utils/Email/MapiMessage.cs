using System;
using System.Runtime.InteropServices;

namespace pdfeditor.Utils.Email
{
	// Token: 0x020000D0 RID: 208
	[StructLayout(LayoutKind.Sequential)]
	public class MapiMessage
	{
		// Token: 0x04000538 RID: 1336
		public int reserved;

		// Token: 0x04000539 RID: 1337
		public string subject;

		// Token: 0x0400053A RID: 1338
		public string noteText;

		// Token: 0x0400053B RID: 1339
		public string messageType;

		// Token: 0x0400053C RID: 1340
		public string dateReceived;

		// Token: 0x0400053D RID: 1341
		public string conversationID;

		// Token: 0x0400053E RID: 1342
		public int flags;

		// Token: 0x0400053F RID: 1343
		public IntPtr originator;

		// Token: 0x04000540 RID: 1344
		public int recipCount;

		// Token: 0x04000541 RID: 1345
		public IntPtr recips;

		// Token: 0x04000542 RID: 1346
		public int fileCount;

		// Token: 0x04000543 RID: 1347
		public IntPtr files;
	}
}
