using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using pdfeditor.Views;

namespace pdfeditor.Controls
{
	// Token: 0x020001C9 RID: 457
	public partial class PdfViewerContextMenu : ContextMenu
	{
		// Token: 0x060019DC RID: 6620 RVA: 0x00066D44 File Offset: 0x00064F44
		static PdfViewerContextMenu()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PdfViewerContextMenu), new FrameworkPropertyMetadata(typeof(PdfViewerContextMenu)));
		}

		// Token: 0x060019DD RID: 6621 RVA: 0x00066DB0 File Offset: 0x00064FB0
		public PdfViewerContextMenu()
		{
			this.currentItems = new List<PdfViewerContextMenuItem>();
			this.timer = new DispatcherTimer(DispatcherPriority.Normal)
			{
				Interval = TimeSpan.FromSeconds(0.05)
			};
			this.timer.Tick += this.Timer_Tick;
			base.Placement = PlacementMode.Custom;
			base.CustomPopupPlacementCallback = new CustomPopupPlacementCallback(this.OnSelectTextContextMenuPlacement);
		}

		// Token: 0x060019DE RID: 6622 RVA: 0x00066E20 File Offset: 0x00065020
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.Border = base.GetTemplateChild("Border") as Border;
			if (this.Border != null)
			{
				this.Border.Opacity = 1.0;
			}
		}

		// Token: 0x060019DF RID: 6623 RVA: 0x00066E5A File Offset: 0x0006505A
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new PdfViewerContextMenuItem();
		}

		// Token: 0x060019E0 RID: 6624 RVA: 0x00066E61 File Offset: 0x00065061
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is PdfViewerContextMenuItem;
		}

		// Token: 0x060019E1 RID: 6625 RVA: 0x00066E6C File Offset: 0x0006506C
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			PdfViewerContextMenuItem pdfViewerContextMenuItem = element as PdfViewerContextMenuItem;
			if (pdfViewerContextMenuItem != null)
			{
				List<PdfViewerContextMenuItem> list = this.currentItems;
				lock (list)
				{
					this.currentItems.Add(pdfViewerContextMenuItem);
				}
			}
			base.PrepareContainerForItemOverride(element, item);
		}

		// Token: 0x060019E2 RID: 6626 RVA: 0x00066EC4 File Offset: 0x000650C4
		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			PdfViewerContextMenuItem pdfViewerContextMenuItem = element as PdfViewerContextMenuItem;
			if (pdfViewerContextMenuItem != null)
			{
				List<PdfViewerContextMenuItem> list = this.currentItems;
				lock (list)
				{
					this.currentItems.Remove(pdfViewerContextMenuItem);
				}
			}
			base.ClearContainerForItemOverride(element, item);
		}

		// Token: 0x060019E3 RID: 6627 RVA: 0x00066F20 File Offset: 0x00065120
		protected override void OnOpened(RoutedEventArgs e)
		{
			base.OnOpened(e);
			if (this.Border != null)
			{
				this.Border.Opacity = 1.0;
				this.centerPoint = new global::System.Windows.Point?(this.Border.PointToScreen(new global::System.Windows.Point(this.Border.ActualWidth / 2.0, this.Border.ActualHeight / 2.0)));
				this.dpiScale = VisualTreeHelper.GetDpi(this.Border);
			}
			else
			{
				this.centerPoint = null;
			}
			if (this.AutoCloseOnMouseLeave)
			{
				this.timer.Start();
			}
		}

		// Token: 0x060019E4 RID: 6628 RVA: 0x00066FC7 File Offset: 0x000651C7
		protected override void OnClosed(RoutedEventArgs e)
		{
			if (this.Border != null)
			{
				this.Border.Opacity = 1.0;
			}
			this.centerPoint = null;
			this.timer.Stop();
			base.OnClosed(e);
		}

		// Token: 0x060019E5 RID: 6629 RVA: 0x00067004 File Offset: 0x00065204
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			MainView mainView = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
			if (mainView != null)
			{
				mainView.RaiseEvent(e);
				if (e.Handled)
				{
					if (base.IsOpen)
					{
						base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
						{
							try
							{
								if (base.IsOpen)
								{
									base.IsOpen = false;
								}
							}
							catch
							{
							}
						}));
					}
					return;
				}
			}
			base.OnPreviewKeyDown(e);
		}

		// Token: 0x060019E6 RID: 6630 RVA: 0x00067064 File Offset: 0x00065264
		private void Timer_Tick(object sender, EventArgs e)
		{
			if (this.Border == null || !base.IsOpen)
			{
				this.timer.Stop();
				return;
			}
			List<PdfViewerContextMenuItem> list = this.currentItems;
			lock (list)
			{
				if (this.currentItems.Any((PdfViewerContextMenuItem c) => c.IsSubmenuOpen))
				{
					this.Border.Opacity = 1.0;
				}
				else
				{
					long pixelDistanceFromCursor = this.GetPixelDistanceFromCursor();
					if (pixelDistanceFromCursor == -1L)
					{
						this.Border.Opacity = 1.0;
					}
					else
					{
						double num = (double)pixelDistanceFromCursor / this.dpiScale.PixelsPerDip;
						double num2 = Math.Max(this.Border.ActualWidth, this.Border.ActualHeight);
						if (num2 <= 150.0)
						{
							num2 = 150.0;
						}
						if (num < num2)
						{
							this.Border.Opacity = 1.0;
						}
						else if (num < num2 * 2.0)
						{
							double num3 = num - num2;
							this.Border.Opacity = (num2 - num3) / num2;
						}
						else
						{
							this.Border.Opacity = 0.0;
						}
					}
				}
			}
			if (this.Border.Opacity < 0.05)
			{
				this.timer.Stop();
				base.IsOpen = false;
			}
		}

		// Token: 0x170009C6 RID: 2502
		// (get) Token: 0x060019E7 RID: 6631 RVA: 0x000671F8 File Offset: 0x000653F8
		// (set) Token: 0x060019E8 RID: 6632 RVA: 0x0006720A File Offset: 0x0006540A
		public bool AutoCloseOnMouseLeave
		{
			get
			{
				return (bool)base.GetValue(PdfViewerContextMenu.AutoCloseOnMouseLeaveProperty);
			}
			set
			{
				base.SetValue(PdfViewerContextMenu.AutoCloseOnMouseLeaveProperty, value);
			}
		}

		// Token: 0x060019E9 RID: 6633 RVA: 0x00067220 File Offset: 0x00065420
		private static void OnAutoCloseOnMouseLeavePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue != (bool)e.OldValue)
			{
				PdfViewerContextMenu pdfViewerContextMenu = d as PdfViewerContextMenu;
				if (pdfViewerContextMenu != null)
				{
					if (pdfViewerContextMenu.IsOpen)
					{
						if ((bool)e.NewValue)
						{
							pdfViewerContextMenu.timer.Start();
						}
						else
						{
							pdfViewerContextMenu.timer.Stop();
						}
					}
					if ((bool)e.NewValue)
					{
						if (pdfViewerContextMenu.IsOpen)
						{
							pdfViewerContextMenu.timer.Start();
						}
						pdfViewerContextMenu.Placement = PlacementMode.Custom;
						pdfViewerContextMenu.CustomPopupPlacementCallback = new CustomPopupPlacementCallback(pdfViewerContextMenu.OnSelectTextContextMenuPlacement);
						return;
					}
					pdfViewerContextMenu.timer.Stop();
					pdfViewerContextMenu.Placement = PlacementMode.MousePoint;
					pdfViewerContextMenu.CustomPopupPlacementCallback = null;
				}
			}
		}

		// Token: 0x060019EA RID: 6634 RVA: 0x000672DC File Offset: 0x000654DC
		private CustomPopupPlacement[] OnSelectTextContextMenuPlacement(global::System.Windows.Size popupSize, global::System.Windows.Size targetSize, global::System.Windows.Point offset)
		{
			FrameworkElement frameworkElement = base.PlacementTarget as FrameworkElement;
			Rect placementRectangle = base.PlacementRectangle;
			double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
			if (frameworkElement == null)
			{
				Action action = null;
				action = delegate
				{
					if (this.IsOpen)
					{
						this.IsOpen = false;
						return;
					}
					this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, action);
				};
				base.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, action);
				return new CustomPopupPlacement[]
				{
					new CustomPopupPlacement(new global::System.Windows.Point(0.0, 0.0), PopupPrimaryAxis.None)
				};
			}
			new global::System.Windows.Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight);
			global::System.Windows.Size size = new global::System.Windows.Size(popupSize.Width / pixelsPerDip, popupSize.Height / pixelsPerDip);
			global::System.Windows.Point position = Mouse.GetPosition(frameworkElement);
			Rect rect = new Rect(position, size);
			if (!placementRectangle.IsEmpty)
			{
				bool flag = Math.Abs(placementRectangle.Left - position.X) < Math.Abs(placementRectangle.Right - position.X);
				bool flag2 = Math.Abs(placementRectangle.Top - position.Y) < Math.Abs(placementRectangle.Bottom - position.Y);
				Rect rect2 = placementRectangle;
				rect2.Intersect(rect);
				if (!rect2.IsEmpty)
				{
					double num = Math.Min(placementRectangle.Left, rect2.Left) - rect.Right;
					double num2 = Math.Max(placementRectangle.Right, rect2.Right) - rect.Left;
					double num3 = Math.Min(placementRectangle.Top, rect2.Top) - rect.Bottom;
					double num4 = Math.Max(placementRectangle.Bottom, rect2.Bottom) - rect.Top;
					double num5 = (flag ? num : num2);
					double num6 = (flag2 ? num3 : num4);
					if (Math.Abs(num5) < Math.Abs(num6))
					{
						rect.Offset(num5, 0.0);
					}
					else
					{
						rect.Offset(0.0, num6);
					}
				}
			}
			double num7 = rect.Left;
			double num8 = rect.Top;
			if (!placementRectangle.IsEmpty)
			{
				num7 -= placementRectangle.Left;
				num8 -= placementRectangle.Top;
			}
			return new CustomPopupPlacement[]
			{
				new CustomPopupPlacement(new global::System.Windows.Point(num7 * pixelsPerDip, num8 * pixelsPerDip), PopupPrimaryAxis.None)
			};
		}

		// Token: 0x060019EB RID: 6635 RVA: 0x00067544 File Offset: 0x00065744
		private long GetPixelDistanceFromCursor()
		{
			PdfViewerContextMenu.POINT point = default(PdfViewerContextMenu.POINT);
			if (!PdfViewerContextMenu.GetCursorPosNative(out point))
			{
				return -1L;
			}
			MainView mainView = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>();
			PdfViewerContextMenu.RECT rect;
			if (mainView != null && PdfViewerContextMenu.GetWindowRect(new WindowInteropHelper(mainView).EnsureHandle(), out rect))
			{
				Rect rect2 = rect;
				if (!rect2.IsEmpty)
				{
					double num = rect2.Width - 17.0 * VisualTreeHelper.GetDpi(mainView).PixelsPerDip;
					if (num >= 0.0)
					{
						rect2.Width = num;
						if (!rect2.Contains((double)point.X, (double)point.Y))
						{
							return 2147483647L;
						}
					}
				}
			}
			if (this.centerPoint != null)
			{
				global::System.Windows.Point value = this.centerPoint.Value;
				return (long)Math.Max(Math.Abs((double)point.X - value.X), Math.Abs((double)point.Y - value.Y));
			}
			return -1L;
		}

		// Token: 0x060019EC RID: 6636
		[DllImport("user32.dll", EntryPoint = "GetCursorPos", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetCursorPosNative(out PdfViewerContextMenu.POINT lpPoint);

		// Token: 0x060019ED RID: 6637
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetWindowRect(IntPtr hWnd, out PdfViewerContextMenu.RECT lpRect);

		// Token: 0x040008EF RID: 2287
		private List<PdfViewerContextMenuItem> currentItems;

		// Token: 0x040008F0 RID: 2288
		private Border Border;

		// Token: 0x040008F1 RID: 2289
		private DispatcherTimer timer;

		// Token: 0x040008F2 RID: 2290
		private global::System.Windows.Point? centerPoint;

		// Token: 0x040008F3 RID: 2291
		private DpiScale dpiScale;

		// Token: 0x040008F4 RID: 2292
		public static readonly DependencyProperty AutoCloseOnMouseLeaveProperty = DependencyProperty.Register("AutoCloseOnMouseLeave", typeof(bool), typeof(PdfViewerContextMenu), new PropertyMetadata(true, new PropertyChangedCallback(PdfViewerContextMenu.OnAutoCloseOnMouseLeavePropertyChanged)));

		// Token: 0x020005F2 RID: 1522
		private struct POINT
		{
			// Token: 0x060032CE RID: 13006 RVA: 0x000F957B File Offset: 0x000F777B
			public POINT(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}

			// Token: 0x060032CF RID: 13007 RVA: 0x000F958B File Offset: 0x000F778B
			public static implicit operator global::System.Drawing.Point(PdfViewerContextMenu.POINT p)
			{
				return new global::System.Drawing.Point(p.X, p.Y);
			}

			// Token: 0x060032D0 RID: 13008 RVA: 0x000F959E File Offset: 0x000F779E
			public static implicit operator PdfViewerContextMenu.POINT(global::System.Drawing.Point p)
			{
				return new PdfViewerContextMenu.POINT(p.X, p.Y);
			}

			// Token: 0x04001FFE RID: 8190
			public int X;

			// Token: 0x04001FFF RID: 8191
			public int Y;
		}

		// Token: 0x020005F3 RID: 1523
		public struct RECT
		{
			// Token: 0x17000D3A RID: 3386
			// (get) Token: 0x060032D1 RID: 13009 RVA: 0x000F95B3 File Offset: 0x000F77B3
			public int Width
			{
				get
				{
					return this.Right - this.Left;
				}
			}

			// Token: 0x17000D3B RID: 3387
			// (get) Token: 0x060032D2 RID: 13010 RVA: 0x000F95C2 File Offset: 0x000F77C2
			public int Height
			{
				get
				{
					return this.Bottom - this.Top;
				}
			}

			// Token: 0x060032D3 RID: 13011 RVA: 0x000F95D4 File Offset: 0x000F77D4
			public static implicit operator PdfViewerContextMenu.RECT(Int32Rect rect)
			{
				return new PdfViewerContextMenu.RECT
				{
					Left = rect.X,
					Top = rect.Y,
					Right = rect.X + rect.Width,
					Bottom = rect.Y + rect.Height
				};
			}

			// Token: 0x060032D4 RID: 13012 RVA: 0x000F9634 File Offset: 0x000F7834
			public static implicit operator Int32Rect(PdfViewerContextMenu.RECT rect)
			{
				if (rect.Left > rect.Right || rect.Top > rect.Bottom)
				{
					return Int32Rect.Empty;
				}
				return new Int32Rect(rect.Left, rect.Top, rect.Width, rect.Height);
			}

			// Token: 0x060032D5 RID: 13013 RVA: 0x000F9684 File Offset: 0x000F7884
			public static implicit operator Rect(PdfViewerContextMenu.RECT rect)
			{
				if (rect.Left > rect.Right || rect.Top > rect.Bottom)
				{
					return Rect.Empty;
				}
				return new Rect((double)rect.Left, (double)rect.Top, (double)rect.Width, (double)rect.Height);
			}

			// Token: 0x04002000 RID: 8192
			public int Left;

			// Token: 0x04002001 RID: 8193
			public int Top;

			// Token: 0x04002002 RID: 8194
			public int Right;

			// Token: 0x04002003 RID: 8195
			public int Bottom;
		}
	}
}
