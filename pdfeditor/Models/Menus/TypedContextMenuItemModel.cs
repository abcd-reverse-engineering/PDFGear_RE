using System;

namespace pdfeditor.Models.Menus
{
	// Token: 0x0200016B RID: 363
	public class TypedContextMenuItemModel : SelectableContextMenuItemModel
	{
		// Token: 0x060015C2 RID: 5570 RVA: 0x000542B0 File Offset: 0x000524B0
		public TypedContextMenuItemModel(ContextMenuItemType type)
		{
			this.Type = type;
		}

		// Token: 0x170008B7 RID: 2231
		// (get) Token: 0x060015C3 RID: 5571 RVA: 0x000542BF File Offset: 0x000524BF
		public ContextMenuItemType Type { get; }
	}
}
