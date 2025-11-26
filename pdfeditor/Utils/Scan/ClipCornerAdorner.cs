using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace pdfeditor.Utils.Scan
{
	// Token: 0x020000B2 RID: 178
	internal class ClipCornerAdorner : Adorner
	{
		// Token: 0x06000AE4 RID: 2788 RVA: 0x000388CF File Offset: 0x00036ACF
		public ClipCornerAdorner(UIElement adornedElement, double radius)
			: base(adornedElement)
		{
			this.radius = radius;
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x000388DF File Offset: 0x00036ADF
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);
			base.AdornedElement.Clip = new RectangleGeometry(new Rect(sizeInfo.NewSize), this.radius, this.radius);
		}

		// Token: 0x040004C0 RID: 1216
		private readonly double radius;
	}
}
