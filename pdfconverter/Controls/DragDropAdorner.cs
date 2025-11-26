using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace pdfconverter.Controls
{
	// Token: 0x020000A4 RID: 164
	public class DragDropAdorner : Adorner
	{
		// Token: 0x0600072B RID: 1835 RVA: 0x0001A2D9 File Offset: 0x000184D9
		public DragDropAdorner(UIElement parent)
			: base(parent)
		{
			base.IsHitTestVisible = false;
			this.mDraggedElement = parent as FrameworkElement;
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x0001A2F8 File Offset: 0x000184F8
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			if (this.mDraggedElement != null)
			{
				Win32.POINT point = default(Win32.POINT);
				if (Win32.GetCursorPos(ref point))
				{
					Point point2 = base.PointFromScreen(new Point((double)point.X, (double)point.Y));
					this.mDraggedElement.PointToScreen(default(Point));
					Rect rect = new Rect(point2.X, point2.Y - this.mDraggedElement.ActualHeight / 2.0, this.mDraggedElement.ActualWidth, this.mDraggedElement.ActualHeight);
					drawingContext.PushOpacity(0.5);
					Brush brush = this.mDraggedElement.TryFindResource(SystemColors.HighlightBrushKey) as Brush;
					if (brush != null)
					{
						drawingContext.DrawRectangle(brush, new Pen(Brushes.Red, 0.0), rect);
					}
					drawingContext.DrawRectangle(new VisualBrush(this.mDraggedElement), new Pen(Brushes.Transparent, 0.0), rect);
					drawingContext.Pop();
				}
			}
		}

		// Token: 0x04000385 RID: 901
		private FrameworkElement mDraggedElement;
	}
}
