using System;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x02000179 RID: 377
	public class ToolbarSettingInkEraserModeModel : ToolbarSettingItemModel
	{
		// Token: 0x060015EB RID: 5611 RVA: 0x000545E4 File Offset: 0x000527E4
		public ToolbarSettingInkEraserModeModel()
			: base(ContextMenuItemType.None)
		{
		}

		// Token: 0x170008C4 RID: 2244
		// (get) Token: 0x060015EC RID: 5612 RVA: 0x000545F4 File Offset: 0x000527F4
		// (set) Token: 0x060015ED RID: 5613 RVA: 0x000545FC File Offset: 0x000527FC
		public bool IsCheckable
		{
			get
			{
				return this.isCheckable;
			}
			set
			{
				if (base.SetProperty<bool>(ref this.isCheckable, value, "IsCheckable") && !value && this.IsChecked)
				{
					this.IsChecked = false;
				}
			}
		}

		// Token: 0x170008C5 RID: 2245
		// (get) Token: 0x060015EE RID: 5614 RVA: 0x00054624 File Offset: 0x00052824
		// (set) Token: 0x060015EF RID: 5615 RVA: 0x0005462C File Offset: 0x0005282C
		public bool IsChecked
		{
			get
			{
				return this.isChecked;
			}
			set
			{
				base.SetProperty<bool>(ref this.isChecked, value, "IsChecked");
			}
		}

		// Token: 0x04000757 RID: 1879
		private bool isCheckable = true;

		// Token: 0x04000758 RID: 1880
		private bool isChecked;
	}
}
