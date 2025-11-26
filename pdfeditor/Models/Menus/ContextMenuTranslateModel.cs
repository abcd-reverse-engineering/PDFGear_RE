using System;

namespace pdfeditor.Models.Menus
{
	// Token: 0x02000159 RID: 345
	public class ContextMenuTranslateModel : ContextMenuItemModel
	{
		// Token: 0x1700084F RID: 2127
		// (get) Token: 0x06001497 RID: 5271 RVA: 0x00051765 File Offset: 0x0004F965
		// (set) Token: 0x06001498 RID: 5272 RVA: 0x0005176D File Offset: 0x0004F96D
		public string TranslatedText
		{
			get
			{
				return this._translatedText;
			}
			set
			{
				base.SetProperty<string>(ref this._translatedText, value, "TranslatedText");
			}
		}

		// Token: 0x17000850 RID: 2128
		// (get) Token: 0x06001499 RID: 5273 RVA: 0x00051782 File Offset: 0x0004F982
		// (set) Token: 0x0600149A RID: 5274 RVA: 0x0005178A File Offset: 0x0004F98A
		public bool Translating
		{
			get
			{
				return this.translating;
			}
			set
			{
				base.SetProperty<bool>(ref this.translating, value, "Translating");
			}
		}

		// Token: 0x17000851 RID: 2129
		// (get) Token: 0x0600149B RID: 5275 RVA: 0x0005179F File Offset: 0x0004F99F
		// (set) Token: 0x0600149C RID: 5276 RVA: 0x000517A7 File Offset: 0x0004F9A7
		public override bool IsChecked
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

		// Token: 0x040006CC RID: 1740
		private string _translatedText;

		// Token: 0x040006CD RID: 1741
		private bool isChecked;

		// Token: 0x040006CE RID: 1742
		private bool translating;
	}
}
