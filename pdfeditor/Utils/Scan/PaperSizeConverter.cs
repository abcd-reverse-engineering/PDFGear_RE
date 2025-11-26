using System;
using pdfeditor.Models.Scan;

namespace pdfeditor.Utils.Scan
{
	// Token: 0x020000B7 RID: 183
	public class PaperSizeConverter
	{
		// Token: 0x06000B0C RID: 2828 RVA: 0x000390B8 File Offset: 0x000372B8
		public static double Convert(double value, PaperSizeUnit fromUnit, PaperSizeUnit toUnit, double resolution = 96.0)
		{
			double num;
			switch (fromUnit)
			{
			case PaperSizeUnit.Pixel:
				num = value / resolution;
				break;
			case PaperSizeUnit.Inch:
				num = value;
				break;
			case PaperSizeUnit.Millimeter:
				num = value / 25.4;
				break;
			default:
				return value;
			}
			switch (toUnit)
			{
			case PaperSizeUnit.Pixel:
				return num * resolution;
			case PaperSizeUnit.Inch:
				return num;
			case PaperSizeUnit.Millimeter:
				return num * 25.4;
			default:
				return value;
			}
		}

		// Token: 0x040004C9 RID: 1225
		private const double Inch2Millimeter = 25.4;
	}
}
