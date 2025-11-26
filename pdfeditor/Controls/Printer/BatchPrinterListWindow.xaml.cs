using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CommonLib.Common;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Patagames.Pdf.Net;
using pdfeditor.Models;
using pdfeditor.Models.Print;
using pdfeditor.Properties;
using pdfeditor.Utils.Print;

namespace pdfeditor.Controls.Printer
{
	// Token: 0x02000226 RID: 550
	public partial class BatchPrinterListWindow : Window
	{
		// Token: 0x06001EAD RID: 7853 RVA: 0x00088410 File Offset: 0x00086610
		public BatchPrinterListWindow()
		{
			this.InitializeComponent();
			this.closeTokenSource = new CancellationTokenSource();
			this.documents = new ObservableCollection<BatchPrintItemModel>();
			this.UpdatePrintProperties(null);
			this.lsvFiles.ItemsSource = this.documents;
			foreach (BatchPrintItemModel batchPrintItemModel in this.documents)
			{
				batchPrintItemModel._PrintArgs = this.printArgs;
			}
			base.Closing += this.BatchPrinterListWindow_Closing;
		}

		// Token: 0x06001EAE RID: 7854 RVA: 0x000884B8 File Offset: 0x000866B8
		private void BatchPrinterListWindow_Closing(object sender, CancelEventArgs e)
		{
			ObservableCollection<BatchPrintItemModel> observableCollection = this.documents;
			if (observableCollection == null)
			{
				return;
			}
			observableCollection.Clear();
		}

		// Token: 0x06001EAF RID: 7855 RVA: 0x000884CC File Offset: 0x000866CC
		private void UpdatePrintProperties(PaperSize paperSize = null)
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
				this.printArgs = new PrintArgs();
				if (printerInfo2 != null)
				{
					this.printArgs.PrinterName = printerInfo2.PrinterName;
					this.printArgs.PrintDevMode = printerInfo2.PrintDevMode;
				}
				PrinterSettings printerSettings = new PrinterSettings
				{
					PrinterName = printerInfo2.PrinterName
				};
				List<PaperSizeInfo> list = (from c in printerSettings.PaperSizes.OfType<PaperSize>()
					select new PaperSizeInfo
					{
						FriendlyName = c.PaperName,
						PaperSize = c
					}).ToList<PaperSizeInfo>();
				PaperSize defaultPaperSize = paperSize ?? printerSettings.DefaultPageSettings.PaperSize;
				PrintSettings PrintSettings = PrintService.LoadSettings(printerInfo2.PrinterName);
				if (PrintSettings != null)
				{
					PrintArgs printArgs = this.printArgs;
					PaperSizeInfo paperSizeInfo;
					if ((paperSizeInfo = list.FirstOrDefault((PaperSizeInfo c) => c.PaperSize.Kind == PrintSettings.Paper.Kind && c.PaperSize.PaperName == PrintSettings.Paper.PaperName)) == null)
					{
						paperSizeInfo = list.FirstOrDefault((PaperSizeInfo c) => c.FriendlyName == "A4") ?? list.FirstOrDefault<PaperSizeInfo>();
					}
					printArgs.PaperSize = paperSizeInfo.PaperSize;
					this.printArgs.Grayscale = PrintSettings.IsGrayscale;
					this.printArgs.Landscape = PrintSettings.Landscape;
				}
				else
				{
					PrintArgs printArgs2 = this.printArgs;
					PaperSizeInfo paperSizeInfo2;
					if ((paperSizeInfo2 = list.FirstOrDefault((PaperSizeInfo c) => defaultPaperSize != null && c.PaperSize.Kind == defaultPaperSize.Kind && c.PaperSize.PaperName == defaultPaperSize.PaperName)) == null)
					{
						paperSizeInfo2 = list.FirstOrDefault((PaperSizeInfo c) => c.FriendlyName == "A4") ?? list.FirstOrDefault<PaperSizeInfo>();
					}
					printArgs2.PaperSize = paperSizeInfo2.PaperSize;
					this.printArgs.Landscape = false;
				}
				this.printArgs.AutoCenter = true;
				this.printArgs.TypesettingModel = PrintTypeSettingModel.Scale;
			}
			catch
			{
			}
		}

		// Token: 0x06001EB0 RID: 7856 RVA: 0x00088754 File Offset: 0x00086954
		public BatchPrinterListWindow(string filePath, PdfDocument doc)
			: this()
		{
			this.AddDocument(filePath, doc);
		}

		// Token: 0x06001EB1 RID: 7857 RVA: 0x00088764 File Offset: 0x00086964
		private void AddDocument(string filePath, PdfDocument doc)
		{
			BatchPrintItemModel batchPrintItemModel = new BatchPrintItemModel(filePath, doc);
			this.AddDocumentCore(batchPrintItemModel);
		}

		// Token: 0x06001EB2 RID: 7858 RVA: 0x00088780 File Offset: 0x00086980
		private void AddDocument(DocumentWrapper wrapper)
		{
			BatchPrintItemModel batchPrintItemModel = new BatchPrintItemModel(wrapper);
			this.AddDocumentCore(batchPrintItemModel);
		}

		// Token: 0x06001EB3 RID: 7859 RVA: 0x0008879C File Offset: 0x0008699C
		private void AddDocumentCore(BatchPrintItemModel item)
		{
			item._PrintArgs = this.printArgs;
			this.documents.Add(item);
			WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.AddHandler(item, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.OnDocumentPropertyChanged));
			item.RemoveCmd = new RelayCommand(delegate
			{
				GAManager.SendEvent("PdfBatchPrintDocument2", "RemoveDocumentBtnClick", "Count", 1L);
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(item, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.OnDocumentPropertyChanged));
				this.documents.Remove(item);
				item.Dispose();
				this.UpdateTotalDocumentNumber();
			});
			item.SettingCmd = new RelayCommand(delegate
			{
				GAManager.SendEvent("PdfBatchPrintDocument2", "SingleFileSettingBtnClick", "Count", 1L);
				WeakEventManager<INotifyPropertyChanged, PropertyChangedEventArgs>.RemoveHandler(item, "PropertyChanged", new EventHandler<PropertyChangedEventArgs>(this.OnDocumentPropertyChanged));
				if (new BatchPrintSettingsWindow(this.documents, item, true)
				{
					Owner = this,
					WindowStartupLocation = WindowStartupLocation.CenterOwner
				}.ShowDialog().GetValueOrDefault())
				{
					this.printArgs = this.documents[0]._PrintArgs;
				}
				this.UpdateTotalDocumentNumber();
			});
			this.UpdateTotalDocumentNumber();
		}

		// Token: 0x06001EB4 RID: 7860 RVA: 0x0008883C File Offset: 0x00086A3C
		private async Task AddDocumentFromFiles(string[] files)
		{
			if (files != null && files.Length != 0)
			{
				string[] array = files;
				for (int i = 0; i < array.Length; i++)
				{
					BatchPrinterListWindow.<>c__DisplayClass12_0 CS$<>8__locals1 = new BatchPrinterListWindow.<>c__DisplayClass12_0();
					CS$<>8__locals1.<>4__this = this;
					CS$<>8__locals1.fileName = array[i];
					if (this.closeTokenSource.IsCancellationRequested)
					{
						return;
					}
					try
					{
						if (CS$<>8__locals1.fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
						{
							DocumentWrapper wrapper = new DocumentWrapper();
							wrapper.PasswordRequested += delegate(object s, DocumentPasswordRequestedEventArgs a)
							{
								if (CS$<>8__locals1.<>4__this.closeTokenSource.IsCancellationRequested)
								{
									a.Cancel = true;
									return;
								}
								EnterPasswordDialog enterPasswordDialog = new EnterPasswordDialog(Path.GetFileName(CS$<>8__locals1.fileName))
								{
									Owner = CS$<>8__locals1.<>4__this,
									WindowStartupLocation = WindowStartupLocation.CenterOwner
								};
								a.Cancel = !enterPasswordDialog.ShowDialog().GetValueOrDefault();
								a.Password = enterPasswordDialog.Password;
							};
							TaskAwaiter<bool> taskAwaiter = wrapper.OpenAsync(CS$<>8__locals1.fileName, null).GetAwaiter();
							if (!taskAwaiter.IsCompleted)
							{
								await taskAwaiter;
								TaskAwaiter<bool> taskAwaiter2;
								taskAwaiter = taskAwaiter2;
								taskAwaiter2 = default(TaskAwaiter<bool>);
							}
							if (taskAwaiter.GetResult())
							{
								if (this.closeTokenSource.IsCancellationRequested)
								{
									return;
								}
								this.AddDocument(wrapper);
							}
							else
							{
								ModernMessageBox.Show(pdfeditor.Properties.Resources.ScannerWinAddFailed.Replace("XXX", CS$<>8__locals1.fileName), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
							}
							wrapper = null;
						}
					}
					catch
					{
						ModernMessageBox.Show(pdfeditor.Properties.Resources.ScannerWinAddFailed.Replace("XXX", CS$<>8__locals1.fileName), UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					}
					CS$<>8__locals1 = null;
				}
				array = null;
			}
		}

		// Token: 0x06001EB5 RID: 7861 RVA: 0x00088887 File Offset: 0x00086A87
		private void OnDocumentPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.UpdateTotalDocumentNumber();
		}

		// Token: 0x06001EB6 RID: 7862 RVA: 0x0008888F File Offset: 0x00086A8F
		private void UpdateTotalDocumentNumber()
		{
			this.UpdatePrintButtonState();
		}

		// Token: 0x06001EB7 RID: 7863 RVA: 0x00088898 File Offset: 0x00086A98
		private void UpdatePrintButtonState()
		{
			this.btnPrint.IsEnabled = this.documents.Sum((BatchPrintItemModel c) => c.SelectedPageCount) > 0;
			this.btnSettings.IsEnabled = this.documents.Sum((BatchPrintItemModel c) => c.SelectedPageCount) > 0;
		}

		// Token: 0x06001EB8 RID: 7864 RVA: 0x00088918 File Offset: 0x00086B18
		private void btnSettings_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PdfBatchPrintDocument2", "BatchSettingsBtn", "Count", 1L);
			if (new BatchPrintSettingsWindow(this.documents, null, false)
			{
				Owner = this,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			}.ShowDialog().GetValueOrDefault())
			{
				this.printArgs = this.documents[0]._PrintArgs;
			}
		}

		// Token: 0x06001EB9 RID: 7865 RVA: 0x0008897C File Offset: 0x00086B7C
		private void btnPrint_Click(object sender, RoutedEventArgs e)
		{
			BatchPrintItemModel batchPrintItemModel = this.documents.FirstOrDefault((BatchPrintItemModel c) => c.DocumentTotalPageCount > 0 && c.SelectedPageCount <= 0);
			if (batchPrintItemModel != null)
			{
				this.lsvFiles.SelectedItem = batchPrintItemModel;
				this.lsvFiles.ScrollIntoView(batchPrintItemModel);
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinWrongPageRangTipContent, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			BatchPrintItemModel[] array = this.documents.ToArray<BatchPrintItemModel>();
			GAManager.SendEvent("PdfBatchPrintDocument2", "PrintBtnClick", array.Length.ToString(), 1L);
			foreach (BatchPrintItemModel batchPrintItemModel2 in array)
			{
				PrintPageIndexMapper printPageIndexMapper = BatchPrintSettingsWindow.GetPrintPageIndexMapper(batchPrintItemModel2);
				if (printPageIndexMapper == null || printPageIndexMapper.DocumentPageCount < 1)
				{
					batchPrintItemModel2.PrintStatus = 3;
				}
				else
				{
					PrintArgs printArgs = new PrintArgs
					{
						AutoRotate = batchPrintItemModel2._PrintArgs.AutoRotate,
						AutoCenter = batchPrintItemModel2._PrintArgs.AutoCenter,
						Collate = batchPrintItemModel2._PrintArgs.Collate,
						DocumentPath = batchPrintItemModel2.FileName,
						PrintDevMode = batchPrintItemModel2._PrintArgs.PrintDevMode,
						Copies = batchPrintItemModel2.Copies,
						PrinterName = batchPrintItemModel2._PrintArgs.PrinterName,
						PaperSize = batchPrintItemModel2._PrintArgs.PaperSize,
						Grayscale = batchPrintItemModel2._PrintArgs.Grayscale,
						Duplex = batchPrintItemModel2._PrintArgs.Duplex,
						Landscape = batchPrintItemModel2._PrintArgs.Landscape,
						PrintSizeMode = batchPrintItemModel2._PrintArgs.PrintSizeMode,
						PapersPerSheet = batchPrintItemModel2._PrintArgs.PapersPerSheet,
						PaperRowNum = batchPrintItemModel2._PrintArgs.PaperRowNum,
						PaperColumnNum = batchPrintItemModel2._PrintArgs.PaperColumnNum,
						TypesettingModel = batchPrintItemModel2._PrintArgs.TypesettingModel,
						PageOrder = batchPrintItemModel2._PrintArgs.PageOrder,
						BindingDirection = batchPrintItemModel2._PrintArgs.BindingDirection,
						bookletSubset = batchPrintItemModel2._PrintArgs.bookletSubset,
						TilePageZoom = batchPrintItemModel2._PrintArgs.TilePageZoom,
						TileOverlap = batchPrintItemModel2._PrintArgs.TileOverlap,
						TileCutMasks = batchPrintItemModel2._PrintArgs.TileCutMasks,
						TileLabels = batchPrintItemModel2._PrintArgs.TileLabels,
						PrintBorder = batchPrintItemModel2._PrintArgs.PrintBorder,
						PageMargins = batchPrintItemModel2._PrintArgs.PageMargins,
						PrintReverse = batchPrintItemModel2._PrintArgs.PrintReverse,
						MutilplePageMargins = batchPrintItemModel2._PrintArgs.MutilplePageMargins,
						TilePageMargins = batchPrintItemModel2._PrintArgs.TilePageMargins,
						Scale = batchPrintItemModel2._PrintArgs.Scale,
						Document = batchPrintItemModel2.Document,
						PrintPageIndexMapper = printPageIndexMapper
					};
					printArgs.DocumentTitle = batchPrintItemModel2.FileName;
					GAManager.SendEvent("PdfBatchPrintDocument2", "PrintMode", printArgs.TypesettingModel.ToString(), 1L);
					this.lsvFiles.ScrollIntoView(batchPrintItemModel2);
					batchPrintItemModel2.PrintStatus = 1;
					if (new WinPrinterProgress(printArgs, batchPrintItemModel2._PrintAnnotations)
					{
						Owner = this,
						WindowStartupLocation = WindowStartupLocation.CenterOwner
					}.ShowDialog().GetValueOrDefault())
					{
						batchPrintItemModel2.PrintStatus = 2;
					}
					else
					{
						batchPrintItemModel2.PrintStatus = 3;
					}
				}
			}
		}

		// Token: 0x06001EBA RID: 7866 RVA: 0x00088CF0 File Offset: 0x00086EF0
		private async void AddFileBtn_Click(object sender, RoutedEventArgs e)
		{
			base.IsEnabled = false;
			this.ProgressBackground.Visibility = Visibility.Visible;
			try
			{
				GAManager.SendEvent("PdfBatchPrintDocument2", "AddDocumentBtnClick", "Begin", 1L);
				OpenFileDialog openFileDialog = new OpenFileDialog
				{
					Filter = "pdf|*.pdf",
					ShowReadOnly = false,
					ReadOnlyChecked = true,
					Multiselect = true
				};
				if (openFileDialog.ShowDialog(this).GetValueOrDefault())
				{
					GAManager.SendEvent("PdfBatchPrintDocument2", "AddDocumentBtnClick", "Done", 1L);
					await this.AddDocumentFromFiles(openFileDialog.FileNames);
				}
			}
			finally
			{
				base.IsEnabled = true;
				this.ProgressBackground.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x06001EBB RID: 7867 RVA: 0x00088D28 File Offset: 0x00086F28
		private void RemoveAllBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PdfBatchPrintDocument2", "RemoveAllDocumentBtnClick", "Count", 1L);
			if (ModernMessageBox.Show(pdfeditor.Properties.Resources.BatchPrintRemoveAllMessage, UtilManager.GetProductName(), MessageBoxButton.YesNo, MessageBoxResult.None, null, false) == MessageBoxResult.Yes)
			{
				ObservableCollection<BatchPrintItemModel> observableCollection = this.documents;
				if (observableCollection != null)
				{
					observableCollection.Clear();
				}
				this.UpdatePrintButtonState();
			}
		}

		// Token: 0x04000BDA RID: 3034
		private ObservableCollection<BatchPrintItemModel> documents;

		// Token: 0x04000BDB RID: 3035
		private CancellationTokenSource closeTokenSource;

		// Token: 0x04000BDC RID: 3036
		private CancellationTokenSource updatePreviewCts;

		// Token: 0x04000BDD RID: 3037
		private PrintArgs printArgs;

		// Token: 0x04000BDE RID: 3038
		private List<PrinterInfo> printLst = new List<PrinterInfo>();
	}
}
