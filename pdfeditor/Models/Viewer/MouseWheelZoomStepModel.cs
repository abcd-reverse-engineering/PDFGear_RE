using System;
using System.Collections.Generic;

namespace pdfeditor.Models.Viewer
{
	// Token: 0x0200012F RID: 303
	public static class MouseWheelZoomStepModel
	{
		// Token: 0x040005E2 RID: 1506
		public static List<int> MouseWheelZoomStepModels = new List<int>
		{
			1, 5, 10, 15, 25, 33, 50, 67, 75, 85,
			100, 125, 150, 200, 208, 250, 300, 400, 600, 800,
			1200, 1600, 2400, 3200, 6400
		};
	}
}
