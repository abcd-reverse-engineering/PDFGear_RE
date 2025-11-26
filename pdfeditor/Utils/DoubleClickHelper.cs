using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace pdfeditor.Utils
{
	// Token: 0x02000079 RID: 121
	public class DoubleClickHelper
	{
		// Token: 0x060008CA RID: 2250 RVA: 0x0002BD41 File Offset: 0x00029F41
		public DoubleClickHelper(UIElement anchorElement)
		{
			if (anchorElement == null)
			{
				throw new ArgumentNullException("anchorElement");
			}
			this.anchorElement = anchorElement;
			this.doubleClickDeltaTime = (long)DoubleClickHelper.GetDoubleClickTime() * 10000L;
			this.doubleClickDeltaSize = DoubleClickHelper.GetDoubleClickDeltaSize();
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x060008CB RID: 2251 RVA: 0x0002BD7D File Offset: 0x00029F7D
		public bool WaitingForSecondClick
		{
			get
			{
				return this.lastClickTick != null && Stopwatch.GetTimestamp() - this.lastClickTick.Value < this.doubleClickDeltaTime;
			}
		}

		// Token: 0x060008CC RID: 2252 RVA: 0x0002BDA8 File Offset: 0x00029FA8
		public bool ProcessMouseClick(MouseButtonEventArgs e)
		{
			if (e == null)
			{
				return false;
			}
			if (e.Handled)
			{
				this.lastClickTick = null;
				this.lastPos = null;
				return false;
			}
			Point physicalPoint = DoubleClickHelper.GetPhysicalPoint(this.anchorElement, e);
			if (!this.WaitingForSecondClick || this.lastPos == null)
			{
				this.lastClickTick = new long?(Stopwatch.GetTimestamp());
				this.lastPos = new Point?(physicalPoint);
				return false;
			}
			if (Stopwatch.GetTimestamp() - this.lastClickTick.Value < this.doubleClickDeltaTime && Math.Abs(this.lastPos.Value.X - physicalPoint.X) <= this.doubleClickDeltaSize.Width && Math.Abs(this.lastPos.Value.Y - physicalPoint.Y) <= this.doubleClickDeltaSize.Height)
			{
				MouseButtonEventHandler mouseDoubleClick = this.MouseDoubleClick;
				if (mouseDoubleClick != null)
				{
					mouseDoubleClick(this, e);
				}
			}
			this.lastClickTick = null;
			this.lastPos = null;
			bool handled = e.Handled;
			e.Handled = false;
			return handled;
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060008CD RID: 2253 RVA: 0x0002BEC8 File Offset: 0x0002A0C8
		// (remove) Token: 0x060008CE RID: 2254 RVA: 0x0002BF00 File Offset: 0x0002A100
		public event MouseButtonEventHandler MouseDoubleClick;

		// Token: 0x060008CF RID: 2255 RVA: 0x0002BF38 File Offset: 0x0002A138
		private static Point GetPhysicalPoint(UIElement anchorElement, MouseButtonEventArgs e)
		{
			Point position = e.GetPosition(anchorElement);
			DpiScale dpi = VisualTreeHelper.GetDpi(anchorElement);
			return new Point(position.X * dpi.PixelsPerDip, position.Y * dpi.PixelsPerDip);
		}

		// Token: 0x060008D0 RID: 2256 RVA: 0x0002BF78 File Offset: 0x0002A178
		private static int GetDoubleClickTime()
		{
			int num = DoubleClickHelper.GetDoubleClickTimeNative();
			if (num < 1)
			{
				num = 100;
			}
			return num;
		}

		// Token: 0x060008D1 RID: 2257 RVA: 0x0002BF93 File Offset: 0x0002A193
		private static Size GetDoubleClickDeltaSize()
		{
			return new Size((double)Math.Max(1, DoubleClickHelper.GetSystemMetricsNative(36) / 2), (double)Math.Max(1, DoubleClickHelper.GetSystemMetricsNative(37) / 2));
		}

		// Token: 0x060008D2 RID: 2258
		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetDoubleClickTime", ExactSpelling = true)]
		private static extern int GetDoubleClickTimeNative();

		// Token: 0x060008D3 RID: 2259
		[DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
		private static extern int GetSystemMetricsNative(int nIndex);

		// Token: 0x0400044C RID: 1100
		private readonly UIElement anchorElement;

		// Token: 0x0400044D RID: 1101
		private long doubleClickDeltaTime;

		// Token: 0x0400044E RID: 1102
		private Size doubleClickDeltaSize;

		// Token: 0x0400044F RID: 1103
		private long? lastClickTick;

		// Token: 0x04000450 RID: 1104
		private Point? lastPos;
	}
}
