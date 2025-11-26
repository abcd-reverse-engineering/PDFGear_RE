using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Patagames.Pdf;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;

namespace pdfeditor.Controls.Annotations.Holders
{
	// Token: 0x020002B1 RID: 689
	public interface IAnnotationHolder
	{
		// Token: 0x17000C3D RID: 3133
		// (get) Token: 0x060027D3 RID: 10195
		bool IsTextMarkupAnnotation { get; }

		// Token: 0x17000C3E RID: 3134
		// (get) Token: 0x060027D4 RID: 10196
		AnnotationCanvas AnnotationCanvas { get; }

		// Token: 0x17000C3F RID: 3135
		// (get) Token: 0x060027D5 RID: 10197
		AnnotationHolderState State { get; }

		// Token: 0x17000C40 RID: 3136
		// (get) Token: 0x060027D6 RID: 10198
		PdfPage CurrentPage { get; }

		// Token: 0x17000C41 RID: 3137
		// (get) Token: 0x060027D7 RID: 10199
		PdfAnnotation SelectedAnnotation { get; }

		// Token: 0x14000047 RID: 71
		// (add) Token: 0x060027D8 RID: 10200
		// (remove) Token: 0x060027D9 RID: 10201
		event EventHandler Canceled;

		// Token: 0x14000048 RID: 72
		// (add) Token: 0x060027DA RID: 10202
		// (remove) Token: 0x060027DB RID: 10203
		event EventHandler StateChanged;

		// Token: 0x14000049 RID: 73
		// (add) Token: 0x060027DC RID: 10204
		// (remove) Token: 0x060027DD RID: 10205
		event EventHandler SelectedAnnotationChanged;

		// Token: 0x060027DE RID: 10206
		void Cancel();

		// Token: 0x060027DF RID: 10207
		Task<global::System.Collections.Generic.IReadOnlyList<PdfAnnotation>> CompleteCreateNewAsync();

		// Token: 0x060027E0 RID: 10208
		void OnPageClientBoundsChanged();

		// Token: 0x060027E1 RID: 10209
		void ProcessCreateNew(PdfPage page, FS_POINTF pagePoint);

		// Token: 0x060027E2 RID: 10210
		void StartCreateNew(PdfPage page, FS_POINTF pagePoint);

		// Token: 0x060027E3 RID: 10211
		void Select(PdfAnnotation annotation, bool afterCreate);

		// Token: 0x060027E4 RID: 10212
		bool OnPropertyChanged(string propertyName);
	}
}
