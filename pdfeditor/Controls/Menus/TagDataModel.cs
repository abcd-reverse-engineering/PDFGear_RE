using System;
using pdfeditor.Models.Menus;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Menus
{
	// Token: 0x0200026C RID: 620
	public class TagDataModel
	{
		// Token: 0x06002414 RID: 9236 RVA: 0x000A8402 File Offset: 0x000A6602
		public TagDataModel()
		{
		}

		// Token: 0x06002415 RID: 9237 RVA: 0x000A840A File Offset: 0x000A660A
		public TagDataModel(bool isTransient)
		{
			this.IsTransient = isTransient;
		}

		// Token: 0x17000B7A RID: 2938
		// (get) Token: 0x06002416 RID: 9238 RVA: 0x000A8419 File Offset: 0x000A6619
		// (set) Token: 0x06002417 RID: 9239 RVA: 0x000A8421 File Offset: 0x000A6621
		public ContextMenuItemType MenuItemType { get; set; }

		// Token: 0x17000B7B RID: 2939
		// (get) Token: 0x06002418 RID: 9240 RVA: 0x000A842A File Offset: 0x000A662A
		// (set) Token: 0x06002419 RID: 9241 RVA: 0x000A8432 File Offset: 0x000A6632
		public object MenuItemValue { get; set; }

		// Token: 0x17000B7C RID: 2940
		// (get) Token: 0x0600241A RID: 9242 RVA: 0x000A843B File Offset: 0x000A663B
		// (set) Token: 0x0600241B RID: 9243 RVA: 0x000A8443 File Offset: 0x000A6643
		public AnnotationMode AnnotationMode { get; set; }

		// Token: 0x17000B7D RID: 2941
		// (get) Token: 0x0600241C RID: 9244 RVA: 0x000A844C File Offset: 0x000A664C
		public bool IsTransient { get; }
	}
}
