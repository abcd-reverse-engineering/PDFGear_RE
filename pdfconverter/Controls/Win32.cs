using System;
using System.Runtime.InteropServices;

namespace pdfconverter.Controls
{
	// Token: 0x020000A5 RID: 165
	public static class Win32
	{
		// Token: 0x0600072D RID: 1837
		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(ref Win32.POINT point);

		// Token: 0x02000162 RID: 354
		public struct POINT
		{
			// Token: 0x040006BD RID: 1725
			public int X;

			// Token: 0x040006BE RID: 1726
			public int Y;
		}
	}
}
