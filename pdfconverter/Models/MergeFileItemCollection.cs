using System;
using System.Collections.ObjectModel;

namespace pdfconverter.Models
{
	// Token: 0x02000069 RID: 105
	public class MergeFileItemCollection : ObservableCollection<MergeFileItem>
	{
		// Token: 0x060005D8 RID: 1496 RVA: 0x000168CF File Offset: 0x00014ACF
		protected override void ClearItems()
		{
			base.ClearItems();
		}
	}
}
