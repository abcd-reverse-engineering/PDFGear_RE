using System;
using System.Windows;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000208 RID: 520
	public class DeleteControlOperation : DrawOperation
	{
		// Token: 0x17000A42 RID: 2626
		// (get) Token: 0x06001D01 RID: 7425 RVA: 0x0007DAB7 File Offset: 0x0007BCB7
		// (set) Token: 0x06001D02 RID: 7426 RVA: 0x0007DABF File Offset: 0x0007BCBF
		public double Left { get; private set; }

		// Token: 0x17000A43 RID: 2627
		// (get) Token: 0x06001D03 RID: 7427 RVA: 0x0007DAC8 File Offset: 0x0007BCC8
		// (set) Token: 0x06001D04 RID: 7428 RVA: 0x0007DAD0 File Offset: 0x0007BCD0
		public double Top { get; private set; }

		// Token: 0x17000A44 RID: 2628
		// (get) Token: 0x06001D05 RID: 7429 RVA: 0x0007DAD9 File Offset: 0x0007BCD9
		// (set) Token: 0x06001D06 RID: 7430 RVA: 0x0007DAE1 File Offset: 0x0007BCE1
		public Rect Location { get; private set; }

		// Token: 0x06001D07 RID: 7431 RVA: 0x0007DAEA File Offset: 0x0007BCEA
		public DeleteControlOperation(UIElement element, double left, double top)
		{
			base.Type = OperationType.DeleteControl;
			base.Element = element;
			this.Left = left;
			this.Top = top;
		}
	}
}
