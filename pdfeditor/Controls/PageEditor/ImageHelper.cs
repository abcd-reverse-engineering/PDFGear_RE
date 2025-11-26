using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x02000249 RID: 585
	internal class ImageHelper
	{
		// Token: 0x06002196 RID: 8598
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteObject(IntPtr hObject);

		// Token: 0x06002197 RID: 8599 RVA: 0x0009A750 File Offset: 0x00098950
		public static Bitmap GetBitmap(string filename)
		{
			try
			{
				using (Image image = Image.FromFile(filename))
				{
					return new Bitmap(image);
				}
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x06002198 RID: 8600 RVA: 0x0009A79C File Offset: 0x0009899C
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

		// Token: 0x06002199 RID: 8601 RVA: 0x0009A7E0 File Offset: 0x000989E0
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

		// Token: 0x0600219A RID: 8602 RVA: 0x0009A854 File Offset: 0x00098A54
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

		// Token: 0x04000DCE RID: 3534
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
