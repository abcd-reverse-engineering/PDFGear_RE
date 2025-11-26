using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using CommonLib.Common;
using CommonLib.Controls;
using pdfeditor.Models.Print;
using pdfeditor.Models.Thumbnails;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.Utils.Print;
using pdfeditor.Utils.Printer;
using pdfeditor.ViewModels;
using pdfeditor.Views;

namespace pdfeditor.Controls.Printer
{
	// Token: 0x0200022E RID: 558
	public partial class WinPrinterDialog : Window
	{
		// Token: 0x06001F79 RID: 8057 RVA: 0x0008C7E8 File Offset: 0x0008A9E8
		public WinPrinterDialog(MainViewModel model, Source source = Source.Default)
		{
			this.InitializeComponent();
			this.VM = model;
			this.PreviewControl.PrintAnnotations = this.VM.IsAnnotationVisible;
			this.PreviewControl.FileComboBoxVisibility = Visibility.Collapsed;
			this.GetPrinterList();
			this.InitPrinterSet(source);
		}

		// Token: 0x06001F7A RID: 8058 RVA: 0x0008C850 File Offset: 0x0008AA50
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

		// Token: 0x06001F7B RID: 8059 RVA: 0x0008C888 File Offset: 0x0008AA88
		private void InitPrinterSet(Source source)
		{
			if (source == Source.Thumbnail)
			{
				this.rdbtnSelectPage.IsChecked = new bool?(true);
				global::System.Windows.Controls.TextBox textBox = this.tboxPageRang;
				MainViewModel vm = this.VM;
				string text;
				if (vm == null)
				{
					text = null;
				}
				else
				{
					List<PdfThumbnailModel> selectedThumbnailList = vm.SelectedThumbnailList;
					if (selectedThumbnailList == null)
					{
						text = null;
					}
					else
					{
						IEnumerable<int> enumerable = selectedThumbnailList.Select((PdfThumbnailModel c) => c.PageIndex);
						if (enumerable == null)
						{
							text = null;
						}
						else
						{
							int[] array = enumerable.ToArray<int>();
							int[] array2;
							text = ((array != null) ? array.ConvertToRange(out array2) : null);
						}
					}
				}
				textBox.Text = text;
			}
			else
			{
				this.tboxPageRang.Text = string.Format("1-{0}", this.VM.Document.Pages.Count);
			}
			this.tballCount.Text = string.Format("/ {0}", this.VM.Document.Pages.Count);
			this.rdbtnCurrentPage.Content = string.Format("{0} ({1} / {2})", pdfeditor.Properties.Resources.WinWatermarkCurrentpageContent, this.VM.CurrnetPageIndex, this.VM.TotalPagesCount);
		}

		// Token: 0x06001F7C RID: 8060 RVA: 0x0008C9A8 File Offset: 0x0008ABA8
		private void GetPrinterList()
		{
			try
			{
				PrintDocument printDocument = new PrintDocument();
				string sDefault = printDocument.PrinterSettings.PrinterName;
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
					this.btnPrint.IsEnabled = false;
				}
				this.cboxPrinterList.ItemsSource = this.printLst;
				this.cboxPrinterList.SelectedIndex = 0;
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06001F7D RID: 8061 RVA: 0x0008CAD4 File Offset: 0x0008ACD4
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
			this.ClearPrinterList();
		}

		// Token: 0x06001F7E RID: 8062 RVA: 0x0008CAE4 File Offset: 0x0008ACE4
		private async void btnBatchPrint_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
			this.ClearPrinterList();
			await this.VM.BatchPrintAsync("PrintWindow");
		}

		// Token: 0x06001F7F RID: 8063 RVA: 0x0008CB1C File Offset: 0x0008AD1C
		private void btnPrint_Click(object sender, RoutedEventArgs e)
		{
			WinPrinterDialog.<>c__DisplayClass10_0 CS$<>8__locals1 = new WinPrinterDialog.<>c__DisplayClass10_0();
			CS$<>8__locals1.lastErrBox = null;
			if (!this.tboxCopies.IsValid)
			{
				CS$<>8__locals1.lastErrBox = this.tboxCopies;
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinPrinterPageRangErrorContent, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			else if (!this.tboxScaleUnit.IsValid)
			{
				CS$<>8__locals1.lastErrBox = this.tboxScaleUnit;
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinPrinterPageRangErrorContent, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
			}
			if (CS$<>8__locals1.lastErrBox != null)
			{
				base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
				{
					WinPrinterDialog.<>c__DisplayClass10_0.<<btnPrint_Click>b__0>d <<btnPrint_Click>b__0>d;
					<<btnPrint_Click>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<btnPrint_Click>b__0>d.<>4__this = CS$<>8__locals1;
					<<btnPrint_Click>b__0>d.<>1__state = -1;
					<<btnPrint_Click>b__0>d.<>t__builder.Start<WinPrinterDialog.<>c__DisplayClass10_0.<<btnPrint_Click>b__0>d>(ref <<btnPrint_Click>b__0>d);
				}));
				return;
			}
			PrintArgs printArgs = this.GetPrintArgs();
			if (printArgs.Document == null || printArgs.PrintPageIndexMapper == null)
			{
				return;
			}
			if (printArgs.AllCount < 1)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinWrongPageRangTipContent, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			PrintService.SaveSettings(new PrintSettings
			{
				Device = printArgs.PrinterName,
				Paper = printArgs.PaperSize,
				IsGrayscale = printArgs.Grayscale,
				Duplex = printArgs.Duplex,
				Landscape = printArgs.Landscape,
				PapersPerSheet = printArgs.PapersPerSheet,
				PaperRowNum = printArgs.PaperRowNum,
				PaperColumnNum = printArgs.PaperColumnNum,
				TypesettingModel = printArgs.TypesettingModel,
				PageOrder = printArgs.PageOrder,
				BindingDirection = printArgs.BindingDirection,
				bookletSubset = printArgs.bookletSubset,
				TilePageZoom = printArgs.TilePageZoom,
				TileOverlap = printArgs.TileOverlap,
				TileCutMasks = printArgs.TileCutMasks,
				TileLabels = printArgs.TileLabels,
				PrintBorder = printArgs.PrintBorder,
				PageMargins = printArgs.PageMargins,
				PrintReverse = printArgs.PrintReverse,
				MutilplePageMargins = printArgs.MutilplePageMargins,
				TilePageMargins = printArgs.TilePageMargins
			});
			base.Close();
			GAManager.SendEvent("PdfPrintDocument", "PrintBtnClick", "Count", 1L);
			GAManager.SendEvent("PdfPrintDocument", "Grayscale", printArgs.Grayscale ? "Gray" : "Color", 1L);
			GAManager.SendEvent("PdfPrintDocument", "ReverseOrder", printArgs.PrintReverse ? "Reverse" : "Normal", 1L);
			GAManager.SendEvent("PdfPrintDocument", "PrintAnnotation", this.PreviewControl.PrintAnnotations ? "HasAnnotation" : "NoAnnotation", 1L);
			GAManager.SendEvent("PdfPrintDocument", "PrintMode", printArgs.TypesettingModel.ToString(), 1L);
			new WinPrinterProgress(printArgs, this.PreviewControl.PrintAnnotations)
			{
				Owner = App.Current.Windows.OfType<MainView>().FirstOrDefault<MainView>()
			}.ShowDialog();
			this.ClearPrinterList();
		}

		// Token: 0x06001F80 RID: 8064 RVA: 0x0008CDDC File Offset: 0x0008AFDC
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
			this.PreviewControl.PrintArgs = null;
		}

		// Token: 0x06001F81 RID: 8065 RVA: 0x0008CE84 File Offset: 0x0008B084
		private PrintArgs GetPrintArgs()
		{
			PrinterInfo printerInfo = this.cboxPrinterList.SelectedItem as PrinterInfo;
			PrintArgs printArgs = new PrintArgs
			{
				Grayscale = this.GrayscaleCheckbox.IsChecked.GetValueOrDefault(),
				PrinterName = printerInfo.PrinterName,
				AutoRotate = (this.TileBtn.IsChecked.GetValueOrDefault() > false),
				AutoCenter = true,
				Copies = (int)this.tboxCopies.Value,
				Collate = this.chkbPrintCollate.IsChecked.Value,
				Landscape = this.rdbtnLandscape.IsChecked.Value,
				PaperSize = ((PaperSizeInfo)this.cboxPaperSize.SelectedItem).PaperSize,
				DocumentPath = this.VM.DocumentWrapper.DocumentPath,
				PrintDevMode = printerInfo.PrintDevMode,
				Duplex = (this.TileBtn.IsChecked.GetValueOrDefault() ? Duplex.Simplex : ((this.BookletSubset.SelectedIndex == 0 && this.BookletBtn.IsChecked.GetValueOrDefault() && !this.rdbtnLandscape.IsChecked.Value) ? Duplex.Vertical : ((this.BookletSubset.SelectedIndex == 0 && this.BookletBtn.IsChecked.GetValueOrDefault() && this.rdbtnLandscape.IsChecked.Value) ? Duplex.Horizontal : ((this.BookletSubset.SelectedIndex != 0 && this.BookletBtn.IsChecked.GetValueOrDefault()) ? Duplex.Simplex : ((!this.chkbDuplex.IsChecked.GetValueOrDefault()) ? Duplex.Simplex : ((!this.rdbtnDuplexVertical.IsChecked.GetValueOrDefault()) ? Duplex.Horizontal : Duplex.Vertical)))))),
				TypesettingModel = (this.ScalBtn.IsChecked.GetValueOrDefault() ? PrintTypeSettingModel.Scale : (this.MutilpleBtn.IsChecked.GetValueOrDefault() ? PrintTypeSettingModel.Multiple : (this.BookletBtn.IsChecked.GetValueOrDefault() ? PrintTypeSettingModel.Booklet : (this.TileBtn.IsChecked.GetValueOrDefault() ? PrintTypeSettingModel.Tile : PrintTypeSettingModel.Scale)))),
				PapersPerSheet = (int)(this.MutilpleRowNum.Value * this.MutilpleColumnNum.Value),
				PaperRowNum = (int)this.MutilpleRowNum.Value,
				PaperColumnNum = (int)this.MutilpleColumnNum.Value,
				PageOrder = (this.Layout1Btn1.IsChecked.GetValueOrDefault() ? PageOrder.Horizontal : (this.Layout1Btn2.IsChecked.GetValueOrDefault() ? PageOrder.HorizontalReverse : (this.Layout1Btn3.IsChecked.GetValueOrDefault() ? PageOrder.Vertical : (this.Layout1Btn4.IsChecked.GetValueOrDefault() ? PageOrder.VerticalReverse : PageOrder.Horizontal)))),
				BindingDirection = ((this.BookletBinding.SelectedIndex == 0) ? BookletBindingDirection.Left : BookletBindingDirection.Right),
				bookletSubset = ((this.BookletSubset.SelectedIndex == 0) ? pdfeditor.Utils.Print.BookletSubset.BothSide : ((this.BookletSubset.SelectedIndex == 1) ? pdfeditor.Utils.Print.BookletSubset.Frontal : pdfeditor.Utils.Print.BookletSubset.Back)),
				TilePageZoom = this.PageZoomNum.Value,
				TileOverlap = this.OverlapNum.Value,
				TileCutMasks = this.CutMasks.IsChecked.Value,
				TileLabels = this.Labels.IsChecked.Value,
				PrintBorder = (this.MutilpleBtn.IsChecked.GetValueOrDefault() && this.MutilplePrintBorderCheckbox.IsChecked.Value),
				PageMargins = (this.MutilpleBtn.IsChecked.GetValueOrDefault() ? this.PrintMarginsTextbox.Value : (this.BookletBtn.IsChecked.GetValueOrDefault() ? this.PrintMarginsBookletTextbox.Value : 0.0)),
				PrintReverse = this.PrintReverseCheckbox.IsChecked.Value,
				MutilplePageMargins = this.PrintMarginsTextbox.Value,
				TilePageMargins = this.PrintMarginsBookletTextbox.Value,
				Document = this.VM.Document,
				PrintPageIndexMapper = this.GetPrintPageIndexMapper()
			};
			if (this.rdbtnFitPage.IsChecked.GetValueOrDefault())
			{
				printArgs.PrintSizeMode = PrintSizeMode.Fit;
			}
			else if (this.rdbtnActualSize.IsChecked.GetValueOrDefault())
			{
				printArgs.PrintSizeMode = PrintSizeMode.ActualSize;
			}
			else
			{
				printArgs.PrintSizeMode = PrintSizeMode.CustomScale;
				printArgs.Scale = (int)this.tboxScaleUnit.Value;
			}
			return printArgs;
		}

		// Token: 0x06001F82 RID: 8066 RVA: 0x0008D334 File Offset: 0x0008B534
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
			this.tboxCopies.Value = (double)args.Copies;
			this.chkbPrintCollate.IsChecked = new bool?(args.Collate);
			if (args.Landscape)
			{
				this.rdbtnLandscape.IsChecked = new bool?(true);
			}
			else
			{
				this.rdbtnPortrait.IsChecked = new bool?(true);
			}
			this.chkbDuplex.IsChecked = new bool?(args.Duplex != Duplex.Simplex);
			this.rdbtnDuplexVertical.IsChecked = new bool?(args.Duplex == Duplex.Vertical);
			this.rdbtnDuplexHorizontal.IsChecked = new bool?(args.Duplex != Duplex.Vertical);
			if (args.PrintSizeMode == PrintSizeMode.Fit)
			{
				this.rdbtnFitPage.IsChecked = new bool?(true);
			}
			else if (args.PrintSizeMode == PrintSizeMode.ActualSize)
			{
				this.rdbtnActualSize.IsChecked = new bool?(true);
			}
			else
			{
				this.rdbtnScale.IsChecked = new bool?(true);
				this.tboxScaleUnit.Value = (double)args.Scale;
			}
			this.UpdatePrintProperties(args.PaperSize);
		}

		// Token: 0x06001F83 RID: 8067 RVA: 0x0008D51F File Offset: 0x0008B71F
		private void cboxPrinterList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PaperSizeInfo paperSizeInfo = this.cboxPaperSize.SelectedItem as PaperSizeInfo;
			this.UpdatePrintProperties((paperSizeInfo != null) ? paperSizeInfo.PaperSize : null);
		}

		// Token: 0x06001F84 RID: 8068 RVA: 0x0008D544 File Offset: 0x0008B744
		private void UpdatePrintProperties(PaperSize paperSize = null)
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
			if (this.paperSizesLst.Count == 0)
			{
				GAManager.SendEvent("PdfPrintDocument", "PaperSizeList", "Empty", 1L);
			}
			this.cboxPaperSize.ItemsSource = this.paperSizesLst;
			PaperSize defaultPaperSize = paperSize ?? printerSettings.DefaultPageSettings.PaperSize;
			PrintSettings PrintSettings = PrintService.LoadSettings(printerInfo.PrinterName);
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
					goto IL_031A;
				case 3:
				case 5:
					break;
				case 4:
					this.TotalPageInPaper.SelectedIndex = 1;
					goto IL_031A;
				case 6:
					this.TotalPageInPaper.SelectedIndex = 2;
					goto IL_031A;
				default:
					if (num == 9)
					{
						this.TotalPageInPaper.SelectedIndex = 3;
						goto IL_031A;
					}
					if (num == 16)
					{
						this.TotalPageInPaper.SelectedIndex = 4;
						goto IL_031A;
					}
					break;
				}
				this.TotalPageInPaper.SelectedIndex = 5;
				IL_031A:
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
				defaultPaperSize = printerSettings.DefaultPageSettings.PaperSize;
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
			this.UpdatePreview();
		}

		// Token: 0x06001F85 RID: 8069 RVA: 0x0008DB04 File Offset: 0x0008BD04
		private PrintPageIndexMapper GetPrintPageIndexMapper()
		{
			int[] array = null;
			if (this.rdbtnAllPages.IsChecked.GetValueOrDefault())
			{
				array = new int[this.VM.Document.Pages.Count];
				for (int i = 0; i < this.VM.Document.Pages.Count; i++)
				{
					array[i] = i;
				}
			}
			else if (this.rdbtnCurrentPage.IsChecked.GetValueOrDefault())
			{
				array = new int[] { this.VM.Document.Pages.CurrentIndex };
			}
			else
			{
				string text = this.tboxPageRang.Text.Trim();
				if (string.IsNullOrEmpty(text))
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.WinWrongPageRangTipContent, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return null;
				}
				int[] array2;
				int num;
				PdfObjectExtensions.TryParsePageRange(text, out array2, out num);
				if (array2 == null)
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.WinWrongPageRangTipContent, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return null;
				}
				if (array2.Length != 0)
				{
					array = array2;
				}
			}
			if (this.cboxSubset.SelectedIndex == 2)
			{
				array = array.Where((int c) => c % 2 == 0).ToArray<int>();
			}
			else if (this.cboxSubset.SelectedIndex == 1)
			{
				array = array.Where((int c) => c % 2 == 1).ToArray<int>();
			}
			if (array.Any((int c) => c < 0 || c >= this.VM.Document.Pages.Count))
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinWrongPageRangTipContent, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return null;
			}
			if (array.Length == 0)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinWrongPageRangTipContent, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return null;
			}
			return new PrintPageIndexMapper(array, this.VM.Document.Pages.Count);
		}

		// Token: 0x06001F86 RID: 8070 RVA: 0x0008DCD1 File Offset: 0x0008BED1
		private void tboxPageRang_TextInput(object sender, TextCompositionEventArgs e)
		{
		}

		// Token: 0x06001F87 RID: 8071 RVA: 0x0008DCD4 File Offset: 0x0008BED4
		private void tboxScaleUnit_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (e.Text != ".")
			{
				Regex regex = new Regex("[^0-9]+");
				e.Handled = regex.IsMatch(e.Text);
			}
		}

		// Token: 0x06001F88 RID: 8072 RVA: 0x0008DD10 File Offset: 0x0008BF10
		private void tboxPageRang_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if (e.Text != "," && e.Text != "-")
			{
				Regex regex = new Regex("[^0-9]+");
				e.Handled = regex.IsMatch(e.Text);
			}
		}

		// Token: 0x06001F89 RID: 8073 RVA: 0x0008DD60 File Offset: 0x0008BF60
		private void PrinterPreferenceButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PdfPrintDocument", "PrinterPreferenceBtnClick", "Count", 1L);
			PrintArgs printArgs = this.GetPrintArgs();
			if (printArgs == null)
			{
				return;
			}
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
				this.UpdatePreview();
			}
		}

		// Token: 0x06001F8A RID: 8074 RVA: 0x0008DEE4 File Offset: 0x0008C0E4
		private void ClassicModeBtn_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
			this.ClearPrinterList();
			GAManager.SendEvent("PdfPrintDocument", "ClassicModeBtnClick", "Count", 1L);
			PrintPageIndexMapper printPageIndexMapper = new PrintPageIndexMapper(null, this.VM.Document.Pages.Count);
			PdfPrintDocument pdfPrintDocument = new PdfPrintDocument(this.VM.Document, printPageIndexMapper, this.PreviewControl.PrintAnnotations);
			global::System.Windows.Forms.PrintDialog dlg = new global::System.Windows.Forms.PrintDialog();
			dlg.AllowCurrentPage = true;
			dlg.AllowSomePages = true;
			dlg.UseEXDialog = true;
			dlg.Document = pdfPrintDocument;
			global::System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
			{
				if (dlg.ShowDialog() == global::System.Windows.Forms.DialogResult.OK)
				{
					try
					{
						dlg.Document.Print();
					}
					catch (Win32Exception ex)
					{
						GAManager.SendEvent("Exception", "PrintBtn", ex.Message, 1L);
					}
				}
			}));
		}

		// Token: 0x06001F8B RID: 8075 RVA: 0x0008DFAC File Offset: 0x0008C1AC
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

		// Token: 0x06001F8C RID: 8076 RVA: 0x0008E004 File Offset: 0x0008C204
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
				PrintArgs printArgs = this.GetPrintArgs();
				this.PreviewControl.PrintArgs = printArgs;
			}));
		}

		// Token: 0x06001F8D RID: 8077 RVA: 0x0008E070 File Offset: 0x0008C270
		private async void PageRange_Checked(object sender, RoutedEventArgs e)
		{
			if (e.Source is global::System.Windows.Controls.RadioButton)
			{
				await base.Dispatcher.InvokeAsync(delegate
				{
					this.UpdatePreview();
				});
			}
		}

		// Token: 0x06001F8E RID: 8078 RVA: 0x0008E0B0 File Offset: 0x0008C2B0
		private async void tboxPageRang_LostFocus(object sender, RoutedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001F8F RID: 8079 RVA: 0x0008E0E8 File Offset: 0x0008C2E8
		private async void cboxSubset_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001F90 RID: 8080 RVA: 0x0008E120 File Offset: 0x0008C320
		private async void cboxPaperSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001F91 RID: 8081 RVA: 0x0008E158 File Offset: 0x0008C358
		private async void PrintOrientation_Checked(object sender, RoutedEventArgs e)
		{
			if (e.Source is global::System.Windows.Controls.RadioButton)
			{
				await base.Dispatcher.InvokeAsync(delegate
				{
					this.UpdatePreview();
				});
			}
		}

		// Token: 0x06001F92 RID: 8082 RVA: 0x0008E198 File Offset: 0x0008C398
		private async void PrintMode_Checked(object sender, RoutedEventArgs e)
		{
			if (e.Source is global::System.Windows.Controls.RadioButton)
			{
				await base.Dispatcher.InvokeAsync(delegate
				{
					this.UpdatePreview();
				});
			}
		}

		// Token: 0x06001F93 RID: 8083 RVA: 0x0008E1D8 File Offset: 0x0008C3D8
		private async void tboxScaleUnit_LostFocus(object sender, RoutedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001F94 RID: 8084 RVA: 0x0008E210 File Offset: 0x0008C410
		private async void GrayscaleCheckbox_Click(object sender, RoutedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001F95 RID: 8085 RVA: 0x0008E248 File Offset: 0x0008C448
		private async void TypeSettings_Checked(object sender, RoutedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001F96 RID: 8086 RVA: 0x0008E280 File Offset: 0x0008C480
		private async void BookletBinding_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001F97 RID: 8087 RVA: 0x0008E2B8 File Offset: 0x0008C4B8
		private async void CutMasks_Checked(object sender, RoutedEventArgs e)
		{
			await base.Dispatcher.InvokeAsync(delegate
			{
				this.UpdatePreview();
			});
		}

		// Token: 0x06001F98 RID: 8088 RVA: 0x0008E2F0 File Offset: 0x0008C4F0
		private async void TotalPageInPaper_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.MutilpleRowNum != null && this.MutilpleColumnNum != null)
			{
				switch ((sender as global::System.Windows.Controls.ComboBox).SelectedIndex)
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

		// Token: 0x04000C67 RID: 3175
		private MainViewModel VM;

		// Token: 0x04000C68 RID: 3176
		public List<PaperSizeInfo> paperSizesLst = new List<PaperSizeInfo>();

		// Token: 0x04000C69 RID: 3177
		public List<PrinterInfo> printLst = new List<PrinterInfo>();

		// Token: 0x04000C6A RID: 3178
		private CancellationTokenSource updatePreviewCts;
	}
}
