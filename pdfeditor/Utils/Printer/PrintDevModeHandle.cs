using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace pdfeditor.Utils.Printer
{
	// Token: 0x020000C9 RID: 201
	public class PrintDevModeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		// Token: 0x06000BA6 RID: 2982 RVA: 0x0003DCCC File Offset: 0x0003BECC
		public PrintDevModeHandle(IntPtr handle)
			: base(true)
		{
			base.SetHandle(handle);
		}

		// Token: 0x06000BA7 RID: 2983 RVA: 0x0003DCDC File Offset: 0x0003BEDC
		protected override bool ReleaseHandle()
		{
			return PrintDevModeHandle.GlobalFree(this.handle) == IntPtr.Zero;
		}

		// Token: 0x06000BA8 RID: 2984
		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GlobalFree(IntPtr handle);
	}
}
