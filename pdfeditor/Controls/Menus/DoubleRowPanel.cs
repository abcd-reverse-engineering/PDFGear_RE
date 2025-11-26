using System;
using System.Windows;
using System.Windows.Controls;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x0200026A RID: 618
	public class DoubleRowPanel : Panel
	{
		// Token: 0x17000B6E RID: 2926
		// (get) Token: 0x060023F8 RID: 9208 RVA: 0x000A7F5A File Offset: 0x000A615A
		// (set) Token: 0x060023F9 RID: 9209 RVA: 0x000A7F6C File Offset: 0x000A616C
		public double RowSpace
		{
			get
			{
				return (double)base.GetValue(DoubleRowPanel.RowSpaceProperty);
			}
			set
			{
				base.SetValue(DoubleRowPanel.RowSpaceProperty, value);
			}
		}

		// Token: 0x17000B6F RID: 2927
		// (get) Token: 0x060023FA RID: 9210 RVA: 0x000A7F7F File Offset: 0x000A617F
		// (set) Token: 0x060023FB RID: 9211 RVA: 0x000A7F91 File Offset: 0x000A6191
		public double ColumnSpace
		{
			get
			{
				return (double)base.GetValue(DoubleRowPanel.ColumnSpaceProperty);
			}
			set
			{
				base.SetValue(DoubleRowPanel.ColumnSpaceProperty, value);
			}
		}

		// Token: 0x17000B70 RID: 2928
		// (get) Token: 0x060023FC RID: 9212 RVA: 0x000A7FA4 File Offset: 0x000A61A4
		// (set) Token: 0x060023FD RID: 9213 RVA: 0x000A7FB6 File Offset: 0x000A61B6
		public Thickness Padding
		{
			get
			{
				return (Thickness)base.GetValue(DoubleRowPanel.PaddingProperty);
			}
			set
			{
				base.SetValue(DoubleRowPanel.PaddingProperty, value);
			}
		}

		// Token: 0x060023FE RID: 9214 RVA: 0x000A7FCC File Offset: 0x000A61CC
		protected override Size MeasureOverride(Size availableSize)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			Thickness padding = this.Padding;
			double columnSpace = this.ColumnSpace;
			double rowSpace = this.RowSpace;
			Size size = availableSize;
			if (!double.IsInfinity(availableSize.Height))
			{
				double num5 = (availableSize.Height - rowSpace - padding.Top - padding.Bottom) / 2.0;
				size = new Size(Math.Max(0.0, availableSize.Width - this.Padding.Left - this.Padding.Right), Math.Max(0.0, num5));
			}
			int num6 = (int)Math.Ceiling((double)base.InternalChildren.Count / 2.0);
			for (int i = 0; i < base.InternalChildren.Count; i++)
			{
				UIElement uielement = base.InternalChildren[i];
				uielement.Measure(size);
				if (i < num6)
				{
					num += uielement.DesiredSize.Width + columnSpace;
					num2 = Math.Max(uielement.DesiredSize.Height, num2);
				}
				else
				{
					num3 += uielement.DesiredSize.Width + columnSpace;
					num4 = Math.Max(uielement.DesiredSize.Height, num4);
				}
			}
			double num7 = (double.IsNaN(base.Width) ? Math.Max(num, num3) : base.Width);
			double num8 = (double.IsNaN(base.Height) ? (num2 + num4 + padding.Top + padding.Bottom + rowSpace) : base.Height);
			return new Size(num7, num8);
		}

		// Token: 0x060023FF RID: 9215 RVA: 0x000A81A4 File Offset: 0x000A63A4
		protected override Size ArrangeOverride(Size finalSize)
		{
			Thickness padding = this.Padding;
			double columnSpace = this.ColumnSpace;
			double rowSpace = this.RowSpace;
			double num = padding.Left;
			double num2 = padding.Top;
			double num3 = (finalSize.Height - rowSpace - padding.Top - padding.Bottom) / 2.0;
			int num4 = (int)Math.Ceiling((double)base.InternalChildren.Count / 2.0);
			double[] array = new double[num4];
			for (int i = 0; i < num4; i++)
			{
				if (i + num4 < base.InternalChildren.Count)
				{
					array[i] = Math.Max(base.InternalChildren[i].DesiredSize.Width, base.InternalChildren[i + num4].DesiredSize.Width);
				}
				else
				{
					array[i] = base.InternalChildren[i].DesiredSize.Width;
				}
			}
			for (int j = 0; j < base.InternalChildren.Count; j++)
			{
				UIElement uielement = base.InternalChildren[j];
				if (j == num4)
				{
					num = padding.Left;
					num2 = padding.Top + num3 + rowSpace;
				}
				if (j < num4)
				{
					uielement.Arrange(new Rect(num, num2, array[j], num3));
					num += array[j] + columnSpace;
				}
				else
				{
					uielement.Arrange(new Rect(num, num2, array[j - num4], num3));
					num += array[j - num4] + columnSpace;
				}
			}
			return finalSize;
		}

		// Token: 0x04000F38 RID: 3896
		public static readonly DependencyProperty RowSpaceProperty = DependencyProperty.Register("RowSpace", typeof(double), typeof(DoubleRowPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

		// Token: 0x04000F39 RID: 3897
		public static readonly DependencyProperty ColumnSpaceProperty = DependencyProperty.Register("ColumnSpace", typeof(double), typeof(DoubleRowPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

		// Token: 0x04000F3A RID: 3898
		public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(DoubleRowPanel), new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
	}
}
