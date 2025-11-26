using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;

namespace pdfeditor.Controls.Copilot
{
	// Token: 0x02000291 RID: 657
	internal class LoadingEllipsis : Run
	{
		// Token: 0x060025D5 RID: 9685 RVA: 0x000B054E File Offset: 0x000AE74E
		public LoadingEllipsis(bool delay)
		{
			base.Loaded += this.LoadingEllipsis_Loaded;
			base.Unloaded += this.LoadingEllipsis_Unloaded;
			base.Text = "...";
			this.delay = delay;
		}

		// Token: 0x060025D6 RID: 9686 RVA: 0x000B058C File Offset: 0x000AE78C
		private void LoadingEllipsis_Loaded(object sender, RoutedEventArgs e)
		{
			this.CreateTimer();
		}

		// Token: 0x060025D7 RID: 9687 RVA: 0x000B0594 File Offset: 0x000AE794
		private void LoadingEllipsis_Unloaded(object sender, RoutedEventArgs e)
		{
			this.RemoveTimer();
		}

		// Token: 0x060025D8 RID: 9688 RVA: 0x000B059C File Offset: 0x000AE79C
		private void CreateTimer()
		{
			this.RemoveTimer();
			this.timer = new DispatcherTimer(DispatcherPriority.Normal)
			{
				Interval = TimeSpan.FromSeconds(this.delay ? 1.0 : 0.3)
			};
			this.timer.Tick += this.Timer_Tick;
			this.timer.Start();
		}

		// Token: 0x060025D9 RID: 9689 RVA: 0x000B0605 File Offset: 0x000AE805
		private void RemoveTimer()
		{
			if (this.timer != null)
			{
				this.timer.Stop();
				this.timer.Tick -= this.Timer_Tick;
				this.timer = null;
			}
		}

		// Token: 0x060025DA RID: 9690 RVA: 0x000B0638 File Offset: 0x000AE838
		private void Timer_Tick(object sender, EventArgs e)
		{
			if (this.timer.Interval.TotalSeconds > 0.99)
			{
				this.timer.Stop();
				this.timer.Interval = TimeSpan.FromSeconds(0.3);
				this.timer.Start();
			}
			string text = base.Text;
			if (text == ".")
			{
				base.Text = "..";
				return;
			}
			if (text == "..")
			{
				base.Text = "...";
				return;
			}
			if (!(text == "..."))
			{
				base.Text = ".";
				return;
			}
			base.Text = "";
		}

		// Token: 0x0400104E RID: 4174
		private DispatcherTimer timer;

		// Token: 0x0400104F RID: 4175
		private readonly bool delay;
	}
}
