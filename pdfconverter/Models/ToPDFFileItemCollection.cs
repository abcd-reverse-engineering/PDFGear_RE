using System;
using System.Collections.ObjectModel;

namespace pdfconverter.Models
{
	// Token: 0x02000084 RID: 132
	public class ToPDFFileItemCollection : ObservableCollection<ToPDFFileItem>
	{
		// Token: 0x06000675 RID: 1653 RVA: 0x00017A1D File Offset: 0x00015C1D
		protected override void ClearItems()
		{
			base.ClearItems();
		}
	}
}
