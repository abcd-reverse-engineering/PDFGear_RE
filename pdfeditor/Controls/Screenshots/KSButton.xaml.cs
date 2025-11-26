using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x0200020D RID: 525
	public partial class KSButton : Button
	{
		// Token: 0x06001D2E RID: 7470 RVA: 0x0007E218 File Offset: 0x0007C418
		static KSButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(KSButton), new FrameworkPropertyMetadata(typeof(KSButton)));
		}

		// Token: 0x17000A4E RID: 2638
		// (get) Token: 0x06001D2F RID: 7471 RVA: 0x0007E283 File Offset: 0x0007C483
		// (set) Token: 0x06001D30 RID: 7472 RVA: 0x0007E295 File Offset: 0x0007C495
		public CornerRadius CornerRadius
		{
			get
			{
				return (CornerRadius)base.GetValue(KSButton.CornerRadiusProperty);
			}
			set
			{
				base.SetValue(KSButton.CornerRadiusProperty, value);
			}
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x0007E2A8 File Offset: 0x0007C4A8
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			base.Focusable = true;
			base.Focus();
			base.Focusable = false;
		}

		// Token: 0x04000AFB RID: 2811
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(KSButton), new PropertyMetadata(new CornerRadius(0.0)));
	}
}
