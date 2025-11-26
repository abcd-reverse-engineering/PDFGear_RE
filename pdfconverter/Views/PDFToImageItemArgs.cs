using System;
using System.Drawing;
using System.Drawing.Imaging;
using PDFKit.ExtractPdfImage;

namespace pdfconverter.Views
{
	// Token: 0x0200002C RID: 44
	internal class PDFToImageItemArgs
	{
		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000276 RID: 630 RVA: 0x0000A8B8 File Offset: 0x00008AB8
		// (set) Token: 0x06000277 RID: 631 RVA: 0x0000A8C0 File Offset: 0x00008AC0
		public ImageFormat OutputFormat { get; set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000278 RID: 632 RVA: 0x0000A8C9 File Offset: 0x00008AC9
		// (set) Token: 0x06000279 RID: 633 RVA: 0x0000A8D1 File Offset: 0x00008AD1
		public float DPI { get; set; }

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x0600027A RID: 634 RVA: 0x0000A8DA File Offset: 0x00008ADA
		// (set) Token: 0x0600027B RID: 635 RVA: 0x0000A8E2 File Offset: 0x00008AE2
		public bool IsEntire { get; set; }

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x0600027C RID: 636 RVA: 0x0000A8EB File Offset: 0x00008AEB
		// (set) Token: 0x0600027D RID: 637 RVA: 0x0000A8F3 File Offset: 0x00008AF3
		public Color BorderColor { get; set; }

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x0600027E RID: 638 RVA: 0x0000A8FC File Offset: 0x00008AFC
		// (set) Token: 0x0600027F RID: 639 RVA: 0x0000A904 File Offset: 0x00008B04
		public int BorderThickness { get; set; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000280 RID: 640 RVA: 0x0000A90D File Offset: 0x00008B0D
		// (set) Token: 0x06000281 RID: 641 RVA: 0x0000A915 File Offset: 0x00008B15
		public PdfPageImageExtractColorMode ColorMode { get; set; }

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x06000282 RID: 642 RVA: 0x0000A91E File Offset: 0x00008B1E
		// (set) Token: 0x06000283 RID: 643 RVA: 0x0000A926 File Offset: 0x00008B26
		public bool RenderAnnotations { get; set; }

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000284 RID: 644 RVA: 0x0000A92F File Offset: 0x00008B2F
		// (set) Token: 0x06000285 RID: 645 RVA: 0x0000A937 File Offset: 0x00008B37
		public string OutputPath { get; set; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000286 RID: 646 RVA: 0x0000A940 File Offset: 0x00008B40
		public string FileExtension
		{
			get
			{
				if (this.OutputFormat == ImageFormat.Png)
				{
					return ".png";
				}
				return ".jpeg";
			}
		}
	}
}
