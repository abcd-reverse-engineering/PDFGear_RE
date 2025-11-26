using System;
using System.Threading;

namespace pdfconverter.Utils.Image
{
	// Token: 0x02000046 RID: 70
	internal class DebounceHelper
	{
		// Token: 0x0600051D RID: 1309 RVA: 0x00015178 File Offset: 0x00013378
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

		// Token: 0x0600051E RID: 1310 RVA: 0x000151D9 File Offset: 0x000133D9
		public void Cancel()
		{
			if (this.timer != null)
			{
				this.timer.Dispose();
				this.timer = null;
			}
		}

		// Token: 0x04000279 RID: 633
		private Timer timer;
	}
}
