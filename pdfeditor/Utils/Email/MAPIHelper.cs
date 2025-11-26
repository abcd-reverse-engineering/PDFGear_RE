using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace pdfeditor.Utils.Email
{
	// Token: 0x020000CF RID: 207
	internal class MAPIHelper
	{
		// Token: 0x06000BB7 RID: 2999 RVA: 0x0003DFEC File Offset: 0x0003C1EC
		public bool AddRecipientTo(string email)
		{
			return this.AddRecipient(email, MAPIHelper.HowTo.MAPI_TO);
		}

		// Token: 0x06000BB8 RID: 3000 RVA: 0x0003DFF6 File Offset: 0x0003C1F6
		public bool AddRecipientCc(string email)
		{
			return this.AddRecipient(email, MAPIHelper.HowTo.MAPI_CC);
		}

		// Token: 0x06000BB9 RID: 3001 RVA: 0x0003E000 File Offset: 0x0003C200
		public bool AddRecipientBcc(string email)
		{
			return this.AddRecipient(email, MAPIHelper.HowTo.MAPI_BCC);
		}

		// Token: 0x06000BBA RID: 3002 RVA: 0x0003E00A File Offset: 0x0003C20A
		public void AddAttachment(string strAttachmentFileName)
		{
			this.m_attachments.Add(strAttachmentFileName);
		}

		// Token: 0x06000BBB RID: 3003 RVA: 0x0003E018 File Offset: 0x0003C218
		public bool SendMailPopup(string strSubject, string strBody)
		{
			return this.SendMail(strSubject, strBody, 9);
		}

		// Token: 0x06000BBC RID: 3004 RVA: 0x0003E024 File Offset: 0x0003C224
		public bool SendMailDirect(string strSubject, string strBody)
		{
			return this.SendMail(strSubject, strBody, 1);
		}

		// Token: 0x06000BBD RID: 3005
		[DllImport("MAPI32.DLL", CharSet = CharSet.Ansi)]
		private static extern int MAPISendMail(IntPtr sess, IntPtr hwnd, MapiMessage message, int flg, int rsv);

		// Token: 0x06000BBE RID: 3006
		[DllImport("MAPI32.DLL", CharSet = CharSet.Unicode)]
		private static extern int MAPISendMailW(IntPtr sess, IntPtr hwnd, MapiMessageW message, int flg, int rsv);

		// Token: 0x06000BBF RID: 3007 RVA: 0x0003E030 File Offset: 0x0003C230
		private bool SendMail(string strSubject, string strBody, int how)
		{
			MapiMessage mapiMessage = new MapiMessage
			{
				subject = strSubject,
				noteText = strBody
			};
			mapiMessage.recips = this.GetRecipients(out mapiMessage.recipCount);
			mapiMessage.files = this.GetAttachments(out mapiMessage.fileCount);
			this.m_lastError = (this._useUnicode ? MAPIHelper.MAPISendMailW(new IntPtr(0), new IntPtr(0), new MapiMessageW(mapiMessage), how, 0) : MAPIHelper.MAPISendMail(new IntPtr(0), new IntPtr(0), mapiMessage, how, 0));
			bool flag = this.m_lastError == 0;
			this.Cleanup(ref mapiMessage);
			return flag;
		}

		// Token: 0x06000BC0 RID: 3008 RVA: 0x0003E0C4 File Offset: 0x0003C2C4
		private bool AddRecipient(string email, MAPIHelper.HowTo howTo)
		{
			MapiRecipDesc mapiRecipDesc = new MapiRecipDesc
			{
				recipClass = (int)howTo,
				name = email
			};
			this.m_recipients.Add(mapiRecipDesc);
			return true;
		}

		// Token: 0x06000BC1 RID: 3009 RVA: 0x0003E0F4 File Offset: 0x0003C2F4
		private IntPtr GetRecipients(out int recipCount)
		{
			recipCount = 0;
			if (this.m_recipients.Count == 0)
			{
				return IntPtr.Zero;
			}
			int num = Marshal.SizeOf(this._useUnicode ? typeof(MapiRecipDescW) : typeof(MapiRecipDesc));
			IntPtr intPtr = Marshal.AllocHGlobal(this.m_recipients.Count * num);
			IntPtr intPtr2 = intPtr;
			foreach (MapiRecipDesc mapiRecipDesc in this.m_recipients)
			{
				if (this._useUnicode)
				{
					Marshal.StructureToPtr<MapiRecipDescW>(new MapiRecipDescW(mapiRecipDesc), intPtr2, false);
				}
				else
				{
					Marshal.StructureToPtr<MapiRecipDesc>(mapiRecipDesc, intPtr2, false);
				}
				IntPtr.Add(intPtr2, num);
			}
			recipCount = this.m_recipients.Count;
			return intPtr;
		}

		// Token: 0x06000BC2 RID: 3010 RVA: 0x0003E1C8 File Offset: 0x0003C3C8
		private IntPtr GetAttachments(out int fileCount)
		{
			fileCount = 0;
			if (this.m_attachments == null)
			{
				return IntPtr.Zero;
			}
			if (this.m_attachments.Count <= 0 || this.m_attachments.Count > 20)
			{
				return IntPtr.Zero;
			}
			int num = Marshal.SizeOf(this._useUnicode ? typeof(MapiFileDescW) : typeof(MapiFileDesc));
			IntPtr intPtr = Marshal.AllocHGlobal(this.m_attachments.Count * num);
			MapiFileDesc mapiFileDesc = new MapiFileDesc
			{
				position = -1
			};
			IntPtr intPtr2 = intPtr;
			foreach (string text in this.m_attachments)
			{
				mapiFileDesc.name = Path.GetFileName(text);
				mapiFileDesc.path = text;
				if (this._useUnicode)
				{
					Marshal.StructureToPtr<MapiFileDescW>(new MapiFileDescW(mapiFileDesc), intPtr2, false);
				}
				else
				{
					Marshal.StructureToPtr<MapiFileDesc>(mapiFileDesc, intPtr2, false);
				}
				IntPtr.Add(intPtr2, num);
			}
			fileCount = this.m_attachments.Count;
			return intPtr;
		}

		// Token: 0x06000BC3 RID: 3011 RVA: 0x0003E2DC File Offset: 0x0003C4DC
		private void Cleanup(ref MapiMessage msg)
		{
			MAPIHelper.<Cleanup>g__FreeStruct|13_0(this._useUnicode ? typeof(MapiRecipDescW) : typeof(MapiRecipDesc), msg.recips, msg.recipCount);
			MAPIHelper.<Cleanup>g__FreeStruct|13_0(this._useUnicode ? typeof(MapiFileDescW) : typeof(MapiFileDesc), msg.files, msg.fileCount);
			this.m_recipients.Clear();
			this.m_attachments.Clear();
			this.m_lastError = 0;
		}

		// Token: 0x06000BC4 RID: 3012 RVA: 0x0003E368 File Offset: 0x0003C568
		public string GetLastError()
		{
			if (this.m_lastError >= 0 && this.m_lastError <= 26)
			{
				return this.Errors[this.m_lastError];
			}
			return "MAPI error [" + this.m_lastError.ToString() + "]";
		}

		// Token: 0x06000BC6 RID: 3014 RVA: 0x0003E4E4 File Offset: 0x0003C6E4
		[CompilerGenerated]
		internal static void <Cleanup>g__FreeStruct|13_0(Type type, IntPtr structPtr, int count)
		{
			int num = Marshal.SizeOf(type);
			if (structPtr == IntPtr.Zero)
			{
				return;
			}
			for (int i = 0; i < count; i++)
			{
				Marshal.DestroyStructure(structPtr, type);
				IntPtr.Add(structPtr, num);
			}
			Marshal.FreeHGlobal(structPtr);
		}

		// Token: 0x04000530 RID: 1328
		private bool _useUnicode = Environment.OSVersion.Version >= new Version(6, 2);

		// Token: 0x04000531 RID: 1329
		private readonly string[] Errors = new string[]
		{
			"OK [0]", "User abort [1]", "General MAPI failure [2]", "MAPI login failure [3]", "Disk full [4]", "Insufficient memory [5]", "Access denied [6]", "-unknown- [7]", "Too many sessions [8]", "Too many files were specified [9]",
			"Too many recipients were specified [10]", "A specified attachment was not found [11]", "Attachment open failure [12]", "Attachment write failure [13]", "Unknown recipient [14]", "Bad recipient type [15]", "No messages [16]", "Invalid message [17]", "Text too large [18]", "Invalid session [19]",
			"Type not supported [20]", "A recipient was specified ambiguously [21]", "Message in use [22]", "Network failure [23]", "Invalid edit fields [24]", "Invalid recipients [25]", "Not supported [26]"
		};

		// Token: 0x04000532 RID: 1330
		private readonly List<MapiRecipDesc> m_recipients = new List<MapiRecipDesc>();

		// Token: 0x04000533 RID: 1331
		private readonly List<string> m_attachments = new List<string>();

		// Token: 0x04000534 RID: 1332
		private int m_lastError;

		// Token: 0x04000535 RID: 1333
		private const int MAPI_LOGON_UI = 1;

		// Token: 0x04000536 RID: 1334
		private const int MAPI_DIALOG = 8;

		// Token: 0x04000537 RID: 1335
		private const int maxAttachments = 20;

		// Token: 0x020004E8 RID: 1256
		private enum HowTo
		{
			// Token: 0x04001B60 RID: 7008
			MAPI_ORIG,
			// Token: 0x04001B61 RID: 7009
			MAPI_TO,
			// Token: 0x04001B62 RID: 7010
			MAPI_CC,
			// Token: 0x04001B63 RID: 7011
			MAPI_BCC
		}
	}
}
