using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using CommonLib.Common;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;

namespace pdfeditor.Utils.Print
{
	// Token: 0x020000C0 RID: 192
	public class PdfPrintDocument : PrintDocument
	{
		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06000B55 RID: 2901 RVA: 0x0003A358 File Offset: 0x00038558
		private int _pageForDoc
		{
			get
			{
				return this.GetDocPageIndex(this._pageForPrint, 0);
			}
		}

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000B56 RID: 2902 RVA: 0x0003A368 File Offset: 0x00038568
		// (remove) Token: 0x06000B57 RID: 2903 RVA: 0x0003A3A0 File Offset: 0x000385A0
		public event EventHandler<BeforeRenderPageEventArgs> BeforeRenderPage;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06000B58 RID: 2904 RVA: 0x0003A3D8 File Offset: 0x000385D8
		// (remove) Token: 0x06000B59 RID: 2905 RVA: 0x0003A410 File Offset: 0x00038610
		public event EventHandler<BeforeRenderPageEventArgs> AfterRenderPage;

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06000B5A RID: 2906 RVA: 0x0003A445 File Offset: 0x00038645
		// (set) Token: 0x06000B5B RID: 2907 RVA: 0x0003A44D File Offset: 0x0003864D
		public bool AutoRotate { get; set; }

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000B5C RID: 2908 RVA: 0x0003A456 File Offset: 0x00038656
		// (set) Token: 0x06000B5D RID: 2909 RVA: 0x0003A45E File Offset: 0x0003865E
		public bool AutoCenter { get; set; }

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000B5E RID: 2910 RVA: 0x0003A467 File Offset: 0x00038667
		// (set) Token: 0x06000B5F RID: 2911 RVA: 0x0003A46F File Offset: 0x0003866F
		public PrintTypeSettingModel PrintTypeSettingModel { get; set; }

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06000B60 RID: 2912 RVA: 0x0003A478 File Offset: 0x00038678
		// (set) Token: 0x06000B61 RID: 2913 RVA: 0x0003A480 File Offset: 0x00038680
		public bool isPreviewControl { get; set; }

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x06000B62 RID: 2914 RVA: 0x0003A489 File Offset: 0x00038689
		// (set) Token: 0x06000B63 RID: 2915 RVA: 0x0003A491 File Offset: 0x00038691
		public int PapersPerSheet { get; set; }

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x06000B64 RID: 2916 RVA: 0x0003A49A File Offset: 0x0003869A
		// (set) Token: 0x06000B65 RID: 2917 RVA: 0x0003A4A2 File Offset: 0x000386A2
		public int PaperRowNum { get; set; }

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x06000B66 RID: 2918 RVA: 0x0003A4AB File Offset: 0x000386AB
		// (set) Token: 0x06000B67 RID: 2919 RVA: 0x0003A4B3 File Offset: 0x000386B3
		public int PaperColumnNum { get; set; }

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x06000B68 RID: 2920 RVA: 0x0003A4BC File Offset: 0x000386BC
		// (set) Token: 0x06000B69 RID: 2921 RVA: 0x0003A4C4 File Offset: 0x000386C4
		public PageOrder PageOrder { get; set; }

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x06000B6A RID: 2922 RVA: 0x0003A4CD File Offset: 0x000386CD
		// (set) Token: 0x06000B6B RID: 2923 RVA: 0x0003A4D5 File Offset: 0x000386D5
		public List<int> BookletPageOrder { get; set; }

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x06000B6C RID: 2924 RVA: 0x0003A4DE File Offset: 0x000386DE
		// (set) Token: 0x06000B6D RID: 2925 RVA: 0x0003A4E6 File Offset: 0x000386E6
		public BookletBindingDirection BookletBindingDirection { get; set; }

		// Token: 0x17000284 RID: 644
		// (get) Token: 0x06000B6E RID: 2926 RVA: 0x0003A4EF File Offset: 0x000386EF
		// (set) Token: 0x06000B6F RID: 2927 RVA: 0x0003A4F7 File Offset: 0x000386F7
		public BookletSubset BookletSubset { get; set; }

		// Token: 0x17000285 RID: 645
		// (get) Token: 0x06000B70 RID: 2928 RVA: 0x0003A500 File Offset: 0x00038700
		// (set) Token: 0x06000B71 RID: 2929 RVA: 0x0003A508 File Offset: 0x00038708
		public double TilePageZoom { get; set; }

		// Token: 0x17000286 RID: 646
		// (get) Token: 0x06000B72 RID: 2930 RVA: 0x0003A511 File Offset: 0x00038711
		// (set) Token: 0x06000B73 RID: 2931 RVA: 0x0003A519 File Offset: 0x00038719
		public double TileOverlap { get; set; }

		// Token: 0x17000287 RID: 647
		// (get) Token: 0x06000B74 RID: 2932 RVA: 0x0003A522 File Offset: 0x00038722
		// (set) Token: 0x06000B75 RID: 2933 RVA: 0x0003A52A File Offset: 0x0003872A
		public bool TileCutMasks { get; set; }

		// Token: 0x17000288 RID: 648
		// (get) Token: 0x06000B76 RID: 2934 RVA: 0x0003A533 File Offset: 0x00038733
		// (set) Token: 0x06000B77 RID: 2935 RVA: 0x0003A53B File Offset: 0x0003873B
		public bool TileLabels { get; set; }

		// Token: 0x17000289 RID: 649
		// (get) Token: 0x06000B78 RID: 2936 RVA: 0x0003A544 File Offset: 0x00038744
		// (set) Token: 0x06000B79 RID: 2937 RVA: 0x0003A54C File Offset: 0x0003874C
		public string TileFilePath { get; set; }

		// Token: 0x1700028A RID: 650
		// (get) Token: 0x06000B7A RID: 2938 RVA: 0x0003A555 File Offset: 0x00038755
		// (set) Token: 0x06000B7B RID: 2939 RVA: 0x0003A55D File Offset: 0x0003875D
		public bool PrintBorder { get; set; }

		// Token: 0x1700028B RID: 651
		// (get) Token: 0x06000B7C RID: 2940 RVA: 0x0003A566 File Offset: 0x00038766
		// (set) Token: 0x06000B7D RID: 2941 RVA: 0x0003A56E File Offset: 0x0003876E
		public double PageMargins { get; set; }

		// Token: 0x1700028C RID: 652
		// (get) Token: 0x06000B7E RID: 2942 RVA: 0x0003A577 File Offset: 0x00038777
		// (set) Token: 0x06000B7F RID: 2943 RVA: 0x0003A57F File Offset: 0x0003877F
		public bool PrintReverse { get; set; }

		// Token: 0x1700028D RID: 653
		// (get) Token: 0x06000B80 RID: 2944 RVA: 0x0003A588 File Offset: 0x00038788
		public PdfDocument Document
		{
			get
			{
				return this._pdfDoc;
			}
		}

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06000B81 RID: 2945 RVA: 0x0003A590 File Offset: 0x00038790
		// (set) Token: 0x06000B82 RID: 2946 RVA: 0x0003A598 File Offset: 0x00038798
		public PrintSizeMode PrintSizeMode { get; set; }

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06000B83 RID: 2947 RVA: 0x0003A5A1 File Offset: 0x000387A1
		// (set) Token: 0x06000B84 RID: 2948 RVA: 0x0003A5AC File Offset: 0x000387AC
		public int Scale
		{
			get
			{
				return this._scale;
			}
			set
			{
				if (value == this._scale)
				{
					return;
				}
				if (value < 1 || value > 1000)
				{
					throw new ArgumentOutOfRangeException("Value", value, string.Format("", 1, 1000));
				}
				this._scale = value;
				this.PrintSizeMode = PrintSizeMode.CustomScale;
			}
		}

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06000B85 RID: 2949 RVA: 0x0003A608 File Offset: 0x00038808
		// (set) Token: 0x06000B86 RID: 2950 RVA: 0x0003A610 File Offset: 0x00038810
		public RenderFlags RenderFlags { get; set; }

		// Token: 0x06000B87 RID: 2951 RVA: 0x0003A61C File Offset: 0x0003881C
		public PdfPrintDocument(PdfDocument Document, PrintPageIndexMapper pageIndexMapper, bool isAnnotationVisible = true)
		{
			if (Document == null)
			{
				throw new ArgumentNullException("Document");
			}
			PdfPrintDocument.IsCancel = false;
			this._pdfDoc = Document;
			this._pageIndexMapper = pageIndexMapper;
			this._isAnnotationVisible = isAnnotationVisible;
			this.AutoRotate = true;
			this.AutoCenter = false;
			this.RenderFlags = RenderFlags.FPDF_ANNOT | RenderFlags.FPDF_PRINTING;
			base.PrinterSettings.MinimumPage = 1;
			base.PrinterSettings.MaximumPage = this._pageIndexMapper.PrintPageCount;
			base.PrinterSettings.FromPage = base.PrinterSettings.MinimumPage;
			base.PrinterSettings.ToPage = base.PrinterSettings.MaximumPage;
			base.PrinterSettings.PrintRange = PrintRange.SomePages;
		}

		// Token: 0x06000B88 RID: 2952 RVA: 0x0003A6DC File Offset: 0x000388DC
		protected virtual void OnBeforeRenderPage(Graphics g, PdfPage page, ref int x, ref int y, ref int width, ref int height, PageRotate rotation)
		{
			if (this.BeforeRenderPage != null)
			{
				BeforeRenderPageEventArgs beforeRenderPageEventArgs = new BeforeRenderPageEventArgs(g, page, x, y, width, height, rotation);
				this.BeforeRenderPage(this, beforeRenderPageEventArgs);
				x = beforeRenderPageEventArgs.X;
				y = beforeRenderPageEventArgs.Y;
				width = beforeRenderPageEventArgs.Width;
				height = beforeRenderPageEventArgs.Height;
			}
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x0003A738 File Offset: 0x00038938
		protected virtual void OnAfterRenderPage(Graphics g, PdfPage page, int x, int y, int width, int height, PageRotate rotation)
		{
			if (this.AfterRenderPage != null)
			{
				this.AfterRenderPage(this, new BeforeRenderPageEventArgs(g, page, x, y, width, height, rotation));
			}
		}

		// Token: 0x06000B8A RID: 2954 RVA: 0x0003A76C File Offset: 0x0003896C
		protected override void OnBeginPrint(PrintEventArgs e)
		{
			base.OnBeginPrint(e);
			if (this._pdfDoc == null)
			{
				throw new ArgumentNullException("Document");
			}
			if (base.PrinterSettings.PrintRange != PrintRange.SomePages)
			{
				throw new ArgumentNullException("PrinterSettings.PrintRange");
			}
			this._docForPrint = this._pdfDoc.Handle;
			if (this._docForPrint == IntPtr.Zero)
			{
				e.Cancel = true;
				return;
			}
			this._pageForPrint = base.PrinterSettings.FromPage - 1;
			this.BookletPageOrder = BookletPageHelper.GetBookletPageOrder(this._pageIndexMapper.PrintPageCount, this.BookletBindingDirection, this.PrintReverse, this.BookletSubset);
		}

		// Token: 0x06000B8B RID: 2955 RVA: 0x0003A814 File Offset: 0x00038A14
		protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
		{
			if (this.AutoRotate)
			{
				IntPtr intPtr = Pdfium.FPDF_StartLoadPage(this._docForPrint, this._pageForDoc);
				if (intPtr == IntPtr.Zero)
				{
					e.Cancel = true;
					return;
				}
				double num = Pdfium.FPDF_GetPageWidth(intPtr);
				double num2 = Pdfium.FPDF_GetPageHeight(intPtr);
				Pdfium.FPDFPage_GetRotation(intPtr);
				bool flag = num > num2;
				e.PageSettings.Landscape = flag;
				if (intPtr != IntPtr.Zero)
				{
					Pdfium.FPDF_ClosePage(intPtr);
				}
			}
			base.OnQueryPageSettings(e);
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x0003A890 File Offset: 0x00038A90
		protected override void OnPrintPage(PrintPageEventArgs e)
		{
			base.OnPrintPage(e);
			if (this._pdfDoc == null)
			{
				throw new ArgumentNullException("Document");
			}
			PdfPrintDocument.PdfPageDrawingContext pdfPageDrawingContext = null;
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				if (PdfPrintDocument.IsCancel)
				{
					GAManager.SendEvent("PdfPrintDocument", "PrintIsCancelled", "Count", 1L);
					e.Cancel = true;
				}
				if (!e.Cancel)
				{
					int pageForDoc = this._pageForDoc;
					double num = (double)e.Graphics.DpiX;
					double num2 = (double)e.Graphics.DpiY;
					pdfPageDrawingContext = new PdfPrintDocument.PdfPageDrawingContext(this, e.Graphics);
					if (this.PrintTypeSettingModel == PrintTypeSettingModel.Scale)
					{
						int num3 = this._pageForPrint;
						if (this.PrintReverse)
						{
							num3 = this._pageIndexMapper.PrintPageCount - 1 - this._pageForPrint;
						}
						int docPageIndex = this.GetDocPageIndex(num3, 0);
						intPtr = Pdfium.FPDF_LoadPage(this._docForPrint, docPageIndex);
						if (intPtr == IntPtr.Zero)
						{
							e.Cancel = true;
							return;
						}
						double num4 = Pdfium.FPDF_GetPageWidth(intPtr) / 72.0 * num;
						double num5 = Pdfium.FPDF_GetPageHeight(intPtr) / 72.0 * num2;
						double num6 = num4 / num5;
						if (base.DefaultPageSettings.Landscape && num6 < 1.0)
						{
							FS_RECTF mediaBox = this.GetMediaBox(intPtr);
							if (Math.Abs((double)(mediaBox.Width / mediaBox.Height) - num6) >= 0.05)
							{
								num5 = (double)mediaBox.Width / 72.0 * num;
								num4 = num6 * num5;
							}
							else
							{
								double num7 = num4;
								num4 = num5;
								num5 = num7;
							}
						}
						double num8;
						double num9;
						Rectangle rectangle;
						this.CalcSize(num, num2, e.PageSettings.PrintableArea, e.MarginBounds, new PointF(e.PageSettings.HardMarginX, e.PageSettings.HardMarginY), base.DefaultPageSettings.Landscape, ref num4, ref num5, out num8, out num9, out rectangle);
						int num10 = (int)num8;
						int num11 = (int)num9;
						int num12 = (int)num4;
						int num13 = (int)num5;
						this.AdaptiveWidthAndHeight(num4, num5, num6, PrintTypeSettingModel.Scale, ref num12, ref num13, ref num10, ref num11, 0, 0.0);
						PdfRenderHelper.AnnotationFlagContext annotationFlagContext = default(PdfRenderHelper.AnnotationFlagContext);
						try
						{
							using (PdfPage pdfPage = PdfPage.FromHandle(this._pdfDoc, intPtr, docPageIndex, true))
							{
								this.OnBeforeRenderPage(e.Graphics, pdfPage, ref num10, ref num11, ref num12, ref num13, PageRotate.Normal);
								if (!this._isAnnotationVisible)
								{
									annotationFlagContext = (PdfRenderHelper.AnnotationFlagContext)PdfRenderHelper.CreateHideFlagContext(pdfPage, this._isAnnotationVisible);
									annotationFlagContext.Page = null;
								}
							}
							if (base.OriginAtMargins)
							{
								Pdfium.IntersectClipRect(pdfPageDrawingContext.HDC, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
							}
							pdfPageDrawingContext.RenderPage(intPtr, num10, num11, num12, num13, PageRotate.Normal, this.RenderFlags);
							pdfPageDrawingContext.Dispose();
							pdfPageDrawingContext = null;
							using (PdfPage pdfPage2 = PdfPage.FromHandle(this._pdfDoc, intPtr, docPageIndex, true))
							{
								this.OnAfterRenderPage(e.Graphics, pdfPage2, num10, num11, num12, num13, PageRotate.Normal);
								if (!this._isAnnotationVisible)
								{
									annotationFlagContext.Page = pdfPage2;
									annotationFlagContext.Dispose();
									annotationFlagContext = default(PdfRenderHelper.AnnotationFlagContext);
								}
							}
							this._pageForPrint++;
							goto IL_2613;
						}
						finally
						{
							annotationFlagContext.Dispose();
						}
					}
					if (this.PrintTypeSettingModel == PrintTypeSettingModel.Multiple)
					{
						int i = 0;
						while (i < this.PapersPerSheet)
						{
							PdfRenderHelper.AnnotationFlagContext annotationFlagContext2 = default(PdfRenderHelper.AnnotationFlagContext);
							int num14 = this._pageForPrint;
							if (this.PrintReverse)
							{
								int num15 = this._pageIndexMapper.PrintPageCount - this._pageForPrint + i;
								num14 = ((num15 % this.PapersPerSheet != 0) ? (num15 / this.PapersPerSheet + 1) : (num15 / this.PapersPerSheet)) * this.PapersPerSheet - this.PapersPerSheet + i;
								if (num14 > this._pageIndexMapper.PrintPageCount - 1)
								{
									break;
								}
							}
							else if (this._pageForPrint > this._pageIndexMapper.PrintPageCount - 1)
							{
								break;
							}
							int docPageIndex2 = this.GetDocPageIndex(num14, 0);
							intPtr = Pdfium.FPDF_LoadPage(this._docForPrint, docPageIndex2);
							if (intPtr == IntPtr.Zero)
							{
								e.Cancel = true;
								return;
							}
							double num16 = Pdfium.FPDF_GetPageWidth(intPtr) / 72.0 * num;
							double num17 = Pdfium.FPDF_GetPageHeight(intPtr) / 72.0 * num2;
							double num18 = num16 / num17;
							if (base.DefaultPageSettings.Landscape && num18 < 1.0)
							{
								FS_RECTF mediaBox2 = this.GetMediaBox(intPtr);
								if (Math.Abs((double)(mediaBox2.Width / mediaBox2.Height) - num18) >= 0.05)
								{
									num17 = (double)mediaBox2.Width / 72.0 * num;
									num16 = num18 * num17;
								}
								else
								{
									double num19 = num16;
									num16 = num17;
									num17 = num19;
								}
							}
							double num20;
							double num21;
							Rectangle rectangle2;
							this.CalcSize(num, num2, e.PageSettings.PrintableArea, e.MarginBounds, new PointF(e.PageSettings.HardMarginX, e.PageSettings.HardMarginY), base.DefaultPageSettings.Landscape, ref num16, ref num17, out num20, out num21, out rectangle2);
							int num22 = (int)num20;
							int num23 = (int)num21;
							int num24 = (int)num16 / this.PaperColumnNum;
							int num25 = (int)num17 / this.PaperRowNum;
							double num26 = (double)(rectangle2.Width + rectangle2.X * 2) - (double)e.PageSettings.PrintableArea.X * num / 50.0;
							double num27 = (double)(rectangle2.Height + rectangle2.Y * 2) - (double)e.PageSettings.PrintableArea.Y * num2 / 50.0;
							if (base.DefaultPageSettings.Landscape)
							{
								try
								{
									this.AdaptiveWidthAndHeight(num26, num27, num18, PrintTypeSettingModel.Multiple, ref num24, ref num25, ref num22, ref num23, i, 0.0);
									if (this.PageMargins > 0.0)
									{
										num24 = (int)((double)num24 - this.PageMargins / 2.0 * 0.39370078740157 * num);
										num25 = (int)((double)num25 - this.PageMargins / 2.0 * 0.39370078740157 * num2);
										if (num24 <= 1)
										{
											num24 = 1;
										}
										if (num25 <= 1)
										{
											num25 = 1;
										}
										if (this.isPreviewControl)
										{
											num22 = (int)num20;
											num23 = (int)num21;
										}
										else
										{
											num22 = 0;
											num23 = 0;
										}
										this.AdaptiveMultipleWidthAndHeight(num26, num27, num18, num, num2, PrintTypeSettingModel.Multiple, ref num24, ref num25, ref num22, ref num23, i, this.PageMargins);
									}
									using (PdfPage pdfPage3 = PdfPage.FromHandle(this._pdfDoc, intPtr, docPageIndex2, true))
									{
										this.OnBeforeRenderPage(e.Graphics, pdfPage3, ref num22, ref num23, ref num24, ref num25, PageRotate.Normal);
										if (!this._isAnnotationVisible)
										{
											annotationFlagContext2 = (PdfRenderHelper.AnnotationFlagContext)PdfRenderHelper.CreateHideFlagContext(pdfPage3, this._isAnnotationVisible);
											annotationFlagContext2.Page = null;
										}
									}
									if (base.OriginAtMargins)
									{
										Pdfium.IntersectClipRect(pdfPageDrawingContext.HDC, rectangle2.Left, rectangle2.Top, rectangle2.Right, rectangle2.Bottom);
									}
									pdfPageDrawingContext.RenderPage(intPtr, num22, num23, num24, num25, PageRotate.Normal, this.RenderFlags);
									using (PdfPage pdfPage4 = PdfPage.FromHandle(this._pdfDoc, intPtr, docPageIndex2, true))
									{
										this.OnAfterRenderPage(e.Graphics, pdfPage4, num22, num23, num24, num25, PageRotate.Normal);
										if (!this._isAnnotationVisible)
										{
											annotationFlagContext2.Page = pdfPage4;
											annotationFlagContext2.Dispose();
											annotationFlagContext2 = default(PdfRenderHelper.AnnotationFlagContext);
										}
									}
									this._pageForPrint++;
									goto IL_091D;
								}
								finally
								{
									annotationFlagContext2.Dispose();
								}
								goto IL_074B;
							}
							goto IL_074B;
							IL_091D:
							i++;
							continue;
							IL_074B:
							try
							{
								this.AdaptiveWidthAndHeight(num26, num27, num18, PrintTypeSettingModel.Multiple, ref num24, ref num25, ref num22, ref num23, i, 0.0);
								if (this.PageMargins > 0.0)
								{
									num24 = (int)((double)num24 - this.PageMargins / 2.0 * 0.39370078740157 * num);
									num25 = (int)((double)num25 - this.PageMargins / 2.0 * 0.39370078740157 * num2);
									if (num24 <= 1)
									{
										num24 = 1;
									}
									if (num25 <= 1)
									{
										num25 = 1;
									}
									if (this.isPreviewControl)
									{
										num22 = (int)num20;
										num23 = (int)num21;
									}
									else
									{
										num22 = 0;
										num23 = 0;
									}
									this.AdaptiveMultipleWidthAndHeight(num26, num27, num18, num, num2, PrintTypeSettingModel.Multiple, ref num24, ref num25, ref num22, ref num23, i, this.PageMargins);
								}
								using (PdfPage pdfPage5 = PdfPage.FromHandle(this._pdfDoc, intPtr, docPageIndex2, true))
								{
									this.OnBeforeRenderPage(e.Graphics, pdfPage5, ref num22, ref num23, ref num24, ref num25, PageRotate.Normal);
									if (!this._isAnnotationVisible)
									{
										annotationFlagContext2 = (PdfRenderHelper.AnnotationFlagContext)PdfRenderHelper.CreateHideFlagContext(pdfPage5, this._isAnnotationVisible);
										annotationFlagContext2.Page = null;
									}
								}
								if (base.OriginAtMargins)
								{
									Pdfium.IntersectClipRect(pdfPageDrawingContext.HDC, rectangle2.Left, rectangle2.Top, rectangle2.Right, rectangle2.Bottom);
								}
								pdfPageDrawingContext.RenderPage(intPtr, num22, num23, num24, num25, PageRotate.Normal, this.RenderFlags);
								using (PdfPage pdfPage6 = PdfPage.FromHandle(this._pdfDoc, intPtr, docPageIndex2, true))
								{
									this.OnAfterRenderPage(e.Graphics, pdfPage6, num22, num23, num24, num25, PageRotate.Normal);
									if (!this._isAnnotationVisible)
									{
										annotationFlagContext2.Page = pdfPage6;
										annotationFlagContext2.Dispose();
										annotationFlagContext2 = default(PdfRenderHelper.AnnotationFlagContext);
									}
								}
								this._pageForPrint++;
							}
							finally
							{
								annotationFlagContext2.Dispose();
							}
							goto IL_091D;
						}
						if (this.PrintBorder)
						{
							if (pdfPageDrawingContext != null)
							{
								pdfPageDrawingContext.Dispose();
							}
							pdfPageDrawingContext = null;
							int num28 = this._pageForPrint;
							if (this.PapersPerSheet > this.Document.Pages.Count)
							{
								this.PapersPerSheet = this.Document.Pages.Count;
							}
							if (this._pageForPrint % this.PapersPerSheet != 0 && !this.PrintReverse)
							{
								this.PapersPerSheet = this._pageForPrint % this.PapersPerSheet;
							}
							int j = 0;
							while (j < this.PapersPerSheet)
							{
								if (this.PrintReverse)
								{
									int num29 = this.Document.Pages.Count - this._pageForPrint + j + 1;
									int num30 = 1;
									if (num29 != 0)
									{
										num30 = ((num29 % this.PapersPerSheet != 0) ? (num29 / this.PapersPerSheet + 1) : (num29 / this.PapersPerSheet));
									}
									num28 = num30 * this.PapersPerSheet;
									if (num28 - this.PapersPerSheet + j <= this.Document.Pages.Count - 1)
									{
										goto IL_0A4E;
									}
								}
								else if (this._pageForPrint - this.PapersPerSheet + j < this._pageIndexMapper.PrintPageCount)
								{
									goto IL_0A4E;
								}
								IL_0E3B:
								j++;
								continue;
								IL_0A4E:
								intPtr = Pdfium.FPDF_LoadPage(this._docForPrint, this.GetDocPageIndex(num28 - this.PapersPerSheet + j, 0));
								if (intPtr == IntPtr.Zero)
								{
									e.Cancel = true;
									return;
								}
								double num31 = Pdfium.FPDF_GetPageWidth(intPtr) / 72.0 * num;
								double num32 = Pdfium.FPDF_GetPageHeight(intPtr) / 72.0 * num2;
								double num33 = num31 / num32;
								if (base.DefaultPageSettings.Landscape && num33 < 1.0)
								{
									FS_RECTF mediaBox3 = this.GetMediaBox(intPtr);
									if (Math.Abs((double)(mediaBox3.Width / mediaBox3.Height) - num33) >= 0.05)
									{
										num32 = (double)mediaBox3.Width / 72.0 * num;
										num31 = num33 * num32;
									}
									else
									{
										double num34 = num31;
										num31 = num32;
										num32 = num34;
									}
								}
								double num35;
								double num36;
								Rectangle rectangle3;
								this.CalcSize(num, num2, e.PageSettings.PrintableArea, e.MarginBounds, new PointF(e.PageSettings.HardMarginX, e.PageSettings.HardMarginY), base.DefaultPageSettings.Landscape, ref num31, ref num32, out num35, out num36, out rectangle3);
								int num37 = (int)num35;
								int num38 = (int)num36;
								int num39 = (int)num31 / this.PaperColumnNum;
								int num40 = (int)num32 / this.PaperRowNum;
								double num41 = (double)(rectangle3.Width + rectangle3.X * 2) - (double)e.PageSettings.PrintableArea.X * num / 50.0 - 10.0;
								double num42 = (double)(rectangle3.Height + rectangle3.Y * 2) - (double)e.PageSettings.PrintableArea.Y * num2 / 50.0 - 10.0;
								this.AdaptiveWidthAndHeight(num41, num42, num33, PrintTypeSettingModel.Multiple, ref num39, ref num40, ref num37, ref num38, j, 0.0);
								if (this.PageMargins > 0.0)
								{
									num39 = (int)((double)num39 - this.PageMargins / 2.0 * 0.39370078740157 * num);
									num40 = (int)((double)num40 - this.PageMargins / 2.0 * 0.39370078740157 * num2);
									if (num39 <= 1)
									{
										num39 = 1;
									}
									if (num40 <= 1)
									{
										num40 = 1;
									}
									if (this.isPreviewControl)
									{
										num37 = (int)num35;
										num38 = (int)num36;
									}
									else
									{
										num37 = 0;
										num38 = 0;
									}
									this.AdaptiveMultipleWidthAndHeight(num41, num42, num33, num, num2, PrintTypeSettingModel.Multiple, ref num39, ref num40, ref num37, ref num38, j, this.PageMargins);
								}
								using (Pen pen = new Pen(Color.Black, 1f))
								{
									e.Graphics.DrawLine(pen, (float)((double)num37 / num * 100.0), (float)((double)num38 / num2 * 100.0), (float)((double)num37 / num * 100.0), (float)((double)(num38 + num40) / num2 * 100.0));
									e.Graphics.DrawLine(pen, (float)((double)num37 / num * 100.0), (float)((double)num38 / num2 * 100.0), (float)((double)(num37 + num39) / num * 100.0), (float)((double)num38 / num2 * 100.0));
									e.Graphics.DrawLine(pen, (float)((double)num37 / num * 100.0), (float)((double)(num38 + num40) / num2 * 100.0), (float)((double)(num37 + num39) / num * 100.0), (float)((double)(num38 + num40) / num2 * 100.0));
									e.Graphics.DrawLine(pen, (float)((double)(num37 + num39) / num * 100.0), (float)((double)num38 / num2 * 100.0), (float)((double)(num37 + num39) / num * 100.0), (float)((double)(num38 + num40) / num2 * 100.0));
								}
								goto IL_0E3B;
							}
							this.PapersPerSheet = this.PaperColumnNum * this.PaperRowNum;
						}
					}
					else if (this.PrintTypeSettingModel == PrintTypeSettingModel.Booklet)
					{
						this.PaperColumnNum = 2;
						this.PaperRowNum = 1;
						this.PapersPerSheet = 2;
						for (int k = 0; k < this.PapersPerSheet; k++)
						{
							PdfRenderHelper.AnnotationFlagContext annotationFlagContext3 = default(PdfRenderHelper.AnnotationFlagContext);
							if (this.GetDocPageIndex(this.BookletPageOrder[this._pageForPrint], 1) >= this.Document.Pages.Count || this.GetDocPageIndex(this.BookletPageOrder[this._pageForPrint], 1) < 0)
							{
								this._pageForPrint++;
							}
							else
							{
								int docPageIndex3 = this.GetDocPageIndex(this.BookletPageOrder[this._pageForPrint], 1);
								intPtr = Pdfium.FPDF_LoadPage(this._docForPrint, docPageIndex3);
								if (intPtr == IntPtr.Zero)
								{
									e.Cancel = true;
									return;
								}
								double num43 = Pdfium.FPDF_GetPageWidth(intPtr) / 72.0 * num;
								double num44 = Pdfium.FPDF_GetPageHeight(intPtr) / 72.0 * num2;
								double num45 = num43 / num44;
								if (num45 < 1.0)
								{
									FS_RECTF mediaBox4 = this.GetMediaBox(intPtr);
									if (Math.Abs((double)(mediaBox4.Width / mediaBox4.Height) - num45) >= 0.05)
									{
										num44 = (double)mediaBox4.Width / 72.0 * num;
										num43 = num45 * num44;
									}
									else
									{
										double num46 = num43;
										num43 = num44;
										num44 = num46;
									}
								}
								double num47;
								double num48;
								Rectangle rectangle4;
								this.CalcSize(num, num2, e.PageSettings.PrintableArea, e.MarginBounds, new PointF(e.PageSettings.HardMarginX, e.PageSettings.HardMarginY), base.DefaultPageSettings.Landscape, ref num43, ref num44, out num47, out num48, out rectangle4);
								int num49 = (int)num47;
								int num50 = (int)num48;
								int num51 = (int)num43 / this.PaperColumnNum;
								int num52 = (int)num44 / this.PaperRowNum;
								double num53 = (double)(rectangle4.Width + rectangle4.X * 2) - (double)e.PageSettings.PrintableArea.X * num / 50.0;
								double num54 = (double)(rectangle4.Height + rectangle4.Y * 2) - (double)e.PageSettings.PrintableArea.Y * num2 / 50.0;
								if (this.BookletPageOrder[this._pageForPrint] > this._pageIndexMapper.PrintPageCount)
								{
									break;
								}
								try
								{
									this.AdaptiveWidthAndHeight(num53, num54, num45, PrintTypeSettingModel.Multiple, ref num51, ref num52, ref num49, ref num50, k, 0.0);
									if (this.PageMargins > 0.0)
									{
										num51 = (int)((double)num51 - this.PageMargins / 2.0 * 0.39370078740157 * num);
										num52 = (int)((double)num52 - this.PageMargins / 2.0 * 0.39370078740157 * num2);
										if (num51 <= 1)
										{
											num51 = 1;
										}
										if (num52 <= 1)
										{
											num52 = 1;
										}
										if (this.isPreviewControl)
										{
											num49 = (int)num47;
											num50 = (int)num48;
										}
										else
										{
											num49 = 0;
											num50 = 0;
										}
										this.AdaptiveMultipleWidthAndHeight(num53, num54, num45, num, num2, PrintTypeSettingModel.Multiple, ref num51, ref num52, ref num49, ref num50, k, this.PageMargins);
									}
									using (PdfPage pdfPage7 = PdfPage.FromHandle(this._pdfDoc, intPtr, docPageIndex3, true))
									{
										this.OnBeforeRenderPage(e.Graphics, pdfPage7, ref num49, ref num50, ref num51, ref num52, PageRotate.Normal);
										if (!this._isAnnotationVisible)
										{
											annotationFlagContext3 = (PdfRenderHelper.AnnotationFlagContext)PdfRenderHelper.CreateHideFlagContext(pdfPage7, this._isAnnotationVisible);
											annotationFlagContext3.Page = null;
										}
									}
									if (base.OriginAtMargins)
									{
										Pdfium.IntersectClipRect(pdfPageDrawingContext.HDC, rectangle4.Left, rectangle4.Top, rectangle4.Right, rectangle4.Bottom);
									}
									pdfPageDrawingContext.RenderPage(intPtr, num49, num50, num51, num52, PageRotate.Normal, this.RenderFlags);
									using (PdfPage pdfPage8 = PdfPage.FromHandle(this._pdfDoc, intPtr, docPageIndex3, true))
									{
										this.OnAfterRenderPage(e.Graphics, pdfPage8, num49, num50, num51, num52, PageRotate.Normal);
										if (!this._isAnnotationVisible)
										{
											annotationFlagContext3.Page = pdfPage8;
											annotationFlagContext3.Dispose();
											annotationFlagContext3 = default(PdfRenderHelper.AnnotationFlagContext);
										}
									}
									this._pageForPrint++;
								}
								finally
								{
									annotationFlagContext3.Dispose();
								}
							}
						}
						if (this.isPreviewControl)
						{
							this._pageForPrint = this._pageIndexMapper.PrintPageCount;
						}
					}
					else if (this.PrintTypeSettingModel == PrintTypeSettingModel.Tile)
					{
						int num55 = this._pageForPrint;
						if (this.PrintReverse)
						{
							num55 = this._pageIndexMapper.PrintPageCount - 1 - this._pageForPrint;
						}
						int docPageIndex4 = this.GetDocPageIndex(num55, 0);
						if (this.isPreviewControl)
						{
							intPtr = Pdfium.FPDF_LoadPage(this._docForPrint, docPageIndex4);
							if (intPtr == IntPtr.Zero)
							{
								e.Cancel = true;
								return;
							}
							double num56 = Pdfium.FPDF_GetPageWidth(intPtr) / 72.0 * num;
							double num57 = Pdfium.FPDF_GetPageHeight(intPtr) / 72.0 * num2;
							double num58 = num56 / num57;
							bool flag = num56 > num57;
							if (flag && num58 < 1.0)
							{
								double num59 = num56;
								num56 = num57;
								num57 = num59;
							}
							double num60;
							double num61;
							Rectangle rectangle5;
							this.CalcSize(num, num2, e.PageSettings.PrintableArea, e.MarginBounds, new PointF(e.PageSettings.HardMarginX, e.PageSettings.HardMarginY), flag, ref num56, ref num57, out num60, out num61, out rectangle5);
							int num62 = (int)num60;
							int num63 = (int)num61;
							int num64 = (int)num56;
							int num65 = (int)num57;
							this.AdaptiveWidthAndHeight(num56, num57, num58, PrintTypeSettingModel.Scale, ref num64, ref num65, ref num62, ref num63, 0, 0.0);
							PdfRenderHelper.AnnotationFlagContext annotationFlagContext4 = default(PdfRenderHelper.AnnotationFlagContext);
							try
							{
								using (PdfPage pdfPage9 = PdfPage.FromHandle(this._pdfDoc, intPtr, docPageIndex4, true))
								{
									this.OnBeforeRenderPage(e.Graphics, pdfPage9, ref num62, ref num63, ref num64, ref num65, PageRotate.Normal);
									if (!this._isAnnotationVisible)
									{
										annotationFlagContext4 = (PdfRenderHelper.AnnotationFlagContext)PdfRenderHelper.CreateHideFlagContext(pdfPage9, this._isAnnotationVisible);
										annotationFlagContext4.Page = null;
									}
								}
								if (base.OriginAtMargins)
								{
									Pdfium.IntersectClipRect(pdfPageDrawingContext.HDC, rectangle5.Left, rectangle5.Top, rectangle5.Right, rectangle5.Bottom);
								}
								pdfPageDrawingContext.RenderPage(intPtr, num62, num63, num64, num65, PageRotate.Normal, this.RenderFlags);
								pdfPageDrawingContext.Dispose();
								pdfPageDrawingContext = null;
								using (PdfPage pdfPage10 = PdfPage.FromHandle(this._pdfDoc, intPtr, docPageIndex4, true))
								{
									this.OnAfterRenderPage(e.Graphics, pdfPage10, num62, num63, num64, num65, PageRotate.Normal);
									if (!this._isAnnotationVisible)
									{
										annotationFlagContext4.Page = pdfPage10;
										annotationFlagContext4.Dispose();
										annotationFlagContext4 = default(PdfRenderHelper.AnnotationFlagContext);
									}
								}
								this._pageForPrint++;
								goto IL_2613;
							}
							finally
							{
								annotationFlagContext4.Dispose();
							}
						}
						intPtr = Pdfium.FPDF_LoadPage(this._docForPrint, docPageIndex4);
						if (intPtr == IntPtr.Zero)
						{
							e.Cancel = true;
							return;
						}
						double num66 = Pdfium.FPDF_GetPageWidth(intPtr) / 72.0 * num;
						double num67 = Pdfium.FPDF_GetPageHeight(intPtr) / 72.0 * num2;
						double num68 = num66 / num67;
						num67 = num67 * this.TilePageZoom / 100.0;
						num66 = num66 * this.TilePageZoom / 100.0;
						double num69 = (double)e.PageSettings.PrintableArea.Height / 100.0 * num2;
						double num70 = (double)e.PageSettings.PrintableArea.Width / 100.0 * num;
						if (this.TileCutMasks || this.TileLabels)
						{
							num67 -= 0.2 * num2 * 2.0;
							num66 -= 0.2 * num * 2.0;
						}
						if (base.DefaultPageSettings.Landscape)
						{
							num69 = (double)(e.PageSettings.PaperSize.Width / 100) * num;
							num70 = (double)(e.PageSettings.PaperSize.Height / 100) * num2;
						}
						int num71 = (int)Math.Ceiling(Math.Round(num67 / num69, 2));
						int num72 = (int)Math.Ceiling(Math.Round(num66 / num70, 2));
						if (this.TileCutMasks || this.TileLabels)
						{
							num67 += 0.2 * num2 * 4.0;
							num66 += 0.2 * num * 4.0;
						}
						double num73 = (double)num71 * num69;
						double num74 = (double)num72 * num70;
						if ((double)(num71 - 1) * this.TileOverlap * 0.39370078740157 * num2 + num67 > num73)
						{
							num71++;
							num73 = (double)num71 * num69;
						}
						if ((double)(num72 - 1) * this.TileOverlap * 0.39370078740157 * num + num66 > num74)
						{
							num72++;
							num74 = (double)num72 * num70;
						}
						double num75;
						double num76;
						Rectangle rectangle6;
						this.CalcSize(num, num2, e.PageSettings.PrintableArea, e.MarginBounds, new PointF(e.PageSettings.HardMarginX, e.PageSettings.HardMarginY), base.DefaultPageSettings.Landscape, ref num66, ref num67, out num75, out num76, out rectangle6);
						int num77 = (int)num75;
						int num78 = (int)num76;
						int num79 = (int)(num66 * this.TilePageZoom / 100.0 + (double)(num72 - 1) * (this.TileOverlap * 0.39370078740157 + (double)(e.PageSettings.PrintableArea.X / 100f)) * num);
						int num80 = (int)(num67 * this.TilePageZoom / 100.0 + (double)(num71 - 1) * (this.TileOverlap * 0.39370078740157 + (double)(e.PageSettings.PrintableArea.Y / 100f)) * num2);
						int num81 = 0;
						int num82 = 0;
						if (this.TilePaperNumber != 0)
						{
							num81 = this.TilePaperNumber % num72;
							num82 = this.TilePaperNumber / num72;
						}
						if (num82 == 0)
						{
							num78 = (int)(num73 - (double)num80) / 2;
						}
						else
						{
							num78 = (int)(-(int)(num69 * (double)num82 - (double)(e.PageSettings.PrintableArea.Y / 100f) * num2 * (double)(2 * num82 - 1) - (double)((int)(num73 - (double)num80) / 2)));
						}
						if (num81 == 0)
						{
							num77 = (int)(num74 - (double)num79) / 2;
						}
						else
						{
							num77 = (int)(-(int)(num70 * (double)num81 - (double)(e.PageSettings.PrintableArea.X / 100f) * num * (double)(2 * num81 - 1) - (double)((int)(num74 - (double)num79) / 2)));
						}
						num79 = (int)(num66 * this.TilePageZoom / 100.0);
						num80 = (int)(num67 * this.TilePageZoom / 100.0);
						if (this.TileOverlap > 0.0)
						{
							num77 += (int)(this.TileOverlap * (double)num81 * 0.39370078740157 * num);
							num78 += (int)(this.TileOverlap * (double)num82 * 0.39370078740157 * num2);
						}
						this.AdaptiveWidthAndHeight(num66, num67, num68, PrintTypeSettingModel.Tile, ref num79, ref num80, ref num77, ref num78, 0, 0.0);
						PdfRenderHelper.AnnotationFlagContext annotationFlagContext5 = default(PdfRenderHelper.AnnotationFlagContext);
						try
						{
							using (PdfPage pdfPage11 = PdfPage.FromHandle(this._pdfDoc, intPtr, docPageIndex4, true))
							{
								this.OnBeforeRenderPage(e.Graphics, pdfPage11, ref num77, ref num78, ref num79, ref num80, PageRotate.Normal);
								if (!this._isAnnotationVisible)
								{
									annotationFlagContext5 = (PdfRenderHelper.AnnotationFlagContext)PdfRenderHelper.CreateHideFlagContext(pdfPage11, this._isAnnotationVisible);
									annotationFlagContext5.Page = null;
								}
							}
							if (base.OriginAtMargins)
							{
								Pdfium.IntersectClipRect(pdfPageDrawingContext.HDC, rectangle6.Left, rectangle6.Top, rectangle6.Right, rectangle6.Bottom);
							}
							if (this.TileCutMasks || this.TileLabels)
							{
								int num83 = (int)(0.2 * num);
								int num84 = (int)(0.2 * num2);
								int num85 = (int)(num70 - 0.2 * num);
								int num86 = (int)(num69 - 0.2 * num2);
								Pdfium.IntersectClipRect(pdfPageDrawingContext.HDC, num83, num84, num85, num86);
							}
							pdfPageDrawingContext.RenderPage(intPtr, num77, num78, num79, num80, PageRotate.Normal, this.RenderFlags);
							pdfPageDrawingContext.Dispose();
							pdfPageDrawingContext = null;
							using (PdfPage pdfPage12 = PdfPage.FromHandle(this._pdfDoc, intPtr, docPageIndex4, true))
							{
								this.OnAfterRenderPage(e.Graphics, pdfPage12, num77, num78, num79, num80, PageRotate.Normal);
								if (!this._isAnnotationVisible)
								{
									annotationFlagContext5.Page = pdfPage12;
									annotationFlagContext5.Dispose();
									annotationFlagContext5 = default(PdfRenderHelper.AnnotationFlagContext);
								}
							}
							if (this.TileCutMasks)
							{
								float num87 = e.PageSettings.PrintableArea.Width;
								float num88 = e.PageSettings.PrintableArea.Height;
								if (base.DefaultPageSettings.Landscape)
								{
									float num89 = num88;
									num88 = num87;
									num87 = num89;
								}
								using (Pen pen2 = new Pen(Color.Black, 1f))
								{
									e.Graphics.DrawLine(pen2, 20f, 0f, 20f, 20f);
									e.Graphics.DrawLine(pen2, 0f, 20f, 20f, 20f);
									e.Graphics.DrawLine(pen2, 0f, (float)((double)num88 - 20.0), 20f, (float)((double)num88 - 20.0));
									e.Graphics.DrawLine(pen2, 20f, (float)((double)num88 - 20.0), 20f, num88);
									e.Graphics.DrawLine(pen2, (float)((double)num87 - 20.0), 0f, (float)((double)num87 - 20.0), 20f);
									e.Graphics.DrawLine(pen2, (float)((double)num87 - 20.0), 20f, num87, 20f);
									e.Graphics.DrawLine(pen2, (float)((double)num87 - 20.0), (float)((double)num88 - 20.0), (float)((double)num87 - 20.0), num88);
									e.Graphics.DrawLine(pen2, (float)((double)num87 - 20.0), (float)((double)num88 - 20.0), num87, (float)((double)num88 - 20.0));
								}
								if (this.TileOverlap > 0.0)
								{
									using (Pen pen3 = new Pen(Color.Black, 1f))
									{
										e.Graphics.DrawLine(pen3, 20f, 0f, 20f, 20f);
										e.Graphics.DrawLine(pen3, 0f, 20f, 20f, 20f);
										if (num81 != 0)
										{
											e.Graphics.DrawLine(pen3, 20f, (float)(20.0 + (double)(num88 / 2f)), (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), (float)(20.0 + (double)(num88 / 2f)));
											e.Graphics.DrawLine(pen3, (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), (float)(20.0 + (double)(num88 / 2f)) - 10f, (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), (float)(20.0 + (double)(num88 / 2f)) + 10f);
											e.Graphics.DrawLine(pen3, (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), 0f, (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), 20f);
											e.Graphics.DrawLine(pen3, (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), (float)((double)num88 - 20.0), (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), num88);
										}
										if (num81 != num72 - 1)
										{
											e.Graphics.DrawLine(pen3, (float)((double)num87 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0 - 20.0), (float)(20.0 + (double)(num88 / 2f)), (float)((double)num87 - 20.0), (float)(20.0 + (double)(num88 / 2f)));
											e.Graphics.DrawLine(pen3, (float)(-20.0 + (double)num87 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), (float)(20.0 + (double)(num88 / 2f)) - 10f, (float)(-20.0 + (double)num87 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), (float)(20.0 + (double)(num88 / 2f)) + 10f);
											e.Graphics.DrawLine(pen3, (float)(-20.0 + (double)num87 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), 0f, (float)(-20.0 + (double)num87 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), 20f);
											e.Graphics.DrawLine(pen3, (float)(-20.0 + (double)num87 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), (float)((double)num88 - 20.0), (float)(-20.0 + (double)num87 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), num88);
										}
										if (num82 != 0)
										{
											e.Graphics.DrawLine(pen3, num87 / 2f, 20f, num87 / 2f, (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0));
											e.Graphics.DrawLine(pen3, num87 / 2f - 10f, (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), num87 / 2f + 10f, (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0));
											e.Graphics.DrawLine(pen3, 0f, (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), 20f, (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0));
											e.Graphics.DrawLine(pen3, num87, (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), (float)((double)num87 - 20.0), (float)(20.0 + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0));
										}
										if (num82 != num71 - 1)
										{
											e.Graphics.DrawLine(pen3, num87 / 2f, (float)(-20.0 + (double)num88 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), num87 / 2f, (float)(-20.0 + (double)num88));
											e.Graphics.DrawLine(pen3, num87 / 2f - 10f, (float)(-20.0 + (double)num88 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), num87 / 2f + 10f, (float)(-20.0 + (double)num88 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0));
											e.Graphics.DrawLine(pen3, 0f, (float)(-20.0 + (double)num88 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), 20f, (float)(-20.0 + (double)num88 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0));
											e.Graphics.DrawLine(pen3, num87, (float)(-20.0 + (double)num88 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), (float)((double)num87 - 20.0), (float)(-20.0 + (double)num88 - this.TileOverlap / 2.0 * 0.39370078740157 * 100.0));
										}
									}
								}
							}
							if (this.TileLabels)
							{
								string text = string.Format("({0},{1})-1 -{2} {3}", new object[]
								{
									num81 + 1,
									num82 + 1,
									this.TileFilePath,
									DateTime.Now
								});
								Font font = new Font("Arial", 8f);
								Brush black = Brushes.Black;
								e.Graphics.DrawString(text, font, black, (float)(20.0 + (double)e.PageSettings.PrintableArea.X + this.TileOverlap / 2.0 * 0.39370078740157 * 100.0), 0f);
							}
							this.TilePaperNumber++;
						}
						finally
						{
							annotationFlagContext5.Dispose();
						}
						if (this.TilePaperNumber == num71 * num72)
						{
							this.TilePaperNumber = 0;
							this._pageForPrint++;
						}
						else
						{
							e.HasMorePages = true;
						}
					}
					IL_2613:
					if (this._pageForPrint < this._pageIndexMapper.PrintPageCount && this._pageForPrint < base.PrinterSettings.ToPage && this.PrintTypeSettingModel != PrintTypeSettingModel.Booklet)
					{
						e.HasMorePages = true;
					}
					else if (this.PrintTypeSettingModel == PrintTypeSettingModel.Booklet && this._pageForPrint < this.BookletPageOrder.Count<int>() && !this.isPreviewControl)
					{
						e.HasMorePages = true;
					}
				}
			}
			finally
			{
				if (pdfPageDrawingContext != null)
				{
					pdfPageDrawingContext.Dispose();
				}
				pdfPageDrawingContext = null;
				if (intPtr != IntPtr.Zero)
				{
					Pdfium.FPDF_ClosePage(intPtr);
				}
			}
		}

		// Token: 0x06000B8D RID: 2957 RVA: 0x0003D148 File Offset: 0x0003B348
		private int GetDocPageIndex(int printPageIndex, int printPageStartIndex = 0)
		{
			int num;
			if (!this._pageIndexMapper.TryGetDocumentPageIndex(printPageIndex, printPageStartIndex, out num))
			{
				return -1;
			}
			return num;
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x0003D16C File Offset: 0x0003B36C
		private FS_RECTF GetMediaBox(IntPtr page)
		{
			float num;
			float num2;
			float num3;
			float num4;
			if (Pdfium.FPDFPage_GetMediaBox(page, out num, out num2, out num3, out num4))
			{
				return new FS_RECTF(num, num4, num3, num2);
			}
			return default(FS_RECTF);
		}

		// Token: 0x06000B8F RID: 2959 RVA: 0x0003D1A0 File Offset: 0x0003B3A0
		private void CalcSize(double dpiX, double dpiY, RectangleF printableArea, Rectangle marginBounds, PointF hardMargin, bool isLandscape, ref double width, ref double height, out double x, out double y, out Rectangle clipRect)
		{
			x = (y = 0.0);
			clipRect = Rectangle.Empty;
			RectangleF rectangleF = ((!base.OriginAtMargins) ? new RectangleF(printableArea.X, printableArea.Y, printableArea.Width, printableArea.Height) : new RectangleF((float)marginBounds.X, (float)marginBounds.Y, (float)marginBounds.Width, (float)marginBounds.Height));
			if (isLandscape)
			{
				rectangleF = new RectangleF(rectangleF.X, rectangleF.Y, rectangleF.Height, rectangleF.Width);
			}
			SizeF sizeF = new SizeF((float)dpiX * rectangleF.Width / 100f, (float)dpiY * rectangleF.Height / 100f);
			SizeF sizeF2 = new SizeF((float)width, (float)height);
			if (base.OriginAtMargins && isLandscape)
			{
				sizeF = new SizeF(sizeF.Height, sizeF.Width);
			}
			if (this.PrintTypeSettingModel == PrintTypeSettingModel.Scale)
			{
				switch (this.PrintSizeMode)
				{
				case PrintSizeMode.Fit:
				{
					SizeF renderSize = this.GetRenderSize(sizeF2, sizeF);
					width = (double)renderSize.Width;
					height = (double)renderSize.Height;
					break;
				}
				case PrintSizeMode.CustomScale:
					width *= (double)this.Scale / 100.0;
					height *= (double)this.Scale / 100.0;
					break;
				}
			}
			else
			{
				SizeF renderSize2 = this.GetRenderSize(sizeF2, sizeF);
				width = (double)renderSize2.Width;
				height = (double)renderSize2.Height;
			}
			x = (double)rectangleF.X * dpiX / 100.0 - (double)hardMargin.X * dpiX / 100.0;
			y = (double)rectangleF.Y * dpiY / 100.0 - (double)hardMargin.Y * dpiY / 100.0;
			if (this.AutoCenter)
			{
				if (this.PrintTypeSettingModel == PrintTypeSettingModel.Multiple || this.PrintTypeSettingModel == PrintTypeSettingModel.Booklet)
				{
					x = (x + (double)printableArea.X * dpiX / 100.0) / (double)this.PaperColumnNum;
					y = (y + (double)printableArea.Y * dpiY / 100.0) / (double)this.PaperRowNum;
				}
				else
				{
					x = x + ((double)sizeF.Width - width) / 2.0 + (double)printableArea.X * dpiX / 100.0;
					y = y + ((double)sizeF.Height - height) / 2.0 + (double)printableArea.Y * dpiY / 100.0;
				}
			}
			clipRect = new Rectangle((int)((double)marginBounds.Left * dpiX / 100.0), (int)((double)marginBounds.Top * dpiY / 100.0), (int)((double)marginBounds.Width * dpiX / 100.0), (int)((double)marginBounds.Height * dpiX / 100.0));
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x0003D4B8 File Offset: 0x0003B6B8
		private SizeF GetRenderSize(SizeF pageSize, SizeF fitSize)
		{
			double num = (double)pageSize.Width;
			double num2 = (double)pageSize.Height;
			double num3 = (double)fitSize.Height;
			double num4 = num * num3 / num2;
			if (num4 > (double)fitSize.Width)
			{
				num4 = (double)fitSize.Width;
				num3 = num2 * num4 / num;
			}
			return new SizeF((float)num4, (float)num3);
		}

		// Token: 0x06000B91 RID: 2961 RVA: 0x0003D50C File Offset: 0x0003B70C
		private void AdaptiveMarginsWidthAndHeight(double pageWidth, double pageHeight, double scal, ref int width, ref int height)
		{
			if (scal >= 1.0)
			{
				if (width >= height)
				{
					if ((double)width / scal > (double)height)
					{
						width = (int)((double)height * scal);
						return;
					}
					height = (int)((double)width / scal);
					return;
				}
				else
				{
					if ((double)width / scal > (double)height)
					{
						width = (int)((double)height * scal);
						return;
					}
					height = (int)((double)width / scal);
					return;
				}
			}
			else if (width >= height)
			{
				if ((double)height * scal > (double)width)
				{
					height = (int)((double)width / scal);
					return;
				}
				width = (int)((double)height * scal);
				return;
			}
			else
			{
				if ((double)width / scal > (double)height)
				{
					width = (int)((double)height * scal);
					return;
				}
				height = (int)((double)width / scal);
				return;
			}
		}

		// Token: 0x06000B92 RID: 2962 RVA: 0x0003D5BC File Offset: 0x0003B7BC
		private void AdaptiveMultipleWidthAndHeight(double pageWidth, double pageHeight, double scal, double dpix, double dpiy, PrintTypeSettingModel printTypeSettingModel, ref int width, ref int height, ref int x2, ref int y2, int i = 0, double pageMsargin = 0.0)
		{
			double num = pageWidth - (double)(this.PaperColumnNum - 1) * (pageMsargin * 0.39370078740157 * dpix);
			double num2 = pageHeight - (double)(this.PaperRowNum - 1) * (pageMsargin * 0.39370078740157 * dpiy);
			double num3 = num / (double)this.PaperColumnNum;
			double num4 = num2 / (double)this.PaperRowNum;
			if (scal >= 1.0)
			{
				double num5 = num3;
				double num6 = num5 / scal;
				if (num6 > num4)
				{
					num6 = num4;
					num5 = num6 * scal;
				}
				width = (int)num5;
				height = (int)num6;
			}
			else
			{
				double num7 = 1.0 / scal;
				double num8 = num4;
				double num9 = num8 * scal;
				if (num9 > num3)
				{
					num9 = num3;
					num8 = num9 / scal;
				}
				width = (int)num9;
				height = (int)num8;
			}
			if (printTypeSettingModel == PrintTypeSettingModel.Multiple)
			{
				int num10;
				int num11;
				switch (this.PageOrder)
				{
				case PageOrder.HorizontalReverse:
					num10 = this.PaperColumnNum - i % this.PaperColumnNum - 1;
					num11 = i / this.PaperColumnNum % this.PaperRowNum;
					break;
				case PageOrder.Vertical:
					num10 = i / this.PaperRowNum % this.PaperColumnNum;
					num11 = i % this.PaperRowNum;
					break;
				case PageOrder.VerticalReverse:
					num10 = this.PaperColumnNum - i / this.PaperRowNum % this.PaperColumnNum - 1;
					num11 = i % this.PaperRowNum;
					break;
				default:
					num10 = i % this.PaperColumnNum;
					num11 = i / this.PaperColumnNum % this.PaperRowNum;
					break;
				}
				double num12 = (double)num10 * (num3 + pageMsargin * 0.39370078740157 * dpix);
				double num13 = (double)num11 * (num4 + pageMsargin * 0.39370078740157 * dpiy);
				int num14 = (int)((num3 - (double)width) / 2.0);
				int num15 = (int)((num4 - (double)height) / 2.0);
				x2 += (int)num12 + num14;
				y2 += (int)num13 + num15;
			}
		}

		// Token: 0x06000B93 RID: 2963 RVA: 0x0003D798 File Offset: 0x0003B998
		private void AdaptiveWidthAndHeight(double pageWidth, double pageHeight, double scal, PrintTypeSettingModel printTypeSettingModel, ref int width, ref int height, ref int x2, ref int y2, int i = 0, double pageMsargin = 0.0)
		{
			if (scal >= 1.0)
			{
				if (width >= height)
				{
					if ((double)width / scal > (double)height)
					{
						width = (int)((double)height * scal);
					}
					else
					{
						height = (int)((double)width / scal);
					}
				}
				else if ((double)width / scal > (double)height)
				{
					width = (int)((double)height * scal);
				}
				else
				{
					height = (int)((double)width / scal);
				}
			}
			else if (width >= height)
			{
				if ((double)height * scal > (double)width)
				{
					height = (int)((double)width / scal);
				}
				else
				{
					width = (int)((double)height * scal);
				}
			}
			else if ((double)width / scal > (double)height)
			{
				width = (int)((double)height * scal);
			}
			else
			{
				height = (int)((double)width / scal);
			}
			if (printTypeSettingModel == PrintTypeSettingModel.Multiple)
			{
				if (this.PageOrder == PageOrder.HorizontalReverse)
				{
					x2 = x2 + (int)pageWidth / this.PaperColumnNum * (this.PaperColumnNum - i % this.PaperColumnNum - 1) + ((int)pageWidth / this.PaperColumnNum - width) / 2;
					y2 = y2 + (int)pageHeight / this.PaperRowNum * (i / this.PaperColumnNum % this.PaperRowNum) + ((int)pageHeight / this.PaperRowNum - height) / 2;
					return;
				}
				if (this.PageOrder == PageOrder.Vertical)
				{
					x2 = x2 + (int)pageWidth / this.PaperColumnNum * (i / this.PaperRowNum % this.PaperColumnNum) + ((int)pageWidth / this.PaperColumnNum - width) / 2;
					y2 = y2 + (int)pageHeight / this.PaperRowNum * (i % this.PaperRowNum) + ((int)pageHeight / this.PaperRowNum - height) / 2;
					return;
				}
				if (this.PageOrder == PageOrder.VerticalReverse)
				{
					x2 = x2 + (int)pageWidth / this.PaperColumnNum * (this.PaperColumnNum - i / this.PaperRowNum % this.PaperColumnNum - 1) + ((int)pageWidth / this.PaperColumnNum - width) / 2;
					y2 = y2 + (int)pageHeight / this.PaperRowNum * (i % this.PaperRowNum) + ((int)pageHeight / this.PaperRowNum - height) / 2;
					return;
				}
				x2 = x2 + (int)pageWidth / this.PaperColumnNum * (i % this.PaperColumnNum) + ((int)pageWidth / this.PaperColumnNum - width) / 2;
				y2 = y2 + (int)pageHeight / this.PaperRowNum * (i / this.PaperColumnNum % this.PaperRowNum) + ((int)pageHeight / this.PaperRowNum - height) / 2;
				return;
			}
			else
			{
				if (printTypeSettingModel == PrintTypeSettingModel.Scale)
				{
					x2 = ((int)pageWidth - width) / 2 + x2;
					y2 = ((int)pageHeight - height) / 2 + y2;
					return;
				}
				return;
			}
		}

		// Token: 0x040004E5 RID: 1253
		private PdfDocument _pdfDoc;

		// Token: 0x040004E6 RID: 1254
		private readonly PrintPageIndexMapper _pageIndexMapper;

		// Token: 0x040004E7 RID: 1255
		private bool _isAnnotationVisible = true;

		// Token: 0x040004E8 RID: 1256
		private int _pageForPrint;

		// Token: 0x040004E9 RID: 1257
		private IntPtr _docForPrint;

		// Token: 0x040004EA RID: 1258
		private int _scale = 100;

		// Token: 0x040004EB RID: 1259
		private int TilePaperNumber;

		// Token: 0x04000503 RID: 1283
		public static bool IsCancel;

		// Token: 0x020004E0 RID: 1248
		private class PdfPageDrawingContext : IDisposable
		{
			// Token: 0x06002F05 RID: 12037 RVA: 0x000E6D86 File Offset: 0x000E4F86
			public PdfPageDrawingContext(PdfPrintDocument printDocument, Graphics graphics)
			{
				this.formBitmaps = new List<PdfPrintDocument.PdfPageDrawingContext.FormBitmap>();
				this.printDocument = printDocument;
				this.graphics = graphics;
				this.hdc = graphics.GetHdc();
			}

			// Token: 0x17000CC8 RID: 3272
			// (get) Token: 0x06002F06 RID: 12038 RVA: 0x000E6DB3 File Offset: 0x000E4FB3
			public IntPtr HDC
			{
				get
				{
					return this.hdc;
				}
			}

			// Token: 0x06002F07 RID: 12039 RVA: 0x000E6DBC File Offset: 0x000E4FBC
			public void RenderPage(IntPtr page, int start_x, int start_y, int size_x, int size_y, PageRotate rotate, RenderFlags flags)
			{
				PdfForms formFill = this.printDocument.Document.FormFill;
				IntPtr intPtr = ((formFill != null) ? formFill.Handle : IntPtr.Zero);
				this.GetAnnotationAndFormsProperties(page, ref intPtr, ref flags);
				Bitmap bitmap = this.CreateFormBitmap(intPtr, page, size_x, size_y, rotate, flags);
				if (bitmap != null)
				{
					this.formBitmaps.Add(new PdfPrintDocument.PdfPageDrawingContext.FormBitmap
					{
						Bitmap = bitmap,
						StartX = start_x,
						StartY = start_y,
						Rotate = rotate
					});
				}
				Pdfium.FPDF_RenderPage(this.hdc, page, start_x, start_y, size_x, size_y, rotate, flags);
			}

			// Token: 0x06002F08 RID: 12040 RVA: 0x000E6E4C File Offset: 0x000E504C
			public void Dispose()
			{
				if (this.hdc != IntPtr.Zero)
				{
					this.graphics.ReleaseHdc(this.hdc);
					this.hdc = IntPtr.Zero;
					GraphicsUnit pageUnit = this.graphics.PageUnit;
					try
					{
						this.graphics.PageUnit = GraphicsUnit.Pixel;
						for (int i = 0; i < this.formBitmaps.Count; i++)
						{
							try
							{
								this.graphics.DrawImageUnscaled(this.formBitmaps[i].Bitmap, this.formBitmaps[i].StartX, this.formBitmaps[i].StartY);
							}
							catch
							{
							}
						}
					}
					finally
					{
						this.graphics.PageUnit = pageUnit;
						for (int j = 0; j < this.formBitmaps.Count; j++)
						{
							try
							{
								this.formBitmaps[j].Bitmap.Dispose();
							}
							catch
							{
							}
						}
						this.formBitmaps = null;
					}
					if (this.formGraphics != null)
					{
						this.formGraphics.Dispose();
						PdfPrintDocument.PdfPageDrawingContext.DeleteDC(this.formHdc);
					}
				}
			}

			// Token: 0x06002F09 RID: 12041 RVA: 0x000E6F8C File Offset: 0x000E518C
			private void GetAnnotationAndFormsProperties(IntPtr page, ref IntPtr form_handle, ref RenderFlags renderFlags)
			{
				if (page == IntPtr.Zero)
				{
					form_handle = IntPtr.Zero;
				}
				if (form_handle != IntPtr.Zero && (renderFlags & RenderFlags.FPDF_ANNOT) != RenderFlags.FPDF_ANNOT)
				{
					form_handle = IntPtr.Zero;
				}
				if (form_handle != IntPtr.Zero)
				{
					int num = 0;
					if (Pdfium.FORM_GetInterForm(form_handle) != IntPtr.Zero)
					{
						num = Pdfium.FPDFInterForm_CountPageControls(this.printDocument.Document.FormFill.InterForm.Handle, page);
					}
					if (num == 0)
					{
						form_handle = IntPtr.Zero;
					}
				}
				if (form_handle != IntPtr.Zero)
				{
					renderFlags &= ~RenderFlags.FPDF_ANNOT;
				}
			}

			// Token: 0x06002F0A RID: 12042 RVA: 0x000E702C File Offset: 0x000E522C
			private Bitmap CreateFormBitmap(IntPtr form_handle, IntPtr page, int size_x, int size_y, PageRotate rotate, RenderFlags renderFlags)
			{
				if (form_handle == IntPtr.Zero)
				{
					return null;
				}
				if (page == IntPtr.Zero)
				{
					return null;
				}
				if (this.formHdc == IntPtr.Zero)
				{
					this.formHdc = PdfPrintDocument.PdfPageDrawingContext.CreateCompatibleDC(this.hdc);
					this.formGraphics = Graphics.FromHdc(this.formHdc);
				}
				Bitmap bitmap = null;
				BitmapData bitmapData = null;
				IntPtr intPtr = 0;
				Bitmap bitmap2;
				try
				{
					bitmap = new Bitmap(size_x, size_y, this.formGraphics);
					bitmapData = bitmap.LockBits(new Rectangle(0, 0, size_x, size_y), ImageLockMode.WriteOnly, bitmap.PixelFormat);
					BitmapFormats bitmapFormats;
					int[] array;
					PdfPrintDocument.PdfPageDrawingContext.GetPdfFormat(bitmap, out bitmapFormats, out array);
					intPtr = Pdfium.FPDFBitmap_CreateEx(size_x, size_y, bitmapFormats, bitmapData.Scan0, bitmapData.Stride);
					Pdfium.FFPDFBitmap_CopyPalette(intPtr, array);
					Pdfium.FPDF_FFLDraw(form_handle, intPtr, page, 0, 0, size_x, size_y, rotate, renderFlags);
					Pdfium.FPDFBitmap_Destroy(intPtr);
					intPtr = 0;
					bitmap.UnlockBits(bitmapData);
					bitmapData = null;
					bitmap2 = bitmap;
				}
				catch
				{
					bitmap2 = null;
				}
				finally
				{
					if (intPtr != IntPtr.Zero)
					{
						Pdfium.FPDFBitmap_Destroy(intPtr);
					}
					if (bitmapData != null)
					{
						bitmap.UnlockBits(bitmapData);
					}
				}
				return bitmap2;
			}

			// Token: 0x06002F0B RID: 12043 RVA: 0x000E7158 File Offset: 0x000E5358
			private static void GetPdfFormat(Bitmap image, out BitmapFormats pdfFormat, out int[] palette)
			{
				if (PdfPrintDocument.PdfPageDrawingContext._getPdfFormatFunction == null)
				{
					PdfPrintDocument.PdfPageDrawingContext._getPdfFormatFunction = (PdfPrintDocument.PdfPageDrawingContext.GetPdfFormatFunction)typeof(PdfBitmap).GetMethod("GetPdfFormat", BindingFlags.Static | BindingFlags.NonPublic).CreateDelegate(typeof(PdfPrintDocument.PdfPageDrawingContext.GetPdfFormatFunction));
				}
				PdfPrintDocument.PdfPageDrawingContext._getPdfFormatFunction(image, out pdfFormat, out palette);
			}

			// Token: 0x06002F0C RID: 12044
			[DllImport("gdi32.dll")]
			private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

			// Token: 0x06002F0D RID: 12045
			[DllImport("gdi32.dll")]
			private static extern int DeleteDC(IntPtr hdc);

			// Token: 0x04001B22 RID: 6946
			private PdfPrintDocument printDocument;

			// Token: 0x04001B23 RID: 6947
			private Graphics graphics;

			// Token: 0x04001B24 RID: 6948
			private IntPtr hdc;

			// Token: 0x04001B25 RID: 6949
			private List<PdfPrintDocument.PdfPageDrawingContext.FormBitmap> formBitmaps;

			// Token: 0x04001B26 RID: 6950
			private IntPtr formHdc;

			// Token: 0x04001B27 RID: 6951
			private Graphics formGraphics;

			// Token: 0x04001B28 RID: 6952
			private static PdfPrintDocument.PdfPageDrawingContext.GetPdfFormatFunction _getPdfFormatFunction;

			// Token: 0x020007FF RID: 2047
			private class FormBitmap
			{
				// Token: 0x17000DAE RID: 3502
				// (get) Token: 0x06003841 RID: 14401 RVA: 0x00126267 File Offset: 0x00124467
				// (set) Token: 0x06003842 RID: 14402 RVA: 0x0012626F File Offset: 0x0012446F
				public Bitmap Bitmap { get; set; }

				// Token: 0x17000DAF RID: 3503
				// (get) Token: 0x06003843 RID: 14403 RVA: 0x00126278 File Offset: 0x00124478
				// (set) Token: 0x06003844 RID: 14404 RVA: 0x00126280 File Offset: 0x00124480
				public int StartX { get; set; }

				// Token: 0x17000DB0 RID: 3504
				// (get) Token: 0x06003845 RID: 14405 RVA: 0x00126289 File Offset: 0x00124489
				// (set) Token: 0x06003846 RID: 14406 RVA: 0x00126291 File Offset: 0x00124491
				public int StartY { get; set; }

				// Token: 0x17000DB1 RID: 3505
				// (get) Token: 0x06003847 RID: 14407 RVA: 0x0012629A File Offset: 0x0012449A
				// (set) Token: 0x06003848 RID: 14408 RVA: 0x001262A2 File Offset: 0x001244A2
				public PageRotate Rotate { get; set; }
			}

			// Token: 0x02000800 RID: 2048
			// (Invoke) Token: 0x0600384B RID: 14411
			private delegate void GetPdfFormatFunction(Bitmap image, out BitmapFormats pdfFormat, out int[] palette);
		}
	}
}
