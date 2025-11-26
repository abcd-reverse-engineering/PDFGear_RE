using System;
using pdfeditor.Utils.Enums;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;

namespace pdfeditor.Utils
{
	// Token: 0x0200008C RID: 140
	internal static class PdfControlViewStateUtils
	{
		// Token: 0x0600093C RID: 2364 RVA: 0x0002DE94 File Offset: 0x0002C094
		public static PdfControlViewStateUtils.PdfViewerViewStateSnapshot CreateViewStateSnapshot(PdfControl pdfControl)
		{
			MainViewModel mainViewModel = ((pdfControl != null) ? pdfControl.DataContext : null) as MainViewModel;
			if (mainViewModel == null)
			{
				return null;
			}
			ScrollAnchorPointUtils.PdfViewerScrollSnapshot pdfViewerScrollSnapshot = ScrollAnchorPointUtils.CreateScrollSnapshot(pdfControl);
			if (pdfViewerScrollSnapshot == null)
			{
				return null;
			}
			return new PdfControlViewStateUtils.PdfViewerViewStateSnapshot
			{
				ScrollSnapshot = pdfViewerScrollSnapshot,
				SizeModesWrap = mainViewModel.ViewToolbar.DocSizeModeWrap,
				SubViewModeContinuous = mainViewModel.ViewToolbar.SubViewModeContinuous,
				SubViewModePage = mainViewModel.ViewToolbar.SubViewModePage,
				Zoom = mainViewModel.ViewToolbar.DocZoom,
				SelectedPageIndex = mainViewModel.SelectedPageIndex
			};
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x0002DF20 File Offset: 0x0002C120
		public static void ApplyViewStateSnapshot(PdfControl pdfControl, PdfControlViewStateUtils.PdfViewerViewStateSnapshot snapshot)
		{
			if (pdfControl == null)
			{
				return;
			}
			if (snapshot == null)
			{
				return;
			}
			MainViewModel mainViewModel = pdfControl.DataContext as MainViewModel;
			if (mainViewModel != null)
			{
				mainViewModel.ViewToolbar.DocSizeModeWrap = snapshot.SizeModesWrap;
				mainViewModel.ViewToolbar.SubViewModeContinuous = snapshot.SubViewModeContinuous;
				mainViewModel.ViewToolbar.SubViewModePage = snapshot.SubViewModePage;
				mainViewModel.ViewToolbar.DocZoom = snapshot.Zoom;
				pdfControl.UpdateLayout();
				ScrollAnchorPointUtils.ApplyScrollSnapshot(pdfControl, snapshot.ScrollSnapshot);
				mainViewModel.SelectedPageIndex = snapshot.SelectedPageIndex;
			}
		}

		// Token: 0x0200045A RID: 1114
		public class PdfViewerViewStateSnapshot
		{
			// Token: 0x17000C96 RID: 3222
			// (get) Token: 0x06002D5F RID: 11615 RVA: 0x000DE552 File Offset: 0x000DC752
			// (set) Token: 0x06002D60 RID: 11616 RVA: 0x000DE55A File Offset: 0x000DC75A
			public ScrollAnchorPointUtils.PdfViewerScrollSnapshot ScrollSnapshot { get; set; }

			// Token: 0x17000C97 RID: 3223
			// (get) Token: 0x06002D61 RID: 11617 RVA: 0x000DE563 File Offset: 0x000DC763
			// (set) Token: 0x06002D62 RID: 11618 RVA: 0x000DE56B File Offset: 0x000DC76B
			public int SelectedPageIndex { get; set; }

			// Token: 0x17000C98 RID: 3224
			// (get) Token: 0x06002D63 RID: 11619 RVA: 0x000DE574 File Offset: 0x000DC774
			// (set) Token: 0x06002D64 RID: 11620 RVA: 0x000DE57C File Offset: 0x000DC77C
			public SizeModesWrap SizeModesWrap { get; set; } = SizeModesWrap.Zoom;

			// Token: 0x17000C99 RID: 3225
			// (get) Token: 0x06002D65 RID: 11621 RVA: 0x000DE585 File Offset: 0x000DC785
			// (set) Token: 0x06002D66 RID: 11622 RVA: 0x000DE58D File Offset: 0x000DC78D
			public SubViewModePage SubViewModePage { get; set; }

			// Token: 0x17000C9A RID: 3226
			// (get) Token: 0x06002D67 RID: 11623 RVA: 0x000DE596 File Offset: 0x000DC796
			// (set) Token: 0x06002D68 RID: 11624 RVA: 0x000DE59E File Offset: 0x000DC79E
			public SubViewModeContinuous SubViewModeContinuous { get; set; } = SubViewModeContinuous.Verticalcontinuous;

			// Token: 0x17000C9B RID: 3227
			// (get) Token: 0x06002D69 RID: 11625 RVA: 0x000DE5A7 File Offset: 0x000DC7A7
			// (set) Token: 0x06002D6A RID: 11626 RVA: 0x000DE5AF File Offset: 0x000DC7AF
			public float Zoom { get; set; } = 1f;
		}
	}
}
