using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using pdfeditor.Utils;

namespace pdfeditor.Controls.Translation.Popups
{
	// Token: 0x020001E5 RID: 485
	public partial class TranslatePopupWindow : Window
	{
		// Token: 0x06001B85 RID: 7045 RVA: 0x0007028C File Offset: 0x0006E48C
		static TranslatePopupWindow()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TranslatePopupWindow), new FrameworkPropertyMetadata(typeof(TranslatePopupWindow)));
			Window.IconProperty.OverrideMetadata(typeof(TranslatePopupWindow), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(TranslatePopupWindow.OnIconPropertyChanged)));
		}

		// Token: 0x06001B86 RID: 7046 RVA: 0x000702E4 File Offset: 0x0006E4E4
		private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TranslatePopupWindow translatePopupWindow = d as TranslatePopupWindow;
			if (translatePopupWindow != null)
			{
				translatePopupWindow.UpdateIcon();
			}
		}

		// Token: 0x06001B87 RID: 7047 RVA: 0x00070304 File Offset: 0x0006E504
		private static ImageSource EnsureDefaultAppIcon()
		{
			if (TranslatePopupWindow.defaultAppIcon == null)
			{
				try
				{
					using (Icon icon = global::System.Drawing.Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName))
					{
						using (Bitmap bitmap = Bitmap.FromHicon(icon.Handle))
						{
							Int32Rect int32Rect = new Int32Rect(0, 0, bitmap.Width, bitmap.Height);
							IntPtr intPtr = IntPtr.Zero;
							try
							{
								intPtr = bitmap.GetHbitmap();
								TranslatePopupWindow.defaultAppIcon = Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, int32Rect, BitmapSizeOptions.FromEmptyOptions());
							}
							finally
							{
								try
								{
									if (intPtr != IntPtr.Zero)
									{
										NativeMethods.DeleteObject(intPtr);
									}
								}
								catch
								{
								}
							}
						}
					}
				}
				catch
				{
				}
			}
			return TranslatePopupWindow.defaultAppIcon;
		}

		// Token: 0x06001B88 RID: 7048 RVA: 0x000703F4 File Offset: 0x0006E5F4
		public TranslatePopupWindow()
		{
			WindowChrome windowChrome = new WindowChrome
			{
				NonClientFrameEdges = (NonClientFrameEdges.Left | NonClientFrameEdges.Right | NonClientFrameEdges.Bottom),
				UseAeroCaptionButtons = false,
				CaptionHeight = 32.0,
				GlassFrameThickness = new Thickness(1.0),
				ResizeBorderThickness = new Thickness(8.0, 4.0, 8.0, 8.0)
			};
			WindowChrome.SetWindowChrome(this, windowChrome);
			base.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, new ExecutedRoutedEventHandler(this.CloseWindow)));
			base.Loaded += this.PopupWindow_Loaded;
		}

		// Token: 0x06001B89 RID: 7049 RVA: 0x000704AC File Offset: 0x0006E6AC
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (this.WindowIcon != null)
			{
				this.WindowIcon.MouseDown -= this.WindowIcon_MouseDown;
			}
			if (this.WindowDefaultIconImage != null)
			{
				this.WindowDefaultIconImage.Source = null;
			}
			this.WindowIcon = base.GetTemplateChild("WindowIcon") as Grid;
			this.WindowDefaultIconImage = base.GetTemplateChild("WindowDefaultIconImage") as global::System.Windows.Controls.Image;
			if (this.WindowIcon != null)
			{
				this.WindowIcon.MouseDown += this.WindowIcon_MouseDown;
			}
			if (this.WindowDefaultIconImage != null)
			{
				this.WindowDefaultIconImage.Source = TranslatePopupWindow.EnsureDefaultAppIcon();
			}
		}

		// Token: 0x06001B8A RID: 7050 RVA: 0x00070558 File Offset: 0x0006E758
		private void WindowIcon_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2)
			{
				SystemCommands.CloseWindow(this);
				return;
			}
			NativeMethods.RECT rect;
			if (this.hwndSource != null && NativeMethods.GetWindowRect(this.hwndSource.Handle, out rect))
			{
				DpiScale dpi = VisualTreeHelper.GetDpi(this);
				double num;
				if (base.FlowDirection == FlowDirection.LeftToRight)
				{
					num = (double)rect.left / dpi.PixelsPerDip + 3.0;
				}
				else
				{
					num = (double)rect.right / dpi.PixelsPerDip - 3.0;
				}
				double num2 = (double)rect.top / dpi.PixelsPerDip + 32.0;
				SystemCommands.ShowSystemMenu(this, new global::System.Windows.Point(num, num2));
			}
		}

		// Token: 0x06001B8B RID: 7051 RVA: 0x0007060A File Offset: 0x0006E80A
		private void PopupWindow_Loaded(object sender, RoutedEventArgs e)
		{
			TranslatePopupWindow.UpdateWindowFrame(this);
		}

		// Token: 0x06001B8C RID: 7052 RVA: 0x00070612 File Offset: 0x0006E812
		protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
		{
			base.OnDpiChanged(oldDpi, newDpi);
			base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
			{
				TranslatePopupWindow.UpdateWindowFrame(this);
			}));
		}

		// Token: 0x06001B8D RID: 7053 RVA: 0x00070636 File Offset: 0x0006E836
		private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
		{
			SystemCommands.CloseWindow(this);
		}

		// Token: 0x06001B8E RID: 7054 RVA: 0x00070640 File Offset: 0x0006E840
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			this.hwndSource = (HwndSource)PresentationSource.FromVisual(this);
			this.hwndSource.AddHook(new HwndSourceHook(this.OnWindowProc));
			int num = NativeMethods.GetWindowLongPtr(this.hwndSource.Handle, -16).ToInt32();
			int num2 = 0;
			TranslatePopupWindow.DisableTitleBar(ref num, ref num2);
			NativeMethods.SetWindowLongPtr(this.hwndSource.Handle, -16, new IntPtr(num));
			base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
			{
				TranslatePopupWindow.UpdateWindowFrame(this);
			}));
		}

		// Token: 0x06001B8F RID: 7055 RVA: 0x000706D8 File Offset: 0x0006E8D8
		private static void UpdateWindowFrame(TranslatePopupWindow window)
		{
			if (VisualTreeHelper.GetChildrenCount(window) > 0)
			{
				FrameworkElement frameworkElement = VisualTreeHelper.GetChild(window, 0) as FrameworkElement;
				if (frameworkElement != null)
				{
					double width = frameworkElement.Width;
					frameworkElement.Width = 0.0;
					frameworkElement.InvalidateMeasure();
					frameworkElement.UpdateLayout();
					frameworkElement.Width = width;
					return;
				}
			}
			window.InvalidateMeasure();
			window.UpdateLayout();
		}

		// Token: 0x06001B90 RID: 7056 RVA: 0x00070734 File Offset: 0x0006E934
		private void UpdateIcon()
		{
		}

		// Token: 0x06001B91 RID: 7057 RVA: 0x00070738 File Offset: 0x0006E938
		private unsafe IntPtr OnWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == 125)
			{
				if (wParam.ToInt32() == -16)
				{
					TranslatePopupWindow.StyleStruct* ptr = (TranslatePopupWindow.StyleStruct*)lParam.ToPointer();
					int styleNew = ptr->styleNew;
					int num = 0;
					TranslatePopupWindow.DisableTitleBar(ref styleNew, ref num);
					ptr->styleNew = styleNew;
					handled = true;
					return IntPtr.Zero;
				}
			}
			else if (msg == 132)
			{
				try
				{
					if (lParam.ToInt64() > 2147483647L)
					{
						handled = true;
					}
				}
				catch (OverflowException)
				{
					handled = true;
				}
			}
			return IntPtr.Zero;
		}

		// Token: 0x06001B92 RID: 7058 RVA: 0x000707BC File Offset: 0x0006E9BC
		private static void DisableTitleBar(ref int style, ref int exStyle)
		{
			style &= -12779521;
		}

		// Token: 0x040009D7 RID: 2519
		private static ImageSource defaultAppIcon;

		// Token: 0x040009D8 RID: 2520
		private HwndSource hwndSource;

		// Token: 0x040009D9 RID: 2521
		private Grid WindowIcon;

		// Token: 0x040009DA RID: 2522
		private global::System.Windows.Controls.Image WindowDefaultIconImage;

		// Token: 0x0200061A RID: 1562
		private struct StyleStruct
		{
			// Token: 0x0400206B RID: 8299
			public int styleOld;

			// Token: 0x0400206C RID: 8300
			public int styleNew;
		}
	}
}
