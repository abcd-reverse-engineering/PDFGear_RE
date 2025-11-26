using System;
using System.Threading;

namespace pdfeditor.Utils.Scan
{
	// Token: 0x020000B3 RID: 179
	internal class DebounceHelper
	{
		// Token: 0x06000AE6 RID: 2790 RVA: 0x00038910 File Offset: 0x00036B10
		public void Invoke(int dueTime, Action<object> action, object state = null)
		{
			if (this.timer != null)
			{
				this.timer.Dispose();
				this.timer = null;
			}
			this.timer = new Timer(delegate(object obj)
			{
				if (this.timer == null)
				{
					return;
				}
				this.timer.Dispose();
				this.timer = null;
				action(state);
			}, state, dueTime, -1);
		}

		// Token: 0x06000AE7 RID: 2791 RVA: 0x00038971 File Offset: 0x00036B71
		public void Cancel()
		{
			if (this.timer != null)
			{
				this.timer.Dispose();
				this.timer = null;
			}
		}

		// Token: 0x040004C1 RID: 1217
		private Timer timer;
	}
}
