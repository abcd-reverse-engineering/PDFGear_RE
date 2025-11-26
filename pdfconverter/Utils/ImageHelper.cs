using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace pdfconverter.Utils
{
	// Token: 0x0200003E RID: 62
	internal class ImageHelper
	{
		// Token: 0x060004D5 RID: 1237
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteObject(IntPtr hObject);

		// Token: 0x060004D6 RID: 1238 RVA: 0x00013A08 File Offset: 0x00011C08
		public static Bitmap GetBitmap(string filename)
		{
			try
			{
				Bitmap bitmap = Image.FromFile(filename) as Bitmap;
				if (bitmap != null)
				{
					return bitmap;
				}
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x00013A40 File Offset: 0x00011C40
		public static BitmapFrame ToBitmapFrame(Bitmap bitmap)
		{
			if (bitmap == null)
			{
				return null;
			}
			try
			{
				BitmapSource bitmapSource = ImageHelper.ToBitmapSource(bitmap);
				if (bitmapSource != null)
				{
					BitmapFrame bitmapFrame = BitmapFrame.Create(bitmapSource);
					bitmapFrame.Freeze();
					return bitmapFrame;
				}
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x00013A84 File Offset: 0x00011C84
		public static BitmapSource ToBitmapSource(Bitmap bitmap)
		{
			if (bitmap == null)
			{
				return null;
			}
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				intPtr = bitmap.GetHbitmap();
				BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				bitmapSource.Freeze();
				return bitmapSource;
			}
			catch
			{
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					ImageHelper.DeleteObject(intPtr);
				}
			}
			return null;
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x00013AF8 File Offset: 0x00011CF8
		public static Bitmap CheckPixelFormat(Bitmap bitmap)
		{
			if (!ImageHelper.indexedPixelFormats.Contains(bitmap.PixelFormat))
			{
				return bitmap;
			}
			Bitmap bitmap2 = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
			bitmap2.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);
			using (Graphics graphics = Graphics.FromImage(bitmap2))
			{
				graphics.DrawImage(bitmap, 0, 0);
			}
			return bitmap2;
		}

		// Token: 0x0400026A RID: 618
		private static readonly PixelFormat[] indexedPixelFormats = new PixelFormat[]
		{
			PixelFormat.Format1bppIndexed,
			PixelFormat.Format4bppIndexed,
			PixelFormat.Format8bppIndexed,
			PixelFormat.Undefined,
			PixelFormat.Undefined,
			PixelFormat.Format16bppArgb1555,
			PixelFormat.Format16bppGrayScale
		};
	}
}
