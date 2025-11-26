using System;
using System.Windows.Media;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x0200020A RID: 522
	public class DrawSettingConstants
	{
		// Token: 0x04000AE3 RID: 2787
		public static readonly double DefaultFontSize = 12.0;

		// Token: 0x04000AE4 RID: 2788
		public static readonly Color[] Colors = new Color[]
		{
			Color.FromRgb(0, 0, 0),
			Color.FromRgb(251, 48, 47),
			Color.FromRgb(253, 153, 39),
			Color.FromRgb(254, 216, 49),
			Color.FromRgb(165, 222, 80),
			Color.FromRgb(67, 217, 239),
			Color.FromRgb(82, 170, 236),
			Color.FromRgb(149, 115, 228)
		};

		// Token: 0x04000AE5 RID: 2789
		public static readonly double[] Thicknesses = new double[] { 1.0, 3.0, 5.0 };

		// Token: 0x04000AE6 RID: 2790
		public static readonly double[] ArrowHeight = new double[] { 12.0, 18.0, 26.0 };

		// Token: 0x04000AE7 RID: 2791
		public static readonly double[] ThicknessListBoxEllipseSize = new double[] { 6.0, 10.0, 14.0 };
	}
}
