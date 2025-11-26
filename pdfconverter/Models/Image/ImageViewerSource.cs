using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using pdfconverter.Utils;

namespace pdfconverter.Models.Image
{
	// Token: 0x0200008B RID: 139
	public class ImageViewerSource
	{
		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06000680 RID: 1664 RVA: 0x00017B16 File Offset: 0x00015D16
		// (set) Token: 0x06000681 RID: 1665 RVA: 0x00017B1E File Offset: 0x00015D1E
		public Bitmap OriginalImage { get; private set; }

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x06000682 RID: 1666 RVA: 0x00017B27 File Offset: 0x00015D27
		// (set) Token: 0x06000683 RID: 1667 RVA: 0x00017B2F File Offset: 0x00015D2F
		public BitmapFrame Image { get; private set; }

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x06000684 RID: 1668 RVA: 0x00017B38 File Offset: 0x00015D38
		// (set) Token: 0x06000685 RID: 1669 RVA: 0x00017B40 File Offset: 0x00015D40
		public double Rotate { get; set; }

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000686 RID: 1670 RVA: 0x00017B49 File Offset: 0x00015D49
		// (set) Token: 0x06000687 RID: 1671 RVA: 0x00017B51 File Offset: 0x00015D51
		public ImageSource ImageSource { get; private set; }

		// Token: 0x06000688 RID: 1672 RVA: 0x00017B5A File Offset: 0x00015D5A
		public ImageViewerSource(Bitmap bitmap, double rotate)
		{
			this.OriginalImage = bitmap;
			this.Image = ImageHelper.ToBitmapFrame(bitmap);
			this.Rotate = rotate;
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x00017B7C File Offset: 0x00015D7C
		public ImageViewerSource(ImageSource bitmap, double rotate)
		{
			this.ImageSource = bitmap;
			this.Rotate = rotate;
		}
	}
}
