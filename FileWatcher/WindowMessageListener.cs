using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace FileWatcher
{
	// Token: 0x0200000F RID: 15
	public class WindowMessageListener : NativeWindow
	{
		// Token: 0x06000031 RID: 49 RVA: 0x00002908 File Offset: 0x00000B08
		public WindowMessageListener()
		{
			this.CreateHandle(new CreateParams
			{
				Caption = "FileWatcher_37CCD8B0-9B92-435E-88A1-79102B13E510"
			});
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002934 File Offset: 0x00000B34
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 74)
			{
				string text = WindowMessageListener.ProcessCopyDataMessage(m.LParam);
				if (string.IsNullOrEmpty(text))
				{
					return;
				}
				try
				{
					MessageData messageData = JsonConvert.DeserializeObject<MessageData>(text);
					if (messageData != null)
					{
						MessageReceivedEventHandler messageReceived = this.MessageReceived;
						if (messageReceived != null)
						{
							messageReceived(this, new MessageReceivedEventArgs(messageData));
						}
					}
				}
				catch
				{
				}
			}
			base.WndProc(ref m);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000029A0 File Offset: 0x00000BA0
		private static string ProcessCopyDataMessage(IntPtr lParam)
		{
			if (lParam == IntPtr.Zero)
			{
				return string.Empty;
			}
			try
			{
				return Marshal.PtrToStructure<WindowMessageListener.COPYDATASTRUCT>(lParam).lpData ?? string.Empty;
			}
			catch
			{
			}
			return string.Empty;
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000034 RID: 52 RVA: 0x000029F4 File Offset: 0x00000BF4
		// (remove) Token: 0x06000035 RID: 53 RVA: 0x00002A2C File Offset: 0x00000C2C
		internal event MessageReceivedEventHandler MessageReceived;

		// Token: 0x0400002D RID: 45
		private const string WindowCaption = "FileWatcher_37CCD8B0-9B92-435E-88A1-79102B13E510";

		// Token: 0x0400002E RID: 46
		private const int WM_COPYDATA = 74;

		// Token: 0x0200001C RID: 28
		private struct COPYDATASTRUCT
		{
			// Token: 0x04000052 RID: 82
			public IntPtr dwData;

			// Token: 0x04000053 RID: 83
			public int cbData;

			// Token: 0x04000054 RID: 84
			[MarshalAs(UnmanagedType.LPStr)]
			public string lpData;
		}
	}
}
