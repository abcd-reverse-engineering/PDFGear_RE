using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using CommonLib.Common;
using CommonLib.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Controls.Printer;
using pdfeditor.Properties;
using pdfeditor.Utils;
using pdfeditor.ViewModels;
using PDFKit;
using PDFKit.Utils.PageHeaderFooters;

namespace pdfeditor.Controls.PageEditor
{
	// Token: 0x02000250 RID: 592
	public partial class PageResizeWindow : Window
	{
		// Token: 0x17000B2B RID: 2859
		// (get) Token: 0x06002235 RID: 8757 RVA: 0x0009E67C File Offset: 0x0009C87C
		// (set) Token: 0x06002236 RID: 8758 RVA: 0x0009E684 File Offset: 0x0009C884
		private PdfDocument Document { get; set; }

		// Token: 0x06002237 RID: 8759 RVA: 0x0009E68D File Offset: 0x0009C88D
		public PageResizeWindow()
		{
			this.InitializeComponent();
		}

		// Token: 0x06002238 RID: 8760 RVA: 0x0009E6B4 File Offset: 0x0009C8B4
		public PageResizeWindow(PdfDocument pdfDocument, int pageNum, int[] pages = null, Source source = Source.Default)
		{
			this.PageIndex = pageNum;
			this.CurrentPageindex = pageNum;
			this.InitializeComponent();
			this.Document = pdfDocument;
			this.PageNumber.Text = this.Document.Pages.Count.ToString();
			if (pages != null)
			{
				if (source == Source.Viewer)
				{
					this.CurrentPageBtn.IsChecked = new bool?(true);
					this.CustomTextBox.Text = "";
				}
				else
				{
					this.CustomTextBox.Text = pages.ConvertToRange();
					this.GetSeletedPageItem(pages);
				}
				this.CurrentPage.Text = string.Format(" ({0} {1})", pdfeditor.Properties.Resources.PageResizeWinPageNumber, pages[0] + 1);
				this.PageindexNumbox.Value = (double)(pages[0] + 1);
				this.PART_VerticalScrollBar.Value = (double)(pages[0] + 1);
			}
			else
			{
				this.AllPageBtn.IsChecked = new bool?(true);
				this.CustomTextBox.Text = "";
				this.CurrentPage.Text = string.Format(" ({0} {1})", pdfeditor.Properties.Resources.PageResizeWinPageNumber, pageNum + 1);
				this.PageindexNumbox.Value = (double)(pageNum + 1);
				this.PART_VerticalScrollBar.Value = (double)(pageNum + 1);
			}
			this.PageindexNumbox.Maximum = (double)this.Document.Pages.Count;
			this.PageCount.Text = this.Document.Pages.Count.ToString();
			string text = Math.Abs(Convert.ToInt32(PageHeaderFooterUtils.PdfPointToCm((double)pdfDocument.Pages[pageNum].Height) * 10.0)).ToString("F0");
			string text2 = Math.Abs(Convert.ToInt32(PageHeaderFooterUtils.PdfPointToCm((double)pdfDocument.Pages[pageNum].Width) * 10.0)).ToString("F0");
			this.Originalwidth.Text = text2 + "mm";
			this.Originalheight.Text = text + "mm";
			this.Resizewidth.Text = text2 + "mm";
			this.Resizeheight.Text = text + "mm";
			this.widthNumBox.Value = (double)Math.Abs(Convert.ToInt32(PageHeaderFooterUtils.PdfPointToCm((double)pdfDocument.Pages[pageNum].Width) * 10.0));
			this.heightNumbox.Value = (double)Math.Abs(Convert.ToInt32(PageHeaderFooterUtils.PdfPointToCm((double)pdfDocument.Pages[pageNum].Height) * 10.0));
			this.widthNumBox.Text = "";
			this.heightNumbox.Text = "";
			this.widthNumBox.Maximum = 9990.0;
			this.heightNumbox.Maximum = 9990.0;
			this.PART_VerticalScrollBar.Maximum = (double)this.Document.Pages.Count;
			if (pageNum <= 0)
			{
				this.FirstPage.IsEnabled = false;
				this.BeforePage.IsEnabled = false;
			}
			if (pageNum >= this.Document.Pages.Count - 1)
			{
				this.NextPage.IsEnabled = false;
				this.EndPage.IsEnabled = false;
			}
			this.UpdatePaperSize();
			base.Closing += this.PageResizeWindow_Closing;
		}

		// Token: 0x06002239 RID: 8761 RVA: 0x0009EA4C File Offset: 0x0009CC4C
		private void GetSeletedPageItem(int[] pages)
		{
			if (pages.Length == this.Document.Pages.Count)
			{
				this.AllPageBtn.IsChecked = new bool?(true);
				return;
			}
			int num = pages[0] % 2;
			bool flag;
			if (num == 1 && this.Document.Pages.Count % 2 == 1)
			{
				flag = (double)pages.Count<int>() >= (double)(this.Document.Pages.Count - 1) / 2.0;
			}
			else
			{
				flag = (double)pages.Count<int>() >= (double)this.Document.Pages.Count / 2.0;
			}
			if (num == 0 && flag)
			{
				bool flag2 = true;
				for (int i = 0; i < pages.Length; i++)
				{
					if (pages[i] % 2 == 1)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					this.AllPageBtn.IsChecked = new bool?(true);
					this.SubsetComboBox.SelectedIndex = 2;
					this.CustomTextBox.Text = "";
					return;
				}
			}
			if (num == 1 && flag)
			{
				bool flag3 = true;
				for (int i = 0; i < pages.Length; i++)
				{
					if (pages[i] % 2 == 0)
					{
						flag3 = false;
						break;
					}
				}
				if (flag3)
				{
					this.AllPageBtn.IsChecked = new bool?(true);
					this.SubsetComboBox.SelectedIndex = 1;
					this.CustomTextBox.Text = "";
					return;
				}
			}
			this.CustomBtn.IsChecked = new bool?(true);
		}

		// Token: 0x0600223A RID: 8762 RVA: 0x0009EBC0 File Offset: 0x0009CDC0
		private void PageResizeWindow_Closing(object sender, CancelEventArgs e)
		{
			this.widthNumBox.ValueChanged -= this.widthNumBox_ValueChanged;
			this.heightNumbox.ValueChanged -= this.widthNumBox_ValueChanged;
			this.Document.Dispose();
		}

		// Token: 0x0600223B RID: 8763 RVA: 0x0009EBFC File Offset: 0x0009CDFC
		private void setResizeImage()
		{
			float num = Math.Abs(this.Document.Pages[this.PageIndex].Width);
			float num2 = Math.Abs(this.Document.Pages[this.PageIndex].Height);
			float num3 = 310f;
			float num4 = 220f;
			if (num3 > num2)
			{
				num3 = num2;
			}
			if (num4 > num)
			{
				num4 = num;
			}
			if (num > num2)
			{
				float num5 = num2 / num;
				this.cropBoder.Height = 330.0;
				this.Cropedimage.Height = (double)(num4 * num5);
			}
			else
			{
				float num6 = num / num2;
				if (num6 * num3 > num4)
				{
					this.cropBoder.Height = 330.0;
					this.Cropedimage.Height = (double)(num4 / num6);
				}
				else
				{
					this.Cropedimage.Height = (double)num3;
					this.cropBoder.Height = 330.0;
					this.Cropedimage.Width = (double)(num3 * num6);
					this.cropBoder.Width = (double)(330f * num6 + 30f);
				}
			}
			if (num > 1f && num2 > 1f)
			{
				if (num > 660f)
				{
					num2 *= 660f / num;
					num = 660f;
				}
				if (num2 > 930f)
				{
					num *= 930f / num2;
					num2 = 930f;
				}
				if (num > 1f && num2 > 1f)
				{
					PdfBitmap pdfBitmap = new PdfBitmap((int)num, (int)num2, BitmapFormats.FXDIB_Argb);
					pdfBitmap.FillRectEx(0, 0, pdfBitmap.Width, pdfBitmap.Height, -1);
					PageRotate pageRotate = PageRotate.Normal;
					Pdfium.FPDF_RenderPageBitmap(pdfBitmap.Handle, this.Document.Pages[this.PageIndex].Handle, 0, 0, pdfBitmap.Width, pdfBitmap.Height, pageRotate, RenderFlags.FPDF_ANNOT);
					this.Cropedimage.Source = PageResizeWindow.GetBitmapImage((Bitmap)pdfBitmap.Image);
					this.Cropedimage.VerticalAlignment = VerticalAlignment.Center;
				}
			}
		}

		// Token: 0x0600223C RID: 8764 RVA: 0x0009EDF8 File Offset: 0x0009CFF8
		private static BitmapImage GetBitmapImage(Bitmap bitmap)
		{
			BitmapImage bitmapImage2;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				bitmap.Save(memoryStream, ImageFormat.Png);
				memoryStream.Position = 0L;
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.StreamSource = memoryStream;
				bitmapImage.EndInit();
				bitmapImage2 = bitmapImage;
			}
			return bitmapImage2;
		}

		// Token: 0x0600223D RID: 8765 RVA: 0x0009EE5C File Offset: 0x0009D05C
		private void PageResize(PdfPage page, double width, double height)
		{
			try
			{
				if (page.Dictionary.ContainsKey("CropBox"))
				{
					FS_MATRIX fs_MATRIX = new FS_MATRIX();
					fs_MATRIX.SetIdentity();
					fs_MATRIX.Translate(-page.CropBox.left, -page.CropBox.bottom, false);
					page.TransformAnnots(fs_MATRIX);
					page.TransformWithClip(fs_MATRIX, page.MediaBox);
					page.ReloadPage();
				}
				float num = Math.Abs(page.Height);
				float num2 = Math.Abs(page.Width);
				float num3 = (float)width / num2;
				float num4 = (float)height / num;
				FS_MATRIX fs_MATRIX2 = new FS_MATRIX();
				fs_MATRIX2.SetIdentity();
				float num5;
				if (num3 > num4)
				{
					num5 = num4;
				}
				else
				{
					num5 = num3;
				}
				float num6 = num2 * num5;
				float num7 = num * num5;
				float num8 = ((float)width - num6) / 2f;
				float num9 = ((float)height - num7) / 2f;
				if (page.Rotation == PageRotate.Rotate90 || page.Rotation == PageRotate.Rotate270)
				{
					double num10 = height;
					height = width;
					width = num10;
				}
				page.MediaBox = new FS_RECTF(0.0, height, width, 0.0);
				page.CropBox = page.MediaBox;
				page.TrimBox = page.MediaBox;
				fs_MATRIX2.Scale(num5, num5, false);
				if (page.Rotation == PageRotate.Rotate90 || page.Rotation == PageRotate.Rotate270)
				{
					fs_MATRIX2.Translate(num9, num8, false);
				}
				else
				{
					fs_MATRIX2.Translate(num8, num9, false);
				}
				page.TransformAnnots(fs_MATRIX2);
				page.TransformWithClip(fs_MATRIX2, page.MediaBox);
				page.ReloadPage();
				page.TryRedrawPageAsync(default(CancellationToken));
			}
			catch
			{
			}
		}

		// Token: 0x0600223E RID: 8766 RVA: 0x0009F008 File Offset: 0x0009D208
		private void PageResize(double width, double height)
		{
			this.RestorePageSize();
			PdfPage pdfPage = this.Document.Pages[this.PageIndex];
			float num = Math.Abs(pdfPage.Height);
			float num2 = Math.Abs(pdfPage.Width);
			float num3 = (float)width / num2;
			float num4 = (float)height / num;
			FS_MATRIX fs_MATRIX = new FS_MATRIX();
			fs_MATRIX.SetIdentity();
			float num5;
			if (num3 > num4)
			{
				num5 = num4;
			}
			else
			{
				num5 = num3;
			}
			float num6 = num2 * num5;
			float num7 = num * num5;
			float num8 = ((float)width - num6) / 2f;
			float num9 = ((float)height - num7) / 2f;
			if (pdfPage.Rotation == PageRotate.Rotate90 || pdfPage.Rotation == PageRotate.Rotate270)
			{
				double num10 = height;
				height = width;
				width = num10;
			}
			pdfPage.MediaBox = new FS_RECTF(0.0, height, width, 0.0);
			pdfPage.CropBox = pdfPage.MediaBox;
			pdfPage.TrimBox = pdfPage.MediaBox;
			fs_MATRIX.Scale(num5, num5, false);
			if (pdfPage.Rotation == PageRotate.Rotate90 || pdfPage.Rotation == PageRotate.Rotate270)
			{
				fs_MATRIX.Translate(num9, num8, false);
			}
			else
			{
				fs_MATRIX.Translate(num8, num9, false);
			}
			pdfPage.TransformAnnots(fs_MATRIX);
			pdfPage.TransformWithClip(fs_MATRIX, pdfPage.MediaBox);
			pdfPage.ReloadPage();
			pdfPage.TryRedrawPageAsync(default(CancellationToken));
			this.setResizeImage();
			this.UpdateResizeDate();
		}

		// Token: 0x0600223F RID: 8767 RVA: 0x0009F160 File Offset: 0x0009D360
		private void PageRestroe(PdfPage page, double width, double height)
		{
			try
			{
				float num = Math.Abs(page.Height);
				float num2 = Math.Abs(page.Width);
				page.MediaBox = new FS_RECTF(0.0, height, width, 0.0);
				if (page.Rotation == PageRotate.Rotate90 || page.Rotation == PageRotate.Rotate270)
				{
					double num3 = height;
					height = width;
					width = num3;
				}
				float num4 = num2 / (float)width;
				float num5 = num / (float)height;
				page.CropBox = page.MediaBox;
				page.TrimBox = page.MediaBox;
				FS_MATRIX fs_MATRIX = new FS_MATRIX();
				fs_MATRIX.SetIdentity();
				float num6;
				if (num4 > num5)
				{
					num6 = num5;
				}
				else
				{
					num6 = num4;
				}
				page.CropBox = page.MediaBox;
				page.TrimBox = page.MediaBox;
				float num7 = (float)width * num6;
				float num8 = (float)height * num6;
				float num9 = -(num2 - num7) / 2f;
				float num10 = -(num - num8) / 2f;
				if (page.Rotation == PageRotate.Rotate90 || page.Rotation == PageRotate.Rotate270)
				{
					fs_MATRIX.Translate(num10, num9, false);
				}
				else
				{
					fs_MATRIX.Translate(num9, num10, false);
				}
				fs_MATRIX.Scale(1f / num6, 1f / num6, false);
				page.TransformAnnots(fs_MATRIX);
				page.TransformWithClip(fs_MATRIX, page.MediaBox);
				page.ReloadPage();
				page.TryRedrawPageAsync(default(CancellationToken));
			}
			catch
			{
			}
		}

		// Token: 0x06002240 RID: 8768 RVA: 0x0009F2CC File Offset: 0x0009D4CC
		private void RestorePageSize()
		{
			PdfPage pdfPage = Ioc.Default.GetRequiredService<MainViewModel>().Document.Pages[this.PageIndex];
			PdfPage pdfPage2 = this.Document.Pages[this.PageIndex];
			if (pdfPage2.Width == pdfPage.Width && pdfPage2.Height == pdfPage.Height)
			{
				if (pdfPage.Dictionary.ContainsKey("CropBox"))
				{
					pdfPage2.MediaBox = new FS_RECTF(0f, pdfPage.CropBox.Height, pdfPage.CropBox.Width, 0f);
					FS_MATRIX fs_MATRIX = new FS_MATRIX();
					fs_MATRIX.SetIdentity();
					fs_MATRIX.Translate(-pdfPage2.CropBox.left, -pdfPage2.CropBox.bottom, false);
					pdfPage2.TransformAnnots(fs_MATRIX);
					pdfPage2.TransformWithClip(fs_MATRIX, pdfPage2.MediaBox);
					pdfPage2.ReloadPage();
				}
				else
				{
					pdfPage2.MediaBox = new FS_RECTF(0f, pdfPage.MediaBox.Height, pdfPage.MediaBox.Width, 0f);
				}
				pdfPage2.CropBox = pdfPage2.MediaBox;
				pdfPage2.TrimBox = pdfPage2.MediaBox;
				pdfPage2.ReloadPage();
				return;
			}
			float num = Math.Abs(pdfPage.Width);
			float num2 = Math.Abs(pdfPage.Height);
			float num3 = Math.Abs(pdfPage2.Height);
			float num4 = Math.Abs(pdfPage2.Width);
			float num5 = num4 / num;
			float num6 = num3 / num2;
			FS_MATRIX fs_MATRIX2 = new FS_MATRIX();
			fs_MATRIX2.SetIdentity();
			float num7;
			if (num5 > num6)
			{
				num7 = num6;
			}
			else
			{
				num7 = num5;
			}
			if (pdfPage.Dictionary.ContainsKey("CropBox"))
			{
				FS_MATRIX fs_MATRIX3 = new FS_MATRIX();
				fs_MATRIX3.SetIdentity();
				fs_MATRIX3.Translate(-pdfPage2.CropBox.left, -pdfPage2.CropBox.bottom, false);
				pdfPage2.TransformAnnots(fs_MATRIX3);
				pdfPage2.TransformWithClip(fs_MATRIX3, pdfPage2.MediaBox);
				pdfPage2.ReloadPage();
			}
			float num8 = num * num7;
			float num9 = num2 * num7;
			float num10 = -(num4 - num8) / 2f;
			float num11 = -(num3 - num9) / 2f;
			if (pdfPage2.Rotation == PageRotate.Rotate90 || pdfPage2.Rotation == PageRotate.Rotate270)
			{
				float num12 = num2;
				num2 = num;
				num = num12;
			}
			pdfPage2.MediaBox = new FS_RECTF(0f, num2, num, 0f);
			pdfPage2.CropBox = pdfPage2.MediaBox;
			pdfPage2.TrimBox = pdfPage2.MediaBox;
			if (pdfPage2.Rotation == PageRotate.Rotate90 || pdfPage2.Rotation == PageRotate.Rotate270)
			{
				fs_MATRIX2.Translate(num11, num10, false);
			}
			else
			{
				fs_MATRIX2.Translate(num10, num11, false);
			}
			fs_MATRIX2.Scale(1f / num7, 1f / num7, false);
			pdfPage2.TransformAnnots(fs_MATRIX2);
			pdfPage2.TransformWithClip(fs_MATRIX2, pdfPage2.MediaBox);
			pdfPage2.ReloadPage();
			pdfPage2.TryRedrawPageAsync(default(CancellationToken));
		}

		// Token: 0x06002241 RID: 8769 RVA: 0x0009F5B8 File Offset: 0x0009D7B8
		private void UpdateResizeDate()
		{
			string text = (this.UnitComboBox.SelectedItem as ComboBoxItem).Name.ToString().ToLower();
			float num = Math.Abs(this.Document.Pages[this.PageIndex].Height);
			float num2 = Math.Abs(this.Document.Pages[this.PageIndex].Width);
			if (text == "cm")
			{
				string text2 = PageHeaderFooterUtils.PdfPointToCm((double)num).ToString("F1");
				string text3 = PageHeaderFooterUtils.PdfPointToCm((double)num2).ToString("F1");
				this.Resizewidth.Text = text3 + "cm";
				this.Resizeheight.Text = text2 + "cm";
			}
			if (text == "mm")
			{
				string text4 = (PageHeaderFooterUtils.PdfPointToCm((double)num) * 10.0).ToString("F0");
				string text5 = (PageHeaderFooterUtils.PdfPointToCm((double)num2) * 10.0).ToString("F0");
				this.Resizewidth.Text = text5 + "mm";
				this.Resizeheight.Text = text4 + "mm";
			}
			if (text == "point")
			{
				string text6 = num.ToString("F0");
				string text7 = num2.ToString("F0");
				this.Resizewidth.Text = text7 ?? "";
				this.Resizeheight.Text = text6 ?? "";
			}
			if (text == "pica")
			{
				string text8 = (num / 12f).ToString("F0");
				string text9 = (num2 / 12f).ToString("F0");
				this.Resizewidth.Text = text9 + "pica";
				this.Resizeheight.Text = text8 + "pica";
			}
			if (text == "inch")
			{
				string text10 = (num / 72f).ToString("F0");
				string text11 = (num2 / 72f).ToString("F0");
				this.Resizewidth.Text = text11 + "inch";
				this.Resizeheight.Text = text10 + "inch";
			}
		}

		// Token: 0x06002242 RID: 8770 RVA: 0x0009F830 File Offset: 0x0009DA30
		private void UpdatePaperSize()
		{
			this.paperSizesLst = Pagesize.paperSizes.Select((PaperSize c) => new PaperSizeInfo
			{
				FriendlyName = c.PaperName,
				PaperSize = c
			}).ToList<PaperSizeInfo>();
			this.cboxPaperSize.ItemsSource = this.paperSizesLst;
			this.cboxPaperSize.SelectedItem = this.paperSizesLst.FirstOrDefault((PaperSizeInfo c) => c.FriendlyName == "A4");
		}

		// Token: 0x06002243 RID: 8771 RVA: 0x0009F8B7 File Offset: 0x0009DAB7
		private void SubsetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		// Token: 0x06002244 RID: 8772 RVA: 0x0009F8B9 File Offset: 0x0009DAB9
		private void CustomTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06002245 RID: 8773 RVA: 0x0009F8BB File Offset: 0x0009DABB
		private void PART_VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.PageindexNumbox != null)
			{
				this.PageindexNumbox.Value = e.NewValue;
			}
		}

		// Token: 0x06002246 RID: 8774 RVA: 0x0009F8D6 File Offset: 0x0009DAD6
		private void FirstPage_Click(object sender, RoutedEventArgs e)
		{
			this.PageindexNumbox.Value = 0.0;
		}

		// Token: 0x06002247 RID: 8775 RVA: 0x0009F8EC File Offset: 0x0009DAEC
		private void BeforePage_Click(object sender, RoutedEventArgs e)
		{
			this.PageindexNumbox.Value = (double)this.PageIndex;
		}

		// Token: 0x06002248 RID: 8776 RVA: 0x0009F900 File Offset: 0x0009DB00
		private void NextPage_Click(object sender, RoutedEventArgs e)
		{
			this.PageindexNumbox.Value = (double)(this.PageIndex + 2);
		}

		// Token: 0x06002249 RID: 8777 RVA: 0x0009F916 File Offset: 0x0009DB16
		private void EndPage_Click(object sender, RoutedEventArgs e)
		{
			this.PageindexNumbox.Value = (double)this.Document.Pages.Count;
		}

		// Token: 0x0600224A RID: 8778 RVA: 0x0009F934 File Offset: 0x0009DB34
		private void PageindexNumbox_TextChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			int num = (int)(e.NewValue - 1.0);
			if (num == this.PageIndex)
			{
				return;
			}
			if (this.NextPage == null)
			{
				return;
			}
			if (num < 0)
			{
				this.PageindexNumbox.Text = "0";
				num = 0;
			}
			if (num > this.Document.Pages.Count - 1)
			{
				this.PageindexNumbox.Text = string.Format("{0}", this.Document.Pages.Count - 1);
				num = this.Document.Pages.Count - 1;
			}
			this.FirstPage.IsEnabled = true;
			this.BeforePage.IsEnabled = true;
			this.NextPage.IsEnabled = true;
			this.EndPage.IsEnabled = true;
			if (num <= 0)
			{
				this.FirstPage.IsEnabled = false;
				this.BeforePage.IsEnabled = false;
			}
			if (num >= this.Document.Pages.Count - 1)
			{
				this.NextPage.IsEnabled = false;
				this.EndPage.IsEnabled = false;
			}
			this.PageIndex = num;
			this.UpdatePageData();
		}

		// Token: 0x0600224B RID: 8779 RVA: 0x0009FA58 File Offset: 0x0009DC58
		private void cboxPaperSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			PaperSizeInfo paperSizeInfo = (sender as ComboBox).SelectedItem as PaperSizeInfo;
			int num = Convert.ToInt32((double)paperSizeInfo.PaperSize.Height / 10.0 * 2.54);
			int num2 = Convert.ToInt32((double)paperSizeInfo.PaperSize.Width / 10.0 * 2.54);
			if (this.PaperOrientation.SelectedIndex == 0)
			{
				double num3 = PageHeaderFooterUtils.CmToPdfPoint((double)num / 10.0);
				double num4 = PageHeaderFooterUtils.CmToPdfPoint((double)num2 / 10.0);
				this.PageResize(num4, num3);
				return;
			}
			double num5 = PageHeaderFooterUtils.CmToPdfPoint((double)num / 10.0);
			double num6 = PageHeaderFooterUtils.CmToPdfPoint((double)num2 / 10.0);
			this.PageResize(num5, num6);
		}

		// Token: 0x0600224C RID: 8780 RVA: 0x0009FB2A File Offset: 0x0009DD2A
		private void widthNumBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			this.UpdatePageSize();
		}

		// Token: 0x0600224D RID: 8781 RVA: 0x0009FB34 File Offset: 0x0009DD34
		private void UpdatePageData()
		{
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			string text = (this.UnitComboBox.SelectedItem as ComboBoxItem).Name.ToString().ToLower();
			float num = Math.Abs(this.Document.Pages[this.PageIndex].Height);
			float num2 = Math.Abs(this.Document.Pages[this.PageIndex].Width);
			float num3 = Math.Abs(requiredService.Document.Pages[this.PageIndex].Height);
			float num4 = Math.Abs(requiredService.Document.Pages[this.PageIndex].Width);
			if (text == "cm")
			{
				string text2 = PageHeaderFooterUtils.PdfPointToCm((double)num).ToString("F1");
				string text3 = PageHeaderFooterUtils.PdfPointToCm((double)num2).ToString("F1");
				string text4 = PageHeaderFooterUtils.PdfPointToCm((double)num3).ToString("F1");
				string text5 = PageHeaderFooterUtils.PdfPointToCm((double)num4).ToString("F1");
				this.Resizewidth.Text = text3 + "cm";
				this.Resizeheight.Text = text2 + "cm";
				this.Originalwidth.Text = text5 + "cm";
				this.Originalheight.Text = text4 + "cm";
				this.Widthtunits.Text = "cm";
				this.Heightunits.Text = "cm";
			}
			if (text == "mm")
			{
				string text6 = (PageHeaderFooterUtils.PdfPointToCm((double)num) * 10.0).ToString("F0");
				string text7 = (PageHeaderFooterUtils.PdfPointToCm((double)num2) * 10.0).ToString("F0");
				string text8 = (PageHeaderFooterUtils.PdfPointToCm((double)num3) * 10.0).ToString("F0");
				string text9 = (PageHeaderFooterUtils.PdfPointToCm((double)num4) * 10.0).ToString("F0");
				this.Resizewidth.Text = text7 + "mm";
				this.Resizeheight.Text = text6 + "mm";
				this.Originalwidth.Text = text9 + "mm";
				this.Originalheight.Text = text8 + "mm";
				this.Widthtunits.Text = "mm";
				this.Heightunits.Text = "mm";
			}
			if (text == "point")
			{
				string text10 = num.ToString("F0");
				string text11 = num2.ToString("F0");
				string text12 = num3.ToString("F0");
				string text13 = num4.ToString("F0");
				this.Resizewidth.Text = text11 ?? "";
				this.Resizeheight.Text = text10 ?? "";
				this.Originalwidth.Text = text13 ?? "";
				this.Originalheight.Text = text12 ?? "";
				this.Widthtunits.Text = "point";
				this.Heightunits.Text = "point";
			}
			if (text == "pica")
			{
				string text14 = (num / 12f).ToString("F0");
				string text15 = (num2 / 12f).ToString("F0");
				string text16 = (num3 / 12f).ToString("F0");
				string text17 = (num4 / 12f).ToString("F0");
				this.Resizewidth.Text = text15 + "pica";
				this.Resizeheight.Text = text14 + "pica";
				this.Originalwidth.Text = text17 + "pica";
				this.Originalheight.Text = text16 + "pica";
				this.Widthtunits.Text = "pica";
				this.Heightunits.Text = "pica";
			}
			if (text == "inch")
			{
				string text18 = (num / 72f).ToString("F0");
				string text19 = (num2 / 72f).ToString("F0");
				string text20 = (num3 / 72f).ToString("F0");
				string text21 = (num4 / 72f).ToString("F0");
				this.Resizewidth.Text = text19 + "inch";
				this.Resizeheight.Text = text18 + "inch";
				this.Originalwidth.Text = text21 + "inch";
				this.Originalheight.Text = text20 + "inch";
				this.Widthtunits.Text = "inch";
				this.Heightunits.Text = "inch";
			}
			if (!this.CustomSize.IsChecked.GetValueOrDefault())
			{
				this.widthNumBox.ValueChanged -= this.widthNumBox_ValueChanged;
				this.heightNumbox.ValueChanged -= this.widthNumBox_ValueChanged;
				this.UpdateUnitMaxValue();
				this.widthNumBox.ValueChanged += this.widthNumBox_ValueChanged;
				this.heightNumbox.ValueChanged += this.widthNumBox_ValueChanged;
			}
			this.UpdatePageSize();
		}

		// Token: 0x0600224E RID: 8782 RVA: 0x000A00FC File Offset: 0x0009E2FC
		private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0 && this.Resizewidth != null)
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				string text = (e.AddedItems[0] as ComboBoxItem).Name.ToString().ToLower();
				float num = Math.Abs(this.Document.Pages[this.PageIndex].Height);
				float num2 = Math.Abs(this.Document.Pages[this.PageIndex].Width);
				float num3 = Math.Abs(requiredService.Document.Pages[this.PageIndex].Height);
				float num4 = Math.Abs(requiredService.Document.Pages[this.PageIndex].Width);
				if (text == "cm")
				{
					string text2 = PageHeaderFooterUtils.PdfPointToCm((double)num).ToString("F1");
					string text3 = PageHeaderFooterUtils.PdfPointToCm((double)num2).ToString("F1");
					string text4 = PageHeaderFooterUtils.PdfPointToCm((double)num3).ToString("F1");
					string text5 = PageHeaderFooterUtils.PdfPointToCm((double)num4).ToString("F1");
					this.Resizewidth.Text = text3 + "cm";
					this.Resizeheight.Text = text2 + "cm";
					this.Originalwidth.Text = text5 + "cm";
					this.Originalheight.Text = text4 + "cm";
					this.Widthtunits.Text = "cm";
					this.Heightunits.Text = "cm";
				}
				if (text == "mm")
				{
					string text6 = (PageHeaderFooterUtils.PdfPointToCm((double)num) * 10.0).ToString("F0");
					string text7 = (PageHeaderFooterUtils.PdfPointToCm((double)num2) * 10.0).ToString("F0");
					string text8 = (PageHeaderFooterUtils.PdfPointToCm((double)num3) * 10.0).ToString("F0");
					string text9 = (PageHeaderFooterUtils.PdfPointToCm((double)num4) * 10.0).ToString("F0");
					this.Resizewidth.Text = text7 + "mm";
					this.Resizeheight.Text = text6 + "mm";
					this.Originalwidth.Text = text9 + "mm";
					this.Originalheight.Text = text8 + "mm";
					this.Widthtunits.Text = "mm";
					this.Heightunits.Text = "mm";
				}
				if (text == "point")
				{
					string text10 = num.ToString("F0");
					string text11 = num2.ToString("F0");
					string text12 = num3.ToString("F0");
					string text13 = num4.ToString("F0");
					this.Resizewidth.Text = text11 ?? "";
					this.Resizeheight.Text = text10 ?? "";
					this.Originalwidth.Text = text13 ?? "";
					this.Originalheight.Text = text12 ?? "";
					this.Widthtunits.Text = "point";
					this.Heightunits.Text = "point";
				}
				if (text == "pica")
				{
					string text14 = (num / 12f).ToString("F0");
					string text15 = (num2 / 12f).ToString("F0");
					string text16 = (num3 / 12f).ToString("F0");
					string text17 = (num4 / 12f).ToString("F0");
					this.Resizewidth.Text = text15 + "pica";
					this.Resizeheight.Text = text14 + "pica";
					this.Originalwidth.Text = text17 + "pica";
					this.Originalheight.Text = text16 + "pica";
					this.Widthtunits.Text = "pica";
					this.Heightunits.Text = "pica";
				}
				if (text == "inch")
				{
					string text18 = (num / 72f).ToString("F0");
					string text19 = (num2 / 72f).ToString("F0");
					string text20 = (num3 / 72f).ToString("F0");
					string text21 = (num4 / 72f).ToString("F0");
					this.Resizewidth.Text = text19 + "inch";
					this.Resizeheight.Text = text18 + "inch";
					this.Originalwidth.Text = text21 + "inch";
					this.Originalheight.Text = text20 + "inch";
					this.Widthtunits.Text = "inch";
					this.Heightunits.Text = "inch";
				}
				this.widthNumBox.ValueChanged -= this.widthNumBox_ValueChanged;
				this.heightNumbox.ValueChanged -= this.widthNumBox_ValueChanged;
				this.UpdateUnitMaxValue();
				this.widthNumBox.ValueChanged += this.widthNumBox_ValueChanged;
				this.heightNumbox.ValueChanged += this.widthNumBox_ValueChanged;
				this.UpdatePageSize();
			}
		}

		// Token: 0x0600224F RID: 8783 RVA: 0x000A06C8 File Offset: 0x0009E8C8
		private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (e.Delta > 0)
			{
				this.PART_VerticalScrollBar.Value = this.PART_VerticalScrollBar.Value - 1.0;
				return;
			}
			if (e.Delta < 0)
			{
				this.PART_VerticalScrollBar.Value = this.PART_VerticalScrollBar.Value + 1.0;
			}
		}

		// Token: 0x06002250 RID: 8784 RVA: 0x000A0728 File Offset: 0x0009E928
		private double[] GetNumbersPoint()
		{
			ComboBoxItem comboBoxItem = this.UnitComboBox.SelectedItem as ComboBoxItem;
			double[] array = new double[2];
			if (comboBoxItem.Content != null)
			{
				if (comboBoxItem.Name.ToString().ToLower() == "cm")
				{
					array[0] = PageHeaderFooterUtils.CmToPdfPoint(this.widthNumBox.Value);
					array[1] = PageHeaderFooterUtils.CmToPdfPoint(this.heightNumbox.Value);
				}
				if (comboBoxItem.Name.ToString().ToLower() == "mm")
				{
					array[0] = PageHeaderFooterUtils.CmToPdfPoint(this.widthNumBox.Value / 10.0);
					array[1] = PageHeaderFooterUtils.CmToPdfPoint(this.heightNumbox.Value / 10.0);
				}
				if (comboBoxItem.Name.ToString().ToLower() == "point")
				{
					array[0] = this.widthNumBox.Value;
					array[1] = this.heightNumbox.Value;
				}
				if (comboBoxItem.Name.ToString().ToLower() == "pica")
				{
					array[0] = this.widthNumBox.Value * 12.0;
					array[1] = this.heightNumbox.Value * 12.0;
				}
				if (comboBoxItem.Name.ToString().ToLower() == "inch")
				{
					array[0] = this.widthNumBox.Value * 72.0;
					array[1] = this.heightNumbox.Value * 72.0;
				}
			}
			return array;
		}

		// Token: 0x06002251 RID: 8785 RVA: 0x000A08C4 File Offset: 0x0009EAC4
		private void UpdateUnitMaxValue()
		{
			ComboBoxItem comboBoxItem = this.UnitComboBox.SelectedItem as ComboBoxItem;
			MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
			float num = Math.Abs(requiredService.Document.Pages[this.PageIndex].Height);
			float num2 = Math.Abs(requiredService.Document.Pages[this.PageIndex].Width);
			if (comboBoxItem.Content != null)
			{
				if (comboBoxItem.Name.ToString().ToLower() == "cm")
				{
					this.widthNumBox.Maximum = 999.0;
					this.heightNumbox.Maximum = 999.0;
					this.widthNumBox.Value = PageHeaderFooterUtils.PdfPointToCm((double)num2);
					this.heightNumbox.Value = PageHeaderFooterUtils.PdfPointToCm((double)num);
				}
				if (comboBoxItem.Name.ToString().ToLower() == "mm")
				{
					this.widthNumBox.Maximum = 9990.0;
					this.heightNumbox.Maximum = 9990.0;
					this.widthNumBox.Value = PageHeaderFooterUtils.PdfPointToCm((double)num2) * 10.0;
					this.heightNumbox.Value = PageHeaderFooterUtils.PdfPointToCm((double)num) * 10.0;
				}
				if (comboBoxItem.Name.ToString().ToLower() == "point")
				{
					this.widthNumBox.Maximum = 9990.0;
					this.heightNumbox.Maximum = 9990.0;
					this.widthNumBox.Value = (double)num2;
					this.heightNumbox.Value = (double)num;
				}
				if (comboBoxItem.Name.ToString().ToLower() == "pica")
				{
					this.widthNumBox.Maximum = 999.0;
					this.heightNumbox.Maximum = 999.0;
					this.widthNumBox.Value = (double)(num2 / 12f);
					this.heightNumbox.Value = (double)(num / 12f);
				}
				if (comboBoxItem.Name.ToString().ToLower() == "inch")
				{
					this.widthNumBox.Maximum = 999.0;
					this.heightNumbox.Maximum = 999.0;
					this.widthNumBox.Value = (double)(num2 / 72f);
					this.heightNumbox.Value = (double)(num / 72f);
				}
			}
		}

		// Token: 0x06002252 RID: 8786 RVA: 0x000A0B50 File Offset: 0x0009ED50
		private void GetPageSize(out double height, out double width)
		{
			height = 0.0;
			width = 0.0;
			if (this.FixedRadioButton.IsChecked.GetValueOrDefault())
			{
				PaperSizeInfo paperSizeInfo = this.cboxPaperSize.SelectedItem as PaperSizeInfo;
				if (paperSizeInfo == null)
				{
					return;
				}
				int num;
				int num2;
				if (this.PaperOrientation.SelectedIndex == 0)
				{
					num = Convert.ToInt32((double)paperSizeInfo.PaperSize.Height / 10.0 * 2.54);
					num2 = Convert.ToInt32((double)paperSizeInfo.PaperSize.Width / 10.0 * 2.54);
				}
				else
				{
					num = Convert.ToInt32((double)paperSizeInfo.PaperSize.Width / 10.0 * 2.54);
					num2 = Convert.ToInt32((double)paperSizeInfo.PaperSize.Height / 10.0 * 2.54);
				}
				if (num > 0 && num2 > 0)
				{
					height = PageHeaderFooterUtils.CmToPdfPoint((double)num / 10.0);
					width = PageHeaderFooterUtils.CmToPdfPoint((double)num2 / 10.0);
					return;
				}
			}
			else if (this.CustomSize.IsChecked.GetValueOrDefault())
			{
				double[] numbersPoint = this.GetNumbersPoint();
				if (numbersPoint != null && numbersPoint[0] > 0.0 && numbersPoint[1] > 0.0)
				{
					width = numbersPoint[0];
					height = numbersPoint[1];
				}
			}
		}

		// Token: 0x06002253 RID: 8787 RVA: 0x000A0CC8 File Offset: 0x0009EEC8
		private void UpdatePageSize()
		{
			Ioc.Default.GetRequiredService<MainViewModel>();
			if (this.FixedRadioButton.IsChecked.GetValueOrDefault())
			{
				PaperSizeInfo paperSizeInfo = this.cboxPaperSize.SelectedItem as PaperSizeInfo;
				if (paperSizeInfo == null)
				{
					return;
				}
				int num;
				int num2;
				if (this.PaperOrientation.SelectedIndex == 0)
				{
					num = Convert.ToInt32((double)paperSizeInfo.PaperSize.Height / 10.0 * 2.54);
					num2 = Convert.ToInt32((double)paperSizeInfo.PaperSize.Width / 10.0 * 2.54);
				}
				else
				{
					num = Convert.ToInt32((double)paperSizeInfo.PaperSize.Width / 10.0 * 2.54);
					num2 = Convert.ToInt32((double)paperSizeInfo.PaperSize.Height / 10.0 * 2.54);
				}
				if (num > 0 && num2 > 0)
				{
					double num3 = PageHeaderFooterUtils.CmToPdfPoint((double)num / 10.0);
					double num4 = PageHeaderFooterUtils.CmToPdfPoint((double)num2 / 10.0);
					this.PageResize(num4, num3);
					return;
				}
			}
			else if (this.CustomSize.IsChecked.GetValueOrDefault())
			{
				double[] numbersPoint = this.GetNumbersPoint();
				if (numbersPoint != null && numbersPoint[0] > 0.0 && numbersPoint[1] > 0.0)
				{
					this.PageResize(numbersPoint[0], numbersPoint[1]);
				}
			}
		}

		// Token: 0x06002254 RID: 8788 RVA: 0x000A0E44 File Offset: 0x0009F044
		private void PaperOrientation_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			PaperSizeInfo paperSizeInfo = this.cboxPaperSize.SelectedItem as PaperSizeInfo;
			if (paperSizeInfo == null)
			{
				return;
			}
			int num;
			int num2;
			if (comboBox.SelectedIndex == 0)
			{
				num = Convert.ToInt32((double)paperSizeInfo.PaperSize.Height / 10.0 * 2.54);
				num2 = Convert.ToInt32((double)paperSizeInfo.PaperSize.Width / 10.0 * 2.54);
			}
			else
			{
				num = Convert.ToInt32((double)paperSizeInfo.PaperSize.Width / 10.0 * 2.54);
				num2 = Convert.ToInt32((double)paperSizeInfo.PaperSize.Height / 10.0 * 2.54);
			}
			if (num > 0 && num2 > 0)
			{
				double num3 = PageHeaderFooterUtils.CmToPdfPoint((double)num / 10.0);
				double num4 = PageHeaderFooterUtils.CmToPdfPoint((double)num2 / 10.0);
				this.PageResize(num4, num3);
			}
		}

		// Token: 0x06002255 RID: 8789 RVA: 0x000A0F4B File Offset: 0x0009F14B
		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("PageResize", "PageResizeWindowCancelBtn", "Count", 1L);
			base.Close();
		}

		// Token: 0x06002256 RID: 8790 RVA: 0x000A0F6C File Offset: 0x0009F16C
		private void OkBtn_Click(object sender, RoutedEventArgs e)
		{
			PageResizeWindow.<>c__DisplayClass38_0 CS$<>8__locals1 = new PageResizeWindow.<>c__DisplayClass38_0();
			CS$<>8__locals1.<>4__this = this;
			if (this.CustomTextBox.IsFocused)
			{
				this.btnOk.Focus();
			}
			GAManager.SendEvent("PageResize", "PageResizeWindowOkBtn", this.GetImportPageCount().ToString(), 1L);
			int[] importPageRange = this.GetImportPageRange();
			if (importPageRange == null)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkPageError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			try
			{
				MainViewModel requiredService = Ioc.Default.GetRequiredService<MainViewModel>();
				double num;
				double num2;
				this.GetPageSize(out num, out num2);
				if (num <= 0.0 || num2 <= 0.0)
				{
					return;
				}
				CS$<>8__locals1.list = new List<global::System.ValueTuple<int, FS_RECTF, FS_RECTF>>();
				foreach (int num3 in importPageRange)
				{
					if (requiredService.Document.Pages[num3].Dictionary.ContainsKey("CropBox"))
					{
						CS$<>8__locals1.list.Add(new global::System.ValueTuple<int, FS_RECTF, FS_RECTF>(num3, requiredService.Document.Pages[num3].CropBox, new FS_RECTF(0.0, num, num2, 0.0)));
					}
					else
					{
						CS$<>8__locals1.list.Add(new global::System.ValueTuple<int, FS_RECTF, FS_RECTF>(num3, requiredService.Document.Pages[num3].MediaBox, new FS_RECTF(0.0, num, num2, 0.0)));
					}
					this.PageResize(requiredService.Document.Pages[num3], num2, num);
				}
				requiredService.PageEditors.FlushViewerAndThumbnail(false);
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(requiredService.Document);
				pdfControl.UpdateLayout();
				pdfControl.UpdateDocLayout();
				pdfControl.TryRedrawVisiblePageAsync(default(CancellationToken));
				requiredService.OperationManager.AddOperationAsync(delegate(PdfDocument doc)
				{
					PageResizeWindow.<>c__DisplayClass38_0.<<OkBtn_Click>b__0>d <<OkBtn_Click>b__0>d;
					<<OkBtn_Click>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<OkBtn_Click>b__0>d.<>4__this = CS$<>8__locals1;
					<<OkBtn_Click>b__0>d.doc = doc;
					<<OkBtn_Click>b__0>d.<>1__state = -1;
					<<OkBtn_Click>b__0>d.<>t__builder.Start<PageResizeWindow.<>c__DisplayClass38_0.<<OkBtn_Click>b__0>d>(ref <<OkBtn_Click>b__0>d);
					return <<OkBtn_Click>b__0>d.<>t__builder.Task;
				}, delegate(PdfDocument doc)
				{
					PageResizeWindow.<>c__DisplayClass38_0.<<OkBtn_Click>b__1>d <<OkBtn_Click>b__1>d;
					<<OkBtn_Click>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<OkBtn_Click>b__1>d.<>4__this = CS$<>8__locals1;
					<<OkBtn_Click>b__1>d.doc = doc;
					<<OkBtn_Click>b__1>d.<>1__state = -1;
					<<OkBtn_Click>b__1>d.<>t__builder.Start<PageResizeWindow.<>c__DisplayClass38_0.<<OkBtn_Click>b__1>d>(ref <<OkBtn_Click>b__1>d);
					return <<OkBtn_Click>b__1>d.<>t__builder.Task;
				}, "");
			}
			catch
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.ChatPanelMessageErrorText, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			base.Closing -= this.PageResizeWindow_Closing;
			this.Document.Dispose();
			base.Close();
		}

		// Token: 0x06002257 RID: 8791 RVA: 0x000A11C0 File Offset: 0x0009F3C0
		private int[] GetImportPageRange()
		{
			int[] array = null;
			if (this.AllPageBtn.IsChecked.GetValueOrDefault())
			{
				array = new int[this.Document.Pages.Count];
				for (int i = 0; i < this.Document.Pages.Count; i++)
				{
					array[i] = i;
				}
			}
			else if (this.CurrentPageBtn.IsChecked.GetValueOrDefault())
			{
				array = new int[1];
				if (this.CurrentPageindex != -1)
				{
					array[0] = this.CurrentPageindex;
				}
				else
				{
					array[0] = this.Document.Pages.CurrentIndex;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(this.CustomTextBox.Text))
				{
					return null;
				}
				int[] array2;
				int num;
				PdfObjectExtensions.TryParsePageRange(this.CustomTextBox.Text, out array2, out num);
				if (array2 == null)
				{
					return null;
				}
				if (array2.Length != 0)
				{
					array = array2;
				}
			}
			if (this.SubsetComboBox.SelectedIndex == 2)
			{
				array = array.Where((int c) => c % 2 == 0).ToArray<int>();
			}
			else if (this.SubsetComboBox.SelectedIndex == 1)
			{
				array = array.Where((int c) => c % 2 == 1).ToArray<int>();
			}
			if (array.Any((int c) => c < 0 || c >= this.Document.Pages.Count))
			{
				return null;
			}
			if (array.Length == 0)
			{
				return null;
			}
			return array;
		}

		// Token: 0x06002258 RID: 8792 RVA: 0x000A1328 File Offset: 0x0009F528
		private int GetImportPageCount()
		{
			int[] array = null;
			if (this.AllPageBtn.IsChecked.GetValueOrDefault())
			{
				array = new int[this.Document.Pages.Count];
				for (int i = 0; i < this.Document.Pages.Count; i++)
				{
					array[i] = i;
				}
			}
			else if (this.CurrentPageBtn.IsChecked.GetValueOrDefault())
			{
				array = new int[1];
				if (this.CurrentPageindex != -1)
				{
					array[0] = this.CurrentPageindex;
				}
				else
				{
					array[0] = this.Document.Pages.CurrentIndex;
				}
			}
			else
			{
				if (string.IsNullOrEmpty(this.CustomTextBox.Text))
				{
					return 0;
				}
				int[] array2;
				int num;
				PdfObjectExtensions.TryParsePageRange(this.CustomTextBox.Text, out array2, out num);
				if (array2 == null)
				{
					return 0;
				}
				if (array2.Length != 0)
				{
					array = array2;
				}
			}
			if (this.SubsetComboBox.SelectedIndex == 2)
			{
				array = array.Where((int c) => c % 2 == 0).ToArray<int>();
			}
			else if (this.SubsetComboBox.SelectedIndex == 1)
			{
				array = array.Where((int c) => c % 2 == 1).ToArray<int>();
			}
			if (array.Any((int c) => c < 0 || c >= this.Document.Pages.Count))
			{
				return 0;
			}
			if (array.Length == 0)
			{
				return 0;
			}
			return array.Count<int>();
		}

		// Token: 0x06002259 RID: 8793 RVA: 0x000A1494 File Offset: 0x0009F694
		private void GroupChange()
		{
			if (this.Document != null)
			{
				double num;
				double num2;
				this.GetPageSize(out num, out num2);
				if (num <= 0.0 || num2 <= 0.0)
				{
					return;
				}
				this.PageResize(num2, num);
			}
		}

		// Token: 0x0600225A RID: 8794 RVA: 0x000A14D4 File Offset: 0x0009F6D4
		private void FixedRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			this.GroupChange();
		}

		// Token: 0x0600225B RID: 8795 RVA: 0x000A14DC File Offset: 0x0009F6DC
		private void CustomSize_Checked(object sender, RoutedEventArgs e)
		{
			this.GroupChange();
		}

		// Token: 0x04000E61 RID: 3681
		public List<PaperSizeInfo> paperSizesLst = new List<PaperSizeInfo>();

		// Token: 0x04000E63 RID: 3683
		private int PageIndex = -1;

		// Token: 0x04000E64 RID: 3684
		private int CurrentPageindex = -1;
	}
}
