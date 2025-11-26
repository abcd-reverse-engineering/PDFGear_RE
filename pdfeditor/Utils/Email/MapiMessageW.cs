using System;
using System.Runtime.InteropServices;

namespace pdfeditor.Utils.Email
{
	// Token: 0x020000D1 RID: 209
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal class MapiMessageW
	{
		// Token: 0x06000BC8 RID: 3016 RVA: 0x0003E534 File Offset: 0x0003C734
		public MapiMessageW(MapiMessage message)
		{
			this.reserved = message.reserved;
			this.subject = message.subject;
			this.noteText = message.noteText;
			this.messageType = message.messageType;
			this.dateReceived = message.dateReceived;
			this.conversationID = message.conversationID;
			this.flags = message.flags;
			this.originator = message.originator;
			this.recipCount = message.recipCount;
			this.recips = message.recips;
			this.fileCount = message.fileCount;
			this.files = message.files;
		}

		// Token: 0x04000544 RID: 1348
		public int reserved;

		// Token: 0x04000545 RID: 1349
		public string subject;

		// Token: 0x04000546 RID: 1350
		public string noteText;

		// Token: 0x04000547 RID: 1351
		public string messageType;

		// Token: 0x04000548 RID: 1352
		public string dateReceived;

		// Token: 0x04000549 RID: 1353
		public string conversationID;

		// Token: 0x0400054A RID: 1354
		public int flags;

		// Token: 0x0400054B RID: 1355
		public IntPtr originator;

		// Token: 0x0400054C RID: 1356
		public int recipCount;

		// Token: 0x0400054D RID: 1357
		public IntPtr recips;

		// Token: 0x0400054E RID: 1358
		public int fileCount;

		// Token: 0x0400054F RID: 1359
		public IntPtr files;
	}
}
