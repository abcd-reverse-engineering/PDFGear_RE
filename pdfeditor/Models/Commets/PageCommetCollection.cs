using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using pdfeditor.Controls;
using pdfeditor.Models.Annotations;
using pdfeditor.Properties;
using pdfeditor.Utils;
using PDFKit.Utils;

namespace pdfeditor.Models.Commets
{
	// Token: 0x0200018E RID: 398
	public class PageCommetCollection : ObservableObject, global::System.Collections.Generic.IReadOnlyList<CommetModel>, IReadOnlyCollection<CommetModel>, IEnumerable<CommetModel>, IEnumerable, ITreeViewNode
	{
		// Token: 0x060016E0 RID: 5856 RVA: 0x00057666 File Offset: 0x00055866
		private PageCommetCollection(PdfDocument document, int pageIndex)
		{
			this.document = document;
			this.pageIndex = pageIndex;
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x00057683 File Offset: 0x00055883
		public PageCommetCollection(PdfDocument document, int pageIndex, List<CommetModel> models)
		{
			this.document = document;
			this.pageIndex = pageIndex;
			this.items = models;
		}

		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x060016E2 RID: 5858 RVA: 0x000576A7 File Offset: 0x000558A7
		public int PageIndex
		{
			get
			{
				return this.pageIndex;
			}
		}

		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x060016E3 RID: 5859 RVA: 0x000576B0 File Offset: 0x000558B0
		public string DisplayPageIndex
		{
			get
			{
				if (!Resources.LeftNavigationAnnotationPageLabelContent.Contains("XXX"))
				{
					return string.Format("{0} {1}", Resources.LeftNavigationAnnotationPageLabelContent, this.pageIndex + 1);
				}
				return Resources.LeftNavigationAnnotationPageLabelContent.Replace("XXX", (this.pageIndex + 1).ToString()) + ":";
			}
		}

		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x060016E4 RID: 5860 RVA: 0x00057714 File Offset: 0x00055914
		public PdfDocument Document
		{
			get
			{
				return this.document;
			}
		}

		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x060016E5 RID: 5861 RVA: 0x0005771C File Offset: 0x0005591C
		// (set) Token: 0x060016E6 RID: 5862 RVA: 0x00057724 File Offset: 0x00055924
		public bool IsExpanded
		{
			get
			{
				return this.isExpanded;
			}
			set
			{
				base.SetProperty<bool>(ref this.isExpanded, value, "IsExpanded");
			}
		}

		// Token: 0x17000917 RID: 2327
		public CommetModel this[int index]
		{
			get
			{
				return this.items[index];
			}
		}

		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x060016E8 RID: 5864 RVA: 0x00057747 File Offset: 0x00055947
		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		// Token: 0x17000919 RID: 2329
		// (get) Token: 0x060016E9 RID: 5865 RVA: 0x00057754 File Offset: 0x00055954
		public ITreeViewNode Parent
		{
			get
			{
				return null;
			}
		}

		// Token: 0x060016EA RID: 5866 RVA: 0x00057757 File Offset: 0x00055957
		public IEnumerator<CommetModel> GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		// Token: 0x060016EB RID: 5867 RVA: 0x00057769 File Offset: 0x00055969
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		// Token: 0x060016EC RID: 5868 RVA: 0x0005777C File Offset: 0x0005597C
		public static PageCommetCollection Create(PdfDocument document, int pageIndex)
		{
			IEnumerable<CommetModel> enumerable = PageCommetCollection.CreateCore(document, pageIndex);
			if (enumerable != null)
			{
				PageCommetCollection pageCommetCollection = new PageCommetCollection(document, pageIndex)
				{
					items = enumerable.ToList<CommetModel>()
				};
				foreach (CommetModel commetModel in pageCommetCollection.items)
				{
					commetModel.Parent = pageCommetCollection;
				}
				return pageCommetCollection;
			}
			return null;
		}

		// Token: 0x060016ED RID: 5869 RVA: 0x000577F0 File Offset: 0x000559F0
		private static IEnumerable<CommetModel> CreateCore(PdfDocument document, int pageIndex)
		{
			if ((!document.IsDisposed && pageIndex < 0) || pageIndex >= document.Pages.Count)
			{
				return null;
			}
			global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> readOnlyList = null;
			PdfPage pdfPage = document.Pages[pageIndex];
			if (pdfPage.IsLoaded)
			{
				PdfPage pdfPage2 = pdfPage;
				lock (pdfPage2)
				{
					if (pdfPage.IsLoaded && pdfPage.Annots != null && pdfPage.Annots.Count > 0)
					{
						readOnlyList = AnnotationFactory.Create(pdfPage);
					}
				}
			}
			if (readOnlyList == null && !pdfPage.IsLoaded)
			{
				IntPtr intPtr = IntPtr.Zero;
				try
				{
					PageDisposeHelper.TryFixPageAnnotations(document, pageIndex);
					intPtr = Pdfium.FPDF_LoadPage(document.Handle, pageIndex);
					pdfPage = PdfPage.FromHandle(document, intPtr, pageIndex, true);
					if (pdfPage.Annots != null && pdfPage.Annots.Count > 0)
					{
						readOnlyList = AnnotationFactory.Create(pdfPage);
					}
				}
				finally
				{
					Pdfium.FPDF_ClosePage(intPtr);
				}
			}
			if (readOnlyList != null && readOnlyList.Count > 0)
			{
				List<CommetModel> list = PageCommetCollection.CreateCommets(readOnlyList);
				if (list != null && list.Count > 0)
				{
					return list;
				}
			}
			return null;
		}

		// Token: 0x060016EE RID: 5870 RVA: 0x0005790C File Offset: 0x00055B0C
		private static List<CommetModel> CreateCommets(global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> annots)
		{
			if (annots == null || annots.Count == 0)
			{
				return new List<CommetModel>();
			}
			IReadOnlyDictionary<BaseAnnotation, global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation>> markupAnnotationRepliesModel = CommetUtils.GetMarkupAnnotationRepliesModel(annots);
			List<CommetModel> list = new List<CommetModel>();
			foreach (BaseAnnotation baseAnnotation in annots)
			{
				if (!(baseAnnotation is PopupAnnotation) && !(baseAnnotation is WatermarkAnnotation) && !(baseAnnotation is LinkAnnotation) && !(baseAnnotation is NotImplementedAnnotation))
				{
					global::System.Collections.Generic.IReadOnlyList<CommetModel> readOnlyList = null;
					BaseMarkupAnnotation baseMarkupAnnotation = baseAnnotation as BaseMarkupAnnotation;
					if (baseMarkupAnnotation != null)
					{
						if (baseMarkupAnnotation.Relationship != RelationTypes.NonSpecified)
						{
							continue;
						}
						global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation> readOnlyList2;
						if (markupAnnotationRepliesModel.TryGetValue(baseMarkupAnnotation, out readOnlyList2))
						{
							readOnlyList = PageCommetCollection.CreateReplies(readOnlyList2);
						}
					}
					CommetModel commetModel = CommetModel.TryCreate(baseAnnotation, readOnlyList);
					if (commetModel != null)
					{
						if (readOnlyList != null)
						{
							foreach (CommetModel commetModel2 in readOnlyList)
							{
								commetModel2.Parent = commetModel;
							}
						}
						list.Add(commetModel);
					}
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list;
		}

		// Token: 0x060016EF RID: 5871 RVA: 0x00057A28 File Offset: 0x00055C28
		private static global::System.Collections.Generic.IReadOnlyList<CommetModel> CreateReplies(global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation> replies)
		{
			if (replies == null || replies.Count == 0)
			{
				return null;
			}
			List<CommetModel> list = new List<CommetModel>();
			for (int i = 0; i < replies.Count; i++)
			{
				CommetModel commetModel = CommetModel.TryCreate(replies[i], null);
				if (commetModel != null)
				{
					list.Add(commetModel);
				}
			}
			return list;
		}

		// Token: 0x040007A3 RID: 1955
		private readonly PdfDocument document;

		// Token: 0x040007A4 RID: 1956
		private readonly int pageIndex;

		// Token: 0x040007A5 RID: 1957
		private List<CommetModel> items;

		// Token: 0x040007A6 RID: 1958
		private bool isExpanded = true;
	}
}
