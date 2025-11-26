using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommonLib.Common;

namespace FileWatcher.Views
{
	// Token: 0x02000017 RID: 23
	public partial class NotifyView : Window
	{
		// Token: 0x06000061 RID: 97 RVA: 0x000030FC File Offset: 0x000012FC
		public NotifyView(string filePath)
		{
			this.InitializeComponent();
			this.filePath = filePath;
			base.Loaded += this.NotifyView_Loaded;
			base.IsVisibleChanged += this.NotifyView_IsVisibleChanged;
			this.LayoutRoot.Opacity = 0.0;
			this.FileName.Text = global::System.IO.Path.GetFileName(filePath);
			this.FilePath.Text = global::System.IO.Path.GetFullPath(filePath);
			long length = new FileInfo(filePath).Length;
			if (length < 102L)
			{
				this.FileSize.Text = string.Format("{0:0.##}B", length);
			}
			else if (length < 102400L)
			{
				this.FileSize.Text = string.Format("{0:0.##}KB", (double)length / 1024.0);
			}
			else if (length < 104857600L)
			{
				this.FileSize.Text = string.Format("{0:0.##}MB", (double)length / 1024.0 / 1024.0);
			}
			else
			{
				this.FileSize.Text = string.Format("{0:0.##}GB", (double)length / 1024.0 / 1024.0 / 1024.0);
			}
			this.closetimer = new DispatcherTimer
			{
				Interval = TimeSpan.FromSeconds(10.0)
			};
			this.closetimer.Tick += this.Closetimer_Tick;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000328F File Offset: 0x0000148F
		private void NotifyView_Loaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003294 File Offset: 0x00001494
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			IntPtr handle = new WindowInteropHelper(this).Handle;
			long num = (long)NotifyView.NativeMethods.GetWindowLongPtr(handle, -20);
			num |= 134217856L;
			NotifyView.NativeMethods.SetWindowLongPtr(handle, -20, new IntPtr(num));
			GAManager2.SendEvent("FileWatcher", "NotifyShow", "Count", 1L);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000032F0 File Offset: 0x000014F0
		private void NotifyView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			object newValue = e.NewValue;
			if (newValue is bool && !(bool)newValue)
			{
				return;
			}
			foreach (NotifyView notifyView in App.Current.Windows.OfType<NotifyView>().ToList<NotifyView>())
			{
				if (notifyView != this)
				{
					notifyView.closed = true;
					notifyView.Close();
				}
			}
			Screen primaryScreen = Screen.PrimaryScreen;
			global::System.Drawing.Rectangle bounds = primaryScreen.Bounds;
			uint num;
			uint num2;
			NotifyView.NativeMethods.GetDpiForMonitor(NotifyView.NativeMethods.MonitorFromPoint(new global::System.Drawing.Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2), NotifyView.NativeMethods.MonitorOptions.MONITOR_DEFAULTTOPRIMARY), out num, out num2);
			double num3 = num / 96.0;
			double width = base.Width;
			double height = base.Height;
			int num4 = (int)Math.Ceiling(width * num3);
			int num5 = (int)Math.Ceiling(height * num3);
			global::System.Drawing.Rectangle workingArea = primaryScreen.WorkingArea;
			int num6 = workingArea.Right - num4;
			int num7 = workingArea.Bottom - num5;
			NotifyView.NativeMethods.SetWindowPos(new WindowInteropHelper(this).Handle, IntPtr.Zero, num6, num7, num4, num5, NotifyView.NativeMethods.SetWindowPosFlags.SWP_NOACTIVATE | NotifyView.NativeMethods.SetWindowPosFlags.SWP_NOZORDER);
			base.Width = width;
			base.Height = height;
			((Storyboard)base.Resources["ShowWindow"]).Begin();
			this.LayoutRoot.Opacity = 1.0;
			this.closetimer.Start();
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000347C File Offset: 0x0000167C
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			this.OpenFile();
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000348C File Offset: 0x0000168C
		private void OpenFile()
		{
			GAManager2.SendEvent("FileWatcher", "OpenFile", "Count", 1L);
			string text = global::System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PDFLauncher.exe");
			if (!this.filePath.StartsWith("\""))
			{
				this.filePath = "\"" + this.filePath + "\"";
			}
			NotifyView.NativeMethods.ShellExecute(IntPtr.Zero, "open", text, this.filePath, "", 1);
			base.Close();
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003514 File Offset: 0x00001714
		private void DisableFileWatcherCheckBox_Click(object sender, RoutedEventArgs e)
		{
			base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				bool flag = !this.DisableFileWatcherCheckBox.IsChecked.GetValueOrDefault();
				GAManager2.SendEvent("FileWatcher", "NotifyCheck", base.IsEnabled.ToString(), 1L);
				SettingsHelper.SetIsEnabled(flag);
			}));
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003530 File Offset: 0x00001730
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			this.closetimer.Stop();
			if (this.closed)
			{
				return;
			}
			e.Cancel = true;
			if (!this.closing)
			{
				this.closing = true;
				((Storyboard)base.Resources["HideWindow"]).Completed += this.NotifyView_Completed;
				((Storyboard)base.Resources["HideWindow"]).Begin();
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000035AE File Offset: 0x000017AE
		private void Closetimer_Tick(object sender, EventArgs e)
		{
			base.Close();
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000035B8 File Offset: 0x000017B8
		private void NotifyView_Completed(object sender, EventArgs e)
		{
			bool flag = !this.DisableFileWatcherCheckBox.IsChecked.GetValueOrDefault();
			this.closed = true;
			base.Close();
			SettingsHelper.SetIsEnabled(flag);
			if (!SettingsHelper.IsEnabled)
			{
				App.Current.Shutdown();
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000035FE File Offset: 0x000017FE
		private void Close_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003606 File Offset: 0x00001806
		private void OpenButton_Click(object sender, RoutedEventArgs e)
		{
			this.OpenFile();
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000360E File Offset: 0x0000180E
		public static CornerRadius GetCornerRadius(DependencyObject obj)
		{
			return (CornerRadius)obj.GetValue(NotifyView.CornerRadiusProperty);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003620 File Offset: 0x00001820
		public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
		{
			obj.SetValue(NotifyView.CornerRadiusProperty, value);
		}

		// Token: 0x0400003F RID: 63
		private string filePath = "";

		// Token: 0x04000040 RID: 64
		private bool closing;

		// Token: 0x04000041 RID: 65
		private bool closed;

		// Token: 0x04000042 RID: 66
		private DispatcherTimer closetimer;

		// Token: 0x04000043 RID: 67
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(NotifyView), new PropertyMetadata(default(CornerRadius)));

		// Token: 0x0200001E RID: 30
		private class NativeMethods
		{
			// Token: 0x06000080 RID: 128
			[DllImport("user32.dll", EntryPoint = "GetWindowLong")]
			private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

			// Token: 0x06000081 RID: 129
			[DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
			private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

			// Token: 0x06000082 RID: 130
			[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
			private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

			// Token: 0x06000083 RID: 131
			[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
			private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

			// Token: 0x06000084 RID: 132 RVA: 0x00003A47 File Offset: 0x00001C47
			public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
			{
				if (IntPtr.Size == 8)
				{
					return NotifyView.NativeMethods.GetWindowLongPtr64(hWnd, nIndex);
				}
				return NotifyView.NativeMethods.GetWindowLongPtr32(hWnd, nIndex);
			}

			// Token: 0x06000085 RID: 133 RVA: 0x00003A60 File Offset: 0x00001C60
			public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
			{
				if (IntPtr.Size == 8)
				{
					return NotifyView.NativeMethods.SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
				}
				return new IntPtr(NotifyView.NativeMethods.SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
			}

			// Token: 0x06000086 RID: 134
			[DllImport("gdi32.dll")]
			public static extern bool DeleteObject(IntPtr hObject);

			// Token: 0x06000087 RID: 135
			[DllImport("user32.dll", SetLastError = true)]
			private static extern IntPtr MonitorFromPoint(NotifyView.NativeMethods.POINT pt, NotifyView.NativeMethods.MonitorOptions dwFlags);

			// Token: 0x06000088 RID: 136 RVA: 0x00003A86 File Offset: 0x00001C86
			public static IntPtr MonitorFromPoint(global::System.Drawing.Point pt, NotifyView.NativeMethods.MonitorOptions dwFlags)
			{
				return NotifyView.NativeMethods.MonitorFromPoint(new NotifyView.NativeMethods.POINT(pt.X, pt.Y), dwFlags);
			}

			// Token: 0x06000089 RID: 137
			[DllImport("shcore.dll", CharSet = CharSet.Auto, SetLastError = true)]
			private static extern uint GetDpiForMonitor(IntPtr hMonitor, int dpiType, out uint dpiX, out uint dpiY);

			// Token: 0x0600008A RID: 138 RVA: 0x00003AA1 File Offset: 0x00001CA1
			public static uint GetDpiForMonitor(IntPtr hMonitor, out uint dpiX, out uint dpiY)
			{
				return NotifyView.NativeMethods.GetDpiForMonitor(hMonitor, 0, out dpiX, out dpiY);
			}

			// Token: 0x0600008B RID: 139
			[DllImport("user32.dll", SetLastError = true)]
			public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, NotifyView.NativeMethods.SetWindowPosFlags uFlags);

			// Token: 0x0600008C RID: 140
			[DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
			public static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

			// Token: 0x02000021 RID: 33
			public enum MonitorOptions : uint
			{
				// Token: 0x0400005F RID: 95
				MONITOR_DEFAULTTONULL,
				// Token: 0x04000060 RID: 96
				MONITOR_DEFAULTTOPRIMARY,
				// Token: 0x04000061 RID: 97
				MONITOR_DEFAULTTONEAREST
			}

			// Token: 0x02000022 RID: 34
			public struct POINT
			{
				// Token: 0x06000093 RID: 147 RVA: 0x00003CE6 File Offset: 0x00001EE6
				public POINT(int x, int y)
				{
					this.X = x;
					this.Y = y;
				}

				// Token: 0x04000062 RID: 98
				public int X;

				// Token: 0x04000063 RID: 99
				public int Y;
			}

			// Token: 0x02000023 RID: 35
			[Flags]
			public enum SetWindowPosFlags : uint
			{
				// Token: 0x04000065 RID: 101
				SWP_ASYNCWINDOWPOS = 16384U,
				// Token: 0x04000066 RID: 102
				SWP_DEFERERASE = 8192U,
				// Token: 0x04000067 RID: 103
				SWP_DRAWFRAME = 32U,
				// Token: 0x04000068 RID: 104
				SWP_FRAMECHANGED = 32U,
				// Token: 0x04000069 RID: 105
				SWP_HIDEWINDOW = 128U,
				// Token: 0x0400006A RID: 106
				SWP_NOACTIVATE = 16U,
				// Token: 0x0400006B RID: 107
				SWP_NOCOPYBITS = 256U,
				// Token: 0x0400006C RID: 108
				SWP_NOMOVE = 2U,
				// Token: 0x0400006D RID: 109
				SWP_NOOWNERZORDER = 512U,
				// Token: 0x0400006E RID: 110
				SWP_NOREDRAW = 8U,
				// Token: 0x0400006F RID: 111
				SWP_NOREPOSITION = 512U,
				// Token: 0x04000070 RID: 112
				SWP_NOSENDCHANGING = 1024U,
				// Token: 0x04000071 RID: 113
				SWP_NOSIZE = 1U,
				// Token: 0x04000072 RID: 114
				SWP_NOZORDER = 4U,
				// Token: 0x04000073 RID: 115
				SWP_SHOWWINDOW = 64U
			}
		}
	}
}
