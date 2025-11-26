using System;
using System.Collections.Generic;

namespace pdfeditor.Utils.Email
{
	// Token: 0x020000CD RID: 205
	public class EmailMessage
	{
		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06000BA9 RID: 2985 RVA: 0x0003DCF3 File Offset: 0x0003BEF3
		// (set) Token: 0x06000BAA RID: 2986 RVA: 0x0003DCFB File Offset: 0x0003BEFB
		public string Subject { get; set; }

		// Token: 0x17000295 RID: 661
		// (get) Token: 0x06000BAB RID: 2987 RVA: 0x0003DD04 File Offset: 0x0003BF04
		public IList<string> AttachmentFilePath
		{
			get
			{
				return this._attachmentFilePaths;
			}
		}

		// Token: 0x17000296 RID: 662
		// (get) Token: 0x06000BAC RID: 2988 RVA: 0x0003DD0C File Offset: 0x0003BF0C
		public IList<string> To
		{
			get
			{
				return this._to;
			}
		}

		// Token: 0x17000297 RID: 663
		// (get) Token: 0x06000BAD RID: 2989 RVA: 0x0003DD14 File Offset: 0x0003BF14
		public IList<string> Cc
		{
			get
			{
				return this._cc;
			}
		}

		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06000BAE RID: 2990 RVA: 0x0003DD1C File Offset: 0x0003BF1C
		public IList<string> Bcc
		{
			get
			{
				return this._bcc;
			}
		}

		// Token: 0x17000299 RID: 665
		// (get) Token: 0x06000BAF RID: 2991 RVA: 0x0003DD24 File Offset: 0x0003BF24
		// (set) Token: 0x06000BB0 RID: 2992 RVA: 0x0003DD2C File Offset: 0x0003BF2C
		public string Body { get; set; }

		// Token: 0x06000BB1 RID: 2993 RVA: 0x0003DD38 File Offset: 0x0003BF38
		public EmailMessage()
		{
			this._to = new List<string>();
			this._cc = new List<string>();
			this._bcc = new List<string>();
			this._attachmentFilePaths = new List<string>();
			this.Subject = "";
			this.Body = "";
		}

		// Token: 0x06000BB2 RID: 2994 RVA: 0x0003DD8D File Offset: 0x0003BF8D
		public bool Send()
		{
			return MailApiProvider.SendMessage(this);
		}

		// Token: 0x0400052A RID: 1322
		private List<string> _attachmentFilePaths;

		// Token: 0x0400052B RID: 1323
		private List<string> _to;

		// Token: 0x0400052C RID: 1324
		private List<string> _bcc;

		// Token: 0x0400052D RID: 1325
		private List<string> _cc;
	}
}
