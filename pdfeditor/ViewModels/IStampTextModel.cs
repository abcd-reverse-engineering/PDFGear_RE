using System;

namespace pdfeditor.ViewModels
{
	// Token: 0x02000055 RID: 85
	public interface IStampTextModel
	{
		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060004D0 RID: 1232
		bool IsPreset { get; }

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060004D1 RID: 1233
		string TextContent { get; }

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060004D2 RID: 1234
		string FontColor { get; }

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060004D3 RID: 1235
		string GroupId { get; }
	}
}
