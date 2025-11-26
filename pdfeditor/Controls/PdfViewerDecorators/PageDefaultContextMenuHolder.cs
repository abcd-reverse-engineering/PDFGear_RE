using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using CommonLib.AppTheme;
using CommonLib.Common;
using CommonLib.Common.HotKeys;
using CommunityToolkit.Mvvm.Input;
using pdfeditor.Models.Menus;
using pdfeditor.Models.Thumbnails;
using pdfeditor.Properties;
using pdfeditor.ViewModels;
using PDFKit;

namespace pdfeditor.Controls.PdfViewerDecorators
{
	// Token: 0x02000238 RID: 568
	internal class PageDefaultContextMenuHolder
	{
		// Token: 0x06002035 RID: 8245 RVA: 0x00091AE3 File Offset: 0x0008FCE3
		public PageDefaultContextMenuHolder(AnnotationCanvas annotationCanvas)
		{
			if (annotationCanvas == null)
			{
				throw new ArgumentNullException("annotationCanvas");
			}
			this.annotationCanvas = annotationCanvas;
			this.InitContextMenu();
		}

		// Token: 0x17000ADB RID: 2779
		// (get) Token: 0x06002036 RID: 8246 RVA: 0x00091B07 File Offset: 0x0008FD07
		private MainViewModel VM
		{
			get
			{
				return this.annotationCanvas.DataContext as MainViewModel;
			}
		}

		// Token: 0x06002037 RID: 8247 RVA: 0x00091B1C File Offset: 0x0008FD1C
		private void InitContextMenu()
		{
			if (PageDefaultContextMenuHolder.IsDesignMode)
			{
				return;
			}
			ContextMenuHorizontalButton contextMenuHorizontalButton = new ContextMenuHorizontalButton
			{
				Name = "HeaderButton",
				Caption0 = Resources.MenuPageDeleteContent,
				Icon0 = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/Delete.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/Delete.png")),
				Command0 = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("PageContextMenu", "DeletePages", "Count", 1L);
					PdfPageEditListModel pdfPageEditListModel = new PdfPageEditListModel(this.VM.Document, this.VM.SelectedPageIndex);
					await this.VM.PageEditors.PageEditorDeleteCmd.ExecuteAsync(pdfPageEditListModel);
					this.Hide();
				}),
				Caption1 = Resources.MenuPageRotateLeftContent,
				Icon1 = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_RotateLeft.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_RotateLeft.png")),
				Command1 = new AsyncRelayCommand(async delegate
				{
					this.VM.ViewToolbar.PageRotateLeftCmd.Execute(null);
					this.Hide();
				}),
				Caption2 = Resources.MenuPageRotateRightContent,
				Icon2 = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_RotateRight.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_RotateRight.png")),
				Command2 = new AsyncRelayCommand(async delegate
				{
					this.VM.ViewToolbar.PageRotateRightCmd.Execute(null);
					this.Hide();
				}),
				Caption3 = Resources.WinViewToolPrintTooltipText,
				Icon3 = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_Print.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_Print.png")),
				Command3 = new AsyncRelayCommand(async delegate
				{
					this.VM.QuickToolPrintModel.Command.Execute(null);
					this.Hide();
				})
			};
			ContextMenuItemModel contextMenuItemModel = new ContextMenuItemModel
			{
				Name = "EditText",
				Caption = Resources.MenuViewEditTextContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_EditText.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_EditText.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.ViewToolbar.EditDocumentButtomModel.Command.Execute(this.VM.ViewToolbar.EditDocumentButtomModel);
				}),
				HotKeyInvokeWhen = "Editor_EditText"
			};
			ContextMenuItemModel contextMenuItemModel2 = new ContextMenuItemModel
			{
				Name = "AddBookmark",
				Caption = Resources.DefaultContextMenuAddBookmark,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_AddBookmark.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_AddBookmark.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("Bookmark", "AddBookmark", "PageContextMenu", 1L);
					await this.VM.BookmarkAddCommand.ExecuteAsync(null);
				}),
				HotKeyInvokeWhen = "Editor_AddBookmark",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel3 = new ContextMenuItemModel
			{
				Name = "MergeAndSplit",
				Caption = Resources.RightMenuMergeSplitPDFItemText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_MergeAndSplit.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_MergeAndSplit.png"))
			};
			ContextMenuItemModel contextMenuItemModel4 = new ContextMenuItemModel
			{
				Name = "MergePDF",
				Caption = Resources.MenuPageMergeContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_MergePDF.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_MergePDF.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("PageContextMenu", "MergePDF", "Count", 1L);
					await this.VM.PageEditors.PageEditorMergeCmd.ExecuteAsync(null);
				})
			};
			ContextMenuItemModel contextMenuItemModel5 = new ContextMenuItemModel
			{
				Name = "SplitPDF",
				Caption = Resources.MenuPageSplitContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_SplitPDF.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_SplitPDF.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("PageContextMenu", "SplitPDF", "Count", 1L);
					await this.VM.PageEditors.PageEditorSplitCmd.ExecuteAsync(null);
				})
			};
			ContextMenuItemModel contextMenuItemModel6 = new ContextMenuItemModel
			{
				Name = "CompressPDF",
				Caption = Resources.RightMenuCompressPDFItemText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_CompreePDF.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_CompreePDF.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("PageContextMenu", "CompressPDF", "Count", 1L);
					this.VM.ConverterCommands.CompressPDF.Execute(null);
				})
			};
			ContextMenuItemModel contextMenuItemModel7 = new ContextMenuItemModel
			{
				Name = "InsertPage",
				Caption = Resources.RightMenuInsertPageText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_InsertPage.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_InsertPage.png"))
			};
			ContextMenuItemModel contextMenuItemModel8 = new ContextMenuItemModel
			{
				Name = "BlankPage",
				Caption = Resources.MenuPageSubInsertBlankPage,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/BlankPage.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/BlankPage.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					PdfPageEditListModel pdfPageEditListModel2 = new PdfPageEditListModel(this.VM.Document, this.VM.SelectedPageIndex);
					await this.VM.PageEditors.PageEditorInsertBlankCmd.ExecuteAsync(pdfPageEditListModel2);
				}),
				IsCheckable = false,
				HotKeyInvokeWhen = "Editor_InsertBlankPage",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel9 = new ContextMenuItemModel
			{
				Name = "FromPDF",
				Caption = Resources.RightMenuInsertPageItemPDF,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/FromPDF.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/FromPDF.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					PdfPageEditListModel pdfPageEditListModel3 = new PdfPageEditListModel(this.VM.Document, this.VM.SelectedPageIndex);
					await this.VM.PageEditors.PageEditorInsertFromPDFCmd.ExecuteAsync(pdfPageEditListModel3);
				}),
				IsCheckable = false,
				HotKeyInvokeWhen = "Editor_InsertPDF",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel10 = new ContextMenuItemModel
			{
				Name = "FromScanner",
				Caption = Resources.MenuPageSubInsertFromScanner,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/FromScanner.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/FromScanner.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					PdfPageEditListModel pdfPageEditListModel4 = new PdfPageEditListModel(this.VM.Document, this.VM.SelectedPageIndex);
					await this.VM.PageEditors.PageEditorInsertFromScannerCmd.ExecuteAsync(pdfPageEditListModel4);
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel11 = new ContextMenuItemModel
			{
				Name = "FromWord",
				Caption = Resources.RightMenuInsertPageItemWord,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/FromWord.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/FromWord.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					PdfPageEditListModel pdfPageEditListModel5 = new PdfPageEditListModel(this.VM.Document, this.VM.SelectedPageIndex);
					await this.VM.PageEditors.PageEditorInsertFromWordCmd.ExecuteAsync(pdfPageEditListModel5);
				}),
				IsCheckable = false,
				HotKeyInvokeWhen = "Editor_InsertWord",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel12 = new ContextMenuItemModel
			{
				Name = "FromImage",
				Caption = Resources.RightMenuInsertPageItemImage,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/FromImage.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/FromImage.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					PdfPageEditListModel pdfPageEditListModel6 = new PdfPageEditListModel(this.VM.Document, this.VM.SelectedPageIndex);
					await this.VM.PageEditors.PageEditorInsertFromImageCmd.ExecuteAsync(pdfPageEditListModel6);
				}),
				IsCheckable = false,
				HotKeyInvokeWhen = "Editor_InsertImage",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel13 = new ContextMenuItemModel
			{
				Name = "ExtractPages",
				Caption = Resources.ShortcutTextPageExtract,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_ExtractPages.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_ExtractPages.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("PageContextMenu", "ExtractPages", "Count", 1L);
					PdfPageEditListModel pdfPageEditListModel7 = new PdfPageEditListModel(this.VM.Document, this.VM.SelectedPageIndex);
					await this.VM.PageEditors.PageEditorExtractCmd.ExecuteAsync(pdfPageEditListModel7);
				}),
				HotKeyInvokeWhen = "Editor_ExtractPage",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel14 = new ContextMenuItemModel
			{
				Name = "DeletePages",
				Caption = Resources.ShortcutTextPageDelete,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/Delete.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/Delete.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("PageContextMenu", "DeletePages", "Count", 1L);
					PdfPageEditListModel pdfPageEditListModel8 = new PdfPageEditListModel(this.VM.Document, this.VM.SelectedPageIndex);
					await this.VM.PageEditors.PageEditorDeleteCmd.ExecuteAsync(pdfPageEditListModel8);
				}),
				HotKeyInvokeWhen = "Editor_DeletePage",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel15 = new ContextMenuItemModel
			{
				Name = "CropPages",
				Caption = Resources.MainViewCropPageContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_CropPages.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_CropPages.png")),
				Command = new RelayCommand(delegate
				{
					GAManager.SendEvent("PageContextMenu", "CropPages", "Count", 1L);
					PdfPageEditListModel pdfPageEditListModel9 = new PdfPageEditListModel(this.VM.Document, this.VM.SelectedPageIndex);
					this.VM.PageEditors.CropPageCmd2.Execute(pdfPageEditListModel9);
				}),
				HotKeyInvokeWhen = "Editor_CropPage",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel16 = new ContextMenuItemModel
			{
				Name = "ResizePages",
				Caption = Resources.PageResizeWinTitle,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/PageEditor/PageResize.png"), new Uri("pack://application:,,,/Style/DarkModeResources/PageEditor/PageResize.png")),
				Command = new RelayCommand(delegate
				{
					GAManager.SendEvent("PageContextMenu", "CropPages", "Count", 1L);
					PdfPageEditListModel pdfPageEditListModel10 = new PdfPageEditListModel(this.VM.Document, this.VM.SelectedPageIndex);
					this.VM.PageEditors.viewerResizePageCmd.Execute(pdfPageEditListModel10);
				}),
				HotKeyInvokeWhen = "Editor_ResizePage",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel17 = new ContextMenuItemModel
			{
				Name = "OCR Pages",
				Caption = Resources.RightMenuOcrPagesItemText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_OcrPages.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_OcrPages.png")),
				Command = new RelayCommand(async delegate
				{
					GAManager.SendEvent("PageContextMenu", "OcrPages", "Count", 1L);
					await this.VM.ViewToolbar.DoOcr(Source.Viewer);
				}),
				HotKeyInvokeWhen = "Editor_Ocr",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel18 = new ContextMenuItemModel
			{
				Name = "Add",
				Caption = Resources.RightMenuAddItemText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_Add.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_Add.png"))
			};
			ContextMenuItemModel contextMenuItemModel19 = new ContextMenuItemModel
			{
				Name = "Image",
				Caption = Resources.RightMenuAddImageItemText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_InsertImage.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_InsertImage.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.AnnotationToolbar.ImageButtonModel.Command.Execute(this.VM.AnnotationToolbar.ImageButtonModel);
				}),
				IsCheckable = false,
				HotKeyInvokeWhen = "Editor_AddImage",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel20 = new ContextMenuItemModel
			{
				Name = "Text",
				Caption = Resources.RightMenuAddTextItemText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_InsertText.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_InsertText.png")),
				Command = new RelayCommand(delegate
				{
					this.VM.AnnotationToolbar.TextButtonModel.IsChecked = true;
					this.VM.AnnotationToolbar.TextButtonModel.Command.Execute(this.VM.AnnotationToolbar.TextButtonModel);
				}),
				IsCheckable = false,
				HotKeyInvokeWhen = "Editor_AddText",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			ContextMenuItemModel contextMenuItemModel21 = new ContextMenuItemModel
			{
				Name = "Watermark",
				Caption = Resources.MenuAnnotateWatermarkContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Annonate/Watermark.png"), new Uri("pack://application:,,,/Style/DarkModeResources/Annonate/Watermark.png")),
				Command = new RelayCommand(delegate
				{
					this.VM.AnnotationToolbar.DoWatermarkInsertCmd2();
				}),
				IsCheckable = false,
				HotKeyInvokeWhen = "Editor_CreateWatermark"
			};
			ContextMenuItemModel contextMenuItemModel22 = new ContextMenuItemModel
			{
				Name = "HeaderAndFooter",
				Caption = Resources.MenuInsertHeaderFooterContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_InsertHeaderAndFooter.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_InsertHeaderAndFooter.png")),
				Command = new RelayCommand(delegate
				{
					this.VM.PageEditors.AddHeaderFooter2();
				}),
				IsCheckable = false,
				HotKeyInvokeWhen = "Editor_AddPageHeaderAndFooter"
			};
			ContextMenuItemModel contextMenuItemModel23 = new ContextMenuItemModel
			{
				Name = "pageNumber",
				Caption = Resources.MenuInsertPageNumberContent,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/ContextMenu/DefaultContextMenu_PageNumber.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/DefaultContextMenu_pageNumber.png")),
				Command = new RelayCommand(delegate
				{
					this.VM.PageEditors.AddPageNumber2();
				}),
				IsCheckable = false,
				HotKeyInvokeWhen = "Editor_AddPageNumber"
			};
			ContextMenuItemModel contextMenuItemModel24 = new ContextMenuItemModel
			{
				Name = "ConvertTo",
				Caption = Resources.RightMenuConvertToItemText,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/convert.png"), new Uri("pack://application:,,,/Style/DarkModeResources/converter/convert.png"))
			};
			ContextMenuItemModel contextMenuItemModel25 = new ContextMenuItemModel
			{
				Name = "PDFtoWord",
				Caption = Resources.MenuConvertPdfToWordContent,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/wordmenu.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.ConverterCommands.DoPDFToWord(null);
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel26 = new ContextMenuItemModel
			{
				Name = "PDFtoExcel",
				Caption = Resources.MenuConvertPdfToExcelContent,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/excelmenu.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.ConverterCommands.DoPDFToExcel();
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel27 = new ContextMenuItemModel
			{
				Name = "PDFtoPPT",
				Caption = Resources.MenuConvertPdfToPPTContent,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/pptmenu.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.ConverterCommands.DoPDFToPPT();
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel28 = new ContextMenuItemModel
			{
				Name = "PDFtoImage",
				Caption = Resources.MenuConvertPdfToImageContent,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/imagemenu.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.ConverterCommands.DoPDFToImage();
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel29 = new ContextMenuItemModel
			{
				Name = "PDFtoJpeg",
				Caption = Resources.MenuConvertPdfToJpegContent,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/imagemenu.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.ConverterCommands.DoPDFToJpeg();
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel30 = new ContextMenuItemModel
			{
				Name = "PDFtoText",
				Caption = Resources.MenuConvertPdfToTxtContent,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/txtmenu.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.ConverterCommands.DoPDFToTxt();
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel31 = new ContextMenuItemModel
			{
				Name = "PDFtoHtml",
				Caption = Resources.MenuConvertPdfToHtmlContent,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/Htmlmenu.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.ConverterCommands.DoPDFToHtml();
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel32 = new ContextMenuItemModel
			{
				Name = "PDFtoRtf",
				Caption = Resources.MenuConvertPdfToRtfContent,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/Rtfmenu.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.ConverterCommands.DoPDFToRtf();
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel33 = new ContextMenuItemModel
			{
				Name = "PDFtoXml",
				Caption = Resources.MenuConvertPdfToXmlContent,
				Icon = new BitmapImage(new Uri("pack://application:,,,/Style/Resources/converter/Xmlmenu.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					this.VM.ConverterCommands.DoPDFToXml();
				}),
				IsCheckable = false
			};
			ContextMenuItemModel contextMenuItemModel34 = new ContextMenuItemModel
			{
				Name = "DocumentProperties",
				Caption = Resources.DocumentPropertiesWindowTitle,
				Icon = ThemeBitmapImage.CreateBitmapImage(new Uri("pack://application:,,,/Style/Resources/Properties.png"), new Uri("pack://application:,,,/Style/DarkModeResources/ContextMenu/Properties.png")),
				Command = new AsyncRelayCommand(async delegate
				{
					GAManager.SendEvent("DocumentPropertiesWindow", "ShowSource", "ContextMenu", 1L);
					this.VM.ShowProperties();
				}),
				HotKeyInvokeWhen = "Editor_DocumentProperties",
				HotKeyInvokeAction = HotKeyInvokeAction.None
			};
			contextMenuItemModel3.Add(contextMenuItemModel4);
			contextMenuItemModel3.Add(contextMenuItemModel5);
			ContextMenuModel contextMenuModel = new ContextMenuModel();
			contextMenuModel.Add(contextMenuItemModel8);
			contextMenuModel.Add(contextMenuItemModel9);
			contextMenuModel.Add(contextMenuItemModel10);
			contextMenuModel.Add(contextMenuItemModel11);
			contextMenuModel.Add(contextMenuItemModel12);
			ContextMenuModel contextMenuModel2 = new ContextMenuModel
			{
				contextMenuItemModel19,
				contextMenuItemModel20,
				new ContextMenuSeparator(),
				contextMenuItemModel21,
				contextMenuItemModel22,
				contextMenuItemModel23
			};
			ContextMenuModel contextMenuModel3 = new ContextMenuModel { contextMenuItemModel25, contextMenuItemModel26, contextMenuItemModel27, contextMenuItemModel28, contextMenuItemModel29, contextMenuItemModel30, contextMenuItemModel31, contextMenuItemModel32, contextMenuItemModel33 };
			foreach (IContextMenuModel contextMenuModel4 in contextMenuModel)
			{
				contextMenuItemModel7.Add(contextMenuModel4);
			}
			foreach (IContextMenuModel contextMenuModel5 in contextMenuModel2)
			{
				contextMenuItemModel18.Add(contextMenuModel5);
			}
			foreach (IContextMenuModel contextMenuModel6 in contextMenuModel3)
			{
				contextMenuItemModel24.Add(contextMenuModel6);
			}
			ContextMenuModel contextMenuModel7 = new ContextMenuModel
			{
				contextMenuHorizontalButton,
				new ContextMenuSeparator(),
				contextMenuItemModel,
				contextMenuItemModel2,
				contextMenuItemModel3,
				contextMenuItemModel6,
				new ContextMenuSeparator(),
				contextMenuItemModel7,
				contextMenuItemModel14,
				contextMenuItemModel13,
				contextMenuItemModel15,
				contextMenuItemModel16,
				contextMenuItemModel17,
				new ContextMenuSeparator(),
				contextMenuItemModel18,
				contextMenuItemModel24,
				new ContextMenuSeparator(),
				contextMenuItemModel34
			};
			this.contextMenu = new PdfViewerContextMenu
			{
				ItemsSource = contextMenuModel7,
				PlacementTarget = this.annotationCanvas,
				AutoCloseOnMouseLeave = false
			};
		}

		// Token: 0x06002038 RID: 8248 RVA: 0x00092A7C File Offset: 0x00090C7C
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
			if (this.annotationCanvas.SelectedAnnotation != null)
			{
				return false;
			}
			this.contextMenu.Placement = PlacementMode.MousePoint;
			return this.contextMenu.IsOpen = true;
		}

		// Token: 0x06002039 RID: 8249 RVA: 0x00092B01 File Offset: 0x00090D01
		public void Hide()
		{
			this.contextMenu.IsOpen = false;
			this.contextMenu.Placement = PlacementMode.Absolute;
		}

		// Token: 0x17000ADC RID: 2780
		// (get) Token: 0x0600203A RID: 8250 RVA: 0x00092B1B File Offset: 0x00090D1B
		private static bool IsDesignMode
		{
			get
			{
				return (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
			}
		}

		// Token: 0x04000CF4 RID: 3316
		private readonly AnnotationCanvas annotationCanvas;

		// Token: 0x04000CF5 RID: 3317
		private PdfViewerContextMenu contextMenu;
	}
}
