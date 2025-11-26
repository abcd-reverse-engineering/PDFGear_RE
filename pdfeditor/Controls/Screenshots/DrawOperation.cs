using System;
using System.Windows;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000201 RID: 513
	public abstract class DrawOperation
	{
		// Token: 0x17000A3B RID: 2619
		// (get) Token: 0x06001CEC RID: 7404 RVA: 0x0007D990 File Offset: 0x0007BB90
		// (set) Token: 0x06001CED RID: 7405 RVA: 0x0007D998 File Offset: 0x0007BB98
		public OperationType Type { get; protected set; }

		// Token: 0x17000A3C RID: 2620
		// (get) Token: 0x06001CEE RID: 7406 RVA: 0x0007D9A1 File Offset: 0x0007BBA1
		// (set) Token: 0x06001CEF RID: 7407 RVA: 0x0007D9A9 File Offset: 0x0007BBA9
		public UIElement Element { get; protected set; }
	}
}
