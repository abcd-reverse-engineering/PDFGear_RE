using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Models.Annotations;
using pdfeditor.Models.Operations;
using pdfeditor.ViewModels;
using PDFKit;

namespace pdfeditor.Utils
{
	// Token: 0x02000088 RID: 136
	public class AnnotationChangedTraceContext : IDisposable
	{
		// Token: 0x06000919 RID: 2329 RVA: 0x0002D606 File Offset: 0x0002B806
		public AnnotationChangedTraceContext(OperationManager manager, PdfDocument pdfDocument, string tag)
		{
			this.manager = manager;
			this.pdfDocument = pdfDocument;
			this.tag = tag;
			this.startAnnotations = new Dictionary<int, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>>();
		}

		// Token: 0x0600091A RID: 2330 RVA: 0x0002D630 File Offset: 0x0002B830
		public void TryAddPage(int pageIndex)
		{
			if (this.completed)
			{
				throw new ObjectDisposedException("AnnotationChangedTraceContext");
			}
			if (pageIndex < 0)
			{
				return;
			}
			if (this.startAnnotations.ContainsKey(pageIndex))
			{
				return;
			}
			this.startAnnotations[pageIndex] = AnnotationFactory.Create(this.pdfDocument.Pages[pageIndex]);
		}

		// Token: 0x0600091B RID: 2331 RVA: 0x0002D688 File Offset: 0x0002B888
		public void Dispose()
		{
			if (this.completed)
			{
				throw new ObjectDisposedException("AnnotationChangedTraceContext");
			}
			this.completed = true;
			Dictionary<int, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>> endAnnotations = this.startAnnotations.ToDictionary((KeyValuePair<int, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>> c) => c.Key, (KeyValuePair<int, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>> c) => AnnotationFactory.Create(this.pdfDocument.Pages[c.Key]));
			if (this.startAnnotations.Count != endAnnotations.Count)
			{
				throw new ArgumentException("Not supported insert or delete annotation");
			}
			foreach (KeyValuePair<int, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>> keyValuePair in this.startAnnotations.ToList<KeyValuePair<int, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>>>())
			{
				int num;
				global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> readOnlyList;
				keyValuePair.Deconstruct(out num, out readOnlyList);
				int num2 = num;
				if (this.startAnnotations[num2].Count != endAnnotations[num2].Count)
				{
					throw new ArgumentException("Not supported insert or delete annotation");
				}
				if (BaseAnnotation.CollectionEqual<BaseAnnotation>(this.startAnnotations[num2], endAnnotations[num2]))
				{
					this.startAnnotations.Remove(num2);
					endAnnotations.Remove(num2);
				}
				else
				{
					List<BaseAnnotation> list = new List<BaseAnnotation>();
					List<BaseAnnotation> list2 = new List<BaseAnnotation>();
					for (int i = 0; i < this.startAnnotations[num2].Count; i++)
					{
						if (!this.startAnnotations[num2][i].Equals(endAnnotations[num2][i]))
						{
							list.Add(this.startAnnotations[num2][i]);
							list2.Add(endAnnotations[num2][i]);
						}
					}
					this.startAnnotations[num2] = list;
					endAnnotations[num2] = list2;
				}
			}
			if (this.startAnnotations.Count == 0)
			{
				return;
			}
			this.manager.AddOperationAsync(delegate(PdfDocument doc)
			{
				global::PDFKit.PdfControl pdfControl = global::PDFKit.PdfControl.GetPdfControl(doc);
				MainViewModel mainViewModel = ((pdfControl != null) ? pdfControl.DataContext : null) as MainViewModel;
				if (mainViewModel != null && !mainViewModel.IsAnnotationVisible)
				{
					mainViewModel.IsAnnotationVisible = true;
				}
				foreach (KeyValuePair<int, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>> keyValuePair2 in this.startAnnotations)
				{
					int num3;
					global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> readOnlyList2;
					keyValuePair2.Deconstruct(out num3, out readOnlyList2);
					int num4 = num3;
					global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> readOnlyList3 = readOnlyList2;
					PdfPage pdfPage = doc.Pages[num4];
					if (pdfPage.Annots == null)
					{
						throw new ArgumentException("Annots is null");
					}
					for (int j = 0; j < readOnlyList3.Count; j++)
					{
						readOnlyList3[j].Apply(pdfPage.Annots[readOnlyList3[j].AnnotIndex]);
						PdfMarkupAnnotation pdfMarkupAnnotation = pdfPage.Annots[readOnlyList3[j].AnnotIndex] as PdfMarkupAnnotation;
						if (pdfMarkupAnnotation != null)
						{
							if (pdfMarkupAnnotation is PdfStampAnnotation)
							{
								PdfPageObjectsCollection normalAppearance = pdfMarkupAnnotation.NormalAppearance;
								if (((normalAppearance != null) ? normalAppearance.OfType<PdfImageObject>().FirstOrDefault<PdfImageObject>() : null) != null)
								{
									pdfPage.TryRedrawPageAsync(default(CancellationToken));
									goto IL_013D;
								}
							}
							pdfMarkupAnnotation.TryRedrawAnnotation(false);
						}
						else if (pdfPage.Annots[readOnlyList3[j].AnnotIndex] is PdfLinkAnnotation)
						{
							pdfPage.TryRedrawPageAsync(default(CancellationToken));
						}
						IL_013D:;
					}
					if (mainViewModel != null)
					{
						PageEditorViewModel pageEditors = mainViewModel.PageEditors;
						if (pageEditors != null)
						{
							pageEditors.NotifyPageAnnotationChanged(num4);
						}
					}
				}
				if (((mainViewModel != null) ? mainViewModel.PageEditors : null) != null)
				{
					foreach (KeyValuePair<int, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>> keyValuePair3 in this.startAnnotations)
					{
						int num3;
						global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> readOnlyList2;
						keyValuePair3.Deconstruct(out num3, out readOnlyList2);
						if (readOnlyList2.Any((BaseAnnotation t) => t is AttachmentAnnotation))
						{
							mainViewModel.PageEditors.NotifyAttachmentChanged();
							break;
						}
					}
				}
			}, delegate(PdfDocument doc)
			{
				global::PDFKit.PdfControl pdfControl2 = global::PDFKit.PdfControl.GetPdfControl(doc);
				MainViewModel mainViewModel2 = ((pdfControl2 != null) ? pdfControl2.DataContext : null) as MainViewModel;
				if (mainViewModel2 != null && !mainViewModel2.IsAnnotationVisible)
				{
					mainViewModel2.IsAnnotationVisible = true;
				}
				foreach (KeyValuePair<int, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>> keyValuePair4 in endAnnotations)
				{
					int num5;
					global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> readOnlyList4;
					keyValuePair4.Deconstruct(out num5, out readOnlyList4);
					int num6 = num5;
					global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> readOnlyList5 = readOnlyList4;
					PdfPage pdfPage2 = doc.Pages[num6];
					if (pdfPage2.Annots == null)
					{
						throw new ArgumentException("Annots is null");
					}
					for (int k = 0; k < readOnlyList5.Count; k++)
					{
						readOnlyList5[k].Apply(pdfPage2.Annots[readOnlyList5[k].AnnotIndex]);
						PdfMarkupAnnotation pdfMarkupAnnotation2 = pdfPage2.Annots[readOnlyList5[k].AnnotIndex] as PdfMarkupAnnotation;
						if (pdfMarkupAnnotation2 != null)
						{
							if (pdfMarkupAnnotation2 is PdfStampAnnotation)
							{
								PdfPageObjectsCollection normalAppearance2 = pdfMarkupAnnotation2.NormalAppearance;
								if (((normalAppearance2 != null) ? normalAppearance2.OfType<PdfImageObject>().FirstOrDefault<PdfImageObject>() : null) != null)
								{
									pdfPage2.TryRedrawPageAsync(default(CancellationToken));
									goto IL_0138;
								}
							}
							pdfMarkupAnnotation2.TryRedrawAnnotation(false);
						}
						else if (pdfPage2.Annots[readOnlyList5[k].AnnotIndex] is PdfLinkAnnotation)
						{
							pdfPage2.TryRedrawPageAsync(default(CancellationToken));
						}
						IL_0138:;
					}
					if (mainViewModel2 != null)
					{
						PageEditorViewModel pageEditors2 = mainViewModel2.PageEditors;
						if (pageEditors2 != null)
						{
							pageEditors2.NotifyPageAnnotationChanged(num6);
						}
					}
				}
				if (((mainViewModel2 != null) ? mainViewModel2.PageEditors : null) != null)
				{
					foreach (KeyValuePair<int, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>> keyValuePair5 in this.startAnnotations)
					{
						int num5;
						global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> readOnlyList4;
						keyValuePair5.Deconstruct(out num5, out readOnlyList4);
						if (readOnlyList4.Any((BaseAnnotation t) => t is AttachmentAnnotation))
						{
							mainViewModel2.PageEditors.NotifyAttachmentChanged();
							break;
						}
					}
				}
			}, this.tag).Wait();
		}

		// Token: 0x04000466 RID: 1126
		private bool completed;

		// Token: 0x04000467 RID: 1127
		private readonly OperationManager manager;

		// Token: 0x04000468 RID: 1128
		private readonly PdfDocument pdfDocument;

		// Token: 0x04000469 RID: 1129
		private readonly string tag;

		// Token: 0x0400046A RID: 1130
		private Dictionary<int, global::System.Collections.Generic.IReadOnlyList<BaseAnnotation>> startAnnotations;
	}
}
