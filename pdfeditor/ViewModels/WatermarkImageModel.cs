using System;
using System.Windows.Media.Imaging;
using Patagames.Pdf;

namespace pdfeditor.ViewModels
{
	// Token: 0x0200005C RID: 92
	public class WatermarkImageModel
	{
		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x06000504 RID: 1284 RVA: 0x00019BDA File Offset: 0x00017DDA
		// (set) Token: 0x06000505 RID: 1285 RVA: 0x00019BE2 File Offset: 0x00017DE2
		public string ImageFilePath { get; set; }

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000506 RID: 1286 RVA: 0x00019BEB File Offset: 0x00017DEB
		// (set) Token: 0x06000507 RID: 1287 RVA: 0x00019BF3 File Offset: 0x00017DF3
		public BitmapSource WatermarkImageSource { get; set; }

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000508 RID: 1288 RVA: 0x00019BFC File Offset: 0x00017DFC
		// (set) Token: 0x06000509 RID: 1289 RVA: 0x00019C04 File Offset: 0x00017E04
		public double ImageWidth { get; set; }

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x0600050A RID: 1290 RVA: 0x00019C0D File Offset: 0x00017E0D
		// (set) Token: 0x0600050B RID: 1291 RVA: 0x00019C15 File Offset: 0x00017E15
		public double ImageHeight { get; set; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x0600050C RID: 1292 RVA: 0x00019C1E File Offset: 0x00017E1E
		// (set) Token: 0x0600050D RID: 1293 RVA: 0x00019C26 File Offset: 0x00017E26
		public FS_SIZEF PageSize { get; set; }
	}
}
