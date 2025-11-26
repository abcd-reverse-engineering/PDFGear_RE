using System;
using System.Collections.Generic;
using System.Linq;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using PDFKit;

namespace pdfeditor.Utils
{
	// Token: 0x02000093 RID: 147
	public static class PdfTextMarkupAnnotationUtils
	{
		// Token: 0x060009AD RID: 2477 RVA: 0x000316A1 File Offset: 0x0002F8A1
		public static IEnumerable<TextInfo> GetTextInfos(PdfDocument document, SelectInfo selectInfo, bool includeSpaceChar = false)
		{
			PdfTextMarkupAnnotationUtils.<GetTextInfos>d__0 <GetTextInfos>d__ = new PdfTextMarkupAnnotationUtils.<GetTextInfos>d__0(-2);
			<GetTextInfos>d__.<>3__document = document;
			<GetTextInfos>d__.<>3__selectInfo = selectInfo;
			<GetTextInfos>d__.<>3__includeSpaceChar = includeSpaceChar;
			return <GetTextInfos>d__;
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x000316C0 File Offset: 0x0002F8C0
		private static global::System.Collections.Generic.IReadOnlyList<FS_RECTF> GetRectsFromTextInfoWithoutSpaceCharacter(PdfPage page, int s, int len)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}
			int pageIndex = page.PageIndex;
			List<IEnumerable<FS_RECTF>> list = new List<IEnumerable<FS_RECTF>>();
			int num = -1;
			int num2 = 0;
			for (int i = s; i < s + len; i++)
			{
				if (page.Text.GetCharacter(i) == ' ')
				{
					if (num >= 0)
					{
						list.Add(page.Text.GetTextInfo(num, num2).Rects);
						num = -1;
						num2 = 0;
					}
				}
				else if (num == -1)
				{
					num = i;
					num2 = 1;
				}
				else
				{
					num2++;
				}
			}
			if (num >= 0)
			{
				list.Add(page.Text.GetTextInfo(num, num2).Rects);
			}
			List<FS_RECTF> list2 = new List<FS_RECTF>();
			foreach (IEnumerable<FS_RECTF> enumerable in list)
			{
				list2.AddRange(enumerable);
			}
			return list2;
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x000317A8 File Offset: 0x0002F9A8
		public static global::System.Collections.Generic.IReadOnlyList<FS_RECTF> GetNormalizedRects(PdfViewer viewer, TextInfo textInfo, bool normalizeRects = false, bool removeOverlap = false)
		{
			if (viewer == null)
			{
				throw new ArgumentNullException("viewer");
			}
			if (textInfo == null)
			{
				throw new ArgumentNullException("textInfo");
			}
			PdfDocument document = viewer.Document;
			if (document == null)
			{
				throw new ArgumentNullException("viewer.Document");
			}
			PdfPage pdfPage = document.Pages[textInfo.PageIndex];
			return PdfTextMarkupAnnotationUtils.GetNormalizedRow(textInfo.Rects, (double)(pdfPage.Width / 40f), normalizeRects, removeOverlap).SelectMany((PdfTextMarkupAnnotationUtils.NormalizedRow c) => c.Rects, (PdfTextMarkupAnnotationUtils.NormalizedRow s, FS_RECTF c) => c).ToArray<FS_RECTF>();
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x0003185C File Offset: 0x0002FA5C
		private static global::System.Collections.Generic.IReadOnlyList<PdfTextMarkupAnnotationUtils.NormalizedRow> GetNormalizedRow(global::System.Collections.Generic.IReadOnlyList<FS_RECTF> rects, double minSpaceWidth, bool normalizeRects, bool removeOverlap)
		{
			if (rects == null)
			{
				return Array.Empty<PdfTextMarkupAnnotationUtils.NormalizedRow>();
			}
			rects = rects.Where((FS_RECTF c) => c.Width > 0f && c.Height > 0f).ToArray<FS_RECTF>();
			if (rects.Count == 1)
			{
				PdfTextMarkupAnnotationUtils.NormalizedRow normalizedRow = new PdfTextMarkupAnnotationUtils.NormalizedRow(rects[0], minSpaceWidth);
				normalizedRow.Complete(normalizeRects);
				return new PdfTextMarkupAnnotationUtils.NormalizedRow[] { normalizedRow };
			}
			List<PdfTextMarkupAnnotationUtils.NormalizedRow> list = new List<PdfTextMarkupAnnotationUtils.NormalizedRow>();
			PdfTextMarkupAnnotationUtils.NormalizedRow normalizedRow2 = new PdfTextMarkupAnnotationUtils.NormalizedRow(rects[0], minSpaceWidth);
			list.Add(normalizedRow2);
			for (int i = 1; i < rects.Count; i++)
			{
				FS_RECTF fs_RECTF = rects[i];
				FS_RECTF fs_RECTF2 = rects[i - 1];
				bool flag;
				if (fs_RECTF2.right <= fs_RECTF.right)
				{
					if (normalizedRow2.RawRects.Count == 1)
					{
						if ((double)(fs_RECTF.left - fs_RECTF2.right) > minSpaceWidth)
						{
							float height = normalizedRow2.Height;
							flag = fs_RECTF.top < normalizedRow2.Bottom + height / 2f || fs_RECTF.bottom > normalizedRow2.Top - height / 2f;
						}
						else
						{
							Math.Max(normalizedRow2.Height, fs_RECTF.Height);
							if ((double)fs_RECTF.Height < 1.6)
							{
								flag = fs_RECTF.top <= normalizedRow2.Bottom - 2f || fs_RECTF.bottom >= normalizedRow2.Top + 2f;
							}
							else
							{
								flag = fs_RECTF.top <= normalizedRow2.Bottom || fs_RECTF.bottom >= normalizedRow2.Top;
							}
						}
					}
					else if (fs_RECTF.Height < normalizedRow2.Height)
					{
						float height2 = fs_RECTF.Height;
						flag = fs_RECTF.top <= normalizedRow2.Bottom || fs_RECTF.bottom >= normalizedRow2.Top;
					}
					else
					{
						float height3 = normalizedRow2.Height;
						flag = fs_RECTF.top < normalizedRow2.Bottom + height3 / 2f || fs_RECTF.bottom > normalizedRow2.Top - height3 / 2f;
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					if (!removeOverlap)
					{
						normalizedRow2.Complete(normalizeRects);
					}
					normalizedRow2 = new PdfTextMarkupAnnotationUtils.NormalizedRow(fs_RECTF, minSpaceWidth);
					list.Add(normalizedRow2);
				}
				else
				{
					normalizedRow2.Add(fs_RECTF);
				}
			}
			if (removeOverlap)
			{
				for (int j = 1; j < list.Count; j++)
				{
					PdfTextMarkupAnnotationUtils.NormalizedRow normalizedRow3 = list[j];
					PdfTextMarkupAnnotationUtils.NormalizedRow normalizedRow4 = list[j - 1];
					if (normalizedRow3.Top > normalizedRow4.Bottom && normalizedRow3.Bottom < normalizedRow4.Bottom && Math.Min(normalizedRow3.Top, normalizedRow4.Top) - Math.Max(normalizedRow3.Bottom, normalizedRow4.Bottom) < Math.Min(normalizedRow3.Height, normalizedRow4.Height) / 2f)
					{
						normalizedRow3.Top = Math.Max(normalizedRow3.Bottom, normalizedRow4.Bottom);
					}
					normalizedRow3.Complete(normalizeRects);
				}
			}
			return list;
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x00031B78 File Offset: 0x0002FD78
		public static FS_RECTF ToPdfRect(this FS_QUADPOINTSF quadPoints)
		{
			float num = Math.Min(Math.Min(quadPoints.x1, quadPoints.x2), Math.Min(quadPoints.x3, quadPoints.x4));
			float num2 = Math.Max(Math.Max(quadPoints.x1, quadPoints.x2), Math.Max(quadPoints.x3, quadPoints.x4));
			float num3 = Math.Max(Math.Max(quadPoints.y1, quadPoints.y2), Math.Max(quadPoints.y3, quadPoints.y4));
			float num4 = Math.Min(Math.Min(quadPoints.y1, quadPoints.y2), Math.Min(quadPoints.y3, quadPoints.y4));
			return new FS_RECTF(num, num3, num2, num4);
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x00031C2C File Offset: 0x0002FE2C
		public static FS_QUADPOINTSF ToQuadPoints(this FS_RECTF rect)
		{
			return rect.ToQuadPoints(0f, 0f, 0f, 0f);
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x00031C48 File Offset: 0x0002FE48
		public static FS_QUADPOINTSF ToQuadPoints(this FS_RECTF rect, float padding)
		{
			return rect.ToQuadPoints(padding, padding, padding, padding);
		}

		// Token: 0x060009B4 RID: 2484 RVA: 0x00031C54 File Offset: 0x0002FE54
		public static FS_QUADPOINTSF ToQuadPoints(this FS_RECTF rect, float leftRightPadding = 0f, float topBottomPadding = 0f)
		{
			return rect.ToQuadPoints(leftRightPadding, topBottomPadding, leftRightPadding, topBottomPadding);
		}

		// Token: 0x060009B5 RID: 2485 RVA: 0x00031C60 File Offset: 0x0002FE60
		public static FS_QUADPOINTSF ToQuadPoints(this FS_RECTF rect, float leftPadding = 0f, float topPadding = 0f, float rightPadding = 0f, float bottomPadding = 0f)
		{
			float num = rect.left - leftPadding;
			float num2 = rect.top + topPadding;
			float num3 = rect.right + rightPadding;
			float num4 = rect.top + topPadding;
			float num5 = rect.left - leftPadding;
			float num6 = rect.bottom - bottomPadding;
			float num7 = rect.right + rightPadding;
			float num8 = rect.bottom - bottomPadding;
			return new FS_QUADPOINTSF(num, num2, num3, num4, num5, num6, num7, num8);
		}

		// Token: 0x02000478 RID: 1144
		private class NormalizedRow
		{
			// Token: 0x06002DBD RID: 11709 RVA: 0x000DFEAC File Offset: 0x000DE0AC
			public NormalizedRow(FS_RECTF rc, double minSpaceWidth)
			{
				this.minSpaceWidth = minSpaceWidth;
				this.Add(rc);
			}

			// Token: 0x17000CA2 RID: 3234
			// (get) Token: 0x06002DBE RID: 11710 RVA: 0x000DFEF9 File Offset: 0x000DE0F9
			public global::System.Collections.Generic.IReadOnlyList<FS_RECTF> RawRects
			{
				get
				{
					return this.rawRects;
				}
			}

			// Token: 0x17000CA3 RID: 3235
			// (get) Token: 0x06002DBF RID: 11711 RVA: 0x000DFF01 File Offset: 0x000DE101
			public global::System.Collections.Generic.IReadOnlyList<FS_RECTF> Rects
			{
				get
				{
					return this.rects;
				}
			}

			// Token: 0x17000CA4 RID: 3236
			// (get) Token: 0x06002DC0 RID: 11712 RVA: 0x000DFF09 File Offset: 0x000DE109
			// (set) Token: 0x06002DC1 RID: 11713 RVA: 0x000DFF11 File Offset: 0x000DE111
			public float Top { get; set; } = float.MinValue;

			// Token: 0x17000CA5 RID: 3237
			// (get) Token: 0x06002DC2 RID: 11714 RVA: 0x000DFF1A File Offset: 0x000DE11A
			// (set) Token: 0x06002DC3 RID: 11715 RVA: 0x000DFF22 File Offset: 0x000DE122
			public float Bottom { get; set; } = float.MaxValue;

			// Token: 0x17000CA6 RID: 3238
			// (get) Token: 0x06002DC4 RID: 11716 RVA: 0x000DFF2B File Offset: 0x000DE12B
			public float Height
			{
				get
				{
					return this.Top - this.Bottom;
				}
			}

			// Token: 0x17000CA7 RID: 3239
			// (get) Token: 0x06002DC5 RID: 11717 RVA: 0x000DFF3A File Offset: 0x000DE13A
			public bool Completed
			{
				get
				{
					return this.completed;
				}
			}

			// Token: 0x06002DC6 RID: 11718 RVA: 0x000DFF44 File Offset: 0x000DE144
			public void Add(FS_RECTF rect)
			{
				if (this.completed)
				{
					throw new ArgumentException("Completed");
				}
				this.rawRects.Add(rect);
				if (this.rects.Count == 0)
				{
					this.rects.Add(rect);
				}
				else
				{
					FS_RECTF fs_RECTF = this.rects.Last<FS_RECTF>();
					if ((double)(rect.left - fs_RECTF.right) < this.minSpaceWidth)
					{
						FS_RECTF fs_RECTF2 = this.rects.Last<FS_RECTF>();
						float num = Math.Min(rect.left, fs_RECTF2.left);
						float num2 = Math.Max(rect.right, fs_RECTF2.right);
						float num3 = Math.Max(rect.top, fs_RECTF2.top);
						float num4 = Math.Min(rect.bottom, fs_RECTF2.bottom);
						FS_RECTF fs_RECTF3 = new FS_RECTF(num, num3, num2, num4);
						this.rects[this.rects.Count - 1] = fs_RECTF3;
					}
					else
					{
						this.rects.Add(rect);
					}
				}
				this.Top = Math.Max(rect.top, this.Top);
				this.Bottom = Math.Min(rect.bottom, this.Bottom);
			}

			// Token: 0x06002DC7 RID: 11719 RVA: 0x000E006C File Offset: 0x000DE26C
			public void Complete(bool normalizeRects)
			{
				this.completed = true;
				if (normalizeRects)
				{
					for (int i = 0; i < this.rects.Count; i++)
					{
						FS_RECTF fs_RECTF = this.rects[i];
						this.rects[i] = new FS_RECTF(fs_RECTF.left, this.Top, fs_RECTF.right, this.Bottom);
					}
				}
			}

			// Token: 0x0400197C RID: 6524
			private readonly double minSpaceWidth;

			// Token: 0x0400197D RID: 6525
			private List<FS_RECTF> rawRects = new List<FS_RECTF>();

			// Token: 0x0400197E RID: 6526
			private List<FS_RECTF> rects = new List<FS_RECTF>();

			// Token: 0x0400197F RID: 6527
			private bool completed;
		}
	}
}
