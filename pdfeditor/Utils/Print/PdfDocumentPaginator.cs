using System;
using System.Drawing.Imaging;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;

namespace pdfeditor.Utils.Print
{
	// Token: 0x020000BF RID: 191
	internal class PdfDocumentPaginator : DocumentPaginator, IDocumentPaginatorSource
	{
		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000B44 RID: 2884 RVA: 0x00039D98 File Offset: 0x00037F98
		// (remove) Token: 0x06000B45 RID: 2885 RVA: 0x00039DD0 File Offset: 0x00037FD0
		public event EventHandler<PagePrintedEventArgs> PagePrinted;

		// Token: 0x06000B46 RID: 2886 RVA: 0x00039E05 File Offset: 0x00038005
		public PdfDocumentPaginator(PdfDocument document, PageRange pageRange)
		{
			this._doc = document;
			this._pageCount = pageRange.PageTo - pageRange.PageFrom + 1;
			this._pageRange = pageRange;
		}

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06000B47 RID: 2887 RVA: 0x00039E39 File Offset: 0x00038039
		public DocumentPaginator DocumentPaginator
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06000B48 RID: 2888 RVA: 0x00039E3C File Offset: 0x0003803C
		public override bool IsPageCountValid
		{
			get
			{
				return this._isValidPageCount;
			}
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06000B49 RID: 2889 RVA: 0x00039E44 File Offset: 0x00038044
		public override int PageCount
		{
			get
			{
				return this._pageCount;
			}
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06000B4A RID: 2890 RVA: 0x00039E4C File Offset: 0x0003804C
		// (set) Token: 0x06000B4B RID: 2891 RVA: 0x00039E54 File Offset: 0x00038054
		public override Size PageSize { get; set; }

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06000B4C RID: 2892 RVA: 0x00039E5D File Offset: 0x0003805D
		public override IDocumentPaginatorSource Source
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06000B4D RID: 2893 RVA: 0x00039E60 File Offset: 0x00038060
		// (set) Token: 0x06000B4E RID: 2894 RVA: 0x00039E68 File Offset: 0x00038068
		public PrintTicket PrinterTicket { get; set; }

		// Token: 0x06000B4F RID: 2895 RVA: 0x00039E74 File Offset: 0x00038074
		public override DocumentPage GetPage(int pageNumber)
		{
			pageNumber = pageNumber + this._pageRange.PageFrom - 1;
			if (this._prevPage != null)
			{
				this._prevPage.Dispose();
			}
			double num = (double)this._doc.Pages[pageNumber].Width;
			double num2 = (double)this._doc.Pages[pageNumber].Height;
			if (this.PageRotation(this._doc.Pages[pageNumber]) == PageRotate.Rotate270 || this.PageRotation(this._doc.Pages[pageNumber]) == PageRotate.Rotate90)
			{
				this.PrinterTicket.PageOrientation = new PageOrientation?(PageOrientation.ReverseLandscape);
			}
			DrawingVisual drawingVisual = new DrawingVisual();
			DocumentPage documentPage = new DocumentPage(drawingVisual);
			documentPage.PageDestroyed += this.Page_PageDestroyed;
			this.RenderPage(pageNumber, drawingVisual);
			this._prevPage = documentPage;
			if (this.PagePrinted != null)
			{
				PagePrintedEventArgs pagePrintedEventArgs = new PagePrintedEventArgs(pageNumber - this._pageRange.PageFrom + 1, this._pageCount);
				this.PagePrinted(this, pagePrintedEventArgs);
				if (pagePrintedEventArgs.Cancel)
				{
					this._pageCount = 0;
				}
			}
			return documentPage;
		}

		// Token: 0x06000B50 RID: 2896 RVA: 0x00039F90 File Offset: 0x00038190
		private PageRotate PageRotation(PdfPage pdfPage)
		{
			int num = pdfPage.Rotation - pdfPage.OriginalRotation;
			if (num < 0)
			{
				num = 4 + num;
			}
			return (PageRotate)num;
		}

		// Token: 0x06000B51 RID: 2897 RVA: 0x00039FB4 File Offset: 0x000381B4
		private Size GetRenderSize(Size pageSize, Size fitSize)
		{
			double width = pageSize.Width;
			double height = pageSize.Height;
			double num = fitSize.Height;
			double num2 = width * num / height;
			if (num2 > fitSize.Width)
			{
				num2 = fitSize.Width;
				num = height * num2 / width;
			}
			return new Size(num2, num);
		}

		// Token: 0x06000B52 RID: 2898 RVA: 0x00039FFE File Offset: 0x000381FE
		private void Page_PageDestroyed(object sender, EventArgs e)
		{
			if (this._mem != null)
			{
				this._mem.Close();
			}
		}

		// Token: 0x06000B53 RID: 2899 RVA: 0x0003A014 File Offset: 0x00038214
		private void RenderPage(int pageNumber, DrawingVisual visual)
		{
			int valueOrDefault = this.PrinterTicket.PageResolution.X.GetValueOrDefault(96);
			int valueOrDefault2 = this.PrinterTicket.PageResolution.Y.GetValueOrDefault(96);
			Size size = new Size
			{
				Width = this.PageSize.Width / 96.0,
				Height = this.PageSize.Height / 96.0
			};
			Size size2 = new Size
			{
				Width = (double)(this._doc.Pages[pageNumber].Width / 72f),
				Height = (double)(this._doc.Pages[pageNumber].Height / 72f)
			};
			if (this._doc.Pages[pageNumber].OriginalRotation == PageRotate.Rotate270 || this._doc.Pages[pageNumber].OriginalRotation == PageRotate.Rotate90)
			{
				size = new Size(size.Height, size.Width);
			}
			Size renderSize = this.GetRenderSize(size2, size);
			int num = (int)(renderSize.Width * (double)valueOrDefault);
			int num2 = (int)(renderSize.Height * (double)valueOrDefault2);
			using (PdfBitmap pdfBitmap = new PdfBitmap(num, num2, true, false))
			{
				this._doc.Pages[pageNumber].RenderEx(pdfBitmap, 0, 0, num, num2, PageRotate.Normal, RenderFlags.FPDF_ANNOT | RenderFlags.FPDF_PRINTING);
				PdfBitmap pdfBitmap2 = null;
				if (this.PageRotation(this._doc.Pages[pageNumber]) == PageRotate.Rotate270)
				{
					pdfBitmap2 = pdfBitmap.SwapXY(false, true, null);
				}
				else if (this.PageRotation(this._doc.Pages[pageNumber]) == PageRotate.Rotate180)
				{
					pdfBitmap2 = pdfBitmap.FlipXY(true, true);
				}
				else if (this.PageRotation(this._doc.Pages[pageNumber]) == PageRotate.Rotate90)
				{
					pdfBitmap2 = pdfBitmap.SwapXY(true, false, null);
				}
				if (pdfBitmap2 != null)
				{
					int stride = pdfBitmap2.Stride;
				}
				else
				{
					int stride2 = pdfBitmap.Stride;
				}
				if (pdfBitmap2 != null)
				{
					int width = pdfBitmap2.Width;
				}
				else
				{
					int width2 = pdfBitmap.Width;
				}
				if (pdfBitmap2 != null)
				{
					int height = pdfBitmap2.Height;
				}
				else
				{
					int height2 = pdfBitmap.Height;
				}
				if (pdfBitmap2 != null)
				{
					IntPtr buffer = pdfBitmap2.Buffer;
				}
				else
				{
					IntPtr buffer2 = pdfBitmap.Buffer;
				}
				BitmapSource bitmapSource = this.CreateImageSource(pdfBitmap2 ?? pdfBitmap);
				if (pdfBitmap2 != null)
				{
					pdfBitmap2.Dispose();
				}
				DrawingContext drawingContext = visual.RenderOpen();
				drawingContext.DrawImage(bitmapSource, new Rect(0.0, 0.0, (double)bitmapSource.PixelWidth / ((double)valueOrDefault / 96.0), bitmapSource.Height / ((double)valueOrDefault2 / 90.0)));
				drawingContext.Close();
			}
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x0003A300 File Offset: 0x00038500
		private BitmapSource CreateImageSource(PdfBitmap bmp)
		{
			this._mem = new MemoryStream();
			bmp.Image.Save(this._mem, ImageFormat.Png);
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.None;
			bitmapImage.StreamSource = this._mem;
			bitmapImage.EndInit();
			bitmapImage.Freeze();
			return bitmapImage;
		}

		// Token: 0x040004DC RID: 1244
		private PdfDocument _doc;

		// Token: 0x040004DD RID: 1245
		private DocumentPage _prevPage;

		// Token: 0x040004DE RID: 1246
		private MemoryStream _mem;

		// Token: 0x040004DF RID: 1247
		private bool _isValidPageCount = true;

		// Token: 0x040004E0 RID: 1248
		private int _pageCount;

		// Token: 0x040004E1 RID: 1249
		private PageRange _pageRange;
	}
}
