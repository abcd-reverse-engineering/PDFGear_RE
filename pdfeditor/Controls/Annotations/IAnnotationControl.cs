using System;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Controls.Annotations.Holders;

namespace pdfeditor.Controls.Annotations
{
	// Token: 0x020002A3 RID: 675
	public interface IAnnotationControl
	{
		// Token: 0x17000BE6 RID: 3046
		// (get) Token: 0x060026E0 RID: 9952
		IAnnotationHolder Holder { get; }

		// Token: 0x17000BE7 RID: 3047
		// (get) Token: 0x060026E1 RID: 9953
		AnnotationCanvas ParentCanvas { get; }

		// Token: 0x17000BE8 RID: 3048
		// (get) Token: 0x060026E2 RID: 9954
		PdfAnnotation Annotation { get; }

		// Token: 0x060026E3 RID: 9955
		void OnPageClientBoundsChanged();

		// Token: 0x060026E4 RID: 9956
		bool OnPropertyChanged(string propertyName);
	}
}
