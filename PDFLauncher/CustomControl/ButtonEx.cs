using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PDFLauncher.CustomControl
{
	// Token: 0x02000023 RID: 35
	public class ButtonEx : Button
	{
		// Token: 0x060001B4 RID: 436 RVA: 0x00006C30 File Offset: 0x00004E30
		static ButtonEx()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonEx), new FrameworkPropertyMetadata(typeof(ButtonEx)));
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x060001B5 RID: 437 RVA: 0x00006E5E File Offset: 0x0000505E
		// (set) Token: 0x060001B6 RID: 438 RVA: 0x00006E70 File Offset: 0x00005070
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

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x00006E83 File Offset: 0x00005083
		// (set) Token: 0x060001B8 RID: 440 RVA: 0x00006E95 File Offset: 0x00005095
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

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x00006EA3 File Offset: 0x000050A3
		// (set) Token: 0x060001BA RID: 442 RVA: 0x00006EB5 File Offset: 0x000050B5
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

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x060001BB RID: 443 RVA: 0x00006EC3 File Offset: 0x000050C3
		// (set) Token: 0x060001BC RID: 444 RVA: 0x00006ED5 File Offset: 0x000050D5
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

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x060001BD RID: 445 RVA: 0x00006EE3 File Offset: 0x000050E3
		// (set) Token: 0x060001BE RID: 446 RVA: 0x00006EF5 File Offset: 0x000050F5
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

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x060001BF RID: 447 RVA: 0x00006F03 File Offset: 0x00005103
		// (set) Token: 0x060001C0 RID: 448 RVA: 0x00006F15 File Offset: 0x00005115
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

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060001C1 RID: 449 RVA: 0x00006F23 File Offset: 0x00005123
		// (set) Token: 0x060001C2 RID: 450 RVA: 0x00006F35 File Offset: 0x00005135
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

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060001C3 RID: 451 RVA: 0x00006F43 File Offset: 0x00005143
		// (set) Token: 0x060001C4 RID: 452 RVA: 0x00006F55 File Offset: 0x00005155
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

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060001C5 RID: 453 RVA: 0x00006F63 File Offset: 0x00005163
		// (set) Token: 0x060001C6 RID: 454 RVA: 0x00006F75 File Offset: 0x00005175
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

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060001C7 RID: 455 RVA: 0x00006F83 File Offset: 0x00005183
		// (set) Token: 0x060001C8 RID: 456 RVA: 0x00006F90 File Offset: 0x00005190
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

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060001C9 RID: 457 RVA: 0x00006F9E File Offset: 0x0000519E
		// (set) Token: 0x060001CA RID: 458 RVA: 0x00006FAB File Offset: 0x000051AB
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

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060001CB RID: 459 RVA: 0x00006FB9 File Offset: 0x000051B9
		// (set) Token: 0x060001CC RID: 460 RVA: 0x00006FC6 File Offset: 0x000051C6
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

		// Token: 0x040000D0 RID: 208
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ButtonEx), new PropertyMetadata(new CornerRadius(0.0)));

		// Token: 0x040000D1 RID: 209
		public static readonly DependencyProperty DisabledBackgroundProperty = DependencyProperty.Register("DisabledBackground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x040000D2 RID: 210
		public static readonly DependencyProperty DisabledForegroundProperty = DependencyProperty.Register("DisabledForeground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x040000D3 RID: 211
		public static readonly DependencyProperty DisabledBorderbrushProperty = DependencyProperty.Register("DisabledBorderbrush", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x040000D4 RID: 212
		public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x040000D5 RID: 213
		public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x040000D6 RID: 214
		public static readonly DependencyProperty MouseOverBorderbrushProperty = DependencyProperty.Register("MouseOverBorderbrush", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x040000D7 RID: 215
		public static readonly DependencyProperty MousePressedBackgroundProperty = DependencyProperty.Register("MousePressedBackground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x040000D8 RID: 216
		public static readonly DependencyProperty MousePressedForegroundProperty = DependencyProperty.Register("MousePressedForeground", typeof(Brush), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x040000D9 RID: 217
		public static readonly DependencyProperty DisabledContentProperty = DependencyProperty.Register("DisabledContent", typeof(object), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x040000DA RID: 218
		public static readonly DependencyProperty MouseOverContentProperty = DependencyProperty.Register("MouseOverContent", typeof(object), typeof(ButtonEx), new PropertyMetadata(null));

		// Token: 0x040000DB RID: 219
		public static readonly DependencyProperty MousePressedContentProperty = DependencyProperty.Register("MousePressedContent", typeof(object), typeof(ButtonEx), new PropertyMetadata(null));
	}
}
