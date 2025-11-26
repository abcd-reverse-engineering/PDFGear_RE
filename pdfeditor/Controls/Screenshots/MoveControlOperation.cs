using System;
using System.Windows;
using System.Windows.Media;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000206 RID: 518
	public class MoveControlOperation : DrawOperation
	{
		// Token: 0x17000A40 RID: 2624
		// (get) Token: 0x06001CFB RID: 7419 RVA: 0x0007DA5B File Offset: 0x0007BC5B
		// (set) Token: 0x06001CFC RID: 7420 RVA: 0x0007DA63 File Offset: 0x0007BC63
		public TransformGroup OriginalTransformGroup { get; private set; }

		// Token: 0x06001CFD RID: 7421 RVA: 0x0007DA6C File Offset: 0x0007BC6C
		public MoveControlOperation(UIElement element, TransformGroup originalTransformGroup)
		{
			base.Type = OperationType.MoveControl;
			base.Element = element;
			this.OriginalTransformGroup = originalTransformGroup;
		}
	}
}
