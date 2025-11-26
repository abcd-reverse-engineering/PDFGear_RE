using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using pdfeditor.Controls.PageEditor;
using pdfeditor.ViewModels;
using pdfeditor.Views;
using PDFKit;
using PDFKit.Utils.PageHeaderFooters;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000214 RID: 532
	public partial class ScreenshotCropPageToolbar : UserControl
	{
		// Token: 0x17000A5F RID: 2655
		// (get) Token: 0x06001D76 RID: 7542 RVA: 0x0007EF1D File Offset: 0x0007D11D
		public MainViewModel VM
		{
			get
			{
				return Ioc.Default.GetRequiredService<MainViewModel>();
			}
		}

		// Token: 0x06001D77 RID: 7543 RVA: 0x0007EF29 File Offset: 0x0007D129
		public ScreenshotCropPageToolbar()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000A60 RID: 2656
		// (get) Token: 0x06001D78 RID: 7544 RVA: 0x0007EF37 File Offset: 0x0007D137
		// (set) Token: 0x06001D79 RID: 7545 RVA: 0x0007EF49 File Offset: 0x0007D149
		public ScreenshotDialog ScreenshotDialog
		{
			get
			{
				return (ScreenshotDialog)base.GetValue(ScreenshotCropPageToolbar.ScreenshotDialogProperty);
			}
			set
			{
				base.SetValue(ScreenshotCropPageToolbar.ScreenshotDialogProperty, value);
			}
		}

		// Token: 0x17000A61 RID: 2657
		// (get) Token: 0x06001D7A RID: 7546 RVA: 0x0007EF57 File Offset: 0x0007D157
		// (set) Token: 0x06001D7B RID: 7547 RVA: 0x0007EF69 File Offset: 0x0007D169
		public MarginModel PageMargin
		{
			get
			{
				return (MarginModel)base.GetValue(ScreenshotCropPageToolbar.PageMarginProperty);
			}
			set
			{
				base.SetValue(ScreenshotCropPageToolbar.PageMarginProperty, value);
			}
		}

		// Token: 0x17000A62 RID: 2658
		// (get) Token: 0x06001D7C RID: 7548 RVA: 0x0007EF77 File Offset: 0x0007D177
		// (set) Token: 0x06001D7D RID: 7549 RVA: 0x0007EF89 File Offset: 0x0007D189
		public PageSizeModel PageSize
		{
			get
			{
				return (PageSizeModel)base.GetValue(ScreenshotCropPageToolbar.PageSizeProperty);
			}
			set
			{
				base.SetValue(ScreenshotCropPageToolbar.PageSizeProperty, value);
			}
		}

		// Token: 0x06001D7E RID: 7550 RVA: 0x0007EF97 File Offset: 0x0007D197
		private static void PageMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
		}

		// Token: 0x17000A63 RID: 2659
		// (get) Token: 0x06001D7F RID: 7551 RVA: 0x0007EF99 File Offset: 0x0007D199
		// (set) Token: 0x06001D80 RID: 7552 RVA: 0x0007EFAB File Offset: 0x0007D1AB
		public bool SelectionBorderVisible
		{
			get
			{
				return (bool)base.GetValue(ScreenshotCropPageToolbar.SelectionBorderVisibleProperty);
			}
			set
			{
				base.SetValue(ScreenshotCropPageToolbar.SelectionBorderVisibleProperty, value);
			}
		}

		// Token: 0x17000A64 RID: 2660
		// (get) Token: 0x06001D81 RID: 7553 RVA: 0x0007EFBE File Offset: 0x0007D1BE
		// (set) Token: 0x06001D82 RID: 7554 RVA: 0x0007EFD0 File Offset: 0x0007D1D0
		public bool PageRangeBorderVisible
		{
			get
			{
				return (bool)base.GetValue(ScreenshotCropPageToolbar.PageRangeBorderVisibleProperty);
			}
			set
			{
				base.SetValue(ScreenshotCropPageToolbar.PageRangeBorderVisibleProperty, value);
			}
		}

		// Token: 0x06001D83 RID: 7555 RVA: 0x0007EFE4 File Offset: 0x0007D1E4
		private void RangeButton_Click(object sender, RoutedEventArgs e)
		{
			CropPageApplyWin cropPageApplyWin = new CropPageApplyWin(this.ApplypageIndex);
			if (cropPageApplyWin.ShowDialog().GetValueOrDefault() && cropPageApplyWin.ApplyPageIndex != null)
			{
				this.ApplypageIndex = cropPageApplyWin.ApplyPageIndex;
				if (Array.IndexOf<int>(this.ApplypageIndex.ToArray<int>(), this.ScreenshotDialog.pageIdx) < 0)
				{
					this.ScreenshotDialog.pageIdx = this.ApplypageIndex[0];
					this.ScreenshotDialog.ReflashViewerselectedBox(this.ApplypageIndex);
					this.ScreenshotDialog.ResetSelectedBox(this.ApplypageIndex[0]);
					this.VM.CurrnetPageIndex = this.ApplypageIndex[0] + 1;
					return;
				}
				this.ScreenshotDialog.ReflashViewerselectedBox(this.ApplypageIndex);
				this.ScreenshotDialog.CreateSeleteBounds(false);
			}
		}

		// Token: 0x06001D84 RID: 7556 RVA: 0x0007F0AF File Offset: 0x0007D2AF
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("CropPage", "CancelBtn", "Count", 1L);
			ScreenshotDialog screenshotDialog = this.ScreenshotDialog;
			if (screenshotDialog != null)
			{
				screenshotDialog.Close(null);
			}
			this.ApplypageIndex = null;
		}

		// Token: 0x06001D85 RID: 7557 RVA: 0x0007F0E0 File Offset: 0x0007D2E0
		private void AcceptButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("CropPage", "DoneBtn", "Count", 1L);
			this.ApplypageIndex = new int[] { this.ScreenshotDialog.pageIdx };
			this.ScreenshotDialog.CompleteCropPageAsync(this.ApplypageIndex, false);
			this.ApplypageIndex = null;
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.VM.Document);
			SortedDictionary<int, FS_RECTF> pageCropBoxInfo = ((pdfControl != null) ? pdfControl.Viewer : null).PageCropBoxInfo;
			if (pageCropBoxInfo == null)
			{
				return;
			}
			pageCropBoxInfo.Clear();
		}

		// Token: 0x06001D86 RID: 7558 RVA: 0x0007F163 File Offset: 0x0007D363
		private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
		{
		}

		// Token: 0x06001D87 RID: 7559 RVA: 0x0007F168 File Offset: 0x0007D368
		private void SetApplyPageIndexCropBoxsInfo(int[] applyIndexs, FS_RECTF rect)
		{
			SortedDictionary<int, FS_RECTF> sortedDictionary = new SortedDictionary<int, FS_RECTF>();
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.VM.Document);
			PdfViewer pdfViewer = ((pdfControl != null) ? pdfControl.Viewer : null);
			for (int i = 0; i < applyIndexs.Length; i++)
			{
				sortedDictionary.Add(applyIndexs[i], rect);
			}
			pdfViewer.PageCropBoxInfo = sortedDictionary;
		}

		// Token: 0x06001D88 RID: 7560 RVA: 0x0007F1B7 File Offset: 0x0007D3B7
		private void btn_PageRangeBorder_Close_Click(object sender, RoutedEventArgs e)
		{
			this.PageRangeBorderVisible = false;
			this.ApplypageIndex = null;
		}

		// Token: 0x06001D89 RID: 7561 RVA: 0x0007F1C8 File Offset: 0x0007D3C8
		public void OpenAdvanceWindow()
		{
			FS_RECTF fs_RECTF = new FS_RECTF
			{
				right = Math.Max(this.ScreenshotDialog.curPt.X, this.ScreenshotDialog.startPt.X),
				bottom = Math.Min(this.ScreenshotDialog.startPt.Y, this.ScreenshotDialog.curPt.Y),
				left = Math.Min(this.ScreenshotDialog.curPt.X, this.ScreenshotDialog.startPt.X),
				top = Math.Max(this.ScreenshotDialog.startPt.Y, this.ScreenshotDialog.curPt.Y)
			};
			this.ScreenshotDialog.startPt.X = fs_RECTF.left;
			this.ScreenshotDialog.startPt.Y = fs_RECTF.top;
			this.ScreenshotDialog.curPt.X = fs_RECTF.right;
			this.ScreenshotDialog.curPt.Y = fs_RECTF.bottom;
			if (PageHeaderFooterUtils.PdfPointToCm((double)Math.Abs(fs_RECTF.Width)) < 0.5 || PageHeaderFooterUtils.PdfPointToCm((double)Math.Abs(fs_RECTF.Height)) < 0.5)
			{
				FS_RECTF fs_RECTF2 = default(FS_RECTF);
				if (this.VM.Document.Pages[this.ScreenshotDialog.pageIdx].Dictionary.ContainsKey("CropBox"))
				{
					fs_RECTF = this.VM.Document.Pages[this.ScreenshotDialog.pageIdx].CropBox;
				}
				else
				{
					fs_RECTF = this.VM.Document.Pages[this.ScreenshotDialog.pageIdx].MediaBox;
				}
				this.ScreenshotDialog.startPt.X = fs_RECTF2.left;
				this.ScreenshotDialog.startPt.Y = fs_RECTF2.top;
				this.ScreenshotDialog.curPt.X = fs_RECTF2.right;
				this.ScreenshotDialog.curPt.Y = fs_RECTF2.bottom;
				this.ScreenshotDialog.UpdatePriviewBounds(false, false);
			}
			try
			{
				PdfDocument pdfDocument = PdfDocument.CreateNew(null);
				pdfDocument.Pages.ImportPages(this.VM.Document, string.Format("1-{0}", this.VM.Document.Pages.Count), 0);
				this.ScreenshotDialog.RootBorder.Visibility = Visibility.Hidden;
				this.ScreenshotDialog.RenderBox(this.ScreenshotDialog.pageIdx);
				new CropPageWindow(pdfDocument, this.ScreenshotDialog.pageIdx, fs_RECTF, this.PageMargin, this.PageSize, this.ScreenshotDialog)
				{
					Owner = Application.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>()
				}.ShowDialog();
			}
			catch
			{
				ModernMessageBox.Show("An unknown error has occurred.", UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
		}

		// Token: 0x06001D8A RID: 7562 RVA: 0x0007F4E8 File Offset: 0x0007D6E8
		private void AdvanceButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("CropPage", "AdvanceBtn", "Count", 1L);
			this.OpenAdvanceWindow();
		}

		// Token: 0x04000B16 RID: 2838
		public int[] ApplypageIndex;

		// Token: 0x04000B17 RID: 2839
		public static readonly DependencyProperty ScreenshotDialogProperty = DependencyProperty.Register("ScreenshotDialog", typeof(ScreenshotDialog), typeof(ScreenshotCropPageToolbar), new PropertyMetadata(null));

		// Token: 0x04000B18 RID: 2840
		public static readonly DependencyProperty PageMarginProperty = DependencyProperty.Register("PageMargin", typeof(MarginModel), typeof(ScreenshotCropPageToolbar), new PropertyMetadata(null));

		// Token: 0x04000B19 RID: 2841
		public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register("PageSize", typeof(PageSizeModel), typeof(ScreenshotCropPageToolbar), new PropertyMetadata(null));

		// Token: 0x04000B1A RID: 2842
		public static readonly DependencyProperty SelectionBorderVisibleProperty = DependencyProperty.Register("SelectionBorderVisible", typeof(bool), typeof(ScreenshotCropPageToolbar), new PropertyMetadata(false));

		// Token: 0x04000B1B RID: 2843
		public static readonly DependencyProperty PageRangeBorderVisibleProperty = DependencyProperty.Register("PageRangeBorderVisible", typeof(bool), typeof(ScreenshotCropPageToolbar), new PropertyMetadata(false));
	}
}
