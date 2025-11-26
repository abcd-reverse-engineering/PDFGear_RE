using System;

namespace pdfeditor.Models.Viewer
{
	// Token: 0x02000132 RID: 306
	internal class ViewOperationResult<T>
	{
		// Token: 0x060012AC RID: 4780 RVA: 0x0004C123 File Offset: 0x0004A323
		public ViewOperationResult(T value)
		{
			this.Value = value;
		}

		// Token: 0x1700079F RID: 1951
		// (get) Token: 0x060012AD RID: 4781 RVA: 0x0004C132 File Offset: 0x0004A332
		public T Value { get; }
	}
}
