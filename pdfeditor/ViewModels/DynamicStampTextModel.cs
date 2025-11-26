using System;
using pdfeditor.Models.DynamicStamps;
using pdfeditor.Properties;

namespace pdfeditor.ViewModels
{
	// Token: 0x02000056 RID: 86
	public class DynamicStampTextModel : IStampTextModel
	{
		// Token: 0x060004D4 RID: 1236 RVA: 0x000199A8 File Offset: 0x00017BA8
		public DynamicStampTextModel(DynamicStampProperties dynamicProperties, string fontColor, string templateName, string groupId = null)
		{
			this.DynamicProperties = dynamicProperties;
			this.FontColor = fontColor;
			this.TemplateName = templateName;
			if (groupId != null)
			{
				this.GroupId = groupId;
				return;
			}
			this.GroupId = Guid.NewGuid().ToString();
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060004D5 RID: 1237 RVA: 0x00019A0C File Offset: 0x00017C0C
		public DynamicStampProperties DynamicProperties { get; }

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060004D6 RID: 1238 RVA: 0x00019A14 File Offset: 0x00017C14
		// (set) Token: 0x060004D7 RID: 1239 RVA: 0x00019A1C File Offset: 0x00017C1C
		public string TextContent { get; set; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060004D8 RID: 1240 RVA: 0x00019A25 File Offset: 0x00017C25
		// (set) Token: 0x060004D9 RID: 1241 RVA: 0x00019A2D File Offset: 0x00017C2D
		public string FontColor { get; set; }

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060004DA RID: 1242 RVA: 0x00019A36 File Offset: 0x00017C36
		// (set) Token: 0x060004DB RID: 1243 RVA: 0x00019A3E File Offset: 0x00017C3E
		public string GroupId { get; set; }

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060004DC RID: 1244 RVA: 0x00019A47 File Offset: 0x00017C47
		public string TemplateName { get; }

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060004DD RID: 1245 RVA: 0x00019A4F File Offset: 0x00017C4F
		// (set) Token: 0x060004DE RID: 1246 RVA: 0x00019A57 File Offset: 0x00017C57
		public DateTime dateTime { get; set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060004DF RID: 1247 RVA: 0x00019A60 File Offset: 0x00017C60
		public bool IsPreset
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x060004E0 RID: 1248 RVA: 0x00019A63 File Offset: 0x00017C63
		// (set) Token: 0x060004E1 RID: 1249 RVA: 0x00019A6B File Offset: 0x00017C6B
		public string GroupName { get; set; } = Resources.EditStampWinUntitledCategoryName;

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060004E2 RID: 1250 RVA: 0x00019A74 File Offset: 0x00017C74
		// (set) Token: 0x060004E3 RID: 1251 RVA: 0x00019A7C File Offset: 0x00017C7C
		public string Name { get; set; } = Resources.EditStampWinDefaultStampName;
	}
}
