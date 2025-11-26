using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;

namespace pdfeditor.Utils
{
	// Token: 0x0200007D RID: 125
	public static class ImageToPDFBitmapHelper
	{
		// Token: 0x060008DD RID: 2269 RVA: 0x0002C314 File Offset: 0x0002A514
		public static PdfBitmap CreatePdfBitmapFromFile(string fileName, out Bitmap bitmap)
		{
			try
			{
				bitmap = Image.FromFile(fileName, true) as Bitmap;
			}
			catch
			{
				bitmap = null;
			}
			if (bitmap == null)
			{
				return null;
			}
			PdfBitmap pdfBitmap;
			try
			{
				pdfBitmap = PdfBitmap.FromBitmap(bitmap);
			}
			catch
			{
				try
				{
					using (Bitmap bitmap2 = bitmap)
					{
						bitmap = new Bitmap(bitmap2.Width, bitmap2.Height, PixelFormat.Format32bppArgb);
						using (Graphics graphics = Graphics.FromImage(bitmap))
						{
							graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
							graphics.DrawImage(bitmap2, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
						}
						return PdfBitmap.FromBitmap(bitmap);
					}
				}
				catch
				{
				}
				pdfBitmap = null;
			}
			return pdfBitmap;
		}

		// Token: 0x060008DE RID: 2270 RVA: 0x0002C3F8 File Offset: 0x0002A5F8
		public static PdfBitmap CreatePdfBitmapFromFile(Bitmap bitmap, PageRotate rotate)
		{
			PdfBitmap pdfBitmap;
			try
			{
				pdfBitmap = PdfBitmap.FromBitmap(bitmap);
			}
			catch
			{
				try
				{
					Bitmap bitmap2 = bitmap;
					bitmap = new Bitmap(bitmap2.Width, bitmap2.Height, PixelFormat.Format32bppArgb);
					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
						graphics.DrawImage(bitmap2, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
					}
					if (rotate == PageRotate.Rotate90)
					{
						bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
					}
					if (rotate == PageRotate.Rotate180)
					{
						bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
					}
					if (rotate == PageRotate.Rotate270)
					{
						bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
					}
					return PdfBitmap.FromBitmap(bitmap);
				}
				catch
				{
				}
				pdfBitmap = null;
			}
			return pdfBitmap;
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x0002C4BC File Offset: 0x0002A6BC
		public static PdfBitmap CreatePdfBitmapFromFile(Bitmap bitmap)
		{
			PdfBitmap pdfBitmap;
			try
			{
				pdfBitmap = PdfBitmap.FromBitmap(bitmap);
			}
			catch
			{
				try
				{
					Bitmap bitmap2 = bitmap;
					bitmap = new Bitmap(bitmap2.Width, bitmap2.Height, PixelFormat.Format32bppArgb);
					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
						graphics.DrawImage(bitmap2, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
					}
					return PdfBitmap.FromBitmap(bitmap);
				}
				catch
				{
				}
				pdfBitmap = null;
			}
			return pdfBitmap;
		}
	}
}
