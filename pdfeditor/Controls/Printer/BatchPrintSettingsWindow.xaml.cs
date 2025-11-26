using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using CommonLib.Common;
using CommonLib.Controls;
using pdfeditor.Models;
using pdfeditor.Models.Print;
using pdfeditor.Properties;
using pdfeditor.Utils.Print;
using pdfeditor.Utils.Printer;
using PDFKit.Utils;

namespace pdfeditor.Controls.Printer
{
	// Token: 0x02000227 RID: 551
	public partial class BatchPrintSettingsWindow : Window
	{
		// Token: 0x06001EBF RID: 7871 RVA: 0x00088E98 File Offset: 0x00087098
		public BatchPrintSettingsWindow(ObservableCollection<BatchPrintItemModel> documentModel, BatchPrintItemModel SelectedItem = null, bool isSingleFile = false)
		{
			this.InitializeComponent();
			GAManager.SendEvent("BatchPrintSettingsWindow", "Show", isSingleFile ? "SingleFileSettings" : "BatchSettings", 1L);
			this.PreviewControl.FileComboBoxVisibility = Visibility.Visible;
			this.documents = documentModel;
			this.RecordDocumentsInfo(documentModel);
			if (SelectedItem == null)
			{
				this.AppliedGrid.Visibility = Visibility.Collapsed;
				this.AllDocumentCheckBox.IsChecked = new bool?(true);
				this.PreviewControl.SetBatchPrintFileList(this.tempDocumentsInfo, null);
			}
			else
			{
				if (documentModel.Count<BatchPrintItemModel>() == 1)
				{
					this.AllDocumentCheckBox.IsChecked = new bool?(true);
				}
				this.PreviewControl.SetBatchPrintFileList(this.tempDocumentsInfo, this.tempDocumentsInfo.FirstOrDefault((BatchPrintItemModel x) => x.FilePath == SelectedItem.FilePath));
			}
			this.GetPrinterList();
			BatchPrintItemModel selectedFile = this.PreviewControl.GetSelectedFile();
			if (selectedFile != null)
			{
				this.ApplyPrintArgs(selectedFile._PrintArgs);
			}
			else
			{
				this.UpdatePreview();
			}
			this.PreviewControl.FileListSelectionChanged += this.PreviewControl_FileListSelectionChanged;
		}

		// Token: 0x06001EC0 RID: 7872 RVA: 0x00088FCC File Offset: 0x000871CC
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);
			FrameworkElement frameworkElement = Keyboard.FocusedElement as FrameworkElement;
			if (frameworkElement != null)
			{
				TraversalRequest traversalRequest = new TraversalRequest(FocusNavigationDirection.Next);
				frameworkElement.MoveFocus(traversalRequest);
			}
			Keyboard.ClearFocus();
		}

		// Token: 0x06001EC1 RID: 7873 RVA: 0x00089004 File Offset: 0x00087204
		private void RecordDocumentsInfo(ObservableCollection<BatchPrintItemModel> documentModel)
		{
			this.tempDocumentsInfo = new ObservableCollection<BatchPrintItemModel>();
			foreach (BatchPrintItemModel batchPrintItemModel in documentModel)
			{
				BatchPrintItemModel batchPrintItemModel2 = new BatchPrintItemModel(batchPrintItemModel.FilePath, batchPrintItemModel.Document);
				batchPrintItemModel2._PrintArgs = new PrintArgs
				{
					AutoRotate = batchPrintItemModel._PrintArgs.AutoRotate,
					AutoCenter = batchPrintItemModel._PrintArgs.AutoCenter,
					Collate = batchPrintItemModel._PrintArgs.Collate,
					DocumentPath = batchPrintItemModel._PrintArgs.DocumentPath,
					PrintDevMode = batchPrintItemModel._PrintArgs.PrintDevMode,
					Copies = batchPrintItemModel.Copies,
					PrinterName = batchPrintItemModel._PrintArgs.PrinterName,
					PaperSize = batchPrintItemModel._PrintArgs.PaperSize,
					Grayscale = batchPrintItemModel._PrintArgs.Grayscale,
					Duplex = batchPrintItemModel._PrintArgs.Duplex,
					Landscape = batchPrintItemModel._PrintArgs.Landscape,
					PrintSizeMode = batchPrintItemModel._PrintArgs.PrintSizeMode,
					PapersPerSheet = batchPrintItemModel._PrintArgs.PapersPerSheet,
					PaperRowNum = batchPrintItemModel._PrintArgs.PaperRowNum,
					PaperColumnNum = batchPrintItemModel._PrintArgs.PaperColumnNum,
					TypesettingModel = batchPrintItemModel._PrintArgs.TypesettingModel,
					PageOrder = batchPrintItemModel._PrintArgs.PageOrder,
					BindingDirection = batchPrintItemModel._PrintArgs.BindingDirection,
					bookletSubset = batchPrintItemModel._PrintArgs.bookletSubset,
					TilePageZoom = batchPrintItemModel._PrintArgs.TilePageZoom,
					TileOverlap = batchPrintItemModel._PrintArgs.TileOverlap,
					TileCutMasks = batchPrintItemModel._PrintArgs.TileCutMasks,
					TileLabels = batchPrintItemModel._PrintArgs.TileLabels,
					PrintBorder = batchPrintItemModel._PrintArgs.PrintBorder,
					PageMargins = batchPrintItemModel._PrintArgs.PageMargins,
					PrintReverse = batchPrintItemModel._PrintArgs.PrintReverse,
					MutilplePageMargins = batchPrintItemModel._PrintArgs.MutilplePageMargins,
					TilePageMargins = batchPrintItemModel._PrintArgs.TilePageMargins,
					Scale = batchPrintItemModel._PrintArgs.Scale
				};
				batchPrintItemModel2.PageRange = batchPrintItemModel.PageRange;
				batchPrintItemModel2._PrintAnnotations = batchPrintItemModel._PrintAnnotations;
				this.tempDocumentsInfo.Add(batchPrintItemModel2);
			}
		}

		// Token: 0x06001EC2 RID: 7874 RVA: 0x0008928C File Offset: 0x0008748C
		private void PreviewControl_FileListSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			BatchPrintItemModel selectedFile = this.PreviewControl.GetSelectedFile();
			if (selectedFile != null)
			{
				if (selectedFile._PrintArgs.PrinterName == (this.cboxPrinterList.SelectedItem as PrinterInfo).PrinterName)
				{
					this.UpdatePrintProperties(null);
					return;
				}
				this.ApplyPrintArgs(selectedFile._PrintArgs);
			}
		}

		// Token: 0x06001EC3 RID: 7875 RVA: 0x000892E4 File Offset: 0x000874E4
		private void GetPrinterList()
		{
			try
			{
				PrinterSettings printerSettings = new PrinterSettings();
				string sDefault = printerSettings.PrinterName;
				foreach (object obj in PrinterSettings.InstalledPrinters)
				{
					string text = (string)obj;
					PrinterInfo printerInfo = new PrinterInfo
					{
						PrinterName = text
					};
					this.printLst.Insert(0, printerInfo);
				}
				PrinterInfo printerInfo2 = this.printLst.Where((PrinterInfo p) => p.PrinterName == sDefault).FirstOrDefault<PrinterInfo>();
				if (printerInfo2 != null)
				{
					this.printLst.Remove(printerInfo2);
					this.printLst.Insert(0, printerInfo2);
				}
				if (this.printLst.Count == 0)
				{
					this.printLst.Add(new PrinterInfo
					{
						PrinterName = pdfeditor.Properties.Resources.WinPrinterNoPrinterContent
					});
				}
				this.cboxPrinterList.ItemsSource = this.printLst;
				this.cboxPrinterList.SelectedIndex = 0;
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06001EC4 RID: 7876 RVA: 0x00089400 File Offset: 0x00087600
		private PrintArgs GetPrintArgs()
		{
			PrinterInfo printerInfo = this.cboxPrinterList.SelectedItem as PrinterInfo;
			PrintArgs printArgs = new PrintArgs();
			printArgs.Grayscale = this.GrayscaleCheckbox.IsChecked.GetValueOrDefault();
			printArgs.PrinterName = printerInfo.PrinterName;
			printArgs.AutoRotate = this.TileBtn.IsChecked.GetValueOrDefault() > false;
			printArgs.AutoCenter = true;
			printArgs.Copies = (int)this.tboxCopies.Value;
			printArgs.Collate = this.chkbPrintCollate.IsChecked.Value;
			printArgs.Landscape = this.rdbtnLandscape.IsChecked.Value;
			printArgs.PaperSize = ((PaperSizeInfo)this.cboxPaperSize.SelectedItem).PaperSize;
			DocumentWrapper documentWrapper = this.PreviewControl.GetSelectedFile().DocumentWrapper;
			printArgs.DocumentPath = ((documentWrapper != null) ? documentWrapper.DocumentPath : null);
			printArgs.PrintDevMode = printerInfo.PrintDevMode;
			printArgs.Duplex = (this.TileBtn.IsChecked.GetValueOrDefault() ? Duplex.Simplex : ((this.BookletSubset.SelectedIndex == 0 && this.BookletBtn.IsChecked.GetValueOrDefault()) ? Duplex.Horizontal : ((!this.chkbDuplex.IsChecked.GetValueOrDefault()) ? Duplex.Simplex : ((!this.rdbtnDuplexVertical.IsChecked.GetValueOrDefault()) ? Duplex.Horizontal : Duplex.Vertical))));
			printArgs.TypesettingModel = (this.ScalBtn.IsChecked.GetValueOrDefault() ? PrintTypeSettingModel.Scale : (this.MutilpleBtn.IsChecked.GetValueOrDefault() ? PrintTypeSettingModel.Multiple : (this.BookletBtn.IsChecked.GetValueOrDefault() ? PrintTypeSettingModel.Booklet : (this.TileBtn.IsChecked.GetValueOrDefault() ? PrintTypeSettingModel.Tile : PrintTypeSettingModel.Scale))));
			printArgs.PapersPerSheet = (int)(this.MutilpleRowNum.Value * this.MutilpleColumnNum.Value);
			printArgs.PaperRowNum = (int)this.MutilpleRowNum.Value;
			printArgs.PaperColumnNum = (int)this.MutilpleColumnNum.Value;
			printArgs.PageOrder = (this.Layout1Btn1.IsChecked.GetValueOrDefault() ? PageOrder.Horizontal : (this.Layout1Btn2.IsChecked.GetValueOrDefault() ? PageOrder.HorizontalReverse : (this.Layout1Btn3.IsChecked.GetValueOrDefault() ? PageOrder.Vertical : (this.Layout1Btn4.IsChecked.GetValueOrDefault() ? PageOrder.VerticalReverse : PageOrder.Horizontal))));
			printArgs.BindingDirection = ((this.BookletBinding.SelectedIndex == 0) ? BookletBindingDirection.Left : BookletBindingDirection.Right);
			printArgs.bookletSubset = ((this.BookletSubset.SelectedIndex == 0) ? pdfeditor.Utils.Print.BookletSubset.BothSide : ((this.BookletSubset.SelectedIndex == 1) ? pdfeditor.Utils.Print.BookletSubset.Frontal : pdfeditor.Utils.Print.BookletSubset.Back));
			printArgs.TilePageZoom = this.PageZoomNum.Value;
			printArgs.TileOverlap = this.OverlapNum.Value;
			printArgs.TileCutMasks = this.CutMasks.IsChecked.Value;
			printArgs.TileLabels = this.Labels.IsChecked.Value;
			printArgs.PrintBorder = this.MutilpleBtn.IsChecked.GetValueOrDefault() && this.MutilplePrintBorderCheckbox.IsChecked.Value;
			printArgs.PageMargins = (this.MutilpleBtn.IsChecked.GetValueOrDefault() ? this.PrintMarginsTextbox.Value : (this.BookletBtn.IsChecked.GetValueOrDefault() ? this.PrintMarginsBookletTextbox.Value : 0.0));
			printArgs.PrintReverse = this.PrintReverseCheckbox.IsChecked.Value;
			printArgs.MutilplePageMargins = this.PrintMarginsTextbox.Value;
			printArgs.TilePageMargins = this.PrintMarginsBookletTextbox.Value;
			PrintArgs printArgs2 = printArgs;
			if (this.rdbtnFitPage.IsChecked.GetValueOrDefault())
			{
				printArgs2.PrintSizeMode = PrintSizeMode.Fit;
			}
			else if (this.rdbtnActualSize.IsChecked.GetValueOrDefault())
			{
				printArgs2.PrintSizeMode = PrintSizeMode.ActualSize;
			}
			else
			{
				printArgs2.PrintSizeMode = PrintSizeMode.CustomScale;
				printArgs2.Scale = (int)this.tboxScaleUnit.Value;
			}
			return printArgs2;
		}

		// Token: 0x06001EC5 RID: 7877 RVA: 0x00089828 File Offset: 0x00087A28
		private void ApplyPrintArgs(PrintArgs args)
		{
			if (args == null)
			{
				return;
			}
			this.GrayscaleCheckbox.IsChecked = new bool?(args.Grayscale);
			PrinterInfo printerInfo = this.printLst.FirstOrDefault((PrinterInfo c) => c.PrinterName == args.PrinterName);
			Selector selector = this.cboxPrinterList;
			PrinterInfo printerInfo2;
			if ((printerInfo2 = printerInfo) == null)
			{
				printerInfo2 = this.cboxPrinterList.SelectedItem ?? this.printLst.FirstOrDefault<PrinterInfo>();
			}
			selector.SelectedItem = printerInfo2;
			if (printerInfo != null)
			{
				if (printerInfo.PrintDevMode != null)
				{
					printerInfo.PrintDevMode.Dispose();
					printerInfo.PrintDevMode = null;
				}
				printerInfo.PrintDevMode = args.PrintDevMode;
			}
			this.UpdatePrintProperties(args.PaperSize);
		}

		// Token: 0x06001EC6 RID: 7878 RVA: 0x000898E8 File Offset: 0x00087AE8
		private void UpdatePrintProperties(PaperSize paperSize = null)
		{
			BatchPrintItemModel selectedFile = this.PreviewControl.GetSelectedFile();
			if (selectedFile != null)
			{
				PrinterInfo printerInfo = this.cboxPrinterList.SelectedItem as PrinterInfo;
				if (printerInfo == null)
				{
					this.cboxPaperSize.IsEnabled = false;
				}
				PrinterSettings printerSettings = new PrinterSettings
				{
					PrinterName = printerInfo.PrinterName
				};
				PrinterSettings.PaperSizeCollection paperSizes = printerSettings.PaperSizes;
				if (paperSizes.Count > 0)
				{
					this.cboxPaperSize.IsEnabled = true;
				}
				this.paperSizesLst = (from c in paperSizes.OfType<PaperSize>()
					select new PaperSizeInfo
					{
						FriendlyName = c.PaperName,
						PaperSize = c
					}).ToList<PaperSizeInfo>();
				this.cboxPaperSize.ItemsSource = this.paperSizesLst;
				PaperSize defaultPaperSize = paperSize ?? printerSettings.DefaultPageSettings.PaperSize;
				this.chkbPrintCollate.IsChecked = new bool?(selectedFile._PrintArgs.Collate);
				if (selectedFile._PrintArgs.Landscape)
				{
					this.rdbtnLandscape.IsChecked = new bool?(true);
				}
				else
				{
					this.rdbtnPortrait.IsChecked = new bool?(true);
				}
				this.chkbDuplex.IsChecked = new bool?(selectedFile._PrintArgs.Duplex != Duplex.Simplex);
				this.rdbtnDuplexVertical.IsChecked = new bool?(selectedFile._PrintArgs.Duplex == Duplex.Vertical);
				this.rdbtnDuplexHorizontal.IsChecked = new bool?(selectedFile._PrintArgs.Duplex != Duplex.Vertical);
				if (selectedFile._PrintArgs.PrintSizeMode == PrintSizeMode.Fit)
				{
					this.rdbtnFitPage.IsChecked = new bool?(true);
				}
				else if (selectedFile._PrintArgs.PrintSizeMode == PrintSizeMode.ActualSize)
				{
					this.rdbtnActualSize.IsChecked = new bool?(true);
				}
				else
				{
					this.rdbtnScale.IsChecked = new bool?(true);
				}
				this.tboxScaleUnit.Value = (double)selectedFile._PrintArgs.Scale;
				PrintSettings PrintSettings = new PrintSettings
				{
					Device = selectedFile._PrintArgs.PrinterName,
					Paper = selectedFile._PrintArgs.PaperSize,
					IsGrayscale = selectedFile._PrintArgs.Grayscale,
					Duplex = selectedFile._PrintArgs.Duplex,
					Landscape = selectedFile._PrintArgs.Landscape,
					PapersPerSheet = selectedFile._PrintArgs.PapersPerSheet,
					PaperRowNum = selectedFile._PrintArgs.PaperRowNum,
					PaperColumnNum = selectedFile._PrintArgs.PaperColumnNum,
					TypesettingModel = selectedFile._PrintArgs.TypesettingModel,
					PageOrder = selectedFile._PrintArgs.PageOrder,
					BindingDirection = selectedFile._PrintArgs.BindingDirection,
					bookletSubset = selectedFile._PrintArgs.bookletSubset,
					TilePageZoom = selectedFile._PrintArgs.TilePageZoom,
					TileOverlap = selectedFile._PrintArgs.TileOverlap,
					TileCutMasks = selectedFile._PrintArgs.TileCutMasks,
					TileLabels = selectedFile._PrintArgs.TileLabels,
					PrintBorder = selectedFile._PrintArgs.PrintBorder,
					PageMargins = selectedFile._PrintArgs.PageMargins,
					PrintReverse = selectedFile._PrintArgs.PrintReverse,
					MutilplePageMargins = selectedFile._PrintArgs.MutilplePageMargins,
					TilePageMargins = selectedFile._PrintArgs.TilePageMargins
				};
				this.tboxCopies.Value = (double)selectedFile._PrintArgs.Copies;
				this.PreviewControl.PrintAnnotations = selectedFile._PrintAnnotations;
				if (PrintSettings != null)
				{
					Selector selector = this.cboxPaperSize;
					PaperSizeInfo paperSizeInfo;
					if ((paperSizeInfo = this.paperSizesLst.FirstOrDefault((PaperSizeInfo c) => c.PaperSize.Kind == PrintSettings.Paper.Kind && c.PaperSize.PaperName == PrintSettings.Paper.PaperName)) == null)
					{
						paperSizeInfo = this.paperSizesLst.FirstOrDefault((PaperSizeInfo c) => c.FriendlyName == "A4") ?? this.paperSizesLst.FirstOrDefault<PaperSizeInfo>();
					}
					selector.SelectedItem = paperSizeInfo;
					this.DuplexPanel.IsEnabled = printerSettings.CanDuplex;
					if (this.DuplexPanel.IsEnabled)
					{
						if (PrintSettings.Duplex == Duplex.Horizontal)
						{
							this.chkbDuplex.IsChecked = new bool?(true);
							this.rdbtnDuplexHorizontal.IsChecked = new bool?(true);
						}
						else if (PrintSettings.Duplex == Duplex.Vertical)
						{
							this.chkbDuplex.IsChecked = new bool?(true);
							this.rdbtnDuplexVertical.IsChecked = new bool?(true);
						}
						else
						{
							this.chkbDuplex.IsChecked = new bool?(false);
						}
					}
					else
					{
						this.chkbDuplex.IsChecked = new bool?(false);
					}
					if (PrintSettings.IsGrayscale)
					{
						this.GrayscaleCheckbox.IsChecked = new bool?(true);
					}
					else
					{
						this.GrayscaleCheckbox.IsChecked = new bool?(false);
					}
					if (PrintSettings.Landscape)
					{
						this.rdbtnLandscape.IsChecked = new bool?(true);
					}
					else
					{
						this.rdbtnPortrait.IsChecked = new bool?(true);
					}
					this.MutilpleRowNum.Value = (double)PrintSettings.PaperRowNum;
					this.MutilpleColumnNum.Value = (double)PrintSettings.PaperColumnNum;
					int num = PrintSettings.PaperRowNum * PrintSettings.PaperColumnNum;
					switch (num)
					{
					case 2:
						this.TotalPageInPaper.SelectedIndex = 0;
						goto IL_05A8;
					case 3:
					case 5:
						break;
					case 4:
						this.TotalPageInPaper.SelectedIndex = 1;
						goto IL_05A8;
					case 6:
						this.TotalPageInPaper.SelectedIndex = 2;
						goto IL_05A8;
					default:
						if (num == 9)
						{
							this.TotalPageInPaper.SelectedIndex = 3;
							goto IL_05A8;
						}
						if (num == 16)
						{
							this.TotalPageInPaper.SelectedIndex = 4;
							goto IL_05A8;
						}
						break;
					}
					this.TotalPageInPaper.SelectedIndex = 5;
					IL_05A8:
					switch (PrintSettings.TypesettingModel)
					{
					case PrintTypeSettingModel.Scale:
						this.ScalBtn.IsChecked = new bool?(true);
						break;
					case PrintTypeSettingModel.Multiple:
						this.MutilpleBtn.IsChecked = new bool?(true);
						break;
					case PrintTypeSettingModel.Tile:
						this.TileBtn.IsChecked = new bool?(true);
						break;
					case PrintTypeSettingModel.Booklet:
						this.BookletBtn.IsChecked = new bool?(true);
						break;
					default:
						this.ScalBtn.IsChecked = new bool?(true);
						break;
					}
					switch (PrintSettings.PageOrder)
					{
					case PageOrder.Horizontal:
						this.Layout1Btn1.IsChecked = new bool?(true);
						break;
					case PageOrder.HorizontalReverse:
						this.Layout1Btn2.IsChecked = new bool?(true);
						break;
					case PageOrder.Vertical:
						this.Layout1Btn3.IsChecked = new bool?(true);
						break;
					case PageOrder.VerticalReverse:
						this.Layout1Btn4.IsChecked = new bool?(true);
						break;
					default:
						this.Layout1Btn1.IsChecked = new bool?(true);
						break;
					}
					if (PrintSettings.BindingDirection == BookletBindingDirection.Left)
					{
						this.BookletBinding.SelectedIndex = 0;
					}
					else
					{
						this.BookletBinding.SelectedIndex = 1;
					}
					this.PageZoomNum.Value = PrintSettings.TilePageZoom;
					this.OverlapNum.Value = PrintSettings.TileOverlap;
					this.CutMasks.IsChecked = new bool?(PrintSettings.TileCutMasks);
					this.Labels.IsChecked = new bool?(PrintSettings.TileLabels);
					this.MutilplePrintBorderCheckbox.IsChecked = new bool?(PrintSettings.PrintBorder);
					this.PrintMarginsBookletTextbox.Value = PrintSettings.TilePageMargins;
					this.PrintMarginsTextbox.Value = PrintSettings.MutilplePageMargins;
					this.PrintReverseCheckbox.IsChecked = new bool?(PrintSettings.PrintReverse);
				}
				else
				{
					Selector selector2 = this.cboxPaperSize;
					PaperSizeInfo paperSizeInfo2;
					if ((paperSizeInfo2 = this.paperSizesLst.FirstOrDefault((PaperSizeInfo c) => defaultPaperSize != null && c.PaperSize.Kind == defaultPaperSize.Kind && c.PaperSize.PaperName == defaultPaperSize.PaperName)) == null)
					{
						paperSizeInfo2 = this.paperSizesLst.FirstOrDefault((PaperSizeInfo c) => c.FriendlyName == "A4") ?? this.paperSizesLst.FirstOrDefault<PaperSizeInfo>();
					}
					selector2.SelectedItem = paperSizeInfo2;
					if (!printerSettings.CanDuplex)
					{
						this.chkbDuplex.IsChecked = new bool?(false);
					}
					this.DuplexPanel.IsEnabled = printerSettings.CanDuplex;
				}
			}
			this.UpdatePreview();
		}

		// Token: 0x06001EC7 RID: 7879 RVA: 0x0008A124 File Offset: 0x00088324
		private void UpdatePreview()
		{
			CancellationTokenSource cancellationTokenSource = this.updatePreviewCts;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			CancellationTokenSource cancellationTokenSource2 = this.updatePreviewCts;
			if (cancellationTokenSource2 != null)
			{
				cancellationTokenSource2.Dispose();
			}
			CancellationTokenSource cts = new CancellationTokenSource();
			this.updatePreviewCts = cts;
			base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				if (cts.IsCancellationRequested)
				{
					return;
				}
				BatchPrintItemModel selectedFile = this.PreviewControl.GetSelectedFile();
				if (selectedFile != null)
				{
					PrintArgs printArgs = this.GetPrintArgs();
					printArgs.Document = selectedFile.Document;
					printArgs.PrintPageIndexMapper = BatchPrintSettingsWindow.GetPrintPageIndexMapper(selectedFile);
					if (printArgs.PrintPageIndexMapper != null)
					{
						this.PreviewControl.PrintArgs = printArgs;
					}
					else
					{
						this.PreviewControl.PrintArgs = null;
					}
					selectedFile._PrintArgs = printArgs;
					selectedFile._PrintAnnotations = this.PreviewControl.PrintAnnotations;
					selectedFile.Copies = (int)this.tboxCopies.Value;
				}
			}));
		}

		// Token: 0x06001EC8 RID: 7880 RVA: 0x0008A190 File Offset: 0x00088390
		internal static PrintPageIndexMapper GetPrintPageIndexMapper(BatchPrintItemModel model)
		{
			int[] array;
			if (!string.IsNullOrEmpty(model.GetActualPageRange(out array)) && array != null && array.Length != 0)
			{
				PageDisposeHelper.TryFixResource(model.Document, array.Min(), array.Max());
				return new PrintPageIndexMapper(array, model.Document.Pages.Count);
			}
			return null;
		}

		// Token: 0x06001EC9 RID: 7881 RVA: 0x0008A1E4 File Offset: 0x000883E4
		private void ClearPrinterList()
		{
			if (this.printLst != null)
			{
				List<PrinterInfo> list = this.printLst;
				lock (list)
				{
					foreach (PrinterInfo printerInfo in this.printLst)
					{
						PrintDevModeHandle printDevMode = printerInfo.PrintDevMode;
						if (printDevMode != null)
						{
							printDevMode.Dispose();
						}
						printerInfo.PrintDevMode = null;
					}
				}
				this.printLst.Clear();
			}
		}

		// Token: 0x06001ECA RID: 7882 RVA: 0x0008A280 File Offset: 0x00088480
		private void cboxPaperSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.UpdatePreview();
		}

		// Token: 0x06001ECB RID: 7883 RVA: 0x0008A288 File Offset: 0x00088488
		private async void PrintOrientation_Checked(object sender, RoutedEventArgs e)
		{
			if (e.Source is RadioButton)
			{
				await base.Dispatcher.InvokeAsync(delegate
				{
					this.UpdatePreview();
				});
			}
		}

		// Token: 0x06001ECC RID: 7884 RVA: 0x0008A2C8 File Offset: 0x000884C8
		private async void PrintMode_Checked(object sender, RoutedEventArgs e)
		{
			if (e.Source is RadioButton)
			{
				await base.Dispatcher.InvokeAsync(delegate
				{
					this.UpdatePreview();
				});
			}
		}

		// Token: 0x06001ECD RID: 7885 RVA: 0x0008A308 File Offset: 0x00088508
		private async void tboxScaleUnit_LostFocus(object sender, RoutedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001ECE RID: 7886 RVA: 0x0008A340 File Offset: 0x00088540
		private async void tboxPageRang_LostFocus(object sender, RoutedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001ECF RID: 7887 RVA: 0x0008A378 File Offset: 0x00088578
		private async void GrayscaleCheckbox_Click(object sender, RoutedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001ED0 RID: 7888 RVA: 0x0008A3AF File Offset: 0x000885AF
		private void cboxPrinterList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PaperSizeInfo paperSizeInfo = this.cboxPaperSize.SelectedItem as PaperSizeInfo;
			this.UpdatePrintProperties((paperSizeInfo != null) ? paperSizeInfo.PaperSize : null);
		}

		// Token: 0x06001ED1 RID: 7889 RVA: 0x0008A3D4 File Offset: 0x000885D4
		private async void PrinterPreferenceButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("BatchPrintSettingsWindow", "PreferenceBtn", "Count", 1L);
			PrintArgs printArgs = this.GetPrintArgs();
			PrinterSettings printerSettings = new PrinterSettings();
			if (printArgs.PrintDevMode != null)
			{
				printerSettings.SetHdevmode(printArgs.PrintDevMode);
			}
			printerSettings.PrinterName = printArgs.PrinterName;
			printerSettings.Copies = (short)printArgs.Copies;
			printerSettings.Collate = printArgs.Collate;
			printerSettings.Duplex = printArgs.Duplex;
			printerSettings.DefaultPageSettings.PaperSize = printArgs.PaperSize;
			printerSettings.DefaultPageSettings.Landscape = printArgs.Landscape;
			printerSettings.DefaultPageSettings.Color = !printArgs.Grayscale;
			PrintDevModeHandle printDevModeHandle = PrinterDevModeHelper.OpenPrinterConfigure(this, printerSettings);
			if (!printDevModeHandle.IsInvalid)
			{
				printArgs.PaperSize = printerSettings.DefaultPageSettings.PaperSize;
				printArgs.Landscape = printerSettings.DefaultPageSettings.Landscape;
				printArgs.Grayscale = !printerSettings.DefaultPageSettings.Color;
				printArgs.PrinterName = printerSettings.PrinterName;
				printArgs.Copies = (int)printerSettings.Copies;
				printArgs.Collate = printerSettings.Collate;
				printArgs.Duplex = printerSettings.Duplex;
				printArgs.PrintDevMode = printDevModeHandle;
				PrintService.SaveSettings(new PrintSettings
				{
					Device = printArgs.PrinterName,
					Paper = printArgs.PaperSize,
					IsGrayscale = printArgs.Grayscale,
					Duplex = printArgs.Duplex,
					Landscape = printArgs.Landscape
				});
				this.ApplyPrintArgs(printArgs);
				await base.Dispatcher.InvokeAsync(delegate
				{
					this.UpdatePreview();
				});
			}
		}

		// Token: 0x06001ED2 RID: 7890 RVA: 0x0008A40C File Offset: 0x0008860C
		private void tboxCopies_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Action func = delegate
			{
				if (e.NewValue - 1.0 < 0.0001)
				{
					this.PrintCollateImage.Opacity = 0.6;
					this.chkbPrintCollate.IsEnabled = false;
					return;
				}
				this.PrintCollateImage.Opacity = 1.0;
				this.chkbPrintCollate.IsEnabled = true;
			};
			Action func2 = null;
			func2 = delegate
			{
				if (!this.IsLoaded)
				{
					this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, func2);
					return;
				}
				func();
			};
			func2();
		}

		// Token: 0x06001ED3 RID: 7891 RVA: 0x0008A464 File Offset: 0x00088664
		private async void TypeSettings_Checked(object sender, RoutedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001ED4 RID: 7892 RVA: 0x0008A49C File Offset: 0x0008869C
		private async void TotalPageInPaper_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.MutilpleRowNum != null && this.MutilpleColumnNum != null)
			{
				switch ((sender as ComboBox).SelectedIndex)
				{
				case 0:
					this.MutilpleRowNum.Value = 2.0;
					this.MutilpleColumnNum.Value = 1.0;
					break;
				case 1:
					this.MutilpleRowNum.Value = 2.0;
					this.MutilpleColumnNum.Value = 2.0;
					break;
				case 2:
					this.MutilpleRowNum.Value = 2.0;
					this.MutilpleColumnNum.Value = 3.0;
					break;
				case 3:
					this.MutilpleRowNum.Value = 3.0;
					this.MutilpleColumnNum.Value = 3.0;
					break;
				case 4:
					this.MutilpleRowNum.Value = 4.0;
					this.MutilpleColumnNum.Value = 4.0;
					break;
				case 5:
					break;
				default:
					return;
				}
				await base.Dispatcher.InvokeAsync(delegate
				{
					this.UpdatePreview();
				});
			}
		}

		// Token: 0x06001ED5 RID: 7893 RVA: 0x0008A4DC File Offset: 0x000886DC
		private async void BookletBinding_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001ED6 RID: 7894 RVA: 0x0008A514 File Offset: 0x00088714
		private async void CutMasks_Checked(object sender, RoutedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001ED7 RID: 7895 RVA: 0x0008A54C File Offset: 0x0008874C
		private void btnSaveSettingsAll_Click(object sender, RoutedEventArgs e)
		{
			string text;
			if (this.AppliedGrid.Visibility == Visibility.Visible)
			{
				text = "SetForBathOrSignal_Show_";
			}
			else
			{
				text = "SetForBathOrSignal_Hide_";
			}
			PrintArgs printArgs = this.GetPrintArgs();
			GAManager.SendEvent("BatchPrintSettingsWindow", "Grayscale", printArgs.Grayscale ? "Gray" : "Color", 1L);
			GAManager.SendEvent("BatchPrintSettingsWindow", "ReverseOrder", printArgs.PrintReverse ? "Reverse" : "Normal", 1L);
			GAManager.SendEvent("BatchPrintSettingsWindow", "PrintAnnotation", this.PreviewControl.PrintAnnotations ? "HasAnnotation" : "NoAnnotation", 1L);
			GAManager.SendEvent("BatchPrintSettingsWindow", "PrintMode", printArgs.TypesettingModel.ToString(), 1L);
			if (this.CurrentDocumentCheckBox.IsChecked.GetValueOrDefault())
			{
				GAManager.SendEvent("BatchPrintSettingsWindow", "SaveBtn", text + "_Signal", 1L);
				BatchPrintItemModel model = this.PreviewControl.GetSelectedFile();
				if (model != null)
				{
					BatchPrintItemModel batchPrintItemModel = this.documents.FirstOrDefault((BatchPrintItemModel x) => x.FilePath == model.FilePath);
					model._PrintArgs = this.GetPrintArgs();
					batchPrintItemModel._PrintArgs = model._PrintArgs;
					batchPrintItemModel._PrintAnnotations = this.PreviewControl.PrintAnnotations;
					batchPrintItemModel.Copies = (int)this.tboxCopies.Value;
					batchPrintItemModel._PrintArgs.Copies = (int)this.tboxCopies.Value;
				}
				base.Close();
				return;
			}
			GAManager.SendEvent("BatchPrintSettingsWindow", "SaveBtn", text + "_Batch", 1L);
			foreach (BatchPrintItemModel batchPrintItemModel2 in this.documents)
			{
				batchPrintItemModel2._PrintArgs = this.GetPrintArgs();
				batchPrintItemModel2._PrintAnnotations = this.PreviewControl.PrintAnnotations;
				batchPrintItemModel2.Copies = (int)this.tboxCopies.Value;
				batchPrintItemModel2._PrintArgs.Copies = (int)this.tboxCopies.Value;
			}
			base.DialogResult = new bool?(true);
		}

		// Token: 0x04000BE6 RID: 3046
		private List<PaperSizeInfo> paperSizesLst = new List<PaperSizeInfo>();

		// Token: 0x04000BE7 RID: 3047
		private List<PrinterInfo> printLst = new List<PrinterInfo>();

		// Token: 0x04000BE8 RID: 3048
		private ObservableCollection<BatchPrintItemModel> documents;

		// Token: 0x04000BE9 RID: 3049
		private ObservableCollection<BatchPrintItemModel> tempDocumentsInfo;

		// Token: 0x04000BEA RID: 3050
		private CancellationTokenSource updatePreviewCts;
	}
}
