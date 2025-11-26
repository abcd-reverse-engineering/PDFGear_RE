using System;
using CommunityToolkit.Mvvm.Input;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x0200017A RID: 378
	public class ToolbarSettingItemApplyToDefaultModel : ToolbarSettingItemModel
	{
		// Token: 0x060015F0 RID: 5616 RVA: 0x00054641 File Offset: 0x00052841
		public ToolbarSettingItemApplyToDefaultModel()
			: base(ContextMenuItemType.None)
		{
			this.InitCommand();
		}

		// Token: 0x060015F1 RID: 5617 RVA: 0x00054650 File Offset: 0x00052850
		private void InitCommand()
		{
			base.Command = new RelayCommand(delegate
			{
				if (!base.InTransientScope)
				{
					return;
				}
				ToolbarSettingModel parent = base.Parent;
				if (parent == null)
				{
					return;
				}
				foreach (ToolbarSettingItemModel toolbarSettingItemModel in parent)
				{
					toolbarSettingItemModel.ApplyTransient(false);
				}
			});
		}
	}
}
