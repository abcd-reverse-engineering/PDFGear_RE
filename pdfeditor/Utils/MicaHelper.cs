using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Shell;

namespace pdfeditor.Utils
{
	// Token: 0x02000086 RID: 134
	internal class MicaHelper
	{
		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000904 RID: 2308 RVA: 0x0002D047 File Offset: 0x0002B247
		public static bool IsSupported
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x0002D04C File Offset: 0x0002B24C
		private MicaHelper(Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException("window");
			}
			this.window = window;
			window.StateChanged += this.Window_StateChanged;
			window.Closed += this.Window_Closed;
			this.windowChrome = new WindowChrome
			{
				CornerRadius = new CornerRadius(0.0),
				GlassFrameThickness = new Thickness(-1.0),
				UseAeroCaptionButtons = true,
				ResizeBorderThickness = new Thickness(6.0),
				CaptionHeight = SystemParameters.CaptionHeight,
				NonClientFrameEdges = (NonClientFrameEdges.Left | NonClientFrameEdges.Right | NonClientFrameEdges.Bottom)
			};
			WindowChrome.SetWindowChrome(window, this.windowChrome);
			SystemParameters.StaticPropertyChanged += this.SystemParameters_StaticPropertyChanged;
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x0002D117 File Offset: 0x0002B317
		private void Window_Closed(object sender, EventArgs e)
		{
			SystemParameters.StaticPropertyChanged -= this.SystemParameters_StaticPropertyChanged;
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x0002D12A File Offset: 0x0002B32A
		private void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CaptionHeight")
			{
				this.UpdateTitlebar();
			}
		}

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000908 RID: 2312 RVA: 0x0002D144 File Offset: 0x0002B344
		// (set) Token: 0x06000909 RID: 2313 RVA: 0x0002D155 File Offset: 0x0002B355
		public bool IsMicaEnabled
		{
			get
			{
				return MicaHelper.IsSupported && this.isMicaEnabled;
			}
			set
			{
				if (this.isMicaEnabled != value)
				{
					this.isMicaEnabled = value;
					MicaHelper.SetMicaState(this.window, MicaHelper.IsSupported && this.isMicaEnabled, false);
				}
			}
		}

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x0600090A RID: 2314 RVA: 0x0002D183 File Offset: 0x0002B383
		// (set) Token: 0x0600090B RID: 2315 RVA: 0x0002D18B File Offset: 0x0002B38B
		public FrameworkElement TitlebarPlaceholder
		{
			get
			{
				return this.titlebarPlaceholder;
			}
			set
			{
				if (this.titlebarPlaceholder != value)
				{
					this.titlebarPlaceholder = value;
					this.UpdateTitlebar();
				}
			}
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x0002D1A3 File Offset: 0x0002B3A3
		private void Window_StateChanged(object sender, EventArgs e)
		{
			this.UpdateTitlebar();
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x0002D1AC File Offset: 0x0002B3AC
		private void UpdateTitlebar()
		{
			WindowState windowState = this.window.WindowState;
			if (windowState == WindowState.Minimized)
			{
				return;
			}
			double captionHeight = SystemParameters.CaptionHeight;
			double top = this.windowChrome.ResizeBorderThickness.Top;
			this.windowChrome.CaptionHeight = captionHeight;
			if (this.TitlebarPlaceholder != null)
			{
				if (windowState == WindowState.Maximized)
				{
					this.TitlebarPlaceholder.Height = captionHeight;
					FrameworkElement frameworkElement = this.window.Content as FrameworkElement;
					if (frameworkElement != null)
					{
						frameworkElement.Margin = new Thickness(this.windowChrome.ResizeBorderThickness.Left + 2.0, this.windowChrome.ResizeBorderThickness.Top + 2.0, this.windowChrome.ResizeBorderThickness.Right + 2.0, this.windowChrome.ResizeBorderThickness.Bottom + 2.0);
						return;
					}
				}
				else
				{
					this.TitlebarPlaceholder.Height = captionHeight + top;
					FrameworkElement frameworkElement2 = this.window.Content as FrameworkElement;
					if (frameworkElement2 != null)
					{
						frameworkElement2.Margin = new Thickness(this.windowChrome.ResizeBorderThickness.Left - 2.0, this.windowChrome.ResizeBorderThickness.Top - 2.0, this.windowChrome.ResizeBorderThickness.Right - 2.0, this.windowChrome.ResizeBorderThickness.Bottom - 2.0);
					}
				}
			}
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x0002D34F File Offset: 0x0002B54F
		public static MicaHelper Create(Window window)
		{
			if (MicaHelper.IsSupported)
			{
				return new MicaHelper(window);
			}
			return null;
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x0002D360 File Offset: 0x0002B560
		private static void SetMicaState(Window window, bool isEnabled, bool darkThemeEnabled)
		{
			int num = 1;
			int num2 = 0;
			IntPtr intPtr = new WindowInteropHelper(window).EnsureHandle();
			if (isEnabled && darkThemeEnabled)
			{
				MicaHelper.DwmSetWindowAttribute(intPtr, MicaHelper.DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref num, Marshal.SizeOf(typeof(int)));
			}
			else
			{
				MicaHelper.DwmSetWindowAttribute(intPtr, MicaHelper.DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref num2, Marshal.SizeOf(typeof(int)));
			}
			if (isEnabled)
			{
				MicaHelper.DwmSetWindowAttribute(intPtr, MicaHelper.DwmWindowAttribute.DWMWA_MICA_EFFECT, ref num, Marshal.SizeOf(typeof(int)));
				return;
			}
			MicaHelper.DwmSetWindowAttribute(intPtr, MicaHelper.DwmWindowAttribute.DWMWA_MICA_EFFECT, ref num2, Marshal.SizeOf(typeof(int)));
		}

		// Token: 0x06000910 RID: 2320
		[DllImport("dwmapi.dll")]
		private static extern int DwmSetWindowAttribute(IntPtr hwnd, MicaHelper.DwmWindowAttribute dwAttribute, ref int pvAttribute, int cbAttribute);

		// Token: 0x04000461 RID: 1121
		private static readonly Version MinSupportedVersion = new Version(10, 0, 22000, 0);

		// Token: 0x04000462 RID: 1122
		private readonly Window window;

		// Token: 0x04000463 RID: 1123
		private WindowChrome windowChrome;

		// Token: 0x04000464 RID: 1124
		private bool isMicaEnabled;

		// Token: 0x04000465 RID: 1125
		private FrameworkElement titlebarPlaceholder;

		// Token: 0x0200043A RID: 1082
		[Flags]
		public enum DwmWindowAttribute : uint
		{
			// Token: 0x0400183C RID: 6204
			DWMWA_USE_HOSTBACKDROPBRUSH = 17U,
			// Token: 0x0400183D RID: 6205
			DWMWA_USE_IMMERSIVE_DARK_MODE = 20U,
			// Token: 0x0400183E RID: 6206
			DWMWA_MICA_EFFECT = 1029U
		}
	}
}
