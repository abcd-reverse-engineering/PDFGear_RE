using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommonLib.Common;
using CommonLib.Controls;
using Microsoft.Win32;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Properties;
using PDFKit.Utils;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x02000218 RID: 536
	public partial class ExtractTextResultDialog : Window
	{
		// Token: 0x06001DA8 RID: 7592 RVA: 0x000800EC File Offset: 0x0007E2EC
		public static ExtractTextResultDialog FromPage(PdfDocument document, ScreenshotDialogResult result)
		{
			return new ExtractTextResultDialog(document, result, true);
		}

		// Token: 0x06001DA9 RID: 7593 RVA: 0x000800F6 File Offset: 0x0007E2F6
		public static ExtractTextResultDialog FromImage(PdfDocument document, ScreenshotDialogResult result)
		{
			return new ExtractTextResultDialog(document, result, false);
		}

		// Token: 0x06001DAA RID: 7594 RVA: 0x00080100 File Offset: 0x0007E300
		private ExtractTextResultDialog(PdfDocument document, ScreenshotDialogResult result, bool fromPage)
		{
			if (fromPage && document == null)
			{
				throw new ArgumentNullException("document");
			}
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			this.InitializeComponent();
			this.document = document;
			this.result = result;
			this.fromPage = fromPage;
			base.Loaded += this.ExtractTextResultDialog_Loaded;
			this.showToastAnimation = this.LayoutRoot.Resources["ShowToastAnimation"] as Storyboard;
			this.LanguageButton.Opacity = 0.0;
			if (fromPage && (result.Mode == ScreenshotDialogMode.Ocr || string.IsNullOrEmpty(result.ExtractedText)))
			{
				GAManager.SendEvent("ExtractText", "OCRInit", "Checked", 1L);
				this.OcrCheckBox.IsChecked = new bool?(true);
				return;
			}
			if (!fromPage)
			{
				this.OcrCheckBox.IsChecked = new bool?(true);
				return;
			}
			GAManager.SendEvent("ExtractText", "OCRInit", "Unchecked", 1L);
		}

		// Token: 0x06001DAB RID: 7595 RVA: 0x00080200 File Offset: 0x0007E400
		private async void ExtractTextResultDialog_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.fromPage)
			{
				if (this.OcrCheckBox.IsChecked.GetValueOrDefault())
				{
					GAManager.SendEvent("ExtractText", "Show", this.result.Mode.ToString() + "Checked", 1L);
				}
				else
				{
					GAManager.SendEvent("ExtractText", "Show", this.result.Mode.ToString() + "Unchecked", 1L);
				}
			}
			else
			{
				this.OcrCheckBox.Visibility = Visibility.Collapsed;
			}
			string text = await ConfigManager.GetScreenshotOcrLanguage(null);
			if (string.IsNullOrEmpty(text))
			{
				text = OcrUtils.GetDefaultCultureInfoName();
			}
			this.cultureInfoName = text;
			if (text == "Auto")
			{
				this.languageDisplayName = pdfeditor.Properties.Resources.AppSettingsLanguageAutoItem;
			}
			else
			{
				this.languageDisplayName = OcrUtils.GetLanguageDisplayName(CultureInfo.GetCultureInfo(this.cultureInfoName));
			}
			this.LanguageButton.Content = this.languageDisplayName;
			this.LanguageButton.Opacity = 1.0;
			if (this.fromPage)
			{
				await this.InitializeImageForPage();
			}
			else
			{
				this.InitializeImageForImage();
			}
			await this.UpdateResultAsync();
		}

		// Token: 0x06001DAC RID: 7596 RVA: 0x00080238 File Offset: 0x0007E438
		private async Task InitializeImageForPage()
		{
			PdfPage pdfPage = this.document.Pages[this.result.PageIndex];
			FS_RECTF boundBox = pdfPage.GetEffectiveBox(PageRotate.Normal, false);
			this.PagePreviewImage.Height = this.PagePreviewImage.Width / (double)boundBox.Width * (double)boundBox.Height;
			double num = this.PagePreviewImage.Height - ((FrameworkElement)this.PagePreviewImage.Parent).ActualHeight;
			if (num > 0.0)
			{
				base.Height += num;
			}
			else
			{
				this.GeoCanvas.Margin = new Thickness(0.0, -num / 2.0, 0.0, 0.0);
			}
			WriteableBitmap image = null;
			try
			{
				WriteableBitmap writeableBitmap = await ScreenshotDialog.GetPageImageAsync(this.PagePreviewImage.Width * 4.0, this.PagePreviewImage.Height * 4.0, pdfPage, null);
				image = writeableBitmap;
			}
			catch
			{
			}
			if (image == null)
			{
				this.ImageColumn.Width = new GridLength(0.0, GridUnitType.Pixel);
			}
			else
			{
				this.PagePreviewImage.Source = image;
				double num2 = this.PagePreviewImage.Width / (double)boundBox.Width;
				FS_RECTF selectedRect = this.result.SelectedRect;
				Rect rect = new Rect((double)(selectedRect.left - boundBox.left) * num2, (double)(boundBox.top - selectedRect.top) * num2, (double)(selectedRect.right - selectedRect.left) * num2, (double)(selectedRect.top - selectedRect.bottom) * num2);
				global::System.Windows.Shapes.Rectangle rectangle = new global::System.Windows.Shapes.Rectangle();
				rectangle.Stroke = global::System.Windows.Media.Brushes.Red;
				rectangle.StrokeThickness = 1.0;
				rectangle.Width = rect.Width;
				rectangle.Height = rect.Height;
				Canvas.SetLeft(rectangle, rect.Left);
				Canvas.SetTop(rectangle, rect.Top);
				this.GeoCanvas.Children.Add(rectangle);
			}
		}

		// Token: 0x06001DAD RID: 7597 RVA: 0x0008027C File Offset: 0x0007E47C
		private void InitializeImageForImage()
		{
			global::System.Drawing.Image ocrImage = this.result.OcrImage;
			byte[] array;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				ocrImage.Save(memoryStream, ImageFormat.Bmp);
				array = memoryStream.ToArray();
			}
			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			bitmapImage.StreamSource = new MemoryStream(array);
			bitmapImage.EndInit();
			ImageSource imageSource = bitmapImage;
			this.PagePreviewImage.Source = imageSource;
		}

		// Token: 0x06001DAE RID: 7598 RVA: 0x000802FC File Offset: 0x0007E4FC
		private void CloseBtn_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x06001DAF RID: 7599 RVA: 0x00080304 File Offset: 0x0007E504
		private async void CopyBtn_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).IsEnabled = false;
			try
			{
				Clipboard.SetDataObject(this.textResult);
				this.showToastAnimation.SkipToFill();
				this.showToastAnimation.Begin();
				await Task.Delay(300);
			}
			catch
			{
				this.showToastAnimation.SkipToFill();
			}
			((Button)sender).IsEnabled = true;
			if (this.OcrCheckBox.IsChecked.GetValueOrDefault())
			{
				GAManager.SendEvent("ExtractText", "Copy", this.result.Mode.ToString() + "Checked", 1L);
			}
			else
			{
				GAManager.SendEvent("ExtractText", "Copy", this.result.Mode.ToString() + "Unchecked", 1L);
			}
		}

		// Token: 0x06001DB0 RID: 7600 RVA: 0x00080344 File Offset: 0x0007E544
		private async void DownloadBtn_Click(object sender, RoutedEventArgs e)
		{
			((Button)sender).IsEnabled = false;
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				AddExtension = true,
				Filter = "txt|*.txt",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
				FileName = "Extract.txt"
			};
			if (saveFileDialog.ShowDialog().GetValueOrDefault())
			{
				string fileName = saveFileDialog.FileName;
				base.Cursor = Cursors.AppStarting;
				try
				{
					File.WriteAllText(fileName, this.textResult, Encoding.UTF8);
					await ExplorerUtils.SelectItemInExplorerAsync(fileName, default(CancellationToken));
				}
				catch
				{
				}
				finally
				{
					base.Cursor = null;
				}
			}
			((Button)sender).IsEnabled = true;
			if (this.OcrCheckBox.IsChecked.GetValueOrDefault())
			{
				GAManager.SendEvent("ExtractText", "Save", this.result.Mode.ToString() + "Checked", 1L);
			}
			else
			{
				GAManager.SendEvent("ExtractText", "Save", this.result.Mode.ToString() + "Unchecked", 1L);
			}
		}

		// Token: 0x06001DB1 RID: 7601 RVA: 0x00080383 File Offset: 0x0007E583
		private void PagePreviewImage_SizeChanged(object sender, SizeChangedEventArgs e)
		{
		}

		// Token: 0x06001DB2 RID: 7602 RVA: 0x00080388 File Offset: 0x0007E588
		private async void OcrCheckBox_Click(object sender, RoutedEventArgs e)
		{
			await base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(async delegate
			{
				if (this.OcrCheckBox.IsChecked.GetValueOrDefault())
				{
					GAManager.SendEvent("ExtractText", "OCRCheckboxClick", this.result.Mode.ToString() + "Checked", 1L);
				}
				else
				{
					GAManager.SendEvent("ExtractText", "OCRCheckboxClick", this.result.Mode.ToString() + "Unchecked", 1L);
				}
				await this.UpdateResultAsync();
			}));
		}

		// Token: 0x06001DB3 RID: 7603 RVA: 0x000803C0 File Offset: 0x0007E5C0
		private async void LanguageButton_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("ExtractText", "ChangeLanguage", "Count", 1L);
			OcrSelectLanguageDialog ocrSelectLanguageDialog = new OcrSelectLanguageDialog(this.cultureInfoName, true);
			ocrSelectLanguageDialog.Owner = this;
			ocrSelectLanguageDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			if (ocrSelectLanguageDialog.ShowDialog().GetValueOrDefault() && this.cultureInfoName != ocrSelectLanguageDialog.SelectedCultureInfoName)
			{
				this.cultureInfoName = ocrSelectLanguageDialog.SelectedCultureInfoName;
				this.languageDisplayName = ocrSelectLanguageDialog.SelectedDisplayName;
				await ConfigManager.SetScreenshotOcrLanguageAsync(this.cultureInfoName);
				this.LanguageButton.Content = this.languageDisplayName;
				await this.UpdateOcrResultAsync();
			}
		}

		// Token: 0x06001DB4 RID: 7604 RVA: 0x000803F8 File Offset: 0x0007E5F8
		private void SetProcessingState(bool processing)
		{
			if (processing)
			{
				this.ProcessingDismissBorder.Visibility = Visibility.Visible;
				this.ProcessingRing.IsActive = true;
				this.CopyBtn.IsEnabled = false;
				this.DownloadBtn.IsEnabled = false;
				return;
			}
			this.ProcessingDismissBorder.Visibility = Visibility.Collapsed;
			this.ProcessingRing.IsActive = false;
			this.CopyBtn.IsEnabled = true;
			this.DownloadBtn.IsEnabled = true;
		}

		// Token: 0x06001DB5 RID: 7605 RVA: 0x0008046C File Offset: 0x0007E66C
		private void SetResult(string text)
		{
			this.textResult = text;
			this.rtb.Document.Blocks.Clear();
			this.rtb.AppendText(text);
			base.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(delegate
			{
				this.rtb.SelectAll();
				this.rtb.Focus();
				Keyboard.Focus(this.rtb);
			}));
			this.SetProcessingState(false);
		}

		// Token: 0x06001DB6 RID: 7606 RVA: 0x000804C4 File Offset: 0x0007E6C4
		private async Task UpdateResultAsync()
		{
			if (this.OcrCheckBox.IsChecked.GetValueOrDefault())
			{
				GAManager.SendEvent("ExtractText", "ExtractText", "OCRChecked", 1L);
				await this.UpdateOcrResultAsync();
			}
			else
			{
				GAManager.SendEvent("ExtractText", "ExtractText", "OCRUnChecked", 1L);
				this.SetResult(this.result.ExtractedText);
				this.SetProcessingState(false);
			}
		}

		// Token: 0x06001DB7 RID: 7607 RVA: 0x00080508 File Offset: 0x0007E708
		private async Task UpdateOcrResultAsync()
		{
			this.SetProcessingState(true);
			try
			{
				string text = this.cultureInfoName;
				bool autoMode = text == "Auto";
				CultureInfo defaultCultureInfo = CultureInfo.GetCultureInfo(OcrUtils.GetDefaultCultureInfoName());
				Stopwatch sw = Stopwatch.StartNew();
				object bitmap = this.result.RotatedImage ?? this.result.OcrImage;
				PdfPage page = this.document.Pages[this.result.PageIndex];
				CultureInfo cultureInfo;
				if (bitmap != null)
				{
					if (autoMode)
					{
						string text2 = page.GetExtensionData("OCR_AUTO_CULTURE_INFO_NAME") as string;
						if (text2 != null)
						{
							cultureInfo = CultureInfo.GetCultureInfo(text2);
						}
						else
						{
							CultureInfo cultureInfo2 = await ExtractTextResultDialog.DetectLanguageAsync(bitmap, defaultCultureInfo);
							cultureInfo = cultureInfo2;
							if (cultureInfo == null)
							{
								cultureInfo = defaultCultureInfo;
							}
						}
						this.LanguageButton.Content = this.languageDisplayName + " (" + cultureInfo.NativeName + ")";
					}
					else
					{
						cultureInfo = CultureInfo.GetCultureInfo(this.cultureInfoName);
						this.LanguageButton.Content = this.languageDisplayName;
					}
					string text3 = await ExtractTextResultDialog.GetStringAsync(bitmap, cultureInfo);
					if (autoMode && !string.IsNullOrEmpty(text3) && text3.Length > 40)
					{
						page.SetExtensionData("OCR_AUTO_CULTURE_INFO_NAME", cultureInfo.Name);
					}
					this.SetResult(text3);
				}
				sw.Stop();
				defaultCultureInfo = null;
				cultureInfo = null;
				sw = null;
				bitmap = null;
				page = null;
			}
			catch (Exception ex)
			{
				GAManager.SendEvent("Exception", "OCR", ex.Message ?? "", 1L);
				Log.WriteLog("OCR failed: " + ex.Message + "\n\r" + ex.StackTrace);
				this.SetProcessingState(false);
			}
		}

		// Token: 0x06001DB8 RID: 7608 RVA: 0x0008054C File Offset: 0x0007E74C
		private static Task<CultureInfo> DetectLanguageAsync(object bitmap, CultureInfo cultureInfo)
		{
			ExtractTextResultDialog.<>c__DisplayClass23_0 CS$<>8__locals1 = new ExtractTextResultDialog.<>c__DisplayClass23_0();
			CS$<>8__locals1.cultureInfo = cultureInfo;
			CS$<>8__locals1.bitmapSource = bitmap as BitmapSource;
			if (CS$<>8__locals1.bitmapSource != null)
			{
				return Task.Run<CultureInfo>(() => OcrUtils.DetectLanguage(CS$<>8__locals1.bitmapSource, CS$<>8__locals1.cultureInfo));
			}
			global::System.Drawing.Image image = bitmap as global::System.Drawing.Image;
			if (image != null)
			{
				return Task.Run<CultureInfo>(() => OcrUtils.DetectLanguage(image, CS$<>8__locals1.cultureInfo));
			}
			return Task.FromResult<CultureInfo>(null);
		}

		// Token: 0x06001DB9 RID: 7609 RVA: 0x000805C8 File Offset: 0x0007E7C8
		private static Task<string> GetStringAsync(object bitmap, CultureInfo cultureInfo)
		{
			BitmapSource bitmapSource = bitmap as BitmapSource;
			if (bitmapSource != null)
			{
				return OcrUtils.GetStringAsync(bitmapSource, cultureInfo);
			}
			global::System.Drawing.Image image = bitmap as global::System.Drawing.Image;
			if (image != null)
			{
				return OcrUtils.GetStringAsync(image, cultureInfo);
			}
			return Task.FromResult<string>(null);
		}

		// Token: 0x04000B36 RID: 2870
		private readonly PdfDocument document;

		// Token: 0x04000B37 RID: 2871
		private readonly ScreenshotDialogResult result;

		// Token: 0x04000B38 RID: 2872
		private readonly Storyboard showToastAnimation;

		// Token: 0x04000B39 RID: 2873
		private string cultureInfoName;

		// Token: 0x04000B3A RID: 2874
		private string languageDisplayName;

		// Token: 0x04000B3B RID: 2875
		private string textResult;

		// Token: 0x04000B3C RID: 2876
		private bool fromPage;
	}
}
