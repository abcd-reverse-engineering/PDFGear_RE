using System;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Patagames.Pdf.Net;
using PDFKit.Utils.PageHeaderFooters;
using PDFKit.Utils.XObjects;

namespace pdfeditor.Controls.PageHeaderFooters
{
	// Token: 0x02000243 RID: 579
	public class ResultModel : ObservableObject
	{
		// Token: 0x0600210D RID: 8461 RVA: 0x0009808C File Offset: 0x0009628C
		public ResultModel()
		{
			this.Text = new ResultModel.TextModel();
			this.FontSize = 10f;
			this.Color = Colors.Black;
			this.pageRange = ResultModel.PageRangeEnum.None;
			this.Margin = new ResultModel.MarginModel();
		}

		// Token: 0x17000AF2 RID: 2802
		// (get) Token: 0x0600210E RID: 8462 RVA: 0x000980C7 File Offset: 0x000962C7
		public ResultModel.TextModel Text { get; }

		// Token: 0x17000AF3 RID: 2803
		// (get) Token: 0x0600210F RID: 8463 RVA: 0x000980CF File Offset: 0x000962CF
		// (set) Token: 0x06002110 RID: 8464 RVA: 0x000980D7 File Offset: 0x000962D7
		public float FontSize
		{
			get
			{
				return this.fontSize;
			}
			set
			{
				base.SetProperty<float>(ref this.fontSize, value, "FontSize");
			}
		}

		// Token: 0x17000AF4 RID: 2804
		// (get) Token: 0x06002111 RID: 8465 RVA: 0x000980EC File Offset: 0x000962EC
		// (set) Token: 0x06002112 RID: 8466 RVA: 0x000980F4 File Offset: 0x000962F4
		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				base.SetProperty<Color>(ref this.color, value, "Color");
			}
		}

		// Token: 0x17000AF5 RID: 2805
		// (get) Token: 0x06002113 RID: 8467 RVA: 0x00098109 File Offset: 0x00096309
		// (set) Token: 0x06002114 RID: 8468 RVA: 0x00098111 File Offset: 0x00096311
		public ResultModel.PageRangeEnum PageRange
		{
			get
			{
				return this.pageRange;
			}
			set
			{
				base.SetProperty<ResultModel.PageRangeEnum>(ref this.pageRange, value, "PageRange");
			}
		}

		// Token: 0x17000AF6 RID: 2806
		// (get) Token: 0x06002115 RID: 8469 RVA: 0x00098126 File Offset: 0x00096326
		// (set) Token: 0x06002116 RID: 8470 RVA: 0x0009812E File Offset: 0x0009632E
		public int SelectedPagesStart
		{
			get
			{
				return this.selectedPagesStart;
			}
			set
			{
				base.SetProperty<int>(ref this.selectedPagesStart, value, "SelectedPagesStart");
			}
		}

		// Token: 0x17000AF7 RID: 2807
		// (get) Token: 0x06002117 RID: 8471 RVA: 0x00098143 File Offset: 0x00096343
		// (set) Token: 0x06002118 RID: 8472 RVA: 0x0009814B File Offset: 0x0009634B
		public int SelectedPagesEnd
		{
			get
			{
				return this.selectedPagesEnd;
			}
			set
			{
				base.SetProperty<int>(ref this.selectedPagesEnd, value, "SelectedPagesEnd");
			}
		}

		// Token: 0x17000AF8 RID: 2808
		// (get) Token: 0x06002119 RID: 8473 RVA: 0x00098160 File Offset: 0x00096360
		// (set) Token: 0x0600211A RID: 8474 RVA: 0x00098168 File Offset: 0x00096368
		public ResultModel.SubsetEnum Subset
		{
			get
			{
				return this.subset;
			}
			set
			{
				base.SetProperty<ResultModel.SubsetEnum>(ref this.subset, value, "Subset");
			}
		}

		// Token: 0x17000AF9 RID: 2809
		// (get) Token: 0x0600211B RID: 8475 RVA: 0x0009817D File Offset: 0x0009637D
		// (set) Token: 0x0600211C RID: 8476 RVA: 0x00098185 File Offset: 0x00096385
		public string PageNumberFormat
		{
			get
			{
				return this.pageNumberFormat;
			}
			set
			{
				base.SetProperty<string>(ref this.pageNumberFormat, value, "PageNumberFormat");
			}
		}

		// Token: 0x17000AFA RID: 2810
		// (get) Token: 0x0600211D RID: 8477 RVA: 0x0009819A File Offset: 0x0009639A
		// (set) Token: 0x0600211E RID: 8478 RVA: 0x000981A2 File Offset: 0x000963A2
		public int PageNumberOffset
		{
			get
			{
				return this.pageNumberOffset;
			}
			set
			{
				base.SetProperty<int>(ref this.pageNumberOffset, value, "PageNumberOffset");
			}
		}

		// Token: 0x17000AFB RID: 2811
		// (get) Token: 0x0600211F RID: 8479 RVA: 0x000981B7 File Offset: 0x000963B7
		// (set) Token: 0x06002120 RID: 8480 RVA: 0x000981BF File Offset: 0x000963BF
		public string DateFormat
		{
			get
			{
				return this.dateFormat;
			}
			set
			{
				base.SetProperty<string>(ref this.dateFormat, value, "DateFormat");
			}
		}

		// Token: 0x17000AFC RID: 2812
		// (get) Token: 0x06002121 RID: 8481 RVA: 0x000981D4 File Offset: 0x000963D4
		public ResultModel.MarginModel Margin { get; }

		// Token: 0x06002122 RID: 8482 RVA: 0x000981DC File Offset: 0x000963DC
		public HeaderFooterSettings ToSettings(PdfDocument doc, int currentPageIndex = -1)
		{
			return ResultModel.ResultModelHelper.ToHeaderFooterSettings(doc, this, currentPageIndex);
		}

		// Token: 0x06002123 RID: 8483 RVA: 0x000981E6 File Offset: 0x000963E6
		public static ResultModel FromSettings(PdfDocument doc, HeaderFooterSettings settings)
		{
			return ResultModel.ResultModelHelper.FromHeaderFooterSettings(doc, settings);
		}

		// Token: 0x04000D74 RID: 3444
		private float fontSize;

		// Token: 0x04000D75 RID: 3445
		private Color color;

		// Token: 0x04000D76 RID: 3446
		private ResultModel.PageRangeEnum pageRange;

		// Token: 0x04000D77 RID: 3447
		private int selectedPagesStart;

		// Token: 0x04000D78 RID: 3448
		private int selectedPagesEnd;

		// Token: 0x04000D79 RID: 3449
		private ResultModel.SubsetEnum subset;

		// Token: 0x04000D7A RID: 3450
		private string pageNumberFormat;

		// Token: 0x04000D7B RID: 3451
		private int pageNumberOffset;

		// Token: 0x04000D7C RID: 3452
		private string dateFormat;

		// Token: 0x020006F9 RID: 1785
		public class TextModel : ObservableObject
		{
			// Token: 0x17000D46 RID: 3398
			// (get) Token: 0x06003555 RID: 13653 RVA: 0x0010C247 File Offset: 0x0010A447
			// (set) Token: 0x06003556 RID: 13654 RVA: 0x0010C24F File Offset: 0x0010A44F
			public string LeftHeaderText
			{
				get
				{
					return this.leftHeaderText;
				}
				set
				{
					base.SetProperty<string>(ref this.leftHeaderText, value.TrimEnd(new char[] { '\r', '\n' }), "LeftHeaderText");
				}
			}

			// Token: 0x17000D47 RID: 3399
			// (get) Token: 0x06003557 RID: 13655 RVA: 0x0010C279 File Offset: 0x0010A479
			// (set) Token: 0x06003558 RID: 13656 RVA: 0x0010C281 File Offset: 0x0010A481
			public string CenterHeaderText
			{
				get
				{
					return this.centerHeaderText;
				}
				set
				{
					base.SetProperty<string>(ref this.centerHeaderText, value.TrimEnd(new char[] { '\r', '\n' }), "CenterHeaderText");
				}
			}

			// Token: 0x17000D48 RID: 3400
			// (get) Token: 0x06003559 RID: 13657 RVA: 0x0010C2AB File Offset: 0x0010A4AB
			// (set) Token: 0x0600355A RID: 13658 RVA: 0x0010C2B3 File Offset: 0x0010A4B3
			public string RightHeaderText
			{
				get
				{
					return this.rightHeaderText;
				}
				set
				{
					base.SetProperty<string>(ref this.rightHeaderText, value.TrimEnd(new char[] { '\r', '\n' }), "RightHeaderText");
				}
			}

			// Token: 0x17000D49 RID: 3401
			// (get) Token: 0x0600355B RID: 13659 RVA: 0x0010C2DD File Offset: 0x0010A4DD
			// (set) Token: 0x0600355C RID: 13660 RVA: 0x0010C2E5 File Offset: 0x0010A4E5
			public string LeftFooterText
			{
				get
				{
					return this.leftFooterText;
				}
				set
				{
					base.SetProperty<string>(ref this.leftFooterText, value.TrimEnd(new char[] { '\r', '\n' }), "LeftFooterText");
				}
			}

			// Token: 0x17000D4A RID: 3402
			// (get) Token: 0x0600355D RID: 13661 RVA: 0x0010C30F File Offset: 0x0010A50F
			// (set) Token: 0x0600355E RID: 13662 RVA: 0x0010C317 File Offset: 0x0010A517
			public string CenterFooterText
			{
				get
				{
					return this.centerFooterText;
				}
				set
				{
					base.SetProperty<string>(ref this.centerFooterText, value.TrimEnd(new char[] { '\r', '\n' }), "CenterFooterText");
				}
			}

			// Token: 0x17000D4B RID: 3403
			// (get) Token: 0x0600355F RID: 13663 RVA: 0x0010C341 File Offset: 0x0010A541
			// (set) Token: 0x06003560 RID: 13664 RVA: 0x0010C349 File Offset: 0x0010A549
			public string RightFooterText
			{
				get
				{
					return this.rightFooterText;
				}
				set
				{
					base.SetProperty<string>(ref this.rightFooterText, value.TrimEnd(new char[] { '\r', '\n' }), "RightFooterText");
				}
			}

			// Token: 0x040023BA RID: 9146
			private string leftHeaderText = string.Empty;

			// Token: 0x040023BB RID: 9147
			private string centerHeaderText = string.Empty;

			// Token: 0x040023BC RID: 9148
			private string rightHeaderText = string.Empty;

			// Token: 0x040023BD RID: 9149
			private string leftFooterText = string.Empty;

			// Token: 0x040023BE RID: 9150
			private string centerFooterText = string.Empty;

			// Token: 0x040023BF RID: 9151
			private string rightFooterText = string.Empty;
		}

		// Token: 0x020006FA RID: 1786
		public class MarginModel : ObservableObject
		{
			// Token: 0x17000D4C RID: 3404
			// (get) Token: 0x06003562 RID: 13666 RVA: 0x0010C3C9 File Offset: 0x0010A5C9
			// (set) Token: 0x06003563 RID: 13667 RVA: 0x0010C3D1 File Offset: 0x0010A5D1
			public double Top
			{
				get
				{
					return this.top;
				}
				set
				{
					if (base.SetProperty<double>(ref this.top, value, "Top"))
					{
						base.OnPropertyChanged("TopCm");
					}
				}
			}

			// Token: 0x17000D4D RID: 3405
			// (get) Token: 0x06003564 RID: 13668 RVA: 0x0010C3F2 File Offset: 0x0010A5F2
			// (set) Token: 0x06003565 RID: 13669 RVA: 0x0010C3FA File Offset: 0x0010A5FA
			public double Bottom
			{
				get
				{
					return this.bottom;
				}
				set
				{
					if (base.SetProperty<double>(ref this.bottom, value, "Bottom"))
					{
						base.OnPropertyChanged("BottomCm");
					}
				}
			}

			// Token: 0x17000D4E RID: 3406
			// (get) Token: 0x06003566 RID: 13670 RVA: 0x0010C41B File Offset: 0x0010A61B
			// (set) Token: 0x06003567 RID: 13671 RVA: 0x0010C423 File Offset: 0x0010A623
			public double Left
			{
				get
				{
					return this.left;
				}
				set
				{
					if (base.SetProperty<double>(ref this.left, value, "Left"))
					{
						base.OnPropertyChanged("LeftCm");
					}
				}
			}

			// Token: 0x17000D4F RID: 3407
			// (get) Token: 0x06003568 RID: 13672 RVA: 0x0010C444 File Offset: 0x0010A644
			// (set) Token: 0x06003569 RID: 13673 RVA: 0x0010C44C File Offset: 0x0010A64C
			public double Right
			{
				get
				{
					return this.right;
				}
				set
				{
					if (base.SetProperty<double>(ref this.right, value, "Right"))
					{
						base.OnPropertyChanged("RightCm");
					}
				}
			}

			// Token: 0x17000D50 RID: 3408
			// (get) Token: 0x0600356A RID: 13674 RVA: 0x0010C46D File Offset: 0x0010A66D
			// (set) Token: 0x0600356B RID: 13675 RVA: 0x0010C47A File Offset: 0x0010A67A
			public double TopCm
			{
				get
				{
					return PageHeaderFooterUtils.PdfPointToCm(this.Top);
				}
				set
				{
					this.Top = PageHeaderFooterUtils.CmToPdfPoint(value);
				}
			}

			// Token: 0x17000D51 RID: 3409
			// (get) Token: 0x0600356C RID: 13676 RVA: 0x0010C488 File Offset: 0x0010A688
			// (set) Token: 0x0600356D RID: 13677 RVA: 0x0010C495 File Offset: 0x0010A695
			public double BottomCm
			{
				get
				{
					return PageHeaderFooterUtils.PdfPointToCm(this.Bottom);
				}
				set
				{
					this.Bottom = PageHeaderFooterUtils.CmToPdfPoint(value);
				}
			}

			// Token: 0x17000D52 RID: 3410
			// (get) Token: 0x0600356E RID: 13678 RVA: 0x0010C4A3 File Offset: 0x0010A6A3
			// (set) Token: 0x0600356F RID: 13679 RVA: 0x0010C4B0 File Offset: 0x0010A6B0
			public double LeftCm
			{
				get
				{
					return PageHeaderFooterUtils.PdfPointToCm(this.Left);
				}
				set
				{
					this.Left = PageHeaderFooterUtils.CmToPdfPoint(value);
				}
			}

			// Token: 0x17000D53 RID: 3411
			// (get) Token: 0x06003570 RID: 13680 RVA: 0x0010C4BE File Offset: 0x0010A6BE
			// (set) Token: 0x06003571 RID: 13681 RVA: 0x0010C4CB File Offset: 0x0010A6CB
			public double RightCm
			{
				get
				{
					return PageHeaderFooterUtils.PdfPointToCm(this.Right);
				}
				set
				{
					this.Right = PageHeaderFooterUtils.CmToPdfPoint(value);
				}
			}

			// Token: 0x040023C0 RID: 9152
			private double top = 36.0;

			// Token: 0x040023C1 RID: 9153
			private double bottom = 36.0;

			// Token: 0x040023C2 RID: 9154
			private double left = 36.0;

			// Token: 0x040023C3 RID: 9155
			private double right = 36.0;
		}

		// Token: 0x020006FB RID: 1787
		public enum PageRangeEnum
		{
			// Token: 0x040023C5 RID: 9157
			None,
			// Token: 0x040023C6 RID: 9158
			AllPages,
			// Token: 0x040023C7 RID: 9159
			CurrentPage,
			// Token: 0x040023C8 RID: 9160
			SelectedPages
		}

		// Token: 0x020006FC RID: 1788
		public enum SubsetEnum
		{
			// Token: 0x040023CA RID: 9162
			AllPages,
			// Token: 0x040023CB RID: 9163
			Odd,
			// Token: 0x040023CC RID: 9164
			Even
		}

		// Token: 0x020006FD RID: 1789
		private static class ResultModelHelper
		{
			// Token: 0x06003573 RID: 13683 RVA: 0x0010C52C File Offset: 0x0010A72C
			public static ResultModel FromHeaderFooterSettings(PdfDocument doc, HeaderFooterSettings hfSettings)
			{
				if (hfSettings == null || doc == null)
				{
					return null;
				}
				ResultModel resultModel = new ResultModel();
				resultModel.Margin.Left = hfSettings.Margin.Left;
				resultModel.Margin.Top = hfSettings.Margin.Top;
				resultModel.Margin.Right = hfSettings.Margin.Right;
				resultModel.Margin.Bottom = hfSettings.Margin.Bottom;
				if (hfSettings.PageRange.Start == -1 && hfSettings.PageRange.End == -1)
				{
					resultModel.PageRange = ResultModel.PageRangeEnum.AllPages;
				}
				else
				{
					int num = ((hfSettings.PageRange.Start == -1) ? 0 : hfSettings.PageRange.Start);
					int num2 = ((hfSettings.PageRange.End == -1) ? doc.Pages.Count : hfSettings.PageRange.End);
					resultModel.PageRange = ResultModel.PageRangeEnum.SelectedPages;
					resultModel.SelectedPagesStart = num;
					resultModel.SelectedPagesEnd = num2;
				}
				if (hfSettings.PageRange.Even && hfSettings.PageRange.Odd)
				{
					resultModel.Subset = ResultModel.SubsetEnum.AllPages;
				}
				else if (hfSettings.PageRange.Even)
				{
					resultModel.Subset = ResultModel.SubsetEnum.Even;
				}
				else
				{
					resultModel.Subset = ResultModel.SubsetEnum.Odd;
				}
				resultModel.FontSize = (float)hfSettings.Font.Size;
				resultModel.Color = Color.FromArgb(byte.MaxValue, (byte)(hfSettings.Color.R * 255.0), (byte)(hfSettings.Color.G * 255.0), (byte)(hfSettings.Color.B * 255.0));
				if (hfSettings.Page != null)
				{
					resultModel.PageNumberFormat = PagePlaceholderFormatter.PageModelToString(hfSettings.Page);
					if (resultModel.PageNumberFormat.StartsWith("<<"))
					{
						resultModel.PageNumberFormat = resultModel.PageNumberFormat.Substring(2, resultModel.PageNumberFormat.Length - 4).Trim();
					}
					resultModel.PageNumberOffset = hfSettings.Page.Offset + 1;
				}
				if (hfSettings.Date != null)
				{
					resultModel.DateFormat = PagePlaceholderFormatter.DateModelToString(hfSettings.Date);
					if (resultModel.DateFormat.StartsWith("<<"))
					{
						resultModel.DateFormat = resultModel.DateFormat.Substring(2, resultModel.DateFormat.Length - 4).Trim();
					}
				}
				resultModel.Text.LeftHeaderText = PagePlaceholderFormatter.LocationToString(hfSettings.Header.Left);
				resultModel.Text.CenterHeaderText = PagePlaceholderFormatter.LocationToString(hfSettings.Header.Center);
				resultModel.Text.RightHeaderText = PagePlaceholderFormatter.LocationToString(hfSettings.Header.Right);
				resultModel.Text.LeftFooterText = PagePlaceholderFormatter.LocationToString(hfSettings.Footer.Left);
				resultModel.Text.CenterFooterText = PagePlaceholderFormatter.LocationToString(hfSettings.Footer.Center);
				resultModel.Text.RightFooterText = PagePlaceholderFormatter.LocationToString(hfSettings.Footer.Right);
				return resultModel;
			}

			// Token: 0x06003574 RID: 13684 RVA: 0x0010C818 File Offset: 0x0010AA18
			public static HeaderFooterSettings ToHeaderFooterSettings(PdfDocument doc, ResultModel model, int currentPageIndex = -1)
			{
				if (model == null || doc == null)
				{
					return null;
				}
				HeaderFooterSettings headerFooterSettings = new HeaderFooterSettings();
				headerFooterSettings.Margin.Left = model.Margin.Left;
				headerFooterSettings.Margin.Top = model.Margin.Top;
				headerFooterSettings.Margin.Right = model.Margin.Right;
				headerFooterSettings.Margin.Bottom = model.Margin.Bottom;
				if (model.PageRange == ResultModel.PageRangeEnum.AllPages)
				{
					headerFooterSettings.PageRange.Start = -1;
					headerFooterSettings.PageRange.End = -1;
				}
				else if (model.PageRange == ResultModel.PageRangeEnum.SelectedPages)
				{
					headerFooterSettings.PageRange.Start = model.SelectedPagesStart;
					headerFooterSettings.PageRange.End = model.SelectedPagesEnd;
				}
				else if (currentPageIndex != -1)
				{
					headerFooterSettings.PageRange.Start = currentPageIndex;
					headerFooterSettings.PageRange.End = currentPageIndex;
				}
				else
				{
					headerFooterSettings.PageRange.Start = -1;
					headerFooterSettings.PageRange.End = -1;
				}
				if (model.Subset == ResultModel.SubsetEnum.AllPages)
				{
					headerFooterSettings.PageRange.Even = true;
					headerFooterSettings.PageRange.Odd = true;
				}
				else if (model.Subset == ResultModel.SubsetEnum.Even)
				{
					headerFooterSettings.PageRange.Even = true;
					headerFooterSettings.PageRange.Odd = false;
				}
				else if (model.Subset == ResultModel.SubsetEnum.Odd)
				{
					headerFooterSettings.PageRange.Even = false;
					headerFooterSettings.PageRange.Odd = true;
				}
				headerFooterSettings.Font.Size = (double)model.fontSize;
				headerFooterSettings.Color.R = (double)model.Color.R / 255.0;
				headerFooterSettings.Color.G = (double)model.Color.G / 255.0;
				headerFooterSettings.Color.B = (double)model.Color.B / 255.0;
				if (!string.IsNullOrEmpty(model.PageNumberFormat))
				{
					HeaderFooterSettings.PageModel pageModel = PagePlaceholderFormatter.StringToPageModel("<<" + model.PageNumberFormat + ">>", model.PageNumberOffset);
					if (pageModel != null)
					{
						int num = pageModel.Offset;
						if (num < 0)
						{
							num = 0;
						}
						headerFooterSettings.Page.Offset = num;
						headerFooterSettings.Page.Clear();
						foreach (object obj in pageModel)
						{
							headerFooterSettings.Page.Add(obj);
						}
					}
				}
				if (!string.IsNullOrEmpty(model.DateFormat))
				{
					HeaderFooterSettings.DateModel dateModel = PagePlaceholderFormatter.StringToDateModel("<<" + model.DateFormat + ">>");
					if (dateModel != null)
					{
						headerFooterSettings.Date.Clear();
						foreach (object obj2 in dateModel)
						{
							headerFooterSettings.Date.Add(obj2);
						}
					}
				}
				PagePlaceholderFormatter.StringToLocation(headerFooterSettings.Header.Left, model.Text.LeftHeaderText, model.PageNumberOffset);
				PagePlaceholderFormatter.StringToLocation(headerFooterSettings.Header.Center, model.Text.CenterHeaderText, model.PageNumberOffset);
				PagePlaceholderFormatter.StringToLocation(headerFooterSettings.Header.Right, model.Text.RightHeaderText, model.PageNumberOffset);
				PagePlaceholderFormatter.StringToLocation(headerFooterSettings.Footer.Left, model.Text.LeftFooterText, model.PageNumberOffset);
				PagePlaceholderFormatter.StringToLocation(headerFooterSettings.Footer.Center, model.Text.CenterFooterText, model.PageNumberOffset);
				PagePlaceholderFormatter.StringToLocation(headerFooterSettings.Footer.Right, model.Text.RightFooterText, model.PageNumberOffset);
				return headerFooterSettings;
			}
		}
	}
}
