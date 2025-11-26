using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfeditor.Controls
{
	// Token: 0x020001CC RID: 460
	internal class PdfViewerScrollViewer : ScrollViewer
	{
		// Token: 0x06001A05 RID: 6661 RVA: 0x00067866 File Offset: 0x00065A66
		protected override void OnManipulationStarting(ManipulationStartingEventArgs e)
		{
			base.OnManipulationStarting(e);
			this.isScale = false;
			e.Mode |= ManipulationModes.Scale;
		}

		// Token: 0x06001A06 RID: 6662 RVA: 0x00067884 File Offset: 0x00065A84
		protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
		{
			Vector scale = e.DeltaManipulation.Scale;
			if (scale.X != 1.0 || scale.Y != 1.0 || this.isScale)
			{
				this.isScale = true;
				return;
			}
			base.OnManipulationDelta(e);
		}

		// Token: 0x06001A07 RID: 6663 RVA: 0x000678D8 File Offset: 0x00065AD8
		protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
		{
			if (!this.isScale)
			{
				base.OnManipulationInertiaStarting(e);
			}
		}

		// Token: 0x06001A08 RID: 6664 RVA: 0x000678E9 File Offset: 0x00065AE9
		protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
		{
			if (!this.isScale)
			{
				base.OnManipulationCompleted(e);
			}
		}

		// Token: 0x040008FD RID: 2301
		private bool isScale;
	}
}
