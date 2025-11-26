using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using pdfeditor.Utils;

namespace pdfeditor.Controls
{
	// Token: 0x020001B9 RID: 441
	public class FullScreenHelper
	{
		// Token: 0x06001918 RID: 6424 RVA: 0x00061327 File Offset: 0x0005F527
		public static bool GetIsFullScreenEnabled(Window obj)
		{
			return (bool)obj.GetValue(FullScreenHelper.IsFullScreenEnabledProperty);
		}

		// Token: 0x06001919 RID: 6425 RVA: 0x00061339 File Offset: 0x0005F539
		public static void SetIsFullScreenEnabled(Window obj, bool value)
		{
			obj.SetValue(FullScreenHelper.IsFullScreenEnabledProperty, value);
		}

		// Token: 0x0600191A RID: 6426 RVA: 0x0006134C File Offset: 0x0005F54C
		private static void OnIsFullScreenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!object.Equals(e.NewValue, e.OldValue))
			{
				Window sender = d as Window;
				if (sender != null)
				{
					object newValue = e.NewValue;
					if (newValue is bool && (bool)newValue)
					{
						sender.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
						{
							FullScreenHelper.EnterFullScreenMode(sender);
						}));
						return;
					}
					sender.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
					{
						FullScreenHelper.ExitFullScreenMode(sender);
					}));
				}
			}
		}

		// Token: 0x0600191B RID: 6427 RVA: 0x000613E0 File Offset: 0x0005F5E0
		private static void EnterFullScreenMode(Window window)
		{
			if (window == null)
			{
				return;
			}
			FullScreenHelper.WindowData windowData = FullScreenHelper.WindowData.CreateFromWindow(window);
			HandleRef handleRef;
			Int32Rect int32Rect;
			Int32Rect int32Rect2;
			bool flag;
			if (windowData != null && (FullScreenHelper.windowDict == null || !FullScreenHelper.windowDict.ContainsKey(windowData.Handle)) && ScreenUtils.TryGetMonitorForWindow(window, out handleRef) && ScreenUtils.GetMonitorInfo(handleRef.Handle, out int32Rect, out int32Rect2, out flag))
			{
				if (FullScreenHelper.windowDict == null)
				{
					Type typeFromHandle = typeof(FullScreenHelper);
					lock (typeFromHandle)
					{
						if (FullScreenHelper.windowDict == null)
						{
							FullScreenHelper.windowDict = new ConcurrentDictionary<IntPtr, FullScreenHelper.WindowData>();
						}
					}
				}
				FullScreenHelper.windowDict[windowData.Handle] = windowData;
				window.Topmost = false;
				window.ResizeMode = ResizeMode.NoResize;
				window.WindowStyle = WindowStyle.None;
				window.WindowState = WindowState.Maximized;
				FullScreenHelper.SetWindowPos(windowData.Handle, (IntPtr)(-1), int32Rect.X, int32Rect.Y, int32Rect.Width, int32Rect.Height, FullScreenHelper.SetWindowPosFlags.SWP_NOZORDER);
				window.Activate();
				window.StateChanged -= FullScreenHelper.Window_StateChanged;
				window.StateChanged += FullScreenHelper.Window_StateChanged;
				return;
			}
			FullScreenHelper.SetIsFullScreenEnabled(window, false);
		}

		// Token: 0x0600191C RID: 6428 RVA: 0x0006151C File Offset: 0x0005F71C
		private static void ExitFullScreenMode(Window window)
		{
			if (window == null)
			{
				return;
			}
			window.StateChanged -= FullScreenHelper.Window_StateChanged;
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = new WindowInteropHelper(window).Handle;
			}
			catch
			{
			}
			if (intPtr == IntPtr.Zero)
			{
				return;
			}
			window.ResizeMode = ResizeMode.CanResize;
			window.WindowStyle = WindowStyle.SingleBorderWindow;
			window.Topmost = false;
			Int32Rect int32Rect = Int32Rect.Empty;
			WindowState windowState = WindowState.Normal;
			bool flag = false;
			FullScreenHelper.WindowData windowData;
			if (FullScreenHelper.windowDict != null && FullScreenHelper.windowDict.TryRemove(intPtr, out windowData))
			{
				int32Rect = windowData.WindowBounds;
				windowState = windowData.WindowState;
				flag = true;
			}
			else
			{
				ScreenUtils.TryGetWindowRect(intPtr, out int32Rect);
			}
			window.WindowState = windowState;
			bool flag2 = false;
			HandleRef handleRef;
			Int32Rect int32Rect2;
			Int32Rect int32Rect3;
			bool flag3;
			if (ScreenUtils.TryGetMonitorForWindow(window, out handleRef) && ScreenUtils.GetMonitorInfo(handleRef.Handle, out int32Rect2, out int32Rect3, out flag3))
			{
				if (int32Rect.Width > int32Rect3.Width)
				{
					flag2 = true;
					int32Rect.Width = int32Rect3.Width;
				}
				if (int32Rect.Height > int32Rect3.Height)
				{
					flag2 = true;
					int32Rect.Height = int32Rect3.Height;
				}
			}
			if (flag || flag2)
			{
				FullScreenHelper.SetWindowPos(intPtr, (IntPtr)(-2), int32Rect.X, int32Rect.Y, int32Rect.Width, int32Rect.Height, (FullScreenHelper.SetWindowPosFlags)0U);
			}
		}

		// Token: 0x0600191D RID: 6429 RVA: 0x00061664 File Offset: 0x0005F864
		private static void Window_StateChanged(object sender, EventArgs e)
		{
			FullScreenHelper.SetIsFullScreenEnabled((Window)sender, false);
		}

		// Token: 0x0600191E RID: 6430 RVA: 0x00061674 File Offset: 0x0005F874
		internal static void SetWindowPositionAndSize(IntPtr hwnd, global::System.Drawing.Point? position, global::System.Drawing.Size? size)
		{
			if (position == null && size == null)
			{
				return;
			}
			FullScreenHelper.SetWindowPosFlags setWindowPosFlags = FullScreenHelper.SetWindowPosFlags.SWP_NOACTIVATE | FullScreenHelper.SetWindowPosFlags.SWP_NOZORDER;
			if (position == null)
			{
				setWindowPosFlags |= FullScreenHelper.SetWindowPosFlags.SWP_NOMOVE;
			}
			if (size == null)
			{
				setWindowPosFlags |= FullScreenHelper.SetWindowPosFlags.SWP_NOSIZE;
			}
			FullScreenHelper.SetWindowPos(hwnd, IntPtr.Zero, (position != null) ? position.GetValueOrDefault().X : 0, (position != null) ? position.GetValueOrDefault().Y : 0, (size != null) ? size.GetValueOrDefault().Width : 0, (size != null) ? size.GetValueOrDefault().Height : 0, setWindowPosFlags);
		}

		// Token: 0x0600191F RID: 6431
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, FullScreenHelper.SetWindowPosFlags uFlags);

		// Token: 0x04000873 RID: 2163
		private static ConcurrentDictionary<IntPtr, FullScreenHelper.WindowData> windowDict;

		// Token: 0x04000874 RID: 2164
		public static readonly DependencyProperty IsFullScreenEnabledProperty = DependencyProperty.RegisterAttached("IsFullScreenEnabled", typeof(bool), typeof(FullScreenHelper), new PropertyMetadata(false, new PropertyChangedCallback(FullScreenHelper.OnIsFullScreenPropertyChanged)));

		// Token: 0x020005D3 RID: 1491
		private class WindowData
		{
			// Token: 0x17000D37 RID: 3383
			// (get) Token: 0x06003285 RID: 12933 RVA: 0x000F6547 File Offset: 0x000F4747
			// (set) Token: 0x06003286 RID: 12934 RVA: 0x000F654F File Offset: 0x000F474F
			public IntPtr Handle { get; private set; }

			// Token: 0x17000D38 RID: 3384
			// (get) Token: 0x06003287 RID: 12935 RVA: 0x000F6558 File Offset: 0x000F4758
			// (set) Token: 0x06003288 RID: 12936 RVA: 0x000F6560 File Offset: 0x000F4760
			public WindowState WindowState { get; private set; }

			// Token: 0x17000D39 RID: 3385
			// (get) Token: 0x06003289 RID: 12937 RVA: 0x000F6569 File Offset: 0x000F4769
			// (set) Token: 0x0600328A RID: 12938 RVA: 0x000F6571 File Offset: 0x000F4771
			public Int32Rect WindowBounds { get; private set; }

			// Token: 0x0600328B RID: 12939 RVA: 0x000F657C File Offset: 0x000F477C
			public static FullScreenHelper.WindowData CreateFromWindow(Window window)
			{
				IntPtr intPtr = IntPtr.Zero;
				try
				{
					intPtr = new WindowInteropHelper(window).EnsureHandle();
				}
				catch
				{
				}
				if (intPtr == IntPtr.Zero)
				{
					return null;
				}
				Int32Rect int32Rect;
				if (ScreenUtils.TryGetWindowRect(intPtr, out int32Rect))
				{
					return new FullScreenHelper.WindowData
					{
						Handle = intPtr,
						WindowState = window.WindowState,
						WindowBounds = int32Rect
					};
				}
				return null;
			}
		}

		// Token: 0x020005D4 RID: 1492
		private enum SpecialWindowHandles
		{
			// Token: 0x04001F65 RID: 8037
			HWND_TOP,
			// Token: 0x04001F66 RID: 8038
			HWND_BOTTOM,
			// Token: 0x04001F67 RID: 8039
			HWND_TOPMOST = -1,
			// Token: 0x04001F68 RID: 8040
			HWND_NOTOPMOST = -2
		}

		// Token: 0x020005D5 RID: 1493
		[Flags]
		private enum SetWindowPosFlags : uint
		{
			// Token: 0x04001F6A RID: 8042
			SWP_ASYNCWINDOWPOS = 16384U,
			// Token: 0x04001F6B RID: 8043
			SWP_DEFERERASE = 8192U,
			// Token: 0x04001F6C RID: 8044
			SWP_DRAWFRAME = 32U,
			// Token: 0x04001F6D RID: 8045
			SWP_FRAMECHANGED = 32U,
			// Token: 0x04001F6E RID: 8046
			SWP_HIDEWINDOW = 128U,
			// Token: 0x04001F6F RID: 8047
			SWP_NOACTIVATE = 16U,
			// Token: 0x04001F70 RID: 8048
			SWP_NOCOPYBITS = 256U,
			// Token: 0x04001F71 RID: 8049
			SWP_NOMOVE = 2U,
			// Token: 0x04001F72 RID: 8050
			SWP_NOOWNERZORDER = 512U,
			// Token: 0x04001F73 RID: 8051
			SWP_NOREDRAW = 8U,
			// Token: 0x04001F74 RID: 8052
			SWP_NOREPOSITION = 512U,
			// Token: 0x04001F75 RID: 8053
			SWP_NOSENDCHANGING = 1024U,
			// Token: 0x04001F76 RID: 8054
			SWP_NOSIZE = 1U,
			// Token: 0x04001F77 RID: 8055
			SWP_NOZORDER = 4U,
			// Token: 0x04001F78 RID: 8056
			SWP_SHOWWINDOW = 64U
		}
	}
}
