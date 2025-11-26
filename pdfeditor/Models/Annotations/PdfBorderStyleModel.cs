using System;
using System.Collections.Generic;
using System.Linq;
using Patagames.Pdf.Enums;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001A2 RID: 418
	public class PdfBorderStyleModel : IEquatable<PdfBorderStyleModel>
	{
		// Token: 0x060017E2 RID: 6114 RVA: 0x0005B634 File Offset: 0x00059834
		public PdfBorderStyleModel(float width, BorderStyles style, float[] dashPattern)
		{
			this.Width = width;
			this.Style = style;
			float[] array;
			if ((array = ((dashPattern != null) ? dashPattern.ToArray<float>() : null)) == null)
			{
				float[] array2 = new float[2];
				array2[0] = 2f;
				array = array2;
				array2[1] = 4f;
			}
			this.DashPattern = array;
		}

		// Token: 0x17000970 RID: 2416
		// (get) Token: 0x060017E3 RID: 6115 RVA: 0x0005B681 File Offset: 0x00059881
		public float Width { get; }

		// Token: 0x17000971 RID: 2417
		// (get) Token: 0x060017E4 RID: 6116 RVA: 0x0005B689 File Offset: 0x00059889
		public BorderStyles Style { get; }

		// Token: 0x17000972 RID: 2418
		// (get) Token: 0x060017E5 RID: 6117 RVA: 0x0005B691 File Offset: 0x00059891
		public IReadOnlyList<float> DashPattern { get; }

		// Token: 0x060017E6 RID: 6118 RVA: 0x0005B699 File Offset: 0x00059899
		public bool Equals(PdfBorderStyleModel other)
		{
			return other != null && this.Width == other.Width && this.Style == other.Style && BaseAnnotation.CollectionEqual<float>(this.DashPattern, this.DashPattern);
		}
	}
}
