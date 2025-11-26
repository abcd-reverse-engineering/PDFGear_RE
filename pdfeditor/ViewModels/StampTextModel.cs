using System;

namespace pdfeditor.ViewModels
{
	// Token: 0x02000058 RID: 88
	public class StampTextModel : IStampTextModel
	{
		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060004EA RID: 1258 RVA: 0x00019AE6 File Offset: 0x00017CE6
		// (set) Token: 0x060004EB RID: 1259 RVA: 0x00019AEE File Offset: 0x00017CEE
		public string TextContent { get; set; }

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060004EC RID: 1260 RVA: 0x00019AF7 File Offset: 0x00017CF7
		// (set) Token: 0x060004ED RID: 1261 RVA: 0x00019AFF File Offset: 0x00017CFF
		public string FontColor { get; set; }

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060004EE RID: 1262 RVA: 0x00019B08 File Offset: 0x00017D08
		// (set) Token: 0x060004EF RID: 1263 RVA: 0x00019B10 File Offset: 0x00017D10
		public string GroupId { get; set; }

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060004F0 RID: 1264 RVA: 0x00019B19 File Offset: 0x00017D19
		public bool IsPreset
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060004F1 RID: 1265 RVA: 0x00019B1C File Offset: 0x00017D1C
		// (set) Token: 0x060004F2 RID: 1266 RVA: 0x00019B24 File Offset: 0x00017D24
		public DateTime dateTime { get; set; }
	}
}
