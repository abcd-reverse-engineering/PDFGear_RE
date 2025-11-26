using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace pdfconverter.Controls
{
	// Token: 0x0200009A RID: 154
	public class ButtonEx : Button
	{
		// Token: 0x060006B5 RID: 1717 RVA: 0x000180A0 File Offset: 0x000162A0
		static ButtonEx()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonEx), new FrameworkPropertyMetadata(typeof(ButtonEx)));
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x060006B6 RID: 1718 RVA: 0x000182CE File Offset: 0x000164CE
		// (set) Token: 0x060006B7 RID: 1719 RVA: 0x000182E0 File Offset: 0x000164E0
		public CornerRadius CornerRadius
		{
			get
			{
				return (CornerRadius)base.GetValue(ButtonEx.CornerRadiusProperty);
			}
			set
			{
				base.SetValue(ButtonEx.CornerRadiusProperty, value);
			}
		}

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x060006B8 RID: 1720 RVA: 0x000182F3 File Offset: 0x000164F3
		// (set) Token: 0x060006B9 RID: 1721 RVA: 0x00018305 File Offset: 0x00016505
		public Brush DisabledBackground
		{
			get
			{
				return (Brush)base.GetValue(ButtonEx.DisabledBackgroundProperty);
			}
			set
			{
				base.SetValue(ButtonEx.DisabledBackgroundProperty, value);
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x060006BA RID: 1722 RVA: 0x00018313 File Offset: 0x00016513
		// (set) Token: 0x060006BB RID: 1723 RVA: 0x00018325 File Offset: 0x00016525
		public Brush DisabledForeground
		{
			get
			{
				return (Brush)base.GetValue(ButtonEx.DisabledForegroundProperty);
			}
			set
			{
				base.SetValue(ButtonEx.DisabledForegroundProperty, value);
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x060006BC RID: 1724 RVA: 0x00018333 File Offset: 0x00016533
		// (set) Token: 0x060006BD RID: 1725 RVA: 0x00018345 File Offset: 0x00016545
		public Brush DisabledBorderbrush
		{
			get
			{
				return (Brush)base.GetValue(ButtonEx.DisabledBorderbrushProperty);
			}
			set
			{
				base.SetValue(ButtonEx.DisabledBorderbrushProperty, value);
			}
		}

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x060006BE RID: 1726 RVA: 0x00018353 File Offset: 0x00016553
		// (set) Token: 0x060006BF RID: 1727 RVA: 0x00018365 File Offset: 0x00016565
		public Brush MouseOverBackground
		{
			get
			{
				return (Brush)base.GetValue(ButtonEx.MouseOverBackgroundProperty);
			}
			set
			{
				base.SetValue(ButtonEx.MouseOverBackgroundProperty, value);
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x060006C0 RID: 1728 RVA: 0x00018373 File Offset: 0x00016573
		// (set) Token: 0x060006C1 RID: 1729 RVA: 0x00018385 File Offset: 0x00016585
		public Brush MouseOverForeground
		{
			get
			{
				return (Brush)base.GetValue(ButtonEx.MouseOverForegroundProperty);
			}
			set
			{
				base.SetValue(ButtonEx.MouseOverForegroundProperty, value);
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x00018393 File Offset: 0x00016593
		// (set) Token: 0x060006C3 RID: 1731 RVA: 0x000183A5 File Offset: 0x000165A5
		public Brush MouseOverBorderbrush
		{
			get
			{
				return (Brush)base.GetValue(ButtonEx.MouseOverBorderbrushProperty);
			}
			set
			{
				base.SetValue(ButtonEx.MouseOverBorderbrushProperty, value);
			}
		}

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x000183B3 File Offset: 0x000165B3
		// (set) Token: 0x060006C5 RID: 1733 RVA: 0x000183C5 File Offset: 0x000165C5
		public Brush MousePressedBackground
		{
			get
			{
				return (Brush)base.GetValue(ButtonEx.MousePressedBackgroundProperty);
			}
			set
			{
				base.SetValue(ButtonEx.MousePressedBackgroundProperty, value);
			}
		}

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x060006C6 RID: 1734 RVA: 0x000183D3 File Offset: 0x000165D3
		// (set) Token: 0x060006C7 RID: 1735 RVA: 0x000183E5 File Offset: 0x000165E5
		public Brush MousePressedForeground
		{
			get
			{
				return (Brush)base.GetValue(ButtonEx.MousePressedForegroundProperty);
			}
			set
			{
				base.SetValue(ButtonEx.MousePressedForegroundProperty, value);
			}
		}

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x060006C8 RID: 1736 RVA: 0x000183F3 File Offset: 0x000165F3
		// (set) Token: 0x060006C9 RID: 1737 RVA: 0x00018400 File Offset: 0x00016600
		public object DisabledContent
		{
			get
			{
				return base.GetValue(ButtonEx.DisabledContentProperty);
			}
			set
			{
				base.SetValue(ButtonEx.DisabledContentProperty, value);
			}
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x060006CA RID: 1738 RVA: 0x0001840E File Offset: 0x0001660E
		// (set) Token: 0x060006CB RID: 1739 RVA: 0x0001841B File Offset: 0x0001661B
		public object MouseOverContent
		{
			get
			{
				return base.GetValue(ButtonEx.MouseOverContentProperty);
			}
			set
			{
				base.SetValue(ButtonEx.MouseOverContentProperty, value);
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x060006CC RID: 1740 RVA: 0x00018429 File Offset: 0x00016629
		// (set) Token: 0x060006CD RID: 1741 RVA: 0x00018436 File Offset: 0x00016636
		public object MousePressedContent
		{
			get
			{
				return base.GetValue(ButtonEx.MousePressedContentProperty);
			}
			set
			{
				base.SetValue(ButtonEx.MousePressedContentProperty, value);
			}
		}

		// Token: 0x04000333 RID: 819
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ButtonEx), new PropertyMetadata(new CornerRadius(0.0)));

		// Token: 0x04000334 RID: 820
		public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register("DisabledBackground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x04000335 RID: 821
		public static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register("DisabledForeground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x04000336 RID: 822
		public static readonly DependencyProperty DisabledBorderbrushProperty = DependencyProperty.Register("DisabledBorderbrush", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x04000337 RID: 823
		public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x04000338 RID: 824
		public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x04000339 RID: 825
		public static readonly DependencyProperty MouseOverBorderbrushProperty = DependencyProperty.Register("MouseOverBorderbrush", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x0400033A RID: 826
		public static readonly DependencyProperty MousePressedBackgroundProperty = DependencyProperty.Register("MousePressedBackground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x0400033B RID: 827
		public static readonly DependencyProperty MousePressedForegroundProperty = DependencyProperty.Register("MousePressedForeground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x0400033C RID: 828
		public static readonly DependencyProperty DisabledContentProperty = DependencyProperty.Register("DisabledContent", typeof(object), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x0400033D RID: 829
		public static readonly DependencyProperty MouseOverContentProperty = DependencyProperty.Register("MouseOverContent", typeof(object), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x0400033E RID: 830
		public static readonly DependencyProperty MousePressedContentProperty = DependencyProperty.Register("MousePressedContent", typeof(object), typeof(ButtonEx), new PropertyMetadata(null));
	}
}
