using System;
using System.Windows;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000212 RID: 530
	public class KSSelectableButton : KSButton
	{
		// Token: 0x17000A5C RID: 2652
		// (get) Token: 0x06001D67 RID: 7527 RVA: 0x0007EC0D File Offset: 0x0007CE0D
		// (set) Token: 0x06001D68 RID: 7528 RVA: 0x0007EC1F File Offset: 0x0007CE1F
		public bool IsSelected
		{
			get
			{
				return (bool)base.GetValue(KSSelectableButton.IsSelectedProperty);
			}
			set
			{
				base.SetValue(KSSelectableButton.IsSelectedProperty, value);
			}
		}

		// Token: 0x04000B10 RID: 2832
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(KSButton), new PropertyMetadata(false));
	}
}
