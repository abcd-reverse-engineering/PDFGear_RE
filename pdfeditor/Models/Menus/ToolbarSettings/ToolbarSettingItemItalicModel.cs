using System;
using System.Collections.Generic;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x02000183 RID: 387
	internal class ToolbarSettingItemItalicModel : ToolbarSettingItemModel
	{
		// Token: 0x0600161E RID: 5662 RVA: 0x00054F1D File Offset: 0x0005311D
		public ToolbarSettingItemItalicModel(ContextMenuItemType type)
			: base(type)
		{
		}

		// Token: 0x0600161F RID: 5663 RVA: 0x00054F26 File Offset: 0x00053126
		public ToolbarSettingItemItalicModel(ContextMenuItemType type, string configCacheKey)
			: base(type, configCacheKey)
		{
		}

		// Token: 0x06001620 RID: 5664 RVA: 0x00054F30 File Offset: 0x00053130
		protected override void SaveConfigCore(Dictionary<string, string> dict)
		{
			base.SaveConfigCore(dict);
			string text = "SelectedValue";
			object nontransientSelectedValue = base.NontransientSelectedValue;
			dict[text] = ((nontransientSelectedValue is bool && (bool)nontransientSelectedValue) ? "true" : "false");
		}

		// Token: 0x06001621 RID: 5665 RVA: 0x00054F74 File Offset: 0x00053174
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
