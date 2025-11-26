using System;
using Newtonsoft.Json;

namespace pdfeditor.Models.Scan
{
	// Token: 0x02000141 RID: 321
	public class ScanSettings
	{
		// Token: 0x170007D6 RID: 2006
		// (get) Token: 0x0600136D RID: 4973 RVA: 0x0004EC47 File Offset: 0x0004CE47
		// (set) Token: 0x0600136E RID: 4974 RVA: 0x0004EC4F File Offset: 0x0004CE4F
		[JsonIgnore]
		public ScannerDeviceInfo Device { get; set; }

		// Token: 0x170007D7 RID: 2007
		// (get) Token: 0x0600136F RID: 4975 RVA: 0x0004EC58 File Offset: 0x0004CE58
		// (set) Token: 0x06001370 RID: 4976 RVA: 0x0004EC60 File Offset: 0x0004CE60
		public ScanSource Source { get; set; }

		// Token: 0x170007D8 RID: 2008
		// (get) Token: 0x06001371 RID: 4977 RVA: 0x0004EC69 File Offset: 0x0004CE69
		// (set) Token: 0x06001372 RID: 4978 RVA: 0x0004EC71 File Offset: 0x0004CE71
		public ScanColor Color { get; set; } = ScanColor.Color;

		// Token: 0x170007D9 RID: 2009
		// (get) Token: 0x06001373 RID: 4979 RVA: 0x0004EC7A File Offset: 0x0004CE7A
		// (set) Token: 0x06001374 RID: 4980 RVA: 0x0004EC82 File Offset: 0x0004CE82
		public ScanResolution Resolution { get; set; } = ScanResolution.Dpi300;

		// Token: 0x170007DA RID: 2010
		// (get) Token: 0x06001375 RID: 4981 RVA: 0x0004EC8B File Offset: 0x0004CE8B
		// (set) Token: 0x06001376 RID: 4982 RVA: 0x0004EC93 File Offset: 0x0004CE93
		public ScanArea Area { get; set; }

		// Token: 0x170007DB RID: 2011
		// (get) Token: 0x06001377 RID: 4983 RVA: 0x0004EC9C File Offset: 0x0004CE9C
		// (set) Token: 0x06001378 RID: 4984 RVA: 0x0004ECA4 File Offset: 0x0004CEA4
		public int Brightness { get; set; }

		// Token: 0x170007DC RID: 2012
		// (get) Token: 0x06001379 RID: 4985 RVA: 0x0004ECAD File Offset: 0x0004CEAD
		// (set) Token: 0x0600137A RID: 4986 RVA: 0x0004ECB5 File Offset: 0x0004CEB5
		public int Contrast { get; set; }

		// Token: 0x170007DD RID: 2013
		// (get) Token: 0x0600137B RID: 4987 RVA: 0x0004ECBE File Offset: 0x0004CEBE
		// (set) Token: 0x0600137C RID: 4988 RVA: 0x0004ECC6 File Offset: 0x0004CEC6
		public int Saturation { get; set; }
	}
}
