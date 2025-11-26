using System;
using System.ComponentModel;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Patagames.Pdf.Net;
using pdfeditor.Models.Menus;
using pdfeditor.Properties;
using pdfeditor.ViewModels;
using PDFKit;

namespace pdfeditor.Models.Thumbnails
{
	// Token: 0x02000137 RID: 311
	public class PdfThumbnailModel : ObservableObject
	{
		// Token: 0x060012F1 RID: 4849 RVA: 0x0004D748 File Offset: 0x0004B948
		public PdfThumbnailModel(PdfDocument document, int pageIndex)
		{
			this.Document = document;
			this.PageIndex = pageIndex;
			this.InitContextMenu();
			this.ActualThumbnailWidth = 150.0 * this.VM.ThumbnailScale;
			this.VM.PropertyChanged += this.VM_PropertyChanged;
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x0004D7A1 File Offset: 0x0004B9A1
		private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ThumbnailScale" && this.VM != null)
			{
				this.ActualThumbnailWidth = 150.0 * this.VM.ThumbnailScale;
			}
		}

		// Token: 0x170007B0 RID: 1968
		// (get) Token: 0x060012F3 RID: 4851 RVA: 0x0004D7D8 File Offset: 0x0004B9D8
		public PdfDocument Document { get; }

		// Token: 0x170007B1 RID: 1969
		// (get) Token: 0x060012F4 RID: 4852 RVA: 0x0004D7E0 File Offset: 0x0004B9E0
		public int PageIndex { get; }

		// Token: 0x170007B2 RID: 1970
		// (get) Token: 0x060012F5 RID: 4853 RVA: 0x0004D7E8 File Offset: 0x0004B9E8
		public int DisplayPageIndex
		{
			get
			{
				return this.PageIndex + 1;
			}
		}

		// Token: 0x170007B3 RID: 1971
		// (get) Token: 0x060012F6 RID: 4854 RVA: 0x0004D7F2 File Offset: 0x0004B9F2
		// (set) Token: 0x060012F7 RID: 4855 RVA: 0x0004D7FA File Offset: 0x0004B9FA
		public ContextMenuModel ContextMenuModel { get; set; }

		// Token: 0x170007B4 RID: 1972
		// (get) Token: 0x060012F8 RID: 4856 RVA: 0x0004D803 File Offset: 0x0004BA03
		// (set) Token: 0x060012F9 RID: 4857 RVA: 0x0004D80B File Offset: 0x0004BA0B
		public double ActualThumbnailWidth
		{
			get
			{
				return this.actualThumnailWidth;
			}
			set
			{
				this.actualThumnailWidth = value;
				base.OnPropertyChanged("ActualThumbnailWidth");
			}
		}

		// Token: 0x170007B5 RID: 1973
		// (get) Token: 0x060012FA RID: 4858 RVA: 0x0004D81F File Offset: 0x0004BA1F
		public MainViewModel VM
		{
			get
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.Document);
				return ((pdfControl != null) ? pdfControl.DataContext : null) as MainViewModel;
			}
		}

		// Token: 0x060012FB RID: 4859 RVA: 0x0004D840 File Offset: 0x0004BA40
		private void InitContextMenu()
		{
			ContextMenuHorizontalButton contextMenuHorizontalButton = new ContextMenuHorizontalButton
			{
				Name = "HeaderButton",
				Caption0 = Resources.MenuPageDeleteContent,
				Icon0 = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/Delete.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/Delete.png")),
				Command0 = new AsyncRelayCommand(async delegate
				{
					await this.VM.PageEditors.SiderbarDeleteCmd.ExecuteAsync(null);
				}),
				Caption1 = Resources.MenuPageRotateLeftContent,
				Icon1 = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_RotateLeft.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_RotateLeft.png")),
				Command1 = new RelayCommand(delegate
				{
					this.VM.PageEditors.SiderbarRotateLeftCmd.Execute(null);
				}),
				Caption2 = Resources.MenuPageRotateRightContent,
				Icon2 = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_RotateRight.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_RotateRight.png")),
				Command2 = new RelayCommand(delegate
				{
					this.VM.PageEditors.SiderbarRotateRightCmd.Execute(null);
				}),
				Caption3 = Resources.WinViewToolPrintTooltipText,
				Icon3 = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_Print.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_Print.png")),
				Command3 = new RelayCommand(delegate
				{
					AsyncRelayCommand printDocCmdFromThumbnail = this.VM.PrintDocCmdFromThumbnail;
					if (printDocCmdFromThumbnail == null)
					{
						return;
					}
					printDocCmdFromThumbnail.Execute(null);
				})
			};
			ContextMenuItemModel contextMenuItemModel = new ContextMenuItemModel
			{
				Name = "InsertPage",
				Caption = Resources.RightMenuInsertPageText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_InsertPage.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_InsertPage.png"))
			};
			ContextMenuItemModel contextMenuItemModel2 = new ContextMenuItemModel
			{
				Name = "BlankPage",
				Caption = Resources.MenuPageSubInsertBlankPage,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/BlankPage.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/BlankPage.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.PageEditors.FormBlankPage.Execute(null);
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel3 = new ContextMenuItemModel
			{
				Name = "FromPDF",
				Caption = Resources.RightMenuInsertPageItemPDF,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/FromPDF.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/FromPDF.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.PageEditors.FormPDF.Execute(null);
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel4 = new ContextMenuItemModel
			{
				Name = "FromWord",
				Caption = Resources.RightMenuInsertPageItemWord,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/FromWord.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/FromWord.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.PageEditors.FormWord.Execute(null);
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel5 = new ContextMenuItemModel
			{
				Name = "FromScanner",
				Caption = Resources.MenuPageSubInsertFromScanner,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/FromScanner.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/FromScanner.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					PdfPageEditListModel pdfPageEditListModel = new PdfPageEditListModel(this.VM.Document, this.VM.SelectedPageIndex);
					await this.VM.PageEditors.PageEditorInsertFromScannerCmd.ExecuteAsync(pdfPageEditListModel);
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel6 = new ContextMenuItemModel
			{
				Name = "FromImage",
				Caption = Resources.RightMenuInsertPageItemImage,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/FromImage.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/FromImage.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.PageEditors.FormImage.Execute(null);
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel7 = new ContextMenuItemModel
			{
				Name = "ExtractPages",
				Caption = Resources.ShortcutTextPageExtract,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_ExtractPages.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_ExtractPages.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("PageContextMenu", "ExtractPages", "Count", 1L);
					await this.VM.PageEditors.SiderbarExtractCmd.ExecuteAsync(null);
				})
			};
			ContextMenuItemModel contextMenuItemModel8 = new ContextMenuItemModel
			{
				Name = "DeletePages",
				Caption = Resources.ShortcutTextPageDelete,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/Delete.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/Delete.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("PageContextMenu", "DeletePages", "Count", 1L);
					await this.VM.PageEditors.SiderbarDeleteCmd.ExecuteAsync(null);
				})
			};
			ContextMenuItemModel contextMenuItemModel9 = new ContextMenuItemModel
			{
				Name = "CropPages",
				Caption = Resources.MainViewCropPageContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_CropPages.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_CropPages.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("PageContextMenu", "CropPages", "Count", 1L);
					this.VM.PageEditors.SiderbarCropPageCmd.Execute(null);
				})
			};
			ContextMenuItemModel contextMenuItemModel10 = new ContextMenuItemModel
			{
				Name = "ResizePages",
				Caption = Resources.PageResizeWinTitle,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/PageResize.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/PageResize.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("PageContextMenu", "ResizePages", "Count", 1L);
					this.VM.PageEditors.SiderbarResizePageCmd.Execute(null);
				})
			};
			ContextMenuItemModel contextMenuItemModel11 = new ContextMenuItemModel
			{
				Name = "OCR Pages",
				Caption = Resources.RightMenuOcrPagesItemText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_OcrPages.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_OcrPages.png")),
				Command = new RelayCommand(async delegate
				{
					GAManager.SendEvent("PageContextMenu", "OcrPages", "Count", 1L);
					await this.VM.ViewToolbar.DoOcr(Source.Thumbnail);
				})
			};
			foreach (IContextMenuModel contextMenuModel in new ContextMenuModel { contextMenuItemModel2, contextMenuItemModel3, contextMenuItemModel5, contextMenuItemModel4, contextMenuItemModel6 })
			{
				contextMenuItemModel.Add(contextMenuModel);
			}
			this.ContextMenuModel = new ContextMenuModel
			{
				contextMenuHorizontalButton,
				new ContextMenuSeparator(),
				contextMenuItemModel,
				contextMenuItemModel8,
				contextMenuItemModel7,
				new ContextMenuSeparator(),
				contextMenuItemModel9,
				contextMenuItemModel10,
				contextMenuItemModel11
			};
		}

		// Token: 0x040005FF RID: 1535
		private double actualThumnailWidth;
	}
}
