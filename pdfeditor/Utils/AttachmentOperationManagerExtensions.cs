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
	// Token: 0x02000075 RID: 117
	public static class AttachmentOperationManagerExtensions
	{
		// Token: 0x060008B9 RID: 2233 RVA: 0x0002B3C8 File Offset: 0x000295C8
		public static async Task TraceAttachmentRemoveAsync(this OperationManager manager, PdfAttachment attachment, PdfFileAttachmentAnnotation annotation, string tag = "")
		{
			await manager.TraceAttachmentRemoveAsync(new PdfAttachment[] { attachment }, new PdfFileAttachmentAnnotation[] { annotation }, tag).ConfigureAwait(false);
		}

		// Token: 0x060008BA RID: 2234 RVA: 0x0002B424 File Offset: 0x00029624
		public static async Task TraceAttachmentRemoveAsync(this OperationManager manager, global::System.Collections.Generic.IReadOnlyList<PdfAttachment> attachments, global::System.Collections.Generic.IReadOnlyList<PdfFileAttachmentAnnotation> annotations, string tag = "")
		{
			AttachmentOperationManagerExtensions.<>c__DisplayClass1_0 CS$<>8__locals1 = new AttachmentOperationManagerExtensions.<>c__DisplayClass1_0();
			CS$<>8__locals1.attachments = attachments;
			CS$<>8__locals1.annotations = annotations;
			if ((CS$<>8__locals1.attachments != null && CS$<>8__locals1.attachments.Count != 0) || (CS$<>8__locals1.annotations != null && CS$<>8__locals1.annotations.Count != 0))
			{
				CS$<>8__locals1.records = (from c in CS$<>8__locals1.annotations
					select AnnotationModelUtils.CreateFlattenedRecord(c) into c
					orderby c.Item1.AnnotIndex
					select c).ToArray<global::System.ValueTuple<BaseAnnotation, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>>>();
				await manager.AddOperationAsync(delegate(PdfDocument doc)
				{
					AttachmentOperationManagerExtensions.<>c__DisplayClass1_0.<<TraceAttachmentRemoveAsync>b__2>d <<TraceAttachmentRemoveAsync>b__2>d;
					<<TraceAttachmentRemoveAsync>b__2>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<TraceAttachmentRemoveAsync>b__2>d.<>4__this = CS$<>8__locals1;
					<<TraceAttachmentRemoveAsync>b__2>d.doc = doc;
					<<TraceAttachmentRemoveAsync>b__2>d.<>1__state = -1;
					<<TraceAttachmentRemoveAsync>b__2>d.<>t__builder.Start<AttachmentOperationManagerExtensions.<>c__DisplayClass1_0.<<TraceAttachmentRemoveAsync>b__2>d>(ref <<TraceAttachmentRemoveAsync>b__2>d);
					return <<TraceAttachmentRemoveAsync>b__2>d.<>t__builder.Task;
				}, delegate(PdfDocument doc)
				{
					AttachmentOperationManagerExtensions.<>c__DisplayClass1_0.<<TraceAttachmentRemoveAsync>b__3>d <<TraceAttachmentRemoveAsync>b__3>d;
					<<TraceAttachmentRemoveAsync>b__3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<TraceAttachmentRemoveAsync>b__3>d.<>4__this = CS$<>8__locals1;
					<<TraceAttachmentRemoveAsync>b__3>d.doc = doc;
					<<TraceAttachmentRemoveAsync>b__3>d.<>1__state = -1;
					<<TraceAttachmentRemoveAsync>b__3>d.<>t__builder.Start<AttachmentOperationManagerExtensions.<>c__DisplayClass1_0.<<TraceAttachmentRemoveAsync>b__3>d>(ref <<TraceAttachmentRemoveAsync>b__3>d);
					return <<TraceAttachmentRemoveAsync>b__3>d.<>t__builder.Task;
				}, tag).ConfigureAwait(false);
			}
		}

		// Token: 0x060008BB RID: 2235 RVA: 0x0002B480 File Offset: 0x00029680
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
