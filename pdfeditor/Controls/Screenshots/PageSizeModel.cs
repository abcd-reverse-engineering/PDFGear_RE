using System;
using CommunityToolkit.Mvvm.ComponentModel;
using PDFKit.Utils.PageHeaderFooters;

namespace pdfeditor.Controls.Screenshots
{
	// Token: 0x0200021E RID: 542
	public class PageSizeModel : ObservableObject
	{
		// Token: 0x17000A7D RID: 2685
		// (get) Token: 0x06001E43 RID: 7747 RVA: 0x00085B86 File Offset: 0x00083D86
		// (set) Token: 0x06001E44 RID: 7748 RVA: 0x00085B8E File Offset: 0x00083D8E
		public ScreenshotDialog Screenshot { get; set; }

		// Token: 0x17000A7E RID: 2686
		// (get) Token: 0x06001E45 RID: 7749 RVA: 0x00085B97 File Offset: 0x00083D97
		// (set) Token: 0x06001E46 RID: 7750 RVA: 0x00085B9F File Offset: 0x00083D9F
		public float Xoffset
		{
			get
			{
				return this.xoffset;
			}
			set
			{
				if (base.SetProperty<float>(ref this.xoffset, value, "Xoffset"))
				{
					base.OnPropertyChanged("XoffsetCm");
					if (this.Screenshot != null)
					{
						this.Screenshot.ResetPagesize((double)this.Yoffset, (double)value, false, -1);
					}
				}
			}
		}

		// Token: 0x17000A7F RID: 2687
		// (get) Token: 0x06001E47 RID: 7751 RVA: 0x00085BDE File Offset: 0x00083DDE
		// (set) Token: 0x06001E48 RID: 7752 RVA: 0x00085BE6 File Offset: 0x00083DE6
		public float Yoffset
		{
			get
			{
				return this.yoffset;
			}
			set
			{
				if (base.SetProperty<float>(ref this.yoffset, value, "Yoffset"))
				{
					base.OnPropertyChanged("YoffsetCm");
					if (this.Screenshot != null)
					{
						this.Screenshot.ResetPagesize((double)value, (double)this.Xoffset, false, -1);
					}
				}
			}
		}

		// Token: 0x17000A80 RID: 2688
		// (get) Token: 0x06001E49 RID: 7753 RVA: 0x00085C25 File Offset: 0x00083E25
		// (set) Token: 0x06001E4A RID: 7754 RVA: 0x00085C2D File Offset: 0x00083E2D
		public float Width
		{
			get
			{
				return this.width;
			}
			set
			{
				if (base.SetProperty<float>(ref this.width, value, "Width"))
				{
					base.OnPropertyChanged("WidthCm");
					if (this.Screenshot != null)
					{
						this.Screenshot.ResetPagesize((double)this.height, (double)value, true, -1);
					}
				}
			}
		}

		// Token: 0x17000A81 RID: 2689
		// (get) Token: 0x06001E4B RID: 7755 RVA: 0x00085C6C File Offset: 0x00083E6C
		// (set) Token: 0x06001E4C RID: 7756 RVA: 0x00085C74 File Offset: 0x00083E74
		public float Height
		{
			get
			{
				return this.height;
			}
			set
			{
				if (base.SetProperty<float>(ref this.height, value, "Height"))
				{
					base.OnPropertyChanged("HeightCm");
					if (this.Screenshot != null)
					{
						this.Screenshot.ResetPagesize((double)value, (double)this.width, true, -1);
					}
				}
			}
		}

		// Token: 0x17000A82 RID: 2690
		// (get) Token: 0x06001E4D RID: 7757 RVA: 0x00085CB3 File Offset: 0x00083EB3
		// (set) Token: 0x06001E4E RID: 7758 RVA: 0x00085CBB File Offset: 0x00083EBB
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

		// Token: 0x17000A83 RID: 2691
		// (get) Token: 0x06001E4F RID: 7759 RVA: 0x00085CD0 File Offset: 0x00083ED0
		// (set) Token: 0x06001E50 RID: 7760 RVA: 0x00085CD8 File Offset: 0x00083ED8
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

		// Token: 0x17000A84 RID: 2692
		// (get) Token: 0x06001E51 RID: 7761 RVA: 0x00085CED File Offset: 0x00083EED
		// (set) Token: 0x06001E52 RID: 7762 RVA: 0x00085CFB File Offset: 0x00083EFB
		public double WidthCm
		{
			get
			{
				return PageHeaderFooterUtils.PdfPointToCm((double)this.Width);
			}
			set
			{
				if (value >= PageHeaderFooterUtils.PdfPointToCm(this.PageWidth))
				{
					this.Width = (float)PageHeaderFooterUtils.CmToPdfPoint(value);
				}
			}
		}

		// Token: 0x17000A85 RID: 2693
		// (get) Token: 0x06001E53 RID: 7763 RVA: 0x00085D18 File Offset: 0x00083F18
		// (set) Token: 0x06001E54 RID: 7764 RVA: 0x00085D26 File Offset: 0x00083F26
		public double HeightCm
		{
			get
			{
				return PageHeaderFooterUtils.PdfPointToCm((double)this.Height);
			}
			set
			{
				if (value >= PageHeaderFooterUtils.PdfPointToCm(this.PageHeight))
				{
					this.Height = (float)PageHeaderFooterUtils.CmToPdfPoint(value);
				}
			}
		}

		// Token: 0x17000A86 RID: 2694
		// (get) Token: 0x06001E55 RID: 7765 RVA: 0x00085D43 File Offset: 0x00083F43
		// (set) Token: 0x06001E56 RID: 7766 RVA: 0x00085D51 File Offset: 0x00083F51
		public double XoffsetCm
		{
			get
			{
				return PageHeaderFooterUtils.PdfPointToCm((double)this.Xoffset);
			}
			set
			{
				if (value > 0.0)
				{
					this.Xoffset = (float)PageHeaderFooterUtils.CmToPdfPoint(value);
				}
			}
		}

		// Token: 0x17000A87 RID: 2695
		// (get) Token: 0x06001E57 RID: 7767 RVA: 0x00085D6C File Offset: 0x00083F6C
		// (set) Token: 0x06001E58 RID: 7768 RVA: 0x00085D7A File Offset: 0x00083F7A
		public double YoffsetCm
		{
			get
			{
				return PageHeaderFooterUtils.PdfPointToCm((double)this.Yoffset);
			}
			set
			{
				if (value > 0.0)
				{
					this.Yoffset = (float)PageHeaderFooterUtils.CmToPdfPoint(value);
				}
			}
		}

		// Token: 0x04000BA7 RID: 2983
		private float width = 36f;

		// Token: 0x04000BA8 RID: 2984
		private float height = 36f;

		// Token: 0x04000BA9 RID: 2985
		public float xoffset;

		// Token: 0x04000BAA RID: 2986
		public float yoffset;

		// Token: 0x04000BAB RID: 2987
		private double pagewidth;

		// Token: 0x04000BAC RID: 2988
		private double pageheight;
	}
}
