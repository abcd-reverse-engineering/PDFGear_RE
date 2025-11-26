using System;

namespace pdfeditor.Models.Viewer
{
	// Token: 0x02000133 RID: 307
	// (Invoke) Token: 0x060012AF RID: 4783
	internal delegate void ViewerOperationEventHandler<T, TResult>(T sender, ViewerOperationCompletedEventArgs<TResult> e);
}
