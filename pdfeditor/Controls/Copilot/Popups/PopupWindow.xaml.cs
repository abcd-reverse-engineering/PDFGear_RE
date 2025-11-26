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

namespace pdfeditor.Controls.Copilot.Popups
{
	// Token: 0x02000293 RID: 659
	public partial class PopupWindow : Window
	{
		// Token: 0x060025E3 RID: 9699 RVA: 0x000B0B64 File Offset: 0x000AED64
		static PopupWindow()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupWindow), new FrameworkPropertyMetadata(typeof(PopupWindow)));
			Window.IconProperty.OverrideMetadata(typeof(PopupWindow), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(PopupWindow.OnIconPropertyChanged)));
		}

		// Token: 0x060025E4 RID: 9700 RVA: 0x000B0BBC File Offset: 0x000AEDBC
		private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PopupWindow popupWindow = d as PopupWindow;
			if (popupWindow != null)
			{
				popupWindow.UpdateIcon();
			}
		}

		// Token: 0x060025E5 RID: 9701 RVA: 0x000B0BDC File Offset: 0x000AEDDC
		private static ImageSource EnsureDefaultAppIcon()
		{
			if (PopupWindow.defaultAppIcon == null)
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
								PopupWindow.defaultAppIcon = Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, int32Rect, BitmapSizeOptions.FromEmptyOptions());
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
			return PopupWindow.defaultAppIcon;
		}

		// Token: 0x060025E6 RID: 9702 RVA: 0x000B0CCC File Offset: 0x000AEECC
		public PopupWindow()
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

		// Token: 0x060025E7 RID: 9703 RVA: 0x000B0D84 File Offset: 0x000AEF84
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
				this.WindowDefaultIconImage.Source = PopupWindow.EnsureDefaultAppIcon();
			}
		}

		// Token: 0x060025E8 RID: 9704 RVA: 0x000B0E30 File Offset: 0x000AF030
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

		// Token: 0x060025E9 RID: 9705 RVA: 0x000B0EE2 File Offset: 0x000AF0E2
		private void PopupWindow_Loaded(object sender, RoutedEventArgs e)
		{
			PopupWindow.UpdateWindowFrame(this);
		}

		// Token: 0x060025EA RID: 9706 RVA: 0x000B0EEA File Offset: 0x000AF0EA
		protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
		{
			base.OnDpiChanged(oldDpi, newDpi);
			base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
			{
				PopupWindow.UpdateWindowFrame(this);
			}));
		}

		// Token: 0x060025EB RID: 9707 RVA: 0x000B0F0E File Offset: 0x000AF10E
		private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
		{
			SystemCommands.CloseWindow(this);
		}

		// Token: 0x060025EC RID: 9708 RVA: 0x000B0F18 File Offset: 0x000AF118
		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			this.hwndSource = (HwndSource)PresentationSource.FromVisual(this);
			this.hwndSource.AddHook(new HwndSourceHook(this.OnWindowProc));
			int num = NativeMethods.GetWindowLongPtr(this.hwndSource.Handle, -16).ToInt32();
			int num2 = 0;
			PopupWindow.DisableTitleBar(ref num, ref num2);
			NativeMethods.SetWindowLongPtr(this.hwndSource.Handle, -16, new IntPtr(num));
			base.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
			{
				PopupWindow.UpdateWindowFrame(this);
			}));
		}

		// Token: 0x060025ED RID: 9709 RVA: 0x000B0FB0 File Offset: 0x000AF1B0
		private static void UpdateWindowFrame(PopupWindow window)
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

		// Token: 0x060025EE RID: 9710 RVA: 0x000B100C File Offset: 0x000AF20C
		private void UpdateIcon()
		{
		}

		// Token: 0x060025EF RID: 9711 RVA: 0x000B1010 File Offset: 0x000AF210
		private unsafe IntPtr OnWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == 125)
			{
				if (wParam.ToInt32() == -16)
				{
					PopupWindow.StyleStruct* ptr = (PopupWindow.StyleStruct*)lParam.ToPointer();
					int styleNew = ptr->styleNew;
					int num = 0;
					PopupWindow.DisableTitleBar(ref styleNew, ref num);
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

		// Token: 0x060025F0 RID: 9712 RVA: 0x000B1094 File Offset: 0x000AF294
		private static void DisableTitleBar(ref int style, ref int exStyle)
		{
			style &= -12779521;
		}

		// Token: 0x0400105D RID: 4189
		private static ImageSource defaultAppIcon;

		// Token: 0x0400105E RID: 4190
		private HwndSource hwndSource;

		// Token: 0x0400105F RID: 4191
		private Grid WindowIcon;

		// Token: 0x04001060 RID: 4192
		private global::System.Windows.Controls.Image WindowDefaultIconImage;

		// Token: 0x0200075B RID: 1883
		private struct StyleStruct
		{
			// Token: 0x0400254C RID: 9548
			public int styleOld;

			// Token: 0x0400254D RID: 9549
			public int styleNew;
		}
	}
}
