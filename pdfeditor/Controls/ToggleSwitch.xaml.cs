using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace pdfeditor.Controls
{
	// Token: 0x020001E0 RID: 480
	public partial class ToggleSwitch : ToggleButton
	{
		// Token: 0x06001B2C RID: 6956 RVA: 0x0006D9E4 File Offset: 0x0006BBE4
		static ToggleSwitch()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleSwitch), new FrameworkPropertyMetadata(typeof(ToggleSwitch)));
		}

		// Token: 0x06001B2D RID: 6957 RVA: 0x0006DA09 File Offset: 0x0006BC09
		public ToggleSwitch()
		{
			base.Loaded += this.ToggleSwitch_Loaded;
		}

		// Token: 0x06001B2E RID: 6958 RVA: 0x0006DA23 File Offset: 0x0006BC23
		private void ToggleSwitch_Loaded(object sender, RoutedEventArgs e)
		{
			this.loadedTick = Stopwatch.GetTimestamp();
		}

		// Token: 0x06001B2F RID: 6959 RVA: 0x0006DA30 File Offset: 0x0006BC30
		protected override void OnChecked(RoutedEventArgs e)
		{
			base.OnChecked(e);
			if (Stopwatch.GetTimestamp() - this.loadedTick < 5000000L)
			{
				VisualStateManager.GoToState(this, "Unchecked", false);
				VisualStateManager.GoToState(this, "Checked", false);
			}
		}

		// Token: 0x04000989 RID: 2441
		private long loadedTick;
	}
}
