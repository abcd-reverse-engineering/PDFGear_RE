using System;
using System.Buffers;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.BasicTypes;

namespace pdfeditor.Models.PageContents
{
	// Token: 0x02000150 RID: 336
	public class PageImageModel
	{
		// Token: 0x1700081D RID: 2077
		// (get) Token: 0x0600141B RID: 5147 RVA: 0x00050259 File Offset: 0x0004E459
		// (set) Token: 0x0600141C RID: 5148 RVA: 0x00050261 File Offset: 0x0004E461
		public int ImagePageIndex { get; set; }

		// Token: 0x1700081E RID: 2078
		// (get) Token: 0x0600141D RID: 5149 RVA: 0x0005026A File Offset: 0x0004E46A
		// (set) Token: 0x0600141E RID: 5150 RVA: 0x00050272 File Offset: 0x0004E472
		public ImageMatrix Matrix { get; set; }

		// Token: 0x1700081F RID: 2079
		// (get) Token: 0x0600141F RID: 5151 RVA: 0x0005027B File Offset: 0x0004E47B
		// (set) Token: 0x06001420 RID: 5152 RVA: 0x00050283 File Offset: 0x0004E483
		public int ImageIndex { get; set; }

		// Token: 0x17000820 RID: 2080
		// (get) Token: 0x06001421 RID: 5153 RVA: 0x0005028C File Offset: 0x0004E48C
		// (set) Token: 0x06001422 RID: 5154 RVA: 0x00050294 File Offset: 0x0004E494
		public Image Image { get; set; }

		// Token: 0x17000821 RID: 2081
		// (get) Token: 0x06001423 RID: 5155 RVA: 0x0005029D File Offset: 0x0004E49D
		// (set) Token: 0x06001424 RID: 5156 RVA: 0x000502A5 File Offset: 0x0004E4A5
		public float Height { get; set; }

		// Token: 0x17000822 RID: 2082
		// (get) Token: 0x06001425 RID: 5157 RVA: 0x000502AE File Offset: 0x0004E4AE
		// (set) Token: 0x06001426 RID: 5158 RVA: 0x000502B6 File Offset: 0x0004E4B6
		public float Width { get; set; }

		// Token: 0x17000823 RID: 2083
		// (get) Token: 0x06001427 RID: 5159 RVA: 0x000502BF File Offset: 0x0004E4BF
		// (set) Token: 0x06001428 RID: 5160 RVA: 0x000502C7 File Offset: 0x0004E4C7
		public int imageHeight { get; set; }

		// Token: 0x17000824 RID: 2084
		// (get) Token: 0x06001429 RID: 5161 RVA: 0x000502D0 File Offset: 0x0004E4D0
		// (set) Token: 0x0600142A RID: 5162 RVA: 0x000502D8 File Offset: 0x0004E4D8
		public int imageWidth { get; set; }

		// Token: 0x17000825 RID: 2085
		// (get) Token: 0x0600142B RID: 5163 RVA: 0x000502E1 File Offset: 0x0004E4E1
		// (set) Token: 0x0600142C RID: 5164 RVA: 0x000502E9 File Offset: 0x0004E4E9
		public PdfTypeBase sMaskRef { get; set; }

		// Token: 0x0600142D RID: 5165 RVA: 0x000502F4 File Offset: 0x0004E4F4
		public PageImageModel(PdfImageObject pdfImageObject, int pageindex)
		{
			this.ImagePageIndex = pageindex;
			if (pdfImageObject.Container == null || pdfImageObject.Container.Form != null)
			{
				throw new ArgumentException(null, "pdfImageObject");
			}
			this.ImageIndex = pdfImageObject.Container.IndexOf(pdfImageObject);
			this.Matrix = new ImageMatrix(pdfImageObject.Matrix.a, pdfImageObject.Matrix.b, pdfImageObject.Matrix.c, pdfImageObject.Matrix.d, pdfImageObject.Matrix.e, pdfImageObject.Matrix.f);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				pdfImageObject.Bitmap.Image.Save(memoryStream, ImageFormat.Png);
				memoryStream.Position = 0L;
				this.Image = Image.FromStream(memoryStream);
			}
			if (pdfImageObject.Stream != null && pdfImageObject.Stream.Dictionary.ContainsKey("SMask") && pdfImageObject.Stream.Dictionary["SMask"].Is<PdfTypeStream>())
			{
				this.sMaskRef = pdfImageObject.Stream.Dictionary["SMask"].As<PdfTypeStream>(true);
			}
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x00050438 File Offset: 0x0004E638
		private static Bitmap CreateSoftMaskBitmap(PdfTypeStream sMaskStream, out string colorSpace)
		{
			int intValue = sMaskStream.Dictionary["Width"].As<PdfTypeNumber>(true).IntValue;
			int intValue2 = sMaskStream.Dictionary["Height"].As<PdfTypeNumber>(true).IntValue;
			int intValue3 = sMaskStream.Dictionary["BitsPerComponent"].As<PdfTypeNumber>(true).IntValue;
			colorSpace = sMaskStream.Dictionary["ColorSpace"].As<PdfTypeName>(true).Value;
			if (!string.IsNullOrEmpty(sMaskStream.Dictionary.ContainsKey("Filter") ? sMaskStream.Dictionary["Filter"].As<PdfTypeName>(true).Value : null))
			{
				return null;
			}
			try
			{
				byte[] decodedData = sMaskStream.DecodedData;
				Memory<byte> memory = new Memory<byte>(decodedData);
				using (MemoryHandle memoryHandle = memory.Pin())
				{
					return new Bitmap(intValue, intValue2, intValue * intValue3 / 8, PixelFormat.Format8bppIndexed, new IntPtr(memoryHandle.Pointer));
				}
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x0005055C File Offset: 0x0004E75C
		private static PdfBitmap CreateFXDIB8bppMask(Bitmap bitmap)
		{
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
			PdfBitmap pdfBitmap;
			try
			{
				pdfBitmap = new PdfBitmap(bitmapData.Width, bitmapData.Height, BitmapFormats.FXDIB_8bppMask, bitmapData.Scan0, bitmapData.Stride);
			}
			finally
			{
				bitmap.UnlockBits(bitmapData);
			}
			return pdfBitmap;
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x000505C8 File Offset: 0x0004E7C8
		private static PdfTypeStream ConvertBitmapToPdfTypeStream(Bitmap bitmap, string color)
		{
			int width = bitmap.Width;
			int height = bitmap.Height;
			PixelFormat pixelFormat = bitmap.PixelFormat;
			if (Image.GetPixelFormatSize(pixelFormat) != 8 || pixelFormat != PixelFormat.Format8bppIndexed)
			{
				throw new ArgumentException("error!");
			}
			PdfTypeStream pdfTypeStream = PdfTypeStream.Create();
			pdfTypeStream.InitEmpty();
			pdfTypeStream.Dictionary.Add("Width", PdfTypeNumber.Create(width));
			pdfTypeStream.Dictionary.Add("Height", PdfTypeNumber.Create(height));
			pdfTypeStream.Dictionary.Add("BitsPerComponent", PdfTypeNumber.Create(8));
			pdfTypeStream.Dictionary.Add("ColorSpace", PdfTypeName.Create(color));
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, pixelFormat);
			try
			{
				int num = Math.Abs(bitmapData.Stride) * height;
				byte[] array = new byte[num];
				Marshal.Copy(bitmapData.Scan0, array, 0, num);
				pdfTypeStream.SetContent(array, true);
			}
			finally
			{
				bitmap.UnlockBits(bitmapData);
			}
			return pdfTypeStream;
		}
	}
}
