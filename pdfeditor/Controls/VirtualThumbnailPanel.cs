using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WpfToolkit.Controls;

namespace pdfeditor.Controls
{
	// Token: 0x020001DC RID: 476
	public class VirtualThumbnailPanel : VirtualizingPanelBase, IScrollInfo
	{
		// Token: 0x170009FC RID: 2556
		// (get) Token: 0x06001AD5 RID: 6869 RVA: 0x0006B8B9 File Offset: 0x00069AB9
		// (set) Token: 0x06001AD6 RID: 6870 RVA: 0x0006B8CB File Offset: 0x00069ACB
		public SpacingMode SpacingMode
		{
			get
			{
				return (SpacingMode)base.GetValue(VirtualThumbnailPanel.SpacingModeProperty);
			}
			set
			{
				base.SetValue(VirtualThumbnailPanel.SpacingModeProperty, value);
			}
		}

		// Token: 0x170009FD RID: 2557
		// (get) Token: 0x06001AD7 RID: 6871 RVA: 0x0006B8DE File Offset: 0x00069ADE
		// (set) Token: 0x06001AD8 RID: 6872 RVA: 0x0006B8F0 File Offset: 0x00069AF0
		public Orientation Orientation
		{
			get
			{
				return (Orientation)base.GetValue(VirtualThumbnailPanel.OrientationProperty);
			}
			set
			{
				base.SetValue(VirtualThumbnailPanel.OrientationProperty, value);
			}
		}

		// Token: 0x170009FE RID: 2558
		// (get) Token: 0x06001AD9 RID: 6873 RVA: 0x0006B903 File Offset: 0x00069B03
		// (set) Token: 0x06001ADA RID: 6874 RVA: 0x0006B915 File Offset: 0x00069B15
		public Size ItemSize
		{
			get
			{
				return (Size)base.GetValue(VirtualThumbnailPanel.ItemSizeProperty);
			}
			set
			{
				base.SetValue(VirtualThumbnailPanel.ItemSizeProperty, value);
			}
		}

		// Token: 0x06001ADB RID: 6875 RVA: 0x0006B928 File Offset: 0x00069B28
		private void Orientation_Changed()
		{
			base.MouseWheelScrollDirection = ((this.Orientation == Orientation.Vertical) ? ScrollDirection.Vertical : ScrollDirection.Horizontal);
		}

		// Token: 0x06001ADC RID: 6876 RVA: 0x0006B93D File Offset: 0x00069B3D
		protected override Size MeasureOverride(Size availableSize)
		{
			this.UpdateChildSize(availableSize);
			return base.MeasureOverride(availableSize);
		}

		// Token: 0x06001ADD RID: 6877 RVA: 0x0006B950 File Offset: 0x00069B50
		public new virtual Rect MakeVisible(Visual visual, Rect rectangle)
		{
			Point point = visual.TransformToAncestor(this).Transform(base.Offset);
			double num = Math.Min(rectangle.Width, base.Viewport.Width);
			double num2 = Math.Min(rectangle.Height, base.Viewport.Height);
			double num3 = point.X - base.Offset.X;
			if (point.X + rectangle.Width >= base.Offset.X && point.X <= base.Offset.X + base.Viewport.Width && point.Y + rectangle.Height >= base.Offset.Y && point.Y <= base.Offset.Y + base.Viewport.Height)
			{
				double num4 = point.Y - base.Offset.Y;
				return new Rect(num3, num4, num, num2);
			}
			double num5 = 0.0;
			if (point.Y < base.Offset.Y)
			{
				num5 = -(base.Offset.Y - point.Y);
			}
			else if (point.Y + rectangle.Height > base.Offset.Y + base.Viewport.Height)
			{
				num5 = point.Y + rectangle.Height - (base.Offset.Y + base.Viewport.Height);
			}
			base.SetVerticalOffset(base.Offset.Y + num5);
			return new Rect(num3, num5, num, num2);
		}

		// Token: 0x06001ADE RID: 6878 RVA: 0x0006BB3C File Offset: 0x00069D3C
		private void UpdateChildSize(Size availableSize)
		{
			if (this.ItemSize != Size.Empty)
			{
				this.childSize = this.ItemSize;
			}
			else if (base.InternalChildren.Count != 0)
			{
				this.childSize = base.InternalChildren[0].DesiredSize;
			}
			else
			{
				this.childSize = this.CalculateChildSize(availableSize);
			}
			if (double.IsInfinity(this.GetWidth(availableSize)))
			{
				this.itemsPerRowCount = base.Items.Count;
			}
			else
			{
				this.itemsPerRowCount = Math.Min(base.Items.Count, Math.Max(1, (int)Math.Floor(this.GetWidth(availableSize) / this.GetWidth(this.childSize))));
			}
			this.rowCount = (int)Math.Ceiling((double)base.Items.Count / (double)this.itemsPerRowCount);
		}

		// Token: 0x06001ADF RID: 6879 RVA: 0x0006BC14 File Offset: 0x00069E14
		private Size CalculateChildSize(Size availableSize)
		{
			if (base.Items.Count == 0)
			{
				return new Size(0.0, 0.0);
			}
			GeneratorPosition generatorPosition = base.ItemContainerGenerator.GeneratorPositionFromIndex(0);
			Size desiredSize;
			using (base.ItemContainerGenerator.StartAt(generatorPosition, GeneratorDirection.Forward, true))
			{
				UIElement uielement = (UIElement)base.ItemContainerGenerator.GenerateNext();
				base.AddInternalChild(uielement);
				base.ItemContainerGenerator.PrepareItemContainer(uielement);
				uielement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				desiredSize = uielement.DesiredSize;
			}
			return desiredSize;
		}

		// Token: 0x06001AE0 RID: 6880 RVA: 0x0006BCC8 File Offset: 0x00069EC8
		protected override Size CalculateExtent(Size availableSize)
		{
			double num;
			if (this.itemsPerRowCount != 1)
			{
				num = ((this.SpacingMode != SpacingMode.None && !double.IsInfinity(this.GetWidth(availableSize))) ? this.GetWidth(availableSize) : (this.GetWidth(this.childSize) * (double)this.itemsPerRowCount));
			}
			else
			{
				num = Math.Max(this.GetWidth(this.childSize), this.GetWidth(availableSize));
			}
			double num2 = this.GetHeight(this.childSize) * (double)this.rowCount;
			return this.CreateSize(num, num2);
		}

		// Token: 0x06001AE1 RID: 6881 RVA: 0x0006BD54 File Offset: 0x00069F54
		protected void CalculateSpacing(Size finalSize, out double innerSpacing, out double outerSpacing)
		{
			Size size = this.CalculateChildArrangeSize(finalSize);
			double width = this.GetWidth(finalSize);
			double num = Math.Min(this.GetWidth(size) * (double)this.itemsPerRowCount, width);
			double num2 = width - num;
			switch (this.SpacingMode)
			{
			case SpacingMode.Uniform:
				if (this.itemsPerRowCount == 1)
				{
					innerSpacing = 0.0;
					outerSpacing = num2 / 2.0;
					return;
				}
				innerSpacing = (outerSpacing = num2 / (double)(this.itemsPerRowCount + 1));
				return;
			case SpacingMode.BetweenItemsOnly:
				innerSpacing = num2 / (double)Math.Max(this.itemsPerRowCount - 1, 1);
				outerSpacing = 0.0;
				return;
			case SpacingMode.StartAndEndOnly:
				innerSpacing = 0.0;
				outerSpacing = num2 / 2.0;
				return;
			}
			innerSpacing = 0.0;
			outerSpacing = 0.0;
		}

		// Token: 0x06001AE2 RID: 6882 RVA: 0x0006BE34 File Offset: 0x0006A034
		protected override Size ArrangeOverride(Size finalSize)
		{
			double x = this.GetX(base.Offset);
			double y = this.GetY(base.Offset);
			Size size = this.CalculateChildArrangeSize(finalSize);
			double num;
			double num2;
			this.CalculateSpacing(finalSize, out num, out num2);
			for (int i = 0; i < base.InternalChildren.Count; i++)
			{
				UIElement uielement = base.InternalChildren[i];
				int itemIndexFromChildIndex = base.GetItemIndexFromChildIndex(i);
				int num3 = itemIndexFromChildIndex % this.itemsPerRowCount;
				double num4 = (double)(itemIndexFromChildIndex / this.itemsPerRowCount);
				double num5 = num2 + (double)num3 * (this.GetWidth(size) + num);
				double num6 = num4 * this.GetHeight(size);
				if (this.GetHeight(finalSize) == 0.0)
				{
					uielement.Arrange(new Rect(0.0, 0.0, 0.0, 0.0));
				}
				else
				{
					uielement.Arrange(this.CreateRect(num5 - x, num6 - y, size.Width, size.Height));
				}
			}
			return finalSize;
		}

		// Token: 0x06001AE3 RID: 6883 RVA: 0x0006BF3B File Offset: 0x0006A13B
		protected Size CalculateChildArrangeSize(Size finalSize)
		{
			return this.childSize;
		}

		// Token: 0x06001AE4 RID: 6884 RVA: 0x0006BF44 File Offset: 0x0006A144
		protected override ItemRange UpdateItemRange()
		{
			if (!base.IsVirtualizing)
			{
				return new ItemRange(0, base.Items.Count - 1);
			}
			double num = this.GetY(base.Offset);
			double num2 = this.GetY(base.Offset) + this.GetHeight(base.Viewport);
			if (base.CacheLengthUnit == VirtualizationCacheLengthUnit.Pixel)
			{
				num = Math.Max(num - base.CacheLength.CacheBeforeViewport, 0.0);
				num2 = Math.Min(num2 + base.CacheLength.CacheAfterViewport, this.GetHeight(base.Extent));
			}
			int num3 = this.GetRowIndex(num) * this.itemsPerRowCount;
			int num4 = Math.Min(this.GetRowIndex(num2) * this.itemsPerRowCount + (this.itemsPerRowCount - 1), base.Items.Count - 1);
			if (base.CacheLengthUnit == VirtualizationCacheLengthUnit.Page)
			{
				int num5 = num4 - num3 + 1;
				num3 = Math.Max(num3 - (int)base.CacheLength.CacheBeforeViewport * num5, 0);
				num4 = Math.Min(num4 + (int)base.CacheLength.CacheAfterViewport * num5, base.Items.Count - 1);
			}
			else if (base.CacheLengthUnit == VirtualizationCacheLengthUnit.Item)
			{
				num3 = Math.Max(num3 - (int)base.CacheLength.CacheBeforeViewport, 0);
				num4 = Math.Min(num4 + (int)base.CacheLength.CacheAfterViewport, base.Items.Count - 1);
			}
			return new ItemRange(num3, num4);
		}

		// Token: 0x06001AE5 RID: 6885 RVA: 0x0006C0BC File Offset: 0x0006A2BC
		private int GetRowIndex(double location)
		{
			int num = (int)Math.Floor(location / this.GetHeight(this.childSize));
			int num2 = (int)Math.Ceiling((double)base.Items.Count / (double)this.itemsPerRowCount);
			return Math.Max(Math.Min(num, num2), 0);
		}

		// Token: 0x06001AE6 RID: 6886 RVA: 0x0006C104 File Offset: 0x0006A304
		protected override void BringIndexIntoView(int index)
		{
			if (index < 0 || index >= base.Items.Count || this.itemsPerRowCount == 0)
			{
				return;
			}
			double num = (double)(index / this.itemsPerRowCount);
			double height = this.GetHeight(this.childSize);
			double num2 = num * height;
			double num3 = num2 + height;
			double y = this.GetY(base.Offset);
			double height2 = this.GetHeight(base.Viewport);
			double num4 = y + height2;
			bool flag = num2 >= y && num3 <= num4;
			bool flag2 = num2 <= y && num3 >= num4;
			if (flag || flag2)
			{
				return;
			}
			double num5;
			if (num2 < y)
			{
				num5 = num2;
			}
			else if (num3 > num4)
			{
				if (height > height2)
				{
					num5 = num2;
				}
				else
				{
					num5 = num3 - height2;
				}
			}
			else
			{
				num5 = num2;
			}
			if (this.Orientation == Orientation.Horizontal)
			{
				base.SetHorizontalOffset(num5);
				return;
			}
			base.SetVerticalOffset(num5);
		}

		// Token: 0x06001AE7 RID: 6887 RVA: 0x0006C1CC File Offset: 0x0006A3CC
		protected override double GetLineUpScrollAmount()
		{
			return -Math.Min(this.childSize.Height * base.ScrollLineDeltaItem, base.Viewport.Height);
		}

		// Token: 0x06001AE8 RID: 6888 RVA: 0x0006C200 File Offset: 0x0006A400
		protected override double GetLineDownScrollAmount()
		{
			return Math.Min(this.childSize.Height * base.ScrollLineDeltaItem, base.Viewport.Height);
		}

		// Token: 0x06001AE9 RID: 6889 RVA: 0x0006C234 File Offset: 0x0006A434
		protected override double GetLineLeftScrollAmount()
		{
			return -Math.Min(this.childSize.Width * base.ScrollLineDeltaItem, base.Viewport.Width);
		}

		// Token: 0x06001AEA RID: 6890 RVA: 0x0006C268 File Offset: 0x0006A468
		protected override double GetLineRightScrollAmount()
		{
			return Math.Min(this.childSize.Width * base.ScrollLineDeltaItem, base.Viewport.Width);
		}

		// Token: 0x06001AEB RID: 6891 RVA: 0x0006C29C File Offset: 0x0006A49C
		protected override double GetMouseWheelUpScrollAmount()
		{
			return -Math.Min(this.childSize.Height * (double)base.MouseWheelDeltaItem, base.Viewport.Height);
		}

		// Token: 0x06001AEC RID: 6892 RVA: 0x0006C2D0 File Offset: 0x0006A4D0
		protected override double GetMouseWheelDownScrollAmount()
		{
			return Math.Min(this.childSize.Height * (double)base.MouseWheelDeltaItem, base.Viewport.Height);
		}

		// Token: 0x06001AED RID: 6893 RVA: 0x0006C304 File Offset: 0x0006A504
		protected override double GetMouseWheelLeftScrollAmount()
		{
			return -Math.Min(this.childSize.Width * (double)base.MouseWheelDeltaItem, base.Viewport.Width);
		}

		// Token: 0x06001AEE RID: 6894 RVA: 0x0006C338 File Offset: 0x0006A538
		protected override double GetMouseWheelRightScrollAmount()
		{
			return Math.Min(this.childSize.Width * (double)base.MouseWheelDeltaItem, base.Viewport.Width);
		}

		// Token: 0x06001AEF RID: 6895 RVA: 0x0006C36C File Offset: 0x0006A56C
		protected override double GetPageUpScrollAmount()
		{
			return -base.Viewport.Height;
		}

		// Token: 0x06001AF0 RID: 6896 RVA: 0x0006C388 File Offset: 0x0006A588
		protected override double GetPageDownScrollAmount()
		{
			return base.Viewport.Height;
		}

		// Token: 0x06001AF1 RID: 6897 RVA: 0x0006C3A4 File Offset: 0x0006A5A4
		protected override double GetPageLeftScrollAmount()
		{
			return -base.Viewport.Width;
		}

		// Token: 0x06001AF2 RID: 6898 RVA: 0x0006C3C0 File Offset: 0x0006A5C0
		protected override double GetPageRightScrollAmount()
		{
			return base.Viewport.Width;
		}

		// Token: 0x06001AF3 RID: 6899 RVA: 0x0006C3DB File Offset: 0x0006A5DB
		protected double GetX(Point point)
		{
			if (this.Orientation != Orientation.Vertical)
			{
				return point.Y;
			}
			return point.X;
		}

		// Token: 0x06001AF4 RID: 6900 RVA: 0x0006C3F5 File Offset: 0x0006A5F5
		protected double GetY(Point point)
		{
			if (this.Orientation != Orientation.Vertical)
			{
				return point.X;
			}
			return point.Y;
		}

		// Token: 0x06001AF5 RID: 6901 RVA: 0x0006C40F File Offset: 0x0006A60F
		protected double GetWidth(Size size)
		{
			if (this.Orientation != Orientation.Vertical)
			{
				return size.Height;
			}
			return size.Width;
		}

		// Token: 0x06001AF6 RID: 6902 RVA: 0x0006C429 File Offset: 0x0006A629
		protected double GetHeight(Size size)
		{
			if (this.Orientation != Orientation.Vertical)
			{
				return size.Width;
			}
			return size.Height;
		}

		// Token: 0x06001AF7 RID: 6903 RVA: 0x0006C443 File Offset: 0x0006A643
		protected Size CreateSize(double width, double height)
		{
			if (this.Orientation != Orientation.Vertical)
			{
				return new Size(height, width);
			}
			return new Size(width, height);
		}

		// Token: 0x06001AF8 RID: 6904 RVA: 0x0006C45D File Offset: 0x0006A65D
		protected Rect CreateRect(double x, double y, double width, double height)
		{
			if (this.Orientation != Orientation.Vertical)
			{
				return new Rect(y, x, width, height);
			}
			return new Rect(x, y, width, height);
		}

		// Token: 0x04000958 RID: 2392
		public static readonly DependencyProperty SpacingModeProperty = DependencyProperty.Register("SpacingMode", typeof(SpacingMode), typeof(VirtualThumbnailPanel), new FrameworkPropertyMetadata(SpacingMode.Uniform, FrameworkPropertyMetadataOptions.AffectsMeasure));

		// Token: 0x04000959 RID: 2393
		public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(VirtualThumbnailPanel), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure, delegate(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((VirtualThumbnailPanel)obj).Orientation_Changed();
		}));

		// Token: 0x0400095A RID: 2394
		public static readonly DependencyProperty ItemSizeProperty = DependencyProperty.Register("ItemSize", typeof(Size), typeof(VirtualThumbnailPanel), new FrameworkPropertyMetadata(Size.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure));

		// Token: 0x0400095B RID: 2395
		protected Size childSize;

		// Token: 0x0400095C RID: 2396
		protected int rowCount;

		// Token: 0x0400095D RID: 2397
		protected int itemsPerRowCount;
	}
}
