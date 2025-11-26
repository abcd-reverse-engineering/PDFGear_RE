using System;
using System.Collections.ObjectModel;

namespace pdfconverter.Models
{
	// Token: 0x02000057 RID: 87
	public class CompressItemCollection : ObservableCollection<CompressItem>
	{
		// Token: 0x06000587 RID: 1415 RVA: 0x000161B9 File Offset: 0x000143B9
		protected override void ClearItems()
		{
			base.ClearItems();
		}
	}
}
