using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using LruCacheNet;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Models.Thumbnails;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit.Utils;

namespace pdfeditor.Services
{
	// Token: 0x02000123 RID: 291
	public class PdfThumbnailService
	{
		// Token: 0x06000D2D RID: 3373 RVA: 0x00042C20 File Offset: 0x00040E20
		public PdfThumbnailService()
			: this(150, 50)
		{
		}

		// Token: 0x06000D2E RID: 3374 RVA: 0x00042C2F File Offset: 0x00040E2F
		public PdfThumbnailService(int thumbnailWidth, int cacheSize)
		{
			if (thumbnailWidth <= 0)
			{
				throw new ArgumentException("thumbnailWidth");
			}
			if (cacheSize <= 0)
			{
				throw new ArgumentException("cacheSize");
			}
			this.thumbnailWidth = thumbnailWidth;
			this.thumbnailCache = new LruCache<PdfThumbnailService.PdfThumbnailCacheKey, WriteableBitmap>(cacheSize);
		}

		// Token: 0x170002AF RID: 687
		// (get) Token: 0x06000D2F RID: 3375 RVA: 0x00042C68 File Offset: 0x00040E68
		public int ThumbnailWidth
		{
			get
			{
				return this.thumbnailWidth;
			}
		}

		// Token: 0x06000D30 RID: 3376 RVA: 0x00042C70 File Offset: 0x00040E70
		public void ClearCache()
		{
			LruCache<PdfThumbnailService.PdfThumbnailCacheKey, WriteableBitmap> lruCache = this.thumbnailCache;
			lock (lruCache)
			{
				if (this.thumbnailCache.Count > 0)
				{
					this.thumbnailCache.Clear();
				}
			}
		}

		// Token: 0x06000D31 RID: 3377 RVA: 0x00042CC4 File Offset: 0x00040EC4
		public global::System.Windows.Size GetThumbnailImageSize(PdfPage page, PageRotate rotate, int width = 0, int height = 0)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}
			FS_SIZEF effectiveSize = page.GetEffectiveSize(rotate, false);
			if (width == 0 && height == 0)
			{
				width = this.ThumbnailWidth;
			}
			if (height == 0)
			{
				height = (int)((double)(Math.Abs(effectiveSize.Height) * (float)width) * 1.0 / (double)Math.Abs(effectiveSize.Width));
			}
			else
			{
				width = (int)((double)(Math.Abs(effectiveSize.Width) * (float)height) * 1.0 / (double)Math.Abs(effectiveSize.Height));
			}
			return new global::System.Windows.Size((double)width, (double)height);
		}

		// Token: 0x06000D32 RID: 3378 RVA: 0x00042D5C File Offset: 0x00040F5C
		public async Task<WriteableBitmap> TryGetPdfBitmapAsync(PdfPage page, global::System.Windows.Media.Color background, PageRotate rotate, CancellationToken cancellationToken)
		{
			return await this.TryGetPdfBitmapAsync(page, background, rotate, 0, 0, cancellationToken).ConfigureAwait(false);
		}

		// Token: 0x06000D33 RID: 3379 RVA: 0x00042DC0 File Offset: 0x00040FC0
		public async Task<WriteableBitmap> TryGetPdfBitmapAsync(PdfPage page, global::System.Windows.Media.Color background, PageRotate rotate, int width, int height, CancellationToken cancellationToken)
		{
			PdfThumbnailService.<>c__DisplayClass10_0 CS$<>8__locals1 = new PdfThumbnailService.<>c__DisplayClass10_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.page = page;
			CS$<>8__locals1.background = background;
			CS$<>8__locals1.rotate = rotate;
			CS$<>8__locals1.cancellationToken = cancellationToken;
			WriteableBitmap writeableBitmap;
			if (CS$<>8__locals1.page == null)
			{
				writeableBitmap = null;
			}
			else
			{
				PdfPageEditList pageEditListItemSource = Ioc.Default.GetRequiredService<MainViewModel>().PageEditors.PageEditListItemSource;
				CS$<>8__locals1.cacheKey = new PdfThumbnailService.PdfThumbnailCacheKey(CS$<>8__locals1.page.PageIndex, CS$<>8__locals1.background, CS$<>8__locals1.rotate);
				global::System.Windows.Size thumbnailImageSize = this.GetThumbnailImageSize(CS$<>8__locals1.page, CS$<>8__locals1.rotate, width, height);
				CS$<>8__locals1._width = (int)thumbnailImageSize.Width;
				CS$<>8__locals1._height = (int)thumbnailImageSize.Height;
				if (CS$<>8__locals1._width <= 0 || CS$<>8__locals1._height <= 0)
				{
					writeableBitmap = null;
				}
				else
				{
					LruCache<PdfThumbnailService.PdfThumbnailCacheKey, WriteableBitmap> lruCache = this.thumbnailCache;
					lock (lruCache)
					{
						WriteableBitmap writeableBitmap2;
						if (this.thumbnailCache.TryGetValue(CS$<>8__locals1.cacheKey, out writeableBitmap2))
						{
							if (writeableBitmap2.PixelWidth == CS$<>8__locals1._width && writeableBitmap2.PixelHeight == CS$<>8__locals1._height)
							{
								return writeableBitmap2;
							}
							if (writeableBitmap2.PixelWidth > CS$<>8__locals1._width || writeableBitmap2.PixelHeight > CS$<>8__locals1._height)
							{
								using (Bitmap bitmap = new Bitmap(writeableBitmap2.PixelWidth, writeableBitmap2.PixelHeight, global::System.Drawing.Imaging.PixelFormat.Format32bppArgb))
								{
									BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, writeableBitmap2.PixelWidth, writeableBitmap2.PixelHeight), ImageLockMode.WriteOnly, global::System.Drawing.Imaging.PixelFormat.Format32bppArgb);
									writeableBitmap2.CopyPixels(new Int32Rect(0, 0, writeableBitmap2.PixelWidth, writeableBitmap2.PixelHeight), bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);
									IntPtr hbitmap = bitmap.GetHbitmap();
									try
									{
										return new WriteableBitmap(Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, new Int32Rect(0, 0, bitmap.Width, bitmap.Height), BitmapSizeOptions.FromWidthAndHeight(CS$<>8__locals1._width, CS$<>8__locals1._height)));
									}
									finally
									{
										try
										{
											if (hbitmap != IntPtr.Zero)
											{
												DrawUtils.DeleteObject(hbitmap);
											}
										}
										catch
										{
										}
									}
								}
							}
							this.thumbnailCache.Remove(CS$<>8__locals1.cacheKey);
						}
					}
					writeableBitmap = await Task.Run<WriteableBitmap>(TaskExceptionHelper.ExceptionBoundary<WriteableBitmap>(delegate
					{
						PdfThumbnailService.<>c__DisplayClass10_0.<<TryGetPdfBitmapAsync>b__0>d <<TryGetPdfBitmapAsync>b__0>d;
						<<TryGetPdfBitmapAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder<WriteableBitmap>.Create();
						<<TryGetPdfBitmapAsync>b__0>d.<>4__this = CS$<>8__locals1;
						<<TryGetPdfBitmapAsync>b__0>d.<>1__state = -1;
						<<TryGetPdfBitmapAsync>b__0>d.<>t__builder.Start<PdfThumbnailService.<>c__DisplayClass10_0.<<TryGetPdfBitmapAsync>b__0>d>(ref <<TryGetPdfBitmapAsync>b__0>d);
						return <<TryGetPdfBitmapAsync>b__0>d.<>t__builder.Task;
					}), CS$<>8__locals1.cancellationToken).ConfigureAwait(false);
				}
			}
			return writeableBitmap;
		}

		// Token: 0x06000D34 RID: 3380 RVA: 0x00042E36 File Offset: 0x00041036
		public void RefreshAllThumbnail()
		{
			this.RefreshThumbnail(new int[] { -1 });
		}

		// Token: 0x06000D35 RID: 3381 RVA: 0x00042E48 File Offset: 0x00041048
		public void RefreshThumbnail(params int[] pageIndexes)
		{
			LruCache<PdfThumbnailService.PdfThumbnailCacheKey, WriteableBitmap> lruCache = this.thumbnailCache;
			lock (lruCache)
			{
				if (pageIndexes != null)
				{
					List<int> list = new List<int>();
					foreach (int num in pageIndexes)
					{
						if (num == -1)
						{
							this.ClearCache();
							StrongReferenceMessenger.Default.Send<ValueChangedMessage<int>, string>(new ValueChangedMessage<int>(-1), "MESSAGE_PAGE_ROTATE_CHANGED");
							return;
						}
						if (num >= 0)
						{
							list.Add(num);
						}
					}
					Dictionary<int, PdfThumbnailService.PdfThumbnailCacheKey[]> dictionary = (from c in this.thumbnailCache.Keys
						group c by c.PageIndex).ToDictionary((IGrouping<int, PdfThumbnailService.PdfThumbnailCacheKey> c) => c.Key, (IGrouping<int, PdfThumbnailService.PdfThumbnailCacheKey> c) => c.ToArray<PdfThumbnailService.PdfThumbnailCacheKey>());
					foreach (int num2 in list)
					{
						PdfThumbnailService.PdfThumbnailCacheKey[] array;
						if (dictionary.TryGetValue(num2, out array))
						{
							foreach (PdfThumbnailService.PdfThumbnailCacheKey pdfThumbnailCacheKey in array)
							{
								this.thumbnailCache.Remove(pdfThumbnailCacheKey);
							}
						}
						StrongReferenceMessenger.Default.Send<ValueChangedMessage<int>, string>(new ValueChangedMessage<int>(num2), "MESSAGE_PAGE_ROTATE_CHANGED");
					}
				}
			}
		}

		// Token: 0x06000D36 RID: 3382 RVA: 0x00042FF4 File Offset: 0x000411F4
		private bool TryRender(PdfBitmap pdfBitmap, PdfPage page, global::System.Windows.Media.Color background, PageRotate rotate)
		{
			if (pdfBitmap == null)
			{
				throw new ArgumentNullException("pdfBitmap");
			}
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}
			IntPtr intPtr = Pdfium.FPDF_LoadPage(page.Document.Handle, page.PageIndex);
			if (intPtr == IntPtr.Zero)
			{
				return false;
			}
			int width = pdfBitmap.Width;
			int height = pdfBitmap.Height;
			try
			{
				pdfBitmap.FillRectEx(0, 0, width, height, PdfThumbnailService.ToArgb(background));
				lock (page)
				{
					Pdfium.FPDF_RenderPageBitmap(pdfBitmap.Handle, intPtr, 0, 0, width, height, PdfThumbnailService.PageRotation(page, new PageRotate?(rotate)), RenderFlags.FPDF_ANNOT);
				}
				return true;
			}
			catch (Exception ex) when (!(ex is OperationCanceledException))
			{
			}
			finally
			{
				Pdfium.FPDF_ClosePage(intPtr);
			}
			return false;
		}

		// Token: 0x06000D37 RID: 3383 RVA: 0x000430F0 File Offset: 0x000412F0
		private static int ToArgb(global::System.Windows.Media.Color color)
		{
			return ((int)color.A << 24) | ((int)color.R << 16) | ((int)color.G << 8) | (int)color.B;
		}

		// Token: 0x06000D38 RID: 3384 RVA: 0x0004311C File Offset: 0x0004131C
		private static PageRotate PageRotation(PdfPage pdfPage, PageRotate? rotate)
		{
			int num = (rotate ?? pdfPage.Rotation) - pdfPage.Rotation;
			if (num < 0)
			{
				num = 4 + num;
			}
			return (PageRotate)num;
		}

		// Token: 0x040005AC RID: 1452
		public const int DefaultThumbnailWidth = 150;

		// Token: 0x040005AD RID: 1453
		private LruCache<PdfThumbnailService.PdfThumbnailCacheKey, WriteableBitmap> thumbnailCache;

		// Token: 0x040005AE RID: 1454
		private int thumbnailWidth;

		// Token: 0x02000536 RID: 1334
		private struct PdfThumbnailCacheKey : IEquatable<PdfThumbnailService.PdfThumbnailCacheKey>
		{
			// Token: 0x0600307D RID: 12413 RVA: 0x000EEB4A File Offset: 0x000ECD4A
			public PdfThumbnailCacheKey(int pageIndex, global::System.Windows.Media.Color background, PageRotate rotate)
			{
				this.Background = background;
				this.Rotate = rotate;
				this.PageIndex = pageIndex;
				this._hashCode = HashCode.Combine<int, global::System.Windows.Media.Color, PageRotate>(pageIndex, background, rotate);
			}

			// Token: 0x17000D14 RID: 3348
			// (get) Token: 0x0600307E RID: 12414 RVA: 0x000EEB6F File Offset: 0x000ECD6F
			public global::System.Windows.Media.Color Background { get; }

			// Token: 0x17000D15 RID: 3349
			// (get) Token: 0x0600307F RID: 12415 RVA: 0x000EEB77 File Offset: 0x000ECD77
			public PageRotate Rotate { get; }

			// Token: 0x17000D16 RID: 3350
			// (get) Token: 0x06003080 RID: 12416 RVA: 0x000EEB7F File Offset: 0x000ECD7F
			public int PageIndex { get; }

			// Token: 0x06003081 RID: 12417 RVA: 0x000EEB88 File Offset: 0x000ECD88
			public bool Equals(PdfThumbnailService.PdfThumbnailCacheKey other)
			{
				return this._hashCode == other._hashCode && this.PageIndex == other.PageIndex && this.Background == other.Background && this.Rotate == other.Rotate;
			}

			// Token: 0x06003082 RID: 12418 RVA: 0x000EEBD8 File Offset: 0x000ECDD8
			public override bool Equals(object obj)
			{
				return obj is PdfThumbnailService.PdfThumbnailCacheKey && ((PdfThumbnailService.PdfThumbnailCacheKey)obj).Equals(this);
			}

			// Token: 0x06003083 RID: 12419 RVA: 0x000EEC03 File Offset: 0x000ECE03
			public override int GetHashCode()
			{
				return this._hashCode;
			}

			// Token: 0x06003084 RID: 12420 RVA: 0x000EEC0B File Offset: 0x000ECE0B
			public static bool operator ==(PdfThumbnailService.PdfThumbnailCacheKey left, PdfThumbnailService.PdfThumbnailCacheKey right)
			{
				return left.Equals(right);
			}

			// Token: 0x06003085 RID: 12421 RVA: 0x000EEC15 File Offset: 0x000ECE15
			public static bool operator !=(PdfThumbnailService.PdfThumbnailCacheKey left, PdfThumbnailService.PdfThumbnailCacheKey right)
			{
				return !left.Equals(right);
			}

			// Token: 0x04001CF9 RID: 7417
			private int _hashCode;
		}
	}
}
