using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PDFLauncher.CustomControl
{
	// Token: 0x02000024 RID: 36
	public class LabelButton : Button
	{
		// Token: 0x060001CE RID: 462 RVA: 0x00006FDC File Offset: 0x000051DC
		static LabelButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelButton), new FrameworkPropertyMetadata(typeof(LabelButton)));
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060001CF RID: 463 RVA: 0x000071AE File Offset: 0x000053AE
		// (set) Token: 0x060001D0 RID: 464 RVA: 0x000071C0 File Offset: 0x000053C0
		public Thickness ImgMargin
		{
			get
			{
				return (Thickness)base.GetValue(LabelButton.ImgMarginProperty);
			}
			set
			{
				base.SetValue(LabelButton.ImgMarginProperty, value);
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060001D1 RID: 465 RVA: 0x000071D3 File Offset: 0x000053D3
		// (set) Token: 0x060001D2 RID: 466 RVA: 0x000071E5 File Offset: 0x000053E5
		public ImageSource Icon
		{
			get
			{
				return (ImageSource)base.GetValue(LabelButton.IconProperty);
			}
			set
			{
				base.SetValue(LabelButton.IconProperty, value);
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x000071F3 File Offset: 0x000053F3
		// (set) Token: 0x060001D4 RID: 468 RVA: 0x00007205 File Offset: 0x00005405
		public string Text
		{
			get
			{
				return (string)base.GetValue(LabelButton.TextProperty);
			}
			set
			{
				base.SetValue(LabelButton.TextProperty, value);
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060001D5 RID: 469 RVA: 0x00007213 File Offset: 0x00005413
		// (set) Token: 0x060001D6 RID: 470 RVA: 0x00007225 File Offset: 0x00005425
		public double IconWidth
		{
			get
			{
				return (double)base.GetValue(LabelButton.IconWidthProperty);
			}
			set
			{
				base.SetValue(LabelButton.IconWidthProperty, value);
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060001D7 RID: 471 RVA: 0x00007238 File Offset: 0x00005438
		// (set) Token: 0x060001D8 RID: 472 RVA: 0x0000724A File Offset: 0x0000544A
		public double IconHeight
		{
			get
			{
				return (double)base.GetValue(LabelButton.IconHeightProperty);
			}
			set
			{
				base.SetValue(LabelButton.IconHeightProperty, value);
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x060001D9 RID: 473 RVA: 0x0000725D File Offset: 0x0000545D
		// (set) Token: 0x060001DA RID: 474 RVA: 0x0000726F File Offset: 0x0000546F
		public bool HasChildren
		{
			get
			{
				return (bool)base.GetValue(LabelButton.HasChildrenProperty);
			}
			set
			{
				base.SetValue(LabelButton.HasChildrenProperty, value);
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060001DB RID: 475 RVA: 0x00007282 File Offset: 0x00005482
		// (set) Token: 0x060001DC RID: 476 RVA: 0x00007294 File Offset: 0x00005494
		public bool ShowIcon
		{
			get
			{
				return (bool)base.GetValue(LabelButton.ShowIconProperty);
			}
			set
			{
				base.SetValue(LabelButton.ShowIconProperty, value);
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060001DD RID: 477 RVA: 0x000072A7 File Offset: 0x000054A7
		// (set) Token: 0x060001DE RID: 478 RVA: 0x000072B9 File Offset: 0x000054B9
		public Visibility ArrowVisibility
		{
			get
			{
				return (Visibility)base.GetValue(LabelButton.ArrowVisibilityProperty);
			}
			set
			{
				base.SetValue(LabelButton.ArrowVisibilityProperty, value);
			}
		}

		// Token: 0x040000DC RID: 220
		public static readonly DependencyProperty ImgMarginProperty = DependencyProperty.Register("ImgMargin", typeof(Thickness), typeof(LabelButton), new PropertyMetadata(new Thickness(5.0, 5.0, 5.0, 5.0)));

		// Token: 0x040000DD RID: 221
		public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(LabelButton), new PropertyMetadata(null));

		// Token: 0x040000DE RID: 222
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(LabelButton), new PropertyMetadata("Button"));

		// Token: 0x040000DF RID: 223
		public static readonly DependencyProperty IconWidthProperty = DependencyProperty.Register("IconWidth", typeof(double), typeof(LabelButton), new PropertyMetadata(32.0));

		// Token: 0x040000E0 RID: 224
		public static readonly DependencyProperty IconHeightProperty = DependencyProperty.Register("IconHeight", typeof(double), typeof(LabelButton), new PropertyMetadata(32.0));

		// Token: 0x040000E1 RID: 225
		public static readonly DependencyProperty HasChildrenProperty = DependencyProperty.Register("HasChildren", typeof(bool), typeof(LabelButton), new PropertyMetadata(false));

		// Token: 0x040000E2 RID: 226
		public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register("ShowIcon", typeof(bool), typeof(LabelButton), new PropertyMetadata(true));

		// Token: 0x040000E3 RID: 227
		public static readonly DependencyProperty ArrowVisibilityProperty = DependencyProperty.Register("ArrowVisibility", typeof(Visibility), typeof(LabelButton), new PropertyMetadata(Visibility.Visible));
	}
}
