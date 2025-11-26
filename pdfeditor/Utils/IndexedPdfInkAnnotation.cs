using System;
using Patagames.Pdf.Net.Annotations;

namespace pdfeditor.Utils
{
	// Token: 0x02000090 RID: 144
	public class IndexedPdfInkAnnotation
	{
		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06000986 RID: 2438 RVA: 0x00030955 File Offset: 0x0002EB55
		// (set) Token: 0x06000987 RID: 2439 RVA: 0x0003095D File Offset: 0x0002EB5D
		public PdfInkAnnotation PdfInkAnnotation { get; set; }

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06000988 RID: 2440 RVA: 0x00030966 File Offset: 0x0002EB66
		// (set) Token: 0x06000989 RID: 2441 RVA: 0x0003096E File Offset: 0x0002EB6E
		public int Index { get; set; }
	}
}
