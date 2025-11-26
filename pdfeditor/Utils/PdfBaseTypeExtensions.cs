using System;
using System.Windows;
using System.Windows.Media;
using Patagames.Pdf;

namespace pdfeditor.Utils
{
	// Token: 0x02000098 RID: 152
	public static class PdfBaseTypeExtensions
	{
		// Token: 0x060009E1 RID: 2529 RVA: 0x0003244E File Offset: 0x0003064E
		public static bool IntersectsWith(this FS_RECTF rect, FS_RECTF rect2)
		{
			return rect.left <= rect2.right && rect.right >= rect2.left && rect.top >= rect2.bottom && rect.bottom <= rect2.top;
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x0003248D File Offset: 0x0003068D
		public static Point ToPoint(this FS_POINTF point)
		{
			return new Point((double)point.X, (double)point.Y);
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x000324A4 File Offset: 0x000306A4
		public static FS_POINTF ToPdfPoint(this Point point)
		{
			return new FS_POINTF(point.X, point.Y);
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x000324B9 File Offset: 0x000306B9
		public static bool Equals(this FS_POINTF point, Point point2)
		{
			return DoubleUtil.AreClose((double)point.X, point2.X) && DoubleUtil.AreClose((double)point.Y, point2.Y);
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x000324E7 File Offset: 0x000306E7
		public static bool Equals(this Point point, FS_POINTF point2)
		{
			return point2.Equals(point);
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x000324FC File Offset: 0x000306FC
		public static Color ToColor(this FS_COLOR color)
		{
			return Color.FromArgb((byte)color.A, (byte)color.R, (byte)color.G, (byte)color.B);
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x00032523 File Offset: 0x00030723
		public static FS_COLOR ToPdfColor(this Color color)
		{
			return new FS_COLOR((int)color.A, (int)color.R, (int)color.G, (int)color.B);
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x00032548 File Offset: 0x00030748
		public static bool Equals(this FS_COLOR color, Color color2)
		{
			return color.A == (int)color2.A && color.R == (int)color2.R && color.G == (int)color2.G && color.B == (int)color2.B;
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x00032597 File Offset: 0x00030797
		public static bool Equals(this Color color, FS_COLOR color2)
		{
			return color2.Equals(color);
		}
	}
}
