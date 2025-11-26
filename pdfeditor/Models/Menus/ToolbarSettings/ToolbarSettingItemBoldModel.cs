using System;
using System.Collections.Generic;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x0200017B RID: 379
	internal class ToolbarSettingItemBoldModel : ToolbarSettingItemModel
	{
		// Token: 0x060015F3 RID: 5619 RVA: 0x000546C8 File Offset: 0x000528C8
		public ToolbarSettingItemBoldModel(ContextMenuItemType type)
			: base(type)
		{
		}

		// Token: 0x060015F4 RID: 5620 RVA: 0x000546D1 File Offset: 0x000528D1
		public ToolbarSettingItemBoldModel(ContextMenuItemType type, string configCacheKey)
			: base(type, configCacheKey)
		{
		}

		// Token: 0x060015F5 RID: 5621 RVA: 0x000546DC File Offset: 0x000528DC
		protected override void SaveConfigCore(Dictionary<string, string> dict)
		{
			base.SaveConfigCore(dict);
			string text = "SelectedValue";
			object nontransientSelectedValue = base.NontransientSelectedValue;
			dict[text] = ((nontransientSelectedValue is bool && (bool)nontransientSelectedValue) ? "true" : "false");
		}

		// Token: 0x060015F6 RID: 5622 RVA: 0x00054720 File Offset: 0x00052920
		protected override void ApplyConfigCore(Dictionary<string, string> dict)
		{
			base.ApplyConfigCore(dict);
			string text;
			if (dict.TryGetValue("SelectedValue", out text))
			{
				base.NontransientSelectedValue = text == "true";
			}
		}
	}
}
