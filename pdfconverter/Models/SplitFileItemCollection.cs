using System;
using System.Collections.ObjectModel;

namespace pdfconverter.Models
{
	// Token: 0x0200007F RID: 127
	public class SplitFileItemCollection : ObservableCollection<SplitFileItem>
	{
		// Token: 0x06000623 RID: 1571 RVA: 0x00016FB0 File Offset: 0x000151B0
		protected override void ClearItems()
		{
			base.ClearItems();
		}
	}
}
