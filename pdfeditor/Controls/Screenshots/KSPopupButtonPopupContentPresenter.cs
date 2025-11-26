using System;
using System.Windows.Controls;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000211 RID: 529
	public class KSPopupButtonPopupContentPresenter : ContentControl
	{
		// Token: 0x06001D65 RID: 7525 RVA: 0x0007EBF4 File Offset: 0x0007CDF4
		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);
			base.RemoveLogicalChild(newContent);
		}
	}
}
