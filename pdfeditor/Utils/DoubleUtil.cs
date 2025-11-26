using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace pdfeditor.Utils
{
	// Token: 0x02000097 RID: 151
	public static class DoubleUtil
	{
		// Token: 0x060009D6 RID: 2518 RVA: 0x000322C0 File Offset: 0x000304C0
		public static bool AreClose(double value1, double value2)
		{
			if (value1 == value2)
			{
				return true;
			}
			double num = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * 2.2204460492503131E-16;
			double num2 = value1 - value2;
			return -num < num2 && num > num2;
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x00032304 File Offset: 0x00030504
		public static bool LessThan(double value1, double value2)
		{
			return value1 < value2 && !DoubleUtil.AreClose(value1, value2);
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x00032316 File Offset: 0x00030516
		public static bool GreaterThan(double value1, double value2)
		{
			return value1 > value2 && !DoubleUtil.AreClose(value1, value2);
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x00032328 File Offset: 0x00030528
		public static bool LessThanOrClose(double value1, double value2)
		{
			return value1 < value2 || DoubleUtil.AreClose(value1, value2);
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x00032337 File Offset: 0x00030537
		public static bool GreaterThanOrClose(double value1, double value2)
		{
			return value1 > value2 || DoubleUtil.AreClose(value1, value2);
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x00032346 File Offset: 0x00030546
		public static bool IsOne(double value)
		{
			return Math.Abs(value - 1.0) < 2.2204460492503131E-15;
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x00032363 File Offset: 0x00030563
		public static bool IsZero(double value)
		{
			return Math.Abs(value) < 2.2204460492503131E-15;
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x00032376 File Offset: 0x00030576
		public static bool AreClose(Vector vector1, Vector vector2)
		{
			return DoubleUtil.AreClose(vector1.X, vector2.X) && DoubleUtil.AreClose(vector1.Y, vector2.Y);
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x000323A2 File Offset: 0x000305A2
		public static bool IsBetweenZeroAndOne(double val)
		{
			return DoubleUtil.GreaterThanOrClose(val, 0.0) && DoubleUtil.LessThanOrClose(val, 1.0);
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x000323C6 File Offset: 0x000305C6
		public static int DoubleToInt(double val)
		{
			if (0.0 >= val)
			{
				return (int)(val - 0.5);
			}
			return (int)(val + 0.5);
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x000323F0 File Offset: 0x000305F0
		public static bool IsNaN(double value)
		{
			DoubleUtil.NanUnion nanUnion = new DoubleUtil.NanUnion
			{
				DoubleValue = value
			};
			ulong num = nanUnion.UintValue & 18442240474082181120UL;
			ulong num2 = nanUnion.UintValue & 4503599627370495UL;
			return (num == 9218868437227405312UL || num == 18442240474082181120UL) && num2 > 0UL;
		}

		// Token: 0x04000482 RID: 1154
		internal const double DBL_EPSILON = 2.2204460492503131E-16;

		// Token: 0x04000483 RID: 1155
		internal const float FLT_MIN = 1.17549435E-38f;

		// Token: 0x0200047F RID: 1151
		[StructLayout(LayoutKind.Explicit)]
		private struct NanUnion
		{
			// Token: 0x0400199F RID: 6559
			[FieldOffset(0)]
			internal double DoubleValue;

			// Token: 0x040019A0 RID: 6560
			[FieldOffset(0)]
			internal ulong UintValue;
		}
	}
}
