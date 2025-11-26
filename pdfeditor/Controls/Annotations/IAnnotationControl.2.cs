using System;
using Patagames.Pdf.Net.Annotations;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002A4 RID: 676
	public interface IAnnotationControl<T> : IAnnotationControl where T : PdfAnnotation
	{
		// Token: 0x17000BE9 RID: 3049
		// (get) Token: 0x060026E5 RID: 9957
		T Annotation { get; }
	}
}
