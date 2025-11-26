using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Models.Annotations;
using pdfeditor.ViewModels;
using PDFKit;

namespace pdfeditor.Utils
{
	// Token: 0x02000071 RID: 113
	public static class AnnotationModelUtils
	{
		// Token: 0x0600088F RID: 2191 RVA: 0x000291F0 File Offset: 0x000273F0
		public static BaseAnnotation CreateRecord(PdfAnnotation annotation, out global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation> replies)
		{
			replies = null;
			if (annotation == null || annotation.Page == null || annotation.Page.Document == null)
			{
				return null;
			}
			if (annotation.Page.Annots == null || annotation.Page.Annots.Count == 0)
			{
				return null;
			}
			if (annotation.Page.Annots.IndexOf(annotation) == -1)
			{
				return null;
			}
			PdfPage page = annotation.Page;
			if (annotation is PdfPopupAnnotation || annotation is PdfMarkupAnnotation)
			{
				global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> readOnlyList = AnnotationFactory.Create(page);
				IReadOnlyDictionary<BaseAnnotation, global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation>> markupAnnotationRepliesModel = CommetUtils.GetMarkupAnnotationRepliesModel(readOnlyList);
				int annotIdx = page.Annots.IndexOf(annotation);
				BaseAnnotation baseAnnotation = readOnlyList.FirstOrDefault((BaseAnnotation c) => c.AnnotIndex == annotIdx);
				if (markupAnnotationRepliesModel != null)
				{
					markupAnnotationRepliesModel.TryGetValue(baseAnnotation, out replies);
				}
				return baseAnnotation;
			}
			return AnnotationFactory.Create(annotation);
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x000292BC File Offset: 0x000274BC
		public static global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> FlattenRecord(BaseAnnotation record, global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation> replies)
		{
			if (record == null)
			{
				return Array.Empty<BaseAnnotation>();
			}
			List<BaseAnnotation> list = new List<BaseAnnotation>();
			AnnotationModelUtils.<FlattenRecord>g__AddCore|1_1(record, list);
			if (replies != null && replies.Count > 0)
			{
				foreach (BaseMarkupAnnotation baseMarkupAnnotation in replies)
				{
					AnnotationModelUtils.<FlattenRecord>g__AddCore|1_1(baseMarkupAnnotation, list);
				}
			}
			List<BaseAnnotation> list2 = new List<BaseAnnotation>();
			foreach (BaseAnnotation baseAnnotation in list)
			{
				if (baseAnnotation != null)
				{
					list2.Add(baseAnnotation);
				}
			}
			return list2.OrderBy((BaseAnnotation c) => c.AnnotIndex).ToList<BaseAnnotation>();
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x00029398 File Offset: 0x00027598
		public static void InsertRecord(PdfDocument doc, BaseAnnotation target, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> flattenedRecord)
		{
			PdfPage pdfPage = doc.Pages[target.PageIndex];
			if (pdfPage.Annots == null)
			{
				pdfPage.CreateAnnotations();
			}
			foreach (BaseAnnotation baseAnnotation in flattenedRecord)
			{
				PdfAnnotation pdfAnnotation = AnnotationFactory.Create(pdfPage, baseAnnotation);
				pdfPage.Annots.Insert(baseAnnotation.AnnotIndex, pdfAnnotation);
			}
			PopupAnnotation popupAnnotation = target as PopupAnnotation;
			if (popupAnnotation != null && popupAnnotation.ParentAnnotationIndex != null)
			{
				((PdfMarkupAnnotation)pdfPage.Annots[popupAnnotation.ParentAnnotationIndex.Value]).Popup = (PdfPopupAnnotation)pdfPage.Annots[popupAnnotation.AnnotIndex];
				((PdfPopupAnnotation)pdfPage.Annots[popupAnnotation.AnnotIndex]).Parent = (PdfMarkupAnnotation)pdfPage.Annots[popupAnnotation.ParentAnnotationIndex.Value];
			}
			foreach (BaseAnnotation baseAnnotation2 in flattenedRecord)
			{
				BaseMarkupAnnotation baseMarkupAnnotation = baseAnnotation2 as BaseMarkupAnnotation;
				if (baseMarkupAnnotation != null)
				{
					if (baseMarkupAnnotation.PopupAnnotationIndex != null)
					{
						((PdfMarkupAnnotation)pdfPage.Annots[baseMarkupAnnotation.AnnotIndex]).Popup = (PdfPopupAnnotation)pdfPage.Annots[baseMarkupAnnotation.PopupAnnotationIndex.Value];
						((PdfPopupAnnotation)pdfPage.Annots[baseMarkupAnnotation.PopupAnnotationIndex.Value]).Parent = (PdfMarkupAnnotation)pdfPage.Annots[baseMarkupAnnotation.AnnotIndex];
					}
					if (baseMarkupAnnotation.RelationshipAnnotation != null && baseMarkupAnnotation.RelationshipAnnotation.AnnotIndex != -1)
					{
						((PdfMarkupAnnotation)pdfPage.Annots[baseMarkupAnnotation.AnnotIndex]).RelationshipAnnotation = pdfPage.Annots[baseMarkupAnnotation.RelationshipAnnotation.AnnotIndex];
					}
				}
				if (baseAnnotation2 is FreeTextAnnotation || baseAnnotation2 is StampAnnotation)
				{
					PdfMarkupAnnotation pdfMarkupAnnotation = pdfPage.Annots[baseAnnotation2.AnnotIndex] as PdfMarkupAnnotation;
					if (pdfMarkupAnnotation != null)
					{
						pdfMarkupAnnotation.TryRedrawAnnotation(false);
					}
				}
				baseAnnotation2.Apply(pdfPage.Annots[baseAnnotation2.AnnotIndex]);
			}
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(doc);
			MainViewModel mainViewModel = ((pdfControl != null) ? pdfControl.DataContext : null) as MainViewModel;
			if (mainViewModel != null)
			{
				PageEditorViewModel pageEditors = mainViewModel.PageEditors;
				if (pageEditors == null)
				{
					return;
				}
				pageEditors.NotifyPageAnnotationChanged(pdfPage.PageIndex);
			}
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x00029660 File Offset: 0x00027860
		public static void RemoveRecord(PdfDocument doc, BaseAnnotation target, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> flattenedRecord)
		{
			PdfPage pdfPage = doc.Pages[target.PageIndex];
			if (pdfPage.Annots == null)
			{
				throw new ArgumentException("Annots is null");
			}
			PdfAnnotation pdfAnnotation = pdfPage.Annots[target.AnnotIndex];
			if (target is PopupAnnotation)
			{
				PdfMarkupAnnotation pdfMarkupAnnotation = ((PdfPopupAnnotation)pdfAnnotation).Parent as PdfMarkupAnnotation;
				if (pdfMarkupAnnotation != null)
				{
					pdfMarkupAnnotation.Popup = null;
				}
			}
			foreach (BaseAnnotation baseAnnotation in flattenedRecord.Reverse<BaseAnnotation>())
			{
				pdfPage.Annots.RemoveAt(baseAnnotation.AnnotIndex);
			}
			global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(doc);
			MainViewModel mainViewModel = ((pdfControl != null) ? pdfControl.DataContext : null) as MainViewModel;
			if (mainViewModel != null)
			{
				PageEditorViewModel pageEditors = mainViewModel.PageEditors;
				if (pageEditors == null)
				{
					return;
				}
				pageEditors.NotifyPageAnnotationChanged(pdfPage.PageIndex);
			}
		}

		// Token: 0x06000893 RID: 2195 RVA: 0x00029750 File Offset: 0x00027950
		[return: global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "target", null })]
		public static global::System.ValueTuple<BaseAnnotation, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>> CreateFlattenedRecord(PdfAnnotation annot)
		{
			if (annot == null)
			{
				throw new ArgumentNullException("annot");
			}
			global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation> readOnlyList;
			BaseAnnotation baseAnnotation = AnnotationModelUtils.CreateRecord(annot, out readOnlyList);
			return new global::System.ValueTuple<BaseAnnotation, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>>(baseAnnotation, AnnotationModelUtils.FlattenRecord(baseAnnotation, readOnlyList));
		}

		// Token: 0x06000894 RID: 2196 RVA: 0x00029780 File Offset: 0x00027980
		public static BaseAnnotation CloneRecord(PdfAnnotation annotation, out global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation> replies)
		{
			if (annotation == null)
			{
				throw new ArgumentNullException("annotation");
			}
			BaseAnnotation baseAnnotation = AnnotationModelUtils.CreateRecord(annotation, out replies);
			global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> readOnlyList = AnnotationModelUtils.FlattenRecord(baseAnnotation, replies);
			BaseAnnotation[] array = readOnlyList.OrderBy((BaseAnnotation c) => c.AnnotIndex).ToArray<BaseAnnotation>();
			if (array.Length == 0)
			{
				return null;
			}
			int num = annotation.Page.Annots.Count - array[0].AnnotIndex;
			foreach (BaseAnnotation baseAnnotation2 in readOnlyList.OrderBy((BaseAnnotation c) => c.AnnotIndex))
			{
				baseAnnotation2.AnnotIndex += num;
			}
			return baseAnnotation;
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x00029864 File Offset: 0x00027A64
		[CompilerGenerated]
		internal static void <FlattenRecord>g__AddCore|1_1(BaseAnnotation _record, List<BaseAnnotation> _list)
		{
			_list.Add(_record);
			BaseMarkupAnnotation baseMarkupAnnotation = _record as BaseMarkupAnnotation;
			if (baseMarkupAnnotation != null && baseMarkupAnnotation.Popup != null)
			{
				_list.Add(baseMarkupAnnotation.Popup);
			}
		}
	}
}
