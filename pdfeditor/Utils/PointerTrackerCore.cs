using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace pdfeditor.Utils
{
	// Token: 0x02000096 RID: 150
	public class PointerTrackerCore
	{
		// Token: 0x060009C7 RID: 2503 RVA: 0x00031EC8 File Offset: 0x000300C8
		public PointerTrackerCore()
		{
			this.pointerContexts = new ConcurrentDictionary<int, PointerTrackerCore.PointerContext>();
		}

		// Token: 0x060009C8 RID: 2504 RVA: 0x00031EE3 File Offset: 0x000300E3
		public void ProcessDownEvent(Point point, MouseButton button)
		{
			this.ProcessDownEventCore(-1, point, button);
		}

		// Token: 0x060009C9 RID: 2505 RVA: 0x00031EEE File Offset: 0x000300EE
		public void ProcessDownEvent(int pointerId, Point point)
		{
			this.ProcessDownEventCore(pointerId, point, MouseButton.Left);
		}

		// Token: 0x060009CA RID: 2506 RVA: 0x00031EF9 File Offset: 0x000300F9
		public void ProcessUpEvent(Point point, MouseButton button)
		{
			this.ProcessUpEventCore(-1, point, button);
		}

		// Token: 0x060009CB RID: 2507 RVA: 0x00031F04 File Offset: 0x00030104
		public void ProcessUpEvent(int pointerId, Point point)
		{
			this.ProcessUpEventCore(pointerId, point, MouseButton.Left);
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x00031F0F File Offset: 0x0003010F
		public void ProcessMoveEvent(Point point)
		{
			this.ProcessMoveEventCore(-1, point);
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x00031F19 File Offset: 0x00030119
		public void ProcessMoveEvent(int pointerId, Point point)
		{
			this.ProcessMoveEventCore(pointerId, point);
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x00031F23 File Offset: 0x00030123
		public void ProcessMouseWheelEvent(double delta, bool isShiftKeyDown, bool isControlKeyDown)
		{
			this.ProcessMouseWheelEventCore(0.0, delta, isShiftKeyDown, isControlKeyDown);
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x00031F37 File Offset: 0x00030137
		public void ProcessMouseWheelEvent(double deltaX, double deltaY, bool isShiftKeyDown, bool isControlKeyDown)
		{
			this.ProcessMouseWheelEventCore(deltaX, deltaY, isShiftKeyDown, isControlKeyDown);
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x00031F44 File Offset: 0x00030144
		private void ProcessDownEventCore(int pointerId, Point point, MouseButton button)
		{
			bool flag = pointerId == -1;
			ConcurrentDictionary<int, PointerTrackerCore.PointerContext> concurrentDictionary = this.pointerContexts;
			lock (concurrentDictionary)
			{
				if (!flag)
				{
					this.TryRemoveTouchPointer();
				}
				PointerTrackerCore.PointerContext pointerContext;
				if (!this.pointerContexts.TryGetValue(pointerId, out pointerContext))
				{
					pointerContext = new PointerTrackerCore.PointerContext();
					this.pointerContexts[pointerId] = pointerContext;
				}
				this.lastPointerId = pointerId;
				this.lastPoint = point;
				this.lastClickButton = button;
				pointerContext.Position = point;
				pointerContext[button].State = PointerTrackerCore.PointerState.Pressed;
			}
		}

		// Token: 0x060009D1 RID: 2513 RVA: 0x00031FD8 File Offset: 0x000301D8
		private void ProcessUpEventCore(int pointerId, Point point, MouseButton button)
		{
			bool flag = pointerId == -1;
			ConcurrentDictionary<int, PointerTrackerCore.PointerContext> concurrentDictionary = this.pointerContexts;
			lock (concurrentDictionary)
			{
				if (!flag)
				{
					this.TryRemoveTouchPointer();
				}
				PointerTrackerCore.PointerContext pointerContext;
				if (!this.pointerContexts.TryGetValue(pointerId, out pointerContext))
				{
					pointerContext = new PointerTrackerCore.PointerContext();
					this.pointerContexts[pointerId] = pointerContext;
				}
				pointerContext.Position = point;
				pointerContext[button].State = PointerTrackerCore.PointerState.Normal;
				if (this.lastPointerId == pointerId && this.lastClickButton == button && new Rect(this.lastPoint.X - 20.0, this.lastPoint.Y - 20.0, 40.0, 40.0).Contains(point))
				{
					DateTime utcNow = DateTime.UtcNow;
					if (button == MouseButton.Left)
					{
						if (pointerContext.LastClickTime == null)
						{
							pointerContext.LastClickTime = new DateTime?(utcNow);
						}
						else
						{
							int doubleClickTime = PointerTrackerCore.GetDoubleClickTime();
							if (utcNow.Millisecond - pointerContext.LastClickTime.Value.Millisecond < doubleClickTime)
							{
								pointerContext.LastClickTime = null;
								this.lastPointerId = -2;
								this.lastPoint = default(Point);
							}
						}
					}
					else
					{
						pointerContext.LastClickTime = null;
						this.lastPointerId = -2;
						this.lastPoint = default(Point);
					}
				}
			}
		}

		// Token: 0x060009D2 RID: 2514 RVA: 0x00032170 File Offset: 0x00030370
		private void ProcessMoveEventCore(int pointerId, Point point)
		{
			bool flag = pointerId == -1;
			ConcurrentDictionary<int, PointerTrackerCore.PointerContext> concurrentDictionary = this.pointerContexts;
			lock (concurrentDictionary)
			{
				if (!flag)
				{
					this.TryRemoveTouchPointer();
				}
				PointerTrackerCore.PointerContext pointerContext;
				if (!this.pointerContexts.TryGetValue(pointerId, out pointerContext))
				{
					pointerContext = new PointerTrackerCore.PointerContext();
					this.pointerContexts[pointerId] = pointerContext;
				}
				pointerContext.Position = point;
			}
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x000321E4 File Offset: 0x000303E4
		public void ProcessMouseWheelEventCore(double deltaX, double deltaY, bool isShiftKeyDown, bool isControlKeyDown)
		{
			ConcurrentDictionary<int, PointerTrackerCore.PointerContext> concurrentDictionary = this.pointerContexts;
			lock (concurrentDictionary)
			{
				this.TryRemoveTouchPointer();
			}
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x00032224 File Offset: 0x00030424
		private void TryRemoveTouchPointer()
		{
			if (this.pointerContexts.Count == 0)
			{
				return;
			}
			if (this.pointerContexts.Count == 1 && this.pointerContexts.Keys.First<int>() == -1)
			{
				return;
			}
			KeyValuePair<int, PointerTrackerCore.PointerContext>[] array = this.pointerContexts.Where((KeyValuePair<int, PointerTrackerCore.PointerContext> c) => c.Key != -1).ToArray<KeyValuePair<int, PointerTrackerCore.PointerContext>>();
			for (int i = 0; i < array.Length; i++)
			{
				int num;
				PointerTrackerCore.PointerContext pointerContext;
				array[i].Deconstruct(out num, out pointerContext);
				int num2 = num;
				this.pointerContexts.TryRemove(num2, out pointerContext);
			}
		}

		// Token: 0x060009D5 RID: 2517
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int GetDoubleClickTime();

		// Token: 0x0400047D RID: 1149
		private int lastPointerId = -2;

		// Token: 0x0400047E RID: 1150
		private Point lastPoint;

		// Token: 0x0400047F RID: 1151
		private MouseButton lastClickButton;

		// Token: 0x04000480 RID: 1152
		private PointerTrackerCore.PointerState state;

		// Token: 0x04000481 RID: 1153
		private ConcurrentDictionary<int, PointerTrackerCore.PointerContext> pointerContexts;

		// Token: 0x0200047B RID: 1147
		private class PointerContext
		{
			// Token: 0x06002DD5 RID: 11733 RVA: 0x000E0494 File Offset: 0x000DE694
			public PointerContext()
			{
				this.leftButtonContext.MouseButton = MouseButton.Left;
				this.middleButtonContext.MouseButton = MouseButton.Middle;
				this.rightButtonContext.MouseButton = MouseButton.Right;
				this.xButton1Context.MouseButton = MouseButton.XButton1;
				this.xButton2Context.MouseButton = MouseButton.XButton2;
			}

			// Token: 0x17000CAA RID: 3242
			public ref PointerTrackerCore.ButtonContext this[MouseButton button]
			{
				get
				{
					switch (button)
					{
					case MouseButton.Left:
						return ref this.leftButtonContext;
					case MouseButton.Middle:
						return ref this.middleButtonContext;
					case MouseButton.Right:
						return ref this.rightButtonContext;
					case MouseButton.XButton1:
						return ref this.xButton1Context;
					case MouseButton.XButton2:
						return ref this.xButton2Context;
					default:
						throw new ArgumentException("button");
					}
				}
			}

			// Token: 0x17000CAB RID: 3243
			// (get) Token: 0x06002DD7 RID: 11735 RVA: 0x000E053A File Offset: 0x000DE73A
			// (set) Token: 0x06002DD8 RID: 11736 RVA: 0x000E0542 File Offset: 0x000DE742
			public DateTime? LastClickTime { get; set; }

			// Token: 0x17000CAC RID: 3244
			// (get) Token: 0x06002DD9 RID: 11737 RVA: 0x000E054B File Offset: 0x000DE74B
			// (set) Token: 0x06002DDA RID: 11738 RVA: 0x000E0553 File Offset: 0x000DE753
			public Point Position { get; set; }

			// Token: 0x04001990 RID: 6544
			private PointerTrackerCore.ButtonContext leftButtonContext;

			// Token: 0x04001991 RID: 6545
			private PointerTrackerCore.ButtonContext middleButtonContext;

			// Token: 0x04001992 RID: 6546
			private PointerTrackerCore.ButtonContext rightButtonContext;

			// Token: 0x04001993 RID: 6547
			private PointerTrackerCore.ButtonContext xButton1Context;

			// Token: 0x04001994 RID: 6548
			private PointerTrackerCore.ButtonContext xButton2Context;
		}

		// Token: 0x0200047C RID: 1148
		private struct ButtonContext
		{
			// Token: 0x17000CAD RID: 3245
			// (get) Token: 0x06002DDB RID: 11739 RVA: 0x000E055C File Offset: 0x000DE75C
			// (set) Token: 0x06002DDC RID: 11740 RVA: 0x000E0564 File Offset: 0x000DE764
			public MouseButton MouseButton { get; set; }

			// Token: 0x17000CAE RID: 3246
			// (get) Token: 0x06002DDD RID: 11741 RVA: 0x000E056D File Offset: 0x000DE76D
			// (set) Token: 0x06002DDE RID: 11742 RVA: 0x000E0575 File Offset: 0x000DE775
			public PointerTrackerCore.PointerState State { get; set; }
		}

		// Token: 0x0200047D RID: 1149
		private enum PointerState
		{
			// Token: 0x0400199A RID: 6554
			Normal,
			// Token: 0x0400199B RID: 6555
			PointerOver,
			// Token: 0x0400199C RID: 6556
			Pressed
		}
	}
}
