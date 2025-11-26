using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommonLib.Common;
using CommonLib.Controls;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Properties;
using pdfeditor.Utils;
using PDFKit;
using PDFKit.Utils;
using PDFKit.Utils.PageHeaderFooters;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x020001FF RID: 511
	public partial class CropPageWindow : Window
	{
		// Token: 0x06001CA5 RID: 7333 RVA: 0x00077AB3 File Offset: 0x00075CB3
		public CropPageWindow()
		{
			this.InitializeComponent();
		}

		// Token: 0x17000A37 RID: 2615
		// (get) Token: 0x06001CA6 RID: 7334 RVA: 0x00077ACF File Offset: 0x00075CCF
		// (set) Token: 0x06001CA7 RID: 7335 RVA: 0x00077AE1 File Offset: 0x00075CE1
		public MarginModel PageMargin2
		{
			get
			{
				return (MarginModel)base.GetValue(CropPageWindow.PageMargin2Property);
			}
			set
			{
				base.SetValue(CropPageWindow.PageMargin2Property, value);
			}
		}

		// Token: 0x17000A38 RID: 2616
		// (get) Token: 0x06001CA8 RID: 7336 RVA: 0x00077AEF File Offset: 0x00075CEF
		// (set) Token: 0x06001CA9 RID: 7337 RVA: 0x00077B01 File Offset: 0x00075D01
		public ScreenshotDialog ScreenshotDialog2
		{
			get
			{
				return (ScreenshotDialog)base.GetValue(CropPageWindow.ScreenshotDialog2Property);
			}
			set
			{
				base.SetValue(CropPageWindow.ScreenshotDialog2Property, value);
			}
		}

		// Token: 0x17000A39 RID: 2617
		// (get) Token: 0x06001CAA RID: 7338 RVA: 0x00077B0F File Offset: 0x00075D0F
		// (set) Token: 0x06001CAB RID: 7339 RVA: 0x00077B21 File Offset: 0x00075D21
		public PageSizeModel PageSize
		{
			get
			{
				return (PageSizeModel)base.GetValue(CropPageWindow.PageSizeProperty);
			}
			set
			{
				base.SetValue(CropPageWindow.PageSizeProperty, value);
			}
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x00077B30 File Offset: 0x00075D30
		private void CropPageViewer_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !this.ZeroCheckBox.IsChecked.Value)
			{
				global::System.Windows.Point position = e.GetPosition(this);
				global::System.Windows.Point point = new global::System.Windows.Point(0.0, 0.0);
				if (this.clickStartPosition == point)
				{
					this.clickStartPosition = e.GetPosition(this);
				}
				float num = (float)(position.X - this.clickStartPosition.X);
				float num2 = (float)(this.clickStartPosition.Y - position.Y);
				double left = Canvas.GetLeft(this.cropRectGrid);
				double bottom = Canvas.GetBottom(this.cropRectGrid);
				double num3 = this.Cropedimage.Width - this.cropRectangle.Width - left - (double)num;
				double num4 = this.Cropedimage.Height - this.cropRectangle.Height - bottom - (double)num2;
				this.clickStartPosition = position;
				if ((this.mousePosition == CropPageWindow.MousePosition.None && ((double)num + this.cropRectangle.Width > (double)((float)this.Cropedimage.Width + 5f) || num3 < -5.0 || (double)num2 + this.cropRectangle.Height > (double)((float)this.Cropedimage.Height + 5f) || num4 < -5.0)) || left + (double)num < -5.0 || bottom + (double)num2 < -5.0)
				{
					return;
				}
				if (!this.ScreenshotDialog2.fixedRate)
				{
					if (this.mousePosition == CropPageWindow.MousePosition.None && this.CropArea.IsEnabled)
					{
						Canvas.SetLeft(this.cropRectGrid, left + (double)num);
						Canvas.SetBottom(this.cropRectGrid, bottom + (double)num2);
					}
					if (this.mousePosition == CropPageWindow.MousePosition.topLeft)
					{
						if (this.cropRectangle.Height + (double)num2 <= 5.0 || this.cropRectangle.Width - (double)num <= 5.0 || this.cropRectangle.Height + (double)num2 + bottom > this.Cropedimage.Height + 5.0)
						{
							return;
						}
						this.cropRectangle.Height = this.cropRectangle.Height + (double)num2;
						this.cropRectangle.Width = this.cropRectangle.Width - (double)num;
						Canvas.SetLeft(this.cropRectGrid, left + (double)num);
					}
					if (this.mousePosition == CropPageWindow.MousePosition.topCenter)
					{
						if (this.cropRectangle.Height + (double)num2 <= 5.0 || this.cropRectangle.Height + (double)num2 + bottom > this.Cropedimage.Height + 5.0)
						{
							return;
						}
						this.cropRectangle.Height = this.cropRectangle.Height + (double)num2;
					}
					if (this.mousePosition == CropPageWindow.MousePosition.topRight)
					{
						if (this.cropRectangle.Height + (double)num2 <= 5.0 || this.cropRectangle.Width + (double)num <= 5.0 || this.cropRectangle.Height + (double)num2 + bottom > this.Cropedimage.Height + 5.0 || this.cropRectangle.Width + (double)num + left > this.Cropedimage.Width + 5.0)
						{
							return;
						}
						this.cropRectangle.Height = this.cropRectangle.Height + (double)num2;
						this.cropRectangle.Width = this.cropRectangle.Width + (double)num;
					}
					if (this.mousePosition == CropPageWindow.MousePosition.leftCenter)
					{
						if (this.cropRectangle.Width - (double)num <= 5.0)
						{
							return;
						}
						this.cropRectangle.Width = this.cropRectangle.Width - (double)num;
						Canvas.SetLeft(this.cropRectGrid, left + (double)num);
					}
					if (this.mousePosition == CropPageWindow.MousePosition.rightCenter)
					{
						if (this.cropRectangle.Width + (double)num <= 5.0 || this.cropRectangle.Width + (double)num + left > this.Cropedimage.Width + 5.0)
						{
							return;
						}
						this.cropRectangle.Width = this.cropRectangle.Width + (double)num;
					}
					if (this.mousePosition == CropPageWindow.MousePosition.bottomLeft)
					{
						if (this.cropRectangle.Height - (double)num2 <= 5.0 || this.cropRectangle.Width - (double)num <= 5.0)
						{
							return;
						}
						this.cropRectangle.Height = this.cropRectangle.Height - (double)num2;
						this.cropRectangle.Width = this.cropRectangle.Width - (double)num;
						Canvas.SetLeft(this.cropRectGrid, left + (double)num);
						Canvas.SetBottom(this.cropRectGrid, bottom + (double)num2);
					}
					if (this.mousePosition == CropPageWindow.MousePosition.bottomCenter)
					{
						if (this.cropRectangle.Height - (double)num2 <= 5.0)
						{
							return;
						}
						this.cropRectangle.Height = this.cropRectangle.Height - (double)num2;
						Canvas.SetBottom(this.cropRectGrid, bottom + (double)num2);
					}
					if (this.mousePosition == CropPageWindow.MousePosition.bottomRight)
					{
						if (this.cropRectangle.Height - (double)num2 <= 5.0 || this.cropRectangle.Width + (double)num <= 5.0 || this.cropRectangle.Width + (double)num + left > this.Cropedimage.Width + 5.0)
						{
							return;
						}
						this.cropRectangle.Height = this.cropRectangle.Height - (double)num2;
						this.cropRectangle.Width = this.cropRectangle.Width + (double)num;
						Canvas.SetBottom(this.cropRectGrid, bottom + (double)num2);
					}
				}
				else
				{
					float num5 = (this.ScreenshotDialog2.startPt.Y - this.ScreenshotDialog2.curPt.Y) / (this.ScreenshotDialog2.curPt.X - this.ScreenshotDialog2.startPt.X);
					if (this.mousePosition == CropPageWindow.MousePosition.None && this.CropArea.IsEnabled)
					{
						Canvas.SetLeft(this.cropRectGrid, left + (double)num);
						Canvas.SetBottom(this.cropRectGrid, bottom + (double)num2);
					}
					if (this.mousePosition == CropPageWindow.MousePosition.topLeft)
					{
						if (this.cropRectangle.Height - (double)(num * num5) < 25.0 || this.cropRectangle.Width - (double)num < 25.0 || this.cropRectangle.Height + (double)(num * num5) + bottom > this.Cropedimage.Height + 5.0)
						{
							return;
						}
						this.cropRectangle.Height = this.cropRectangle.Height - (double)(num * num5);
						this.cropRectangle.Width = this.cropRectangle.Width - (double)num;
						Canvas.SetLeft(this.cropRectGrid, left + (double)num);
					}
					if (this.mousePosition == CropPageWindow.MousePosition.topRight)
					{
						if (this.cropRectangle.Height + (double)(num * num5) < 25.0 || this.cropRectangle.Width + (double)num < 25.0 || this.cropRectangle.Height + (double)(num * num5) + bottom > this.Cropedimage.Height + 5.0 || this.cropRectangle.Width + (double)num + left > this.Cropedimage.Width + 5.0)
						{
							return;
						}
						this.cropRectangle.Height = this.cropRectangle.Height + (double)(num * num5);
						this.cropRectangle.Width = this.cropRectangle.Width + (double)num;
					}
					if (this.mousePosition == CropPageWindow.MousePosition.bottomLeft)
					{
						if (this.cropRectangle.Height - (double)(num * num5) < 25.0 || this.cropRectangle.Width - (double)num < 25.0)
						{
							return;
						}
						this.cropRectangle.Height = this.cropRectangle.Height - (double)(num * num5);
						this.cropRectangle.Width = this.cropRectangle.Width - (double)num;
						Canvas.SetLeft(this.cropRectGrid, left + (double)num);
						Canvas.SetBottom(this.cropRectGrid, bottom + (double)(num * num5));
					}
					if (this.mousePosition == CropPageWindow.MousePosition.bottomRight)
					{
						if (this.cropRectangle.Height + (double)(num * num5) < 25.0 || this.cropRectangle.Width + (double)num < 25.0 || this.cropRectangle.Width + (double)num + left > this.Cropedimage.Width + 5.0)
						{
							return;
						}
						this.cropRectangle.Height = this.cropRectangle.Height + (double)(num * num5);
						this.cropRectangle.Width = this.cropRectangle.Width + (double)num;
						Canvas.SetBottom(this.cropRectGrid, bottom - (double)(num * num5));
					}
				}
				float num6 = (float)this.Cropedimage.Width;
				float num7 = (float)this.Cropedimage.Height;
				FS_RECTF mediaBox = CropPageWindow.GetMediaBox(this.Document.Pages[this.PageIndex].Handle);
				double num10;
				double num11;
				double num12;
				double num13;
				if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate90)
				{
					float num8 = num7 / Math.Abs(mediaBox.Width);
					float num9 = num6 / Math.Abs(mediaBox.Height);
					num10 = Canvas.GetLeft(this.cropRectGrid) / (double)num9;
					num11 = ((double)num7 - Canvas.GetBottom(this.cropRectGrid)) / (double)num8;
					num12 = num11 - this.cropRectangle.Height / (double)num8;
					num13 = this.cropRectangle.Width / (double)num9 + num10;
				}
				else if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate270)
				{
					float num14 = num7 / Math.Abs(mediaBox.Width);
					float num15 = num6 / Math.Abs(mediaBox.Height);
					num12 = Canvas.GetBottom(this.cropRectGrid) / (double)num14;
					num13 = ((double)num6 - Canvas.GetLeft(this.cropRectGrid)) / (double)num15;
					num10 = num13 - this.cropRectangle.Width / (double)num15;
					num11 = num12 + this.cropRectangle.Height / (double)num14;
				}
				else if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate180)
				{
					float num16 = num6 / Math.Abs(mediaBox.Width);
					float num17 = num7 / Math.Abs(mediaBox.Height);
					num11 = ((double)num6 - Canvas.GetLeft(this.cropRectGrid)) / (double)num16;
					num13 = ((double)num7 - Canvas.GetBottom(this.cropRectGrid)) / (double)num17;
					num10 = num13 - this.cropRectangle.Height / (double)num17;
					num12 = num11 - this.cropRectangle.Width / (double)num16;
				}
				else
				{
					float num18 = num6 / Math.Abs(mediaBox.Width);
					float num19 = num7 / Math.Abs(mediaBox.Height);
					num12 = Canvas.GetLeft(this.cropRectGrid) / (double)num18;
					num10 = Canvas.GetBottom(this.cropRectGrid) / (double)num19;
					num13 = (this.cropRectangle.Height + Canvas.GetBottom(this.cropRectGrid)) / (double)num19;
					num11 = (this.cropRectangle.Width + Canvas.GetLeft(this.cropRectGrid)) / (double)num18;
				}
				this.UpdateBoxSize(num13 - num10, num11 - num12);
			}
		}

		// Token: 0x06001CAD RID: 7341 RVA: 0x0007870C File Offset: 0x0007690C
		private bool HitCropBox(global::System.Windows.Point point)
		{
			double height = this.cropRectangle.Height;
			double width = this.cropRectangle.Width;
			global::System.Windows.Point position = Mouse.GetPosition(this.cropRectGrid);
			return position.X >= 0.0 && position.Y >= 0.0 && position.X <= width && position.Y <= height;
		}

		// Token: 0x06001CAE RID: 7342 RVA: 0x00078778 File Offset: 0x00076978
		public void UpdateData()
		{
			FS_RECTF mediaBox = CropPageWindow.GetMediaBox(this.Document.Pages[this.PageIndex].Handle);
			float num = (float)this.Cropedimage.Width;
			float num2 = (float)this.Cropedimage.Height;
			double num5;
			double num6;
			double num7;
			double num8;
			if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate90)
			{
				float num3 = num2 / Math.Abs(mediaBox.Width);
				float num4 = num / Math.Abs(mediaBox.Height);
				num5 = Canvas.GetLeft(this.cropRectGrid) / (double)num4;
				num6 = ((double)num2 - Canvas.GetBottom(this.cropRectGrid)) / (double)num3;
				num7 = num6 - this.cropRectangle.Height / (double)num3;
				num8 = this.cropRectangle.Width / (double)num4 + num5;
				this.ScreenshotDialog2.startPt.X = (float)num7;
				this.ScreenshotDialog2.startPt.Y = (float)num8;
				this.ScreenshotDialog2.curPt.X = (float)num6;
				this.ScreenshotDialog2.curPt.Y = (float)num5;
			}
			else if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate270)
			{
				float num9 = num2 / Math.Abs(mediaBox.Width);
				float num10 = num / Math.Abs(mediaBox.Height);
				num7 = Canvas.GetBottom(this.cropRectGrid) / (double)num9;
				num8 = ((double)num - Canvas.GetLeft(this.cropRectGrid)) / (double)num10;
				num5 = num8 - this.cropRectangle.Width / (double)num10;
				num6 = num7 + this.cropRectangle.Height / (double)num9;
				this.ScreenshotDialog2.startPt.X = (float)num7;
				this.ScreenshotDialog2.startPt.Y = (float)num8;
				this.ScreenshotDialog2.curPt.X = (float)num6;
				this.ScreenshotDialog2.curPt.Y = (float)num5;
			}
			else if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate180)
			{
				float num11 = num / Math.Abs(mediaBox.Width);
				float num12 = num2 / Math.Abs(mediaBox.Height);
				num6 = ((double)num - Canvas.GetLeft(this.cropRectGrid)) / (double)num11;
				num8 = ((double)num2 - Canvas.GetBottom(this.cropRectGrid)) / (double)num12;
				num5 = num8 - this.cropRectangle.Height / (double)num12;
				num7 = num6 - this.cropRectangle.Width / (double)num11;
				this.ScreenshotDialog2.startPt.X = (float)num7;
				this.ScreenshotDialog2.startPt.Y = (float)num8;
				this.ScreenshotDialog2.curPt.X = (float)num6;
				this.ScreenshotDialog2.curPt.Y = (float)num5;
			}
			else
			{
				float num13 = num / Math.Abs(mediaBox.Width);
				float num14 = num2 / Math.Abs(mediaBox.Height);
				num7 = Canvas.GetLeft(this.cropRectGrid) / (double)num13;
				num5 = Canvas.GetBottom(this.cropRectGrid) / (double)num14;
				num8 = (this.cropRectangle.Height + Canvas.GetBottom(this.cropRectGrid)) / (double)num14;
				num6 = (this.cropRectangle.Width + Canvas.GetLeft(this.cropRectGrid)) / (double)num13;
				this.ScreenshotDialog2.startPt.X = (float)num7;
				this.ScreenshotDialog2.startPt.Y = (float)num8;
				this.ScreenshotDialog2.curPt.X = (float)num6;
				this.ScreenshotDialog2.curPt.Y = (float)num5;
			}
			this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
			this.PageMargin2 = this.ScreenshotDialog2.reflashData();
			this.PageSize = this.ScreenshotDialog2.reflashSizeData();
			this.UpdateUnitMaxValue();
			this.UpdateBoxSize(num8 - num5, num6 - num7);
		}

		// Token: 0x06001CAF RID: 7343 RVA: 0x00078B88 File Offset: 0x00076D88
		private void UpdateBoxSize(double height, double width)
		{
			string text = (this.UnitComboBox.SelectedItem as ComboBoxItem).Name.ToString().ToLower();
			if (text == "cm")
			{
				string text2 = PageHeaderFooterUtils.PdfPointToCm(height).ToString("F2");
				string text3 = PageHeaderFooterUtils.PdfPointToCm(width).ToString("F2");
				this.BoxSize.Text = string.Concat(new string[]
				{
					text3,
					" * ",
					text2,
					" ",
					this.UnitComboBox.Text
				});
			}
			if (text == "mm")
			{
				string text4 = (PageHeaderFooterUtils.PdfPointToCm(height) * 10.0).ToString("F2");
				string text5 = (PageHeaderFooterUtils.PdfPointToCm(width) * 10.0).ToString("F2");
				this.BoxSize.Text = string.Concat(new string[]
				{
					text5,
					" * ",
					text4,
					" ",
					this.UnitComboBox.Text
				});
			}
			if (text == "point")
			{
				string text6 = height.ToString("F2");
				string text7 = width.ToString("F2");
				this.BoxSize.Text = string.Concat(new string[]
				{
					text7,
					" * ",
					text6,
					" ",
					this.UnitComboBox.Text
				});
			}
			if (text == "pica")
			{
				string text8 = (height / 12.0).ToString("F2");
				string text9 = (width / 12.0).ToString("F2");
				this.BoxSize.Text = string.Concat(new string[]
				{
					text9,
					" * ",
					text8,
					" ",
					this.UnitComboBox.Text
				});
			}
			if (text == "inch")
			{
				string text10 = (height / 72.0).ToString("F2");
				string text11 = (width / 72.0).ToString("F2");
				this.BoxSize.Text = string.Concat(new string[]
				{
					text11,
					" * ",
					text10,
					" ",
					this.UnitComboBox.Text
				});
			}
		}

		// Token: 0x06001CB0 RID: 7344 RVA: 0x00078E14 File Offset: 0x00077014
		private static FS_RECTF NormalizeRect(float left, float bottom, float right, float top)
		{
			return new FS_RECTF(Math.Min(left, right), Math.Max(top, bottom), Math.Max(left, right), Math.Min(top, bottom));
		}

		// Token: 0x06001CB1 RID: 7345 RVA: 0x00078E38 File Offset: 0x00077038
		private static FS_RECTF GetMediaBox(IntPtr pageHandle)
		{
			float num;
			float num2;
			float num3;
			float num4;
			if (!Pdfium.FPDFPage_GetMediaBox(pageHandle, out num, out num2, out num3, out num4))
			{
				throw new ArgumentException("pageHandle");
			}
			return CropPageWindow.NormalizeRect(num, num2, num3, num4);
		}

		// Token: 0x06001CB2 RID: 7346 RVA: 0x00078E6C File Offset: 0x0007706C
		public void CreateCropbox(FS_RECTF CropBox)
		{
			float num = (float)this.Cropedimage.Width;
			float num2 = (float)this.Cropedimage.Height;
			FS_RECTF mediaBox = CropPageWindow.GetMediaBox(this.Document.Pages[this.PageIndex].Handle);
			float num3 = num / Math.Abs(mediaBox.Width);
			float num4 = num2 / Math.Abs(mediaBox.Height);
			this.PageSize = this.ScreenshotDialog2.reflashSizeData();
			this.PageMargin2 = this.ScreenshotDialog2.reflashData();
			if (!this.ResizeControl.IsChecked.Value)
			{
				if (this.rotate == PageRotate.Normal)
				{
					FS_RECTF fs_RECTF = new FS_RECTF
					{
						right = num3 * CropBox.right,
						left = num3 * CropBox.left,
						top = num4 * CropBox.top,
						bottom = num4 * CropBox.bottom
					};
					this.cropRectangle.Height = (double)Math.Abs(fs_RECTF.Height);
					this.cropRectangle.Width = (double)Math.Abs(fs_RECTF.Width);
					Canvas.SetLeft(this.cropRectGrid, (double)fs_RECTF.left);
					Canvas.SetBottom(this.cropRectGrid, (double)fs_RECTF.bottom);
				}
				else if (this.rotate == PageRotate.Rotate90)
				{
					num3 = num2 / Math.Abs(mediaBox.Width);
					num4 = num / Math.Abs(mediaBox.Height);
					FS_RECTF fs_RECTF2 = new FS_RECTF
					{
						right = num3 * CropBox.right,
						left = num3 * CropBox.left,
						top = num4 * CropBox.top,
						bottom = num4 * CropBox.bottom
					};
					this.cropRectangle.Height = (double)Math.Abs(fs_RECTF2.Width);
					this.cropRectangle.Width = (double)Math.Abs(fs_RECTF2.Height);
					Canvas.SetLeft(this.cropRectGrid, (double)fs_RECTF2.bottom);
					Canvas.SetBottom(this.cropRectGrid, (double)(num2 - fs_RECTF2.right));
				}
				else if (this.rotate == PageRotate.Rotate180)
				{
					FS_RECTF fs_RECTF3 = new FS_RECTF
					{
						right = num3 * CropBox.right,
						left = num3 * CropBox.left,
						top = num4 * CropBox.top,
						bottom = num4 * CropBox.bottom
					};
					this.cropRectangle.Height = (double)Math.Abs(fs_RECTF3.Height);
					this.cropRectangle.Width = (double)Math.Abs(fs_RECTF3.Width);
					Canvas.SetLeft(this.cropRectGrid, (double)fs_RECTF3.bottom);
					Canvas.SetBottom(this.cropRectGrid, (double)(num2 - fs_RECTF3.top));
				}
				else if (this.rotate == PageRotate.Rotate270)
				{
					num3 = num2 / Math.Abs(mediaBox.Width);
					num4 = num / Math.Abs(mediaBox.Height);
					FS_RECTF fs_RECTF4 = new FS_RECTF
					{
						left = num3 * CropBox.left,
						right = num3 * CropBox.right,
						top = num4 * CropBox.top,
						bottom = num4 * CropBox.bottom
					};
					this.cropRectangle.Height = (double)Math.Abs(fs_RECTF4.Width);
					this.cropRectangle.Width = (double)Math.Abs(fs_RECTF4.Height);
					Canvas.SetLeft(this.cropRectGrid, (double)(num - fs_RECTF4.top));
					Canvas.SetBottom(this.cropRectGrid, (double)fs_RECTF4.left);
				}
			}
			this.setCropImage();
			this.ScreenshotDialog2.startPt.X = this.originalBox.left;
			this.ScreenshotDialog2.startPt.Y = this.originalBox.top;
			this.ScreenshotDialog2.curPt.X = this.originalBox.right;
			this.ScreenshotDialog2.curPt.Y = this.originalBox.bottom;
			this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
			this.PageMargin2 = this.ScreenshotDialog2.reflashData();
		}

		// Token: 0x06001CB3 RID: 7347 RVA: 0x000792A0 File Offset: 0x000774A0
		public void UpdateCropbox(FS_RECTF CropBox)
		{
			float num = (float)this.Cropedimage.Width;
			float num2 = (float)this.Cropedimage.Height;
			FS_RECTF mediaBox = CropPageWindow.GetMediaBox(this.Document.Pages[this.PageIndex].Handle);
			float num3 = num / Math.Abs(mediaBox.Width);
			float num4 = num2 / Math.Abs(mediaBox.Height);
			this.PageSize = this.ScreenshotDialog2.reflashSizeData();
			this.PageMargin2 = this.ScreenshotDialog2.reflashData();
			if (!this.ResizeControl.IsChecked.Value)
			{
				if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate90)
				{
					num3 = num2 / Math.Abs(mediaBox.Width);
					num4 = num / Math.Abs(mediaBox.Height);
					FS_RECTF fs_RECTF = new FS_RECTF
					{
						right = num3 * CropBox.right,
						left = num3 * CropBox.left,
						top = num4 * CropBox.top,
						bottom = num4 * CropBox.bottom
					};
					this.cropRectangle.Height = (double)Math.Abs(fs_RECTF.Width);
					this.cropRectangle.Width = (double)Math.Abs(fs_RECTF.Height);
					Canvas.SetLeft(this.cropRectGrid, (double)fs_RECTF.bottom);
					Canvas.SetBottom(this.cropRectGrid, (double)(num2 - fs_RECTF.right));
				}
				else if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate180)
				{
					FS_RECTF fs_RECTF2 = new FS_RECTF
					{
						right = num3 * CropBox.right,
						left = num3 * CropBox.left,
						top = num4 * CropBox.top,
						bottom = num4 * CropBox.bottom
					};
					this.cropRectangle.Height = (double)fs_RECTF2.Height;
					this.cropRectangle.Width = (double)fs_RECTF2.Width;
					Canvas.SetLeft(this.cropRectGrid, (double)(num - fs_RECTF2.right));
					Canvas.SetBottom(this.cropRectGrid, (double)(num2 - fs_RECTF2.top));
				}
				else if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate270)
				{
					num3 = num2 / Math.Abs(mediaBox.Width);
					num4 = num / Math.Abs(mediaBox.Height);
					FS_RECTF fs_RECTF3 = new FS_RECTF
					{
						right = num3 * CropBox.right,
						left = num3 * CropBox.left,
						top = num4 * CropBox.top,
						bottom = num4 * CropBox.bottom
					};
					this.cropRectangle.Height = (double)Math.Abs(fs_RECTF3.Width);
					this.cropRectangle.Width = (double)Math.Abs(fs_RECTF3.Height);
					Canvas.SetLeft(this.cropRectGrid, (double)(num - fs_RECTF3.top));
					Canvas.SetBottom(this.cropRectGrid, (double)fs_RECTF3.left);
				}
				else
				{
					FS_RECTF fs_RECTF4 = new FS_RECTF
					{
						right = num3 * CropBox.right,
						left = num3 * CropBox.left,
						top = num4 * CropBox.top,
						bottom = num4 * CropBox.bottom
					};
					this.cropRectangle.Height = (double)Math.Abs(fs_RECTF4.Height);
					this.cropRectangle.Width = (double)Math.Abs(fs_RECTF4.Width);
					Canvas.SetLeft(this.cropRectGrid, (double)fs_RECTF4.left);
					Canvas.SetBottom(this.cropRectGrid, (double)fs_RECTF4.bottom);
				}
			}
			this.setCropImage();
			this.UpdateBoxSize((double)(CropBox.top - CropBox.bottom), (double)(CropBox.right - CropBox.left));
		}

		// Token: 0x06001CB4 RID: 7348 RVA: 0x00079698 File Offset: 0x00077898
		private void setCropImage()
		{
			this.Document.Pages[this.PageIndex].SetPageCropBox(this.Document.Pages[this.PageIndex].MediaBox);
			this.Document.Pages[this.PageIndex].ReloadPage();
			float num = this.Document.Pages[this.PageIndex].Width;
			float num2 = this.Document.Pages[this.PageIndex].Height;
			if (num > num2)
			{
				float num3 = num2 / num;
				this.cropBoder.Height = (double)(270f * num3 + 2f);
				this.Cropedimage.Height = (double)(270f * num3);
			}
			else
			{
				float num4 = num / num2;
				if (num4 * 373f > 270f)
				{
					this.cropBoder.Height = (double)(270f / num4 + 3f);
					this.Cropedimage.Height = (double)(270f / num4);
				}
				else
				{
					this.Cropedimage.Height = 373.0;
					this.cropBoder.Height = 376.0;
					this.Cropedimage.Width = (double)(373f * num4);
					this.cropBoder.Width = (double)(373f * num4 + 3f);
				}
			}
			if (num > 1f && num2 > 1f)
			{
				if (num > 810f)
				{
					num2 *= 810f / num;
					num = 810f;
				}
				if (num2 > 1119f)
				{
					num *= 1119f / num2;
					num2 = 1119f;
				}
				if (num > 1f && num2 > 1f)
				{
					PdfBitmap pdfBitmap = new PdfBitmap((int)num, (int)num2, BitmapFormats.FXDIB_Argb);
					pdfBitmap.FillRectEx(0, 0, pdfBitmap.Width, pdfBitmap.Height, -1);
					PageRotate pageRotate = PageRotate.Normal;
					Pdfium.FPDF_RenderPageBitmap(pdfBitmap.Handle, this.Document.Pages[this.PageIndex].Handle, 0, 0, pdfBitmap.Width, pdfBitmap.Height, pageRotate, RenderFlags.FPDF_ANNOT);
					this.Cropedimage.Source = CropPageWindow.GetBitmapImage((Bitmap)pdfBitmap.Image);
				}
			}
		}

		// Token: 0x06001CB5 RID: 7349 RVA: 0x000798D8 File Offset: 0x00077AD8
		public CropPageWindow(PdfDocument pdfDocument, int pageNum, FS_RECTF CropBox, MarginModel marginModel, PageSizeModel pageSizeModel, ScreenshotDialog ScreenshotDialog)
		{
			this.Document = pdfDocument;
			this.PageMargin2 = marginModel;
			this.PageSize = pageSizeModel;
			this.ScreenshotDialog2 = ScreenshotDialog;
			this.ScreenshotDialog2.setCopyDocument(pdfDocument);
			this.ScreenshotDialog2.PageIndexs = pageNum;
			this.PageIndex = pageNum;
			this.OriginalIndex = pageNum;
			this.rotate = this.Document.Pages[pageNum].Rotation;
			this.originalBox = CropBox;
			this.InitializeComponent();
			this.CropControl.IsChecked = new bool?(true);
			this.PageNumber.Text = this.Document.Pages.Count.ToString();
			this.PageindexNumbox.Maximum = (double)this.Document.Pages.Count;
			this.PART_VerticalScrollBar.Maximum = (double)this.Document.Pages.Count;
			if (ScreenshotDialog.Pages != null)
			{
				this.CustomTextBox.Text = ScreenshotDialog.Pages.ConvertToRange();
				this.PageindexNumbox.Value = (double)(this.PageIndex + 1);
				this.PART_VerticalScrollBar.Value = (double)(this.PageIndex + 1);
				this.GetSeletedPageItem(ScreenshotDialog.Pages);
				int[] array = this.ImportPageRange();
				if (array != null && !array.Contains(this.PageIndex))
				{
					this.CustomTextBox.Text = ScreenshotDialog.Pages.ConvertToRange();
					this.SubsetComboBox.SelectedIndex = 0;
					this.CurrentPageBtn.IsChecked = new bool?(true);
				}
			}
			else
			{
				this.CustomTextBox.Text = "";
				this.CurrentPageBtn.IsChecked = new bool?(true);
				this.PageindexNumbox.Value = (double)(pageNum + 1);
				this.PART_VerticalScrollBar.Value = (double)(pageNum + 1);
			}
			this.CurrentPage.Text = string.Format("({0} {1})", pdfeditor.Properties.Resources.PageResizeWinPageNumber, pageNum + 1);
			this.PageCount.Text = this.Document.Pages.Count.ToString();
			PageHeaderFooterUtils.PdfPointToCm((double)(CropBox.top - CropBox.bottom)).ToString("F2");
			PageHeaderFooterUtils.PdfPointToCm((double)(CropBox.right - CropBox.left)).ToString("F2");
			this.topNumBox.Maximum = PageHeaderFooterUtils.PdfPointToCm(this.PageMargin2.PageHeight - this.PageMargin2.Bottom);
			this.bottomNumbox.Maximum = PageHeaderFooterUtils.PdfPointToCm(this.PageMargin2.PageHeight - this.PageMargin2.Top);
			this.leftNumbox.Maximum = PageHeaderFooterUtils.PdfPointToCm(this.PageMargin2.PageWidth - this.PageMargin2.Right);
			this.rightNumbox.Maximum = PageHeaderFooterUtils.PdfPointToCm(this.PageMargin2.PageWidth - this.PageMargin2.Left);
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
			this.CropPageViewer.MouseMove += this.CropPageViewer_MouseMove;
			this.CropPageViewer.MouseDown += this.CropPageViewer_MouseDown;
			base.MouseUp += this.CropPageViewer_MouseUp;
			base.Closing += this.CropPageWindow_Closing;
			this.clickStartPosition = new global::System.Windows.Point(0.0, 0.0);
			float num = this.Document.Pages[this.PageIndex].Width;
			float num2 = this.Document.Pages[this.PageIndex].Height;
			if (num > 1f && num2 > 1f)
			{
				if (num > 810f)
				{
					num2 *= 810f / num;
					num = 810f;
				}
				if (num2 > 1119f)
				{
					num *= 1119f / num2;
					num2 = 1119f;
				}
				if (num > num2)
				{
					float num3 = num2 / num;
					this.cropBoder.Height = (double)(270f * num3 + 2f);
					this.Cropedimage.Height = (double)(270f * num3);
				}
				else
				{
					float num4 = num / num2;
					if (num4 * 373f > 270f)
					{
						this.cropBoder.Height = (double)(270f / num4 + 3f);
						this.Cropedimage.Height = (double)(270f / num4);
					}
					else
					{
						this.Cropedimage.Height = 373.0;
						this.cropBoder.Height = 376.0;
						this.Cropedimage.Width = (double)(373f * num4);
						this.cropBoder.Width = (double)(373f * num4 + 3f);
					}
				}
			}
			this.CreateCropbox(CropBox);
			GAManager.SendEvent("CropPage", "CropPageWindowShow", "Count", 1L);
		}

		// Token: 0x06001CB6 RID: 7350 RVA: 0x00079E0C File Offset: 0x0007800C
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

		// Token: 0x06001CB7 RID: 7351 RVA: 0x00079F80 File Offset: 0x00078180
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

		// Token: 0x06001CB8 RID: 7352 RVA: 0x00079FB8 File Offset: 0x000781B8
		private void CropPageWindow_Closing(object sender, CancelEventArgs e)
		{
			if (this.CustomTextBox.IsFocused)
			{
				this.IsClosing = true;
				this.btnCancel.Focus();
			}
			this.widthNumBox.ValueChanged -= this.widthNumBox_ValueChanged;
			this.heightNumbox.ValueChanged -= this.widthNumBox_ValueChanged;
			this.xNumBox.ValueChanged -= this.widthNumBox_ValueChanged;
			this.yNumbox.ValueChanged -= this.widthNumBox_ValueChanged;
			this.ScreenshotDialog2.RootBorder.Visibility = Visibility.Visible;
			this.ScreenshotDialog2.fixedRate = false;
			this.ScreenshotDialog2.startPt.X = this.originalBox.left;
			this.ScreenshotDialog2.startPt.Y = this.originalBox.top;
			this.ScreenshotDialog2.curPt.X = this.originalBox.right;
			this.ScreenshotDialog2.curPt.Y = this.originalBox.bottom;
			this.ScreenshotDialog2.UpdateBounds(false, false);
			this.ScreenshotDialog2.VM.ViewToolbar.DocSizeMode = SizeModes.FitToSize;
			this.Document.Dispose();
		}

		// Token: 0x06001CB9 RID: 7353 RVA: 0x0007A0F8 File Offset: 0x000782F8
		private void CropPageViewer_MouseDown(object sender, MouseButtonEventArgs e)
		{
			FrameworkElement frameworkElement = e.OriginalSource as FrameworkElement;
			if (frameworkElement != null && frameworkElement.Parent == this.cropRectGrid)
			{
				frameworkElement.CaptureMouse();
			}
		}

		// Token: 0x06001CBA RID: 7354 RVA: 0x0007A12C File Offset: 0x0007832C
		private void CropPageViewer_MouseUp(object sender, MouseButtonEventArgs e)
		{
			IInputElement captured = Mouse.Captured;
			if (captured != null)
			{
				captured.ReleaseMouseCapture();
			}
			this.clickStartPosition = new global::System.Windows.Point(0.0, 0.0);
			this.mousePosition = CropPageWindow.MousePosition.None;
			double left = Canvas.GetLeft(this.cropRectGrid);
			double bottom = Canvas.GetBottom(this.cropRectGrid);
			float num = (float)this.Cropedimage.Width;
			float num2 = (float)this.Cropedimage.Height;
			if (this.mandatoryCheckBox.IsChecked.GetValueOrDefault() && (left < 0.0 || bottom < 0.0 || left + this.cropRectangle.Width > 270.0 || bottom + this.cropRectangle.Height > 373.0))
			{
				this.mandatoryCheckBox.IsChecked = new bool?(false);
			}
			if (left < 0.0)
			{
				double num3 = this.cropRectangle.Width + left;
				if (num3 <= 30.0)
				{
					this.cropRectangle.Width = 30.0;
				}
				else
				{
					this.cropRectangle.Width = num3;
				}
				Canvas.SetLeft(this.cropRectGrid, 0.0);
			}
			if (bottom < 0.0)
			{
				double num4 = this.cropRectangle.Height + bottom;
				if (num4 <= 30.0)
				{
					this.cropRectangle.Height = 30.0;
				}
				else
				{
					this.cropRectangle.Height = num4;
				}
				Canvas.SetBottom(this.cropRectGrid, 0.0);
			}
			if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Normal || this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate180)
			{
				if (left + this.cropRectangle.Width > (double)num)
				{
					double num5 = (double)num - left;
					if (num5 <= 30.0)
					{
						this.cropRectangle.Width = 30.0;
						Canvas.SetLeft(this.cropRectGrid, (double)(num - 20f));
					}
					else
					{
						this.cropRectangle.Width = num5;
					}
				}
				if (bottom + this.cropRectangle.Height > (double)num2)
				{
					double num6 = (double)num2 - bottom;
					if (num6 <= 30.0)
					{
						this.cropRectangle.Height = 30.0;
						Canvas.SetBottom(this.cropRectGrid, (double)(num2 - 20f));
					}
					else
					{
						this.cropRectangle.Height = num6;
					}
				}
			}
			else if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate90 || this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate270)
			{
				if (left + this.cropRectangle.Width > (double)num)
				{
					double num7 = (double)num - left;
					if (num7 <= 30.0)
					{
						this.cropRectangle.Width = 30.0;
						Canvas.SetLeft(this.cropRectGrid, (double)(num - 20f));
					}
					else
					{
						this.cropRectangle.Width = num7;
					}
				}
				if (bottom + this.cropRectangle.Height > (double)num2)
				{
					double num8 = (double)num2 - bottom;
					if (num8 <= 30.0)
					{
						this.cropRectangle.Height = 30.0;
						Canvas.SetBottom(this.cropRectGrid, (double)(num2 - 20f));
					}
					else
					{
						this.cropRectangle.Height = num8;
					}
				}
			}
			this.UpdateData();
		}

		// Token: 0x06001CBB RID: 7355 RVA: 0x0007A4C4 File Offset: 0x000786C4
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

		// Token: 0x17000A3A RID: 2618
		// (get) Token: 0x06001CBC RID: 7356 RVA: 0x0007A528 File Offset: 0x00078728
		// (set) Token: 0x06001CBD RID: 7357 RVA: 0x0007A530 File Offset: 0x00078730
		private PdfDocument Document { get; set; }

		// Token: 0x06001CBE RID: 7358 RVA: 0x0007A53C File Offset: 0x0007873C
		private void cropboxChanged()
		{
			ComboBoxItem comboBoxItem = this.cropboxPaperSize.SelectedItem as ComboBoxItem;
			if (comboBoxItem.Content != null)
			{
				this.Iscenter.IsChecked = new bool?(true);
				if (comboBoxItem.Content.ToString() == "WithOut")
				{
					this.ScreenshotDialog2.ResetPages(this.PageIndex);
				}
				if (comboBoxItem.Content.ToString() == "A3")
				{
					double num = PageHeaderFooterUtils.CmToPdfPoint(29.7);
					double num2 = PageHeaderFooterUtils.CmToPdfPoint(42.0);
					this.ScreenshotDialog2.ResetPagesize(num2, num, this.Iscenter.IsChecked.Value, this.PageIndex);
				}
				if (comboBoxItem.Content.ToString() == "A2")
				{
					double num3 = PageHeaderFooterUtils.CmToPdfPoint(42.0);
					double num4 = PageHeaderFooterUtils.CmToPdfPoint(59.4);
					this.ScreenshotDialog2.ResetPagesize(num4, num3, this.Iscenter.IsChecked.Value, this.PageIndex);
				}
				if (comboBoxItem.Content.ToString() == "A1")
				{
					double num5 = PageHeaderFooterUtils.CmToPdfPoint(59.4);
					double num6 = PageHeaderFooterUtils.CmToPdfPoint(84.1);
					this.ScreenshotDialog2.ResetPagesize(num6, num5, this.Iscenter.IsChecked.Value, this.PageIndex);
				}
				if (comboBoxItem.Content.ToString() == "A0")
				{
					double num7 = PageHeaderFooterUtils.CmToPdfPoint(84.1);
					double num8 = PageHeaderFooterUtils.CmToPdfPoint(118.9);
					this.ScreenshotDialog2.ResetPagesize(num8, num7, this.Iscenter.IsChecked.Value, this.PageIndex);
				}
				FS_RECTF fs_RECTF = new FS_RECTF
				{
					right = this.ScreenshotDialog2.curPt.X,
					bottom = this.ScreenshotDialog2.curPt.Y,
					left = this.ScreenshotDialog2.startPt.X,
					top = this.ScreenshotDialog2.startPt.Y
				};
				this.UpdateCropbox(fs_RECTF);
			}
		}

		// Token: 0x06001CBF RID: 7359 RVA: 0x0007A788 File Offset: 0x00078988
		private void cropboxPaperSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.cropboxChanged();
		}

		// Token: 0x06001CC0 RID: 7360 RVA: 0x0007A790 File Offset: 0x00078990
		private void Iscenter_Unchecked(object sender, RoutedEventArgs e)
		{
			if (this.xNumBox != null && this.yNumbox != null)
			{
				this.xNumBox.IsEnabled = true;
				this.yNumbox.IsEnabled = true;
			}
		}

		// Token: 0x06001CC1 RID: 7361 RVA: 0x0007A7BC File Offset: 0x000789BC
		private void Iscenter_Checked(object sender, RoutedEventArgs e)
		{
			if (this.xNumBox != null && this.yNumbox != null)
			{
				this.xNumBox.IsEnabled = false;
				this.yNumbox.IsEnabled = false;
			}
			if (this.ScreenshotDialog2 != null && this.Cropedimage != null)
			{
				float height = this.PageSize.Height;
				float width = this.PageSize.Width;
				this.ScreenshotDialog2.ResetPages(this.PageIndex);
				this.ScreenshotDialog2.ResetPagesize((double)height, (double)width, true, this.PageIndex);
				this.PageSize = this.ScreenshotDialog2.reflashSizeData();
				this.PageMargin2 = this.ScreenshotDialog2.reflashData();
				this.setCropImage();
			}
		}

		// Token: 0x06001CC2 RID: 7362 RVA: 0x0007A868 File Offset: 0x00078A68
		private int GetImportPageCount()
		{
			if (string.IsNullOrEmpty(this.CustomTextBox.Text))
			{
				return 0;
			}
			int[] array = null;
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

		// Token: 0x06001CC3 RID: 7363 RVA: 0x0007A944 File Offset: 0x00078B44
		private int[] GetImportPageRange()
		{
			int[] array = null;
			if (this.AllPageBtn.IsChecked.GetValueOrDefault())
			{
				array = new int[this.Document.Pages.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = i;
				}
			}
			else if (this.CurrentPageBtn.IsChecked.GetValueOrDefault())
			{
				array = new int[] { this.OriginalIndex };
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

		// Token: 0x06001CC4 RID: 7364 RVA: 0x0007AA80 File Offset: 0x00078C80
		private int[] ImportPageRange()
		{
			int[] array = null;
			if (this.AllPageBtn.IsChecked.GetValueOrDefault())
			{
				array = new int[this.Document.Pages.Count];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = i;
				}
			}
			else if (this.CurrentPageBtn.IsChecked.GetValueOrDefault())
			{
				array = new int[] { this.OriginalIndex };
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
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinSignaturePageRangeIncorrect, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return null;
			}
			if (array.Length == 0)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.WinSignaturePageRangeIncorrect, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return null;
			}
			return array;
		}

		// Token: 0x06001CC5 RID: 7365 RVA: 0x0007ABE4 File Offset: 0x00078DE4
		private void MouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.clickStartPosition = e.GetPosition(this);
			if (sender == this.Topleft)
			{
				this.mousePosition = CropPageWindow.MousePosition.topLeft;
				return;
			}
			if (sender == this.Topright)
			{
				this.mousePosition = CropPageWindow.MousePosition.topRight;
				return;
			}
			if (sender == this.Bottomright)
			{
				this.mousePosition = CropPageWindow.MousePosition.bottomRight;
				return;
			}
			if (sender == this.Bottomleft)
			{
				this.mousePosition = CropPageWindow.MousePosition.bottomLeft;
				return;
			}
			if (sender == this.Topcenter)
			{
				this.mousePosition = CropPageWindow.MousePosition.topCenter;
				return;
			}
			if (sender == this.Leftcenter)
			{
				this.mousePosition = CropPageWindow.MousePosition.leftCenter;
				return;
			}
			if (sender == this.Rightcenter)
			{
				this.mousePosition = CropPageWindow.MousePosition.rightCenter;
				return;
			}
			if (sender == this.Bottomcenter)
			{
				this.mousePosition = CropPageWindow.MousePosition.bottomCenter;
			}
		}

		// Token: 0x06001CC6 RID: 7366 RVA: 0x0007AC85 File Offset: 0x00078E85
		private void Topleft_MouseUp(object sender, MouseButtonEventArgs e)
		{
			e.GetPosition(this);
			this.UpdateData();
		}

		// Token: 0x06001CC7 RID: 7367 RVA: 0x0007AC98 File Offset: 0x00078E98
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.ScreenshotDialog2.startPt.X = 0f - this.PageSize.xoffset;
			this.ScreenshotDialog2.startPt.Y = this.Document.Pages[this.PageIndex].MediaBox.Height - this.PageSize.yoffset;
			this.ScreenshotDialog2.curPt.X = this.Document.Pages[this.PageIndex].MediaBox.Width - this.PageSize.xoffset;
			this.ScreenshotDialog2.curPt.Y = 0f - this.PageSize.yoffset;
			this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
			this.PageMargin2 = this.ScreenshotDialog2.reflashData();
		}

		// Token: 0x06001CC8 RID: 7368 RVA: 0x0007AD84 File Offset: 0x00078F84
		private void ReSetBtn_Click(object sender, RoutedEventArgs e)
		{
			FS_RECTF fs_RECTF = this.originalBox;
			FS_RECTF mediaBox = this.Document.Pages[this.PageIndex].MediaBox;
			if (this.originalBox.left > mediaBox.Width || this.originalBox.bottom > mediaBox.Height || this.originalBox.top > mediaBox.Height || this.originalBox.right > mediaBox.Width)
			{
				this.ScreenshotDialog2.startPt.X = 0f;
				this.ScreenshotDialog2.startPt.Y = mediaBox.Height;
				this.ScreenshotDialog2.curPt.X = mediaBox.Width;
				this.ScreenshotDialog2.curPt.Y = 0f;
			}
			else
			{
				this.ScreenshotDialog2.startPt.X = this.originalBox.left;
				this.ScreenshotDialog2.startPt.Y = this.originalBox.top;
				this.ScreenshotDialog2.curPt.X = this.originalBox.right;
				this.ScreenshotDialog2.curPt.Y = this.originalBox.bottom;
			}
			this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
			this.PageMargin2 = this.ScreenshotDialog2.reflashData();
		}

		// Token: 0x06001CC9 RID: 7369 RVA: 0x0007AEE8 File Offset: 0x000790E8
		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			GAManager.SendEvent("CropPage", "CropPageWindowCancelBtn", "Count", 1L);
			this.ScreenshotDialog2.RootBorder.Visibility = Visibility.Visible;
			this.ScreenshotDialog2.startPt.X = this.originalBox.left;
			this.ScreenshotDialog2.startPt.Y = this.originalBox.top;
			this.ScreenshotDialog2.curPt.X = this.originalBox.right;
			this.ScreenshotDialog2.curPt.Y = this.originalBox.bottom;
			this.ScreenshotDialog2.UpdateBounds(false, false);
			this.ScreenshotDialog2.ScrollToPage();
			this.PageMargin2 = this.ScreenshotDialog2.reflashData();
			base.Close();
		}

		// Token: 0x06001CCA RID: 7370 RVA: 0x0007AFB8 File Offset: 0x000791B8
		private void OkBtn_Click(object sender, RoutedEventArgs e)
		{
			if (this.CustomTextBox.IsFocused)
			{
				this.btnOk.Focus();
			}
			GAManager.SendEvent("CropPage", "CropPageWindowOkBtn", this.GetImportPageCount().ToString(), 1L);
			int[] importPageRange = this.GetImportPageRange();
			if (importPageRange == null)
			{
				ModernMessageBox.Show(pdfeditor.Properties.Resources.LinkPageError, UtilManager.GetProductName(), MessageBoxButton.OK, MessageBoxResult.None, null, false);
				return;
			}
			if (this.CropControl.IsChecked.GetValueOrDefault())
			{
				this.ScreenshotDialog2.UpdateBounds(false, false);
			}
			this.ScreenshotDialog2.RootBorder.Visibility = Visibility.Visible;
			this.ScreenshotDialog2.UpdateCropPagemarginModel();
			this.ScreenshotDialog2.CropPageToolbar.ApplypageIndex = importPageRange;
			this.ScreenshotDialog2.CreateSeleteBounds(this.ResizeControl.IsChecked.Value);
			this.ScreenshotDialog2.CompleteCropPageAsync(importPageRange, this.ZeroCheckBox.IsChecked.Value);
			this.ScreenshotDialog2.RootBorder.Visibility = Visibility.Visible;
			base.Closing -= this.CropPageWindow_Closing;
			this.ScreenshotDialog2.UpdateBounds(false, false);
			this.Document.Dispose();
			base.Close();
		}

		// Token: 0x06001CCB RID: 7371 RVA: 0x0007B0E9 File Offset: 0x000792E9
		private void FirstPage_Click(object sender, RoutedEventArgs e)
		{
			this.PageindexNumbox.Value = 0.0;
		}

		// Token: 0x06001CCC RID: 7372 RVA: 0x0007B0FF File Offset: 0x000792FF
		private void BeforePage_Click(object sender, RoutedEventArgs e)
		{
			this.PageindexNumbox.Value = (double)this.PageIndex;
		}

		// Token: 0x06001CCD RID: 7373 RVA: 0x0007B113 File Offset: 0x00079313
		private void NextPage_Click(object sender, RoutedEventArgs e)
		{
			this.PageindexNumbox.Value = (double)(this.PageIndex + 2);
		}

		// Token: 0x06001CCE RID: 7374 RVA: 0x0007B129 File Offset: 0x00079329
		private void EndPage_Click(object sender, RoutedEventArgs e)
		{
			this.PageindexNumbox.Value = (double)this.Document.Pages.Count;
		}

		// Token: 0x06001CCF RID: 7375 RVA: 0x0007B148 File Offset: 0x00079348
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
			int[] array = this.ImportPageRange();
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
			this.ScreenshotDialog2.PageIndexs = num;
			if (this.ZeroCheckBox.IsChecked.Value)
			{
				this.SetZeroMargin();
			}
			else if (array != null && array.Contains(num))
			{
				this.GetNewCropBox();
			}
			this.ScreenshotDialog2.cropPageRotate = this.Document.Pages[this.PageIndex].Rotation;
			FS_RECTF fs_RECTF = new FS_RECTF
			{
				right = this.ScreenshotDialog2.curPt.X,
				bottom = this.ScreenshotDialog2.curPt.Y,
				left = this.ScreenshotDialog2.startPt.X,
				top = this.ScreenshotDialog2.startPt.Y
			};
			if (this.ResizeControl.IsChecked.Value)
			{
				this.ScreenshotDialog2.ResetPagesize((double)this.PageSize.Height, (double)this.PageSize.Width, this.Iscenter.IsChecked.Value, this.PageIndex);
			}
			else
			{
				this.ScreenshotDialog2.ResetPages(this.PageIndex);
			}
			float num2 = this.Document.Pages[this.PageIndex].Width;
			float num3 = this.Document.Pages[this.PageIndex].Height;
			if (num2 > 1f && num3 > 1f)
			{
				if (num2 > 810f)
				{
					num3 *= 810f / num2;
					num2 = 810f;
				}
				if (num3 > 1119f)
				{
					num2 *= 1119f / num3;
					num3 = 1119f;
				}
				if (num2 > num3)
				{
					float num4 = num3 / num2;
					this.cropBoder.Height = (double)(270f * num4 + 2f);
					this.Cropedimage.Height = (double)(270f * num4);
				}
				else
				{
					float num5 = num2 / num3;
					if (num5 * 373f > 270f)
					{
						this.cropBoder.Height = (double)(270f / num5 + 3f);
						this.Cropedimage.Height = (double)(270f / num5);
					}
					else
					{
						this.Cropedimage.Height = 373.0;
						this.cropBoder.Height = 376.0;
						this.Cropedimage.Width = (double)(373f * num5);
						this.cropBoder.Width = (double)(373f * num5 + 3f);
					}
				}
			}
			this.UpdateCropbox(fs_RECTF);
			if (array == null)
			{
				this.cropRectGrid.Visibility = Visibility.Collapsed;
				this.CropArea.IsEnabled = false;
				return;
			}
			if (this.PART_VerticalScrollBar.Value != (double)(this.PageIndex + 1))
			{
				this.PART_VerticalScrollBar.Value = (double)(this.PageIndex + 1);
			}
			if (array.Contains(num))
			{
				this.cropRectGrid.Visibility = Visibility.Visible;
				this.CropArea.IsEnabled = true;
				this.CropArea.Visibility = Visibility.Visible;
				this.WithoutPagesArea.Visibility = Visibility.Collapsed;
				if (array.Contains(num) && !this.ResizeControl.IsChecked.Value)
				{
					this.cropRectGrid.Visibility = Visibility.Visible;
					this.CropArea.IsEnabled = true;
				}
				else
				{
					this.cropRectGrid.Visibility = Visibility.Collapsed;
					this.CropArea.IsEnabled = false;
				}
				this.UpdateData();
				return;
			}
			this.cropRectGrid.Visibility = Visibility.Collapsed;
			this.CropArea.IsEnabled = false;
			this.CropArea.Visibility = Visibility.Collapsed;
			this.WithoutPagesArea.Visibility = Visibility.Visible;
		}

		// Token: 0x06001CD0 RID: 7376 RVA: 0x0007B608 File Offset: 0x00079808
		private void GetNewCropBox()
		{
			FS_RECTF mediaBox = this.Document.Pages[this.PageIndex].MediaBox;
			float num = (float)this.PageMargin2.Left;
			float num2 = mediaBox.Height - (float)this.PageMargin2.Top;
			float num3 = mediaBox.Width - (float)this.PageMargin2.Right;
			float num4 = (float)this.PageMargin2.Bottom;
			if (num > mediaBox.Width || num2 > mediaBox.Height || num3 > mediaBox.Width || num4 > mediaBox.Height || num2 < 0f || num3 < 0f)
			{
				num = 0f;
				num2 = mediaBox.Height;
				num3 = mediaBox.Width;
				num4 = 0f;
			}
			this.ScreenshotDialog2.startPt.X = num;
			this.ScreenshotDialog2.startPt.Y = num2;
			this.ScreenshotDialog2.curPt.X = num3;
			this.ScreenshotDialog2.curPt.Y = num4;
			this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
			this.PageMargin2 = this.ScreenshotDialog2.reflashData();
		}

		// Token: 0x06001CD1 RID: 7377 RVA: 0x0007B730 File Offset: 0x00079930
		private void widthNumBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			FS_RECTF fs_RECTF = new FS_RECTF
			{
				right = this.ScreenshotDialog2.curPt.X,
				bottom = this.ScreenshotDialog2.curPt.Y,
				left = this.ScreenshotDialog2.startPt.X,
				top = this.ScreenshotDialog2.startPt.Y
			};
			this.UpdateCropbox(fs_RECTF);
		}

		// Token: 0x06001CD2 RID: 7378 RVA: 0x0007B7AA File Offset: 0x000799AA
		private void ResizeControl_Checked(object sender, RoutedEventArgs e)
		{
			this.cropRectGrid.Visibility = Visibility.Collapsed;
			this.BoxSize.Visibility = Visibility.Collapsed;
			this.CropArea.IsEnabled = false;
			this.cropboxChanged();
		}

		// Token: 0x06001CD3 RID: 7379 RVA: 0x0007B7D8 File Offset: 0x000799D8
		private void ResizeControl_Unchecked(object sender, RoutedEventArgs e)
		{
			if (this.Document == null)
			{
				return;
			}
			int[] array = this.ImportPageRange();
			this.BoxSize.Visibility = Visibility.Visible;
			if (array != null && array.Contains(this.PageIndex))
			{
				this.cropRectGrid.Visibility = Visibility.Visible;
			}
			this.CropArea.IsEnabled = true;
			FS_RECTF fs_RECTF = this.originalBox;
			this.ScreenshotDialog2.startPt.X = this.originalBox.left;
			this.ScreenshotDialog2.startPt.Y = this.originalBox.top;
			this.ScreenshotDialog2.curPt.X = this.originalBox.right;
			this.ScreenshotDialog2.curPt.Y = this.originalBox.bottom;
			this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
			this.PageMargin2 = this.ScreenshotDialog2.reflashData();
			this.ScreenshotDialog2.ResetPages(this.PageIndex);
			this.UpdateCropbox(this.originalBox);
			this.UpdateData();
		}

		// Token: 0x06001CD4 RID: 7380 RVA: 0x0007B8E0 File Offset: 0x00079AE0
		private void SetZeroMargin()
		{
			List<FS_RECTF> list = new List<FS_RECTF>();
			foreach (PdfPageObject pdfPageObject in this.Document.Pages[this.PageIndex].PageObjects)
			{
				list.Add(pdfPageObject.BoundingBox);
			}
			FS_RECTF mediaBox = this.Document.Pages[this.PageIndex].MediaBox;
			float num = mediaBox.Width;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = mediaBox.Height;
			for (int i = 0; i < list.Count; i++)
			{
				num = Math.Min(list[i].left, num);
				num2 = Math.Max(list[i].top, num2);
				num3 = Math.Max(list[i].right, num3);
				num4 = Math.Min(list[i].bottom, num4);
			}
			if (num < 0f)
			{
				num = 0f;
			}
			if (num2 > mediaBox.Height)
			{
				num2 = mediaBox.Height;
			}
			if (num3 > mediaBox.Width)
			{
				num3 = mediaBox.Width;
			}
			if (num4 < 0f)
			{
				num4 = 0f;
			}
			this.ScreenshotDialog2.startPt.X = num;
			this.ScreenshotDialog2.startPt.Y = num2;
			this.ScreenshotDialog2.curPt.X = num3;
			this.ScreenshotDialog2.curPt.Y = num4;
			this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
			this.PageMargin2 = this.ScreenshotDialog2.reflashData();
		}

		// Token: 0x06001CD5 RID: 7381 RVA: 0x0007BAA0 File Offset: 0x00079CA0
		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			this.SetZeroMargin();
			this.SetzeroBtn.IsEnabled = false;
			this.ReSetBtn.IsEnabled = false;
			this.topNumBox.IsEnabled = false;
			this.bottomNumbox.IsEnabled = false;
			this.leftNumbox.IsEnabled = false;
			this.rightNumbox.IsEnabled = false;
			this.Topcenter.Visibility = Visibility.Collapsed;
			this.Leftcenter.Visibility = Visibility.Collapsed;
			this.Bottomcenter.Visibility = Visibility.Collapsed;
			this.Rightcenter.Visibility = Visibility.Collapsed;
			this.Topright.Visibility = Visibility.Collapsed;
			this.Bottomright.Visibility = Visibility.Collapsed;
			this.Topleft.Visibility = Visibility.Collapsed;
			this.Bottomleft.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06001CD6 RID: 7382 RVA: 0x0007BB5C File Offset: 0x00079D5C
		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			FS_RECTF fs_RECTF = this.originalBox;
			this.ScreenshotDialog2.startPt.X = this.originalBox.left;
			this.ScreenshotDialog2.startPt.Y = this.originalBox.top;
			this.ScreenshotDialog2.curPt.X = this.originalBox.right;
			this.ScreenshotDialog2.curPt.Y = this.originalBox.bottom;
			this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
			this.PageMargin2 = this.ScreenshotDialog2.reflashData();
			this.SetzeroBtn.IsEnabled = true;
			this.ReSetBtn.IsEnabled = true;
			this.topNumBox.IsEnabled = true;
			this.bottomNumbox.IsEnabled = true;
			this.leftNumbox.IsEnabled = true;
			this.rightNumbox.IsEnabled = true;
			this.mandatoryCheckBox.IsChecked = new bool?(false);
			this.Topcenter.Visibility = Visibility.Visible;
			this.Leftcenter.Visibility = Visibility.Visible;
			this.Bottomcenter.Visibility = Visibility.Visible;
			this.Rightcenter.Visibility = Visibility.Visible;
			this.Topright.Visibility = Visibility.Visible;
			this.Bottomright.Visibility = Visibility.Visible;
			this.Topleft.Visibility = Visibility.Visible;
			this.Bottomleft.Visibility = Visibility.Visible;
		}

		// Token: 0x06001CD7 RID: 7383 RVA: 0x0007BCB3 File Offset: 0x00079EB3
		private void mandatoryCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			this.ScreenshotDialog2.fixedRate = true;
			this.Topcenter.Visibility = Visibility.Collapsed;
			this.Leftcenter.Visibility = Visibility.Collapsed;
			this.Bottomcenter.Visibility = Visibility.Collapsed;
			this.Rightcenter.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06001CD8 RID: 7384 RVA: 0x0007BCF1 File Offset: 0x00079EF1
		private void mandatoryCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			this.ScreenshotDialog2.fixedRate = false;
			this.Topcenter.Visibility = Visibility.Visible;
			this.Leftcenter.Visibility = Visibility.Visible;
			this.Bottomcenter.Visibility = Visibility.Visible;
			this.Rightcenter.Visibility = Visibility.Visible;
		}

		// Token: 0x06001CD9 RID: 7385 RVA: 0x0007BD30 File Offset: 0x00079F30
		private void MandatoryBox(bool Ischecked)
		{
			if (Ischecked)
			{
				PdfPage pdfPage = this.Document.Pages[this.PageIndex];
				float num = (pdfPage.Width - this.ScreenshotDialog2.curPt.X + this.ScreenshotDialog2.startPt.X) / 2f;
				float num2 = (pdfPage.Height - this.ScreenshotDialog2.startPt.Y + this.ScreenshotDialog2.curPt.Y) / 2f;
				float num3 = this.ScreenshotDialog2.curPt.X - this.ScreenshotDialog2.startPt.X + num;
				float num4 = this.ScreenshotDialog2.startPt.Y - this.ScreenshotDialog2.curPt.Y + num2;
				this.ScreenshotDialog2.startPt.X = num;
				this.ScreenshotDialog2.startPt.Y = num4;
				this.ScreenshotDialog2.curPt.X = num3;
				this.ScreenshotDialog2.curPt.Y = num2;
				this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
				this.PageMargin2 = this.ScreenshotDialog2.reflashData();
				return;
			}
			FS_RECTF fs_RECTF = this.originalBox;
			this.ScreenshotDialog2.startPt.X = this.originalBox.left;
			this.ScreenshotDialog2.startPt.Y = this.originalBox.top;
			this.ScreenshotDialog2.curPt.X = this.originalBox.right;
			this.ScreenshotDialog2.curPt.Y = this.originalBox.bottom;
			this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
			this.PageMargin2 = this.ScreenshotDialog2.reflashData();
		}

		// Token: 0x06001CDA RID: 7386 RVA: 0x0007BEF2 File Offset: 0x0007A0F2
		private void PART_VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.PageindexNumbox != null)
			{
				this.PageindexNumbox.Value = e.NewValue;
			}
		}

		// Token: 0x06001CDB RID: 7387 RVA: 0x0007BF10 File Offset: 0x0007A110
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

		// Token: 0x06001CDC RID: 7388 RVA: 0x0007BF70 File Offset: 0x0007A170
		private void UpdateUnitMaxValue()
		{
			ComboBoxItem comboBoxItem = this.UnitComboBox.SelectedItem as ComboBoxItem;
			if (comboBoxItem.Content != null && this.cropRectangle != null)
			{
				if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate90)
				{
					this.rightNumbox.Maximum = this.PageMargin2.PageHeight - this.PageMargin2.Bottom;
					this.leftNumbox.Maximum = this.PageMargin2.PageHeight - this.PageMargin2.Top;
					this.topNumBox.Maximum = this.PageMargin2.PageWidth - this.PageMargin2.Right;
					this.bottomNumbox.Maximum = this.PageMargin2.PageWidth - this.PageMargin2.Left;
				}
				else if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate180)
				{
					this.bottomNumbox.Maximum = this.PageMargin2.PageHeight - this.PageMargin2.Bottom;
					this.topNumBox.Maximum = this.PageMargin2.PageHeight - this.PageMargin2.Top;
					this.rightNumbox.Maximum = this.PageMargin2.PageWidth - this.PageMargin2.Right;
					this.leftNumbox.Maximum = this.PageMargin2.PageWidth - this.PageMargin2.Left;
				}
				else if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate270)
				{
					this.leftNumbox.Maximum = this.PageMargin2.PageHeight - this.PageMargin2.Bottom;
					this.rightNumbox.Maximum = this.PageMargin2.PageHeight - this.PageMargin2.Top;
					this.bottomNumbox.Maximum = this.PageMargin2.PageWidth - this.PageMargin2.Right;
					this.topNumBox.Maximum = this.PageMargin2.PageWidth - this.PageMargin2.Left;
				}
				else
				{
					this.topNumBox.Maximum = this.PageMargin2.PageHeight - this.PageMargin2.Bottom;
					this.bottomNumbox.Maximum = this.PageMargin2.PageHeight - this.PageMargin2.Top;
					this.leftNumbox.Maximum = this.PageMargin2.PageWidth - this.PageMargin2.Right;
					this.rightNumbox.Maximum = this.PageMargin2.PageWidth - this.PageMargin2.Left;
				}
				double num = this.PageMargin2.PageHeight - this.PageMargin2.Bottom;
				double num2 = this.PageMargin2.PageHeight - this.PageMargin2.Top;
				double num3 = this.PageMargin2.PageWidth - this.PageMargin2.Right;
				double num4 = this.PageMargin2.PageWidth - this.PageMargin2.Left;
				if (comboBoxItem.Name.ToString().ToLower() == "cm")
				{
					this.ScreenshotDialog2.units = ScreenshotDialog.Dimensionunits.CM;
					this.ScreenshotDialog2.UpdateCropPagemarginModel();
					this.PageMargin2 = this.ScreenshotDialog2.reflashData();
					num = PageHeaderFooterUtils.PdfPointToCm(this.PageMargin2.PageHeight - this.PageMargin2.Bottom);
					num2 = PageHeaderFooterUtils.PdfPointToCm(this.PageMargin2.PageHeight - this.PageMargin2.Top);
					num3 = PageHeaderFooterUtils.PdfPointToCm(this.PageMargin2.PageWidth - this.PageMargin2.Right);
					num4 = PageHeaderFooterUtils.PdfPointToCm(this.PageMargin2.PageWidth - this.PageMargin2.Left);
				}
				if (comboBoxItem.Name.ToString().ToLower() == "mm")
				{
					this.ScreenshotDialog2.units = ScreenshotDialog.Dimensionunits.MM;
					this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
					this.PageMargin2 = this.ScreenshotDialog2.reflashData();
					num = PageHeaderFooterUtils.PdfPointToCm(this.PageMargin2.PageHeight - this.PageMargin2.Bottom) * 10.0;
					num2 = PageHeaderFooterUtils.PdfPointToCm(this.PageMargin2.PageHeight - this.PageMargin2.Top) * 10.0;
					num3 = PageHeaderFooterUtils.PdfPointToCm(this.PageMargin2.PageWidth - this.PageMargin2.Right) * 10.0;
					num4 = PageHeaderFooterUtils.PdfPointToCm(this.PageMargin2.PageWidth - this.PageMargin2.Left) * 10.0;
				}
				if (comboBoxItem.Name.ToString().ToLower() == "point")
				{
					this.ScreenshotDialog2.units = ScreenshotDialog.Dimensionunits.Point;
					this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
					this.PageMargin2 = this.ScreenshotDialog2.reflashData();
					num = this.PageMargin2.PageHeight - this.PageMargin2.Bottom;
					num2 = this.PageMargin2.PageHeight - this.PageMargin2.Top;
					num3 = this.PageMargin2.PageWidth - this.PageMargin2.Right;
					num4 = this.PageMargin2.PageWidth - this.PageMargin2.Left;
				}
				if (comboBoxItem.Name.ToString().ToLower() == "pica")
				{
					this.ScreenshotDialog2.units = ScreenshotDialog.Dimensionunits.Pica;
					this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
					this.PageMargin2 = this.ScreenshotDialog2.reflashData();
					num = (this.PageMargin2.PageHeight - this.PageMargin2.Bottom) / 12.0;
					num2 = (this.PageMargin2.PageHeight - this.PageMargin2.Top) / 12.0;
					num3 = (this.PageMargin2.PageWidth - this.PageMargin2.Right) / 12.0;
					num4 = (this.PageMargin2.PageWidth - this.PageMargin2.Left) / 12.0;
				}
				if (comboBoxItem.Name.ToString().ToLower() == "inch")
				{
					this.ScreenshotDialog2.units = ScreenshotDialog.Dimensionunits.Inch;
					this.ScreenshotDialog2.UpdatePriviewBounds(false, false);
					this.PageMargin2 = this.ScreenshotDialog2.reflashData();
					num = (this.PageMargin2.PageHeight - this.PageMargin2.Bottom) / 72.0;
					num2 = (this.PageMargin2.PageHeight - this.PageMargin2.Top) / 72.0;
					num3 = (this.PageMargin2.PageWidth - this.PageMargin2.Right) / 72.0;
					num4 = (this.PageMargin2.PageWidth - this.PageMargin2.Left) / 72.0;
				}
				if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate90)
				{
					this.rightNumbox.Maximum = num;
					this.leftNumbox.Maximum = num2;
					this.topNumBox.Maximum = num3;
					this.bottomNumbox.Maximum = num4;
					return;
				}
				if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate180)
				{
					this.bottomNumbox.Maximum = num;
					this.topNumBox.Maximum = num2;
					this.rightNumbox.Maximum = num3;
					this.leftNumbox.Maximum = num4;
					return;
				}
				if (this.Document.Pages[this.PageIndex].Rotation == PageRotate.Rotate270)
				{
					this.leftNumbox.Maximum = num;
					this.rightNumbox.Maximum = num2;
					this.bottomNumbox.Maximum = num3;
					this.topNumBox.Maximum = num4;
					return;
				}
				this.topNumBox.Maximum = num;
				this.bottomNumbox.Maximum = num2;
				this.leftNumbox.Maximum = num3;
				this.rightNumbox.Maximum = num4;
			}
		}

		// Token: 0x06001CDD RID: 7389 RVA: 0x0007C7A8 File Offset: 0x0007A9A8
		private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.UpdateUnitMaxValue();
			if (e.AddedItems.Count > 0 && this.BoxSize != null)
			{
				string text = (e.AddedItems[0] as ComboBoxItem).Name.ToString().ToLower();
				float num = this.ScreenshotDialog2.startPt.Y - this.ScreenshotDialog2.curPt.Y;
				float num2 = this.ScreenshotDialog2.curPt.X - this.ScreenshotDialog2.startPt.X;
				if (text == "cm")
				{
					string text2 = PageHeaderFooterUtils.PdfPointToCm((double)num).ToString("F2");
					string text3 = PageHeaderFooterUtils.PdfPointToCm((double)num2).ToString("F2");
					this.BoxSize.Text = string.Format("{0} * {1} {2}", text3, text2, (e.AddedItems[0] as ComboBoxItem).Content);
				}
				if (text == "mm")
				{
					string text4 = (PageHeaderFooterUtils.PdfPointToCm((double)num) * 10.0).ToString("F2");
					string text5 = (PageHeaderFooterUtils.PdfPointToCm((double)num2) * 10.0).ToString("F2");
					this.BoxSize.Text = string.Format("{0} * {1} {2}", text5, text4, (e.AddedItems[0] as ComboBoxItem).Content);
				}
				if (text == "point")
				{
					string text6 = num.ToString("F2");
					string text7 = num2.ToString("F2");
					this.BoxSize.Text = string.Format("{0} * {1} {2}", text7, text6, (e.AddedItems[0] as ComboBoxItem).Content);
				}
				if (text == "pica")
				{
					string text8 = (num / 12f).ToString("F2");
					string text9 = (num2 / 12f).ToString("F2");
					this.BoxSize.Text = string.Format("{0} * {1} {2}", text9, text8, (e.AddedItems[0] as ComboBoxItem).Content);
				}
				if (text == "inch")
				{
					string text10 = (num / 72f).ToString("F2");
					string text11 = (num2 / 72f).ToString("F2");
					this.BoxSize.Text = string.Format("{0} * {1} {2}", text11, text10, (e.AddedItems[0] as ComboBoxItem).Content);
				}
			}
		}

		// Token: 0x06001CDE RID: 7390 RVA: 0x0007CA4A File Offset: 0x0007AC4A
		private void CustomTextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			base.Dispatcher.InvokeAsync(delegate
			{
				if (this.btnCancel.IsFocused)
				{
					return;
				}
				this.<CustomTextBox_LostFocus>g__DoLostFocus|73_1();
			}, DispatcherPriority.Loaded);
		}

		// Token: 0x06001CDF RID: 7391 RVA: 0x0007CA68 File Offset: 0x0007AC68
		private void SubsetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int[] array = this.ImportPageRange();
			if (array == null)
			{
				(sender as ComboBox).SelectedIndex = 0;
				e.Handled = true;
				return;
			}
			if (this.PageindexNumbox != null)
			{
				if (array.Contains((int)this.PageindexNumbox.Value - 1))
				{
					this.cropRectGrid.Visibility = Visibility.Visible;
					this.CropArea.IsEnabled = true;
					this.CropArea.Visibility = Visibility.Visible;
					this.WithoutPagesArea.Visibility = Visibility.Collapsed;
					return;
				}
				this.cropRectGrid.Visibility = Visibility.Collapsed;
				this.CropArea.IsEnabled = false;
				this.CropArea.Visibility = Visibility.Collapsed;
				this.WithoutPagesArea.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06001CE0 RID: 7392 RVA: 0x0007CB14 File Offset: 0x0007AD14
		private void PageBtn_Checked(object sender, RoutedEventArgs e)
		{
			if (this.CustomTextBox == null)
			{
				return;
			}
			int[] array = this.ImportPageRange();
			if (array != null && this.PageindexNumbox != null)
			{
				if (array.Contains((int)this.PageindexNumbox.Value - 1))
				{
					this.cropRectGrid.Visibility = Visibility.Visible;
					this.CropArea.IsEnabled = true;
					this.CropArea.Visibility = Visibility.Visible;
					this.WithoutPagesArea.Visibility = Visibility.Collapsed;
					return;
				}
				this.cropRectGrid.Visibility = Visibility.Collapsed;
				this.CropArea.IsEnabled = false;
				this.CropArea.Visibility = Visibility.Collapsed;
				this.WithoutPagesArea.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06001CE8 RID: 7400 RVA: 0x0007D534 File Offset: 0x0007B734
		[CompilerGenerated]
		private void <CustomTextBox_LostFocus>g__DoLostFocus|73_1()
		{
			if (this.IsClosing)
			{
				return;
			}
			int[] array = this.ImportPageRange();
			FS_RECTF fs_RECTF = new FS_RECTF
			{
				right = this.ScreenshotDialog2.curPt.X,
				bottom = this.ScreenshotDialog2.curPt.Y,
				left = this.ScreenshotDialog2.startPt.X,
				top = this.ScreenshotDialog2.startPt.Y
			};
			if (this.ResizeControl.IsChecked.Value)
			{
				this.ScreenshotDialog2.ResetPagesize((double)this.PageSize.Height, (double)this.PageSize.Width, this.Iscenter.IsChecked.Value, this.PageIndex);
			}
			else
			{
				this.ScreenshotDialog2.ResetPages(this.PageIndex);
			}
			this.UpdateCropbox(fs_RECTF);
			if (array == null)
			{
				this.cropRectGrid.Visibility = Visibility.Collapsed;
				this.CropArea.IsEnabled = false;
				return;
			}
			if (array.Contains(this.PageIndex))
			{
				this.GetNewCropBox();
				this.cropRectGrid.Visibility = Visibility.Visible;
				this.CropArea.IsEnabled = true;
				this.CropArea.Visibility = Visibility.Visible;
				this.WithoutPagesArea.Visibility = Visibility.Collapsed;
			}
			else
			{
				this.cropRectGrid.Visibility = Visibility.Collapsed;
				this.CropArea.IsEnabled = false;
				this.CropArea.Visibility = Visibility.Collapsed;
				this.WithoutPagesArea.Visibility = Visibility.Visible;
			}
			if (array.Contains(this.PageIndex) && !this.ResizeControl.IsChecked.Value)
			{
				this.cropRectGrid.Visibility = Visibility.Visible;
				this.CropArea.IsEnabled = true;
			}
			else
			{
				this.cropRectGrid.Visibility = Visibility.Collapsed;
				this.CropArea.IsEnabled = false;
			}
			if (this.PART_VerticalScrollBar.Value != (double)(this.PageIndex + 1))
			{
				this.PART_VerticalScrollBar.Value = (double)(this.PageIndex + 1);
			}
		}

		// Token: 0x04000A84 RID: 2692
		private int PageIndex = -1;

		// Token: 0x04000A85 RID: 2693
		private FS_RECTF originalBox;

		// Token: 0x04000A86 RID: 2694
		public static readonly DependencyProperty PageMargin2Property = DependencyProperty.Register("PageMargin2", typeof(MarginModel), typeof(CropPageWindow), new PropertyMetadata(null));

		// Token: 0x04000A87 RID: 2695
		public static readonly DependencyProperty ScreenshotDialog2Property = DependencyProperty.Register("ScreenshotDialog2", typeof(ScreenshotDialog), typeof(CropPageWindow), new PropertyMetadata(null));

		// Token: 0x04000A88 RID: 2696
		public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register("PageSize2", typeof(PageSizeModel), typeof(CropPageWindow), new PropertyMetadata(null));

		// Token: 0x04000A89 RID: 2697
		private PageRotate rotate;

		// Token: 0x04000A8A RID: 2698
		private int OriginalIndex;

		// Token: 0x04000A8B RID: 2699
		private bool IsClosing;

		// Token: 0x04000A8D RID: 2701
		public CropPageWindow.MousePosition mousePosition = CropPageWindow.MousePosition.None;

		// Token: 0x04000A8E RID: 2702
		public global::System.Windows.Point clickStartPosition;

		// Token: 0x02000634 RID: 1588
		public enum MousePosition
		{
			// Token: 0x040020A2 RID: 8354
			topLeft,
			// Token: 0x040020A3 RID: 8355
			topRight,
			// Token: 0x040020A4 RID: 8356
			bottomLeft,
			// Token: 0x040020A5 RID: 8357
			bottomRight,
			// Token: 0x040020A6 RID: 8358
			topCenter,
			// Token: 0x040020A7 RID: 8359
			leftCenter,
			// Token: 0x040020A8 RID: 8360
			rightCenter,
			// Token: 0x040020A9 RID: 8361
			bottomCenter,
			// Token: 0x040020AA RID: 8362
			None
		}
	}
}
