using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x0200020E RID: 526
	public partial class KSIcon : Control
	{
		// Token: 0x06001D33 RID: 7475 RVA: 0x0007E2D0 File Offset: 0x0007C4D0
		static KSIcon()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(KSIcon), new FrameworkPropertyMetadata(typeof(KSIcon)));
		}

		// Token: 0x17000A4F RID: 2639
		// (get) Token: 0x06001D34 RID: 7476 RVA: 0x0007E3BF File Offset: 0x0007C5BF
		// (set) Token: 0x06001D35 RID: 7477 RVA: 0x0007E3D1 File Offset: 0x0007C5D1
		public Brush IconBrush
		{
			get
			{
				return (Brush)base.GetValue(KSIcon.IconBrushProperty);
			}
			set
			{
				base.SetValue(KSIcon.IconBrushProperty, value);
			}
		}

		// Token: 0x17000A50 RID: 2640
		// (get) Token: 0x06001D36 RID: 7478 RVA: 0x0007E3DF File Offset: 0x0007C5DF
		// (set) Token: 0x06001D37 RID: 7479 RVA: 0x0007E3F1 File Offset: 0x0007C5F1
		public Geometry IconGeometry
		{
			get
			{
				return (Geometry)base.GetValue(KSIcon.IconGeometryProperty);
			}
			set
			{
				base.SetValue(KSIcon.IconGeometryProperty, value);
			}
		}

		// Token: 0x17000A51 RID: 2641
		// (get) Token: 0x06001D38 RID: 7480 RVA: 0x0007E3FF File Offset: 0x0007C5FF
		// (set) Token: 0x06001D39 RID: 7481 RVA: 0x0007E411 File Offset: 0x0007C611
		public Pen IconPen
		{
			get
			{
				return (Pen)base.GetValue(KSIcon.IconPenProperty);
			}
			set
			{
				base.SetValue(KSIcon.IconPenProperty, value);
			}
		}

		// Token: 0x17000A52 RID: 2642
		// (get) Token: 0x06001D3A RID: 7482 RVA: 0x0007E41F File Offset: 0x0007C61F
		// (set) Token: 0x06001D3B RID: 7483 RVA: 0x0007E431 File Offset: 0x0007C631
		public CornerRadius CornerRadius
		{
			get
			{
				return (CornerRadius)base.GetValue(KSIcon.CornerRadiusProperty);
			}
			set
			{
				base.SetValue(KSIcon.CornerRadiusProperty, value);
			}
		}

		// Token: 0x04000AFC RID: 2812
		public static readonly DependencyProperty IconBrushProperty = DependencyProperty.Register("IconBrush", typeof(Brush), typeof(KSIcon), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

		// Token: 0x04000AFD RID: 2813
		public static readonly DependencyProperty IconGeometryProperty = DependencyProperty.Register("IconGeometry", typeof(Geometry), typeof(KSIcon), new PropertyMetadata(null));

		// Token: 0x04000AFE RID: 2814
		public static readonly DependencyProperty IconPenProperty = DependencyProperty.Register("IconPen", typeof(Pen), typeof(KSIcon), new PropertyMetadata(null));

		// Token: 0x04000AFF RID: 2815
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(KSIcon), new PropertyMetadata(new CornerRadius(0.0)));
	}
}
