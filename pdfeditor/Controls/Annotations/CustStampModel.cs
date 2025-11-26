using System;
using System.Windows.Media.Imaging;
using Patagames.Pdf;
using pdfeditor.ViewModels;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002AD RID: 685
	public class CustStampModel
	{
		// Token: 0x17000C1E RID: 3102
		// (get) Token: 0x06002784 RID: 10116 RVA: 0x000BA552 File Offset: 0x000B8752
		// (set) Token: 0x06002785 RID: 10117 RVA: 0x000BA55A File Offset: 0x000B875A
		public string ImageFilePath { get; set; }

		// Token: 0x17000C1F RID: 3103
		// (get) Token: 0x06002786 RID: 10118 RVA: 0x000BA563 File Offset: 0x000B8763
		// (set) Token: 0x06002787 RID: 10119 RVA: 0x000BA56B File Offset: 0x000B876B
		public BitmapSource StampImageSource { get; set; }

		// Token: 0x17000C20 RID: 3104
		// (get) Token: 0x06002788 RID: 10120 RVA: 0x000BA574 File Offset: 0x000B8774
		// (set) Token: 0x06002789 RID: 10121 RVA: 0x000BA57C File Offset: 0x000B877C
		public double ImageWidth { get; set; }

		// Token: 0x17000C21 RID: 3105
		// (get) Token: 0x0600278A RID: 10122 RVA: 0x000BA585 File Offset: 0x000B8785
		// (set) Token: 0x0600278B RID: 10123 RVA: 0x000BA58D File Offset: 0x000B878D
		public double ImageHeight { get; set; }

		// Token: 0x17000C22 RID: 3106
		// (get) Token: 0x0600278C RID: 10124 RVA: 0x000BA596 File Offset: 0x000B8796
		// (set) Token: 0x0600278D RID: 10125 RVA: 0x000BA59E File Offset: 0x000B879E
		public FS_SIZEF PageSize { get; set; }

		// Token: 0x17000C23 RID: 3107
		// (get) Token: 0x0600278E RID: 10126 RVA: 0x000BA5A7 File Offset: 0x000B87A7
		// (set) Token: 0x0600278F RID: 10127 RVA: 0x000BA5AF File Offset: 0x000B87AF
		public string Text { get; set; }

		// Token: 0x17000C24 RID: 3108
		// (get) Token: 0x06002790 RID: 10128 RVA: 0x000BA5B8 File Offset: 0x000B87B8
		// (set) Token: 0x06002791 RID: 10129 RVA: 0x000BA5C0 File Offset: 0x000B87C0
		public string Image { get; set; }

		// Token: 0x17000C25 RID: 3109
		// (get) Token: 0x06002792 RID: 10130 RVA: 0x000BA5C9 File Offset: 0x000B87C9
		// (set) Token: 0x06002793 RID: 10131 RVA: 0x000BA5D1 File Offset: 0x000B87D1
		public string TextContent { get; set; }

		// Token: 0x17000C26 RID: 3110
		// (get) Token: 0x06002794 RID: 10132 RVA: 0x000BA5DA File Offset: 0x000B87DA
		// (set) Token: 0x06002795 RID: 10133 RVA: 0x000BA5E2 File Offset: 0x000B87E2
		public string FontColor { get; set; }

		// Token: 0x17000C27 RID: 3111
		// (get) Token: 0x06002796 RID: 10134 RVA: 0x000BA5EB File Offset: 0x000B87EB
		// (set) Token: 0x06002797 RID: 10135 RVA: 0x000BA5F3 File Offset: 0x000B87F3
		public string GroupId { get; set; }

		// Token: 0x17000C28 RID: 3112
		// (get) Token: 0x06002798 RID: 10136 RVA: 0x000BA5FC File Offset: 0x000B87FC
		// (set) Token: 0x06002799 RID: 10137 RVA: 0x000BA604 File Offset: 0x000B8804
		public DateTime dateTime { get; set; }

		// Token: 0x17000C29 RID: 3113
		// (get) Token: 0x0600279A RID: 10138 RVA: 0x000BA60D File Offset: 0x000B880D
		// (set) Token: 0x0600279B RID: 10139 RVA: 0x000BA615 File Offset: 0x000B8815
		public string TimeFormat { get; set; }

		// Token: 0x17000C2A RID: 3114
		// (get) Token: 0x0600279C RID: 10140 RVA: 0x000BA61E File Offset: 0x000B881E
		// (set) Token: 0x0600279D RID: 10141 RVA: 0x000BA626 File Offset: 0x000B8826
		public DynamicStampTextModel DynamicStampTextModel { get; set; }
	}
}
