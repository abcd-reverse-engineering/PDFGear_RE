using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommonLib.Common.MessageBoxHelper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.CSharp.RuntimeBinder;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Controls;
using pdfeditor.Controls.Annotations.Holders;
using pdfeditor.Controls.Menus;
using pdfeditor.Controls.OcrComponents;
using pdfeditor.Controls.Presentation;
using pdfeditor.Controls.Speech;
using pdfeditor.Models;
using pdfeditor.Models.Menus;
using pdfeditor.Models.Menus.ToolbarSettings;
using pdfeditor.Models.Protection;
using pdfeditor.Models.Thumbnails;
using pdfeditor.Models.Viewer;
using pdfeditor.Properties;
using pdfeditor.Services;
using pdfeditor.Utils;
using pdfeditor.Utils.Enums;
using pdfeditor.Views;
using PDFKit;
using PDFKit.Contents.Utils;
using PDFKit.Utils;
using PDFKit.Utils.PageContents;

namespace pdfeditor.ViewModels
{
	// Token: 0x0200006E RID: 110
	public class ViewToolbarViewModel : ObservableObject
	{
		// Token: 0x0600080D RID: 2061 RVA: 0x00026EF7 File Offset: 0x000250F7
		public ViewToolbarViewModel(MainViewModel mainViewModel)
		{
			this.mainViewModel = mainViewModel;
			this.InitViewerButton();
		}

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x0600080E RID: 2062 RVA: 0x00026F17 File Offset: 0x00025117
		// (set) Token: 0x0600080F RID: 2063 RVA: 0x00026F1F File Offset: 0x0002511F
		public ToolbarButtonModel EditPageTextObjectButtonModel
		{
			get
			{
				return this.editPageTextObjectButtonModel;
			}
			set
			{
				base.SetProperty<ToolbarButtonModel>(ref this.editPageTextObjectButtonModel, value, "EditPageTextObjectButtonModel");
			}
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06000810 RID: 2064 RVA: 0x00026F34 File Offset: 0x00025134
		// (set) Token: 0x06000811 RID: 2065 RVA: 0x00026F3C File Offset: 0x0002513C
		public ToolbarButtonModel EditDocumentButtomModel
		{
			get
			{
				return this.editDocumentButtomModel;
			}
			set
			{
				base.SetProperty<ToolbarButtonModel>(ref this.editDocumentButtomModel, value, "EditDocumentButtomModel");
			}
		}

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x06000812 RID: 2066 RVA: 0x00026F51 File Offset: 0x00025151
		// (set) Token: 0x06000813 RID: 2067 RVA: 0x00026F59 File Offset: 0x00025159
		public ToolbarButtonModel PresentButtonModel
		{
			get
			{
				return this.presentButtonModel;
			}
			set
			{
				base.SetProperty<ToolbarButtonModel>(ref this.presentButtonModel, value, "PresentButtonModel");
			}
		}

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06000814 RID: 2068 RVA: 0x00026F6E File Offset: 0x0002516E
		// (set) Token: 0x06000815 RID: 2069 RVA: 0x00026F76 File Offset: 0x00025176
		public ToolbarButtonModel SaveAsFlattenModel
		{
			get
			{
				return this.saveAsFlattenModel;
			}
			set
			{
				base.SetProperty<ToolbarButtonModel>(ref this.saveAsFlattenModel, value, "SaveAsFlattenModel");
			}
		}

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x06000816 RID: 2070 RVA: 0x00026F8B File Offset: 0x0002518B
		// (set) Token: 0x06000817 RID: 2071 RVA: 0x00026F93 File Offset: 0x00025193
		public ToolbarSettingModel EditDocumentToolbarSetting
		{
			get
			{
				return this.editDocumentToolbarSetting;
			}
			set
			{
				if (base.SetProperty<ToolbarSettingModel>(ref this.editDocumentToolbarSetting, value, "EditDocumentToolbarSetting"))
				{
					this.mainViewModel.AnnotationToolbar.NotifyPropertyChanged("CheckedButtonToolbarSetting");
				}
			}
		}

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x06000818 RID: 2072 RVA: 0x00026FBE File Offset: 0x000251BE
		// (set) Token: 0x06000819 RID: 2073 RVA: 0x00026FC6 File Offset: 0x000251C6
		public ToolbarButtonModel ConvertToSearchableDocumentButtonModel
		{
			get
			{
				return this.convertToSearchableDocumentButtonModel;
			}
			set
			{
				base.SetProperty<ToolbarButtonModel>(ref this.convertToSearchableDocumentButtonModel, value, "ConvertToSearchableDocumentButtonModel");
			}
		}

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x0600081A RID: 2074 RVA: 0x00026FDB File Offset: 0x000251DB
		// (set) Token: 0x0600081B RID: 2075 RVA: 0x00026FE3 File Offset: 0x000251E3
		public ToolbarButtonModel TranslateButtonModel
		{
			get
			{
				return this.translateButtonModel;
			}
			set
			{
				base.SetProperty<ToolbarButtonModel>(ref this.translateButtonModel, value, "TranslateButtonModel");
			}
		}

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x0600081C RID: 2076 RVA: 0x00026FF8 File Offset: 0x000251F8
		// (set) Token: 0x0600081D RID: 2077 RVA: 0x00027000 File Offset: 0x00025200
		public ToolbarButtonModel PDFGearAIButtonModel
		{
			get
			{
				return this.pdfGearButtonModel;
			}
			set
			{
				base.SetProperty<ToolbarButtonModel>(ref this.pdfGearButtonModel, value, "PDFGearAIButtonModel");
			}
		}

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x0600081E RID: 2078 RVA: 0x00027015 File Offset: 0x00025215
		// (set) Token: 0x0600081F RID: 2079 RVA: 0x0002701D File Offset: 0x0002521D
		public SelectableContextMenuItemModel ConvertMenuItems
		{
			get
			{
				return this.convertMenuItems;
			}
			set
			{
				base.SetProperty<SelectableContextMenuItemModel>(ref this.convertMenuItems, value, "ConvertMenuItems");
			}
		}

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06000820 RID: 2080 RVA: 0x00027032 File Offset: 0x00025232
		// (set) Token: 0x06000821 RID: 2081 RVA: 0x0002703A File Offset: 0x0002523A
		public SelectableContextMenuItemModel BackgroundMenuItems
		{
			get
			{
				return this.backgroundMenuItems;
			}
			set
			{
				base.SetProperty<SelectableContextMenuItemModel>(ref this.backgroundMenuItems, value, "BackgroundMenuItems");
			}
		}

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06000822 RID: 2082 RVA: 0x0002704F File Offset: 0x0002524F
		// (set) Token: 0x06000823 RID: 2083 RVA: 0x00027057 File Offset: 0x00025257
		public SelectableContextMenuItemModel AutoScrollMenuItems
		{
			get
			{
				return this.autoScrollMenuItems;
			}
			set
			{
				base.SetProperty<SelectableContextMenuItemModel>(ref this.autoScrollMenuItems, value, "AutoScrollMenuItems");
			}
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000824 RID: 2084 RVA: 0x0002706C File Offset: 0x0002526C
		// (set) Token: 0x06000825 RID: 2085 RVA: 0x00027074 File Offset: 0x00025274
		public ToolbarAnnotationButtonModel ConvertButtonModel
		{
			get
			{
				return this.convertButtonModel;
			}
			set
			{
				base.SetProperty<ToolbarAnnotationButtonModel>(ref this.convertButtonModel, value, "ConvertButtonModel");
			}
		}

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06000826 RID: 2086 RVA: 0x00027089 File Offset: 0x00025289
		// (set) Token: 0x06000827 RID: 2087 RVA: 0x00027091 File Offset: 0x00025291
		public ToolbarAnnotationButtonModel BackgroundButtonModel
		{
			get
			{
				return this.backgroundButtonModel;
			}
			set
			{
				base.SetProperty<ToolbarAnnotationButtonModel>(ref this.backgroundButtonModel, value, "BackgroundButtonModel");
			}
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06000828 RID: 2088 RVA: 0x000270A6 File Offset: 0x000252A6
		// (set) Token: 0x06000829 RID: 2089 RVA: 0x000270AE File Offset: 0x000252AE
		public ToolbarButtonModel AutoScrollButtonModel
		{
			get
			{
				return this.autoScrollButtonModel;
			}
			set
			{
				base.SetProperty<ToolbarButtonModel>(ref this.autoScrollButtonModel, value, "AutoScrollButtonModel");
			}
		}

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x0600082A RID: 2090 RVA: 0x000270C4 File Offset: 0x000252C4
		public AsyncRelayCommand PageRotateLeftCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.pageRotateLeftCmd) == null)
				{
					asyncRelayCommand = (this.pageRotateLeftCmd = new AsyncRelayCommand(async delegate
					{
						await this.PageRotateLeft();
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x000270F8 File Offset: 0x000252F8
		private async Task PageRotateLeft()
		{
			if (this.mainViewModel.Document != null && this.mainViewModel.Document.Pages != null)
			{
				int curpgindex = this.mainViewModel.Document.Pages.CurrentIndex;
				if (curpgindex >= 0 && curpgindex <= this.mainViewModel.Document.Pages.Count)
				{
					GAManager.SendEvent("MainWindow", "PageRotate", "Left", 1L);
					global::PDFKit.PdfControl viewer = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
					AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(viewer);
					if (annotationHolderManager != null)
					{
						annotationHolderManager.CancelAll();
						await annotationHolderManager.WaitForCancelCompletedAsync();
					}
					MainViewModel.RotatePageCore(this.mainViewModel.Document, new int[] { curpgindex }, false);
					if (viewer != null && viewer.Document != null)
					{
						viewer.UpdateDocLayout();
						this.mainViewModel.SelectedPageIndex = curpgindex;
					}
					this.mainViewModel.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
					{
						MainViewModel.RotatePageCore(doc, new int[] { curpgindex }, true);
						global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
						if (pdfControl != null && pdfControl.Document != null)
						{
							pdfControl.UpdateDocLayout();
						}
					}, delegate(PdfDocument doc)
					{
						MainViewModel.RotatePageCore(this.mainViewModel.Document, new int[] { curpgindex }, false);
						global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
						if (pdfControl2 != null && pdfControl2.Document != null)
						{
							pdfControl2.UpdateDocLayout();
						}
					}, "");
				}
			}
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x0600082C RID: 2092 RVA: 0x0002713C File Offset: 0x0002533C
		public AsyncRelayCommand PageRotateRightCmd
		{
			get
			{
				AsyncRelayCommand asyncRelayCommand;
				if ((asyncRelayCommand = this.pageRotateRightCmd) == null)
				{
					asyncRelayCommand = (this.pageRotateRightCmd = new AsyncRelayCommand(async delegate
					{
						await this.PageRotateRight();
					}));
				}
				return asyncRelayCommand;
			}
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x00027170 File Offset: 0x00025370
		private async Task PageRotateRight()
		{
			if (this.mainViewModel.Document != null && this.mainViewModel.Document.Pages != null)
			{
				int curpgindex = this.mainViewModel.Document.Pages.CurrentIndex;
				if (curpgindex >= 0 && curpgindex <= this.mainViewModel.Document.Pages.Count)
				{
					GAManager.SendEvent("MainWindow", "PageRotate", "Right", 1L);
					global::PDFKit.PdfControl viewer = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
					AnnotationHolderManager annotationHolderManager = PdfObjectExtensions.GetAnnotationHolderManager(viewer);
					if (annotationHolderManager != null)
					{
						annotationHolderManager.CancelAll();
						await annotationHolderManager.WaitForCancelCompletedAsync();
					}
					MainViewModel.RotatePageCore(this.mainViewModel.Document, new int[] { curpgindex }, true);
					if (viewer != null && viewer.Document != null)
					{
						viewer.UpdateDocLayout();
						this.mainViewModel.SelectedPageIndex = curpgindex;
					}
					this.mainViewModel.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
					{
						MainViewModel.RotatePageCore(doc, new int[] { curpgindex }, false);
						global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
						if (pdfControl != null && pdfControl.Document != null)
						{
							pdfControl.UpdateDocLayout();
						}
					}, delegate(PdfDocument doc)
					{
						MainViewModel.RotatePageCore(doc, new int[] { curpgindex }, true);
						global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
						if (pdfControl2 != null && pdfControl2.Document != null)
						{
							pdfControl2.UpdateDocLayout();
						}
					}, "");
				}
			}
		}

		// Token: 0x1700021C RID: 540
		// (get) Token: 0x0600082E RID: 2094 RVA: 0x000271B3 File Offset: 0x000253B3
		// (set) Token: 0x0600082F RID: 2095 RVA: 0x000271BB File Offset: 0x000253BB
		public ViewModes DocViewMode
		{
			get
			{
				return this.docViewMode;
			}
			set
			{
				base.SetProperty<ViewModes>(ref this.docViewMode, value, "DocViewMode");
			}
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06000830 RID: 2096 RVA: 0x000271D0 File Offset: 0x000253D0
		// (set) Token: 0x06000831 RID: 2097 RVA: 0x000271D8 File Offset: 0x000253D8
		public SubViewModePage SubViewModePage
		{
			get
			{
				return this.subViewModePage;
			}
			set
			{
				base.SetProperty<SubViewModePage>(ref this.subViewModePage, value, "SubViewModePage");
				this.UpdateDocumentViewMode();
			}
		}

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06000832 RID: 2098 RVA: 0x000271F3 File Offset: 0x000253F3
		// (set) Token: 0x06000833 RID: 2099 RVA: 0x000271FB File Offset: 0x000253FB
		public SubViewModeContinuous SubViewModeContinuous
		{
			get
			{
				return this.subViewModeContinuous;
			}
			set
			{
				base.SetProperty<SubViewModeContinuous>(ref this.subViewModeContinuous, value, "SubViewModeContinuous");
				this.UpdateDocumentViewMode();
			}
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x00027218 File Offset: 0x00025418
		private void UpdateDocumentViewMode()
		{
			if (this.subViewModePage == SubViewModePage.SinglePage)
			{
				if (this.subViewModeContinuous == SubViewModeContinuous.Discontinuous)
				{
					this.DocViewMode = ViewModes.SinglePage;
					this.StopAutoScroll();
					return;
				}
				if (this.subViewModeContinuous == SubViewModeContinuous.Verticalcontinuous)
				{
					this.DocViewMode = ViewModes.Vertical;
					return;
				}
				if (this.subViewModeContinuous == SubViewModeContinuous.Horizontalcontinuous)
				{
					this.DocViewMode = ViewModes.Horizontal;
					return;
				}
			}
			else if (this.subViewModePage == SubViewModePage.DoublePages)
			{
				if (this.subViewModeContinuous == SubViewModeContinuous.Discontinuous)
				{
					this.DocViewMode = ViewModes.TilesLine;
					this.StopAutoScroll();
					return;
				}
				if (this.subViewModeContinuous == SubViewModeContinuous.Verticalcontinuous)
				{
					this.DocViewMode = ViewModes.TilesVertical;
					return;
				}
				if (this.subViewModeContinuous == SubViewModeContinuous.Horizontalcontinuous)
				{
					this.DocViewMode = ViewModes.TilesHorizontal;
				}
			}
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06000835 RID: 2101 RVA: 0x000272A5 File Offset: 0x000254A5
		// (set) Token: 0x06000836 RID: 2102 RVA: 0x000272AD File Offset: 0x000254AD
		public SizeModes DocSizeMode
		{
			get
			{
				return this.docSizeMode;
			}
			set
			{
				if (base.SetProperty<SizeModes>(ref this.docSizeMode, value, "DocSizeMode"))
				{
					this.UpdateSizeModeState();
				}
			}
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x06000837 RID: 2103 RVA: 0x000272C9 File Offset: 0x000254C9
		// (set) Token: 0x06000838 RID: 2104 RVA: 0x000272D4 File Offset: 0x000254D4
		public SizeModesWrap DocSizeModeWrap
		{
			get
			{
				return this.docSizeModeWrap;
			}
			set
			{
				if (base.SetProperty<SizeModesWrap>(ref this.docSizeModeWrap, value, "DocSizeModeWrap"))
				{
					if (value != SizeModesWrap.Zoom)
					{
						ConfigManager.SetPageSizeModeAsync(value.ToString());
						DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
						ConfigManager.SetPageSizeZoomModelAsync((documentWrapper != null) ? documentWrapper.DocumentPath : null, value.ToString(), this.DocZoom);
					}
					this.UpdateSizeModeOccordingtoWrap();
				}
			}
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x00027344 File Offset: 0x00025544
		private void UpdateSizeModeOccordingtoWrap()
		{
			switch (this.DocSizeModeWrap)
			{
			case SizeModesWrap.FitToSize:
				this.DocSizeMode = SizeModes.FitToSize;
				return;
			case SizeModesWrap.FitToWidth:
				this.DocSizeMode = SizeModes.FitToWidth;
				return;
			case SizeModesWrap.FitToHeight:
				this.DocSizeMode = SizeModes.FitToHeight;
				return;
			case SizeModesWrap.Zoom:
				this.DocSizeMode = SizeModes.Zoom;
				return;
			case SizeModesWrap.ZoomActualSize:
				this.DocSizeMode = SizeModes.Zoom;
				this.DocZoom = 1f;
				return;
			default:
				this.DocSizeMode = SizeModes.Zoom;
				return;
			}
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x000273B0 File Offset: 0x000255B0
		private void UpdateSizeModeState()
		{
			switch (this.DocSizeMode)
			{
			case SizeModes.FitToSize:
				this.DocSizeModeWrap = SizeModesWrap.FitToSize;
				return;
			case SizeModes.FitToWidth:
				this.DocSizeModeWrap = SizeModesWrap.FitToWidth;
				return;
			case SizeModes.FitToHeight:
				this.DocSizeModeWrap = SizeModesWrap.FitToHeight;
				return;
			case SizeModes.Zoom:
				if (this.DocZoom == 1f)
				{
					this.DocSizeModeWrap = SizeModesWrap.ZoomActualSize;
					return;
				}
				this.DocSizeModeWrap = SizeModesWrap.Zoom;
				return;
			default:
				return;
			}
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x0600083B RID: 2107 RVA: 0x0002740F File Offset: 0x0002560F
		// (set) Token: 0x0600083C RID: 2108 RVA: 0x00027417 File Offset: 0x00025617
		public float DocZoom
		{
			get
			{
				return this.docZoom;
			}
			set
			{
				if (base.SetProperty<float>(ref this.docZoom, value, "DocZoom"))
				{
					this.UpdateSizeModeState();
				}
				this.DocZoomOutCmd.NotifyCanExecuteChanged();
				this.DocZoomInCmd.NotifyCanExecuteChanged();
			}
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x0600083D RID: 2109 RVA: 0x0002744C File Offset: 0x0002564C
		public RelayCommand DocZoomInCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.docZoomInCmd) == null)
				{
					relayCommand = (this.docZoomInCmd = new RelayCommand(delegate
					{
						this.DocZoomIn(false, 0.1f);
					}, () => this.CanDocZoomIn()));
				}
				return relayCommand;
			}
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x0002748C File Offset: 0x0002568C
		public void DocZoomIn(bool zoomFromMousePoint = false, float zoomStep = 0.1f)
		{
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			if (this.DocZoom >= 64f)
			{
				return;
			}
			float num = Math.Min(this.DocZoom + zoomStep, 64f);
			this.UpdateDocToZoom(num, zoomFromMousePoint, null);
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x000274EB File Offset: 0x000256EB
		private bool CanDocZoomIn()
		{
			return (double)this.DocZoom < 63.9999;
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06000840 RID: 2112 RVA: 0x00027500 File Offset: 0x00025700
		public RelayCommand DocZoomOutCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.docZoomoutCmd) == null)
				{
					relayCommand = (this.docZoomoutCmd = new RelayCommand(delegate
					{
						this.DocZoomOut(false, 0.1f);
					}, () => this.CanDocZoomOut()));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x00027540 File Offset: 0x00025740
		public void DocZoomOut(bool zoomFromMousePoint = false, float zoomStep = 0.1f)
		{
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			if (this.DocZoom <= 0.01f)
			{
				return;
			}
			float num = Math.Max(this.DocZoom - zoomStep, 0.01f);
			this.UpdateDocToZoom(num, zoomFromMousePoint, null);
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x0002759F File Offset: 0x0002579F
		private bool CanDocZoomOut()
		{
			return (double)this.DocZoom > 0.010099999776482581;
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x000275B4 File Offset: 0x000257B4
		public void UpdateDocToZoom(float newzoom, bool zoomFromMousePoint = false, Point? mousePointOverride = null)
		{
			if (this.DocSizeModeWrap != SizeModesWrap.Zoom)
			{
				this.DocSizeModeWrap = SizeModesWrap.Zoom;
			}
			if (this.DocSizeMode != SizeModes.Zoom)
			{
				this.DocSizeMode = SizeModes.Zoom;
			}
			global::PDFKit.PdfControl pdfControl = null;
			ScrollAnchorPointUtils.PdfViewerZoomPointSnapshot pdfViewerZoomPointSnapshot = null;
			if (zoomFromMousePoint)
			{
				pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
				pdfViewerZoomPointSnapshot = ScrollAnchorPointUtils.CreateZoomPointSnapshot(pdfControl, mousePointOverride);
				if (pdfViewerZoomPointSnapshot != null && pdfViewerZoomPointSnapshot.MousePointPageIndex == -1 && pdfViewerZoomPointSnapshot.CenterPointPageIndex == -1)
				{
					pdfViewerZoomPointSnapshot = null;
				}
			}
			try
			{
				if (pdfControl != null)
				{
					pdfControl.IsRenderPaused = true;
				}
				this.DocZoom = newzoom;
				DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
				ConfigManager.SetPageSizeZoomModelAsync((documentWrapper != null) ? documentWrapper.DocumentPath : null, this.DocSizeModeWrap.ToString(), this.DocZoom);
				if (pdfControl != null)
				{
					pdfControl.UpdateLayout();
					ScrollAnchorPointUtils.ApplyZoomScrollOffset(pdfControl, pdfViewerZoomPointSnapshot);
				}
			}
			finally
			{
				if (pdfControl != null)
				{
					pdfControl.IsRenderPaused = false;
				}
			}
		}

		// Token: 0x06000844 RID: 2116 RVA: 0x00027690 File Offset: 0x00025890
		private void InitViewerButton()
		{
			this.convertMenuItems = ToolbarContextMenuHelper.CreateConverterContextMenu(new Action<ContextMenuItemModel>(this.DoConvertMenuItemCmd));
			this.backgroundMenuItems = ToolbarContextMenuHelper.CreateBackgroundContextMenu("", new Action<ContextMenuItemModel>(this.DoBackgroundMenuItemCmd));
			this.autoScrollMenuItems = ToolbarContextMenuHelper.CreateAutoScrollContextMenu(1, delegate(ContextMenuItemModel model)
			{
				this.AutoScrollSpeed = Convert.ToInt32(model.TagData.MenuItemValue);
				ConfigManager.SetAutoScrollSpeedAsync(this.AutoScrollSpeed).GetAwaiter().GetResult();
				if (!this.AutoScrollButtonModel.IsChecked && this.mainViewModel.Document != null)
				{
					this.AutoScrollButtonModel.Tap();
				}
			});
			this.ConvertButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.None)
			{
				Caption = Resources.WinViewToolConvertText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/convert.png"), new Uri("pack://application:,,,/Style/DarkModeResources/converter/convert.png")),
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					ContextMenu = this.convertMenuItems
				},
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.OpenContextMenuCommandFunc))
			};
			this.BackgroundButtonModel = new ToolbarAnnotationButtonModel(AnnotationMode.None)
			{
				Caption = Resources.WinViewToolBackgroundText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/Background.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/Background.png")),
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					ContextMenu = this.backgroundMenuItems
				},
				Command = new RelayCommand<ToolbarAnnotationButtonModel>(new Action<ToolbarAnnotationButtonModel>(this.OpenContextMenuCommandFunc))
			};
			this.AutoScrollButtonModel = new ToolbarButtonModel
			{
				Caption = Resources.MenuViewAutoScrollContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/autoscroll.png"), new Uri("pack://application:,,,/Style/DarkModeResources/autoscroll.png")),
				IsCheckable = true,
				Tooltip = Resources.MenuViewAutoScrollContent,
				Command = new AsyncRelayCommand<ToolbarButtonModel>(async delegate([Nullable(2)] ToolbarButtonModel model)
				{
					bool autoScroll = model.IsChecked;
					if (autoScroll && this.SubViewModeContinuous == SubViewModeContinuous.Discontinuous)
					{
						this.SubViewModeContinuous = SubViewModeContinuous.Verticalcontinuous;
					}
					global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
					AnnotationCanvas annotCanvas = PdfObjectExtensions.GetAnnotationCanvas(pdfControl);
					AnnotationCanvas annotCanvas3 = annotCanvas;
					if (((annotCanvas3 != null) ? annotCanvas3.AutoScrollHelper : null) != null)
					{
						this.mainViewModel.ExitTransientMode(false, false, true, false, false);
						await this.mainViewModel.ReleaseViewerFocusAsync(true);
						await DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
						{
							AnnotationCanvas annotCanvas2 = annotCanvas;
							if (((annotCanvas2 != null) ? annotCanvas2.AutoScrollHelper : null) != null)
							{
								if (autoScroll && annotCanvas.AutoScrollHelper.State == PdfViewerAutoScrollState.Stop)
								{
									this.ExitAnnotationSelectAndMode();
									annotCanvas.AutoScrollHelper.StartAutoScroll();
									return;
								}
								if (!autoScroll && annotCanvas.AutoScrollHelper.State == PdfViewerAutoScrollState.Scrolling)
								{
									annotCanvas.AutoScrollHelper.StopAutoScroll();
								}
							}
						}));
					}
					else
					{
						model.IsChecked = false;
					}
				}),
				ChildButtonModel = new ToolbarChildCheckableButtonModel
				{
					ContextMenu = this.autoScrollMenuItems
				}
			};
			this.EditPageTextObjectButtonModel = new ToolbarButtonModel
			{
				Caption = Resources.MenuViewEditObjectContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/object.png"), new Uri("pack://application:,,,/Style/DarkModeResources/object.png")),
				IsCheckable = true,
				Command = new AsyncRelayCommand<ToolbarButtonModel>(async delegate([Nullable(2)] ToolbarButtonModel model)
				{
					bool edit = model.IsChecked;
					model.IsChecked = false;
					this.mainViewModel.ExitTransientMode(false, true, false, false, false);
					await DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(delegate
					{
						if (edit && this.mainViewModel.Document != null)
						{
							this.ExitAnnotationSelectAndMode();
							model.IsChecked = true;
							if (ViewToolbarViewModel.<>o__115.<>p__0 == null)
							{
								ViewToolbarViewModel.<>o__115.<>p__0 = CallSite<Func<CallSite, object, MouseModes, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "Value", typeof(ViewToolbarViewModel), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
								}));
							}
							ViewToolbarViewModel.<>o__115.<>p__0.Target(ViewToolbarViewModel.<>o__115.<>p__0, this.mainViewModel.ViewerMouseMode, MouseModes.Default);
							this.mainViewModel.SelectedAnnotation = null;
							this.mainViewModel.EditingPageObjectType = PageObjectType.Text;
							GAManager.SendEvent("MainWindow", "TextEditor", "Count", 1L);
							PdfPage currentPage = this.mainViewModel.Document.Pages.CurrentPage;
							global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
							FS_RECTF effectiveBox = currentPage.GetEffectiveBox(PageRotate.Normal, false);
							AnnotationCanvas annotationCanvas = PdfObjectExtensions.GetAnnotationCanvas(pdfControl2);
							Rect rect;
							if (annotationCanvas == null || !pdfControl2.TryGetClientRect(currentPage.PageIndex, effectiveBox, out rect))
							{
								return;
							}
							double num = rect.Left;
							double num2 = rect.Top;
							double num3 = rect.Right;
							double num4 = rect.Bottom;
							if (num < 0.0)
							{
								num = 0.0;
							}
							if (num2 < 0.0)
							{
								num2 = 0.0;
							}
							if (num3 > pdfControl2.ViewportWidth)
							{
								num3 = pdfControl2.ViewportWidth;
							}
							if (num4 > pdfControl2.ViewportHeight)
							{
								num4 = pdfControl2.ViewportHeight;
							}
							FS_RECTF fs_RECTF;
							if (num >= pdfControl2.ViewportWidth || num2 >= pdfControl2.ViewportHeight || num3 <= num || num4 <= num2 || !pdfControl2.TryGetPageRect(currentPage.PageIndex, new Rect(num, num2, num3 - num, num4 - num2), out fs_RECTF))
							{
								return;
							}
							fs_RECTF.Inflate(new FS_RECTF(-10f, -10f, -10f, -10f));
							if (fs_RECTF.Width <= 0f || fs_RECTF.Height <= 0f)
							{
								return;
							}
							using (IEnumerator<PdfTextObject> enumerator = ViewToolbarViewModel.<InitViewerButton>g__GetAllTextObject|115_5(currentPage.PageObjects).OrderBy(delegate(PdfTextObject c)
							{
								try
								{
									return c.BoundingBox;
								}
								catch
								{
								}
								return new FS_RECTF(-1f, -1f, -1f, -1f);
							}, Comparer<FS_RECTF>.Create(delegate(FS_RECTF x, FS_RECTF y)
							{
								if (x == y)
								{
									return 0;
								}
								float num5 = Math.Min(x.left, x.right);
								float num6 = Math.Min(y.left, y.right);
								float num7 = Math.Max(x.top, x.bottom);
								float num8 = Math.Max(y.top, y.bottom);
								float num9 = num6 - num5;
								float num10 = num8 - num7;
								if (num10 < -10f)
								{
									return -1;
								}
								if (num10 > 10f)
								{
									return 1;
								}
								if (num9 <= 0f)
								{
									return 1;
								}
								return -1;
							})).GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									PdfTextObject pdfTextObject = enumerator.Current;
									FS_RECTF boundingBox = pdfTextObject.BoundingBox;
									if (fs_RECTF.IntersectsWith(boundingBox))
									{
										try
										{
											if (!string.IsNullOrWhiteSpace(pdfTextObject.TextUnicode))
											{
												annotationCanvas.TextObjectHolder.SelectTextObject(currentPage, pdfTextObject, true);
												break;
											}
										}
										catch
										{
										}
									}
								}
								return;
							}
						}
						this.mainViewModel.ExitTransientMode(false, false, false, false, false);
						model.IsChecked = false;
					}));
				})
			};
			this.PresentButtonModel = new ToolbarButtonModel
			{
				Caption = Resources.MenuViewPresentContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Present.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Present.png")),
				IsCheckable = false,
				Command = new RelayCommand<ToolbarButtonModel>(delegate(ToolbarButtonModel model)
				{
					GAManager.SendEvent("MainWindow", "PresentBtn", "Count", 1L);
					this.Present();
				})
			};
			this.EditDocumentButtomModel = new ToolbarButtonModel
			{
				Caption = Resources.MenuViewEditTextContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Edit_Text.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Edit_Text.png")),
				IsCheckable = false,
				Command = new AsyncRelayCommand<ToolbarButtonModel>(async delegate([Nullable(2)] ToolbarButtonModel model)
				{
					TaskAwaiter<MainViewModel.SavingExtraObjects> taskAwaiter = this.mainViewModel.GetSavingExtraObjectsAsync().GetAwaiter();
					if (!taskAwaiter.IsCompleted)
					{
						await taskAwaiter;
						TaskAwaiter<MainViewModel.SavingExtraObjects> taskAwaiter2;
						taskAwaiter = taskAwaiter2;
						taskAwaiter2 = default(TaskAwaiter<MainViewModel.SavingExtraObjects>);
					}
					if (taskAwaiter.GetResult().HasUnembeddedSignature)
					{
						ModernMessageBox.Show(Resources.PageSplitMergeCheckMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					}
					else
					{
						GAManager.SendEvent("TextEditor2", "BeginEditing", "Count", 1L);
						this.mainViewModel.Menus.SearchModel.IsSearchVisible = false;
						this.mainViewModel.QuickToolOpenModel.IsVisible = false;
						this.mainViewModel.QuickToolPrintModel.IsVisible = false;
						this.mainViewModel.ChatPanelVisible = false;
						this.mainViewModel.ChatButtonVisible = false;
						this.mainViewModel.TranslatePanelVisible = false;
						await this.mainViewModel.ReleaseViewerFocusAsync(false);
						this.mainViewModel.ExitTransientMode(false, false, false, false, false);
						this.ExitAnnotationSelectAndMode();
						this.mainViewModel.SelectedAnnotation = null;
						this.EditDocumentToolbarSetting = this.editDocumentToolbarSettingCache;
						this.UpdateEditDocumentToolbarSettingValues();
						global::PDFKit.PdfControl pdfControl3 = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
						TextProperties textProperties = pdfControl3.Editor.TextProperties;
						textProperties.PropertyChanged -= this.EditorTextProps_PropertyChanged;
						textProperties.PropertyChanged += this.EditorTextProps_PropertyChanged;
						pdfControl3.IsEditing = true;
					}
				})
			};
			this.editDocumentToolbarSettingCache = new ToolbarSettingModel(ToolbarSettingId.CreateEditDocument())
			{
				new ToolbarSettingItemBoldModel(ContextMenuItemType.FontWeightBold)
				{
					Command = new AsyncRelayCommand<ToolbarSettingItemModel>(new Func<ToolbarSettingItemModel, Task>(this.OnEditDocumentToolbarSettingInvoked))
				},
				new ToolbarSettingItemItalicModel(ContextMenuItemType.FontStyleItalic)
				{
					Command = new AsyncRelayCommand<ToolbarSettingItemModel>(new Func<ToolbarSettingItemModel, Task>(this.OnEditDocumentToolbarSettingInvoked))
				},
				new ToolbarSettingItemFontNameModel(ContextMenuItemType.FontName)
				{
					Command = new AsyncRelayCommand<ToolbarSettingItemModel>(new Func<ToolbarSettingItemModel, Task>(this.OnEditDocumentToolbarSettingInvoked))
				},
				ToolbarSettingsHelper.CreateCollapsedColor(ToolbarSettingId.CreateEditDocument(), ContextMenuItemType.FontColor, delegate(ToolbarSettingItemModel m)
				{
					this.OnEditDocumentToolbarSettingInvoked(m);
				}, null),
				ToolbarSettingsHelper.CreateFontSize(AnnotationMode.None, delegate(ToolbarSettingItemModel m)
				{
					this.OnEditDocumentToolbarSettingInvoked(m);
				}, null),
				new ToolbarSettingItemTextEditingButtonsModel
				{
					Command = new AsyncRelayCommand<ToolbarSettingItemModel>(new Func<ToolbarSettingItemModel, Task>(this.OnEditDocumentToolbarSettingInvoked))
				}
			};
			this.SaveAsFlattenModel = new ToolbarButtonModel
			{
				Caption = Resources.WinSignatureContextMenuFlatten,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Protect/SaveAsFlatten.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Protect/SaveAsFlatten.png")),
				IsCheckable = false,
				Tooltip = Resources.WinSignatureContextMenuFlatten,
				Command = new AsyncRelayCommand<ToolbarButtonModel>(async delegate([Nullable(2)] ToolbarButtonModel model)
				{
					ViewToolbarViewModel.<>c__DisplayClass115_2 CS$<>8__locals3 = new ViewToolbarViewModel.<>c__DisplayClass115_2();
					CS$<>8__locals3.<>4__this = this;
					if (this.mainViewModel.CanSave)
					{
						ModernMessageBox.Show(Resources.PageSplitMergeCheckMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					}
					else
					{
						GAManager.SendEvent("Flatten", "FlattenAnnotation", "Count", 1L);
						PdfPageEditList pageEditListItemSource = this.mainViewModel.PageEditors.PageEditListItemSource;
						PdfPageEditListModel[] array;
						if (pageEditListItemSource == null)
						{
							array = null;
						}
						else
						{
							IReadOnlyCollection<PdfPageEditListModel> selectedItems = pageEditListItemSource.SelectedItems;
							array = ((selectedItems != null) ? selectedItems.ToArray<PdfPageEditListModel>() : null);
						}
						PdfPageEditListModel[] array2 = array;
						CS$<>8__locals3.idxArr = array2.Select((PdfPageEditListModel c) => c.PageIndex).ToArray<int>();
						int[] array3;
						FlattenWindow flattenWindow = new FlattenWindow(CS$<>8__locals3.idxArr.ConvertToRange(out array3), CS$<>8__locals3.idxArr, this.mainViewModel.Document);
						flattenWindow.Owner = (MainView)Application.Current.MainWindow;
						flattenWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
						flattenWindow.ShowDialog();
						if (flattenWindow.DialogResult.GetValueOrDefault())
						{
							CS$<>8__locals3.idxArr = flattenWindow.Indexs;
							await this.mainViewModel.ReleaseViewerFocusAsync(false);
							this.mainViewModel.ExitTransientMode(false, false, false, false, false);
							await this.ExitAnnotationSelectAndMode();
							this.mainViewModel.SelectedAnnotation = null;
							string password = this.mainViewModel.Password;
							CS$<>8__locals3.result = await this.mainViewModel.SaveAsync(new MainViewModel.SaveOptions
							{
								ForceSaveAs = true,
								AllowSaveToCurrentFile = false,
								SaveAsWhenSaveFailed = false,
								DocumentModified = true,
								ValidCanSaveBeforeActionInvoke = false,
								ShowProgress = true,
								ProgressDelayTime = TimeSpan.FromSeconds(1.0),
								RemoveExistsDigitalSignaturesWhenSaveAs = true
							});
							if (CS$<>8__locals3.result.FailedResult == MainViewModel.SaveFailedResult.Successed)
							{
								CS$<>8__locals3.success = false;
								ProgressUtils.ShowProgressBar(delegate(ProgressUtils.ProgressAction task)
								{
									ViewToolbarViewModel.<>c__DisplayClass115_2.<<InitViewerButton>b__14>d <<InitViewerButton>b__14>d;
									<<InitViewerButton>b__14>d.<>t__builder = AsyncTaskMethodBuilder.Create();
									<<InitViewerButton>b__14>d.<>4__this = CS$<>8__locals3;
									<<InitViewerButton>b__14>d.task = task;
									<<InitViewerButton>b__14>d.<>1__state = -1;
									<<InitViewerButton>b__14>d.<>t__builder.Start<ViewToolbarViewModel.<>c__DisplayClass115_2.<<InitViewerButton>b__14>d>(ref <<InitViewerButton>b__14>d);
									return <<InitViewerButton>b__14>d.<>t__builder.Task;
								}, Resources.FlattenBtn, null, true, App.Current.MainWindow, 0);
								if (CS$<>8__locals3.success)
								{
									await this.mainViewModel.OpenDocumentCoreAsync(CS$<>8__locals3.result.File.FullName, password, true);
								}
								else
								{
									try
									{
										CS$<>8__locals3.result.File.Delete();
									}
									catch (Exception ex)
									{
										Log.Instance.Error<Exception>(ex);
									}
								}
							}
						}
					}
				})
			};
			this.ConvertToSearchableDocumentButtonModel = new ToolbarButtonModel
			{
				Caption = "OCR",
				Icon = new BitmapImage(new Uri("/Style/Resources/screenshot_ocr.png", UriKind.Relative)),
				IsCheckable = false,
				Tooltip = "OCR",
				Command = new AsyncRelayCommand<ToolbarButtonModel>(async delegate([Nullable(2)] ToolbarButtonModel model)
				{
					await this.DoOcr(Source.Viewer);
				})
			};
			ToolbarButtonModel toolbarButtonModel = new ToolbarButtonModel();
			toolbarButtonModel.Caption = "PDFgear AI";
			toolbarButtonModel.Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PDFAIBtn.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PDFAIBtn.png"));
			toolbarButtonModel.IsCheckable = false;
			toolbarButtonModel.Tooltip = "PDFgear AI";
			toolbarButtonModel.Command = new AsyncRelayCommand<ToolbarButtonModel>(async delegate([Nullable(2)] ToolbarButtonModel model)
			{
				GAManager.SendEvent("ChatPdf", "ToolbarAI", "Count", 1L);
				Ioc.Default.GetRequiredService<MainViewModel>().ChatPanelVisible = true;
				ConfigManager.SetChatPanelClosed(false);
			});
			this.PDFGearAIButtonModel = toolbarButtonModel;
			ToolbarButtonModel toolbarButtonModel2 = new ToolbarButtonModel();
			toolbarButtonModel2.Caption = Resources.MenuAnnotateTranslateContent;
			toolbarButtonModel2.Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/Translate.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/Translate.png"));
			toolbarButtonModel2.IsCheckable = false;
			toolbarButtonModel2.Tooltip = Resources.MenuAnnotateTranslateContent;
			toolbarButtonModel2.Command = new AsyncRelayCommand<ToolbarButtonModel>(async delegate([Nullable(2)] ToolbarButtonModel model)
			{
				Ioc.Default.GetRequiredService<MainViewModel>().TranslatePanelVisible = true;
			});
			this.TranslateButtonModel = toolbarButtonModel2;
		}

		// Token: 0x06000845 RID: 2117 RVA: 0x00027B88 File Offset: 0x00025D88
		public async Task DoOcr(Source source = Source.Default)
		{
			if (this.mainViewModel.Document != null)
			{
				await this.mainViewModel.ReleaseViewerFocusAsync(false);
				this.mainViewModel.ExitTransientMode(false, false, false, false, false);
				await this.ExitAnnotationSelectAndMode();
				this.mainViewModel.SelectedAnnotation = null;
				MessageBoxResult messageBoxResult = MessageBoxResult.Yes;
				if (this.mainViewModel.CanSave)
				{
					string documentPath = this.mainViewModel.DocumentWrapper.DocumentPath;
					messageBoxResult = ModernMessageBox.Show(new ModernMessageBoxOptions
					{
						Caption = UtilManager.GetProductName(),
						MessageBoxContent = Resources.OcrDialogSaveDocMessage.Replace("XXX", documentPath),
						Button = MessageBoxButton.YesNoCancel,
						DefaultResult = MessageBoxResult.Yes,
						UIOverrides = 
						{
							YesButtonContent = Resources.WinBtnSaveContent,
							NoButtonContent = Resources.OcrDialogSaveDocDoNotSaveBtn
						}
					});
				}
				if (messageBoxResult != MessageBoxResult.Cancel)
				{
					bool flag = messageBoxResult != MessageBoxResult.No;
					new ConvertToSearchablePdfDialog(this.mainViewModel.Document, flag, source)
					{
						Owner = App.Current.MainWindow,
						WindowStartupLocation = WindowStartupLocation.CenterOwner
					}.ShowDialog();
				}
			}
		}

		// Token: 0x06000846 RID: 2118 RVA: 0x00027BD3 File Offset: 0x00025DD3
		private void EditorTextProps_PropertyChanged(object sender, EventArgs e)
		{
			this.UpdateEditDocumentToolbarSettingValues();
		}

		// Token: 0x06000847 RID: 2119 RVA: 0x00027BDC File Offset: 0x00025DDC
		private void UpdateEditDocumentToolbarSettingValues()
		{
			ToolbarSettingModel toolbarSettingModel = this.EditDocumentToolbarSetting;
			if (toolbarSettingModel == null)
			{
				return;
			}
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			if (!pdfControl.IsEditing)
			{
				return;
			}
			TextProperties textProperties = pdfControl.Editor.TextProperties;
			foreach (ToolbarSettingItemModel toolbarSettingItemModel in toolbarSettingModel)
			{
				string text;
				object obj2;
				if (toolbarSettingItemModel.Type == ContextMenuItemType.FontWeightBold)
				{
					((ToolbarSettingItemBoldModel)toolbarSettingItemModel).SelectedValue = textProperties.IsBold;
				}
				else if (toolbarSettingItemModel.Type == ContextMenuItemType.FontStyleItalic)
				{
					((ToolbarSettingItemItalicModel)toolbarSettingItemModel).SelectedValue = textProperties.IsItalic;
				}
				else if (toolbarSettingItemModel.Type == ContextMenuItemType.FontName)
				{
					ToolbarSettingItemModel toolbarSettingItemModel2 = (ToolbarSettingItemFontNameModel)toolbarSettingItemModel;
					FontData font = textProperties.GetFont();
					toolbarSettingItemModel2.SelectedValue = ((font != null) ? font.FontFamily : null);
				}
				else if (toolbarSettingItemModel.Type == ContextMenuItemType.FontColor)
				{
					object obj;
					if (ToolbarContextMenuHelper.TryParseMenuValue(AnnotationMode.None, ContextMenuItemType.FontColor, textProperties.TextColor, out text, out obj))
					{
						((ToolbarSettingItemColorCollapseModel)toolbarSettingItemModel).SelectedValue = obj;
					}
				}
				else if (toolbarSettingItemModel.Type == ContextMenuItemType.FontSize && ToolbarContextMenuHelper.TryParseMenuValue(AnnotationMode.None, ContextMenuItemType.FontSize, textProperties.FontSize, out text, out obj2))
				{
					((ToolbarSettingItemFontSizeModel)toolbarSettingItemModel).SelectedValue = obj2;
				}
			}
		}

		// Token: 0x06000848 RID: 2120 RVA: 0x00027D30 File Offset: 0x00025F30
		private void ExitEditing()
		{
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			TextProperties textProperties = ((pdfControl != null) ? pdfControl.Editor.TextProperties : null);
			bool flag;
			if (this.IsDocumentEdited)
			{
				GAManager.SendEvent("TextEditor2", "ExitEditing", "ExitBtnDocEdited", 1L);
				MessageBoxResult messageBoxResult = ModernMessageBox.Show(Resources.ViewToorbarFileChanged, UtilManager.GetProductName(), MessageBoxButton.YesNoCancel, MessageBoxResult.None, null, false);
				if (messageBoxResult == MessageBoxResult.Cancel)
				{
					GAManager.SendEvent("TextEditor2", "ExitEditingChoice", "Cancel", 1L);
					return;
				}
				if (messageBoxResult == MessageBoxResult.Yes)
				{
					GAManager.SendEvent("TextEditor2", "ExitEditingChoice", "KeepEditing", 1L);
					flag = true;
					this.UpdateEditedDocumentContent(true);
					this.mainViewModel.SetCanSaveFlag("Editor", true);
				}
				else
				{
					GAManager.SendEvent("TextEditor2", "ExitEditingChoice", "DiscardEditing", 1L);
					flag = true;
					this.UpdateEditedDocumentContent(false);
				}
			}
			else
			{
				GAManager.SendEvent("TextEditor2", "ExitEditing", "ExitBtnDocNotEdited", 1L);
				flag = true;
			}
			if (flag)
			{
				this.EditDocumentToolbarSetting = null;
				if (pdfControl != null)
				{
					pdfControl.IsEditing = false;
				}
				if (textProperties != null)
				{
					textProperties.PropertyChanged -= this.EditorTextProps_PropertyChanged;
				}
				this.mainViewModel.QuickToolOpenModel.IsVisible = true;
				this.mainViewModel.QuickToolPrintModel.IsVisible = true;
				this.mainViewModel.ChatButtonVisible = ConfigManager.GetShowcaseChatButtonFlag();
				this.mainViewModel.ChatPanelVisible = !ConfigManager.GetChatPanelClosed();
			}
		}

		// Token: 0x06000849 RID: 2121 RVA: 0x00027E94 File Offset: 0x00026094
		private Task OnEditDocumentToolbarSettingInvoked(ToolbarSettingItemModel model)
		{
			if (model == null)
			{
				return Task.CompletedTask;
			}
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
			TextProperties textProperties = pdfControl.Editor.TextProperties;
			if (!pdfControl.IsEditing)
			{
				return Task.CompletedTask;
			}
			if (model is ToolbarSettingItemExitModel)
			{
				this.ExitEditing();
				return Task.CompletedTask;
			}
			if (model.Type == ContextMenuItemType.FontWeightBold)
			{
				GAManager.SendEvent("TextEditor2", "TextEditingTools", "FontWeightBold", 1L);
				if ((bool)((ToolbarSettingItemBoldModel)model).SelectedValue != textProperties.IsBold)
				{
					textProperties.ToggleBold();
				}
			}
			else if (model.Type == ContextMenuItemType.FontStyleItalic)
			{
				GAManager.SendEvent("TextEditor2", "TextEditingTools", "FontStyleItalic", 1L);
				if ((bool)((ToolbarSettingItemItalicModel)model).SelectedValue != textProperties.IsItalic)
				{
					textProperties.ToggleItalic();
				}
			}
			else if (model.Type == ContextMenuItemType.FontName)
			{
				GAManager.SendEvent("TextEditor2", "TextEditingTools", "FontName", 1L);
				string text = (string)((ToolbarSettingItemFontNameModel)model).SelectedValue;
				if (!string.IsNullOrEmpty(text))
				{
					textProperties.SetFont(new FontData(text));
				}
			}
			else if (model.Type == ContextMenuItemType.FontColor)
			{
				GAManager.SendEvent("TextEditor2", "TextEditingTools", "FontColor", 1L);
				FS_COLOR fs_COLOR = ((Color)ColorConverter.ConvertFromString((string)((ToolbarSettingItemColorCollapseModel)model).SelectedValue)).ToPdfColor();
				textProperties.TextColor = new FS_COLOR?(fs_COLOR);
			}
			else if (model.Type == ContextMenuItemType.FontSize)
			{
				GAManager.SendEvent("TextEditor2", "TextEditingTools", "FontSize", 1L);
				textProperties.FontSize = (float)((ToolbarSettingItemFontSizeModel)model).SelectedValue;
			}
			this.UpdateEditDocumentToolbarSettingValues();
			return Task.CompletedTask;
		}

		// Token: 0x0600084A RID: 2122 RVA: 0x0002804C File Offset: 0x0002624C
		private async Task ExitAnnotationSelectAndMode()
		{
			MainViewModel mainViewModel = this.mainViewModel;
			if (((mainViewModel != null) ? mainViewModel.Document : null) != null)
			{
				this.mainViewModel.AnnotationMode = AnnotationMode.None;
				await this.mainViewModel.ReleaseViewerFocusAsync(true).ConfigureAwait(false);
			}
		}

		// Token: 0x0600084B RID: 2123 RVA: 0x00028090 File Offset: 0x00026290
		private void OpenContextMenuCommandFunc(ToolbarAnnotationButtonModel model)
		{
			ViewToolbarViewModel.<>c__DisplayClass122_0 CS$<>8__locals1 = new ViewToolbarViewModel.<>c__DisplayClass122_0();
			CS$<>8__locals1.model = model;
			DispatcherHelper.UIDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				ViewToolbarViewModel.<>c__DisplayClass122_0.<<OpenContextMenuCommandFunc>b__0>d <<OpenContextMenuCommandFunc>b__0>d;
				<<OpenContextMenuCommandFunc>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<OpenContextMenuCommandFunc>b__0>d.<>4__this = CS$<>8__locals1;
				<<OpenContextMenuCommandFunc>b__0>d.<>1__state = -1;
				<<OpenContextMenuCommandFunc>b__0>d.<>t__builder.Start<ViewToolbarViewModel.<>c__DisplayClass122_0.<<OpenContextMenuCommandFunc>b__0>d>(ref <<OpenContextMenuCommandFunc>b__0>d);
			}));
		}

		// Token: 0x0600084C RID: 2124 RVA: 0x000280C4 File Offset: 0x000262C4
		private void DoConvertMenuItemCmd(ContextMenuItemModel model)
		{
			if (this.mainViewModel.Document == null)
			{
				return;
			}
			string name = (model as ConvertContextMenuItemModel).Name;
			if (name != null)
			{
				switch (name.Length)
				{
				case 8:
				{
					char c = name[1];
					if (c <= 'P')
					{
						if (c != 'D')
						{
							if (c != 'P')
							{
								return;
							}
							if (!(name == "PPTtoPDF"))
							{
								return;
							}
							this.mainViewModel.ConverterCommands.DoPPTToPDF();
							return;
						}
						else
						{
							if (name == "PDFtoPPT")
							{
								this.mainViewModel.ConverterCommands.DoPDFToPPT();
								return;
							}
							if (name == "PDFtoRtf")
							{
								this.mainViewModel.ConverterCommands.DoPDFToRtf();
								return;
							}
							if (!(name == "PDFtoXml"))
							{
								return;
							}
							this.mainViewModel.ConverterCommands.DoPDFToXml();
							return;
						}
					}
					else if (c != 't')
					{
						if (c != 'x')
						{
							return;
						}
						if (!(name == "TxttoPDF"))
						{
							return;
						}
						this.mainViewModel.ConverterCommands.DoTxtToPDF();
					}
					else
					{
						if (!(name == "RtftoPDF"))
						{
							return;
						}
						this.mainViewModel.ConverterCommands.DoRtfToPDF();
						return;
					}
					break;
				}
				case 9:
				{
					char c = name[5];
					if (c <= 'J')
					{
						if (c != 'H')
						{
							if (c != 'J')
							{
								return;
							}
							if (!(name == "PDFtoJpeg"))
							{
								return;
							}
							this.mainViewModel.ConverterCommands.DoPDFToJpeg();
							return;
						}
						else
						{
							if (!(name == "PDFtoHtml"))
							{
								return;
							}
							this.mainViewModel.ConverterCommands.DoPDFToHtml();
							return;
						}
					}
					else if (c != 'T')
					{
						if (c != 'W')
						{
							if (c != 'o')
							{
								return;
							}
							if (!(name == "WordtoPDF"))
							{
								return;
							}
							this.mainViewModel.ConverterCommands.DoWordToPDF();
							return;
						}
						else
						{
							if (!(name == "PDFtoWord"))
							{
								return;
							}
							this.mainViewModel.ConverterCommands.DoPDFToWord(null);
							return;
						}
					}
					else
					{
						if (!(name == "PDFtoText"))
						{
							return;
						}
						this.mainViewModel.ConverterCommands.DoPDFToTxt();
						return;
					}
					break;
				}
				case 10:
				{
					char c = name[0];
					if (c != 'E')
					{
						if (c != 'I')
						{
							if (c != 'P')
							{
								return;
							}
							if (name == "PDFtoExcel")
							{
								this.mainViewModel.ConverterCommands.DoPDFToExcel();
								return;
							}
							if (!(name == "PDFtoImage"))
							{
								return;
							}
							this.mainViewModel.ConverterCommands.DoPDFToImage();
							return;
						}
						else
						{
							if (!(name == "ImagetoPDF"))
							{
								return;
							}
							this.mainViewModel.ConverterCommands.DoImageToPDF();
							return;
						}
					}
					else
					{
						if (!(name == "ExceltoPDF"))
						{
							return;
						}
						this.mainViewModel.ConverterCommands.DoExcelToPDF();
						return;
					}
					break;
				}
				default:
					return;
				}
			}
		}

		// Token: 0x0600084D RID: 2125 RVA: 0x00028388 File Offset: 0x00026588
		private void DoBackgroundMenuItemCmd(ContextMenuItemModel model)
		{
			this.TryUpdateViewerBackground();
			SelectableContextMenuItemModel selectableContextMenuItemModel = this.backgroundMenuItems;
			object obj;
			if (selectableContextMenuItemModel == null)
			{
				obj = null;
			}
			else
			{
				ContextMenuItemModel selectedItem = selectableContextMenuItemModel.SelectedItem;
				obj = ((selectedItem != null) ? selectedItem.TagData.MenuItemValue : null);
			}
			BackgroundColorSetting backgroundColorSetting = obj as BackgroundColorSetting;
			if (backgroundColorSetting != null)
			{
				ConfigManager.SetBackgroundAsync(backgroundColorSetting.Name, backgroundColorSetting.PageMaskColor.ToString(), backgroundColorSetting.BackgroundColor.ToString());
			}
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x000283FC File Offset: 0x000265FC
		public void SetViewerBackground(string settingName)
		{
			ContextMenuItemModel contextMenuItemModel = this.backgroundMenuItems.OfType<ContextMenuItemModel>().FirstOrDefault(delegate(ContextMenuItemModel c)
			{
				object obj;
				if (c == null)
				{
					obj = null;
				}
				else
				{
					TagDataModel tagData = c.TagData;
					obj = ((tagData != null) ? tagData.MenuItemValue : null);
				}
				BackgroundColorSetting backgroundColorSetting = obj as BackgroundColorSetting;
				return ((backgroundColorSetting != null) ? backgroundColorSetting.Name : null) == settingName;
			});
			if (contextMenuItemModel != null && contextMenuItemModel.IsCheckable)
			{
				contextMenuItemModel.IsChecked = true;
				this.DoBackgroundMenuItemCmd(contextMenuItemModel);
			}
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x0002844C File Offset: 0x0002664C
		public void TryUpdateViewerBackground()
		{
			if (this.mainViewModel.Document == null)
			{
				return;
			}
			SelectableContextMenuItemModel selectableContextMenuItemModel = this.BackgroundMenuItems;
			object obj;
			if (selectableContextMenuItemModel == null)
			{
				obj = null;
			}
			else
			{
				ContextMenuItemModel selectedItem = selectableContextMenuItemModel.SelectedItem;
				if (selectedItem == null)
				{
					obj = null;
				}
				else
				{
					TagDataModel tagData = selectedItem.TagData;
					obj = ((tagData != null) ? tagData.MenuItemValue : null);
				}
			}
			BackgroundColorSetting backgroundColorSetting = obj as BackgroundColorSetting;
			if (backgroundColorSetting != null)
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
				if (pdfControl != null)
				{
					pdfControl.PageMaskBrush = new SolidColorBrush(backgroundColorSetting.PageMaskColor);
					pdfControl.PageBackground = new SolidColorBrush(backgroundColorSetting.BackgroundColor);
				}
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06000850 RID: 2128 RVA: 0x000284D0 File Offset: 0x000266D0
		public RelayCommand GotoPrevPageCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.gotoPrevPageCmd) == null)
				{
					relayCommand = (this.gotoPrevPageCmd = new RelayCommand(delegate
					{
						this.GotoPrevPage();
					}, () => this.CanGotoPrevPage()));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000851 RID: 2129 RVA: 0x00028510 File Offset: 0x00026710
		private void GotoPrevPage()
		{
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			int num = this.mainViewModel.Document.Pages.CurrentIndex;
			if (num < 0 || num > this.mainViewModel.Document.Pages.Count)
			{
				return;
			}
			num--;
			if (num < 0)
			{
				return;
			}
			this.mainViewModel.SelectedPageIndex = num;
		}

		// Token: 0x06000852 RID: 2130 RVA: 0x00028585 File Offset: 0x00026785
		private bool CanGotoPrevPage()
		{
			return true;
		}

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06000853 RID: 2131 RVA: 0x00028588 File Offset: 0x00026788
		public RelayCommand GotoNextPageCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.gotoNextPageCmd) == null)
				{
					relayCommand = (this.gotoNextPageCmd = new RelayCommand(delegate
					{
						this.GotoNextPage();
					}, () => this.CanGotoNextPage()));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x000285C8 File Offset: 0x000267C8
		private void GotoNextPage()
		{
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			int num = this.mainViewModel.Document.Pages.CurrentIndex;
			if (num < 0 || num >= this.mainViewModel.Document.Pages.Count - 1)
			{
				return;
			}
			num++;
			if (num > this.mainViewModel.Document.Pages.Count - 1)
			{
				return;
			}
			this.mainViewModel.SelectedPageIndex = num;
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x00028655 File Offset: 0x00026855
		private bool CanGotoNextPage()
		{
			return true;
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06000856 RID: 2134 RVA: 0x00028658 File Offset: 0x00026858
		public RelayCommand GotoFirstPageCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.gotoFirstPageCmd) == null)
				{
					relayCommand = (this.gotoFirstPageCmd = new RelayCommand(delegate
					{
						this.GotoFirstPage();
					}, () => this.CanGotoFirstPage()));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000857 RID: 2135 RVA: 0x00028695 File Offset: 0x00026895
		private void GotoFirstPage()
		{
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			this.mainViewModel.SelectedPageIndex = 0;
		}

		// Token: 0x06000858 RID: 2136 RVA: 0x000286C3 File Offset: 0x000268C3
		private bool CanGotoFirstPage()
		{
			return true;
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x06000859 RID: 2137 RVA: 0x000286C8 File Offset: 0x000268C8
		public RelayCommand GotoLastPageCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.gotoLastPageCmd) == null)
				{
					relayCommand = (this.gotoLastPageCmd = new RelayCommand(delegate
					{
						this.GotoLastPage();
					}, () => this.CanGotoLastPage()));
				}
				return relayCommand;
			}
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x00028708 File Offset: 0x00026908
		private void GotoLastPage()
		{
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			this.mainViewModel.SelectedPageIndex = this.mainViewModel.Document.Pages.Count - 1;
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x00028757 File Offset: 0x00026957
		private bool CanGotoLastPage()
		{
			return true;
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x0600085C RID: 2140 RVA: 0x0002875C File Offset: 0x0002695C
		public RelayCommand GotoPrevViewCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.gotoPrevViewCmd) == null)
				{
					relayCommand = (this.gotoPrevViewCmd = new RelayCommand(delegate
					{
						this.GotoPrevView();
					}, () => this.CanGotoPrevView()));
				}
				return relayCommand;
			}
		}

		// Token: 0x0600085D RID: 2141 RVA: 0x0002879C File Offset: 0x0002699C
		private void GotoPrevView()
		{
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			int currentIndex = this.mainViewModel.Document.Pages.CurrentIndex;
			this.mainViewModel.Jumping = true;
			if (!this.mainViewModel.ViewJumpManager.IsFirstView)
			{
				this.mainViewModel.SelectedPageIndex = this.mainViewModel.ViewJumpManager.ViewBackCmd(currentIndex);
			}
		}

		// Token: 0x0600085E RID: 2142 RVA: 0x00028819 File Offset: 0x00026A19
		private bool CanGotoPrevView()
		{
			return true;
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x0600085F RID: 2143 RVA: 0x0002881C File Offset: 0x00026A1C
		public RelayCommand GotoNextViewCmd
		{
			get
			{
				RelayCommand relayCommand;
				if ((relayCommand = this.gotoNextViewCmd) == null)
				{
					relayCommand = (this.gotoNextViewCmd = new RelayCommand(delegate
					{
						this.GotoNextView();
					}, () => this.CanGotoPrevView()));
				}
				return relayCommand;
			}
		}

		// Token: 0x06000860 RID: 2144 RVA: 0x0002885C File Offset: 0x00026A5C
		private void GotoNextView()
		{
			if (this.mainViewModel.Document == null || this.mainViewModel.Document.Pages == null)
			{
				return;
			}
			int currentIndex = this.mainViewModel.Document.Pages.CurrentIndex;
			this.mainViewModel.Jumping = true;
			if (!this.mainViewModel.ViewJumpManager.IsLastView)
			{
				this.mainViewModel.SelectedPageIndex = this.mainViewModel.ViewJumpManager.ViewPreCmd(currentIndex);
			}
		}

		// Token: 0x06000861 RID: 2145 RVA: 0x000288D9 File Offset: 0x00026AD9
		private bool CanGotoNextView()
		{
			return true;
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x06000862 RID: 2146 RVA: 0x000288DC File Offset: 0x00026ADC
		// (set) Token: 0x06000863 RID: 2147 RVA: 0x000288E4 File Offset: 0x00026AE4
		public int AutoScrollSpeed
		{
			get
			{
				return this.autoScrollSpeed;
			}
			set
			{
				base.SetProperty<int>(ref this.autoScrollSpeed, value, "AutoScrollSpeed");
			}
		}

		// Token: 0x06000864 RID: 2148 RVA: 0x000288F9 File Offset: 0x00026AF9
		public void PauseAutoScroll(int milliseconds)
		{
			AnnotationCanvas annotationCanvas = PdfObjectExtensions.GetAnnotationCanvas(global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document));
			if (annotationCanvas == null)
			{
				return;
			}
			PdfViewerAutoScrollHelper autoScrollHelper = annotationCanvas.AutoScrollHelper;
			if (autoScrollHelper == null)
			{
				return;
			}
			autoScrollHelper.Pause(milliseconds);
		}

		// Token: 0x06000865 RID: 2149 RVA: 0x00028928 File Offset: 0x00026B28
		public void StopAutoScroll()
		{
			if (this.AutoScrollButtonModel != null && this.AutoScrollButtonModel.IsChecked)
			{
				this.AutoScrollButtonModel.IsChecked = false;
				this.AutoScrollButtonModel.Command.Execute(this.AutoScrollButtonModel.CommandParameter ?? this.AutoScrollButtonModel);
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06000866 RID: 2150 RVA: 0x0002897C File Offset: 0x00026B7C
		public bool IsDocumentEdited
		{
			get
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.mainViewModel.Document);
				return pdfControl != null && pdfControl.IsEditing && pdfControl.GetChangedPageIndexes().Length != 0;
			}
		}

		// Token: 0x06000867 RID: 2151 RVA: 0x000289B4 File Offset: 0x00026BB4
		public async Task<bool> DocumentEditedSaveAsync(bool forceSaveAs = false)
		{
			ViewToolbarViewModel.<>c__DisplayClass158_0 CS$<>8__locals1 = new ViewToolbarViewModel.<>c__DisplayClass158_0();
			CS$<>8__locals1.<>4__this = this;
			bool flag;
			if (this.IsDocumentEdited)
			{
				ViewToolbarViewModel.<>c__DisplayClass158_0 CS$<>8__locals2 = CS$<>8__locals1;
				DocumentWrapper documentWrapper = this.mainViewModel.DocumentWrapper;
				string text;
				if (documentWrapper == null)
				{
					text = null;
				}
				else
				{
					EncryptManage encryptManage = documentWrapper.EncryptManage;
					text = ((encryptManage != null) ? encryptManage.UserPassword : null);
				}
				CS$<>8__locals2.password = text;
				if (string.IsNullOrEmpty(CS$<>8__locals1.password))
				{
					CS$<>8__locals1.password = null;
				}
				MainViewModel.SavingExtraObjects savingExtraObjects = await this.mainViewModel.GetSavingExtraObjectsAsync();
				CS$<>8__locals1.savingExtraObjects = savingExtraObjects;
				if (this.mainViewModel.DocumentWrapper.IsUntitledFile)
				{
					forceSaveAs = true;
				}
				TaskAwaiter<MainViewModel.SaveResult> taskAwaiter = this.mainViewModel.SaveAsync(new MainViewModel.SaveOptions
				{
					ForceSaveAs = forceSaveAs,
					SaveAsWhenSaveFailed = true,
					ShowProgress = forceSaveAs,
					AllowSaveToCurrentFile = !forceSaveAs,
					ValidCanSaveBeforeActionInvoke = true,
					InitialFileNamePostfixOverride = "Edited",
					RemoveExistsDigitalSignaturesWhenSaveAs = true,
					BeforeSaveAction = delegate(MainViewModel.SaveOptions options, MainViewModel.BeforeSaveActionArgs args)
					{
						ViewToolbarViewModel.<>c__DisplayClass158_0.<<DocumentEditedSaveAsync>b__0>d <<DocumentEditedSaveAsync>b__0>d;
						<<DocumentEditedSaveAsync>b__0>d.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
						<<DocumentEditedSaveAsync>b__0>d.<>4__this = CS$<>8__locals1;
						<<DocumentEditedSaveAsync>b__0>d.<>1__state = -1;
						<<DocumentEditedSaveAsync>b__0>d.<>t__builder.Start<ViewToolbarViewModel.<>c__DisplayClass158_0.<<DocumentEditedSaveAsync>b__0>d>(ref <<DocumentEditedSaveAsync>b__0>d);
						return <<DocumentEditedSaveAsync>b__0>d.<>t__builder.Task;
					},
					AfterSaveAction = delegate(MainViewModel.SaveOptions options, MainViewModel.SaveResult result)
					{
						ViewToolbarViewModel.<>c__DisplayClass158_0.<<DocumentEditedSaveAsync>b__1>d <<DocumentEditedSaveAsync>b__1>d;
						<<DocumentEditedSaveAsync>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<DocumentEditedSaveAsync>b__1>d.<>4__this = CS$<>8__locals1;
						<<DocumentEditedSaveAsync>b__1>d.result = result;
						<<DocumentEditedSaveAsync>b__1>d.<>1__state = -1;
						<<DocumentEditedSaveAsync>b__1>d.<>t__builder.Start<ViewToolbarViewModel.<>c__DisplayClass158_0.<<DocumentEditedSaveAsync>b__1>d>(ref <<DocumentEditedSaveAsync>b__1>d);
						return <<DocumentEditedSaveAsync>b__1>d.<>t__builder.Task;
					}
				}).GetAwaiter();
				if (!taskAwaiter.IsCompleted)
				{
					await taskAwaiter;
					TaskAwaiter<MainViewModel.SaveResult> taskAwaiter2;
					taskAwaiter = taskAwaiter2;
					taskAwaiter2 = default(TaskAwaiter<MainViewModel.SaveResult>);
				}
				flag = taskAwaiter.GetResult().FailedResult == MainViewModel.SaveFailedResult.Successed;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000868 RID: 2152 RVA: 0x00028A00 File Offset: 0x00026C00
		private bool UpdateEditedDocumentContent(bool applyEdit)
		{
			PdfDocument document = this.mainViewModel.Document;
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(document);
			if (pdfControl != null && pdfControl.IsEditing)
			{
				int[] changedPageIndexes = pdfControl.GetChangedPageIndexes();
				if (changedPageIndexes.Length != 0)
				{
					pdfControl.IsRenderPaused = true;
					pdfControl.ClearEditorUndoStack();
					try
					{
						foreach (int num in changedPageIndexes)
						{
							PdfPage pdfPage = document.Pages[num];
							if (applyEdit)
							{
								pdfPage.GenerateContentAdvance(new GenerateContentOptions
								{
									ImagesOnly = false,
									KeepHeaderFooterData = true
								});
							}
							else
							{
								pdfPage.Dispose();
							}
						}
					}
					finally
					{
						pdfControl.IsRenderPaused = false;
					}
					Ioc.Default.GetService<PdfThumbnailService>().RefreshThumbnail(changedPageIndexes);
					return true;
				}
			}
			return false;
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06000869 RID: 2153 RVA: 0x00028ABC File Offset: 0x00026CBC
		public ToolbarButtonModel ReadButtonModel
		{
			get
			{
				ToolbarButtonModel toolbarButtonModel;
				if ((toolbarButtonModel = this.readButtonModel) == null)
				{
					ToolbarButtonModel toolbarButtonModel2 = new ToolbarButtonModel();
					toolbarButtonModel2.Caption = Resources.ReadWinTitle;
					toolbarButtonModel2.Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/speach.png"), new Uri("pack://application:,,,/Style/DarkModeResources/speach.png"));
					toolbarButtonModel2.IsChecked = false;
					toolbarButtonModel2.ChildButtonModel = new ToolbarChildCheckableButtonModel
					{
						ContextMenu = new ContextMenuModel
						{
							ToolbarContextMenuHelper.SpeakCurrent(null),
							ToolbarContextMenuHelper.SpeakFormCurrent(null),
							ToolbarContextMenuHelper.SpeakAll(null),
							ToolbarContextMenuHelper.SpeechToolbarMenu(null)
						}
					};
					toolbarButtonModel2.Command = new RelayCommand(delegate
					{
						MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
						ContextMenuModel contextMenuModel = (requiredService.ViewToolbar.ReadButtonModel.ChildButtonModel as ToolbarChildCheckableButtonModel).ContextMenu as ContextMenuModel;
						if (!requiredService.IsReading)
						{
							if (requiredService.Document == null)
							{
								requiredService.ViewToolbar.ReadButtonModel.IsChecked = false;
								return;
							}
							PdfDocument document = requiredService.Document;
							requiredService.ViewToolbar.ReadButtonModel.IsChecked = true;
							requiredService.IsReading = true;
							SpeechUtils speechUtils = requiredService.speechUtils;
							if (speechUtils != null)
							{
								speechUtils.Dispose();
							}
							requiredService.speechUtils = new SpeechUtils(document);
							(contextMenuModel[0] as SpeedContextMenuItemModel).IsEnabled = false;
							(contextMenuModel[1] as SpeedContextMenuItemModel).IsEnabled = false;
							(contextMenuModel[2] as SpeedContextMenuItemModel).IsEnabled = false;
							if (requiredService.speechControl == null)
							{
								requiredService.speechControl = new SpeechControl();
								requiredService.speechControl.Owner = App.Current.MainWindow;
								if (requiredService.speechControl.Owner.WindowState == WindowState.Normal)
								{
									requiredService.speechControl.Top = requiredService.speechControl.Owner.Top + 152.0;
									requiredService.speechControl.Left = requiredService.speechControl.Owner.Left + requiredService.speechControl.Owner.ActualWidth - 520.0;
								}
								else if (requiredService.speechControl.Owner.WindowState == WindowState.Maximized)
								{
									requiredService.speechControl.Top = 152.0;
									requiredService.speechControl.Left = requiredService.speechControl.Owner.ActualWidth - 520.0;
								}
								requiredService.speechControl.Show();
								if (requiredService.speechUtils.ProcessorStream == null)
								{
									requiredService.speechUtils.SpeakPages(requiredService.CurrnetPageIndex - 1, requiredService.Document.Pages.Count - 1);
									(contextMenuModel[1] as SpeedContextMenuItemModel).IsChecked = true;
									GAManager.SendEvent("PDFReader", "Read", "FromCurrentPageDefault", 1L);
									return;
								}
							}
							else
							{
								requiredService.speechUtils.Rate = requiredService.speechControl.SpeedSli.Value * 2.0 - 10.0;
								requiredService.speechUtils.SpeechVolume = (float)Convert.ToInt32(requiredService.speechControl.VolumeSlider.Value);
								requiredService.speechUtils.Pitch = (double)Convert.ToInt32(requiredService.speechControl.ToneSli.Value - 5.0);
								if (requiredService.speechControl.CultureListBox.SelectedIndex < 0)
								{
									requiredService.speechUtils.CultureIndex = requiredService.speechUtils.GetcultureIndex();
								}
								else
								{
									requiredService.speechUtils.CultureIndex = requiredService.speechControl.CultureListBox.SelectedIndex;
								}
								if ((contextMenuModel[0] as SpeedContextMenuItemModel).IsChecked)
								{
									requiredService.speechUtils.SpeakCurrentPage(requiredService.CurrnetPageIndex - 1);
									(contextMenuModel[0] as SpeedContextMenuItemModel).IsChecked = true;
									return;
								}
								if ((contextMenuModel[1] as SpeedContextMenuItemModel).IsChecked)
								{
									requiredService.speechUtils.SpeakPages(requiredService.CurrnetPageIndex - 1, requiredService.Document.Pages.Count - 1);
									(contextMenuModel[1] as SpeedContextMenuItemModel).IsChecked = true;
									return;
								}
								requiredService.speechUtils.SpeakPages(0, requiredService.Document.Pages.Count - 1);
								(contextMenuModel[2] as SpeedContextMenuItemModel).IsChecked = true;
								return;
							}
						}
						else
						{
							requiredService.ViewToolbar.ReadButtonModel.IsChecked = false;
							requiredService.IsReading = false;
							SpeechUtils speechUtils2 = requiredService.speechUtils;
							if (speechUtils2 != null)
							{
								speechUtils2.Dispose();
							}
							requiredService.speechUtils = null;
							(contextMenuModel[0] as SpeedContextMenuItemModel).IsEnabled = true;
							(contextMenuModel[1] as SpeedContextMenuItemModel).IsEnabled = true;
							(contextMenuModel[2] as SpeedContextMenuItemModel).IsEnabled = true;
							(contextMenuModel[0] as SpeedContextMenuItemModel).Icon = null;
							(contextMenuModel[1] as SpeedContextMenuItemModel).Icon = null;
							(contextMenuModel[2] as SpeedContextMenuItemModel).Icon = null;
						}
					});
					ToolbarButtonModel toolbarButtonModel3 = toolbarButtonModel2;
					this.readButtonModel = toolbarButtonModel2;
					toolbarButtonModel = toolbarButtonModel3;
				}
				return toolbarButtonModel;
			}
		}

		// Token: 0x0600086A RID: 2154 RVA: 0x00028B88 File Offset: 0x00026D88
		public void Present()
		{
			if (App.Current.Windows.OfType<PresentationWindow>().Any<PresentationWindow>())
			{
				return;
			}
			if (this.mainViewModel.Document == null)
			{
				return;
			}
			this.mainViewModel.ExitTransientMode(false, false, false, false, false);
			new PresentationWindow(this.mainViewModel.Document, this.mainViewModel.CurrentFileName)
			{
				PageIndex = this.mainViewModel.SelectedPageIndex
			}.Show();
		}

		// Token: 0x06000874 RID: 2164 RVA: 0x00028DA7 File Offset: 0x00026FA7
		[CompilerGenerated]
		internal static IEnumerable<PdfTextObject> <InitViewerButton>g__GetAllTextObject|115_5(PdfPageObjectsCollection _collection)
		{
			ViewToolbarViewModel.<<InitViewerButton>g__GetAllTextObject|115_5>d <<InitViewerButton>g__GetAllTextObject|115_5>d = new ViewToolbarViewModel.<<InitViewerButton>g__GetAllTextObject|115_5>d(-2);
			<<InitViewerButton>g__GetAllTextObject|115_5>d.<>3___collection = _collection;
			return <<InitViewerButton>g__GetAllTextObject|115_5>d;
		}

		// Token: 0x0400041E RID: 1054
		private readonly MainViewModel mainViewModel;

		// Token: 0x0400041F RID: 1055
		private const float ZoomMinValue = 0.01f;

		// Token: 0x04000420 RID: 1056
		private const float ZoomMaxValue = 64f;

		// Token: 0x04000421 RID: 1057
		private const float ZoomStep = 0.1f;

		// Token: 0x04000422 RID: 1058
		private SelectableContextMenuItemModel convertMenuItems;

		// Token: 0x04000423 RID: 1059
		private SelectableContextMenuItemModel backgroundMenuItems;

		// Token: 0x04000424 RID: 1060
		private SelectableContextMenuItemModel autoScrollMenuItems;

		// Token: 0x04000425 RID: 1061
		private ToolbarAnnotationButtonModel convertButtonModel;

		// Token: 0x04000426 RID: 1062
		private ToolbarAnnotationButtonModel backgroundButtonModel;

		// Token: 0x04000427 RID: 1063
		private ToolbarButtonModel autoScrollButtonModel;

		// Token: 0x04000428 RID: 1064
		private AsyncRelayCommand pageRotateLeftCmd;

		// Token: 0x04000429 RID: 1065
		private AsyncRelayCommand pageRotateRightCmd;

		// Token: 0x0400042A RID: 1066
		private ViewModes docViewMode;

		// Token: 0x0400042B RID: 1067
		private SubViewModePage subViewModePage;

		// Token: 0x0400042C RID: 1068
		private SubViewModeContinuous subViewModeContinuous;

		// Token: 0x0400042D RID: 1069
		private SizeModes docSizeMode;

		// Token: 0x0400042E RID: 1070
		private SizeModesWrap docSizeModeWrap;

		// Token: 0x0400042F RID: 1071
		private float docZoom = 1f;

		// Token: 0x04000430 RID: 1072
		private RelayCommand docZoomInCmd;

		// Token: 0x04000431 RID: 1073
		private RelayCommand docZoomoutCmd;

		// Token: 0x04000432 RID: 1074
		private RelayCommand gotoPrevPageCmd;

		// Token: 0x04000433 RID: 1075
		private RelayCommand gotoNextPageCmd;

		// Token: 0x04000434 RID: 1076
		private RelayCommand gotoFirstPageCmd;

		// Token: 0x04000435 RID: 1077
		private RelayCommand gotoLastPageCmd;

		// Token: 0x04000436 RID: 1078
		private RelayCommand gotoPrevViewCmd;

		// Token: 0x04000437 RID: 1079
		private RelayCommand gotoNextViewCmd;

		// Token: 0x04000438 RID: 1080
		private ToolbarButtonModel editPageTextObjectButtonModel;

		// Token: 0x04000439 RID: 1081
		private ToolbarButtonModel editDocumentButtomModel;

		// Token: 0x0400043A RID: 1082
		private ToolbarButtonModel presentButtonModel;

		// Token: 0x0400043B RID: 1083
		private ToolbarButtonModel saveAsFlattenModel;

		// Token: 0x0400043C RID: 1084
		private ToolbarButtonModel convertToSearchableDocumentButtonModel;

		// Token: 0x0400043D RID: 1085
		private ToolbarButtonModel translateButtonModel;

		// Token: 0x0400043E RID: 1086
		private ToolbarButtonModel pdfGearButtonModel;

		// Token: 0x0400043F RID: 1087
		private ToolbarSettingModel editDocumentToolbarSetting;

		// Token: 0x04000440 RID: 1088
		private ToolbarSettingModel editDocumentToolbarSettingCache;

		// Token: 0x04000441 RID: 1089
		private int autoScrollSpeed;

		// Token: 0x04000442 RID: 1090
		private ToolbarButtonModel readButtonModel;
	}
}
