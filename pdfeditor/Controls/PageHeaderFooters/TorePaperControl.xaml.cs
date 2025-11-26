using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace pdfeditor.Controls.PageHeaderFooters
{
	// Token: 0x02000244 RID: 580
	public partial class TorePaperControl : UserControl
	{
		// Token: 0x06002124 RID: 8484 RVA: 0x000981EF File Offset: 0x000963EF
		public TorePaperControl()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000AFD RID: 2813
		// (get) Token: 0x06002125 RID: 8485 RVA: 0x000981FD File Offset: 0x000963FD
		// (set) Token: 0x06002126 RID: 8486 RVA: 0x0009820F File Offset: 0x0009640F
		public ToreEdge ToreEdge
		{
			get
			{
				return (ToreEdge)base.GetValue(TorePaperControl.ToreEdgeProperty);
			}
			set
			{
				base.SetValue(TorePaperControl.ToreEdgeProperty, value);
			}
		}

		// Token: 0x06002127 RID: 8487 RVA: 0x00098224 File Offset: 0x00096424
		private static void OnToreEdgePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TorePaperControl torePaperControl = d as TorePaperControl;
			if (torePaperControl != null)
			{
				torePaperControl.UpdateContent();
			}
		}

		// Token: 0x17000AFE RID: 2814
		// (get) Token: 0x06002128 RID: 8488 RVA: 0x00098241 File Offset: 0x00096441
		// (set) Token: 0x06002129 RID: 8489 RVA: 0x00098253 File Offset: 0x00096453
		public Brush ContentBrush
		{
			get
			{
				return (Brush)base.GetValue(TorePaperControl.ContentBrushProperty);
			}
			set
			{
				base.SetValue(TorePaperControl.ContentBrushProperty, value);
			}
		}

		// Token: 0x0600212A RID: 8490 RVA: 0x00098264 File Offset: 0x00096464
		private void UpdateContent()
		{
			Geometry geometry = this.BuildContentGeometry();
			this.ContentPath.Data = geometry;
		}

		// Token: 0x0600212B RID: 8491 RVA: 0x00098284 File Offset: 0x00096484
		private Geometry BuildContentGeometry()
		{
			ToreEdge toreEdge = this.ToreEdge;
			double actualWidth = base.ActualWidth;
			double actualHeight = base.ActualHeight;
			if (actualHeight <= 10.0)
			{
				return new RectangleGeometry
				{
					Rect = new Rect(0.0, 0.0, actualWidth, actualHeight)
				};
			}
			int num = (int)Math.Ceiling(actualWidth / 35.0);
			PathFigure pathFigure = new PathFigure();
			if (toreEdge == ToreEdge.Bottom)
			{
				pathFigure.StartPoint = new Point(0.0, actualHeight);
			}
			else
			{
				pathFigure.StartPoint = new Point(0.0, 10.0);
			}
			for (int i = 0; i < num; i++)
			{
				double num2 = 35.0 * (double)(i + 1);
				double num3 = ((i % 2 == 0) ? 0.0 : 10.0);
				if (num2 > actualWidth)
				{
					double num4 = 35.0 - (num2 - actualWidth);
					double num5 = 0.2857142857142857;
					num3 = ((i % 2 == 0) ? (10.0 - num5 * num4) : (num5 * num4));
					num2 = actualWidth;
				}
				if (toreEdge == ToreEdge.Bottom)
				{
					num3 += actualHeight - 10.0;
				}
				pathFigure.Segments.Add(new LineSegment(new Point(num2, num3), true));
			}
			if (toreEdge == ToreEdge.Bottom)
			{
				pathFigure.Segments.Add(new LineSegment(new Point(actualWidth, 0.0), true));
				pathFigure.Segments.Add(new LineSegment(new Point(0.0, 0.0), true));
			}
			else
			{
				pathFigure.Segments.Add(new LineSegment(new Point(actualWidth, actualHeight), true));
				pathFigure.Segments.Add(new LineSegment(new Point(0.0, actualHeight), true));
			}
			pathFigure.IsClosed = true;
			return new PathGeometry
			{
				Figures = { pathFigure }
			};
		}

		// Token: 0x0600212C RID: 8492 RVA: 0x0009847E File Offset: 0x0009667E
		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateContent();
		}

		// Token: 0x04000D7F RID: 3455
		private const double ToreHeight = 10.0;

		// Token: 0x04000D80 RID: 3456
		private const double HalfToreWidth = 35.0;

		// Token: 0x04000D81 RID: 3457
		public static readonly DependencyProperty ToreEdgeProperty = DependencyProperty.Register("ToreEdge", typeof(ToreEdge), typeof(TorePaperControl), new PropertyMetadata(ToreEdge.Bottom, new PropertyChangedCallback(TorePaperControl.OnToreEdgePropertyChanged)));

		// Token: 0x04000D82 RID: 3458
		public static readonly DependencyProperty ContentBrushProperty = DependencyProperty.Register("ContentBrush", typeof(Brush), typeof(TorePaperControl), new PropertyMetadata(null));
	}
}
