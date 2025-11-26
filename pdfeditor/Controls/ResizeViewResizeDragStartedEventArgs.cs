using System;

namespace pdfeditor.Controls
{
	// Token: 0x020001D0 RID: 464
	public class ResizeViewResizeDragStartedEventArgs
	{
		// Token: 0x06001A54 RID: 6740 RVA: 0x00069DCB File Offset: 0x00067FCB
		public ResizeViewResizeDragStartedEventArgs(ResizeViewOperation operation)
		{
			this.Operation = operation;
		}

		// Token: 0x170009DE RID: 2526
		// (get) Token: 0x06001A55 RID: 6741 RVA: 0x00069DDA File Offset: 0x00067FDA
		public ResizeViewOperation Operation { get; }
	}
}
