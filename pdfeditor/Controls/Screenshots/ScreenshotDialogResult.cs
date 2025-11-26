using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using Patagames.Pdf;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x0200021A RID: 538
	public class ScreenshotDialogResult
	{
		// Token: 0x06001E1F RID: 7711 RVA: 0x00085894 File Offset: 0x00083A94
		private ScreenshotDialogResult()
		{
		}

		// Token: 0x17000A6F RID: 2671
		// (get) Token: 0x06001E20 RID: 7712 RVA: 0x000858AE File Offset: 0x00083AAE
		// (set) Token: 0x06001E21 RID: 7713 RVA: 0x000858B6 File Offset: 0x00083AB6
		public ScreenshotDialogMode Mode { get; private set; }

		// Token: 0x17000A70 RID: 2672
		// (get) Token: 0x06001E22 RID: 7714 RVA: 0x000858BF File Offset: 0x00083ABF
		// (set) Token: 0x06001E23 RID: 7715 RVA: 0x000858C7 File Offset: 0x00083AC7
		public bool Completed { get; private set; }

		// Token: 0x17000A71 RID: 2673
		// (get) Token: 0x06001E24 RID: 7716 RVA: 0x000858D0 File Offset: 0x00083AD0
		// (set) Token: 0x06001E25 RID: 7717 RVA: 0x000858D8 File Offset: 0x00083AD8
		public string ExtractedText { get; private set; } = "";

		// Token: 0x17000A72 RID: 2674
		// (get) Token: 0x06001E26 RID: 7718 RVA: 0x000858E1 File Offset: 0x00083AE1
		// (set) Token: 0x06001E27 RID: 7719 RVA: 0x000858E9 File Offset: 0x00083AE9
		public FS_RECTF? BeforeRect { get; private set; }

		// Token: 0x17000A73 RID: 2675
		// (get) Token: 0x06001E28 RID: 7720 RVA: 0x000858F2 File Offset: 0x00083AF2
		// (set) Token: 0x06001E29 RID: 7721 RVA: 0x000858FA File Offset: 0x00083AFA
		public FS_RECTF SelectedRect { get; private set; }

		// Token: 0x17000A74 RID: 2676
		// (get) Token: 0x06001E2A RID: 7722 RVA: 0x00085903 File Offset: 0x00083B03
		// (set) Token: 0x06001E2B RID: 7723 RVA: 0x0008590B File Offset: 0x00083B0B
		public Rect SelectedClientRect { get; private set; }

		// Token: 0x17000A75 RID: 2677
		// (get) Token: 0x06001E2C RID: 7724 RVA: 0x00085914 File Offset: 0x00083B14
		// (set) Token: 0x06001E2D RID: 7725 RVA: 0x0008591C File Offset: 0x00083B1C
		public int PageIndex { get; private set; } = -1;

		// Token: 0x17000A76 RID: 2678
		// (get) Token: 0x06001E2E RID: 7726 RVA: 0x00085925 File Offset: 0x00083B25
		// (set) Token: 0x06001E2F RID: 7727 RVA: 0x0008592D File Offset: 0x00083B2D
		public WriteableBitmap Image { get; private set; }

		// Token: 0x17000A77 RID: 2679
		// (get) Token: 0x06001E30 RID: 7728 RVA: 0x00085936 File Offset: 0x00083B36
		// (set) Token: 0x06001E31 RID: 7729 RVA: 0x0008593E File Offset: 0x00083B3E
		public WriteableBitmap RotatedImage { get; private set; }

		// Token: 0x17000A78 RID: 2680
		// (get) Token: 0x06001E32 RID: 7730 RVA: 0x00085947 File Offset: 0x00083B47
		// (set) Token: 0x06001E33 RID: 7731 RVA: 0x0008594F File Offset: 0x00083B4F
		public Image OcrImage { get; private set; }

		// Token: 0x17000A79 RID: 2681
		// (get) Token: 0x06001E34 RID: 7732 RVA: 0x00085958 File Offset: 0x00083B58
		// (set) Token: 0x06001E35 RID: 7733 RVA: 0x00085960 File Offset: 0x00083B60
		public int[] ApplyPageIndex { get; private set; }

		// Token: 0x17000A7A RID: 2682
		// (get) Token: 0x06001E36 RID: 7734 RVA: 0x00085969 File Offset: 0x00083B69
		// (set) Token: 0x06001E37 RID: 7735 RVA: 0x00085971 File Offset: 0x00083B71
		public FS_RECTF[] BeforeRects { get; private set; }

		// Token: 0x17000A7B RID: 2683
		// (get) Token: 0x06001E38 RID: 7736 RVA: 0x0008597A File Offset: 0x00083B7A
		// (set) Token: 0x06001E39 RID: 7737 RVA: 0x00085982 File Offset: 0x00083B82
		public FS_RECTF[] SelectedRects { get; private set; }

		// Token: 0x17000A7C RID: 2684
		// (get) Token: 0x06001E3A RID: 7738 RVA: 0x0008598B File Offset: 0x00083B8B
		// (set) Token: 0x06001E3B RID: 7739 RVA: 0x00085993 File Offset: 0x00083B93
		public Rect[] SelectedClientRects { get; private set; }

		// Token: 0x06001E3C RID: 7740 RVA: 0x0008599C File Offset: 0x00083B9C
		public static ScreenshotDialogResult CreateCancel()
		{
			return new ScreenshotDialogResult
			{
				Completed = false
			};
		}

		// Token: 0x06001E3D RID: 7741 RVA: 0x000859AC File Offset: 0x00083BAC
		public static ScreenshotDialogResult CreateExtractedText(int pageIndex, string text, WriteableBitmap image, WriteableBitmap rotatedImage, FS_RECTF rect, Rect selectedClientRect, bool ocr)
		{
			return new ScreenshotDialogResult
			{
				Completed = (image != null),
				Mode = (ocr ? ScreenshotDialogMode.Ocr : ScreenshotDialogMode.ExtractText),
				PageIndex = pageIndex,
				Image = image,
				RotatedImage = rotatedImage,
				ExtractedText = (text ?? ""),
				SelectedRect = rect,
				SelectedClientRect = selectedClientRect
			};
		}

		// Token: 0x06001E3E RID: 7742 RVA: 0x00085A0C File Offset: 0x00083C0C
		public static ScreenshotDialogResult CreateExtractImageText(int pageIndex, string text, WriteableBitmap image, Image OcrImage, FS_RECTF rect, Rect selectedClientRect, bool ocr)
		{
			return new ScreenshotDialogResult
			{
				Completed = (image != null),
				Mode = (ocr ? ScreenshotDialogMode.Ocr : ScreenshotDialogMode.ExtractText),
				PageIndex = pageIndex,
				Image = image,
				OcrImage = OcrImage,
				ExtractedText = (text ?? ""),
				SelectedRect = rect,
				SelectedClientRect = selectedClientRect
			};
		}

		// Token: 0x06001E3F RID: 7743 RVA: 0x00085A6B File Offset: 0x00083C6B
		public static ScreenshotDialogResult CreateImage(int pageIndex, WriteableBitmap image, WriteableBitmap rotatedImage, FS_RECTF rect, Rect selectedClientRect)
		{
			return new ScreenshotDialogResult
			{
				Completed = (image != null),
				Mode = ScreenshotDialogMode.Screenshot,
				PageIndex = pageIndex,
				Image = image,
				RotatedImage = rotatedImage,
				SelectedRect = rect,
				SelectedClientRect = selectedClientRect
			};
		}

		// Token: 0x06001E40 RID: 7744 RVA: 0x00085AA7 File Offset: 0x00083CA7
		public static ScreenshotDialogResult GetCropBox(int pageIndex, int[] pageIndexs, FS_RECTF beforeRect, FS_RECTF rect, Rect selectedClientRect)
		{
			return new ScreenshotDialogResult
			{
				Completed = true,
				Mode = ScreenshotDialogMode.CropPage,
				PageIndex = pageIndex,
				SelectedRect = rect,
				BeforeRect = new FS_RECTF?(beforeRect),
				SelectedClientRect = selectedClientRect,
				ApplyPageIndex = pageIndexs
			};
		}

		// Token: 0x06001E41 RID: 7745 RVA: 0x00085AE8 File Offset: 0x00083CE8
		public static ScreenshotDialogResult GetCropBoxs(int pageIndex, int[] pageIndexs, FS_RECTF[] beforeRect, FS_RECTF[] rect, Rect[] selectedClientRect)
		{
			return new ScreenshotDialogResult
			{
				Completed = (rect != null && selectedClientRect != null),
				Mode = ScreenshotDialogMode.CropPage,
				PageIndex = pageIndex,
				SelectedRects = rect,
				BeforeRects = beforeRect,
				SelectedClientRects = selectedClientRect,
				ApplyPageIndex = pageIndexs
			};
		}

		// Token: 0x06001E42 RID: 7746 RVA: 0x00085B38 File Offset: 0x00083D38
		public static ScreenshotDialogResult GetResizeBoxs(int pageIndex, int[] pageIndexs, FS_RECTF[] beforeRect, FS_RECTF[] rect, Rect[] selectedClientRect)
		{
			return new ScreenshotDialogResult
			{
				Completed = (rect != null && selectedClientRect != null),
				Mode = ScreenshotDialogMode.ResizePage,
				PageIndex = pageIndex,
				SelectedRects = rect,
				BeforeRects = beforeRect,
				SelectedClientRects = selectedClientRect,
				ApplyPageIndex = pageIndexs
			};
		}
	}
}
