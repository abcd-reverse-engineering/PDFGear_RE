using System;
using System.Windows;
using System.Windows.Media;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000203 RID: 515
	public class ChangeColorOperation : DrawOperation
	{
		// Token: 0x17000A3D RID: 2621
		// (get) Token: 0x06001CF2 RID: 7410 RVA: 0x0007D9D0 File Offset: 0x0007BBD0
		// (set) Token: 0x06001CF3 RID: 7411 RVA: 0x0007D9D8 File Offset: 0x0007BBD8
		public Brush OriginalBrush { get; private set; }

		// Token: 0x06001CF4 RID: 7412 RVA: 0x0007D9E1 File Offset: 0x0007BBE1
		public ChangeColorOperation(UIElement element, Brush originalBrush)
		{
			base.Type = OperationType.ChangeColor;
			base.Element = element;
			this.OriginalBrush = originalBrush;
		}
	}
}
