using System;

namespace FileWatcher
{
	// Token: 0x02000010 RID: 16
	internal class MessageReceivedEventArgs
	{
		// Token: 0x06000036 RID: 54 RVA: 0x00002A61 File Offset: 0x00000C61
		public MessageReceivedEventArgs(MessageData messageData)
		{
			this.MessageData = messageData;
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002A70 File Offset: 0x00000C70
		public MessageData MessageData { get; }
	}
}
