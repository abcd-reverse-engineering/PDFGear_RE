using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Models.Annotations;

namespace pdfeditor.Utils
{
	// Token: 0x02000072 RID: 114
	public class AnnotationTrace
	{
		// Token: 0x06000896 RID: 2198 RVA: 0x00029898 File Offset: 0x00027A98
		public AnnotationTrace(PdfDocument document)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			if (document.Pages.Count > 0)
			{
				this.pages = document.Pages.Take(20).ToArray<PdfPage>();
				return;
			}
			this.pages = Array.Empty<PdfPage>();
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x000298EC File Offset: 0x00027AEC
		public object[] GetPageAnnotationModels(int pageIndex, bool onlyType)
		{
			if (this.pages.Length == 0)
			{
				return Array.Empty<object>();
			}
			if (pageIndex < 0 || pageIndex >= this.pages.Length)
			{
				throw new ArgumentException("pageIndex");
			}
			IEnumerable<IGrouping<string, BaseAnnotation>> enumerable = from c in this.pages[pageIndex].Annots.Where((PdfAnnotation c) => c.NormalAppearance != null || c is PdfPopupAnnotation).Select(delegate(PdfAnnotation c)
				{
					try
					{
						return AnnotationFactory.Create(c);
					}
					catch
					{
					}
					return null;
				})
				where c != null
				group c by c.AnnotationType;
			if (onlyType)
			{
				return enumerable.Select((IGrouping<string, BaseAnnotation> c) => new
				{
					type = c.Key,
					count = c.Count<BaseAnnotation>()
				}).ToArray();
			}
			return enumerable.Select(delegate(IGrouping<string, BaseAnnotation> c)
			{
				BaseAnnotation[] array = c.ToArray<BaseAnnotation>();
				return new
				{
					type = c.Key,
					count = array.Length,
					items = array
				};
			}).ToArray();
		}

		// Token: 0x06000898 RID: 2200 RVA: 0x00029A1C File Offset: 0x00027C1C
		public object[] GetAnnotationModelTraceObject()
		{
			if (this.pages.Length == 0)
			{
				return Array.Empty<object>();
			}
			return this.pages.Select((PdfPage c) => new
			{
				pageIndex = c.PageIndex,
				annots = this.GetPageAnnotationModels(c.PageIndex, false)
			}).ToArray();
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x00029A58 File Offset: 0x00027C58
		public object[] GetAnnotationTypeTraceObject()
		{
			if (this.pages.Length == 0)
			{
				return Array.Empty<object>();
			}
			return this.pages.Select((PdfPage c) => new
			{
				pageIndex = c.PageIndex,
				annots = this.GetPageAnnotationModels(c.PageIndex, true)
			}).ToArray();
		}

		// Token: 0x0600089A RID: 2202 RVA: 0x00029A94 File Offset: 0x00027C94
		[return: global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "type", "count" })]
		public global::System.ValueTuple<string, int>[] GetAnnotationTypeTraceObjectAllPages()
		{
			if (this.pages.Length == 0)
			{
				return Array.Empty<global::System.ValueTuple<string, int>>();
			}
			IEnumerable<PdfAnnotation> enumerable = this.pages.Where((PdfPage a) => a.Annots != null).SelectMany((PdfPage p) => p.Annots);
			if (enumerable == null || (enumerable != null && enumerable.Count<PdfAnnotation>() <= 0))
			{
				return Array.Empty<global::System.ValueTuple<string, int>>();
			}
			IEnumerable<IGrouping<string, BaseAnnotation>> enumerable2 = from c in enumerable.Select(delegate(PdfAnnotation c)
				{
					try
					{
						return AnnotationFactory.Create(c);
					}
					catch
					{
					}
					return null;
				})
				where c != null
				group c by c.AnnotationType;
			if (enumerable2 == null || (enumerable2 != null && enumerable2.Count<IGrouping<string, BaseAnnotation>>() <= 0))
			{
				return Array.Empty<global::System.ValueTuple<string, int>>();
			}
			return enumerable2.Select((IGrouping<string, BaseAnnotation> c) => new global::System.ValueTuple<string, int>(c.Key, c.Count<BaseAnnotation>())).ToArray<global::System.ValueTuple<string, int>>();
		}

		// Token: 0x04000443 RID: 1091
		public const int MAX_TRACE_PAGE_COUNT = 20;

		// Token: 0x04000444 RID: 1092
		private PdfPage[] pages;
	}
}
