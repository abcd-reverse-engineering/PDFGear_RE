using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using CommonLib.Common;
using Patagames.Pdf.Net;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.Views;
using PDFKit;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x020001FE RID: 510
	public partial class ComparisonWindow : Window
	{
		// Token: 0x06001C8D RID: 7309 RVA: 0x000773BC File Offset: 0x000755BC
		public ComparisonWindow()
		{
			this.InitializeComponent();
			base.Width = 1.0;
			base.Height = 1.0;
			base.Left = 0.0;
			base.Top = 0.0;
			this.LayoutRoot.Opacity = 0.0;
			base.Loaded += this.ComparisonWindow_Loaded;
			base.SourceInitialized += this.ComparisonWindow_SourceInitialized;
			base.MouseLeftButtonDown += this.ComparisonWindow_MouseLeftButtonDown;
		}

		// Token: 0x06001C8E RID: 7310 RVA: 0x0007745B File Offset: 0x0007565B
		private void ComparisonWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			base.DragMove();
		}

		// Token: 0x06001C8F RID: 7311 RVA: 0x00077464 File Offset: 0x00075664
		private void ComparisonWindow_SourceInitialized(object sender, EventArgs e)
		{
			IntPtr handle = new WindowInteropHelper(this).Handle;
			this.SetWindowActivable(handle, false);
		}

		// Token: 0x06001C90 RID: 7312 RVA: 0x00077485 File Offset: 0x00075685
		private void ComparisonWindow_Loaded(object sender, RoutedEventArgs e)
		{
			this.UpdateWindowState();
		}

		// Token: 0x06001C91 RID: 7313 RVA: 0x00077490 File Offset: 0x00075690
		public void SetContent(PdfDocument document, ScreenshotDialogResult result)
		{
			this.result = result;
			if (result == null)
			{
				base.Hide();
				this.ContentImage.Source = null;
				this.viewer = null;
				return;
			}
			if (document != null)
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(document);
				this.viewer = ((pdfControl != null) ? pdfControl.Viewer : null);
			}
			this.IsThumbnail = true;
			base.Show();
			this.UpdateWindowState();
			this.SetThumbnailLocation();
			this.ContentImage.Source = result.RotatedImage ?? result.Image;
		}

		// Token: 0x06001C92 RID: 7314 RVA: 0x00077510 File Offset: 0x00075710
		private void SetThumbnailLocation()
		{
			int num = 96;
			double num2;
			double num3;
			if (this.viewer != null)
			{
				Window window = Window.GetWindow(this.viewer);
				if (window != null)
				{
					num = (int)VisualTreeHelper.GetDpi(this.viewer).PixelsPerInchX;
					global::System.Windows.Point point = this.viewer.TranslatePoint(default(global::System.Windows.Point), window);
					int titlebarHeight = ComparisonWindow.GetTitlebarHeight();
					num2 = window.Left + 20.0;
					num3 = window.Top + point.Y + (double)((float)titlebarHeight * 96f / (float)num) + 20.0;
					goto IL_00CB;
				}
			}
			Rect scaledWorkArea = this.GetScaledWorkArea(out num);
			num2 = scaledWorkArea.Left + 100.0;
			num3 = scaledWorkArea.Top + 100.0;
			IL_00CB:
			base.Left = num2;
			base.Top = num3;
		}

		// Token: 0x17000A36 RID: 2614
		// (get) Token: 0x06001C93 RID: 7315 RVA: 0x000775F6 File Offset: 0x000757F6
		// (set) Token: 0x06001C94 RID: 7316 RVA: 0x00077608 File Offset: 0x00075808
		public bool IsThumbnail
		{
			get
			{
				return (bool)base.GetValue(ComparisonWindow.IsThumbnailProperty);
			}
			set
			{
				base.SetValue(ComparisonWindow.IsThumbnailProperty, value);
			}
		}

		// Token: 0x06001C95 RID: 7317 RVA: 0x0007761C File Offset: 0x0007581C
		private static void OnIsThumbnailPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue != (bool)e.OldValue)
			{
				ComparisonWindow comparisonWindow = d as ComparisonWindow;
				if (comparisonWindow != null)
				{
					comparisonWindow.UpdateWindowState();
				}
			}
		}

		// Token: 0x06001C96 RID: 7318 RVA: 0x00077654 File Offset: 0x00075854
		private void UpdateWindowState()
		{
			if (!base.IsVisible || this.result == null)
			{
				return;
			}
			this.LayoutRoot.Opacity = 1.0;
			int num;
			Rect scaledWorkArea = this.GetScaledWorkArea(out num);
			string text;
			string text2;
			double num2;
			double num3;
			if (this.IsThumbnail)
			{
				text = "\ue1d9";
				text2 = pdfeditor.Properties.Resources.WinScreenshotToolbarZoomInContent;
				num2 = 140.0;
				num3 = 140.0;
			}
			else
			{
				text = "\ue1d8";
				text2 = pdfeditor.Properties.Resources.WinScreenshotToolbarZoomOutContent;
				global::System.Windows.Size size = this.result.SelectedClientRect.Size;
				if (size.Width > scaledWorkArea.Width)
				{
					size = new global::System.Windows.Size(scaledWorkArea.Width, size.Height * scaledWorkArea.Width / size.Width);
				}
				if (size.Height > scaledWorkArea.Height)
				{
					size = new global::System.Windows.Size(size.Width * scaledWorkArea.Height / size.Height, scaledWorkArea.Height);
				}
				num2 = size.Width;
				num3 = size.Height;
			}
			this.ScaleButton.Content = text;
			ToolTipService.SetToolTip(this.ScaleButton, text2);
			base.Width = num2;
			base.Height = num3;
			this.ContentImage.Width = num2;
			this.ContentImage.Height = num3;
		}

		// Token: 0x06001C97 RID: 7319 RVA: 0x000777BC File Offset: 0x000759BC
		private Rect GetScaledWorkArea(out int dpi)
		{
			HandleRef? handleRef = null;
			dpi = 96;
			if (ComparisonWindow.MultiMonitorSupport())
			{
				MainView mainView = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
				HandleRef handleRef2;
				uint num;
				uint num2;
				if (mainView != null && ScreenUtils.TryGetMonitorForWindow(mainView, out handleRef2) && ScreenUtils.TryGetDpiForMonitor(handleRef2, out num, out num2))
				{
					handleRef = new HandleRef?(handleRef2);
					dpi = (int)num;
				}
			}
			Int32Rect int32Rect;
			Int32Rect int32Rect2;
			bool flag;
			if (handleRef != null && ScreenUtils.GetMonitorInfo(handleRef.Value.Handle, out int32Rect, out int32Rect2, out flag))
			{
				float num3 = (float)dpi / 96f;
				return new Rect((double)((float)int32Rect2.X * num3), (double)((float)int32Rect2.Y * num3), (double)((float)int32Rect2.Width * num3), (double)((float)int32Rect2.Height * num3));
			}
			using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
			{
				dpi = (int)graphics.DpiX;
			}
			return SystemParameters.WorkArea;
		}

		// Token: 0x06001C98 RID: 7320 RVA: 0x000778B8 File Offset: 0x00075AB8
		public static int GetTitlebarHeight()
		{
			return ComparisonWindow.GetSystemMetrics(4);
		}

		// Token: 0x06001C99 RID: 7321 RVA: 0x000778C0 File Offset: 0x00075AC0
		private static bool MultiMonitorSupport()
		{
			return ComparisonWindow.GetSystemMetrics(80) != 0;
		}

		// Token: 0x06001C9A RID: 7322 RVA: 0x000778CC File Offset: 0x00075ACC
		private void SetWindowActivable(IntPtr hwnd, bool value)
		{
			int num = ComparisonWindow.GetWindowLongW(hwnd, -20);
			if (value)
			{
				num |= 134217728;
			}
			else
			{
				num &= -134217729;
			}
			ComparisonWindow.SetWindowLongW(hwnd, -20, num);
		}

		// Token: 0x06001C9B RID: 7323
		[DllImport("user32", ExactSpelling = true, SetLastError = true)]
		private static extern int GetWindowLongW(IntPtr hWnd, int nIndex);

		// Token: 0x06001C9C RID: 7324
		[DllImport("user32", ExactSpelling = true, SetLastError = true)]
		private static extern int SetWindowLongW(IntPtr hWnd, int nIndex, int dwNewLong);

		// Token: 0x06001C9D RID: 7325
		[DllImport("user32", ExactSpelling = true)]
		private static extern int GetSystemMetrics(int nIndex);

		// Token: 0x06001C9E RID: 7326 RVA: 0x00077904 File Offset: 0x00075B04
		private async void CopyButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.result != null)
			{
				((Button)sender).IsEnabled = false;
				try
				{
					Clipboard.SetImage(this.result.RotatedImage ?? this.result.Image);
				}
				catch
				{
				}
				await Task.Delay(300);
				((Button)sender).IsEnabled = true;
				GAManager.SendEvent("Screenshot", "Copy2", "Count", 1L);
			}
		}

		// Token: 0x06001C9F RID: 7327 RVA: 0x00077943 File Offset: 0x00075B43
		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			this.SetContent(null, null);
		}

		// Token: 0x06001CA0 RID: 7328 RVA: 0x0007794D File Offset: 0x00075B4D
		private void ScaleButton_Click(object sender, RoutedEventArgs e)
		{
			this.IsThumbnail = !this.IsThumbnail;
		}

		// Token: 0x06001CA1 RID: 7329 RVA: 0x0007795E File Offset: 0x00075B5E
		private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.IsThumbnail = !this.IsThumbnail;
		}

		// Token: 0x04000A7B RID: 2683
		private ScreenshotDialogResult result;

		// Token: 0x04000A7C RID: 2684
		private PdfViewer viewer;

		// Token: 0x04000A7D RID: 2685
		public static readonly DependencyProperty IsThumbnailProperty = DependencyProperty.Register("IsThumbnail", typeof(bool), typeof(ComparisonWindow), new PropertyMetadata(true, new PropertyChangedCallback(ComparisonWindow.OnIsThumbnailPropertyChanged)));
	}
}
