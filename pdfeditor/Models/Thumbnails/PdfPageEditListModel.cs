using System;
using CommonLib.AppTheme;
using CommonLib.Common.HotKeys;
using CommunityToolkit.Mvvm.Input;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Models.Menus;
using pdfeditor.Properties;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Thumbnails
{
	// Token: 0x02000136 RID: 310
	public class PdfPageEditListModel : PdfThumbnailModel
	{
		// Token: 0x060012D3 RID: 4819 RVA: 0x0004CCD8 File Offset: 0x0004AED8
		public PdfPageEditListModel(PdfDocument document, int pageIndex)
			: base(document, pageIndex)
		{
			Pdfium.FPDF_GetPageSizeByIndex(document.Handle, pageIndex, out this.pageWidth, out this.pageHeight);
			Pdfium.FPDF_GetPageRotationByIndex(document.Handle, pageIndex, out this.pageRotate);
			if (this.pageRotate == PageRotate.Rotate90 || this.pageRotate == PageRotate.Rotate270)
			{
				double num = this.pageWidth;
				this.pageWidth = this.pageHeight;
				this.pageHeight = num;
			}
			this.InitContextMenu();
		}

		// Token: 0x170007A9 RID: 1961
		// (get) Token: 0x060012D4 RID: 4820 RVA: 0x0004CD4B File Offset: 0x0004AF4B
		public static double DefaultThumbnailWidth
		{
			get
			{
				return 300.0;
			}
		}

		// Token: 0x170007AA RID: 1962
		// (get) Token: 0x060012D5 RID: 4821 RVA: 0x0004CD56 File Offset: 0x0004AF56
		// (set) Token: 0x060012D6 RID: 4822 RVA: 0x0004CD5E File Offset: 0x0004AF5E
		public bool Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				base.SetProperty<bool>(ref this.selected, value, "Selected");
			}
		}

		// Token: 0x170007AB RID: 1963
		// (get) Token: 0x060012D7 RID: 4823 RVA: 0x0004CD73 File Offset: 0x0004AF73
		// (set) Token: 0x060012D8 RID: 4824 RVA: 0x0004CD7B File Offset: 0x0004AF7B
		public double PageWidth
		{
			get
			{
				return this.pageWidth;
			}
			set
			{
				base.SetProperty<double>(ref this.pageWidth, value, "PageWidth");
			}
		}

		// Token: 0x170007AC RID: 1964
		// (get) Token: 0x060012D9 RID: 4825 RVA: 0x0004CD90 File Offset: 0x0004AF90
		// (set) Token: 0x060012DA RID: 4826 RVA: 0x0004CD98 File Offset: 0x0004AF98
		public double PageHeight
		{
			get
			{
				return this.pageHeight;
			}
			set
			{
				base.SetProperty<double>(ref this.pageHeight, value, "PageHeight");
			}
		}

		// Token: 0x170007AD RID: 1965
		// (get) Token: 0x060012DB RID: 4827 RVA: 0x0004CDAD File Offset: 0x0004AFAD
		// (set) Token: 0x060012DC RID: 4828 RVA: 0x0004CDB5 File Offset: 0x0004AFB5
		public PageRotate PageRotate
		{
			get
			{
				return this.pageRotate;
			}
			set
			{
				base.SetProperty<PageRotate>(ref this.pageRotate, value, "PageRotate");
			}
		}

		// Token: 0x170007AE RID: 1966
		// (get) Token: 0x060012DD RID: 4829 RVA: 0x0004CDCA File Offset: 0x0004AFCA
		// (set) Token: 0x060012DE RID: 4830 RVA: 0x0004CDD2 File Offset: 0x0004AFD2
		public double ThumbnailWidth
		{
			get
			{
				return this.thumbnailWidth;
			}
			set
			{
				base.SetProperty<double>(ref this.thumbnailWidth, value, "ThumbnailWidth");
			}
		}

		// Token: 0x170007AF RID: 1967
		// (get) Token: 0x060012DF RID: 4831 RVA: 0x0004CDE7 File Offset: 0x0004AFE7
		// (set) Token: 0x060012E0 RID: 4832 RVA: 0x0004CDEF File Offset: 0x0004AFEF
		public double ThumbnailHeight
		{
			get
			{
				return this.thumbnailHeight;
			}
			set
			{
				base.SetProperty<double>(ref this.thumbnailHeight, value, "ThumbnailHeight");
			}
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x0004CE04 File Offset: 0x0004B004
		internal void UpdateThumbnailSize(double scale, double minAspectRatio)
		{
			if (minAspectRatio == 0.0)
			{
				this.ThumbnailWidth = 0.0;
				this.ThumbnailHeight = 0.0;
				return;
			}
			double num = PdfPageEditListModel.DefaultThumbnailWidth * scale;
			double num2 = num * 1.414 * 2.0;
			double num3 = Math.Min(num / minAspectRatio, num2);
			this.ThumbnailWidth = num;
			this.ThumbnailHeight = num3;
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x0004CE74 File Offset: 0x0004B074
		private void InitContextMenu()
		{
			ContextMenuHorizontalButton contextMenuHorizontalButton = new ContextMenuHorizontalButton
			{
				Name = "HeaderButton",
				Caption0 = Resources.MenuPageDeleteContent,
				Icon0 = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/Delete.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/Delete.png")),
				Command0 = new AsyncRelayCommand(async delegate
				{
					await base.VM.PageEditors.PageEditorDeleteCmd.ExecuteAsync(null);
				}),
				Caption1 = Resources.MenuPageRotateLeftContent,
				Icon1 = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_RotateLeft.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_RotateLeft.png")),
				Command1 = new RelayCommand(delegate
				{
					base.VM.PageEditors.PageEditorRotateLeftCmd.Execute(null);
				}),
				Caption2 = Resources.MenuPageRotateRightContent,
				Icon2 = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_RotateRight.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_RotateRight.png")),
				Command2 = new RelayCommand(delegate
				{
					base.VM.PageEditors.PageEditorRotateRightCmd.Execute(null);
				}),
				Caption3 = Resources.WinViewToolPrintTooltipText,
				Icon3 = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_Print.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_Print.png")),
				Command3 = new RelayCommand(delegate
				{
					base.VM.QuickToolPrintModel.Command.Execute(null);
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
					await base.VM.PageEditors.PageEditorInsertBlankCmd.ExecuteAsync(null);
				}),
				IsCheckable = false,
				HotKeyInvokeWhen = "Editor_InsertBlankPage",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel3 = new ContextMenuItemModel
			{
				Name = "FromPDF",
				Caption = Resources.RightMenuInsertPageItemPDF,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/FromPDF.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/FromPDF.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					await base.VM.PageEditors.PageEditorInsertFromPDFCmd.ExecuteAsync(null);
				}),
				IsCheckable = false,
				HotKeyInvokeWhen = "Editor_InsertPDF",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel4 = new ContextMenuItemModel
			{
				Name = "FromWord",
				Caption = Resources.RightMenuInsertPageItemWord,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/FromWord.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/FromWord.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					await base.VM.PageEditors.PageEditorInsertFromWordCmd.ExecuteAsync(null);
				}),
				IsCheckable = false,
				HotKeyInvokeWhen = "Editor_InsertWord",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel5 = new ContextMenuItemModel
			{
				Name = "FromScanner",
				Caption = Resources.MenuPageSubInsertFromScanner,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/FromScanner.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/FromScanner.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					PdfPageEditListModel pdfPageEditListModel = new PdfPageEditListModel(base.VM.Document, base.VM.SelectedPageIndex);
					await base.VM.PageEditors.PageEditorInsertFromScannerCmd.ExecuteAsync(pdfPageEditListModel);
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
					await base.VM.PageEditors.PageEditorInsertFromImageCmd.ExecuteAsync(null);
				}),
				IsCheckable = false,
				HotKeyInvokeWhen = "Editor_InsertImage",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel7 = new ContextMenuItemModel
			{
				Name = "ExtractPages",
				Caption = Resources.ShortcutTextPageExtract,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_ExtractPages.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_ExtractPages.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					await base.VM.PageEditors.PageEditorExtractCmd.ExecuteAsync(null);
				}),
				HotKeyInvokeWhen = "Editor_ExtractPage",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel8 = new ContextMenuItemModel
			{
				Name = "DeletePages",
				Caption = Resources.ShortcutTextPageDelete,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/Delete.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/Delete.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					await base.VM.PageEditors.PageEditorDeleteCmd.ExecuteAsync(null);
				}),
				HotKeyInvokeWhen = "Editor_DeletePage",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel9 = new ContextMenuItemModel();
			contextMenuItemModel9.Name = "CropPages";
			contextMenuItemModel9.Caption = Resources.MainViewCropPageContent;
			contextMenuItemModel9.Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_CropPages.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_CropPages.png"));
			contextMenuItemModel9.Command = new RelayCommand(delegate
			{
				base.VM.PageEditors.CropPageCmd.Execute(null);
			});
			contextMenuItemModel9.HotKeyInvokeWhen = "Editor_CropPage";
			contextMenuItemModel9.HotKeyInvokeAction = HotKeyInvokeAction.None;
			ContextMenuItemModel contextMenuItemModel10 = new ContextMenuItemModel
			{
				Name = "ResizePages",
				Caption = Resources.PageResizeWinTitle,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/PageResize.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/PageResize.png")),
				Command = new RelayCommand(delegate
				{
					base.VM.PageEditors.ResizePageCmd.Execute(null);
				}),
				HotKeyInvokeWhen = "Editor_ResizePage",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel11 = new ContextMenuItemModel();
			contextMenuItemModel11.Name = "OCRPages";
			contextMenuItemModel11.Caption = Resources.RightMenuOcrPagesText;
			contextMenuItemModel11.Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_OcrPages.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_OcrPages.png"));
			contextMenuItemModel11.Command = new RelayCommand(async delegate
			{
				await base.VM.ViewToolbar.DoOcr(Source.Default);
			});
			contextMenuItemModel11.HotKeyInvokeWhen = "Editor_Ocr";
			contextMenuItemModel11.HotKeyInvokeAction = HotKeyInvokeAction.None;
			foreach (IContextMenuModel contextMenuModel in new ContextMenuModel { contextMenuItemModel2, contextMenuItemModel3, contextMenuItemModel5, contextMenuItemModel4, contextMenuItemModel6 })
			{
				contextMenuItemModel.Add(contextMenuModel);
			}
			base.ContextMenuModel = new ContextMenuModel
			{
				contextMenuHorizontalButton,
				new ContextMenuSeparator(),
				contextMenuItemModel,
				contextMenuItemModel8,
				contextMenuItemModel7,
				contextMenuItemModel10
			};
		}

		// Token: 0x040005F6 RID: 1526
		private bool selected;

		// Token: 0x040005F7 RID: 1527
		private double pageWidth;

		// Token: 0x040005F8 RID: 1528
		private double pageHeight;

		// Token: 0x040005F9 RID: 1529
		private PageRotate pageRotate;

		// Token: 0x040005FA RID: 1530
		private double thumbnailWidth;

		// Token: 0x040005FB RID: 1531
		private double thumbnailHeight;
	}
}
