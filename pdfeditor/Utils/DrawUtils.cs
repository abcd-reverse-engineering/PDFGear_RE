using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using CommonLib.Common;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using pdfeditor.Properties;

namespace pdfeditor.Utils
{
	// Token: 0x02000099 RID: 153
	public static class DrawUtils
	{
		// Token: 0x060009EA RID: 2538 RVA: 0x000325AC File Offset: 0x000307AC
		private static bool CheckUIThread()
		{
			return DispatcherHelper.UIDispatcher.CheckAccess();
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x000325B8 File Offset: 0x000307B8
		public static void ShowUnsupportedImageMessage()
		{
			string productName = UtilManager.GetProductName();
			ModernMessageBox.Show(Resources.UnsupportedImageMsg, productName, MessageBoxButton.OK, MessageBoxResult.None, null, false);
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x000325DC File Offset: 0x000307DC
		public static async Task<PdfBitmap> CloneAsync(this PdfBitmap pdfBitmap, FX_RECT clip, CancellationToken cancellationToken)
		{
			if (pdfBitmap == null)
			{
				throw new ArgumentNullException("pdfBitmap");
			}
			cancellationToken.ThrowIfCancellationRequested();
			return await Task.Run<PdfBitmap>(TaskExceptionHelper.ExceptionBoundary<PdfBitmap>(delegate
			{
				cancellationToken.ThrowIfCancellationRequested();
				IntPtr intPtr = Pdfium.FFPDFBitmap_Clone(pdfBitmap.Handle, clip);
				if (cancellationToken.IsCancellationRequested)
				{
					Pdfium.FPDFBitmap_Destroy(intPtr);
				}
				cancellationToken.ThrowIfCancellationRequested();
				return new PdfBitmap(intPtr);
			})).ConfigureAwait(false);
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x00032630 File Offset: 0x00030830
		public static async Task<WriteableBitmap> ToWriteableBitmapAsync(this PdfBitmap pdfBitmap, CancellationToken cancellationToken)
		{
			return await pdfBitmap.ToWriteableBitmapAsync(96U, 96U, cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x0003267C File Offset: 0x0003087C
		public static async Task<WriteableBitmap> ToWriteableBitmapAsync(this PdfBitmap pdfBitmap, uint dpiX, uint dpiY, CancellationToken cancellationToken)
		{
			DrawUtils.<>c__DisplayClass4_0 CS$<>8__locals1 = new DrawUtils.<>c__DisplayClass4_0();
			CS$<>8__locals1.pdfBitmap = pdfBitmap;
			CS$<>8__locals1.dpiX = dpiX;
			CS$<>8__locals1.dpiY = dpiY;
			CS$<>8__locals1.cancellationToken = cancellationToken;
			if (CS$<>8__locals1.pdfBitmap == null)
			{
				throw new ArgumentNullException("pdfBitmap");
			}
			CS$<>8__locals1.cancellationToken.ThrowIfCancellationRequested();
			CS$<>8__locals1.func = delegate
			{
				DrawUtils.<>c__DisplayClass4_0.<<ToWriteableBitmapAsync>b__0>d <<ToWriteableBitmapAsync>b__0>d;
				<<ToWriteableBitmapAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder<WriteableBitmap>.Create();
				<<ToWriteableBitmapAsync>b__0>d.<>4__this = CS$<>8__locals1;
				<<ToWriteableBitmapAsync>b__0>d.<>1__state = -1;
				<<ToWriteableBitmapAsync>b__0>d.<>t__builder.Start<DrawUtils.<>c__DisplayClass4_0.<<ToWriteableBitmapAsync>b__0>d>(ref <<ToWriteableBitmapAsync>b__0>d);
				return <<ToWriteableBitmapAsync>b__0>d.<>t__builder.Task;
			};
			WriteableBitmap writeableBitmap;
			if (DrawUtils.CheckUIThread())
			{
				writeableBitmap = await CS$<>8__locals1.func().ConfigureAwait(false);
			}
			else
			{
				writeableBitmap = await (await DispatcherHelper.UIDispatcher.InvokeAsync<Task<WriteableBitmap>>(delegate
				{
					DrawUtils.<>c__DisplayClass4_0.<<ToWriteableBitmapAsync>b__1>d <<ToWriteableBitmapAsync>b__1>d;
					<<ToWriteableBitmapAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder<WriteableBitmap>.Create();
					<<ToWriteableBitmapAsync>b__1>d.<>4__this = CS$<>8__locals1;
					<<ToWriteableBitmapAsync>b__1>d.<>1__state = -1;
					<<ToWriteableBitmapAsync>b__1>d.<>t__builder.Start<DrawUtils.<>c__DisplayClass4_0.<<ToWriteableBitmapAsync>b__1>d>(ref <<ToWriteableBitmapAsync>b__1>d);
					return <<ToWriteableBitmapAsync>b__1>d.<>t__builder.Task;
				})).ConfigureAwait(false);
			}
			return writeableBitmap;
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x000326D8 File Offset: 0x000308D8
		public static async Task DrawAsync(WriteableBitmap bitmap, PdfBitmap pdfBitmap, CancellationToken cancellationToken)
		{
			if (bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			if (pdfBitmap == null)
			{
				throw new ArgumentNullException("pdfBitmap");
			}
			cancellationToken.ThrowIfCancellationRequested();
			uint num = (uint)(pdfBitmap.Stride * pdfBitmap.Height);
			await DrawUtils.DrawAsync(bitmap, pdfBitmap.Buffer, num, cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x0003272C File Offset: 0x0003092C
		public static async Task DrawAsync(WriteableBitmap bitmap, IntPtr pBuffer, uint length, CancellationToken cancellationToken)
		{
			DrawUtils.<>c__DisplayClass6_0 CS$<>8__locals1 = new DrawUtils.<>c__DisplayClass6_0();
			CS$<>8__locals1.bitmap = bitmap;
			CS$<>8__locals1.length = length;
			CS$<>8__locals1.cancellationToken = cancellationToken;
			CS$<>8__locals1.pBuffer = pBuffer;
			if (CS$<>8__locals1.bitmap == null)
			{
				throw new ArgumentNullException("bitmap");
			}
			CS$<>8__locals1.cancellationToken.ThrowIfCancellationRequested();
			bool isUIThread = DrawUtils.CheckUIThread();
			if (isUIThread)
			{
				CS$<>8__locals1.bitmap.Lock();
			}
			else
			{
				await DispatcherHelper.UIDispatcher.InvokeAsync(delegate
				{
					CS$<>8__locals1.bitmap.Lock();
				});
			}
			object obj = null;
			try
			{
				DrawUtils.<>c__DisplayClass6_1 CS$<>8__locals2 = new DrawUtils.<>c__DisplayClass6_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.pBackBuffer = CS$<>8__locals2.CS$<>8__locals1.bitmap.BackBuffer;
				await Task.Run(TaskExceptionHelper.ExceptionBoundary(delegate
				{
					uint num2;
					for (uint num = 0U; num < CS$<>8__locals2.CS$<>8__locals1.length; num += num2)
					{
						num2 = 4194304U;
						if (num + num2 > CS$<>8__locals2.CS$<>8__locals1.length)
						{
							num2 = CS$<>8__locals2.CS$<>8__locals1.length - num;
						}
						CS$<>8__locals2.CS$<>8__locals1.cancellationToken.ThrowIfCancellationRequested();
						DrawUtils.CopyMemory(CS$<>8__locals2.pBackBuffer + (int)num, CS$<>8__locals2.CS$<>8__locals1.pBuffer + (int)num, num2);
						CS$<>8__locals2.CS$<>8__locals1.cancellationToken.ThrowIfCancellationRequested();
					}
				}), CS$<>8__locals2.CS$<>8__locals1.cancellationToken).ConfigureAwait(isUIThread);
				CS$<>8__locals2.CS$<>8__locals1.cancellationToken.ThrowIfCancellationRequested();
				Action action = delegate
				{
					CS$<>8__locals2.CS$<>8__locals1.bitmap.AddDirtyRect(new Int32Rect(0, 0, CS$<>8__locals2.CS$<>8__locals1.bitmap.PixelWidth, CS$<>8__locals2.CS$<>8__locals1.bitmap.PixelHeight));
				};
				if (isUIThread)
				{
					action();
				}
				else
				{
					await DispatcherHelper.UIDispatcher.InvokeAsync(action);
				}
				CS$<>8__locals2 = null;
			}
			catch (object obj)
			{
			}
			if (isUIThread)
			{
				CS$<>8__locals1.bitmap.Unlock();
			}
			else
			{
				await DispatcherHelper.UIDispatcher.InvokeAsync(delegate
				{
					CS$<>8__locals1.bitmap.Unlock();
				});
			}
			object obj2 = obj;
			if (obj2 != null)
			{
				Exception ex = obj2 as Exception;
				if (ex == null)
				{
					throw obj2;
				}
				ExceptionDispatchInfo.Capture(ex).Throw();
			}
			obj = null;
		}

		// Token: 0x060009F1 RID: 2545
		[DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
		public static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

		// Token: 0x060009F2 RID: 2546
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool DeleteObject(IntPtr hObject);
	}
}
