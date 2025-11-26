using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Models.Annotations;
using pdfeditor.Models.Operations;
using pdfeditor.ViewModels;
using PDFKit;

namespace pdfeditor.Utils
{
	// Token: 0x02000087 RID: 135
	public static class AnnotationOperationManagerExtensions
	{
		// Token: 0x06000912 RID: 2322 RVA: 0x0002D40B File Offset: 0x0002B60B
		public static AnnotationChangedTraceContext TraceAnnotationChange(this OperationManager manager, PdfDocument pdfDocument, string tag = "")
		{
			if (manager == null)
			{
				throw new ArgumentNullException("manager");
			}
			return new AnnotationChangedTraceContext(manager, pdfDocument, tag);
		}

		// Token: 0x06000913 RID: 2323 RVA: 0x0002D424 File Offset: 0x0002B624
		public static IDisposable TraceAnnotationChange(this OperationManager manager, PdfPage page, string tag = "")
		{
			if (manager == null)
			{
				throw new ArgumentNullException("manager");
			}
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}
			if (page.Document == null)
			{
				throw new ArgumentNullException("document");
			}
			AnnotationChangedTraceContext annotationChangedTraceContext = new AnnotationChangedTraceContext(manager, page.Document, tag);
			annotationChangedTraceContext.TryAddPage(page.PageIndex);
			return annotationChangedTraceContext;
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x0002D47C File Offset: 0x0002B67C
		public static async Task TraceAnnotationInsertAsync(this OperationManager manager, PdfAnnotation annotation, string tag = "")
		{
			await manager.TraceAnnotationInsertAsync(new PdfAnnotation[] { annotation }, tag).ConfigureAwait(false);
		}

		// Token: 0x06000915 RID: 2325 RVA: 0x0002D4D0 File Offset: 0x0002B6D0
		public static async Task TraceAnnotationInsertAsync(this OperationManager manager, global::System.Collections.Generic.IReadOnlyList<PdfAnnotation> annotations, string tag = "")
		{
			AnnotationOperationManagerExtensions.<>c__DisplayClass3_0 CS$<>8__locals1 = new AnnotationOperationManagerExtensions.<>c__DisplayClass3_0();
			if (annotations != null && annotations.Count != 0)
			{
				if (!annotations.Any(delegate(PdfAnnotation c)
				{
					object obj;
					if (c == null)
					{
						obj = null;
					}
					else
					{
						PdfPage page = c.Page;
						obj = ((page != null) ? page.Document : null);
					}
					return obj == null;
				}))
				{
					CS$<>8__locals1.records = (from c in annotations
						select AnnotationModelUtils.CreateFlattenedRecord(c) into c
						orderby c.Item1.AnnotIndex
						select c).ToArray<global::System.ValueTuple<BaseAnnotation, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>>>();
					await manager.AddOperationAsync(delegate(PdfDocument doc)
					{
						AnnotationOperationManagerExtensions.<>c__DisplayClass3_0.<<TraceAnnotationInsertAsync>b__3>d <<TraceAnnotationInsertAsync>b__3>d;
						<<TraceAnnotationInsertAsync>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<TraceAnnotationInsertAsync>b__3>d.<>4__this = CS$<>8__locals1;
						<<TraceAnnotationInsertAsync>b__3>d.doc = doc;
						<<TraceAnnotationInsertAsync>b__3>d.<>1__state = -1;
						<<TraceAnnotationInsertAsync>b__3>d.<>t__builder.Start<AnnotationOperationManagerExtensions.<>c__DisplayClass3_0.<<TraceAnnotationInsertAsync>b__3>d>(ref <<TraceAnnotationInsertAsync>b__3>d);
						return <<TraceAnnotationInsertAsync>b__3>d.<>t__builder.Task;
					}, delegate(PdfDocument doc)
					{
						AnnotationOperationManagerExtensions.<>c__DisplayClass3_0.<<TraceAnnotationInsertAsync>b__4>d <<TraceAnnotationInsertAsync>b__4>d;
						<<TraceAnnotationInsertAsync>b__4>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<TraceAnnotationInsertAsync>b__4>d.<>4__this = CS$<>8__locals1;
						<<TraceAnnotationInsertAsync>b__4>d.doc = doc;
						<<TraceAnnotationInsertAsync>b__4>d.<>1__state = -1;
						<<TraceAnnotationInsertAsync>b__4>d.<>t__builder.Start<AnnotationOperationManagerExtensions.<>c__DisplayClass3_0.<<TraceAnnotationInsertAsync>b__4>d>(ref <<TraceAnnotationInsertAsync>b__4>d);
						return <<TraceAnnotationInsertAsync>b__4>d.<>t__builder.Task;
					}, tag).ConfigureAwait(false);
					return;
				}
			}
			throw new ArgumentException("annotations");
		}

		// Token: 0x06000916 RID: 2326 RVA: 0x0002D524 File Offset: 0x0002B724
		public static async Task TraceAnnotationRemoveAsync(this OperationManager manager, PdfAnnotation annotation, string tag = "")
		{
			await manager.TraceAnnotationRemoveAsync(new PdfAnnotation[] { annotation }, tag).ConfigureAwait(false);
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x0002D578 File Offset: 0x0002B778
		public static async Task TraceAnnotationRemoveAsync(this OperationManager manager, global::System.Collections.Generic.IReadOnlyList<PdfAnnotation> annotations, string tag = "")
		{
			AnnotationOperationManagerExtensions.<>c__DisplayClass5_0 CS$<>8__locals1 = new AnnotationOperationManagerExtensions.<>c__DisplayClass5_0();
			if (annotations != null && annotations.Count != 0)
			{
				if (!annotations.Any(delegate(PdfAnnotation c)
				{
					object obj;
					if (c == null)
					{
						obj = null;
					}
					else
					{
						PdfPage page = c.Page;
						obj = ((page != null) ? page.Document : null);
					}
					return obj == null;
				}))
				{
					CS$<>8__locals1.records = (from c in annotations
						select AnnotationModelUtils.CreateFlattenedRecord(c) into c
						orderby c.Item1.AnnotIndex
						select c).ToArray<global::System.ValueTuple<BaseAnnotation, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>>>();
					await manager.AddOperationAsync(delegate(PdfDocument doc)
					{
						AnnotationOperationManagerExtensions.<>c__DisplayClass5_0.<<TraceAnnotationRemoveAsync>b__3>d <<TraceAnnotationRemoveAsync>b__3>d;
						<<TraceAnnotationRemoveAsync>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<TraceAnnotationRemoveAsync>b__3>d.<>4__this = CS$<>8__locals1;
						<<TraceAnnotationRemoveAsync>b__3>d.doc = doc;
						<<TraceAnnotationRemoveAsync>b__3>d.<>1__state = -1;
						<<TraceAnnotationRemoveAsync>b__3>d.<>t__builder.Start<AnnotationOperationManagerExtensions.<>c__DisplayClass5_0.<<TraceAnnotationRemoveAsync>b__3>d>(ref <<TraceAnnotationRemoveAsync>b__3>d);
						return <<TraceAnnotationRemoveAsync>b__3>d.<>t__builder.Task;
					}, delegate(PdfDocument doc)
					{
						AnnotationOperationManagerExtensions.<>c__DisplayClass5_0.<<TraceAnnotationRemoveAsync>b__4>d <<TraceAnnotationRemoveAsync>b__4>d;
						<<TraceAnnotationRemoveAsync>b__4>d.<>t__builder = AsyncTaskMethodBuilder.Create();
						<<TraceAnnotationRemoveAsync>b__4>d.<>4__this = CS$<>8__locals1;
						<<TraceAnnotationRemoveAsync>b__4>d.doc = doc;
						<<TraceAnnotationRemoveAsync>b__4>d.<>1__state = -1;
						<<TraceAnnotationRemoveAsync>b__4>d.<>t__builder.Start<AnnotationOperationManagerExtensions.<>c__DisplayClass5_0.<<TraceAnnotationRemoveAsync>b__4>d>(ref <<TraceAnnotationRemoveAsync>b__4>d);
						return <<TraceAnnotationRemoveAsync>b__4>d.<>t__builder.Task;
					}, tag).ConfigureAwait(false);
					return;
				}
			}
			throw new ArgumentException("annotations");
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x0002D5CC File Offset: 0x0002B7CC
		private static void TryShowAnnotations(PdfDocument doc)
		{
			if (doc != null)
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(doc);
				MainViewModel mainViewModel = ((pdfControl != null) ? pdfControl.DataContext : null) as MainViewModel;
				if (mainViewModel != null && !mainViewModel.IsAnnotationVisible)
				{
					mainViewModel.IsAnnotationVisible = true;
				}
			}
		}
	}
}
