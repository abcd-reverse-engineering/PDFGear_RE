using System;
using System.Windows.Media.Imaging;
using Patagames.Pdf;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002AC RID: 684
	public class ImageStampModel
	{
		// Token: 0x17000C19 RID: 3097
		// (get) Token: 0x06002779 RID: 10105 RVA: 0x000BA4F5 File Offset: 0x000B86F5
		// (set) Token: 0x0600277A RID: 10106 RVA: 0x000BA4FD File Offset: 0x000B86FD
		public string ImageFilePath { get; set; }

		// Token: 0x17000C1A RID: 3098
		// (get) Token: 0x0600277B RID: 10107 RVA: 0x000BA506 File Offset: 0x000B8706
		// (set) Token: 0x0600277C RID: 10108 RVA: 0x000BA50E File Offset: 0x000B870E
		public BitmapSource StampImageSource { get; set; }

		// Token: 0x17000C1B RID: 3099
		// (get) Token: 0x0600277D RID: 10109 RVA: 0x000BA517 File Offset: 0x000B8717
		// (set) Token: 0x0600277E RID: 10110 RVA: 0x000BA51F File Offset: 0x000B871F
		public double ImageWidth { get; set; }

		// Token: 0x17000C1C RID: 3100
		// (get) Token: 0x0600277F RID: 10111 RVA: 0x000BA528 File Offset: 0x000B8728
		// (set) Token: 0x06002780 RID: 10112 RVA: 0x000BA530 File Offset: 0x000B8730
		public double ImageHeight { get; set; }

		// Token: 0x17000C1D RID: 3101
		// (get) Token: 0x06002781 RID: 10113 RVA: 0x000BA539 File Offset: 0x000B8739
		// (set) Token: 0x06002782 RID: 10114 RVA: 0x000BA541 File Offset: 0x000B8741
		public FS_SIZEF PageSize { get; set; }
	}
}
