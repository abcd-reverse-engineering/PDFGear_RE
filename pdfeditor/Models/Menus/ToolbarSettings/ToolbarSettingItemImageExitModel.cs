using System;
using pdfeditor.Properties;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x02000180 RID: 384
	public class ToolbarSettingItemImageExitModel : ToolbarSettingItemModel
	{
		// Token: 0x06001614 RID: 5652 RVA: 0x00054E0D File Offset: 0x0005300D
		public ToolbarSettingItemImageExitModel()
			: this(null)
		{
		}

		// Token: 0x06001615 RID: 5653 RVA: 0x00054E16 File Offset: 0x00053016
		public ToolbarSettingItemImageExitModel(string text)
			: base(ContextMenuItemType.None)
		{
			if (!string.IsNullOrEmpty(text))
			{
				this.text = text;
				return;
			}
			this.text = Resources.ToolbarExitEditButtonContent;
		}

		// Token: 0x170008CB RID: 2251
		// (get) Token: 0x06001616 RID: 5654 RVA: 0x00054E3A File Offset: 0x0005303A
		// (set) Token: 0x06001617 RID: 5655 RVA: 0x00054E42 File Offset: 0x00053042
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				base.SetProperty<string>(ref this.text, value, "Text");
			}
		}

		// Token: 0x04000761 RID: 1889
		private string text;
	}
}
