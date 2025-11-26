using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Models.Menus;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;

namespace pdfeditor.Controls.PdfViewerDecorators
{
	// Token: 0x02000237 RID: 567
	internal class LinkRightMenu
	{
		// Token: 0x0600202D RID: 8237 RVA: 0x00091863 File Offset: 0x0008FA63
		public LinkRightMenu(AnnotationCanvas annotationCanvas, PdfLinkAnnotation link, PdfDocument document)
		{
			if (annotationCanvas == null)
			{
				throw new ArgumentNullException("annotationCanvas");
			}
			this.annotationCanvas = annotationCanvas;
			this.linkAnnot = link;
			this.Document = document;
			this.InitContextMenu();
		}

		// Token: 0x17000AD9 RID: 2777
		// (get) Token: 0x0600202E RID: 8238 RVA: 0x00091895 File Offset: 0x0008FA95
		private MainViewModel VM
		{
			get
			{
				return this.annotationCanvas.DataContext as MainViewModel;
			}
		}

		// Token: 0x0600202F RID: 8239 RVA: 0x000918A8 File Offset: 0x0008FAA8
		private void InitContextMenu()
		{
			if (LinkRightMenu.IsDesignMode)
			{
				return;
			}
			ContextMenuItemModel contextMenuItemModel = new ContextMenuItemModel
			{
				Name = "Edit Link",
				Caption = Resources.LinkWinTitleEdit,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/LinkCE.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/LinkCE.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("PDFLink", "EditBtnClick", "ContextMenu", 1L);
					float docZoom = Ioc.Default.GetRequiredService<MainViewModel>().ViewToolbar.DocZoom;
					LinkAnnotationUtils.LinkAnnotationop(this.linkAnnot, this.Document, this.linkAnnot.Page, docZoom, this.VM);
				})
			};
			ContextMenuItemModel contextMenuItemModel2 = new ContextMenuItemModel
			{
				Name = "Delete Link",
				Caption = Resources.LinkRightDelete,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/Select_AnnotDelete.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/Select_AnnotDelete.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("PDFLink", "DeleteBtnClick", "ContextMenu", 1L);
					if (MessageBox.Show(Resources.LinkDeleteOne, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.No)
					{
						await this.VM.DeleteSelectedAnnotCmd.ExecuteAsync(null);
					}
				})
			};
			ContextMenuModel contextMenuModel = new ContextMenuModel { contextMenuItemModel, contextMenuItemModel2 };
			this.contextMenu = new PdfViewerContextMenu
			{
				ItemsSource = contextMenuModel,
				PlacementTarget = this.annotationCanvas,
				AutoCloseOnMouseLeave = false
			};
		}

		// Token: 0x06002030 RID: 8240 RVA: 0x0009199C File Offset: 0x0008FB9C
		public bool Show()
		{
			PdfViewer pdfViewer = this.annotationCanvas.PdfViewer;
			if (((pdfViewer != null) ? pdfViewer.Document : null) == null)
			{
				return false;
			}
			PdfViewer pdfViewer2 = this.annotationCanvas.PdfViewer;
			if (pdfViewer2 != null && pdfViewer2.MouseMode == MouseModes.PanTool)
			{
				return false;
			}
			if (this.annotationCanvas.HasSelectedText())
			{
				return false;
			}
			if (!(this.annotationCanvas.SelectedAnnotation is PdfLinkAnnotation))
			{
				return false;
			}
			this.contextMenu.Placement = PlacementMode.MousePoint;
			return this.contextMenu.IsOpen = true;
		}

		// Token: 0x06002031 RID: 8241 RVA: 0x00091A20 File Offset: 0x0008FC20
		public void Hide()
		{
			this.contextMenu.IsOpen = false;
			this.contextMenu.Placement = PlacementMode.Absolute;
		}

		// Token: 0x17000ADA RID: 2778
		// (get) Token: 0x06002032 RID: 8242 RVA: 0x00091A3A File Offset: 0x0008FC3A
		private static bool IsDesignMode
		{
			get
			{
				return (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
			}
		}

		// Token: 0x04000CF0 RID: 3312
		private readonly AnnotationCanvas annotationCanvas;

		// Token: 0x04000CF1 RID: 3313
		private PdfViewerContextMenu contextMenu;

		// Token: 0x04000CF2 RID: 3314
		private PdfLinkAnnotation linkAnnot;

		// Token: 0x04000CF3 RID: 3315
		private PdfDocument Document;
	}
}
