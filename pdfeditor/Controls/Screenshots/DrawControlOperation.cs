using System;
using System.Windows;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000202 RID: 514
	public class DrawControlOperation : DrawOperation
	{
		// Token: 0x06001CF1 RID: 7409 RVA: 0x0007D9BA File Offset: 0x0007BBBA
		public DrawControlOperation(OperationType type, UIElement element)
		{
			base.Type = type;
			base.Element = element;
		}
	}
}
