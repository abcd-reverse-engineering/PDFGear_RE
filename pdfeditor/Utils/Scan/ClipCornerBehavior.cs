using System;
using System.Windows;
using System.Windows.Documents;
using HandyControl.Interactivity;

namespace pdfeditor.Utils.Scan
{
	// Token: 0x020000B1 RID: 177
	internal class ClipCornerBehavior : Behavior<UIElement>
	{
		// Token: 0x17000264 RID: 612
		// (get) Token: 0x06000AE0 RID: 2784 RVA: 0x0003888E File Offset: 0x00036A8E
		// (set) Token: 0x06000AE1 RID: 2785 RVA: 0x00038896 File Offset: 0x00036A96
		public double Radius { get; set; }

		// Token: 0x06000AE2 RID: 2786 RVA: 0x0003889F File Offset: 0x00036A9F
		protected override void OnAttached()
		{
			AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(base.AssociatedObject);
			if (adornerLayer == null)
			{
				return;
			}
			adornerLayer.Add(new ClipCornerAdorner(base.AssociatedObject, this.Radius));
		}
	}
}
