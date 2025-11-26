using System;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000162 RID: 354
	public class SelectableModelSelectionChangedEventArgs : EventArgs
	{
		// Token: 0x06001563 RID: 5475 RVA: 0x000535D5 File Offset: 0x000517D5
		public SelectableModelSelectionChangedEventArgs(ContextMenuItemModel item)
		{
			this.SelectedItem = item;
		}

		// Token: 0x17000895 RID: 2197
		// (get) Token: 0x06001564 RID: 5476 RVA: 0x000535E4 File Offset: 0x000517E4
		public ContextMenuItemModel SelectedItem { get; }
	}
}
