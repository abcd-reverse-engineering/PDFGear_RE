using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using CommonLib.Common;
using CommonLib.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.AutoSaveRestore;
using pdfeditor.Models.Scan;
using pdfeditor.Properties;
using pdfeditor.Services;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x0200024C RID: 588
	public partial class InsertPageFromScanner : Window
	{
		// Token: 0x17000B1E RID: 2846
		// (get) Token: 0x060021D0 RID: 8656 RVA: 0x0009C113 File Offset: 0x0009A313
		// (set) Token: 0x060021D1 RID: 8657 RVA: 0x0009C11B File Offset: 0x0009A31B
		public int InsertPageIndex { get; private set; }

		// Token: 0x17000B1F RID: 2847
		// (get) Token: 0x060021D2 RID: 8658 RVA: 0x0009C124 File Offset: 0x0009A324
		// (set) Token: 0x060021D3 RID: 8659 RVA: 0x0009C12C File Offset: 0x0009A32C
		public bool InsertBefore { get; private set; }

		// Token: 0x060021D4 RID: 8660 RVA: 0x0009C138 File Offset: 0x0009A338
		public InsertPageFromScanner(IEnumerable<int> selectedPages, PdfDocument doc, bool fromSinglePageCmd = false, bool creatNew = false, MainViewModel mainViewModel = null)
		{
			this.InitializeComponent();
			this.insertFromScannerViewModel = new InsertFromScannerViewModel();
			base.DataContext = this.insertFromScannerViewModel;
			this.CreateNew = creatNew;
			this.FromSinglePageCmd = fromSinglePageCmd;
			base.Loaded += this.InsertPageFromScanner_Loaded;
			this.doc = doc;
			if (creatNew)
			{
				base.Title = pdfeditor.Properties.Resources.MainWindowNewFilefromScannerBtn;
				this.OkBtn.Content = pdfeditor.Properties.Resources.WinScreenshotToolbarDoneContent;
				this.OkBtn2.Content = pdfeditor.Properties.Resources.WinScreenshotToolbarDoneContent;
			}
			else
			{
				this.SelectedPages = selectedPages;
				List<int> list = selectedPages.ToList<int>();
				list.Sort();
				if (list.Count > 0)
				{
					this.selectedFirstIndex = list[0];
				}
				else
				{
					this.selectedFirstIndex = 0;
				}
			}
			GAManager.SendEvent("ScannerWindow", "Show", this.CreateNew ? "NewPDF" : "InsertPages", 1L);
			base.Closing += this.InsertPageFromScanner_Closing;
			if (mainViewModel != null)
			{
				this.mainViewModel = mainViewModel;
				mainViewModel.ExitTransientMode(false, false, false, false, false);
			}
		}

		// Token: 0x060021D5 RID: 8661 RVA: 0x0009C2A2 File Offset: 0x0009A4A2
		private void InsertPageFromScanner_Loaded(object sender, RoutedEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(delegate
			{
				this.insertFromScannerViewModel.Loaded();
			});
			base.Focus();
		}

		// Token: 0x060021D6 RID: 8662 RVA: 0x0009C2C8 File Offset: 0x0009A4C8
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (e.Key == Key.Delete && Keyboard.Modifiers == ModifierKeys.None && this.insertFromScannerViewModel.SelectedItems.Count > 0)
			{
				this.insertFromScannerViewModel.DeleteCommand.Execute(null);
			}
			if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.A)
			{
				e.Handled = true;
				this.insertFromScannerViewModel.IsCheckedAll = new bool?(true);
			}
		}

		// Token: 0x060021D7 RID: 8663 RVA: 0x0009C33C File Offset: 0x0009A53C
		private void InsertPageFromScanner_Closing(object sender, CancelEventArgs e)
		{
			if (this.insertFromScannerViewModel.PageList.Count > 0 && ModernMessageBox.Show(pdfeditor.Properties.Resources.ScannerWinClosingWarning, UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxResult.None, null, false) == MessageBoxResult.Cancel)
			{
				e.Cancel = true;
				return;
			}
			this.insertFromScannerViewModel.cts.Cancel();
			this.insertFromScannerViewModel.Dispose();
		}

		// Token: 0x060021D8 RID: 8664 RVA: 0x0009C395 File Offset: 0x0009A595
		private static int GetMMValueFromPix(float pix)
		{
			return (int)Math.Round((double)(pix * 2540f / 7200f), 0);
		}

		// Token: 0x060021D9 RID: 8665 RVA: 0x0009C3AC File Offset: 0x0009A5AC
		private async void OKButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (this.insertFromScannerViewModel.Visibility == Visibility.Visible)
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.ScanningWarning, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				}
				else
				{
					GAManager.SendEvent("ScannerWindow", "OkBtn", this.CreateNew ? "NewPDF" : "InsertPages", 1L);
					GAManager.SendEvent("ScannerWindow", "ExportedPagesCount", this.insertFromScannerViewModel.PageCount.ToString(), 1L);
					if (this.insertFromScannerViewModel.SelectedCount == this.insertFromScannerViewModel.PageCount || this.insertFromScannerViewModel.SelectedCount == 0 || ModernMessageBox.Show(pdfeditor.Properties.Resources.ScannerWinInsertConfirm.Replace("XXX", this.insertFromScannerViewModel.SelectedCount.ToString()), UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxResult.None, null, false) != MessageBoxResult.Cancel)
					{
						InsertPageProperty insertPageProperty = new InsertPageProperty(this.SelectedPages, this.doc, this.FromSinglePageCmd, this.CreateNew);
						insertPageProperty.Owner = this;
						insertPageProperty.WindowStartupLocation = WindowStartupLocation.CenterOwner;
						insertPageProperty.ShowDialog();
						if (insertPageProperty.DialogResult.GetValueOrDefault())
						{
							SizeF pagesize = this.GetPagesize(insertPageProperty);
							if (this.CreateNew)
							{
								await this.CreateFormScanner(pagesize);
								base.Closing -= this.InsertPageFromScanner_Closing;
								base.Close();
							}
							else
							{
								if (insertPageProperty.BeginingRadioButton.IsChecked.GetValueOrDefault())
								{
									this.InsertPageIndex = 0;
									this.InsertBefore = true;
								}
								else if (insertPageProperty.EndRadioButton.IsChecked.GetValueOrDefault())
								{
									this.InsertPageIndex = this.doc.Pages.Count - 1;
									this.InsertBefore = false;
								}
								if (insertPageProperty.PageRadioButton.IsChecked.GetValueOrDefault())
								{
									if (insertPageProperty.InsertPosition.SelectedIndex == 0)
									{
										this.InsertBefore = false;
									}
									else
									{
										this.InsertBefore = true;
									}
									this.InsertPageIndex = insertPageProperty.InsertPageIndex;
								}
								await this.InsertFormScanner(pagesize);
								base.Closing -= this.InsertPageFromScanner_Closing;
								base.Close();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		// Token: 0x060021DA RID: 8666 RVA: 0x0009C3E4 File Offset: 0x0009A5E4
		private async Task CreateFormScanner(SizeF size)
		{
			try
			{
				GAManager.SendEvent("ScannerWindow", "CreateFormScanner", "Begin", 1L);
				int num = 0;
				using (PdfDocument newdoc = PdfDocument.CreateNew(null))
				{
					List<ScannedPage> orderedPagesForExport = this.GetOrderedPagesForExport();
					int count = orderedPagesForExport.Count;
					newdoc.Producer = "PDF gear";
					if (orderedPagesForExport != null && orderedPagesForExport.Count > 0)
					{
						foreach (ScannedPage scannedPage in orderedPagesForExport)
						{
							Bitmap bitmap = scannedPage.ImageBitmap ?? null;
							if (bitmap != null)
							{
								if (scannedPage.Rotate == 90)
								{
									bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
								}
								else if (scannedPage.Rotate == 180)
								{
									bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
								}
								else if (scannedPage.Rotate == 270)
								{
									bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
								}
								SizeF sizeF = size;
								if (size.Width == 0f && size.Height == 0f)
								{
									sizeF = new SizeF((float)bitmap.Width, (float)bitmap.Height);
								}
								PdfPage pdfPage = newdoc.Pages.InsertPageAt(num, sizeF.Width, sizeF.Height);
								num++;
								float num2 = sizeF.Width - (float)(this.marginWidth * 2);
								float num3 = sizeF.Height - (float)(this.marginHeight * 2);
								float num4 = (float)(bitmap.Height * 72) / bitmap.VerticalResolution;
								float num5 = (float)(bitmap.Width * 72) / bitmap.HorizontalResolution;
								float num6 = Math.Min(num2 / num5, num3 / num4);
								if (num6 < 1f)
								{
									num4 *= num6;
									num5 *= num6;
								}
								float num7 = (num2 - num5) / 2f + (float)this.marginWidth;
								float num8 = (num3 - num4) / 2f + (float)this.marginHeight;
								num7 = ((num7 < 0f) ? 0f : num7);
								num8 = ((num8 < 0f) ? 0f : num8);
								PdfBitmap pdfBitmap = ImageToPDFBitmapHelper.CreatePdfBitmapFromFile(bitmap);
								if (pdfBitmap == null)
								{
									DrawUtils.ShowUnsupportedImageMessage();
								}
								else
								{
									try
									{
										PdfImageObject pdfImageObject = PdfImageObject.Create(newdoc, pdfBitmap, 0f, 0f);
										pdfPage.PageObjects.Add(pdfImageObject);
										pdfImageObject.Matrix = new FS_MATRIX(num5, 0f, 0f, num4, num7, num8);
										pdfPage.GenerateContent(false);
									}
									catch
									{
										DrawUtils.ShowUnsupportedImageMessage();
									}
									finally
									{
										if (pdfBitmap != null)
										{
											pdfBitmap.Dispose();
										}
									}
								}
							}
						}
					}
					string text = Path.Combine(UtilManager.GetTemporaryPath(), "Documents");
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					int num9 = 0;
					string text2 = "";
					do
					{
						num9++;
						string text3 = string.Format("{0}{1}", pdfeditor.Properties.Resources.NewFileName, num9);
						text3 += ".pdf";
						text2 = Path.Combine(text, text3);
					}
					while (File.Exists(text2));
					if (Directory.Exists(text))
					{
						try
						{
							using (FileStream fileStream = File.OpenWrite(text2))
							{
								fileStream.Seek(0L, SeekOrigin.Begin);
								newdoc.Save(fileStream, SaveFlags.NoIncremental, 0);
								fileStream.SetLength(fileStream.Position);
							}
							if (this.doc != null)
							{
								CreateFileHelper.OpenPDFFile(text2, "open:CreatedFile");
							}
							else
							{
								bool flag = await this.mainViewModel.OpenDocumentCoreAsync(text2, null, true);
								if (flag && flag)
								{
									this.mainViewModel.DocumentWrapper.SetUntitledFile();
									this.mainViewModel.SetCanSaveFlag("CreateNew", false);
									pdfeditor.AutoSaveRestore.AutoSaveManager.GetInstance().LastOperationVersion = "CreateNew";
								}
							}
							GAManager.SendEvent("ScannerWindow", "CreateFormScanner", "Success", 1L);
						}
						catch
						{
						}
					}
				}
				PdfDocument newdoc = null;
			}
			catch
			{
			}
		}

		// Token: 0x060021DB RID: 8667 RVA: 0x0009C430 File Offset: 0x0009A630
		private static void OpenPDFFile(string file, string action = null)
		{
			char[] array = new char[] { '\\', '/', ' ' };
			string text = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory.TrimEnd(array)).FullName, "pdfeditor", "pdfeditor.exe");
			string text2 = "\"" + file + "\"";
			if (!string.IsNullOrEmpty(action))
			{
				text2 = text2 + " -action " + action.Trim();
			}
			ProcessManager.RunProcess(text, text2);
		}

		// Token: 0x060021DC RID: 8668 RVA: 0x0009C4AC File Offset: 0x0009A6AC
		private async Task InsertFormScanner(SizeF size)
		{
			try
			{
				GAManager.SendEvent("ScannerWindow", "InsertFormScanner", "Begin", 1L);
				int insertIndex = (this.InsertBefore ? this.InsertPageIndex : (this.InsertPageIndex + 1));
				int insertIndex5 = insertIndex;
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.doc);
				MainViewModel mainViewModel = ((pdfControl != null) ? pdfControl.DataContext : null) as MainViewModel;
				List<ScannedPage> list = this.GetOrderedPagesForExport();
				int listCount = list.Count;
				if (list != null && list.Count > 0)
				{
					foreach (ScannedPage scannedPage in list)
					{
						Bitmap bitmap = scannedPage.ImageBitmap ?? null;
						if (bitmap != null)
						{
							if (scannedPage.Rotate == 90)
							{
								bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
							}
							else if (scannedPage.Rotate == 180)
							{
								bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
							}
							else if (scannedPage.Rotate == 270)
							{
								bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
							}
							SizeF size2 = size;
							if (size.Width == 0f && size.Height == 0f)
							{
								size2 = new SizeF((float)bitmap.Width, (float)bitmap.Height);
							}
							PdfPage pdfPage = mainViewModel.Document.Pages.InsertPageAt(insertIndex, size2.Width, size2.Height);
							int insertIndex2 = insertIndex;
							insertIndex = insertIndex2 + 1;
							float num = size2.Width - (float)(this.marginWidth * 2);
							float num2 = size2.Height - (float)(this.marginHeight * 2);
							float num3 = (float)(bitmap.Height * 72) / bitmap.VerticalResolution;
							float num4 = (float)(bitmap.Width * 72) / bitmap.HorizontalResolution;
							float num5 = Math.Min(num / num4, num2 / num3);
							if (num5 < 1f)
							{
								num3 *= num5;
								num4 *= num5;
							}
							float num6 = (num - num4) / 2f + (float)this.marginWidth;
							float num7 = (num2 - num3) / 2f + (float)this.marginHeight;
							num6 = ((num6 < 0f) ? 0f : num6);
							num7 = ((num7 < 0f) ? 0f : num7);
							PdfBitmap pdfBitmap = ImageToPDFBitmapHelper.CreatePdfBitmapFromFile(bitmap);
							if (pdfBitmap == null)
							{
								DrawUtils.ShowUnsupportedImageMessage();
							}
							else
							{
								try
								{
									PdfImageObject pdfImageObject = PdfImageObject.Create(mainViewModel.Document, pdfBitmap, 0f, 0f);
									pdfPage.PageObjects.Add(pdfImageObject);
									pdfImageObject.Matrix = new FS_MATRIX(num4, 0f, 0f, num3, num6, num7);
									pdfPage.GenerateContent(false);
								}
								catch
								{
									DrawUtils.ShowUnsupportedImageMessage();
								}
								finally
								{
									if (pdfBitmap != null)
									{
										pdfBitmap.Dispose();
									}
								}
							}
						}
					}
				}
				int num8 = (this.InsertBefore ? this.InsertPageIndex : (this.InsertPageIndex + 1));
				mainViewModel.LastViewPage = mainViewModel.Document.Pages[num8];
				mainViewModel.UpdateDocumentCore();
				mainViewModel.PageEditors.FlushViewerAndThumbnail(false);
				StrongReferenceMessenger.Default.Send<ValueChangedMessage<global::System.ValueTuple<int, int>>, string>(new ValueChangedMessage<global::System.ValueTuple<int, int>>(new global::System.ValueTuple<int, int>(insertIndex5, insertIndex - 1)), "MESSAGE_PAGE_EDITOR_SELECT_INDEX");
				GAManager.SendEvent("ScannerWindow", "InsertFormScanner", "Suceess", 1L);
				await mainViewModel.OperationManager.AddOperationAsync(delegate(PdfDocument Document)
				{
					global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl(Document);
					MainViewModel mainViewModel2 = ((pdfControl2 != null) ? pdfControl2.DataContext : null) as MainViewModel;
					for (int i = 0; i < listCount; i++)
					{
						int insertIndex3 = insertIndex;
						insertIndex = insertIndex3 - 1;
						Document.Pages.DeleteAt(insertIndex);
					}
					if (mainViewModel2 != null)
					{
						mainViewModel2.UpdateDocumentCore();
					}
				}, delegate(PdfDocument Document)
				{
					foreach (ScannedPage scannedPage2 in list)
					{
						PdfPage pdfPage2 = Document.Pages.InsertPageAt(insertIndex, size.Width, size.Height);
						int insertIndex4 = insertIndex;
						insertIndex = insertIndex4 + 1;
						Bitmap imageBitmap = scannedPage2.ImageBitmap;
						float num9 = (float)(imageBitmap.Height * 72) / imageBitmap.VerticalResolution;
						float num10 = (float)(imageBitmap.Width * 72) / imageBitmap.HorizontalResolution;
						float num11 = size.Width - (float)(this.marginWidth * 2);
						float num12 = size.Height - (float)(this.marginHeight * 2);
						float num13 = Math.Min(num11 / num10, num12 / num9);
						if (num13 < 1f)
						{
							num9 *= num13;
							num10 *= num13;
						}
						float num14 = (num11 - num10) / 2f + (float)this.marginWidth;
						float num15 = (num12 - num9) / 2f + (float)this.marginHeight;
						num14 = ((num14 < 0f) ? 0f : num14);
						num15 = ((num15 < 0f) ? 0f : num15);
						PdfBitmap pdfBitmap2 = ImageToPDFBitmapHelper.CreatePdfBitmapFromFile(imageBitmap);
						if (pdfBitmap2 == null)
						{
							DrawUtils.ShowUnsupportedImageMessage();
						}
						else
						{
							try
							{
								PdfImageObject pdfImageObject2 = PdfImageObject.Create(Document, pdfBitmap2, 0f, 0f);
								pdfPage2.PageObjects.Add(pdfImageObject2);
								pdfImageObject2.Matrix = new FS_MATRIX(num10, 0f, 0f, num9, num14, num15);
								pdfPage2.GenerateContent(false);
							}
							catch
							{
							}
							finally
							{
								if (pdfBitmap2 != null)
								{
									pdfBitmap2.Dispose();
								}
							}
						}
					}
					global::PDFKit.PdfControl pdfControl3 = global::PDFKit.PdfControl.GetPdfControl(Document);
					MainViewModel mainViewModel3 = ((pdfControl3 != null) ? pdfControl3.DataContext : null) as MainViewModel;
					if (mainViewModel3 != null)
					{
						mainViewModel3.UpdateDocumentCore();
					}
					if (mainViewModel3 == null)
					{
						return;
					}
					PageEditorViewModel pageEditors = mainViewModel3.PageEditors;
					if (pageEditors == null)
					{
						return;
					}
					pageEditors.FlushViewerAndThumbnail(false);
				}, "");
			}
			catch
			{
				DrawUtils.ShowUnsupportedImageMessage();
			}
		}

		// Token: 0x060021DD RID: 8669 RVA: 0x0009C4F8 File Offset: 0x0009A6F8
		public void FlushViewerAndThumbnail(bool forceRedraw = false)
		{
			Ioc.Default.GetRequiredService<PdfThumbnailService>().RefreshAllThumbnail();
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(this.doc);
			global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl((((pdfControl != null) ? pdfControl.DataContext : null) as MainViewModel).Document);
			if (pdfControl2 != null)
			{
				pdfControl2.Redraw(forceRedraw);
			}
			AnnotationCanvas annotationCanvas = PdfObjectExtensions.GetAnnotationCanvas(pdfControl2);
			if (annotationCanvas != null && annotationCanvas.ImageControl.Visibility == Visibility.Visible)
			{
				annotationCanvas.ImageControl.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x060021DE RID: 8670 RVA: 0x0009C568 File Offset: 0x0009A768
		private Bitmap GetBitmap(BitmapFrame bitmapFrame)
		{
			Bitmap bitmap2;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new PngBitmapEncoder
				{
					Frames = { bitmapFrame }
				}.Save(memoryStream);
				BitmapFrame bitmapFrame2 = BitmapDecoder.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.Default).Frames[0];
				Bitmap bitmap;
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					new BmpBitmapEncoder
					{
						Frames = { bitmapFrame2 }
					}.Save(memoryStream2);
					bitmap = new Bitmap(memoryStream2);
				}
				bitmap2 = bitmap;
			}
			return bitmap2;
		}

		// Token: 0x060021DF RID: 8671 RVA: 0x0009C604 File Offset: 0x0009A804
		private List<ScannedPage> GetOrderedPagesForExport()
		{
			InsertFromScannerViewModel insertFromScannerViewModel = base.DataContext as InsertFromScannerViewModel;
			List<ScannedPage> list = insertFromScannerViewModel.PageList.ToList<ScannedPage>();
			if (insertFromScannerViewModel.IsExportAll || insertFromScannerViewModel.PageCount == insertFromScannerViewModel.SelectedCount || insertFromScannerViewModel.SelectedCount == 0)
			{
				return list;
			}
			List<ScannedPage> list2 = new List<ScannedPage>();
			foreach (ScannedPage scannedPage in list)
			{
				if (insertFromScannerViewModel.SelectedItems.Contains(scannedPage))
				{
					list2.Add(scannedPage);
				}
			}
			return list2;
		}

		// Token: 0x060021E0 RID: 8672 RVA: 0x0009C6A4 File Offset: 0x0009A8A4
		private SizeF GetPagesize(InsertPageProperty insertPageProperty)
		{
			float num = 0f;
			float num2 = 0f;
			if (insertPageProperty.cbPageSize.SelectedIndex == 3 && !this.CreateNew)
			{
				return new SizeF(num, num2);
			}
			if (insertPageProperty.cbPageSize.SelectedIndex == 0)
			{
				bool? flag = insertPageProperty.PortraitRadioButton.IsChecked;
				bool flag2 = false;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					num = InsertPageFromScanner.GetPixValueFromMM((int)insertPageProperty.DefaultSize.Height);
					num2 = InsertPageFromScanner.GetPixValueFromMM((int)insertPageProperty.DefaultSize.Width);
				}
				else
				{
					num = InsertPageFromScanner.GetPixValueFromMM((int)insertPageProperty.DefaultSize.Width);
					num2 = InsertPageFromScanner.GetPixValueFromMM((int)insertPageProperty.DefaultSize.Height);
				}
			}
			else if (insertPageProperty.cbPageSize.SelectedIndex == 1)
			{
				bool? flag = insertPageProperty.PortraitRadioButton.IsChecked;
				bool flag2 = false;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					num = InsertPageFromScanner.GetPixValueFromMM((int)this.A4SizeF.Height);
					num2 = InsertPageFromScanner.GetPixValueFromMM((int)this.A4SizeF.Width);
				}
				else
				{
					num = InsertPageFromScanner.GetPixValueFromMM((int)this.A4SizeF.Width);
					num2 = InsertPageFromScanner.GetPixValueFromMM((int)this.A4SizeF.Height);
				}
			}
			else if (insertPageProperty.cbPageSize.SelectedIndex == 2)
			{
				bool? flag = insertPageProperty.PortraitRadioButton.IsChecked;
				bool flag2 = false;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					num = InsertPageFromScanner.GetPixValueFromMM((int)this.A3SizeF.Height);
					num2 = InsertPageFromScanner.GetPixValueFromMM((int)this.A3SizeF.Width);
				}
				else
				{
					num = InsertPageFromScanner.GetPixValueFromMM((int)this.A3SizeF.Width);
					num2 = InsertPageFromScanner.GetPixValueFromMM((int)this.A3SizeF.Height);
				}
			}
			else
			{
				bool? flag = insertPageProperty.PortraitRadioButton.IsChecked;
				bool flag2 = false;
				int num3;
				int num4;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					num3 = int.Parse(insertPageProperty.tboxPageHeight.Text.Trim());
					num4 = int.Parse(insertPageProperty.tboxPageWidth.Text.Trim());
				}
				else
				{
					num3 = int.Parse(insertPageProperty.tboxPageWidth.Text.Trim());
					num4 = int.Parse(insertPageProperty.tboxPageHeight.Text.Trim());
				}
				num = InsertPageFromScanner.GetPixValueFromMM(num3);
				num2 = InsertPageFromScanner.GetPixValueFromMM(num4);
			}
			return new SizeF(num, num2);
		}

		// Token: 0x060021E1 RID: 8673 RVA: 0x0009C8FD File Offset: 0x0009AAFD
		private static float GetPixValueFromMM(int mm)
		{
			return (float)((double)(mm * 72) / 25.4);
		}

		// Token: 0x060021E2 RID: 8674 RVA: 0x0009C910 File Offset: 0x0009AB10
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ScannerWindow", "CancelBtn", this.CreateNew ? "NewPDF" : "InsertPages", 1L);
			base.Closing -= this.InsertPageFromScanner_Closing;
			if (this.insertFromScannerViewModel.PageList.Count > 0 && ModernMessageBox.Show(pdfeditor.Properties.Resources.ScannerWinClosingWarning, UtilManager.GetProductName(), MessageBoxButton.OKCancel, MessageBoxResult.None, null, false) == MessageBoxResult.Cancel)
			{
				base.Closing += this.InsertPageFromScanner_Closing;
				return;
			}
			this.insertFromScannerViewModel.cts.Cancel();
			this.insertFromScannerViewModel.Dispose();
			base.Close();
		}

		// Token: 0x060021E3 RID: 8675 RVA: 0x0009C9B1 File Offset: 0x0009ABB1
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ScannerWindow", "NewScanner", this.CreateNew ? "NewPDF" : "InsertPages", 1L);
			Process.Start("ms-settings:printers");
		}

		// Token: 0x060021E4 RID: 8676 RVA: 0x0009C9E4 File Offset: 0x0009ABE4
		private void RefreshBtn_Click(object sender, RoutedEventArgs e)
		{
			this.insertFromScannerViewModel.ScannerConnecting = true;
			GAManager.SendEvent("ScannerWindow", "RefreshBtn", this.CreateNew ? "NewPDF" : "InsertPages", 1L);
			Application.Current.Dispatcher.Invoke(delegate
			{
				this.insertFromScannerViewModel.Refreshed();
			});
		}

		// Token: 0x060021E8 RID: 8680 RVA: 0x0009CB86 File Offset: 0x0009AD86
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 1)
			{
				((Button)target).Click += this.Button_Click;
			}
		}

		// Token: 0x04000E09 RID: 3593
		public InsertFromScannerViewModel insertFromScannerViewModel;

		// Token: 0x04000E0A RID: 3594
		private readonly PdfDocument doc;

		// Token: 0x04000E0B RID: 3595
		private int firstSelectedPage = -1;

		// Token: 0x04000E0C RID: 3596
		private int lastSelectedPage = -1;

		// Token: 0x04000E0D RID: 3597
		private int selectedFirstIndex = -1;

		// Token: 0x04000E0E RID: 3598
		private SizeF DefaultSize = new SizeF(210f, 297f);

		// Token: 0x04000E0F RID: 3599
		private SizeF A4SizeF = new SizeF(210f, 297f);

		// Token: 0x04000E10 RID: 3600
		private SizeF A3SizeF = new SizeF(297f, 420f);

		// Token: 0x04000E11 RID: 3601
		private Dictionary<string, SizeF> dicItem = new Dictionary<string, SizeF>();

		// Token: 0x04000E12 RID: 3602
		private bool CreateNew;

		// Token: 0x04000E13 RID: 3603
		private int marginWidth;

		// Token: 0x04000E14 RID: 3604
		private int marginHeight;

		// Token: 0x04000E15 RID: 3605
		private MainViewModel mainViewModel;

		// Token: 0x04000E16 RID: 3606
		private bool FromSinglePageCmd;

		// Token: 0x04000E17 RID: 3607
		private IEnumerable<int> SelectedPages;
	}
}
