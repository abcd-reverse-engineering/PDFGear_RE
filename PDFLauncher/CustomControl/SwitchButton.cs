using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PDFLauncher.CustomControl
{
	// Token: 0x02000029 RID: 41
	public class SwitchButton : CheckBox
	{
		// Token: 0x060001F9 RID: 505 RVA: 0x000076CC File Offset: 0x000058CC
		static SwitchButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SwitchButton), new FrameworkPropertyMetadata(typeof(SwitchButton)));
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060001FA RID: 506 RVA: 0x00007725 File Offset: 0x00005925
		// (set) Token: 0x060001FB RID: 507 RVA: 0x00007737 File Offset: 0x00005937
		public ImageSource Icon
		{
			get
			{
				return (ImageSource)base.GetValue(SwitchButton.IconProperty);
			}
			set
			{
				base.SetValue(SwitchButton.IconProperty, value);
			}
		}

		// Token: 0x040000EA RID: 234
		public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(SwitchButton), new PropertyMetadata(null));
	}
}
