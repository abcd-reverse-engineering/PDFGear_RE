using System;
using Patagames.Pdf.Enums;
using pdfeditor.Utils;

namespace pdfeditor.ViewModels
{
	// Token: 0x02000057 RID: 87
	public class PresetStampTextModel : IStampTextModel
	{
		// Token: 0x060004E4 RID: 1252 RVA: 0x00019A85 File Offset: 0x00017C85
		public PresetStampTextModel(StampIconNames stampIconName, string fontColor, string groupId)
		{
			if (this.IconName == StampIconNames.Extended)
			{
				throw new ArgumentException("stampIconName");
			}
			this.IconName = stampIconName;
			this.TextContent = ToolbarContextMenuHelper.GetPresetStampTextContext(stampIconName);
			this.FontColor = fontColor;
			this.GroupId = groupId;
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060004E5 RID: 1253 RVA: 0x00019AC3 File Offset: 0x00017CC3
		public StampIconNames IconName { get; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060004E6 RID: 1254 RVA: 0x00019ACB File Offset: 0x00017CCB
		public string TextContent { get; }

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060004E7 RID: 1255 RVA: 0x00019AD3 File Offset: 0x00017CD3
		public string FontColor { get; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060004E8 RID: 1256 RVA: 0x00019ADB File Offset: 0x00017CDB
		public string GroupId { get; }

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060004E9 RID: 1257 RVA: 0x00019AE3 File Offset: 0x00017CE3
		public bool IsPreset
		{
			get
			{
				return true;
			}
		}
	}
}
