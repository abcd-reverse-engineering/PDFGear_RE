using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Patagames.Pdf.Enums;
using pdfeditor.Properties;
using PDFKit.Utils.PageHeaderFooters;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x0200021F RID: 543
	public class MarginModel : ObservableObject
	{
		// Token: 0x17000A88 RID: 2696
		// (get) Token: 0x06001E5A RID: 7770 RVA: 0x00085DB3 File Offset: 0x00083FB3
		// (set) Token: 0x06001E5B RID: 7771 RVA: 0x00085DBB File Offset: 0x00083FBB
		public ScreenshotDialog Screenshot { get; set; }

		// Token: 0x06001E5C RID: 7772 RVA: 0x00085DC4 File Offset: 0x00083FC4
		private void fixedCropBoxLRRate(double changevalue)
		{
			if (2.0 * changevalue + this.left + this.right >= this.pagewidth - 10.0)
			{
				MessageBox.Show(Resources.CropPageExceedingRange);
				return;
			}
			float num = (this.Screenshot.startPt.Y - this.Screenshot.curPt.Y) / (this.Screenshot.curPt.X - this.Screenshot.startPt.X);
			base.SetProperty<double>(ref this.top, this.top + changevalue * (double)num, "fixedCropBoxLRRate");
			base.SetProperty<double>(ref this.right, this.right + changevalue, "fixedCropBoxLRRate");
			base.SetProperty<double>(ref this.bottom, this.bottom + changevalue * (double)num, "fixedCropBoxLRRate");
			base.SetProperty<double>(ref this.left, this.left + changevalue, "fixedCropBoxLRRate");
			base.OnPropertyChanged("TopCm");
			base.OnPropertyChanged("LeftCm");
			base.OnPropertyChanged("RightCm");
			base.OnPropertyChanged("BottomCm");
			if (this.Screenshot != null)
			{
				this.Screenshot.curPt.X = (float)(this.pagewidth - this.Right);
				this.Screenshot.curPt.Y = (float)this.Bottom;
				this.Screenshot.startPt.Y = (float)(this.pageheight - this.Top);
				this.Screenshot.startPt.X = (float)this.Left;
				this.Screenshot.UpdatePriviewBounds(false, true);
			}
		}

		// Token: 0x06001E5D RID: 7773 RVA: 0x00085F68 File Offset: 0x00084168
		private void fixedCropBoxTBRate(double changevalue)
		{
			if (this.top + 2.0 * changevalue + this.bottom >= this.pageheight - 10.0)
			{
				MessageBox.Show("Exceeding the legal range");
				return;
			}
			float num = (this.Screenshot.curPt.X - this.Screenshot.startPt.X) / (this.Screenshot.startPt.Y - this.Screenshot.curPt.Y);
			base.SetProperty<double>(ref this.top, this.top + changevalue, "fixedCropBoxTBRate");
			base.SetProperty<double>(ref this.right, this.right + changevalue * (double)num, "fixedCropBoxTBRate");
			base.SetProperty<double>(ref this.bottom, this.bottom + changevalue, "fixedCropBoxTBRate");
			base.SetProperty<double>(ref this.left, this.left + changevalue * (double)num, "fixedCropBoxTBRate");
			base.OnPropertyChanged("TopCm");
			base.OnPropertyChanged("LeftCm");
			base.OnPropertyChanged("RightCm");
			base.OnPropertyChanged("BottomCm");
			if (this.Screenshot != null)
			{
				this.Screenshot.curPt.X = (float)(this.pagewidth - this.Right);
				this.Screenshot.curPt.Y = (float)this.Bottom;
				this.Screenshot.startPt.Y = (float)(this.pageheight - this.Top);
				this.Screenshot.startPt.X = (float)this.Left;
				this.Screenshot.UpdatePriviewBounds(false, true);
			}
		}

		// Token: 0x17000A89 RID: 2697
		// (get) Token: 0x06001E5E RID: 7774 RVA: 0x00086109 File Offset: 0x00084309
		// (set) Token: 0x06001E5F RID: 7775 RVA: 0x00086114 File Offset: 0x00084314
		public double Top
		{
			get
			{
				return this.top;
			}
			set
			{
				if (this.Screenshot != null && this.Screenshot.fixedRate)
				{
					this.fixedCropBoxTBRate(value - this.top);
					return;
				}
				if (base.SetProperty<double>(ref this.top, value, "Top"))
				{
					base.OnPropertyChanged("TopCm");
					if (this.Screenshot != null)
					{
						this.Screenshot.curPt.X = (float)(this.pagewidth - this.Right);
						this.Screenshot.curPt.Y = (float)this.Bottom;
						this.Screenshot.startPt.Y = (float)(this.pageheight - this.Top);
						this.Screenshot.startPt.X = (float)this.Left;
						this.Screenshot.UpdatePriviewBounds(false, true);
					}
				}
			}
		}

		// Token: 0x17000A8A RID: 2698
		// (get) Token: 0x06001E60 RID: 7776 RVA: 0x000861E6 File Offset: 0x000843E6
		// (set) Token: 0x06001E61 RID: 7777 RVA: 0x000861F0 File Offset: 0x000843F0
		public double Bottom
		{
			get
			{
				return this.bottom;
			}
			set
			{
				if (this.Screenshot != null && this.Screenshot.fixedRate)
				{
					this.fixedCropBoxTBRate(value - this.bottom);
					return;
				}
				if (base.SetProperty<double>(ref this.bottom, value, "Bottom"))
				{
					base.OnPropertyChanged("BottomCm");
					if (this.Screenshot != null)
					{
						this.Screenshot.curPt.X = (float)(this.pagewidth - this.Right);
						this.Screenshot.curPt.Y = (float)this.Bottom;
						this.Screenshot.startPt.Y = (float)(this.pageheight - this.Top);
						this.Screenshot.startPt.X = (float)this.Left;
						this.Screenshot.UpdatePriviewBounds(false, true);
					}
				}
			}
		}

		// Token: 0x17000A8B RID: 2699
		// (get) Token: 0x06001E62 RID: 7778 RVA: 0x000862C2 File Offset: 0x000844C2
		// (set) Token: 0x06001E63 RID: 7779 RVA: 0x000862CC File Offset: 0x000844CC
		public double Left
		{
			get
			{
				return this.left;
			}
			set
			{
				if (this.Screenshot != null && this.Screenshot.fixedRate)
				{
					this.fixedCropBoxLRRate(value - this.left);
					return;
				}
				if (base.SetProperty<double>(ref this.left, value, "Left"))
				{
					base.OnPropertyChanged("LeftCm");
					if (this.Screenshot != null)
					{
						this.Screenshot.curPt.X = (float)(this.pagewidth - this.Right);
						this.Screenshot.curPt.Y = (float)this.Bottom;
						this.Screenshot.startPt.Y = (float)(this.pageheight - this.Top);
						this.Screenshot.startPt.X = (float)this.Left;
						this.Screenshot.UpdatePriviewBounds(false, true);
					}
				}
			}
		}

		// Token: 0x17000A8C RID: 2700
		// (get) Token: 0x06001E64 RID: 7780 RVA: 0x0008639E File Offset: 0x0008459E
		// (set) Token: 0x06001E65 RID: 7781 RVA: 0x000863A8 File Offset: 0x000845A8
		public double Right
		{
			get
			{
				return this.right;
			}
			set
			{
				if (this.Screenshot != null && this.Screenshot.fixedRate)
				{
					this.fixedCropBoxLRRate(value - this.right);
					return;
				}
				if (base.SetProperty<double>(ref this.right, value, "Right"))
				{
					base.OnPropertyChanged("RightCm");
					if (this.Screenshot != null)
					{
						this.Screenshot.curPt.X = (float)(this.pagewidth - this.Right);
						this.Screenshot.curPt.Y = (float)this.Bottom;
						this.Screenshot.startPt.Y = (float)(this.pageheight - this.Top);
						this.Screenshot.startPt.X = (float)this.Left;
						this.Screenshot.UpdatePriviewBounds(false, true);
					}
				}
			}
		}

		// Token: 0x17000A8D RID: 2701
		// (get) Token: 0x06001E66 RID: 7782 RVA: 0x0008647A File Offset: 0x0008467A
		// (set) Token: 0x06001E67 RID: 7783 RVA: 0x00086482 File Offset: 0x00084682
		public double PageWidth
		{
			get
			{
				return this.pagewidth;
			}
			set
			{
				base.SetProperty<double>(ref this.pagewidth, value, "PageWidth");
			}
		}

		// Token: 0x17000A8E RID: 2702
		// (get) Token: 0x06001E68 RID: 7784 RVA: 0x00086497 File Offset: 0x00084697
		// (set) Token: 0x06001E69 RID: 7785 RVA: 0x0008649F File Offset: 0x0008469F
		public double PageHeight
		{
			get
			{
				return this.pageheight;
			}
			set
			{
				base.SetProperty<double>(ref this.pageheight, value, "PageHeight");
			}
		}

		// Token: 0x06001E6A RID: 7786 RVA: 0x000864B4 File Offset: 0x000846B4
		private double PdfPointToInch(double number)
		{
			return number / 72.0;
		}

		// Token: 0x06001E6B RID: 7787 RVA: 0x000864C1 File Offset: 0x000846C1
		private double PdfPointToPica(double number)
		{
			return number / 12.0;
		}

		// Token: 0x06001E6C RID: 7788 RVA: 0x000864CE File Offset: 0x000846CE
		private double InchToPdfPoint(double number)
		{
			return number * 72.0;
		}

		// Token: 0x06001E6D RID: 7789 RVA: 0x000864DB File Offset: 0x000846DB
		private double PicaToPdfPoint(double number)
		{
			return number * 12.0;
		}

		// Token: 0x06001E6E RID: 7790 RVA: 0x000864E8 File Offset: 0x000846E8
		private double CropPageRotateTureDirection(MarginModel.OriganlForm OriganlFormName)
		{
			if ((OriganlFormName == MarginModel.OriganlForm.Top && this.Screenshot.cropPageRotate == PageRotate.Normal) || (OriganlFormName == MarginModel.OriganlForm.Left && this.Screenshot.cropPageRotate == PageRotate.Rotate270) || (OriganlFormName == MarginModel.OriganlForm.Right && this.Screenshot.cropPageRotate == PageRotate.Rotate90) || (OriganlFormName == MarginModel.OriganlForm.Bottom && this.Screenshot.cropPageRotate == PageRotate.Rotate180))
			{
				return this.Top;
			}
			if ((OriganlFormName == MarginModel.OriganlForm.Bottom && this.Screenshot.cropPageRotate == PageRotate.Normal) || (OriganlFormName == MarginModel.OriganlForm.Right && this.Screenshot.cropPageRotate == PageRotate.Rotate270) || (OriganlFormName == MarginModel.OriganlForm.Left && this.Screenshot.cropPageRotate == PageRotate.Rotate90) || (OriganlFormName == MarginModel.OriganlForm.Top && this.Screenshot.cropPageRotate == PageRotate.Rotate180))
			{
				return this.Bottom;
			}
			if ((OriganlFormName == MarginModel.OriganlForm.Left && this.Screenshot.cropPageRotate == PageRotate.Normal) || (OriganlFormName == MarginModel.OriganlForm.Bottom && this.Screenshot.cropPageRotate == PageRotate.Rotate270) || (OriganlFormName == MarginModel.OriganlForm.Top && this.Screenshot.cropPageRotate == PageRotate.Rotate90) || (OriganlFormName == MarginModel.OriganlForm.Right && this.Screenshot.cropPageRotate == PageRotate.Rotate180))
			{
				return this.Left;
			}
			if ((OriganlFormName == MarginModel.OriganlForm.Right && this.Screenshot.cropPageRotate == PageRotate.Normal) || (OriganlFormName == MarginModel.OriganlForm.Top && this.Screenshot.cropPageRotate == PageRotate.Rotate270) || (OriganlFormName == MarginModel.OriganlForm.Bottom && this.Screenshot.cropPageRotate == PageRotate.Rotate90) || (OriganlFormName == MarginModel.OriganlForm.Left && this.Screenshot.cropPageRotate == PageRotate.Rotate180))
			{
				return this.Right;
			}
			return -1.0;
		}

		// Token: 0x06001E6F RID: 7791 RVA: 0x00086634 File Offset: 0x00084834
		private void CropPageRotateSetTureValue(double value, MarginModel.OriganlForm OriganlFormName)
		{
			if ((OriganlFormName == MarginModel.OriganlForm.Top && this.Screenshot.cropPageRotate == PageRotate.Normal) || (OriganlFormName == MarginModel.OriganlForm.Left && this.Screenshot.cropPageRotate == PageRotate.Rotate270) || (OriganlFormName == MarginModel.OriganlForm.Right && this.Screenshot.cropPageRotate == PageRotate.Rotate90) || (OriganlFormName == MarginModel.OriganlForm.Bottom && this.Screenshot.cropPageRotate == PageRotate.Rotate180))
			{
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.CM)
				{
					if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageHeight - this.Bottom))
					{
						this.Top = PageHeaderFooterUtils.CmToPdfPoint(value);
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.MM)
				{
					if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageHeight - this.Bottom) * 10.0)
					{
						this.Top = PageHeaderFooterUtils.CmToPdfPoint(value / 10.0);
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Point)
				{
					if (value >= 0.0 && value <= this.PageHeight - this.Bottom)
					{
						this.Top = value;
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Pica)
				{
					if (value >= 0.0 && value <= this.PicaToPdfPoint(this.PageHeight - this.Bottom))
					{
						this.Top = this.PicaToPdfPoint(value);
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Inch && value >= 0.0 && value <= this.InchToPdfPoint(this.PageHeight - this.Bottom))
				{
					this.Top = this.InchToPdfPoint(value);
					return;
				}
			}
			else if ((OriganlFormName == MarginModel.OriganlForm.Bottom && this.Screenshot.cropPageRotate == PageRotate.Normal) || (OriganlFormName == MarginModel.OriganlForm.Right && this.Screenshot.cropPageRotate == PageRotate.Rotate270) || (OriganlFormName == MarginModel.OriganlForm.Left && this.Screenshot.cropPageRotate == PageRotate.Rotate90) || (OriganlFormName == MarginModel.OriganlForm.Top && this.Screenshot.cropPageRotate == PageRotate.Rotate180))
			{
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.CM)
				{
					if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageHeight - this.Bottom))
					{
						this.Bottom = PageHeaderFooterUtils.CmToPdfPoint(value);
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.MM)
				{
					if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageHeight - this.Bottom) * 10.0)
					{
						this.Bottom = PageHeaderFooterUtils.CmToPdfPoint(value / 10.0);
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Point)
				{
					if (value >= 0.0 && value <= this.PageHeight - this.Bottom)
					{
						this.Bottom = value;
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Pica)
				{
					if (value >= 0.0 && value <= this.PicaToPdfPoint(this.PageHeight - this.Bottom))
					{
						this.Bottom = this.PicaToPdfPoint(value);
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Inch && value >= 0.0 && value <= this.InchToPdfPoint(this.PageHeight - this.Bottom))
				{
					this.Bottom = this.InchToPdfPoint(value);
					return;
				}
			}
			else if ((OriganlFormName == MarginModel.OriganlForm.Left && this.Screenshot.cropPageRotate == PageRotate.Normal) || (OriganlFormName == MarginModel.OriganlForm.Bottom && this.Screenshot.cropPageRotate == PageRotate.Rotate270) || (OriganlFormName == MarginModel.OriganlForm.Top && this.Screenshot.cropPageRotate == PageRotate.Rotate90) || (OriganlFormName == MarginModel.OriganlForm.Right && this.Screenshot.cropPageRotate == PageRotate.Rotate180))
			{
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.CM)
				{
					if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageHeight - this.Bottom))
					{
						this.Left = PageHeaderFooterUtils.CmToPdfPoint(value);
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.MM)
				{
					if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageHeight - this.Bottom) * 10.0)
					{
						this.Left = PageHeaderFooterUtils.CmToPdfPoint(value / 10.0);
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Point)
				{
					if (value >= 0.0 && value <= this.PageHeight - this.Bottom)
					{
						this.Left = value;
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Pica)
				{
					if (value >= 0.0 && value <= this.PicaToPdfPoint(this.PageHeight - this.Bottom))
					{
						this.Left = this.PicaToPdfPoint(value);
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Inch && value >= 0.0 && value <= this.InchToPdfPoint(this.PageHeight - this.Bottom))
				{
					this.Left = this.InchToPdfPoint(value);
					return;
				}
			}
			else if ((OriganlFormName == MarginModel.OriganlForm.Right && this.Screenshot.cropPageRotate == PageRotate.Normal) || (OriganlFormName == MarginModel.OriganlForm.Top && this.Screenshot.cropPageRotate == PageRotate.Rotate270) || (OriganlFormName == MarginModel.OriganlForm.Bottom && this.Screenshot.cropPageRotate == PageRotate.Rotate90) || (OriganlFormName == MarginModel.OriganlForm.Left && this.Screenshot.cropPageRotate == PageRotate.Rotate180))
			{
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.CM)
				{
					if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageHeight - this.Bottom))
					{
						this.Right = PageHeaderFooterUtils.CmToPdfPoint(value);
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.MM)
				{
					if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageHeight - this.Bottom) * 10.0)
					{
						this.Right = PageHeaderFooterUtils.CmToPdfPoint(value / 10.0);
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Point)
				{
					if (value >= 0.0 && value <= this.PageHeight - this.Bottom)
					{
						this.Right = value;
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Pica)
				{
					if (value >= 0.0 && value <= this.PicaToPdfPoint(this.PageHeight - this.Bottom))
					{
						this.Right = this.PicaToPdfPoint(value);
						return;
					}
				}
				else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Inch && value >= 0.0 && value <= this.InchToPdfPoint(this.PageHeight - this.Bottom))
				{
					this.Right = this.InchToPdfPoint(value);
				}
			}
		}

		// Token: 0x17000A8F RID: 2703
		// (get) Token: 0x06001E70 RID: 7792 RVA: 0x00086CCC File Offset: 0x00084ECC
		// (set) Token: 0x06001E71 RID: 7793 RVA: 0x00086D74 File Offset: 0x00084F74
		public double TopCm
		{
			get
			{
				double num = this.CropPageRotateTureDirection(MarginModel.OriganlForm.Top);
				if (num == -1.0)
				{
					return PageHeaderFooterUtils.PdfPointToCm(this.Top);
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.CM)
				{
					return PageHeaderFooterUtils.PdfPointToCm(num);
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.MM)
				{
					return PageHeaderFooterUtils.PdfPointToCm(num) * 10.0;
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Point)
				{
					return num;
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Pica)
				{
					return this.PdfPointToPica(num);
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Inch)
				{
					return this.PdfPointToInch(num);
				}
				return PageHeaderFooterUtils.PdfPointToCm(this.Top);
			}
			set
			{
				if (this.Screenshot.cropPageRotate == PageRotate.Normal)
				{
					if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.CM)
					{
						if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageHeight - this.Bottom))
						{
							this.Top = PageHeaderFooterUtils.CmToPdfPoint(value);
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.MM)
					{
						if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageHeight - this.Bottom) * 10.0)
						{
							this.Top = PageHeaderFooterUtils.CmToPdfPoint(value / 10.0);
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Point)
					{
						if (value >= 0.0 && value <= this.PageHeight - this.Bottom)
						{
							this.Top = value;
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Pica)
					{
						if (value >= 0.0 && value <= this.PicaToPdfPoint(this.PageHeight - this.Bottom))
						{
							this.Top = this.PicaToPdfPoint(value);
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Inch && value >= 0.0 && value <= this.InchToPdfPoint(this.PageHeight - this.Bottom))
					{
						this.Top = this.InchToPdfPoint(value);
						return;
					}
				}
				else
				{
					this.CropPageRotateSetTureValue(value, MarginModel.OriganlForm.Top);
				}
			}
		}

		// Token: 0x17000A90 RID: 2704
		// (get) Token: 0x06001E72 RID: 7794 RVA: 0x00086EE4 File Offset: 0x000850E4
		// (set) Token: 0x06001E73 RID: 7795 RVA: 0x00086F8C File Offset: 0x0008518C
		public double BottomCm
		{
			get
			{
				double num = this.CropPageRotateTureDirection(MarginModel.OriganlForm.Bottom);
				if (num == -1.0)
				{
					return PageHeaderFooterUtils.PdfPointToCm(this.Bottom);
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.CM)
				{
					return PageHeaderFooterUtils.PdfPointToCm(num);
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.MM)
				{
					return PageHeaderFooterUtils.PdfPointToCm(num) * 10.0;
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Point)
				{
					return num;
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Pica)
				{
					return this.PdfPointToPica(num);
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Inch)
				{
					return this.PdfPointToInch(num);
				}
				return PageHeaderFooterUtils.PdfPointToCm(this.Bottom);
			}
			set
			{
				if (this.Screenshot.cropPageRotate == PageRotate.Normal)
				{
					if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.CM)
					{
						if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageHeight - this.Top))
						{
							this.Bottom = PageHeaderFooterUtils.CmToPdfPoint(value);
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.MM)
					{
						if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageHeight - this.Top) * 10.0)
						{
							this.Bottom = PageHeaderFooterUtils.CmToPdfPoint(value / 10.0);
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Point)
					{
						if (value >= 0.0 && value <= this.PageHeight - this.Top)
						{
							this.Bottom = value;
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Pica)
					{
						if (value >= 0.0 && value <= this.PicaToPdfPoint(this.PageHeight - this.Top))
						{
							this.Bottom = this.PicaToPdfPoint(value);
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Inch && value >= 0.0 && value <= this.InchToPdfPoint(this.PageHeight - this.Top))
					{
						this.Bottom = this.InchToPdfPoint(value);
						return;
					}
				}
				else
				{
					this.CropPageRotateSetTureValue(value, MarginModel.OriganlForm.Bottom);
				}
			}
		}

		// Token: 0x17000A91 RID: 2705
		// (get) Token: 0x06001E74 RID: 7796 RVA: 0x000870FC File Offset: 0x000852FC
		// (set) Token: 0x06001E75 RID: 7797 RVA: 0x000871A4 File Offset: 0x000853A4
		public double LeftCm
		{
			get
			{
				double num = this.CropPageRotateTureDirection(MarginModel.OriganlForm.Left);
				if (num == -1.0)
				{
					return PageHeaderFooterUtils.PdfPointToCm(this.Left);
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.CM)
				{
					return PageHeaderFooterUtils.PdfPointToCm(num);
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.MM)
				{
					return PageHeaderFooterUtils.PdfPointToCm(num) * 10.0;
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Point)
				{
					return num;
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Pica)
				{
					return this.PdfPointToPica(num);
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Inch)
				{
					return this.PdfPointToInch(num);
				}
				return PageHeaderFooterUtils.PdfPointToCm(this.Left);
			}
			set
			{
				if (this.Screenshot.cropPageRotate == PageRotate.Normal)
				{
					if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.CM)
					{
						if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageWidth - this.Right))
						{
							this.Left = PageHeaderFooterUtils.CmToPdfPoint(value);
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.MM)
					{
						if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageWidth - this.Right) * 10.0)
						{
							this.Left = PageHeaderFooterUtils.CmToPdfPoint(value / 10.0);
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Point)
					{
						if (value >= 0.0 && value <= this.PageWidth - this.Right)
						{
							this.Left = value;
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Pica)
					{
						if (value >= 0.0 && value <= this.PicaToPdfPoint(this.PageWidth - this.Right))
						{
							this.Left = this.PicaToPdfPoint(value);
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Inch && value >= 0.0 && value <= this.InchToPdfPoint(this.PageWidth - this.Right))
					{
						this.Left = this.InchToPdfPoint(value);
						return;
					}
				}
				else
				{
					this.CropPageRotateSetTureValue(value, MarginModel.OriganlForm.Left);
				}
			}
		}

		// Token: 0x17000A92 RID: 2706
		// (get) Token: 0x06001E76 RID: 7798 RVA: 0x00087314 File Offset: 0x00085514
		// (set) Token: 0x06001E77 RID: 7799 RVA: 0x000873BC File Offset: 0x000855BC
		public double RightCm
		{
			get
			{
				double num = this.CropPageRotateTureDirection(MarginModel.OriganlForm.Right);
				if (num == -1.0)
				{
					return PageHeaderFooterUtils.PdfPointToCm(this.Right);
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.CM)
				{
					return PageHeaderFooterUtils.PdfPointToCm(num);
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.MM)
				{
					return PageHeaderFooterUtils.PdfPointToCm(num) * 10.0;
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Point)
				{
					return num;
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Pica)
				{
					return this.PdfPointToPica(num);
				}
				if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Inch)
				{
					return this.PdfPointToInch(num);
				}
				return PageHeaderFooterUtils.PdfPointToCm(this.Right);
			}
			set
			{
				if (this.Screenshot.cropPageRotate == PageRotate.Normal)
				{
					if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.CM)
					{
						if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageWidth - this.Left))
						{
							this.Right = PageHeaderFooterUtils.CmToPdfPoint(value);
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.MM)
					{
						if (value >= 0.0 && value <= PageHeaderFooterUtils.PdfPointToCm(this.PageWidth - this.Left) * 10.0)
						{
							this.Right = PageHeaderFooterUtils.CmToPdfPoint(value / 10.0);
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Point)
					{
						if (value >= 0.0 && value <= this.PageWidth - this.Left)
						{
							this.Right = value;
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Pica)
					{
						if (value >= 0.0 && value <= this.PicaToPdfPoint(this.PageWidth - this.Left))
						{
							this.Right = this.PicaToPdfPoint(value);
							return;
						}
					}
					else if (this.Screenshot.units == ScreenshotDialog.Dimensionunits.Inch && value >= 0.0 && value <= this.InchToPdfPoint(this.PageWidth - this.Left))
					{
						this.Right = this.InchToPdfPoint(value);
						return;
					}
				}
				else
				{
					this.CropPageRotateSetTureValue(value, MarginModel.OriganlForm.Right);
				}
			}
		}

		// Token: 0x04000BAE RID: 2990
		private double top = 36.0;

		// Token: 0x04000BAF RID: 2991
		private double bottom = 36.0;

		// Token: 0x04000BB0 RID: 2992
		private double left = 36.0;

		// Token: 0x04000BB1 RID: 2993
		private double right = 36.0;

		// Token: 0x04000BB2 RID: 2994
		private double pagewidth;

		// Token: 0x04000BB3 RID: 2995
		private double pageheight;

		// Token: 0x0200065E RID: 1630
		private enum OriganlForm
		{
			// Token: 0x04002169 RID: 8553
			Top,
			// Token: 0x0400216A RID: 8554
			Left,
			// Token: 0x0400216B RID: 8555
			Right,
			// Token: 0x0400216C RID: 8556
			Bottom
		}
	}
}
