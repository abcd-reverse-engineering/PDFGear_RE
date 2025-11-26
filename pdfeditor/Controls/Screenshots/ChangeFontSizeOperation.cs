using System;
using System.Windows;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000205 RID: 517
	public class ChangeFontSizeOperation : DrawOperation
	{
		// Token: 0x17000A3F RID: 2623
		// (get) Token: 0x06001CF8 RID: 7416 RVA: 0x0007DA2C File Offset: 0x0007BC2C
		// (set) Token: 0x06001CF9 RID: 7417 RVA: 0x0007DA34 File Offset: 0x0007BC34
		public double OriginalFontSize { get; private set; }

		// Token: 0x06001CFA RID: 7418 RVA: 0x0007DA3D File Offset: 0x0007BC3D
		public ChangeFontSizeOperation(UIElement element, double originalFontSize)
		{
			base.Type = OperationType.ChangeFontSize;
			base.Element = element;
			this.OriginalFontSize = originalFontSize;
		}
	}
}
