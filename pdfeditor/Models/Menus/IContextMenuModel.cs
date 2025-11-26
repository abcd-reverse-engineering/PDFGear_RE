using System;
using System.ComponentModel;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000157 RID: 343
	public interface IContextMenuModel : INotifyPropertyChanging, INotifyPropertyChanged
	{
		// Token: 0x1700084A RID: 2122
		// (get) Token: 0x0600148F RID: 5263
		string Name { get; }

		// Token: 0x1700084B RID: 2123
		// (get) Token: 0x06001490 RID: 5264
		// (set) Token: 0x06001491 RID: 5265
		IContextMenuModel Parent { get; set; }

		// Token: 0x1700084C RID: 2124
		// (get) Token: 0x06001492 RID: 5266
		int Level { get; }
	}
}
