using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonLib.Common;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Win32;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit.Utils;

namespace pdfeditor.Controls.Signature
{
	// Token: 0x020001F5 RID: 501
	public partial class SignatureCreateWin : Window
	{
		// Token: 0x17000A26 RID: 2598
		// (get) Token: 0x06001C38 RID: 7224 RVA: 0x00075782 File Offset: 0x00073982
		// (set) Token: 0x06001C39 RID: 7225 RVA: 0x00075794 File Offset: 0x00073994
		public bool ClearVisible
		{
			get
			{
				return (bool)base.GetValue(SignatureCreateWin.ClearVisibleProperty);
			}
			set
			{
				base.SetValue(SignatureCreateWin.ClearVisibleProperty, value);
			}
		}

		// Token: 0x17000A27 RID: 2599
		// (get) Token: 0x06001C3A RID: 7226 RVA: 0x000757A7 File Offset: 0x000739A7
		public SignatureCreateDialogResult ResultModel
		{
			get
			{
				return this.resultModel;
			}
		}

		// Token: 0x06001C3B RID: 7227 RVA: 0x000757B0 File Offset: 0x000739B0
		public SignatureCreateWin()
		{
			this.InitializeComponent();
			this.MainMenus = new ObservableCollection<MenuModel>();
			this.FontItemList = new ObservableCollection<FontItem>();
			this.InitMenu();
			this.Menus.ItemsSource = this.MainMenus;
			this.Menus.SelectedIndex = 0;
			this.WriteStrokeWidths.SelectedIndex = 0;
			this.inkCanvas.StrokeCollected += delegate(object o, InkCanvasStrokeCollectedEventArgs e)
			{
				this.ShowClear();
			};
			this.TypeWriterCtrl.TextChanged += delegate(object o, TextChangedEventArgs e)
			{
				this.ShowClear();
				this.TypeWriterCtrl.FontSize = this.CalculateMaxWidthFontSize();
			};
			this.WriteStrokeWidths.SelectionChanged += delegate(object o, SelectionChangedEventArgs e)
			{
				string text = ((e.AddedItems.Count > 0) ? ((string)e.AddedItems[0]) : "1pt");
				double num;
				if (double.TryParse(text.Substring(0, text.Length - 2), out num))
				{
					this.inkCanvas.DefaultDrawingAttributes.Width = num;
					this.inkCanvas.DefaultDrawingAttributes.Height = num;
				}
			};
			this.btnOk.Click += delegate(object o, RoutedEventArgs e)
			{
				if (!this.CheckOk())
				{
					return;
				}
				Ioc.Default.GetRequiredService<MainViewModel>();
				if (this.Menus.SelectedIndex == 0)
				{
					this.SavePictureImg();
				}
				else if (this.Menus.SelectedIndex == 2)
				{
					this.SaveInkToImg();
				}
				else if (this.Menus.SelectedIndex == 1 && !this.SaveTypeImg())
				{
					return;
				}
				this.showPicture.Source = null;
				this.ResultModel.RemoveBackground = this.ckbRemoveBg.IsChecked.Value;
				if (this.ResultModel.RemoveBackground)
				{
					this.SaveConfigRemoveBg(this.ResultModel.ImageFilePath);
				}
				try
				{
					GAManager.SendEvent("PdfStampAnnotationSignature", "SaveSignature", this.GetTemplateCount().ToString(), 1L);
				}
				catch
				{
				}
				base.DialogResult = new bool?(true);
			};
			this.btnCancel.Click += delegate(object o, RoutedEventArgs e)
			{
				Ioc.Default.GetRequiredService<MainViewModel>();
				this.showPicture.Source = null;
				base.DialogResult = new bool?(false);
				base.Close();
			};
			this.inkCanvas.UseCustomCursor = true;
			this.inkCanvas.Cursor = CursorHelper.CreateCursor(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Style\\\\Resources\\\\Annonate\\\\signaturewrite.png"), 11U, 32U);
		}

		// Token: 0x06001C3C RID: 7228 RVA: 0x000758DA File Offset: 0x00073ADA
		private void SaveConfigRemoveBg(string fileName)
		{
			ConfigManager.AddSignatureRemoveBg(fileName);
		}

		// Token: 0x06001C3D RID: 7229 RVA: 0x000758E4 File Offset: 0x00073AE4
		private int GetTemplateCount()
		{
			string text = Path.Combine(AppDataHelper.LocalFolder, "Signature");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return Directory.GetFiles(text).ToList<string>().Count;
		}

		// Token: 0x06001C3E RID: 7230 RVA: 0x00075920 File Offset: 0x00073B20
		private bool CheckOk()
		{
			return (!(this.SelectedMenuModel.Title == pdfeditor.Properties.Resources.WinSignatureMenuPictureContent) || this.showPicture.Source != null) && (!(this.SelectedMenuModel.Title == pdfeditor.Properties.Resources.WinSignatureMenuWriteContent) || (this.inkCanvas.Strokes != null && this.inkCanvas.Strokes.Count != 0));
		}

		// Token: 0x06001C3F RID: 7231 RVA: 0x0007598C File Offset: 0x00073B8C
		private void ShowClear()
		{
			string title = this.SelectedMenuModel.Title;
			string tag = this.SelectedMenuModel.Tag;
			if (tag == "Write")
			{
				this.ClearVisible = this.inkCanvas.Strokes.Count > 0;
				return;
			}
			if (tag == "Picture")
			{
				this.ClearVisible = this.showPicture.Source != null;
				return;
			}
			if (tag == "Type")
			{
				this.ClearVisible = !string.IsNullOrEmpty(this.TypeWriterCtrl.Text);
			}
		}

		// Token: 0x06001C40 RID: 7232 RVA: 0x00075A20 File Offset: 0x00073C20
		private void InitMenu()
		{
			this.MainMenus.Clear();
			this.MainMenus.Add(new MenuModel
			{
				Title = pdfeditor.Properties.Resources.WinSignatureMenuPictureContent,
				Tag = "Picture"
			});
			this.MainMenus.Add(new MenuModel
			{
				Title = pdfeditor.Properties.Resources.WinSignatureMenuInputContent,
				Tag = "Type"
			});
			this.MainMenus.Add(new MenuModel
			{
				Title = pdfeditor.Properties.Resources.WinSignatureMenuWriteContent,
				Tag = "Write"
			});
			this.GetLocaltem();
			this.FontFamilysCtrl.ItemsSource = this.FontItemList;
		}

		// Token: 0x06001C41 RID: 7233 RVA: 0x00075AC4 File Offset: 0x00073CC4
		private void SaveInkToImg()
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			if (this.SelectedMenuModel != null && this.SelectedMenuModel.Tag == "Write" && this.inkCanvas.Strokes.Count > 0)
			{
				RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)this.PathCtrl.ActualWidth, (int)this.PathCtrl.ActualHeight, 96.0, 96.0, PixelFormats.Default);
				renderTargetBitmap.Render(this.inkCanvas);
				PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
				BitmapSource bitmapSource = this.ReplaceTransparency(renderTargetBitmap, global::System.Windows.Media.Brushes.Transparent.Color);
				pngBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
				renderTargetBitmap.Clear();
				string text = DateTime.Now.ToString("yyyyMMddHHmmss");
				string text2 = Path.Combine(AppDataHelper.LocalFolder, "Signature");
				if (!Directory.Exists(text2))
				{
					Directory.CreateDirectory(text2);
				}
				string text3 = Path.Combine(text2, "SignatureWrite" + text + ".png");
				try
				{
					using (Stream stream = File.Create(text3))
					{
						pngBitmapEncoder.Save(stream);
						this.ResultModel.ImageFilePath = text3;
						requiredService.AnnotationToolbar.StampImgFileOkTime = DateTime.Now;
					}
				}
				catch (Exception)
				{
					this.ResultModel.ImageFilePath = string.Empty;
				}
				finally
				{
					renderTargetBitmap.Freeze();
					GC.Collect();
					renderTargetBitmap = null;
				}
			}
		}

		// Token: 0x06001C42 RID: 7234 RVA: 0x00075C60 File Offset: 0x00073E60
		private bool SaveTypeImg()
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			if (this.TypeWriterCtrl.Text.Trim() == string.Empty)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinSignatureMenuInputIsEmptyMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return false;
			}
			if (this.SelectedMenuModel != null && this.SelectedMenuModel.Tag == "Type" && !string.IsNullOrEmpty(this.TypeWriterCtrl.Text))
			{
				FormattedText formattedText = this.MeasureTextWidth(this.TypeWriterCtrl.FontSize);
				RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)formattedText.Width, (int)formattedText.Height, 96.0, 96.0, PixelFormats.Default);
				renderTargetBitmap.Render(this.TypeWriterCtrl);
				PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
				BitmapSource bitmapSource = this.CreateTextRender(renderTargetBitmap, global::System.Windows.Media.Brushes.Transparent.Color, this.TypeWriterCtrl.Text);
				pngBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapSource));
				renderTargetBitmap.Clear();
				string text = DateTime.Now.ToString("yyyyMMddHHmmss");
				string text2 = Path.Combine(AppDataHelper.LocalFolder, "Signature");
				if (!Directory.Exists(text2))
				{
					Directory.CreateDirectory(text2);
				}
				string text3 = Path.Combine(text2, "SignatureWrite" + text + ".png");
				try
				{
					using (Stream stream = File.Create(text3))
					{
						pngBitmapEncoder.Save(stream);
						this.ResultModel.ImageFilePath = text3;
						requiredService.AnnotationToolbar.StampImgFileOkTime = DateTime.Now;
					}
				}
				catch (Exception)
				{
					this.ResultModel.ImageFilePath = string.Empty;
					return false;
				}
				finally
				{
					renderTargetBitmap.Freeze();
					GC.Collect();
					renderTargetBitmap = null;
				}
				return true;
			}
			return false;
		}

		// Token: 0x06001C43 RID: 7235 RVA: 0x00075E4C File Offset: 0x0007404C
		private void Menus_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Selector selector = sender as Selector;
			MenuModel menuModel = ((selector != null) ? selector.SelectedItem : null) as MenuModel;
			if (menuModel != null)
			{
				this.SelectedMenuModel = menuModel;
				this.ShowClear();
				string title = this.SelectedMenuModel.Title;
				string tag = this.SelectedMenuModel.Tag;
				if (tag == "Picture")
				{
					this.PathCtrl.Visibility = Visibility.Collapsed;
					this.WriteStrokeWidths.Visibility = Visibility.Collapsed;
					this.showPicture.Visibility = Visibility.Visible;
					this.ckbRemoveBg.Visibility = Visibility.Visible;
					this.imgHelp.Visibility = Visibility.Visible;
					this.PictureCtrl.Visibility = ((this.showPicture.Source == null) ? Visibility.Visible : Visibility.Collapsed);
					this.TypeWriterCtrl.Visibility = Visibility.Collapsed;
					this.FontFamilysCtrl.Visibility = Visibility.Collapsed;
					return;
				}
				if (tag == "Write")
				{
					this.PathCtrl.Visibility = Visibility.Visible;
					this.PictureCtrl.Visibility = Visibility.Collapsed;
					this.ckbRemoveBg.Visibility = Visibility.Collapsed;
					this.imgHelp.Visibility = Visibility.Collapsed;
					this.WriteStrokeWidths.Visibility = Visibility.Visible;
					this.showPicture.Visibility = Visibility.Collapsed;
					this.TypeWriterCtrl.Visibility = Visibility.Collapsed;
					this.FontFamilysCtrl.Visibility = Visibility.Collapsed;
					return;
				}
				if (tag == "Type")
				{
					this.PathCtrl.Visibility = Visibility.Collapsed;
					this.PictureCtrl.Visibility = Visibility.Collapsed;
					this.ckbRemoveBg.Visibility = Visibility.Collapsed;
					this.imgHelp.Visibility = Visibility.Collapsed;
					this.WriteStrokeWidths.Visibility = Visibility.Collapsed;
					this.showPicture.Visibility = Visibility.Collapsed;
					this.TypeWriterCtrl.Visibility = Visibility.Visible;
					this.FontFamilysCtrl.Visibility = Visibility.Visible;
					this.TypeWriterCtrl.Focus();
				}
			}
		}

		// Token: 0x06001C44 RID: 7236 RVA: 0x00076004 File Offset: 0x00074204
		private void btnClear_Click(object sender, RoutedEventArgs e)
		{
			if (this.SelectedMenuModel != null)
			{
				string title = this.SelectedMenuModel.Title;
				string tag = this.SelectedMenuModel.Tag;
				if (tag == "Picture")
				{
					this.showPicture.Source = null;
					this.PictureCtrl.Visibility = Visibility.Visible;
					this.ShowClear();
				}
				if (tag == "Write")
				{
					this.inkCanvas.Strokes.Clear();
					this.ShowClear();
				}
				if (tag == "Type")
				{
					this.TypeWriterCtrl.Text = string.Empty;
					this.ShowClear();
				}
			}
		}

		// Token: 0x06001C45 RID: 7237 RVA: 0x000760A4 File Offset: 0x000742A4
		private void PictureCtrl_Click(object sender, RoutedEventArgs e)
		{
			Ioc.Default.GetRequiredService<MainViewModel>();
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "All Image Files|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.png;*.tif;*.tiff|Windows Bitmap(*.bmp)|*.bmp|Windows Icon(*.ico)|*.ico|Graphics Interchange Format (*.gif)|(*.gif)|JPEG File Interchange Format (*.jpg)|*.jpg;*.jpeg|Portable Network Graphics (*.png)|*.png|Tag Image File Format (*.tif)|*.tif;*.tiff",
				ShowReadOnly = false,
				ReadOnlyChecked = true
			};
			if (openFileDialog.ShowDialog().GetValueOrDefault() && !string.IsNullOrEmpty(openFileDialog.FileName))
			{
				try
				{
					this.showPicture.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
					this.FileDiaoligFiePath = openFileDialog.FileName;
					this.PictureCtrl.Visibility = Visibility.Collapsed;
					this.ShowClear();
					goto IL_009B;
				}
				catch
				{
					DrawUtils.ShowUnsupportedImageMessage();
					return;
				}
			}
			this.ResultModel.ImageFilePath = string.Empty;
			IL_009B:
			base.Activate();
		}

		// Token: 0x06001C46 RID: 7238 RVA: 0x00076164 File Offset: 0x00074364
		public void SavePictureImg()
		{
			if (string.IsNullOrEmpty(this.FileDiaoligFiePath))
			{
				return;
			}
			Ioc.Default.GetRequiredService<MainViewModel>();
			string text = DateTime.Now.ToString("yyyyMMddHHmmss");
			string text2 = Path.Combine(AppDataHelper.LocalFolder, "Signature");
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			string text3 = Path.Combine(text2, "SignatureWrite" + text + ".png");
			File.Copy(this.FileDiaoligFiePath, text3, true);
			this.ResultModel.ImageFilePath = text3;
			this.FileDiaoligFiePath = string.Empty;
		}

		// Token: 0x06001C47 RID: 7239 RVA: 0x000761F8 File Offset: 0x000743F8
		public void FileStreamUseCopy(string source, string target)
		{
			using (FileStream fileStream = new FileStream(source, FileMode.OpenOrCreate, FileAccess.Read))
			{
				using (FileStream fileStream2 = new FileStream(target, FileMode.OpenOrCreate, FileAccess.Write))
				{
					byte[] array = new byte[2097152];
					int num;
					while ((num = fileStream.Read(array, 0, array.Length)) > 0)
					{
						fileStream2.Write(array, 0, num);
					}
				}
			}
		}

		// Token: 0x06001C48 RID: 7240 RVA: 0x00076274 File Offset: 0x00074474
		public BitmapSource ReplaceTransparency(BitmapSource bitmap, global::System.Windows.Media.Color color)
		{
			DrawingVisual drawingVisual = new DrawingVisual();
			Rect rect = new Rect(0.0, 0.0, (double)bitmap.PixelWidth, (double)bitmap.PixelHeight);
			DrawingContext drawingContext = drawingVisual.RenderOpen();
			drawingContext.DrawRectangle(new SolidColorBrush(color), null, rect);
			drawingContext.DrawImage(bitmap, rect);
			drawingContext.Close();
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(bitmap.PixelWidth, bitmap.PixelHeight, 96.0, 96.0, PixelFormats.Pbgra32);
			renderTargetBitmap.Render(drawingVisual);
			return renderTargetBitmap;
		}

		// Token: 0x06001C49 RID: 7241 RVA: 0x00076300 File Offset: 0x00074500
		public BitmapSource CreateTextRender(BitmapSource bitmap, global::System.Windows.Media.Color color, string Text)
		{
			DrawingVisual drawingVisual = new DrawingVisual();
			Rect rect = new Rect(0.0, 0.0, (double)bitmap.PixelWidth, (double)bitmap.PixelHeight);
			DrawingContext drawingContext = drawingVisual.RenderOpen();
			drawingContext.DrawRectangle(new SolidColorBrush(color), null, rect);
			global::System.Windows.Media.FontFamily fontFamily = this.TypeWriterCtrl.FontFamily;
			FormattedText formattedText = new FormattedText(Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(fontFamily, base.FontStyle, base.FontWeight, base.FontStretch), this.TypeWriterCtrl.FontSize, global::System.Windows.Media.Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
			drawingContext.DrawText(formattedText, new global::System.Windows.Point(rect.Left, rect.Top));
			drawingContext.Close();
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(bitmap.PixelWidth, bitmap.PixelHeight, 96.0, 96.0, PixelFormats.Pbgra32);
			renderTargetBitmap.Render(drawingVisual);
			return renderTargetBitmap;
		}

		// Token: 0x06001C4A RID: 7242 RVA: 0x000763EC File Offset: 0x000745EC
		private FormattedText MeasureTextWidth(double fontSize)
		{
			global::System.Windows.Media.FontFamily fontFamily = this.TypeWriterCtrl.FontFamily;
			return new FormattedText(this.TypeWriterCtrl.Text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(fontFamily, base.FontStyle, base.FontWeight, base.FontStretch), fontSize, global::System.Windows.Media.Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
		}

		// Token: 0x06001C4B RID: 7243 RVA: 0x00076448 File Offset: 0x00074648
		private double CalculateMaxWidthFontSize()
		{
			double width = this.TypeWriterCtrl.Width;
			double num = 96.0;
			if (double.IsNaN(width))
			{
				return num;
			}
			if (this.MeasureTextWidth(num).WidthIncludingTrailingWhitespace < width)
			{
				return num;
			}
			double num2 = 0.0;
			while (num > 0.0)
			{
				double num3 = (num + num2) / 2.0;
				if (this.MeasureTextWidth(num3).WidthIncludingTrailingWhitespace < width && this.MeasureTextWidth(num3 + this.space).WidthIncludingTrailingWhitespace > width)
				{
					return num3;
				}
				if (this.MeasureTextWidth(num3).WidthIncludingTrailingWhitespace < width)
				{
					num2 = num3 + this.space;
				}
				else
				{
					if (this.MeasureTextWidth(num3).WidthIncludingTrailingWhitespace <= width)
					{
						return num3;
					}
					num = num3 - this.space;
				}
			}
			return base.FontSize;
		}

		// Token: 0x06001C4C RID: 7244 RVA: 0x0007650F File Offset: 0x0007470F
		private void inkCanvas_Loaded(object sender, RoutedEventArgs e)
		{
			InkCanvas inkCanvas = (InkCanvas)sender;
			inkCanvas.DefaultDrawingAttributes.Width = 1.0;
			inkCanvas.DefaultDrawingAttributes.Height = 1.0;
			inkCanvas.DefaultDrawingAttributes.FitToCurve = true;
		}

		// Token: 0x06001C4D RID: 7245 RVA: 0x0007654C File Offset: 0x0007474C
		private void GetLocaltem()
		{
			global::System.Drawing.FontFamily[] array = null;
			try
			{
				array = new InstalledFontCollection().Families;
			}
			catch (Exception)
			{
				array = new global::System.Drawing.FontFamily[0];
			}
			ObservableCollection<FontItem> fontItemList = this.FontItemList;
			if (fontItemList != null)
			{
				fontItemList.Clear();
			}
			FontItem defaultItem = new FontItem();
			string text = PdfFontUtils.TryGetDefaultUIFont(CultureInfo.CurrentUICulture);
			if (!string.IsNullOrEmpty(text))
			{
				defaultItem.Name = text;
				defaultItem.DisplayName = text;
			}
			else
			{
				defaultItem.Name = base.FontFamily.Source;
				defaultItem.DisplayName = base.FontFamily.Source;
			}
			global::System.Drawing.FontFamily fontFamily = array.FirstOrDefault((global::System.Drawing.FontFamily x) => x.Name == defaultItem.Name);
			if (fontFamily != null)
			{
				int num = array.ToList<global::System.Drawing.FontFamily>().IndexOf(fontFamily);
				this.FontFamilysCtrl.SelectedIndex = num;
			}
			else
			{
				this.FontItemList.Add(defaultItem);
				this.FontFamilysCtrl.SelectedIndex = 0;
			}
			foreach (global::System.Drawing.FontFamily fontFamily2 in array)
			{
				FontItem fontItem = new FontItem();
				fontItem.Name = fontFamily2.Name;
				fontItem.DisplayName = fontFamily2.Name;
				this.FontItemList.Add(fontItem);
			}
		}

		// Token: 0x06001C4E RID: 7246 RVA: 0x0007669C File Offset: 0x0007489C
		private void FontFamilysCtrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			FontItem fontItem = this.FontFamilysCtrl.SelectedItem as FontItem;
			if (fontItem != null && fontItem.Name != "")
			{
				this.TypeWriterCtrl.FontFamily = new global::System.Windows.Media.FontFamily(fontItem.Name);
			}
		}

		// Token: 0x04000A49 RID: 2633
		private ObservableCollection<MenuModel> MainMenus;

		// Token: 0x04000A4A RID: 2634
		private ObservableCollection<FontItem> FontItemList;

		// Token: 0x04000A4B RID: 2635
		private string FileDiaoligFiePath = string.Empty;

		// Token: 0x04000A4C RID: 2636
		private SignatureCreateDialogResult resultModel = new SignatureCreateDialogResult();

		// Token: 0x04000A4D RID: 2637
		public static readonly DependencyProperty ClearVisibleProperty = DependencyProperty.Register("ClearVisible", typeof(bool), typeof(SignatureCreateWin), new PropertyMetadata(false));

		// Token: 0x04000A4E RID: 2638
		private MenuModel SelectedMenuModel;

		// Token: 0x04000A4F RID: 2639
		private double space = 1.0;
	}
}
