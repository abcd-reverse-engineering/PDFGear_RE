using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.Models.Annotations;

namespace pdfeditor.Utils
{
	// Token: 0x02000077 RID: 119
	public static class CommetUtils
	{
		// Token: 0x060008C1 RID: 2241 RVA: 0x0002B66C File Offset: 0x0002986C
		public static IReadOnlyDictionary<PdfAnnotation, global::System.Collections.Generic.IReadOnlyList<PdfMarkupAnnotation>> GetMarkupAnnotationReplies(PdfPage page)
		{
			if (page == null || page.Annots == null || page.Annots.Count == 0)
			{
				return null;
			}
			Func<BaseMarkupAnnotation, PdfAnnotation> <>9__2;
			return CommetUtils.GetMarkupAnnotationRepliesModel(AnnotationFactory.Create(page)).ToDictionary((KeyValuePair<BaseAnnotation, global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation>> c) => page.Annots[c.Key.AnnotIndex], delegate(KeyValuePair<BaseAnnotation, global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation>> c)
			{
				IEnumerable<BaseMarkupAnnotation> value = c.Value;
				Func<BaseMarkupAnnotation, PdfAnnotation> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = (BaseMarkupAnnotation x) => page.Annots[x.AnnotIndex]);
				}
				return value.Select(func).OfType<PdfMarkupAnnotation>().ToArray<PdfMarkupAnnotation>();
			}, new CommetUtils.PdfAnnotationCompare());
		}

		// Token: 0x060008C2 RID: 2242 RVA: 0x0002B6E4 File Offset: 0x000298E4
		public static IReadOnlyDictionary<BaseAnnotation, global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation>> GetMarkupAnnotationRepliesModel(global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> annots)
		{
			if (annots == null || annots.Count == 0)
			{
				return null;
			}
			return (from c in (from c in annots.OfType<BaseMarkupAnnotation>()
					where c.Relationship == RelationTypes.Reply && c.RelationshipAnnotation != null
					select c).GroupBy((BaseMarkupAnnotation c) => CommetUtils.GetRelationshipRoot(c), (BaseAnnotation c, IEnumerable<BaseMarkupAnnotation> s) => new global::System.ValueTuple<BaseAnnotation, global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation>>(c, s.ToArray<BaseMarkupAnnotation>()))
				where c.Item2.Count > 0
				select c).ToDictionary(([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "Key", "Value" })] global::System.ValueTuple<BaseAnnotation, global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation>> c) => c.Item1, ([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "Key", "Value" })] global::System.ValueTuple<BaseAnnotation, global::System.Collections.Generic.IReadOnlyList<BaseMarkupAnnotation>> c) => c.Item2);
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x0002B7D4 File Offset: 0x000299D4
		private static BaseAnnotation GetRelationshipRoot(BaseMarkupAnnotation annot)
		{
			if (annot.Relationship != RelationTypes.Reply)
			{
				return null;
			}
			if (annot.RelationshipAnnotation == null)
			{
				return null;
			}
			BaseMarkupAnnotation baseMarkupAnnotation = annot.RelationshipAnnotation as BaseMarkupAnnotation;
			if (baseMarkupAnnotation != null)
			{
				return CommetUtils.GetRelationshipRoot(baseMarkupAnnotation) ?? baseMarkupAnnotation;
			}
			return annot.RelationshipAnnotation;
		}

		// Token: 0x02000430 RID: 1072
		private class PdfAnnotationCompare : IEqualityComparer<PdfAnnotation>
		{
			// Token: 0x06002CD4 RID: 11476 RVA: 0x000DB666 File Offset: 0x000D9866
			public bool Equals(PdfAnnotation x, PdfAnnotation y)
			{
				return x == y || (x != null && x.Equals(y));
			}

			// Token: 0x06002CD5 RID: 11477 RVA: 0x000DB67F File Offset: 0x000D987F
			public int GetHashCode(PdfAnnotation obj)
			{
				if (obj == null)
				{
					return int.MinValue;
				}
				return (int)(long)obj.Dictionary.Handle;
			}
		}
	}
}
