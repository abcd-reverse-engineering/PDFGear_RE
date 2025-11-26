using System;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000158 RID: 344
	public class ContextMenuModel : ContextMenuItemModel
	{
		// Token: 0x1700084D RID: 2125
		// (get) Token: 0x06001493 RID: 5267 RVA: 0x0005174B File Offset: 0x0004F94B
		// (set) Token: 0x06001494 RID: 5268 RVA: 0x0005174E File Offset: 0x0004F94E
		public override IContextMenuModel Parent
		{
			get
			{
				return null;
			}
			set
			{
				throw new ArgumentException("ContextMenuModel");
			}
		}

		// Token: 0x1700084E RID: 2126
		// (get) Token: 0x06001495 RID: 5269 RVA: 0x0005175A File Offset: 0x0004F95A
		public override int Level
		{
			get
			{
				return 0;
			}
		}
	}
}
