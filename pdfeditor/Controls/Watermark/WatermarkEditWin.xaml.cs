using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonLib.Common;
using CommonLib.Controls.ColorPickers;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Win32;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using pdfeditor.Controls.PageEditor;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils;
using PDFKit.Utils.WatermarkUtils;

namespace pdfeditor.Controls.Watermark
{
	// Token: 0x020001E2 RID: 482
	public partial class WatermarkEditWin : Window
	{
		// Token: 0x17000A04 RID: 2564
		// (get) Token: 0x06001B36 RID: 6966 RVA: 0x0006DD8A File Offset: 0x0006BF8A
		// (set) Token: 0x06001B37 RID: 6967 RVA: 0x0006DD9C File Offset: 0x0006BF9C
		public string CustomRotate
		{
			get
			{
				return (string)base.GetValue(WatermarkEditWin.CustomRotateProperty);
			}
			set
			{
				base.SetValue(WatermarkEditWin.CustomRotateProperty, value);
			}
		}

		// Token: 0x06001B38 RID: 6968 RVA: 0x0006DDAC File Offset: 0x0006BFAC
		public WatermarkEditWin()
		{
			this.InitializeComponent();
			this.InitColorButton();
			base.Loaded += this.WatermarkEditWin_Loaded;
		}

		// Token: 0x06001B39 RID: 6969 RVA: 0x0006DEBC File Offset: 0x0006C0BC
		private void InitColorButton()
		{
			this.PART_PanelColor.Children.Clear();
			foreach (string text in this._colorPresetList)
			{
				this.PART_PanelColor.Children.Add(this.CreateColorButton(text));
			}
		}

		// Token: 0x06001B3A RID: 6970 RVA: 0x0006DF30 File Offset: 0x0006C130
		private Button CreateColorButton(string colorStr)
		{
			object obj;
			if ((obj = global::System.Windows.Media.ColorConverter.ConvertFromString(colorStr)) == null)
			{
				obj = default(global::System.Windows.Media.Color);
			}
			SolidColorBrush solidColorBrush = new SolidColorBrush((global::System.Windows.Media.Color)obj);
			Button button = new Button();
			button.Background = solidColorBrush;
			button.Margin = new Thickness(6.0);
			button.Content = new Border
			{
				Background = solidColorBrush,
				Width = 12.0,
				Height = 12.0,
				CornerRadius = new CornerRadius(2.0)
			};
			button.Click += delegate(object s, RoutedEventArgs e)
			{
				this.SelectedFontground = (s as Button).Background.ToString();
				this.PopColor.IsOpen = false;
			};
			return button;
		}

		// Token: 0x06001B3B RID: 6971 RVA: 0x0006DFD8 File Offset: 0x0006C1D8
		private void WatermarkEditWin_Loaded(object sender, RoutedEventArgs e)
		{
			global::System.Windows.Media.Color color = (global::System.Windows.Media.Color)global::System.Windows.Media.ColorConverter.ConvertFromString(this.SelectedFontground);
			this.WatermarkColorPicker.SelectedColor = color;
		}

		// Token: 0x06001B3C RID: 6972 RVA: 0x0006E004 File Offset: 0x0006C204
		private async void btnOk_Click(object sender, RoutedEventArgs e)
		{
			global::System.Windows.Media.Color selectedColor = this.WatermarkColorPicker.SelectedColor;
			this.SelectedFontground = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", new object[] { selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B });
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			if (this.textWatermarkRadioButton.IsChecked.GetValueOrDefault())
			{
				requiredService.AnnotationToolbar.WatermarkModel = WatermarkAnnonationModel.Text;
				if (string.IsNullOrWhiteSpace(this.txtWatermarkConent.Text.Trim()))
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.WinWatermarkTextEmptyMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return;
				}
				this.TextModel = new WatermarkTextModel();
				this.TextModel.Content = this.txtWatermarkConent.Text.Trim();
				this.TextModel.Foreground = this.SelectedFontground;
				float num;
				if (float.TryParse((this.fontSizeComboBox.SelectedItem as ComboBoxItem).Content.ToString(), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num))
				{
					this.TextModel.FontSize = num;
				}
				requiredService.AnnotationToolbar.TextWatermarkModel = this.TextModel;
			}
			else if (this.fileWatermarkRadioButton.IsChecked.GetValueOrDefault())
			{
				requiredService.AnnotationToolbar.WatermarkModel = WatermarkAnnonationModel.Image;
				if (string.IsNullOrWhiteSpace(this.txtWatermarkfile.Text.Trim()))
				{
					ModernMessageBox.Show(pdfeditor.Properties.Resources.WinWatermarkFilePathEmptyMsg, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
					return;
				}
				if (this.ImageModel == null)
				{
					this.ImageModel = new WatermarkImageModel();
				}
				this.ImageModel.ImageFilePath = this.txtWatermarkfile.Text;
				WriteableBitmap writeableBitmap = null;
				try
				{
					using (FileStream fileStream = File.OpenRead(this.ImageModel.ImageFilePath))
					{
						BitmapImage bitmapImage = new BitmapImage();
						bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
						bitmapImage.BeginInit();
						bitmapImage.StreamSource = fileStream;
						bitmapImage.EndInit();
						writeableBitmap = new WriteableBitmap(bitmapImage);
					}
				}
				catch
				{
				}
				this.ImageModel.WatermarkImageSource = writeableBitmap;
				requiredService.AnnotationToolbar.ImageWatermarkModel = this.ImageModel;
			}
			requiredService.AnnotationToolbar.WatermarkParam = this.GetWatermarkParam(requiredService.Document);
			await this.GenerateWaterMarkAsync();
			base.DialogResult = new bool?(this.TextModel != null || this.ImageModel != null);
		}

		// Token: 0x06001B3D RID: 6973 RVA: 0x0006E03C File Offset: 0x0006C23C
		private async Task GenerateWaterMarkAsync()
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			this.watermarkParam = requiredService.AnnotationToolbar.WatermarkParam;
			if (this.watermarkParam.PageRange != null && this.watermarkParam.PageRange.Length != 0)
			{
				GAManager.SendEvent("AnnotationAction", "PdfWatermarkAnnotation", "New", 1L);
				Action<PdfPage> action = null;
				if (requiredService.AnnotationToolbar.WatermarkModel == WatermarkAnnonationModel.Text)
				{
					WatermarkTextModel textWatermarkModel = requiredService.AnnotationToolbar.TextWatermarkModel;
					action = this.CreateTextWatermarkFunc(requiredService.Document, this.watermarkParam, textWatermarkModel);
				}
				else if (requiredService.AnnotationToolbar.WatermarkModel == WatermarkAnnonationModel.Image)
				{
					WatermarkImageModel imageWatermarkModel = requiredService.AnnotationToolbar.ImageWatermarkModel;
					action = this.CreateImageWatermarkFunc(this.watermarkParam, imageWatermarkModel);
				}
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(requiredService.Document);
				global::System.ValueTuple<int, int> valueTuple = ((pdfControl != null) ? pdfControl.GetVisiblePageRange() : new global::System.ValueTuple<int, int>(-1, -1));
				int item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				if (action != null)
				{
					for (int i = 0; i < this.watermarkParam.PageRange.Length; i++)
					{
						int num = this.watermarkParam.PageRange[i];
						IntPtr intPtr = IntPtr.Zero;
						PdfPage pdfPage = null;
						try
						{
							intPtr = Pdfium.FPDF_LoadPage(requiredService.Document.Handle, num);
							if (intPtr != IntPtr.Zero)
							{
								pdfPage = PdfPage.FromHandle(requiredService.Document, intPtr, num, true);
								action(pdfPage);
							}
						}
						finally
						{
							if (pdfPage != null && (pdfPage.PageIndex > item2 || pdfPage.PageIndex < item))
							{
								PageDisposeHelper.DisposePage(pdfPage);
							}
							if (intPtr != IntPtr.Zero)
							{
								Pdfium.FPDF_ClosePage(intPtr);
							}
						}
					}
				}
				if (pdfControl != null)
				{
					await pdfControl.TryRedrawVisiblePageAsync(default(CancellationToken));
				}
			}
		}

		// Token: 0x06001B3E RID: 6974 RVA: 0x0006E080 File Offset: 0x0006C280
		private Action<PdfPage> CreateTextWatermarkFunc(PdfDocument doc, WatermarkParam watermarkParam, WatermarkTextModel textModel)
		{
			global::System.Windows.Media.Color color = (global::System.Windows.Media.Color)global::System.Windows.Media.ColorConverter.ConvertFromString(textModel.Foreground);
			FS_COLOR fillColor = new FS_COLOR((int)((byte)((float)color.A * watermarkParam.Opacity)), (int)color.R, (int)color.G, (int)color.B);
			bool isAuto = textModel.FontSize == 0f;
			float globalFontSize = (isAuto ? 1f : textModel.FontSize);
			float globalBottomBaseline;
			FS_RECTF globalTextBounds;
			global::System.Collections.Generic.IReadOnlyList<TextWithFallbackFontFamily> fallbackFonts = WatermarkUtil.CreateWatermarkTextFonts(textModel.Content, fillColor, "Arial", globalFontSize, out globalTextBounds, out globalBottomBaseline);
			PdfFont[] fonts = fallbackFonts.Select(delegate(TextWithFallbackFontFamily c)
			{
				global::System.Windows.Media.FontFamily fallbackFontFamily = c.FallbackFontFamily;
				string text = (string.IsNullOrEmpty((fallbackFontFamily != null) ? fallbackFontFamily.Source : null) ? "Arial" : c.FallbackFontFamily.Source);
				return PdfFontUtils.CreateFont(doc, text, c.FontWeight, c.FontStyle, c.CharSet);
			}).ToArray<PdfFont>();
			if (watermarkParam.IsTile)
			{
				return delegate(PdfPage p)
				{
					global::System.Collections.Generic.IReadOnlyList<PdfTextObject> readOnlyList = base.<CreateTextWatermarkFunc>g__CreateWatermarkTextObjects|1(fallbackFonts, globalBottomBaseline, globalFontSize, fillColor);
					float num = 1f;
					if (readOnlyList.Count == 0)
					{
						return;
					}
					FS_SIZEF effectiveSize = p.GetEffectiveSize(PageRotate.Normal, false);
					FS_MATRIX fs_MATRIX = new FS_MATRIX();
					fs_MATRIX.SetIdentity();
					fs_MATRIX.Rotate((float)((double)watermarkParam.Rotation * 3.1415926535897931 / 180.0), false);
					if (isAuto)
					{
						num = Math.Min(effectiveSize.Width / 2f / globalTextBounds.Width, effectiveSize.Height / 2f / globalTextBounds.Height);
					}
					fs_MATRIX.Scale(num, num, false);
					FS_RECTF globalTextBounds3 = globalTextBounds;
					fs_MATRIX.TransformRect(ref globalTextBounds3);
					int num2 = (int)Math.Ceiling((double)((effectiveSize.Width + 50f) / (globalTextBounds3.Width + 50f)));
					int num3 = (int)Math.Ceiling((double)((effectiveSize.Height + 50f) / (globalTextBounds3.Height + 50f)));
					PdfWatermarkAnnotation pdfWatermarkAnnotation = new PdfWatermarkAnnotation(p);
					pdfWatermarkAnnotation.Contents = textModel.Content;
					pdfWatermarkAnnotation.Flags |= AnnotationFlags.Print;
					pdfWatermarkAnnotation.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
					pdfWatermarkAnnotation.CreateEmptyAppearance(AppearanceStreamModes.Normal);
					for (int i = 0; i < num3; i++)
					{
						float num4 = effectiveSize.Height - (globalTextBounds3.Height + 50f) * (float)i;
						int j = 0;
						while (j < num2)
						{
							float num5 = (globalTextBounds3.Width + 50f) * (float)j;
							FS_RECTF fs_RECTF = new FS_RECTF(num5, num4, num5 + globalTextBounds3.Width, num4 - globalTextBounds3.Height);
							if (readOnlyList != null)
							{
								goto IL_01D1;
							}
							readOnlyList = base.<CreateTextWatermarkFunc>g__CreateWatermarkTextObjects|1(fallbackFonts, globalBottomBaseline, globalFontSize, fillColor);
							if (readOnlyList.Count != 0)
							{
								goto IL_01D1;
							}
							IL_02BE:
							j++;
							continue;
							IL_01D1:
							for (int k = 0; k < readOnlyList.Count; k++)
							{
								PdfTextObject pdfTextObject = readOnlyList[k];
								FS_POINTF location = pdfTextObject.Location;
								double num6 = (double)watermarkParam.Rotation * 3.1415926535897931 / 180.0;
								FS_MATRIX fs_MATRIX2 = pdfTextObject.Matrix ?? new FS_MATRIX();
								fs_MATRIX2.Translate(-globalTextBounds.Width / 2f, -globalTextBounds.Height / 2f, false);
								fs_MATRIX2.Rotate((float)num6, false);
								fs_MATRIX2.Scale(num, num, false);
								fs_MATRIX2.Translate(fs_RECTF.left + fs_RECTF.Width / 2f, fs_RECTF.bottom + fs_RECTF.Height / 2f, false);
								pdfTextObject.Matrix = fs_MATRIX2;
								pdfWatermarkAnnotation.NormalAppearance.Add(pdfTextObject);
							}
							readOnlyList = null;
							goto IL_02BE;
						}
					}
					pdfWatermarkAnnotation.GenerateAppearance(AppearanceStreamModes.Normal);
					if (p.Annots == null)
					{
						p.CreateAnnotations();
					}
					p.Annots.Add(pdfWatermarkAnnotation);
					foreach (PdfTextObject pdfTextObject2 in pdfWatermarkAnnotation.NormalAppearance.OfType<PdfTextObject>())
					{
						pdfTextObject2.Font.Dictionary.Dispose();
					}
					pdfWatermarkAnnotation.NormalAppearance.Dispose();
					pdfWatermarkAnnotation.Dispose();
					pdfWatermarkAnnotation = null;
				};
			}
			return delegate(PdfPage p)
			{
				float num7 = 1f;
				global::System.Collections.Generic.IReadOnlyList<PdfTextObject> readOnlyList2 = base.<CreateTextWatermarkFunc>g__CreateWatermarkTextObjects|1(fallbackFonts, globalBottomBaseline, globalFontSize, fillColor);
				if (readOnlyList2.Count == 0)
				{
					return;
				}
				FS_SIZEF effectiveSize2 = p.GetEffectiveSize(PageRotate.Normal, false);
				FS_MATRIX fs_MATRIX3 = new FS_MATRIX();
				fs_MATRIX3.SetIdentity();
				fs_MATRIX3.Rotate((float)((double)watermarkParam.Rotation * 3.1415926535897931 / 180.0), false);
				if (isAuto)
				{
					num7 = Math.Min(effectiveSize2.Width / 2f / globalTextBounds.Width, effectiveSize2.Height / 2f / globalTextBounds.Height);
				}
				fs_MATRIX3.Scale(num7, num7, false);
				FS_RECTF globalTextBounds2 = globalTextBounds;
				fs_MATRIX3.TransformRect(ref globalTextBounds2);
				FS_RECTF pdfObjectBounds = this.GetPdfObjectBounds((double)globalTextBounds2.Width, (double)globalTextBounds2.Height, watermarkParam.Alignment, (double)effectiveSize2.Width, (double)effectiveSize2.Height);
				PdfWatermarkAnnotation pdfWatermarkAnnotation2 = new PdfWatermarkAnnotation(p);
				pdfWatermarkAnnotation2.Contents = textModel.Content;
				pdfWatermarkAnnotation2.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
				pdfWatermarkAnnotation2.Flags |= AnnotationFlags.Print;
				pdfWatermarkAnnotation2.CreateEmptyAppearance(AppearanceStreamModes.Normal);
				for (int l = 0; l < readOnlyList2.Count; l++)
				{
					PdfTextObject pdfTextObject3 = readOnlyList2[l];
					FS_POINTF location2 = pdfTextObject3.Location;
					double num8 = (double)watermarkParam.Rotation * 3.1415926535897931 / 180.0;
					FS_MATRIX fs_MATRIX4 = pdfTextObject3.Matrix ?? new FS_MATRIX();
					fs_MATRIX4.Translate(-globalTextBounds.Width / 2f, -globalTextBounds.Height / 2f, false);
					fs_MATRIX4.Rotate((float)num8, false);
					fs_MATRIX4.Scale(num7, num7, false);
					fs_MATRIX4.Translate(pdfObjectBounds.left + pdfObjectBounds.Width / 2f, pdfObjectBounds.bottom + pdfObjectBounds.Height / 2f, false);
					pdfTextObject3.Matrix = fs_MATRIX4;
					pdfWatermarkAnnotation2.NormalAppearance.Add(pdfTextObject3);
				}
				pdfWatermarkAnnotation2.GenerateAppearance(AppearanceStreamModes.Normal);
				if (p.Annots == null)
				{
					p.CreateAnnotations();
				}
				p.Annots.Add(pdfWatermarkAnnotation2);
				pdfWatermarkAnnotation2.NormalAppearance.Dispose();
				pdfWatermarkAnnotation2.Dispose();
			};
		}

		// Token: 0x06001B3F RID: 6975 RVA: 0x0006E1B0 File Offset: 0x0006C3B0
		private Action<PdfPage> CreateImageWatermarkFunc(WatermarkParam watermarkParam, WatermarkImageModel imageModel)
		{
			WriteableBitmap bitmap = null;
			if (((imageModel != null) ? imageModel.WatermarkImageSource : null) != null)
			{
				if (imageModel.WatermarkImageSource.Format == PixelFormats.Bgra32)
				{
					bitmap = new WriteableBitmap(imageModel.WatermarkImageSource);
				}
				else
				{
					FormatConvertedBitmap formatConvertedBitmap = new FormatConvertedBitmap(imageModel.WatermarkImageSource, PixelFormats.Bgra32, null, 0.0);
					bitmap = new WriteableBitmap(formatConvertedBitmap);
				}
			}
			Dictionary<global::System.ValueTuple<int, int, float>, WriteableBitmap> bmpCache = new Dictionary<global::System.ValueTuple<int, int, float>, WriteableBitmap>();
			if (watermarkParam.IsTile)
			{
				return delegate(PdfPage p)
				{
					float num;
					float num2;
					p.GetEffectiveSize(PageRotate.Normal, false).Deconstruct(out num, out num2);
					float num3 = num;
					float num4 = num2;
					global::System.Windows.Size size = new global::System.Windows.Size((double)num3, (double)num4);
					float num5 = WatermarkEditWin.<CreateImageWatermarkFunc>g__GetPageDpi|18_1(p);
					WriteableBitmap writeableBitmap = base.<CreateImageWatermarkFunc>g__GetScaledBitmap|3(size, num5, bitmap, watermarkParam.Opacity);
					FS_MATRIX fs_MATRIX = new FS_MATRIX();
					fs_MATRIX.SetIdentity();
					fs_MATRIX.Rotate((float)((double)watermarkParam.Rotation * 3.1415926535897931 / 180.0), false);
					FS_RECTF fs_RECTF = new FS_RECTF(0f, (float)writeableBitmap.PixelHeight, (float)writeableBitmap.PixelWidth, 0f);
					fs_MATRIX.TransformRect(ref fs_RECTF);
					int num6 = (int)Math.Ceiling((size.Width + 50.0) / (double)(fs_RECTF.Width + 50f));
					int num7 = (int)Math.Ceiling((size.Height + 50.0) / (double)(fs_RECTF.Height + 50f));
					PdfWatermarkAnnotation pdfWatermarkAnnotation = new PdfWatermarkAnnotation(p);
					pdfWatermarkAnnotation.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
					pdfWatermarkAnnotation.Flags |= AnnotationFlags.Print;
					pdfWatermarkAnnotation.CreateEmptyAppearance(AppearanceStreamModes.Normal);
					for (int i = 0; i < num7; i++)
					{
						double num8 = size.Height - (double)((fs_RECTF.Height + 50f) * (float)i);
						for (int j = 0; j < num6; j++)
						{
							float num9 = (fs_RECTF.Width + 50f) * (float)j;
							FS_RECTF fs_RECTF2 = new FS_RECTF((double)num9, num8, (double)(num9 + fs_RECTF.Width), num8 - (double)fs_RECTF.Height);
							using (PdfBitmap pdfBitmap = new PdfBitmap(writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, true, false))
							{
								int num10 = pdfBitmap.Stride * pdfBitmap.Height;
								writeableBitmap.CopyPixels(new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight), pdfBitmap.Buffer, num10, pdfBitmap.Stride);
								PdfImageObject pdfImageObject = PdfImageObject.Create(p.Document, pdfBitmap, 0f, 0f);
								FS_MATRIX fs_MATRIX2 = new FS_MATRIX();
								fs_MATRIX2.SetIdentity();
								fs_MATRIX2.Scale((float)pdfBitmap.Width, (float)pdfBitmap.Height, false);
								fs_MATRIX2.Translate(-pdfBitmap.Width / 2, -pdfBitmap.Height / 2, false);
								fs_MATRIX2.Rotate((float)((double)watermarkParam.Rotation * 3.1415926535897931 / 180.0), false);
								fs_MATRIX2.Translate(fs_RECTF2.left + fs_RECTF2.Width / 2f, fs_RECTF2.bottom + fs_RECTF2.Height / 2f, false);
								pdfImageObject.Matrix = fs_MATRIX2;
								pdfWatermarkAnnotation.NormalAppearance.Add(pdfImageObject);
							}
						}
					}
					pdfWatermarkAnnotation.GenerateAppearance(AppearanceStreamModes.Normal);
					if (p.Annots == null)
					{
						p.CreateAnnotations();
					}
					p.Annots.Add(pdfWatermarkAnnotation);
					pdfWatermarkAnnotation.NormalAppearance.Dispose();
					pdfWatermarkAnnotation.Dispose();
					pdfWatermarkAnnotation = null;
				};
			}
			return delegate(PdfPage p)
			{
				float num11;
				float num12;
				p.GetEffectiveSize(PageRotate.Normal, false).Deconstruct(out num11, out num12);
				float num13 = num11;
				float num14 = num12;
				global::System.Windows.Size size2 = new global::System.Windows.Size((double)num13, (double)num14);
				float num15 = WatermarkEditWin.<CreateImageWatermarkFunc>g__GetPageDpi|18_1(p);
				WriteableBitmap writeableBitmap2 = base.<CreateImageWatermarkFunc>g__GetScaledBitmap|3(size2, num15, bitmap, watermarkParam.Opacity);
				FS_MATRIX fs_MATRIX3 = new FS_MATRIX();
				fs_MATRIX3.SetIdentity();
				fs_MATRIX3.Rotate((float)((double)watermarkParam.Rotation * 3.1415926535897931 / 180.0), false);
				FS_RECTF fs_RECTF3 = new FS_RECTF(0f, (float)writeableBitmap2.PixelHeight, (float)writeableBitmap2.PixelWidth, 0f);
				fs_MATRIX3.TransformRect(ref fs_RECTF3);
				FS_RECTF pdfObjectBounds = this.GetPdfObjectBounds((double)fs_RECTF3.Width, (double)fs_RECTF3.Height, watermarkParam.Alignment, size2.Width, size2.Height);
				using (PdfBitmap pdfBitmap2 = new PdfBitmap(writeableBitmap2.PixelWidth, writeableBitmap2.PixelHeight, true, false))
				{
					int num16 = pdfBitmap2.Stride * pdfBitmap2.Height;
					writeableBitmap2.CopyPixels(new Int32Rect(0, 0, writeableBitmap2.PixelWidth, writeableBitmap2.PixelHeight), pdfBitmap2.Buffer, num16, pdfBitmap2.Stride);
					PdfWatermarkAnnotation pdfWatermarkAnnotation2 = new PdfWatermarkAnnotation(p);
					pdfWatermarkAnnotation2.ModificationDate = DateTimeOffset.Now.ToModificationDateString();
					pdfWatermarkAnnotation2.Flags |= AnnotationFlags.Print;
					pdfWatermarkAnnotation2.CreateEmptyAppearance(AppearanceStreamModes.Normal);
					PdfImageObject pdfImageObject2 = PdfImageObject.Create(p.Document, pdfBitmap2, 0f, 0f);
					FS_MATRIX fs_MATRIX4 = new FS_MATRIX();
					fs_MATRIX4.SetIdentity();
					fs_MATRIX4.Scale((float)pdfBitmap2.Width, (float)pdfBitmap2.Height, false);
					fs_MATRIX4.Translate(-pdfBitmap2.Width / 2, -pdfBitmap2.Height / 2, false);
					fs_MATRIX4.Rotate((float)((double)watermarkParam.Rotation * 3.1415926535897931 / 180.0), false);
					fs_MATRIX4.Translate(pdfObjectBounds.left + pdfObjectBounds.Width / 2f, pdfObjectBounds.bottom + pdfObjectBounds.Height / 2f, false);
					pdfImageObject2.Matrix = fs_MATRIX4;
					pdfWatermarkAnnotation2.NormalAppearance.Add(pdfImageObject2);
					pdfWatermarkAnnotation2.GenerateAppearance(AppearanceStreamModes.Normal);
					if (p.Annots == null)
					{
						p.CreateAnnotations();
					}
					p.Annots.Add(pdfWatermarkAnnotation2);
					pdfWatermarkAnnotation2.NormalAppearance.Dispose();
					pdfWatermarkAnnotation2.Dispose();
				}
			};
		}

		// Token: 0x06001B40 RID: 6976 RVA: 0x0006E268 File Offset: 0x0006C468
		private void SeTextObjectLocation(PdfTextObject text, PdfContentAlignment alignment, double pageWidth, double pageHeight)
		{
			float width = text.BoundingBox.Width;
			float height = text.BoundingBox.Height;
			if (alignment <= PdfContentAlignment.MiddleCenter)
			{
				switch (alignment)
				{
				case PdfContentAlignment.TopLeft:
					text.Location = new FS_POINTF(5.0, pageHeight - (double)height);
					return;
				case PdfContentAlignment.TopCenter:
					text.Location = new FS_POINTF(pageWidth / 2.0 - (double)(width / 2f), pageHeight - (double)height);
					return;
				case (PdfContentAlignment)3:
					break;
				case PdfContentAlignment.TopRight:
					text.Location = new FS_POINTF(pageWidth - (double)width - 5.0, pageHeight - (double)height);
					break;
				default:
					if (alignment == PdfContentAlignment.MiddleLeft)
					{
						text.Location = new FS_POINTF(5.0, pageHeight / 2.0 - (double)(height / 2f));
						return;
					}
					if (alignment != PdfContentAlignment.MiddleCenter)
					{
						return;
					}
					text.Location = new FS_POINTF(pageWidth / 2.0 - (double)(width / 2f), pageHeight / 2.0 - (double)(height / 2f));
					return;
				}
				return;
			}
			if (alignment <= PdfContentAlignment.BottomLeft)
			{
				if (alignment == PdfContentAlignment.MiddleRight)
				{
					text.Location = new FS_POINTF(pageWidth - (double)width, pageHeight / 2.0 - (double)(height / 2f));
					return;
				}
				if (alignment != PdfContentAlignment.BottomLeft)
				{
					return;
				}
				text.Location = new FS_POINTF(5f, 0f);
				return;
			}
			else
			{
				if (alignment == PdfContentAlignment.BottomCenter)
				{
					text.Location = new FS_POINTF(pageWidth / 2.0 - (double)(width / 2f), 0.0);
					return;
				}
				if (alignment != PdfContentAlignment.BottomRight)
				{
					return;
				}
				text.Location = new FS_POINTF(pageWidth - (double)width, 0.0);
				return;
			}
		}

		// Token: 0x06001B41 RID: 6977 RVA: 0x0006E424 File Offset: 0x0006C624
		private FS_RECTF GetPdfObjectBounds(double textWidth, double textHeight, PdfContentAlignment alignment, double pageWidth, double pageHeight)
		{
			FS_POINTF fs_POINTF = default(FS_POINTF);
			if (alignment <= PdfContentAlignment.MiddleCenter)
			{
				switch (alignment)
				{
				case PdfContentAlignment.TopLeft:
					fs_POINTF = new FS_POINTF(5.0, pageHeight - textHeight);
					break;
				case PdfContentAlignment.TopCenter:
					fs_POINTF = new FS_POINTF(pageWidth / 2.0 - textWidth / 2.0 + 8.0, pageHeight - textHeight);
					break;
				case (PdfContentAlignment)3:
					break;
				case PdfContentAlignment.TopRight:
					fs_POINTF = new FS_POINTF(pageWidth - textWidth - 5.0, pageHeight - textHeight);
					break;
				default:
					if (alignment != PdfContentAlignment.MiddleLeft)
					{
						if (alignment == PdfContentAlignment.MiddleCenter)
						{
							fs_POINTF = new FS_POINTF(pageWidth / 2.0 - textWidth / 2.0 + 8.0, pageHeight / 2.0 - textHeight / 2.0);
						}
					}
					else
					{
						fs_POINTF = new FS_POINTF(5.0, pageHeight / 2.0 - textHeight / 2.0);
					}
					break;
				}
			}
			else if (alignment <= PdfContentAlignment.BottomLeft)
			{
				if (alignment != PdfContentAlignment.MiddleRight)
				{
					if (alignment == PdfContentAlignment.BottomLeft)
					{
						fs_POINTF = new FS_POINTF(5f, 0f);
					}
				}
				else
				{
					fs_POINTF = new FS_POINTF(pageWidth - textWidth, pageHeight / 2.0 - textHeight / 2.0);
				}
			}
			else if (alignment != PdfContentAlignment.BottomCenter)
			{
				if (alignment == PdfContentAlignment.BottomRight)
				{
					fs_POINTF = new FS_POINTF(pageWidth - textWidth, 0.0);
				}
			}
			else
			{
				fs_POINTF = new FS_POINTF(pageWidth / 2.0 - textWidth / 2.0 + 8.0, 0.0);
			}
			return new FS_RECTF((double)fs_POINTF.X, (double)fs_POINTF.Y + textHeight, (double)fs_POINTF.X + textWidth, (double)fs_POINTF.Y);
		}

		// Token: 0x06001B42 RID: 6978 RVA: 0x0006E624 File Offset: 0x0006C824
		private FS_POINTF SeImgObjectLocation(PdfImageObject img, PdfContentAlignment alignment, double pageWidth, double pageHeight, float scale)
		{
			float num = img.BoundingBox.Width * scale;
			float num2 = img.BoundingBox.Height * scale;
			FS_POINTF fs_POINTF = default(FS_POINTF);
			if (alignment <= PdfContentAlignment.MiddleCenter)
			{
				switch (alignment)
				{
				case PdfContentAlignment.TopLeft:
					fs_POINTF = new FS_POINTF(5.0, pageHeight - (double)num2);
					break;
				case PdfContentAlignment.TopCenter:
					fs_POINTF = new FS_POINTF(pageWidth / 2.0 - (double)(num / 2f), pageHeight - (double)num2);
					break;
				case (PdfContentAlignment)3:
					break;
				case PdfContentAlignment.TopRight:
					fs_POINTF = new FS_POINTF(pageWidth - (double)num - 5.0, pageHeight - (double)num2);
					break;
				default:
					if (alignment != PdfContentAlignment.MiddleLeft)
					{
						if (alignment == PdfContentAlignment.MiddleCenter)
						{
							fs_POINTF = new FS_POINTF(pageWidth / 2.0 - (double)(num / 2f), pageHeight / 2.0 - (double)(num2 / 2f));
						}
					}
					else
					{
						fs_POINTF = new FS_POINTF(5.0, pageHeight / 2.0 - (double)(num2 / 2f));
					}
					break;
				}
			}
			else if (alignment <= PdfContentAlignment.BottomLeft)
			{
				if (alignment != PdfContentAlignment.MiddleRight)
				{
					if (alignment == PdfContentAlignment.BottomLeft)
					{
						fs_POINTF = new FS_POINTF(5f, num / 2f);
					}
				}
				else
				{
					fs_POINTF = new FS_POINTF(pageWidth - (double)num, pageHeight / 2.0 - (double)(num2 / 2f));
				}
			}
			else if (alignment != PdfContentAlignment.BottomCenter)
			{
				if (alignment == PdfContentAlignment.BottomRight)
				{
					fs_POINTF = new FS_POINTF(pageWidth - (double)num, 0.0);
				}
			}
			else
			{
				fs_POINTF = new FS_POINTF(pageWidth / 2.0 - (double)(num / 2f), 0.0);
			}
			return fs_POINTF;
		}

		// Token: 0x06001B43 RID: 6979 RVA: 0x0006E7F0 File Offset: 0x0006C9F0
		private void SetImageRotate(PdfImageObject img, float rotation, float scale, bool isTile, float x, float y)
		{
			FS_MATRIX fs_MATRIX = img.Matrix;
			if (fs_MATRIX == null)
			{
				fs_MATRIX = new FS_MATRIX();
			}
			float num = (float)img.Bitmap.Width * scale;
			float num2 = (float)img.Bitmap.Height * scale;
			float num3 = rotation * 3.14f / 180f;
			fs_MATRIX.SetIdentity();
			fs_MATRIX.Scale(num, num2, false);
			fs_MATRIX.Translate(-num / 2f, -num2 / 2f, false);
			fs_MATRIX.Rotate(num3, false);
			fs_MATRIX.Translate(num / 2f, num2 / 2f, false);
			fs_MATRIX.Translate(x, y, false);
			img.Matrix = fs_MATRIX;
		}

		// Token: 0x06001B44 RID: 6980 RVA: 0x0006E890 File Offset: 0x0006CA90
		private void SetImageObjectSoftMask(PdfImageObject img, float Opacity)
		{
			PdfTypeStream pdfTypeStream = null;
			PdfTypeBase pdfTypeBase;
			if (img.SoftMask != null && img.SoftMask.Is<PdfTypeStream>())
			{
				pdfTypeStream = img.SoftMask.As<PdfTypeStream>(true);
			}
			else if (img.Stream.Dictionary != null && img.Stream.Dictionary.TryGetValue("SMask", out pdfTypeBase) && pdfTypeBase != null && pdfTypeBase.Is<PdfTypeStream>())
			{
				pdfTypeStream = pdfTypeBase.As<PdfTypeStream>(true);
			}
			byte[] array = new byte[pdfTypeStream.Content.Length];
			for (int i = 0; i < pdfTypeStream.Content.Length; i++)
			{
				array[i] = (byte)((float)pdfTypeStream.Content[i] * Opacity);
			}
			pdfTypeStream.SetContent(array, false);
			img.SoftMask = pdfTypeStream;
		}

		// Token: 0x06001B45 RID: 6981 RVA: 0x0006E93C File Offset: 0x0006CB3C
		private WatermarkParam GetWatermarkParam(PdfDocument doc)
		{
			WatermarkParam watermarkParam = new WatermarkParam();
			UIElementAligent uielementAligent = this.alignmentSelector;
			watermarkParam.Alignment = uielementAligent.Alignment;
			watermarkParam.Opacity = 0.5f;
			float num;
			if (float.TryParse((this.opacityCB.SelectedItem as ComboBoxItem).Tag.ToString(), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num))
			{
				watermarkParam.Opacity = num;
			}
			FrameworkElement frameworkElement = this.rotateComboBox.SelectedItem as ComboBoxItem;
			watermarkParam.Rotation = 0f;
			float num2;
			if (float.TryParse(frameworkElement.Tag.ToString(), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out num2))
			{
				watermarkParam.Rotation = num2;
			}
			watermarkParam.Scale = 1f;
			watermarkParam.Vdistance = 0f;
			watermarkParam.Hdistance = 0f;
			watermarkParam.IsTile = this.checkTile.IsChecked.Value;
			watermarkParam.PageRange = this.GetImportPageRange(doc);
			return watermarkParam;
		}

		// Token: 0x06001B46 RID: 6982 RVA: 0x0006EA29 File Offset: 0x0006CC29
		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			base.DialogResult = new bool?(false);
		}

		// Token: 0x06001B47 RID: 6983 RVA: 0x0006EA38 File Offset: 0x0006CC38
		private void btnOpenFile_Click(object sender, RoutedEventArgs e)
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "All Image Files|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.png;*.tif;*.tiff|Windows Bitmap(*.bmp)|*.bmp|Windows Icon(*.ico)|*.ico|Graphics Interchange Format (*.gif)|(*.gif)|JPEG File Interchange Format (*.jpg)|*.jpg;*.jpeg|Portable Network Graphics (*.png)|*.png|Tag Image File Format (*.tif)|*.tif;*.tiff",
				ShowReadOnly = false,
				ReadOnlyChecked = true
			};
			bool? flag = openFileDialog.ShowDialog();
			this.ImageModel = new WatermarkImageModel();
			if (flag.GetValueOrDefault() && !string.IsNullOrEmpty(openFileDialog.FileName))
			{
				this.ImageModel.ImageFilePath = openFileDialog.FileName;
				this.txtWatermarkfile.Text = openFileDialog.FileName;
				requiredService.AnnotationToolbar.StampImgFileOkTime = DateTime.Now;
				return;
			}
			this.txtWatermarkfile.Text = string.Empty;
			this.ImageModel = null;
		}

		// Token: 0x06001B48 RID: 6984 RVA: 0x0006EAE4 File Offset: 0x0006CCE4
		private int[] GetImportPageRange(PdfDocument doc)
		{
			if (this.CurrentPageRadioButton.IsChecked.GetValueOrDefault())
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				return new int[] { requiredService.Document.Pages.CurrentIndex };
			}
			int[] array;
			if (this.AllPagesRadioButton.IsChecked.GetValueOrDefault())
			{
				if (doc.Pages.Count == 0)
				{
					return null;
				}
				array = Enumerable.Range(0, doc.Pages.Count).ToArray<int>();
			}
			else
			{
				array = this.RangeBox.PageIndexes.ToArray<int>();
			}
			if (array.Any((int c) => c < 0 || c >= doc.Pages.Count))
			{
				return null;
			}
			if (this.applyToComboBox.SelectedIndex == 1)
			{
				array = array.Where((int c) => c % 2 == 0).ToArray<int>();
			}
			else if (this.applyToComboBox.SelectedIndex == 2)
			{
				array = array.Where((int c) => c % 2 == 1).ToArray<int>();
			}
			if (array.Length == 0)
			{
				return null;
			}
			return array;
		}

		// Token: 0x06001B49 RID: 6985 RVA: 0x0006EC23 File Offset: 0x0006CE23
		private void btn_PopColor_Click(object sender, RoutedEventArgs e)
		{
			this.PopColor.IsOpen = true;
		}

		// Token: 0x06001B4A RID: 6986 RVA: 0x0006EC31 File Offset: 0x0006CE31
		private void CurrentPageRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			this.UpdateApplyToComboBoxEnabledState();
		}

		// Token: 0x06001B4B RID: 6987 RVA: 0x0006EC39 File Offset: 0x0006CE39
		private void CurrentPageRadioButton_Unchecked(object sender, RoutedEventArgs e)
		{
			this.UpdateApplyToComboBoxEnabledState();
		}

		// Token: 0x06001B4C RID: 6988 RVA: 0x0006EC44 File Offset: 0x0006CE44
		private void UpdateApplyToComboBoxEnabledState()
		{
			if (this.applyToComboBox != null && this.CurrentPageRadioButton != null)
			{
				this.applyToComboBox.IsEnabled = !this.CurrentPageRadioButton.IsChecked.GetValueOrDefault();
			}
		}

		// Token: 0x06001B52 RID: 6994 RVA: 0x0006EEFC File Offset: 0x0006D0FC
		[CompilerGenerated]
		internal static WriteableBitmap <CreateImageWatermarkFunc>g__ResizeBitmap|18_0(WriteableBitmap _source, int _newWidth, int _newHeight, float _opacity)
		{
			if (_source.Width == (double)_newWidth && _source.Height == (double)_newHeight && _opacity == 1f)
			{
				return _source;
			}
			WriteableBitmap writeableBitmap;
			using (Bitmap bitmap = new Bitmap(_source.PixelWidth, _source.PixelHeight, global::System.Drawing.Imaging.PixelFormat.Format32bppArgb))
			{
				BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, _source.PixelWidth, _source.PixelHeight), ImageLockMode.WriteOnly, global::System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				_source.CopyPixels(new Int32Rect(0, 0, _source.PixelWidth, _source.PixelHeight), bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);
				bitmap.UnlockBits(bitmapData);
				BitmapSizeOptions bitmapSizeOptions = BitmapSizeOptions.FromWidthAndHeight(_newWidth, _newHeight);
				Int32Rect int32Rect = new Int32Rect(0, 0, bitmap.Width, bitmap.Height);
				BitmapSource bitmapSource = null;
				if (_opacity != 1f)
				{
					using (Bitmap bitmap2 = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat))
					{
						float num = Math.Max(0f, Math.Min(1f, _opacity));
						ColorMatrix colorMatrix = new ColorMatrix();
						colorMatrix.Matrix33 = num;
						using (Graphics graphics = Graphics.FromImage(bitmap2))
						{
							using (ImageAttributes imageAttributes = new ImageAttributes())
							{
								imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
								graphics.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttributes);
							}
						}
						IntPtr hbitmap = bitmap2.GetHbitmap();
						try
						{
							bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hbitmap, IntPtr.Zero, int32Rect, bitmapSizeOptions);
							goto IL_01D5;
						}
						finally
						{
							try
							{
								if (hbitmap != IntPtr.Zero)
								{
									DrawUtils.DeleteObject(hbitmap);
								}
							}
							catch
							{
							}
						}
					}
				}
				IntPtr hbitmap2 = bitmap.GetHbitmap();
				try
				{
					bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hbitmap2, IntPtr.Zero, int32Rect, bitmapSizeOptions);
				}
				finally
				{
					try
					{
						if (hbitmap2 != IntPtr.Zero)
						{
							DrawUtils.DeleteObject(hbitmap2);
						}
					}
					catch
					{
					}
				}
				IL_01D5:
				writeableBitmap = new WriteableBitmap(bitmapSource);
			}
			return writeableBitmap;
		}

		// Token: 0x06001B53 RID: 6995 RVA: 0x0006F1BC File Offset: 0x0006D3BC
		[CompilerGenerated]
		internal static float <CreateImageWatermarkFunc>g__GetPageDpi|18_1(PdfPage page)
		{
			float num = 72f;
			if (page.Dictionary.ContainsKey("UserUnit"))
			{
				num = page.Dictionary["UserUnit"].As<PdfTypeNumber>(true).FloatValue * 72f;
			}
			return num;
		}

		// Token: 0x06001B54 RID: 6996 RVA: 0x0006F204 File Offset: 0x0006D404
		[CompilerGenerated]
		internal static global::System.Windows.Size <CreateImageWatermarkFunc>g__GetBitmapPageSize|18_2(double bitmapWidth, double bitmapHeight, FS_RECTF pageBounds)
		{
			global::System.Windows.Size size = new global::System.Windows.Size(bitmapWidth * 96.0 / 72.0, bitmapHeight * 96.0 / 72.0);
			if (size.Width > (double)(pageBounds.Width / 2f) || size.Height > (double)(pageBounds.Height / 2f))
			{
				double num = size.Width / (double)(pageBounds.Width / 2f);
				double num2 = size.Height / (double)(pageBounds.Height / 2f);
				double num3 = Math.Max(num, num2);
				size = new global::System.Windows.Size(size.Width / num3, size.Height / num3);
			}
			else if (size.Width < 10.0 && size.Height < 10.0)
			{
				double num4 = size.Width / 10.0;
				double num5 = size.Height / 10.0;
				double num6 = Math.Min(num4, num5);
				size = new global::System.Windows.Size(size.Width / num6, size.Height / num6);
			}
			return size;
		}

		// Token: 0x04000996 RID: 2454
		private WatermarkImageModel ImageModel;

		// Token: 0x04000997 RID: 2455
		private WatermarkTextModel TextModel;

		// Token: 0x04000998 RID: 2456
		private WatermarkParam watermarkParam;

		// Token: 0x04000999 RID: 2457
		private const float HTileSpan = 50f;

		// Token: 0x0400099A RID: 2458
		private const float VTileSpan = 50f;

		// Token: 0x0400099B RID: 2459
		private string SelectedFontground = "#FF000000";

		// Token: 0x0400099C RID: 2460
		public static readonly DependencyProperty CustomRotateProperty = DependencyProperty.Register("CustomRotate", typeof(string), typeof(WatermarkEditWin), new PropertyMetadata("0"));

		// Token: 0x0400099D RID: 2461
		private readonly List<string> _colorPresetList = new List<string>
		{
			"#f44336", "#e91e63", "#9c27b0", "#673ab7", "#3f51b5", "#2196f3", "#03a9f4", "#00bcd4", "#009688", "#4caf50",
			"#8bc34a", "#cddc39", "#ffeb3b", "#ffc107", "#ff9800", "#ff5722", "#795548", "#000000"
		};
	}
}
