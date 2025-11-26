using System;

namespace pdfeditor.Models.Viewer
{
	// Token: 0x02000131 RID: 305
	internal class ViewerOperationCompletedEventArgs<T>
	{
		// Token: 0x060012AA RID: 4778 RVA: 0x0004C10C File Offset: 0x0004A30C
		public ViewerOperationCompletedEventArgs(ViewOperationResult<T> result)
		{
			this.Result = result;
		}

		// Token: 0x1700079E RID: 1950
		// (get) Token: 0x060012AB RID: 4779 RVA: 0x0004C11B File Offset: 0x0004A31B
		public ViewOperationResult<T> Result { get; }
	}
}
