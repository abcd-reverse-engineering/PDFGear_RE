using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using pdfeditor.Utils.Scan;

namespace pdfeditor.Models.Scan
{
	// Token: 0x0200013D RID: 317
	public class ImageViewerSource
	{
		// Token: 0x170007C1 RID: 1985
		// (get) Token: 0x06001334 RID: 4916 RVA: 0x0004E6E6 File Offset: 0x0004C8E6
		// (set) Token: 0x06001335 RID: 4917 RVA: 0x0004E6EE File Offset: 0x0004C8EE
		public Bitmap OriginalImage { get; private set; }

		// Token: 0x170007C2 RID: 1986
		// (get) Token: 0x06001336 RID: 4918 RVA: 0x0004E6F7 File Offset: 0x0004C8F7
		// (set) Token: 0x06001337 RID: 4919 RVA: 0x0004E6FF File Offset: 0x0004C8FF
		public BitmapFrame Image { get; private set; }

		// Token: 0x170007C3 RID: 1987
		// (get) Token: 0x06001338 RID: 4920 RVA: 0x0004E708 File Offset: 0x0004C908
		// (set) Token: 0x06001339 RID: 4921 RVA: 0x0004E710 File Offset: 0x0004C910
		public double Rotate { get; set; }

		// Token: 0x0600133A RID: 4922 RVA: 0x0004E719 File Offset: 0x0004C919
		public ImageViewerSource(Bitmap bitmap, double rotate)
		{
			this.OriginalImage = bitmap;
			this.Image = ScannerImageHelper.ToBitmapFrame(bitmap);
			this.Rotate = rotate;
		}
	}
}
