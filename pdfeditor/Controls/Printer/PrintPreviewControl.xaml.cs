using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonLib.Common;
using CommonLib.Controls;
using CommunityToolkit.Mvvm.Input;
using LruCacheNet;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Models.Print;
using pdfeditor.Utils.Print;
using pdfeditor.Utils.Printer;

namespace pdfeditor.Controls.Printer
{
	// Token: 0x02000229 RID: 553
	public partial class PrintPreviewControl : UserControl
	{
		// Token: 0x06001EEA RID: 7914 RVA: 0x0008AF88 File Offset: 0x00089188
		public PrintPreviewControl()
		{
			this.InitializeComponent();
			this.thumbnailCache = new LruCache<int, global::System.ValueTuple<BitmapSource, double, double>>(5);
			this.NextPageCommand = new RelayCommand(delegate
			{
				if (this.CurrentPage + 1 <= this.TotalPage)
				{
					this.CurrentPage++;
				}
				((RelayCommand)this.PrevPageCommand).NotifyCanExecuteChanged();
			}, () => this.CurrentPage + 1 <= this.TotalPage);
			this.PrevPageCommand = new RelayCommand(delegate
			{
				if (this.CurrentPage - 1 > 0)
				{
					this.CurrentPage--;
				}
				((RelayCommand)this.NextPageCommand).NotifyCanExecuteChanged();
			}, () => this.CurrentPage - 1 > 0);
			this.printingArgs = new ConcurrentDictionary<PrintArgs, int>();
			base.Unloaded += this.PrintPreviewControl_Unloaded;
		}

		// Token: 0x17000A99 RID: 2713
		// (get) Token: 0x06001EEB RID: 7915 RVA: 0x0008B010 File Offset: 0x00089210
		// (set) Token: 0x06001EEC RID: 7916 RVA: 0x0008B022 File Offset: 0x00089222
		public int TotalPage
		{
			get
			{
				return (int)base.GetValue(PrintPreviewControl.TotalPageProperty);
			}
			private set
			{
				base.SetValue(PrintPreviewControl.TotalPagePropertyKey, value);
			}
		}

		// Token: 0x17000A9A RID: 2714
		// (get) Token: 0x06001EED RID: 7917 RVA: 0x0008B035 File Offset: 0x00089235
		public static DependencyProperty TotalPageProperty
		{
			get
			{
				return PrintPreviewControl.TotalPagePropertyKey.DependencyProperty;
			}
		}

		// Token: 0x17000A9B RID: 2715
		// (get) Token: 0x06001EEE RID: 7918 RVA: 0x0008B041 File Offset: 0x00089241
		// (set) Token: 0x06001EEF RID: 7919 RVA: 0x0008B053 File Offset: 0x00089253
		public int CurrentPage
		{
			get
			{
				return (int)base.GetValue(PrintPreviewControl.CurrentPageProperty);
			}
			set
			{
				base.SetValue(PrintPreviewControl.CurrentPageProperty, value);
			}
		}

		// Token: 0x06001EF0 RID: 7920 RVA: 0x0008B068 File Offset: 0x00089268
		private static async void OnCurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PrintPreviewControl printPreviewControl = d as PrintPreviewControl;
			if (printPreviewControl != null && !object.Equals(e.NewValue, e.OldValue))
			{
				object newValue = e.NewValue;
				if (newValue is int)
				{
					int num = (int)newValue;
					if (printPreviewControl.TotalPage != 0 && (num <= 0 || num > printPreviewControl.TotalPage))
					{
						printPreviewControl.CurrentPage = (int)e.OldValue;
						return;
					}
				}
				((RelayCommand)printPreviewControl.NextPageCommand).NotifyCanExecuteChanged();
				((RelayCommand)printPreviewControl.PrevPageCommand).NotifyCanExecuteChanged();
				if (!printPreviewControl.innerSet)
				{
					await printPreviewControl.UpdateImageAsync();
				}
			}
		}

		// Token: 0x17000A9C RID: 2716
		// (get) Token: 0x06001EF1 RID: 7921 RVA: 0x0008B0A7 File Offset: 0x000892A7
		// (set) Token: 0x06001EF2 RID: 7922 RVA: 0x0008B0B9 File Offset: 0x000892B9
		public PrintArgs PrintArgs
		{
			get
			{
				return (PrintArgs)base.GetValue(PrintPreviewControl.PrintArgsProperty);
			}
			set
			{
				base.SetValue(PrintPreviewControl.PrintArgsProperty, value);
			}
		}

		// Token: 0x06001EF3 RID: 7923 RVA: 0x0008B0C8 File Offset: 0x000892C8
		private static async void OnPrintArgsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PrintPreviewControl printPreviewControl = d as PrintPreviewControl;
			if (printPreviewControl != null && e.NewValue != e.OldValue)
			{
				PrintArgs printArgs = e.OldValue as PrintArgs;
				if (printArgs != null)
				{
					printPreviewControl.PrintArgsReleaseRef(printArgs);
				}
				PrintArgs printArgs2 = e.NewValue as PrintArgs;
				if (printArgs2 != null)
				{
					printPreviewControl.PrintArgsAddRef(printArgs2);
					if (printArgs2.TypesettingModel == PrintTypeSettingModel.Scale || printArgs2.TypesettingModel == PrintTypeSettingModel.Tile)
					{
						printPreviewControl.TotalPage = printArgs2.AllCount;
					}
					else if (printArgs2.TypesettingModel == PrintTypeSettingModel.Multiple)
					{
						printPreviewControl.TotalPage = ((printArgs2.AllCount % printArgs2.PapersPerSheet != 0) ? (printArgs2.AllCount / printArgs2.PapersPerSheet + 1) : (printArgs2.AllCount / printArgs2.PapersPerSheet));
					}
					else if (printArgs2.TypesettingModel == PrintTypeSettingModel.Booklet)
					{
						if (printArgs2.AllCount % 4 != 0)
						{
							printPreviewControl.TotalPage = (printArgs2.AllCount / 4 + 1) * 2;
						}
						else
						{
							printPreviewControl.TotalPage = printArgs2.AllCount / 2;
						}
						if (printArgs2.bookletSubset != BookletSubset.BothSide)
						{
							printPreviewControl.TotalPage /= 2;
						}
					}
					printPreviewControl.innerSet = true;
					printPreviewControl.CurrentPage = 1;
					printPreviewControl.innerSet = false;
					LruCache<int, global::System.ValueTuple<BitmapSource, double, double>> lruCache = printPreviewControl.thumbnailCache;
					lock (lruCache)
					{
						printPreviewControl.thumbnailCache.Clear();
					}
					((RelayCommand)printPreviewControl.NextPageCommand).NotifyCanExecuteChanged();
					((RelayCommand)printPreviewControl.PrevPageCommand).NotifyCanExecuteChanged();
					await printPreviewControl.UpdateImageAsync();
				}
			}
		}

		// Token: 0x17000A9D RID: 2717
		// (get) Token: 0x06001EF4 RID: 7924 RVA: 0x0008B107 File Offset: 0x00089307
		// (set) Token: 0x06001EF5 RID: 7925 RVA: 0x0008B119 File Offset: 0x00089319
		public bool UseAntiAlias
		{
			get
			{
				return (bool)base.GetValue(PrintPreviewControl.UseAntiAliasProperty);
			}
			set
			{
				base.SetValue(PrintPreviewControl.UseAntiAliasProperty, value);
			}
		}

		// Token: 0x17000A9E RID: 2718
		// (get) Token: 0x06001EF6 RID: 7926 RVA: 0x0008B12C File Offset: 0x0008932C
		// (set) Token: 0x06001EF7 RID: 7927 RVA: 0x0008B13E File Offset: 0x0008933E
		public ICommand NextPageCommand
		{
			get
			{
				return (ICommand)base.GetValue(PrintPreviewControl.NextPageCommandProperty);
			}
			private set
			{
				base.SetValue(PrintPreviewControl.NextPageCommandPropertyKey, value);
			}
		}

		// Token: 0x17000A9F RID: 2719
		// (get) Token: 0x06001EF8 RID: 7928 RVA: 0x0008B14C File Offset: 0x0008934C
		public static DependencyProperty NextPageCommandProperty
		{
			get
			{
				return PrintPreviewControl.NextPageCommandPropertyKey.DependencyProperty;
			}
		}

		// Token: 0x17000AA0 RID: 2720
		// (get) Token: 0x06001EF9 RID: 7929 RVA: 0x0008B158 File Offset: 0x00089358
		// (set) Token: 0x06001EFA RID: 7930 RVA: 0x0008B16A File Offset: 0x0008936A
		public ICommand PrevPageCommand
		{
			get
			{
				return (ICommand)base.GetValue(PrintPreviewControl.PrevPageCommandProperty);
			}
			private set
			{
				base.SetValue(PrintPreviewControl.PrevPageCommandPropertyKey, value);
			}
		}

		// Token: 0x17000AA1 RID: 2721
		// (get) Token: 0x06001EFB RID: 7931 RVA: 0x0008B178 File Offset: 0x00089378
		public static DependencyProperty PrevPageCommandProperty
		{
			get
			{
				return PrintPreviewControl.PrevPageCommandPropertyKey.DependencyProperty;
			}
		}

		// Token: 0x17000AA2 RID: 2722
		// (get) Token: 0x06001EFC RID: 7932 RVA: 0x0008B184 File Offset: 0x00089384
		// (set) Token: 0x06001EFD RID: 7933 RVA: 0x0008B196 File Offset: 0x00089396
		public bool PrintAnnotations
		{
			get
			{
				return (bool)base.GetValue(PrintPreviewControl.PrintAnnotationsProperty);
			}
			set
			{
				base.SetValue(PrintPreviewControl.PrintAnnotationsProperty, value);
			}
		}

		// Token: 0x06001EFE RID: 7934 RVA: 0x0008B1AC File Offset: 0x000893AC
		private static async void OnPrintAnnotationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			PrintPreviewControl sender = d as PrintPreviewControl;
			if (sender != null && !object.Equals(e.NewValue, e.OldValue))
			{
				LruCache<int, global::System.ValueTuple<BitmapSource, double, double>> lruCache = sender.thumbnailCache;
				lock (lruCache)
				{
					sender.thumbnailCache.Clear();
				}
				await sender.UpdateImageAsync();
				if (sender._FileList.HasItems && sender._FileList.SelectedItem != null)
				{
					BatchPrintItemModel batchPrintItemModel = sender._FileList.SelectedItem as BatchPrintItemModel;
					if (batchPrintItemModel != null)
					{
						batchPrintItemModel._PrintAnnotations = (bool)e.NewValue;
					}
				}
			}
		}

		// Token: 0x17000AA3 RID: 2723
		// (get) Token: 0x06001EFF RID: 7935 RVA: 0x0008B1EB File Offset: 0x000893EB
		// (set) Token: 0x06001F00 RID: 7936 RVA: 0x0008B1FD File Offset: 0x000893FD
		public Visibility FileComboBoxVisibility
		{
			get
			{
				return (Visibility)base.GetValue(PrintPreviewControl.FileComboBoxVisibilityProperty);
			}
			set
			{
				base.SetValue(PrintPreviewControl.FileComboBoxVisibilityProperty, value);
			}
		}

		// Token: 0x06001F01 RID: 7937 RVA: 0x0008B210 File Offset: 0x00089410
		public void SetBatchPrintFileList(ObservableCollection<BatchPrintItemModel> batchPrintItemModels, BatchPrintItemModel SelectedItem = null)
		{
			this._FileList.ItemsSource = batchPrintItemModels;
			if (SelectedItem != null)
			{
				this._FileList.SelectedItem = SelectedItem;
				return;
			}
			this._FileList.SelectedIndex = 0;
		}

		// Token: 0x06001F02 RID: 7938 RVA: 0x0008B23A File Offset: 0x0008943A
		public BatchPrintItemModel GetSelectedFile()
		{
			return this._FileList.SelectedItem as BatchPrintItemModel;
		}

		// Token: 0x06001F03 RID: 7939 RVA: 0x0008B24C File Offset: 0x0008944C
		protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
		{
			base.OnDpiChanged(oldDpi, newDpi);
			LruCache<int, global::System.ValueTuple<BitmapSource, double, double>> lruCache = this.thumbnailCache;
			lock (lruCache)
			{
				this.thumbnailCache.Clear();
			}
			double num;
			double num2;
			BitmapSource imageCore = this.GetImageCore(out num, out num2);
			if (imageCore != null)
			{
				this._Image.Source = imageCore;
				this._Image.Width = num;
				this._Image.Height = num2;
				this._Image.Visibility = Visibility.Visible;
				return;
			}
			this._Image.Source = null;
			this._Image.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06001F04 RID: 7940 RVA: 0x0008B2F4 File Offset: 0x000894F4
		private async Task UpdateImageAsync()
		{
			int curPage = this.CurrentPage;
			BitmapSource bitmapSource = null;
			double num = 0.0;
			double num2 = 0.0;
			if (curPage >= 0 && curPage <= this.TotalPage)
			{
				LruCache<int, global::System.ValueTuple<BitmapSource, double, double>> lruCache = this.thumbnailCache;
				lock (lruCache)
				{
					global::System.ValueTuple<BitmapSource, double, double> valueTuple;
					if (this.thumbnailCache.TryGetValue(curPage, out valueTuple))
					{
						bitmapSource = valueTuple.Item1;
						num = valueTuple.Item2;
						num2 = valueTuple.Item3;
					}
				}
				if (bitmapSource == null)
				{
					try
					{
						await this.ComputePreviewAsync();
					}
					catch
					{
						return;
					}
					bitmapSource = this.GetImageCore(out num, out num2);
					lruCache = this.thumbnailCache;
					lock (lruCache)
					{
						this.thumbnailCache[curPage] = new global::System.ValueTuple<BitmapSource, double, double>(bitmapSource, num, num2);
					}
				}
			}
			if (bitmapSource != null)
			{
				this._Image.Source = bitmapSource;
				this._Image.Width = num;
				this._Image.Height = num2;
				this._Image.Visibility = Visibility.Visible;
			}
			else
			{
				this._Image.Source = null;
				this._Image.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x06001F05 RID: 7941 RVA: 0x0008B338 File Offset: 0x00089538
		private BitmapSource GetImageCore(out double width, out double height)
		{
			width = 0.0;
			height = 0.0;
			if (this.pageInfo != null && this.pageInfo.Length != 0)
			{
				PreviewPageInfo previewPageInfo = this.pageInfo[0];
				IntPtr intPtr = IntPtr.Zero;
				try
				{
					global::System.Drawing.Size physicalSize = previewPageInfo.PhysicalSize;
					if (this._ImageContainer.ActualWidth == 0.0 || this._ImageContainer.ActualHeight == 0.0 || physicalSize.IsEmpty || physicalSize.Width == 0 || physicalSize.Height == 0)
					{
						return null;
					}
					double num = this._ImageContainer.ActualWidth;
					double num2 = num * (double)physicalSize.Height / (double)physicalSize.Width;
					if (num2 > base.ActualHeight)
					{
						num2 = this._ImageContainer.ActualHeight;
						num = num2 * (double)physicalSize.Width / (double)physicalSize.Height;
					}
					width = num;
					height = num2;
					double pixelsPerDip = VisualTreeHelper.GetDpi(this._ImageContainer).PixelsPerDip;
					num *= pixelsPerDip;
					num2 *= pixelsPerDip;
					if (this.PrintArgs.TypesettingModel == PrintTypeSettingModel.Tile)
					{
						double num3 = (double)this.PrintArgs.Document.Pages[this.PrintArgs.PrintPageIndexMapper.GetPageRange()[this.CurrentPage - 1]].Height * 100.0 / 72.0 * 1.0;
						double num4 = (double)this.PrintArgs.Document.Pages[this.PrintArgs.PrintPageIndexMapper.GetPageRange()[this.CurrentPage - 1]].Width * 100.0 / 72.0 * 1.0;
						num *= this.PrintArgs.TilePageZoom / 100.0;
						num2 *= this.PrintArgs.TilePageZoom / 100.0;
						if (num / num2 > num4 / num3)
						{
							num4 = num3 * (num / num2);
						}
						else
						{
							num3 = num4 / (num / num2);
						}
						double tileOverlap = this.PrintArgs.TileOverlap;
						double num5;
						double num6;
						int num7;
						int num8;
						this.GetTileSize(tileOverlap, ref num3, ref num4, this.PrintArgs.TilePageZoom, out num5, out num6, out num7, out num8);
						using (Bitmap bitmap = new Bitmap((int)num6 * 2, (int)num5 * 2))
						{
							using (Graphics graphics = Graphics.FromImage(bitmap))
							{
								graphics.FillRectangle(global::System.Drawing.Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
								double tileOverlap2 = this.PrintArgs.TileOverlap;
								double tileOverlap3 = this.PrintArgs.TileOverlap;
								graphics.DrawImage(previewPageInfo.Image, (float)((double)bitmap.Width - num4 * 2.0) / 2f, (float)((double)bitmap.Height - num3 * 2.0) / 2f, (float)num4 * 2f, (float)num3 * 2f);
								float[] array = new float[] { 4f, 2f };
								using (global::System.Drawing.Pen pen = new global::System.Drawing.Pen(global::System.Drawing.Color.Black, 10f))
								{
									pen.DashPattern = array;
									double num9 = num6 / (double)num8 * 2.0;
									double num10 = num5 / (double)num7 * 2.0;
									if (num8 > 1)
									{
										for (int i = 1; i < num8; i++)
										{
											graphics.DrawLine(pen, (float)num9 * (float)i, 0f, (float)num9 * (float)i, (float)num5 * 2f);
										}
									}
									if (num7 > 1)
									{
										for (int j = 1; j < num7; j++)
										{
											graphics.DrawLine(pen, 0f, (float)num10 * (float)j, (float)num6 * 2f, (float)num10 * (float)j);
										}
									}
								}
							}
							intPtr = bitmap.GetHbitmap();
							return Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, new Int32Rect(0, 0, bitmap.Width, bitmap.Height), BitmapSizeOptions.FromWidthAndHeight((int)Math.Ceiling(num6), (int)Math.Ceiling(num5)));
						}
					}
					using (Bitmap bitmap2 = new Bitmap(physicalSize.Width * 2, physicalSize.Height * 2))
					{
						using (Graphics graphics2 = Graphics.FromImage(bitmap2))
						{
							graphics2.FillRectangle(global::System.Drawing.Brushes.White, 0, 0, bitmap2.Width, bitmap2.Height);
							graphics2.DrawImage(previewPageInfo.Image, 0, 0, bitmap2.Width, bitmap2.Height);
						}
						intPtr = bitmap2.GetHbitmap();
						return Imaging.CreateBitmapSourceFromHBitmap(intPtr, IntPtr.Zero, new Int32Rect(0, 0, bitmap2.Width, bitmap2.Height), BitmapSizeOptions.FromWidthAndHeight((int)Math.Ceiling(num), (int)Math.Ceiling(num2)));
					}
				}
				catch
				{
				}
				finally
				{
					if (intPtr != IntPtr.Zero)
					{
						PrintPreviewControl.DeleteObject(intPtr);
					}
				}
			}
			return null;
		}

		// Token: 0x06001F06 RID: 7942 RVA: 0x0008B8E0 File Offset: 0x00089AE0
		private void GetTileSize(double TileOverlap, ref double imageHeight, ref double imageWidth, double pageZoom, out double TotalHeight, out double TotalWidth, out int ceilingHeightResult, out int ceilingWidthResult)
		{
			if (this.PrintArgs.TileCutMasks || this.PrintArgs.TileLabels)
			{
				imageHeight = imageHeight * pageZoom / 100.0 - 40.0;
				imageWidth = imageWidth * pageZoom / 100.0 - 40.0;
			}
			else
			{
				imageHeight = imageHeight * pageZoom / 100.0;
				imageWidth = imageWidth * pageZoom / 100.0;
			}
			global::System.Windows.Size printableArea = this.GetPrintableArea(this.PrintArgs.PrinterName);
			double num = (double)this.PrintArgs.PaperSize.Height - printableArea.Height * 2.0;
			double num2 = (double)this.PrintArgs.PaperSize.Width - printableArea.Width * 2.0;
			if (this.PrintArgs.Landscape)
			{
				num = (double)this.PrintArgs.PaperSize.Width - printableArea.Width * 2.0;
				num2 = (double)this.PrintArgs.PaperSize.Height - printableArea.Height * 2.0;
			}
			if (this.PrintArgs.TileCutMasks || this.PrintArgs.TileLabels)
			{
				num -= 80.0;
				num2 -= 80.0;
			}
			ceilingHeightResult = (int)Math.Ceiling(imageHeight / num);
			ceilingWidthResult = (int)Math.Ceiling(imageWidth / num2);
			TotalHeight = (double)ceilingHeightResult * num;
			TotalWidth = (double)ceilingWidthResult * num2;
			if ((double)(ceilingHeightResult - 1) * TileOverlap * 0.39370078740157 * 100.0 + imageHeight > TotalHeight)
			{
				ceilingHeightResult++;
				TotalHeight = (double)ceilingHeightResult * num;
			}
			if ((double)(ceilingWidthResult - 1) * TileOverlap * 0.39370078740157 * 100.0 + imageWidth > TotalWidth)
			{
				ceilingWidthResult++;
				TotalWidth = (double)ceilingWidthResult * num2;
			}
		}

		// Token: 0x06001F07 RID: 7943 RVA: 0x0008BAD8 File Offset: 0x00089CD8
		public global::System.Windows.Size GetPrintableArea(string printerName)
		{
			global::System.Windows.Size size;
			using (LocalPrintServer localPrintServer = new LocalPrintServer())
			{
				using (PrintQueue printQueue = localPrintServer.GetPrintQueue(printerName))
				{
					PrintCapabilities printCapabilities = printQueue.GetPrintCapabilities();
					if (((printCapabilities != null) ? printCapabilities.PageImageableArea : null) == null)
					{
						size = new global::System.Windows.Size(0.0, 0.0);
					}
					else
					{
						size = new global::System.Windows.Size(printCapabilities.PageImageableArea.OriginWidth, printCapabilities.PageImageableArea.OriginHeight);
					}
				}
			}
			return size;
		}

		// Token: 0x06001F08 RID: 7944 RVA: 0x0008BB74 File Offset: 0x00089D74
		private async Task ComputePreviewAsync()
		{
			PrintPreviewControl.<>c__DisplayClass57_0 CS$<>8__locals1 = new PrintPreviewControl.<>c__DisplayClass57_0();
			CancellationTokenSource cancellationTokenSource = this.cts;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			CS$<>8__locals1.cts = new CancellationTokenSource();
			this.cts = CS$<>8__locals1.cts;
			PreviewPageInfo[] old = this.pageInfo;
			PrintArgs args = this.PrintArgs;
			this._Image.Visibility = Visibility.Collapsed;
			this._LoadingProgressRing.Visibility = Visibility.Visible;
			PrintPreviewControl.<>c__DisplayClass57_1 CS$<>8__locals2 = new PrintPreviewControl.<>c__DisplayClass57_1();
			CS$<>8__locals2.document = this.CreatePrintDocument(args);
			try
			{
				if (CS$<>8__locals2.document == null)
				{
					this.pageInfo = new PreviewPageInfo[0];
				}
				else
				{
					try
					{
						this.PrintArgsAddRef(args);
						PrintController printController = CS$<>8__locals2.document.PrintController;
						PreviewPrintController previewPrintController = new PreviewPrintController();
						previewPrintController.UseAntiAlias = this.UseAntiAlias;
						CS$<>8__locals2.document.PrintController = previewPrintController;
						try
						{
							this.printing = true;
							CS$<>8__locals2.document.BeginPrint += CS$<>8__locals1.<ComputePreviewAsync>g__Document_BeginPrint|0;
							CS$<>8__locals2.document.PrintPage += CS$<>8__locals1.<ComputePreviewAsync>g__Document_PrintPage|1;
							await Task.Run(delegate
							{
								try
								{
									CS$<>8__locals2.document.Print();
								}
								catch
								{
								}
							}, CS$<>8__locals1.cts.Token);
							this.pageInfo = previewPrintController.GetPreviewPageInfo();
							CS$<>8__locals2.document.PrintController = printController;
						}
						finally
						{
							this.printing = false;
							CS$<>8__locals2.document.BeginPrint -= CS$<>8__locals1.<ComputePreviewAsync>g__Document_BeginPrint|0;
							CS$<>8__locals2.document.PrintPage -= CS$<>8__locals1.<ComputePreviewAsync>g__Document_PrintPage|1;
							this.PrintArgsReleaseRef(args);
						}
						printController = null;
						previewPrintController = null;
					}
					catch
					{
						this.pageInfo = new PreviewPageInfo[0];
					}
				}
			}
			finally
			{
				if (CS$<>8__locals2.document != null)
				{
					((IDisposable)CS$<>8__locals2.document).Dispose();
				}
			}
			CS$<>8__locals2 = null;
			if (old != null)
			{
				PreviewPageInfo[] array = old;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Image.Dispose();
				}
			}
			CS$<>8__locals1.cts.Token.ThrowIfCancellationRequested();
			this._LoadingProgressRing.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06001F09 RID: 7945 RVA: 0x0008BBB8 File Offset: 0x00089DB8
		private PrintDocument CreatePrintDocument(PrintArgs args)
		{
			if (this.CurrentPage < 0 || this.TotalPage <= 0 || this.CurrentPage > this.TotalPage)
			{
				return null;
			}
			if (args == null || args.Document == null)
			{
				return null;
			}
			PdfPrintDocument pdfPrintDocument = new PdfPrintDocument(args.Document, args.PrintPageIndexMapper, this.PrintAnnotations);
			pdfPrintDocument.RenderFlags = RenderFlags.FPDF_ANNOT | RenderFlags.FPDF_LCD_TEXT;
			if (args.Grayscale)
			{
				pdfPrintDocument.RenderFlags |= RenderFlags.FPDF_GRAYSCALE;
			}
			pdfPrintDocument.PrinterSettings.PrinterName = args.PrinterName;
			PageSettings pageSettings = new PageSettings(pdfPrintDocument.PrinterSettings);
			pdfPrintDocument.DefaultPageSettings = pageSettings;
			pdfPrintDocument.PrinterSettings.SetHdevmode(args.PrintDevMode);
			pdfPrintDocument.AutoRotate = args.AutoRotate;
			pdfPrintDocument.AutoCenter = args.AutoCenter;
			pdfPrintDocument.PrintSizeMode = args.PrintSizeMode;
			pdfPrintDocument.Scale = args.Scale;
			pdfPrintDocument.PrinterSettings.PrintRange = PrintRange.SomePages;
			if (args.TypesettingModel == PrintTypeSettingModel.Multiple)
			{
				pdfPrintDocument.PrinterSettings.FromPage = 1 + (this.CurrentPage - 1) * args.PapersPerSheet;
				pdfPrintDocument.PrinterSettings.ToPage = 1 + (this.CurrentPage - 1) * args.PapersPerSheet + args.PapersPerSheet;
				if (pdfPrintDocument.PrinterSettings.ToPage > args.Document.Pages.Count)
				{
					pdfPrintDocument.PrinterSettings.ToPage = args.Document.Pages.Count;
				}
			}
			else if (args.TypesettingModel == PrintTypeSettingModel.Booklet)
			{
				if (args.bookletSubset == BookletSubset.BothSide)
				{
					pdfPrintDocument.PrinterSettings.FromPage = 1 + (this.CurrentPage - 1) * 2;
					args.Duplex = Duplex.Horizontal;
				}
				else
				{
					pdfPrintDocument.PrinterSettings.FromPage = 1 + (this.CurrentPage - 1) * 2;
					args.Duplex = Duplex.Simplex;
				}
				pdfPrintDocument.PrinterSettings.ToPage = args.Document.Pages.Count<PdfPage>();
			}
			else
			{
				pdfPrintDocument.PrinterSettings.FromPage = this.CurrentPage;
				pdfPrintDocument.PrinterSettings.ToPage = this.CurrentPage;
			}
			pdfPrintDocument.isPreviewControl = true;
			pdfPrintDocument.PrintTypeSettingModel = args.TypesettingModel;
			pdfPrintDocument.PapersPerSheet = args.PapersPerSheet;
			pdfPrintDocument.PaperRowNum = args.PaperRowNum;
			pdfPrintDocument.PaperColumnNum = args.PaperColumnNum;
			pdfPrintDocument.PageOrder = args.PageOrder;
			pdfPrintDocument.BookletBindingDirection = args.BindingDirection;
			pdfPrintDocument.BookletSubset = args.bookletSubset;
			pdfPrintDocument.TilePageZoom = args.TilePageZoom;
			pdfPrintDocument.PrintBorder = args.PrintBorder;
			pdfPrintDocument.PageMargins = args.PageMargins;
			pdfPrintDocument.PrintReverse = args.PrintReverse;
			pageSettings.PaperSize = args.PaperSize;
			pageSettings.Landscape = args.TypesettingModel != PrintTypeSettingModel.Tile && args.Landscape;
			pageSettings.Color = !args.Grayscale;
			this.PrintMarginX = (double)pdfPrintDocument.DefaultPageSettings.PrintableArea.X;
			this.PrintMarginY = (double)pdfPrintDocument.DefaultPageSettings.PrintableArea.Y;
			return pdfPrintDocument;
		}

		// Token: 0x06001F0A RID: 7946 RVA: 0x0008BEA4 File Offset: 0x0008A0A4
		private void PrintPreviewControl_Unloaded(object sender, RoutedEventArgs e)
		{
			this.thumbnailCache.Clear();
			this._Image.Source = null;
			PreviewPageInfo[] array = this.pageInfo;
			this.pageInfo = null;
			if (array != null)
			{
				PreviewPageInfo[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].Image.Dispose();
				}
			}
		}

		// Token: 0x06001F0B RID: 7947 RVA: 0x0008BEF8 File Offset: 0x0008A0F8
		private void PrintArgsAddRef(PrintArgs args)
		{
			if (args == null)
			{
				return;
			}
			ConcurrentDictionary<PrintArgs, int> concurrentDictionary = this.printingArgs;
			lock (concurrentDictionary)
			{
				int num;
				if (this.printingArgs.TryGetValue(args, out num))
				{
					this.printingArgs[args] = num + 1;
				}
				else
				{
					this.printingArgs[args] = 1;
				}
			}
		}

		// Token: 0x06001F0C RID: 7948 RVA: 0x0008BF64 File Offset: 0x0008A164
		private void PrintArgsReleaseRef(PrintArgs args)
		{
			if (args == null)
			{
				return;
			}
			ConcurrentDictionary<PrintArgs, int> concurrentDictionary = this.printingArgs;
			lock (concurrentDictionary)
			{
				int num;
				if (this.printingArgs.TryGetValue(args, out num))
				{
					num--;
					if (num == 0)
					{
						int num2;
						this.printingArgs.TryRemove(args, out num2);
					}
					else
					{
						this.printingArgs[args] = num;
					}
				}
			}
		}

		// Token: 0x06001F0D RID: 7949
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool DeleteObject(IntPtr hObject);

		// Token: 0x06001F0E RID: 7950 RVA: 0x0008BFD8 File Offset: 0x0008A1D8
		private async void _PrintAnnotCheckBox_Click(object sender, RoutedEventArgs e)
		{
			await Task.Yield();
			string text = ((sender as CheckBox).IsChecked.GetValueOrDefault() ? "Checked" : "Unchecked");
			GAManager.SendEvent("PrintAnnotations", text, "Count", 1L);
		}

		// Token: 0x1400003C RID: 60
		// (add) Token: 0x06001F0F RID: 7951 RVA: 0x0008C010 File Offset: 0x0008A210
		// (remove) Token: 0x06001F10 RID: 7952 RVA: 0x0008C048 File Offset: 0x0008A248
		public event EventHandler<SelectionChangedEventArgs> FileListSelectionChanged;

		// Token: 0x06001F11 RID: 7953 RVA: 0x0008C07D File Offset: 0x0008A27D
		private void _FileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			EventHandler<SelectionChangedEventArgs> fileListSelectionChanged = this.FileListSelectionChanged;
			if (fileListSelectionChanged == null)
			{
				return;
			}
			fileListSelectionChanged(this, e);
		}

		// Token: 0x04000C23 RID: 3107
		private PreviewPageInfo[] pageInfo;

		// Token: 0x04000C24 RID: 3108
		private CancellationTokenSource cts;

		// Token: 0x04000C25 RID: 3109
		private bool innerSet;

		// Token: 0x04000C26 RID: 3110
		[global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "bitmap", "width", "height" })]
		private LruCache<int, global::System.ValueTuple<BitmapSource, double, double>> thumbnailCache;

		// Token: 0x04000C27 RID: 3111
		private ConcurrentDictionary<PrintArgs, int> printingArgs;

		// Token: 0x04000C28 RID: 3112
		private bool printing;

		// Token: 0x04000C29 RID: 3113
		private double PrintMarginX;

		// Token: 0x04000C2A RID: 3114
		private double PrintMarginY;

		// Token: 0x04000C2B RID: 3115
		private static readonly DependencyPropertyKey TotalPagePropertyKey = DependencyProperty.RegisterReadOnly("TotalPage", typeof(int), typeof(PrintPreviewControl), new PropertyMetadata(0));

		// Token: 0x04000C2C RID: 3116
		public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(int), typeof(PrintPreviewControl), new PropertyMetadata(-1, new PropertyChangedCallback(PrintPreviewControl.OnCurrentPageChanged)));

		// Token: 0x04000C2D RID: 3117
		public static readonly DependencyProperty PrintArgsProperty = DependencyProperty.Register("PrintArgs", typeof(PrintArgs), typeof(PrintPreviewControl), new PropertyMetadata(null, new PropertyChangedCallback(PrintPreviewControl.OnPrintArgsChanged)));

		// Token: 0x04000C2E RID: 3118
		public static readonly DependencyProperty UseAntiAliasProperty = DependencyProperty.Register("UseAntiAlias", typeof(bool), typeof(PrintPreviewControl), new PropertyMetadata(true));

		// Token: 0x04000C2F RID: 3119
		public static readonly DependencyPropertyKey NextPageCommandPropertyKey = DependencyProperty.RegisterReadOnly("NextPageCommand", typeof(ICommand), typeof(PrintPreviewControl), new PropertyMetadata(null));

		// Token: 0x04000C30 RID: 3120
		public static readonly DependencyPropertyKey PrevPageCommandPropertyKey = DependencyProperty.RegisterReadOnly("PrevPageCommand", typeof(ICommand), typeof(PrintPreviewControl), new PropertyMetadata(null));

		// Token: 0x04000C31 RID: 3121
		public static readonly DependencyProperty PrintAnnotationsProperty = DependencyProperty.Register("PrintAnnotations", typeof(bool), typeof(PrintPreviewControl), new PropertyMetadata(true, new PropertyChangedCallback(PrintPreviewControl.OnPrintAnnotationPropertyChanged)));

		// Token: 0x04000C32 RID: 3122
		public static readonly DependencyProperty FileComboBoxVisibilityProperty = DependencyProperty.Register("FileComboBoxVisibility", typeof(Visibility), typeof(PrintPreviewControl), new PropertyMetadata(Visibility.Collapsed, null));
	}
}
