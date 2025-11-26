using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000217 RID: 535
	public class SelectedAdorner : Adorner
	{
		// Token: 0x06001DA5 RID: 7589 RVA: 0x0007FD6C File Offset: 0x0007DF6C
		public SelectedAdorner(UIElement adornedElement)
			: base(adornedElement)
		{
			this._pen = new Pen(Brushes.Black, 1.0)
			{
				DashStyle = new DashStyle(new double[] { 2.5, 2.5 }, 0.0)
			};
		}

		// Token: 0x06001DA6 RID: 7590 RVA: 0x0007FDD0 File Offset: 0x0007DFD0
		protected override void OnRender(DrawingContext drawingContext)
		{
			Rect adornedElementRect = this.GetAdornedElementRect();
			drawingContext.DrawLine(this._pen, new Point(adornedElementRect.TopLeft.X - 3.0, adornedElementRect.TopLeft.Y - 3.0), new Point(adornedElementRect.TopRight.X + 3.0, adornedElementRect.TopRight.Y - 3.0));
			drawingContext.DrawLine(this._pen, new Point(adornedElementRect.TopRight.X + 3.0, adornedElementRect.TopRight.Y - 3.0), new Point(adornedElementRect.BottomRight.X + 3.0, adornedElementRect.BottomRight.Y + 3.0));
			drawingContext.DrawLine(this._pen, new Point(adornedElementRect.BottomRight.X + 3.0, adornedElementRect.BottomRight.Y + 3.0), new Point(adornedElementRect.BottomLeft.X - 3.0, adornedElementRect.BottomLeft.Y + 3.0));
			drawingContext.DrawLine(this._pen, new Point(adornedElementRect.BottomLeft.X - 3.0, adornedElementRect.BottomLeft.Y + 3.0), new Point(adornedElementRect.TopLeft.X - 3.0, adornedElementRect.TopLeft.Y - 3.0));
			base.OnRender(drawingContext);
		}

		// Token: 0x06001DA7 RID: 7591 RVA: 0x0007FFD4 File Offset: 0x0007E1D4
		private Rect GetAdornedElementRect()
		{
			if (!(base.AdornedElement is Polyline))
			{
				return new Rect(base.AdornedElement.RenderSize);
			}
			Polyline polyline = (Polyline)base.AdornedElement;
			double num = polyline.Points.Min((Point p) => p.X);
			double num2 = polyline.Points.Max((Point p) => p.X);
			double num3 = polyline.Points.Min((Point p) => p.Y);
			double num4 = polyline.Points.Max((Point p) => p.Y);
			double num5 = num2 - num + polyline.StrokeThickness / 2.0;
			double num6 = num4 - num3 + polyline.StrokeThickness / 2.0;
			return new Rect(num, num3, num5, num6);
		}

		// Token: 0x04000B35 RID: 2869
		private Pen _pen;
	}
}
