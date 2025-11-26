using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace pdfeditor.Utils
{
	// Token: 0x0200009F RID: 159
	public static class ScreenUtils
	{
		// Token: 0x06000A0C RID: 2572 RVA: 0x00033087 File Offset: 0x00031287
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetDpiForMonitor(HandleRef hMonitor, out uint dpiX, out uint dpiY)
		{
			return ScreenUtils.GetDpiForMonitor(hMonitor, ScreenUtils.MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out dpiX, out dpiY) == 0U;
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x00033095 File Offset: 0x00031295
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetDpiForWindow(Window window, out uint dpiX, out uint dpiY)
		{
			return ScreenUtils.TryGetDpiForVisual(window, out dpiX, out dpiY);
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x000330A0 File Offset: 0x000312A0
		public static bool TryGetDpiForVisual(Visual visual, out uint dpiX, out uint dpiY)
		{
			dpiX = 0U;
			dpiY = 0U;
			if (visual == null)
			{
				return false;
			}
			try
			{
				PresentationSource presentationSource = PresentationSource.FromVisual(visual);
				if (presentationSource != null)
				{
					dpiX = (uint)(presentationSource.CompositionTarget.TransformToDevice.M11 * 96.0);
					dpiY = (uint)(presentationSource.CompositionTarget.TransformToDevice.M22 * 96.0);
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x00033120 File Offset: 0x00031320
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetMonitorForWindow(Window window, out HandleRef hMonitor)
		{
			ScreenUtils.RECT rect;
			if (ScreenUtils.GetWindowRect(new WindowInteropHelper(window).Handle, out rect))
			{
				IntPtr intPtr = ScreenUtils.MonitorFromPoint(new ScreenUtils.POINT((rect.left + rect.right) / 2, (rect.top + rect.bottom) / 2), ScreenUtils.MonitorOptions.MONITOR_DEFAULTTONEAREST);
				hMonitor = new HandleRef(null, intPtr);
				return true;
			}
			hMonitor = default(HandleRef);
			return false;
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x00033184 File Offset: 0x00031384
		public static bool TryGetWindowRect(IntPtr hwnd, out Int32Rect bounds)
		{
			bounds = Int32Rect.Empty;
			ScreenUtils.RECT rect;
			if (ScreenUtils.GetWindowRect(hwnd, out rect))
			{
				bounds = rect;
				return true;
			}
			return false;
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x000331B8 File Offset: 0x000313B8
		public static bool GetMonitorInfo(IntPtr hMonitor, out Int32Rect bounds, out Int32Rect workArea, out bool primary)
		{
			ScreenUtils.MONITORINFOEXW monitorinfoexw = ScreenUtils.<GetMonitorInfo>g__CreateMonitorInfoEX|5_0();
			bounds = Int32Rect.Empty;
			workArea = Int32Rect.Empty;
			primary = false;
			bool flag = ScreenUtils.GetMonitorInfoW(hMonitor, ref monitorinfoexw) != 0;
			if (flag)
			{
				bounds = monitorinfoexw.rcMonitor;
				workArea = monitorinfoexw.rcWork;
				primary = (monitorinfoexw.dwFlags & ScreenUtils.MONITORINFOF.PRIMARY) > (ScreenUtils.MONITORINFOF)0U;
			}
			return flag;
		}

		// Token: 0x06000A12 RID: 2578
		[DllImport("shcore.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern uint GetDpiForMonitor(HandleRef hMonitor, ScreenUtils.MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY);

		// Token: 0x06000A13 RID: 2579
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr MonitorFromPoint(ScreenUtils.POINT pt, ScreenUtils.MonitorOptions dwFlags);

		// Token: 0x06000A14 RID: 2580
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool GetWindowRect(IntPtr hWnd, out ScreenUtils.RECT lpRect);

		// Token: 0x06000A15 RID: 2581
		[DllImport("user32", ExactSpelling = true)]
		private static extern int GetMonitorInfoW(IntPtr hMonitor, ref ScreenUtils.MONITORINFOEXW lpmi);

		// Token: 0x06000A16 RID: 2582 RVA: 0x00033224 File Offset: 0x00031424
		[CompilerGenerated]
		internal unsafe static ScreenUtils.MONITORINFOEXW <GetMonitorInfo>g__CreateMonitorInfoEX|5_0()
		{
			return new ScreenUtils.MONITORINFOEXW
			{
				cbSize = (uint)sizeof(ScreenUtils.MONITORINFOEXW)
			};
		}

		// Token: 0x02000494 RID: 1172
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct MONITORINFOEXW
		{
			// Token: 0x040019F7 RID: 6647
			public uint cbSize;

			// Token: 0x040019F8 RID: 6648
			public ScreenUtils.RECT rcMonitor;

			// Token: 0x040019F9 RID: 6649
			public ScreenUtils.RECT rcWork;

			// Token: 0x040019FA RID: 6650
			public ScreenUtils.MONITORINFOF dwFlags;

			// Token: 0x040019FB RID: 6651
			[FixedBuffer(typeof(char), 32)]
			public ScreenUtils.MONITORINFOEXW.<szDevice>e__FixedBuffer szDevice;

			// Token: 0x020007FB RID: 2043
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 64)]
			public struct <szDevice>e__FixedBuffer
			{
				// Token: 0x04002855 RID: 10325
				public char FixedElementField;
			}
		}

		// Token: 0x02000495 RID: 1173
		[Flags]
		public enum MONITORINFOF : uint
		{
			// Token: 0x040019FD RID: 6653
			PRIMARY = 1U
		}

		// Token: 0x02000496 RID: 1174
		private struct RECT
		{
			// Token: 0x06002E16 RID: 11798 RVA: 0x000E1762 File Offset: 0x000DF962
			public RECT(int left, int top, int right, int bottom)
			{
				this.left = left;
				this.top = top;
				this.right = right;
				this.bottom = bottom;
			}

			// Token: 0x17000CB1 RID: 3249
			// (get) Token: 0x06002E17 RID: 11799 RVA: 0x000E1781 File Offset: 0x000DF981
			public int X
			{
				get
				{
					return this.left;
				}
			}

			// Token: 0x17000CB2 RID: 3250
			// (get) Token: 0x06002E18 RID: 11800 RVA: 0x000E1789 File Offset: 0x000DF989
			public int Y
			{
				get
				{
					return this.top;
				}
			}

			// Token: 0x17000CB3 RID: 3251
			// (get) Token: 0x06002E19 RID: 11801 RVA: 0x000E1791 File Offset: 0x000DF991
			public int Width
			{
				get
				{
					return this.right - this.left;
				}
			}

			// Token: 0x17000CB4 RID: 3252
			// (get) Token: 0x06002E1A RID: 11802 RVA: 0x000E17A0 File Offset: 0x000DF9A0
			public int Height
			{
				get
				{
					return this.bottom - this.top;
				}
			}

			// Token: 0x17000CB5 RID: 3253
			// (get) Token: 0x06002E1B RID: 11803 RVA: 0x000E17AF File Offset: 0x000DF9AF
			public Size Size
			{
				get
				{
					return new Size((double)this.Width, (double)this.Height);
				}
			}

			// Token: 0x06002E1C RID: 11804 RVA: 0x000E17C4 File Offset: 0x000DF9C4
			public override string ToString()
			{
				return string.Format("{{{0}, {1}, {2}, {3} (LTRB)}}", new object[] { this.left, this.top, this.right, this.bottom });
			}

			// Token: 0x06002E1D RID: 11805 RVA: 0x000E1819 File Offset: 0x000DFA19
			public static implicit operator Int32Rect(ScreenUtils.RECT rect)
			{
				return new Int32Rect(rect.X, rect.Y, rect.Width, rect.Height);
			}

			// Token: 0x040019FE RID: 6654
			public int left;

			// Token: 0x040019FF RID: 6655
			public int top;

			// Token: 0x04001A00 RID: 6656
			public int right;

			// Token: 0x04001A01 RID: 6657
			public int bottom;
		}

		// Token: 0x02000497 RID: 1175
		private struct POINT
		{
			// Token: 0x06002E1E RID: 11806 RVA: 0x000E183C File Offset: 0x000DFA3C
			public POINT(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}

			// Token: 0x04001A02 RID: 6658
			public int X;

			// Token: 0x04001A03 RID: 6659
			public int Y;
		}

		// Token: 0x02000498 RID: 1176
		private enum MONITOR_DPI_TYPE
		{
			// Token: 0x04001A05 RID: 6661
			MDT_EFFECTIVE_DPI,
			// Token: 0x04001A06 RID: 6662
			MDT_ANGULAR_DPI,
			// Token: 0x04001A07 RID: 6663
			MDT_RAW_DPI,
			// Token: 0x04001A08 RID: 6664
			MDT_DEFAULT = 0
		}

		// Token: 0x02000499 RID: 1177
		private enum MonitorOptions : uint
		{
			// Token: 0x04001A0A RID: 6666
			MONITOR_DEFAULTTONULL,
			// Token: 0x04001A0B RID: 6667
			MONITOR_DEFAULTTOPRIMARY,
			// Token: 0x04001A0C RID: 6668
			MONITOR_DEFAULTTONEAREST
		}
	}
}
