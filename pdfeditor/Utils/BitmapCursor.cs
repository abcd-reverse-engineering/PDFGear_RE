using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.ViewModels;

namespace pdfeditor.Utils
{
	// Token: 0x02000091 RID: 145
	internal class BitmapCursor : SafeHandle
	{
		// Token: 0x17000253 RID: 595
		// (get) Token: 0x0600098B RID: 2443 RVA: 0x0003097F File Offset: 0x0002EB7F
		public override bool IsInvalid
		{
			get
			{
				return this.handle == (IntPtr)(-1);
			}
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x00030992 File Offset: 0x0002EB92
		public static Cursor CreateBmpCursor(Bitmap cursorBitmap)
		{
			return CursorInteropHelper.Create(new BitmapCursor(cursorBitmap));
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x0003099F File Offset: 0x0002EB9F
		protected BitmapCursor(Bitmap cursorBitmap)
			: base((IntPtr)(-1), true)
		{
			this.handle = cursorBitmap.GetHicon();
		}

		// Token: 0x0600098E RID: 2446 RVA: 0x000309BA File Offset: 0x0002EBBA
		protected override bool ReleaseHandle()
		{
			bool flag = BitmapCursor.DestroyIcon(this.handle);
			this.handle = (IntPtr)(-1);
			return flag;
		}

		// Token: 0x0600098F RID: 2447
		[DllImport("user32")]
		private static extern bool DestroyIcon(IntPtr hIcon);

		// Token: 0x06000990 RID: 2448 RVA: 0x000309D4 File Offset: 0x0002EBD4
		public static Cursor CreateCustomCursor(ToolbarSettingInkEraserModel inkEraserModel, MainViewModel VM, int cursorSize)
		{
			EllipseGeometry ellipseGeometry = new EllipseGeometry(new global::System.Windows.Point((double)(cursorSize / 2), (double)(cursorSize / 2)), (double)(cursorSize / 2), (double)(cursorSize / 2));
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			{
				SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.White);
				drawingContext.DrawGeometry(solidColorBrush, null, ellipseGeometry);
			}
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(cursorSize, cursorSize, 96.0, 96.0, PixelFormats.Default);
			renderTargetBitmap.Render(drawingVisual);
			PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
			pngBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
			Cursor cursor;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				pngBitmapEncoder.Save(memoryStream);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (Bitmap bitmap = new Bitmap(memoryStream))
				{
					cursor = BitmapCursor.CreateBmpCursor(bitmap);
				}
			}
			return cursor;
		}
	}
}
