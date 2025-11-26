using System;
using System.Windows;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000207 RID: 519
	public class ChangeTextBoxTextOperation : DrawOperation
	{
		// Token: 0x17000A41 RID: 2625
		// (get) Token: 0x06001CFE RID: 7422 RVA: 0x0007DA89 File Offset: 0x0007BC89
		// (set) Token: 0x06001CFF RID: 7423 RVA: 0x0007DA91 File Offset: 0x0007BC91
		public string OriginalText { get; private set; }

		// Token: 0x06001D00 RID: 7424 RVA: 0x0007DA9A File Offset: 0x0007BC9A
		public ChangeTextBoxTextOperation(UIElement element, string originalText)
		{
			base.Type = OperationType.ChangeTextBoxText;
			base.Element = element;
			this.OriginalText = originalText;
		}
	}
}
