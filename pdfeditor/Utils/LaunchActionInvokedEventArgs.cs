using System;

namespace pdfeditor.Utils
{
	// Token: 0x02000080 RID: 128
	public class LaunchActionInvokedEventArgs
	{
		// Token: 0x060008E9 RID: 2281 RVA: 0x0002C693 File Offset: 0x0002A893
		public LaunchActionInvokedEventArgs(string action)
		{
			this.Action = action;
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x060008EA RID: 2282 RVA: 0x0002C6A2 File Offset: 0x0002A8A2
		public string Action { get; }
	}
}
