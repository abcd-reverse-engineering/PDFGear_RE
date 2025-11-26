using System;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000169 RID: 361
	public class SelectedAccessorSelectionChangedEventArgs : EventArgs
	{
		// Token: 0x060015B0 RID: 5552 RVA: 0x00053F7D File Offset: 0x0005217D
		public SelectedAccessorSelectionChangedEventArgs(ContextMenuItemModel oldItem, ContextMenuItemModel newItem, ContextMenuItemType type)
		{
			this.OldItem = oldItem;
			this.NewItem = newItem;
			this.Type = type;
		}

		// Token: 0x170008AE RID: 2222
		// (get) Token: 0x060015B1 RID: 5553 RVA: 0x00053F9A File Offset: 0x0005219A
		public ContextMenuItemType Type { get; }

		// Token: 0x170008AF RID: 2223
		// (get) Token: 0x060015B2 RID: 5554 RVA: 0x00053FA2 File Offset: 0x000521A2
		public ContextMenuItemModel OldItem { get; }

		// Token: 0x170008B0 RID: 2224
		// (get) Token: 0x060015B3 RID: 5555 RVA: 0x00053FAA File Offset: 0x000521AA
		public ContextMenuItemModel NewItem { get; }
	}
}
