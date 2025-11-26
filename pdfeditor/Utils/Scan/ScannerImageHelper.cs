using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace pdfeditor.Utils.Scan
{
	// Token: 0x020000B9 RID: 185
	public class ScannerImageHelper
	{
		// Token: 0x06000B10 RID: 2832
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteObject(IntPtr hObject);

		// Token: 0x06000B11 RID: 2833 RVA: 0x00039168 File Offset: 0x00037368
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

		// Token: 0x06000B12 RID: 2834 RVA: 0x000391A0 File Offset: 0x000373A0
		public static BitmapFrame ToBitmapFrame(Bitmap bitmap)
		{
			if (bitmap == null)
			{
				return null;
			}
			try
			{
				BitmapSource bitmapSource = ScannerImageHelper.ToBitmapSource(bitmap);
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

		// Token: 0x06000B13 RID: 2835 RVA: 0x000391E4 File Offset: 0x000373E4
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
					ScannerImageHelper.DeleteObject(intPtr);
				}
			}
			return null;
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x00039258 File Offset: 0x00037458
		public static Bitmap CheckPixelFormat(Bitmap bitmap)
		{
			if (!ScannerImageHelper.indexedPixelFormats.Contains(bitmap.PixelFormat))
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

		// Token: 0x040004CA RID: 1226
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
