using System;
using System.Windows;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000204 RID: 516
	public class ChangeThicknessOperation : DrawOperation
	{
		// Token: 0x17000A3E RID: 2622
		// (get) Token: 0x06001CF5 RID: 7413 RVA: 0x0007D9FE File Offset: 0x0007BBFE
		// (set) Token: 0x06001CF6 RID: 7414 RVA: 0x0007DA06 File Offset: 0x0007BC06
		public double OriginalThickness { get; private set; }

		// Token: 0x06001CF7 RID: 7415 RVA: 0x0007DA0F File Offset: 0x0007BC0F
		public ChangeThicknessOperation(UIElement element, double originalThickness)
		{
			base.Type = OperationType.ChangeThickness;
			base.Element = element;
			this.OriginalThickness = originalThickness;
		}
	}
}
