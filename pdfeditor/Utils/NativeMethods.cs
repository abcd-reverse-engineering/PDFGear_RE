using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace pdfeditor.Utils
{
	// Token: 0x02000089 RID: 137
	internal class NativeMethods
	{
		// Token: 0x0600091C RID: 2332
		[DllImport("user32.dll", EntryPoint = "GetWindowLong")]
		private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

		// Token: 0x0600091D RID: 2333
		[DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
		private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

		// Token: 0x0600091E RID: 2334
		[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
		private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

		// Token: 0x0600091F RID: 2335
		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
		private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		// Token: 0x06000920 RID: 2336 RVA: 0x0002D8D0 File Offset: 0x0002BAD0
		public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
		{
			if (IntPtr.Size == 8)
			{
				return NativeMethods.GetWindowLongPtr64(hWnd, nIndex);
			}
			return NativeMethods.GetWindowLongPtr32(hWnd, nIndex);
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x0002D8E9 File Offset: 0x0002BAE9
		public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
		{
			if (IntPtr.Size == 8)
			{
				return NativeMethods.SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
			}
			return new IntPtr(NativeMethods.SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
		}

		// Token: 0x06000922 RID: 2338
		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		// Token: 0x06000923 RID: 2339
		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr MonitorFromPoint(NativeMethods.POINT pt, NativeMethods.MonitorOptions dwFlags);

		// Token: 0x06000924 RID: 2340 RVA: 0x0002D90F File Offset: 0x0002BB0F
		public static IntPtr MonitorFromPoint(Point pt, NativeMethods.MonitorOptions dwFlags)
		{
			return NativeMethods.MonitorFromPoint(new NativeMethods.POINT(pt.X, pt.Y), dwFlags);
		}

		// Token: 0x06000925 RID: 2341
		[DllImport("shcore.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern uint GetDpiForMonitor(IntPtr hMonitor, int dpiType, out uint dpiX, out uint dpiY);

		// Token: 0x06000926 RID: 2342 RVA: 0x0002D92A File Offset: 0x0002BB2A
		public static uint GetDpiForMonitor(IntPtr hMonitor, out uint dpiX, out uint dpiY)
		{
			return NativeMethods.GetDpiForMonitor(hMonitor, 0, out dpiX, out dpiY);
		}

		// Token: 0x06000927 RID: 2343
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, NativeMethods.SetWindowPosFlags uFlags);

		// Token: 0x06000928 RID: 2344
		[DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

		// Token: 0x06000929 RID: 2345
		[DllImport("shell32.dll", EntryPoint = "ExtractIconW")]
		public static extern IntPtr ExtractIcon(IntPtr hInst, string pszExeFileName, int nIconIndex);

		// Token: 0x0600092A RID: 2346
		[DllImport("shell32", EntryPoint = "ExtractIconExW")]
		public static extern int ExtractIconEx(string lpszFile, int nIconIndex, IntPtr[] phIconLarge, IntPtr[] phIconSmall, int nIcons);

		// Token: 0x0600092B RID: 2347
		[DllImport("shell32.dll", EntryPoint = "ExtractAssociatedIconW")]
		public static extern IntPtr ExtractAssociatedIcon(IntPtr hInst, string lpIconPath, ref int lpiIcon);

		// Token: 0x0600092C RID: 2348
		[DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW")]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		// Token: 0x0600092D RID: 2349
		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out NativeMethods.POINT lpPoint);

		// Token: 0x0600092E RID: 2350
		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, out NativeMethods.RECT lpRect);

		// Token: 0x0400046B RID: 1131
		public const int GWL_STYLE = -16;

		// Token: 0x02000444 RID: 1092
		public enum MonitorOptions : uint
		{
			// Token: 0x0400186F RID: 6255
			MONITOR_DEFAULTTONULL,
			// Token: 0x04001870 RID: 6256
			MONITOR_DEFAULTTOPRIMARY,
			// Token: 0x04001871 RID: 6257
			MONITOR_DEFAULTTONEAREST
		}

		// Token: 0x02000445 RID: 1093
		public struct POINT
		{
			// Token: 0x06002D26 RID: 11558 RVA: 0x000DC9B4 File Offset: 0x000DABB4
			public POINT(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}

			// Token: 0x04001872 RID: 6258
			public int X;

			// Token: 0x04001873 RID: 6259
			public int Y;
		}

		// Token: 0x02000446 RID: 1094
		public struct RECT
		{
			// Token: 0x04001874 RID: 6260
			public int left;

			// Token: 0x04001875 RID: 6261
			public int top;

			// Token: 0x04001876 RID: 6262
			public int right;

			// Token: 0x04001877 RID: 6263
			public int bottom;
		}

		// Token: 0x02000447 RID: 1095
		[Flags]
		public enum SetWindowPosFlags : uint
		{
			// Token: 0x04001879 RID: 6265
			SWP_ASYNCWINDOWPOS = 16384U,
			// Token: 0x0400187A RID: 6266
			SWP_DEFERERASE = 8192U,
			// Token: 0x0400187B RID: 6267
			SWP_DRAWFRAME = 32U,
			// Token: 0x0400187C RID: 6268
			SWP_FRAMECHANGED = 32U,
			// Token: 0x0400187D RID: 6269
			SWP_HIDEWINDOW = 128U,
			// Token: 0x0400187E RID: 6270
			SWP_NOACTIVATE = 16U,
			// Token: 0x0400187F RID: 6271
			SWP_NOCOPYBITS = 256U,
			// Token: 0x04001880 RID: 6272
			SWP_NOMOVE = 2U,
			// Token: 0x04001881 RID: 6273
			SWP_NOOWNERZORDER = 512U,
			// Token: 0x04001882 RID: 6274
			SWP_NOREDRAW = 8U,
			// Token: 0x04001883 RID: 6275
			SWP_NOREPOSITION = 512U,
			// Token: 0x04001884 RID: 6276
			SWP_NOSENDCHANGING = 1024U,
			// Token: 0x04001885 RID: 6277
			SWP_NOSIZE = 1U,
			// Token: 0x04001886 RID: 6278
			SWP_NOZORDER = 4U,
			// Token: 0x04001887 RID: 6279
			SWP_SHOWWINDOW = 64U
		}
	}
}
