using System;
using System.Windows;

namespace pdfeditor.Controls.Stamp
{
	// Token: 0x020001EC RID: 492
	public class BindingProxy : Freezable
	{
		// Token: 0x06001BE6 RID: 7142 RVA: 0x0007348B File Offset: 0x0007168B
		protected override Freezable CreateInstanceCore()
		{
			return new BindingProxy();
		}

		// Token: 0x17000A1B RID: 2587
		// (get) Token: 0x06001BE7 RID: 7143 RVA: 0x00073492 File Offset: 0x00071692
		// (set) Token: 0x06001BE8 RID: 7144 RVA: 0x0007349F File Offset: 0x0007169F
		public object Data
		{
			get
			{
				return base.GetValue(BindingProxy.DataProperty);
			}
			set
			{
				base.SetValue(BindingProxy.DataProperty, value);
			}
		}

		// Token: 0x04000A1C RID: 2588
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
	}
}
