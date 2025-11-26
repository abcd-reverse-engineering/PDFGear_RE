using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Printing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using CommonLib.Common;
using CommonLib.Controls;
using Patagames.Pdf.Enums;
using pdfeditor.Properties;
using pdfeditor.Utils.Print;
using pdfeditor.Utils.Printer;

namespace pdfeditor.Controls.Printer
{
	// Token: 0x02000230 RID: 560
	public partial class WinPrinterProgress : Window
	{
		// Token: 0x06001FA9 RID: 8105 RVA: 0x0008EB1C File Offset: 0x0008CD1C
		public WinPrinterProgress(PrintArgs args, bool printAnnotations = true)
		{
			WinPrinterProgress <>4__this = this;
			this.InitializeComponent();
			this.printArgs = args;
			this.printAnnotations = printAnnotations;
			if (!string.IsNullOrEmpty(args.DocumentTitle))
			{
				this.DocumentTitle.Visibility = Visibility.Visible;
				this.DocumentTitle.Text = args.DocumentTitle;
			}
			this.GetPrintPagesCount();
			Task.Run(delegate
			{
				<>4__this.SilentPrinting(args);
			});
		}

		// Token: 0x06001FAA RID: 8106 RVA: 0x0008EBB4 File Offset: 0x0008CDB4
		private void GetPrintPagesCount()
		{
			if (this.printArgs.TypesettingModel == PrintTypeSettingModel.Scale)
			{
				this.printArgs.TotalPages = this.printArgs.AllCount;
				return;
			}
			if (this.printArgs.TypesettingModel == PrintTypeSettingModel.Multiple)
			{
				this.printArgs.TotalPages = ((this.printArgs.AllCount % this.printArgs.PapersPerSheet != 0) ? (this.printArgs.AllCount / this.printArgs.PapersPerSheet + 1) : (this.printArgs.AllCount / this.printArgs.PapersPerSheet));
				return;
			}
			if (this.printArgs.TypesettingModel == PrintTypeSettingModel.Booklet)
			{
				if (this.printArgs.AllCount % 4 != 0)
				{
					this.printArgs.TotalPages = (this.printArgs.AllCount / 4 + 1) * 2;
				}
				else
				{
					this.printArgs.TotalPages = this.printArgs.AllCount / 2;
				}
				if (this.printArgs.bookletSubset != BookletSubset.BothSide)
				{
					this.printArgs.TotalPages = this.printArgs.TotalPages / 2;
					return;
				}
			}
			else if (this.printArgs.TypesettingModel == PrintTypeSettingModel.Tile)
			{
				int num = 0;
				foreach (int num2 in this.printArgs.PrintPageIndexMapper.GetPageRange())
				{
					double num3 = (double)this.printArgs.Document.Pages[num2].Width / 72.0;
					double num4 = (double)this.printArgs.Document.Pages[num2].Height / 72.0;
					double num5 = num3 / num4;
					num4 = num4 * this.printArgs.TilePageZoom / 100.0;
					num3 = num3 * this.printArgs.TilePageZoom / 100.0;
					Size printableArea = this.GetPrintableArea(this.printArgs.PrinterName);
					double num6 = (double)this.printArgs.PaperSize.Height / 100.0 - printableArea.Height * 2.0 / 100.0;
					double num7 = (double)this.printArgs.PaperSize.Width / 100.0 - printableArea.Width * 2.0 / 100.0;
					if (this.printArgs.Landscape)
					{
						num6 = (double)(this.printArgs.PaperSize.Width / 100) - printableArea.Width * 2.0 / 100.0;
						num7 = (double)(this.printArgs.PaperSize.Height / 100) - printableArea.Height * 2.0 / 100.0;
					}
					if (this.printArgs.TileCutMasks || this.printArgs.TileLabels)
					{
						num6 -= 0.8;
						num7 -= 0.8;
					}
					int num8 = (int)Math.Ceiling(Math.Round(num4 / num6, 2));
					int num9 = (int)Math.Ceiling(Math.Round(num3 / num7, 2));
					if (this.printArgs.TileCutMasks || this.printArgs.TileLabels)
					{
						num4 += 0.8;
						num3 += 0.8;
					}
					double num10 = (double)num8 * num6;
					double num11 = (double)num9 * num7;
					if ((double)(num8 - 1) * this.printArgs.TileOverlap * 0.39370078740157 + num4 > num10)
					{
						num8++;
						num10 = (double)num8 * num6;
					}
					if ((double)(num9 - 1) * this.printArgs.TileOverlap * 0.39370078740157 + num3 > num11)
					{
						num9++;
						num11 = (double)num9 * num7;
					}
					num += num9 * num8;
				}
				this.printArgs.TotalPages = num;
			}
		}

		// Token: 0x06001FAB RID: 8107 RVA: 0x0008EF98 File Offset: 0x0008D198
		public Size GetPrintableArea(string printerName)
		{
			Size size;
			using (LocalPrintServer localPrintServer = new LocalPrintServer())
			{
				using (PrintQueue printQueue = localPrintServer.GetPrintQueue(printerName))
				{
					PrintCapabilities printCapabilities = printQueue.GetPrintCapabilities();
					if (((printCapabilities != null) ? printCapabilities.PageImageableArea : null) == null)
					{
						size = new Size(0.0, 0.0);
					}
					else
					{
						size = new Size(printCapabilities.PageImageableArea.OriginWidth, printCapabilities.PageImageableArea.OriginHeight);
					}
				}
			}
			return size;
		}

		// Token: 0x06001FAC RID: 8108 RVA: 0x0008F034 File Offset: 0x0008D234
		private async Task<bool> SilentPrinting(PrintArgs args)
		{
			bool flag;
			try
			{
				await Task.Run(delegate
				{
					using (PdfPrintDocument pdfPrintDocument = new PdfPrintDocument(args.Document, args.PrintPageIndexMapper, this.printAnnotations))
					{
						pdfPrintDocument.PrinterSettings.PrinterName = args.PrinterName;
						PageSettings pageSettings = new PageSettings(pdfPrintDocument.PrinterSettings);
						pdfPrintDocument.DefaultPageSettings = pageSettings;
						pdfPrintDocument.PrinterSettings.SetHdevmode(args.PrintDevMode);
						pdfPrintDocument.AutoRotate = args.TypesettingModel != PrintTypeSettingModel.Tile && args.AutoRotate;
						pdfPrintDocument.AutoCenter = args.AutoCenter;
						pdfPrintDocument.PrinterSettings.Copies = (short)args.Copies;
						pdfPrintDocument.PrintSizeMode = args.PrintSizeMode;
						pdfPrintDocument.Scale = args.Scale;
						pdfPrintDocument.PrinterSettings.Collate = args.Collate;
						pdfPrintDocument.PrinterSettings.Duplex = args.Duplex;
						if (args.Grayscale)
						{
							pdfPrintDocument.RenderFlags |= RenderFlags.FPDF_GRAYSCALE;
						}
						pdfPrintDocument.PrintTypeSettingModel = args.TypesettingModel;
						pdfPrintDocument.PapersPerSheet = args.PapersPerSheet;
						pdfPrintDocument.PaperRowNum = args.PaperRowNum;
						pdfPrintDocument.PaperColumnNum = args.PaperColumnNum;
						pdfPrintDocument.PageOrder = args.PageOrder;
						pdfPrintDocument.isPreviewControl = false;
						pdfPrintDocument.TilePageZoom = args.TilePageZoom;
						pdfPrintDocument.TileCutMasks = args.TileCutMasks;
						pdfPrintDocument.TileLabels = args.TileLabels;
						pdfPrintDocument.TileFilePath = args.DocumentPath;
						pdfPrintDocument.TileOverlap = args.TileOverlap;
						pdfPrintDocument.PrintBorder = args.PrintBorder;
						pdfPrintDocument.PageMargins = args.PageMargins;
						pdfPrintDocument.PrintReverse = args.PrintReverse;
						pdfPrintDocument.BookletBindingDirection = args.BindingDirection;
						pdfPrintDocument.BookletSubset = args.bookletSubset;
						pageSettings.PaperSize = args.PaperSize;
						pageSettings.Landscape = args.Landscape;
						pageSettings.Color = !args.Grayscale;
						pdfPrintDocument.PrintController = this.printController;
						pdfPrintDocument.PrintPage += this.PrintPage;
						pdfPrintDocument.EndPrint += this.EndPrint;
						pdfPrintDocument.Print();
					}
				});
				flag = true;
			}
			catch (Exception ex)
			{
				GAManager.SendEvent("Exception", "PrintBtn", ex.Message, 1L);
				base.Dispatcher.Invoke(delegate
				{
					this.DialogResult = new bool?(false);
				});
				flag = false;
			}
			return flag;
		}

		// Token: 0x06001FAD RID: 8109 RVA: 0x0008F07F File Offset: 0x0008D27F
		private void EndPrint(object sender, PrintEventArgs e)
		{
			base.Dispatcher.Invoke(delegate
			{
				this.Value = 1.0;
				base.Close();
			});
		}

		// Token: 0x06001FAE RID: 8110 RVA: 0x0008F098 File Offset: 0x0008D298
		private void PrintPage(object sender, PrintPageEventArgs e)
		{
			base.Dispatcher.Invoke(delegate
			{
				this.printedCount += 1.0;
				this.ProgressText.Text = string.Format("{0} / {1}", this.printedCount, this.printArgs.TotalPages);
				this.Value = this.printedCount / Convert.ToDouble(this.printArgs.TotalPages);
				if (this.Value == 1.0)
				{
					base.DialogResult = new bool?(true);
				}
			});
		}

		// Token: 0x17000AD1 RID: 2769
		// (get) Token: 0x06001FAF RID: 8111 RVA: 0x0008F0B1 File Offset: 0x0008D2B1
		// (set) Token: 0x06001FB0 RID: 8112 RVA: 0x0008F0C3 File Offset: 0x0008D2C3
		public double Value
		{
			get
			{
				return (double)base.GetValue(WinPrinterProgress.ValueProperty);
			}
			set
			{
				base.SetValue(WinPrinterProgress.ValueProperty, value);
			}
		}

		// Token: 0x06001FB1 RID: 8113 RVA: 0x0008F0D6 File Offset: 0x0008D2D6
		private void Window_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				base.DragMove();
			}
		}

		// Token: 0x06001FB2 RID: 8114 RVA: 0x0008F0E8 File Offset: 0x0008D2E8
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			if (ModernMessageBox.Show(this, pdfeditor.Properties.Resources.WinPrinterCancelPrintContent, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, true) == MessageBoxResult.Yes)
			{
				GAManager.SendEvent("PdfPrintDocument", "CancelBtnClick", "Count", 1L);
				PdfPrintDocument.IsCancel = true;
				base.DialogResult = new bool?(false);
			}
		}

		// Token: 0x04000CA8 RID: 3240
		private PrintArgs printArgs;

		// Token: 0x04000CA9 RID: 3241
		private readonly bool printAnnotations;

		// Token: 0x04000CAA RID: 3242
		private readonly StandardPrintController printController = new StandardPrintController();

		// Token: 0x04000CAB RID: 3243
		private double printedCount;

		// Token: 0x04000CAC RID: 3244
		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(WinPrinterProgress), new PropertyMetadata(0.0));
	}
}
