using System;

namespace pdfeditor.Models.Menus.ToolbarSettings
{
	// Token: 0x02000188 RID: 392
	public class ToolBarSettingTextBlock : ToolbarSettingItemModel
	{
		// Token: 0x06001669 RID: 5737 RVA: 0x0005599F File Offset: 0x00053B9F
		public ToolBarSettingTextBlock()
			: this(null)
		{
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x000559A8 File Offset: 0x00053BA8
		public ToolBarSettingTextBlock(string text)
			: base(ContextMenuItemType.None)
		{
			if (!string.IsNullOrEmpty(text))
			{
				this.text = text;
				return;
			}
			this.text = "You are in editing mode.";
		}

		// Token: 0x170008DC RID: 2268
		// (get) Token: 0x0600166B RID: 5739 RVA: 0x000559CC File Offset: 0x00053BCC
		// (set) Token: 0x0600166C RID: 5740 RVA: 0x000559D4 File Offset: 0x00053BD4
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

		// Token: 0x04000779 RID: 1913
		private string text;
	}
}
