using System;
using System.Collections.Generic;
using System.Linq;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;

namespace pdfeditor.Models.PageContents
{
	// Token: 0x0200014D RID: 333
	public abstract class PageBaseObject : IEquatable<PageBaseObject>
	{
		// Token: 0x1700080C RID: 2060
		// (get) Token: 0x060013EB RID: 5099
		public abstract PageObjectTypes ObjectType { get; }

		// Token: 0x1700080D RID: 2061
		// (get) Token: 0x060013EC RID: 5100 RVA: 0x0004F94C File Offset: 0x0004DB4C
		// (set) Token: 0x060013ED RID: 5101 RVA: 0x0004F954 File Offset: 0x0004DB54
		public List<MarkedContentModel> MarkedContent { get; protected set; }

		// Token: 0x1700080E RID: 2062
		// (get) Token: 0x060013EE RID: 5102 RVA: 0x0004F95D File Offset: 0x0004DB5D
		// (set) Token: 0x060013EF RID: 5103 RVA: 0x0004F965 File Offset: 0x0004DB65
		public FS_COLOR FillColor { get; protected set; }

		// Token: 0x1700080F RID: 2063
		// (get) Token: 0x060013F0 RID: 5104 RVA: 0x0004F96E File Offset: 0x0004DB6E
		// (set) Token: 0x060013F1 RID: 5105 RVA: 0x0004F976 File Offset: 0x0004DB76
		public FS_COLOR StrokeColor { get; protected set; }

		// Token: 0x17000810 RID: 2064
		// (get) Token: 0x060013F2 RID: 5106 RVA: 0x0004F97F File Offset: 0x0004DB7F
		// (set) Token: 0x060013F3 RID: 5107 RVA: 0x0004F987 File Offset: 0x0004DB87
		public float Flatness { get; protected set; }

		// Token: 0x17000811 RID: 2065
		// (get) Token: 0x060013F4 RID: 5108 RVA: 0x0004F990 File Offset: 0x0004DB90
		// (set) Token: 0x060013F5 RID: 5109 RVA: 0x0004F998 File Offset: 0x0004DB98
		public float Smoothness { get; protected set; }

		// Token: 0x17000812 RID: 2066
		// (get) Token: 0x060013F6 RID: 5110 RVA: 0x0004F9A1 File Offset: 0x0004DBA1
		// (set) Token: 0x060013F7 RID: 5111 RVA: 0x0004F9A9 File Offset: 0x0004DBA9
		public BlendTypes BlendMode { get; protected set; }

		// Token: 0x17000813 RID: 2067
		// (get) Token: 0x060013F8 RID: 5112 RVA: 0x0004F9B2 File Offset: 0x0004DBB2
		// (set) Token: 0x060013F9 RID: 5113 RVA: 0x0004F9BA File Offset: 0x0004DBBA
		public RenderIntent RenderIntent { get; protected set; }

		// Token: 0x17000814 RID: 2068
		// (get) Token: 0x060013FA RID: 5114 RVA: 0x0004F9C3 File Offset: 0x0004DBC3
		// (set) Token: 0x060013FB RID: 5115 RVA: 0x0004F9CB File Offset: 0x0004DBCB
		public OverprintModes OverprintMode { get; protected set; }

		// Token: 0x17000815 RID: 2069
		// (get) Token: 0x060013FC RID: 5116 RVA: 0x0004F9D4 File Offset: 0x0004DBD4
		// (set) Token: 0x060013FD RID: 5117 RVA: 0x0004F9DC File Offset: 0x0004DBDC
		public bool StrokeOverprint { get; protected set; }

		// Token: 0x17000816 RID: 2070
		// (get) Token: 0x060013FE RID: 5118 RVA: 0x0004F9E5 File Offset: 0x0004DBE5
		// (set) Token: 0x060013FF RID: 5119 RVA: 0x0004F9ED File Offset: 0x0004DBED
		public bool FillOverprint { get; protected set; }

		// Token: 0x17000817 RID: 2071
		// (get) Token: 0x06001400 RID: 5120 RVA: 0x0004F9F6 File Offset: 0x0004DBF6
		// (set) Token: 0x06001401 RID: 5121 RVA: 0x0004F9FE File Offset: 0x0004DBFE
		public bool AlphaShape { get; protected set; }

		// Token: 0x17000818 RID: 2072
		// (get) Token: 0x06001402 RID: 5122 RVA: 0x0004FA07 File Offset: 0x0004DC07
		// (set) Token: 0x06001403 RID: 5123 RVA: 0x0004FA0F File Offset: 0x0004DC0F
		public FS_MATRIX Matrix { get; protected set; }

		// Token: 0x06001404 RID: 5124 RVA: 0x0004FA18 File Offset: 0x0004DC18
		protected virtual void Init(PdfPageObject pageObject)
		{
			if (pageObject.ObjectType != this.ObjectType)
			{
				throw new ArgumentException("pageObject");
			}
			this.FillColor = PageBaseObject.ReturnValueOrDefault<FS_COLOR>(() => pageObject.FillColor);
			this.StrokeColor = PageBaseObject.ReturnValueOrDefault<FS_COLOR>(() => pageObject.StrokeColor);
			this.Flatness = PageBaseObject.ReturnValueOrDefault<float>(() => pageObject.Flatness);
			this.Smoothness = PageBaseObject.ReturnValueOrDefault<float>(() => pageObject.Smoothness);
			this.BlendMode = PageBaseObject.ReturnValueOrDefault<BlendTypes>(() => pageObject.BlendMode);
			this.RenderIntent = PageBaseObject.ReturnValueOrDefault<RenderIntent>(() => pageObject.RenderIntent);
			this.OverprintMode = PageBaseObject.ReturnValueOrDefault<OverprintModes>(() => pageObject.OverprintMode);
			this.StrokeOverprint = PageBaseObject.ReturnValueOrDefault<bool>(() => pageObject.StrokeOverprint);
			this.FillOverprint = PageBaseObject.ReturnValueOrDefault<bool>(() => pageObject.FillOverprint);
			this.AlphaShape = PageBaseObject.ReturnValueOrDefault<bool>(() => pageObject.AlphaShape);
			this.Matrix = PageBaseObject.ReturnValueOrDefault<FS_MATRIX>(() => new FS_MATRIX(pageObject.Matrix.ToArray()));
			this.MarkedContent = PageBaseObject.ReturnValueOrDefault<List<MarkedContentModel>>(delegate
			{
				PdfMarkedContentCollection markedContent = pageObject.MarkedContent;
				List<MarkedContentModel> list;
				if (markedContent == null)
				{
					list = null;
				}
				else
				{
					list = markedContent.Select((PdfMarkedContent c) => MarkedContentModel.Create(c)).ToList<MarkedContentModel>();
				}
				return list ?? new List<MarkedContentModel>();
			});
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x0004FB64 File Offset: 0x0004DD64
		public void Apply(PdfPageObject pageObject)
		{
			PageObjectTypes? pageObjectTypes = ((pageObject != null) ? new PageObjectTypes?(pageObject.ObjectType) : null);
			PageObjectTypes objectType = this.ObjectType;
			if (!((pageObjectTypes.GetValueOrDefault() == objectType) & (pageObjectTypes != null)))
			{
				throw new ArgumentException("pageObject");
			}
			pageObject.FillColor = this.FillColor;
			pageObject.StrokeColor = this.StrokeColor;
			pageObject.Flatness = this.Flatness;
			pageObject.Smoothness = this.Smoothness;
			pageObject.BlendMode = this.BlendMode;
			pageObject.RenderIntent = this.RenderIntent;
			pageObject.OverprintMode = this.OverprintMode;
			pageObject.StrokeOverprint = this.StrokeOverprint;
			pageObject.FillOverprint = this.FillOverprint;
			pageObject.AlphaShape = this.AlphaShape;
			pageObject.Matrix = this.Matrix;
			if (pageObject.MarkedContent != null)
			{
				pageObject.MarkedContent.Clear();
			}
			if (this.MarkedContent != null && this.MarkedContent.Count > 0)
			{
				foreach (MarkedContentModel markedContentModel in this.MarkedContent)
				{
					pageObject.MarkedContent.Add(markedContentModel.ToMarkedContent());
				}
			}
			this.ApplyCore(pageObject);
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x0004FCB8 File Offset: 0x0004DEB8
		protected virtual void ApplyCore(PdfPageObject pageObject)
		{
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x0004FCBA File Offset: 0x0004DEBA
		public static void InitModelProperties(PdfPageObject pageObject, PageBaseObject model)
		{
			if (pageObject == null || model == null)
			{
				return;
			}
			model.Init(pageObject);
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x0004FCCC File Offset: 0x0004DECC
		public bool Equals(PageBaseObject other)
		{
			return other != null && this.FillColor == other.FillColor && this.StrokeColor == other.StrokeColor && this.Flatness == other.Flatness && this.Smoothness == other.Smoothness && this.BlendMode == other.BlendMode && this.RenderIntent == other.RenderIntent && this.OverprintMode == other.OverprintMode && this.StrokeOverprint == other.StrokeOverprint && this.FillOverprint == other.FillOverprint && this.AlphaShape == other.AlphaShape && PageBaseObject.FsMatrixEquals(this.Matrix, other.Matrix) && PageBaseObject.CollectionEqual<MarkedContentModel>(this.MarkedContent, other.MarkedContent) && this.EqualsCore(other);
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x0004FDB2 File Offset: 0x0004DFB2
		protected virtual bool EqualsCore(PageBaseObject other)
		{
			return true;
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x0004FDB8 File Offset: 0x0004DFB8
		public static bool CollectionEqual<T>(global::System.Collections.Generic.IReadOnlyList<T> first, global::System.Collections.Generic.IReadOnlyList<T> second)
		{
			if (first == second)
			{
				return true;
			}
			bool flag = first == null || first.Count == 0;
			bool flag2 = second == null || second.Count == 0;
			return (flag && flag2) || (!flag && !flag2 && first.Count == second.Count && first.SequenceEqual(second));
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x0004FE10 File Offset: 0x0004E010
		public static bool CollectionEqual<TCollection, T>(global::System.Collections.Generic.IReadOnlyList<TCollection> first, global::System.Collections.Generic.IReadOnlyList<TCollection> second) where TCollection : global::System.Collections.Generic.IReadOnlyList<T>
		{
			if (first == second)
			{
				return true;
			}
			bool flag = first == null || first.Count == 0;
			bool flag2 = second == null || second.Count == 0;
			if (flag && flag2)
			{
				return true;
			}
			if (flag || flag2)
			{
				return false;
			}
			if (first.Count != second.Count)
			{
				return false;
			}
			bool flag3 = true;
			for (int i = 0; i < first.Count; i++)
			{
				flag3 &= PageBaseObject.CollectionEqual<T>(first[i], second[i]);
				if (!flag3)
				{
					break;
				}
			}
			return flag3;
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x0004FE98 File Offset: 0x0004E098
		protected static T[] ReturnArrayOrEmpty<T>(Func<T[]> action)
		{
			try
			{
				return ((action != null) ? action() : null) ?? Array.Empty<T>();
			}
			catch
			{
			}
			return Array.Empty<T>();
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x0004FED8 File Offset: 0x0004E0D8
		protected static T ReturnValueOrDefault<T>(Func<T> action)
		{
			try
			{
				if (action != null)
				{
					T t = action();
					if (typeof(T).IsValueType || !object.Equals(t, null))
					{
						return t;
					}
				}
			}
			catch
			{
			}
			return default(T);
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x0004FF34 File Offset: 0x0004E134
		private static bool FsMatrixEquals(FS_MATRIX matrix1, FS_MATRIX matrix2)
		{
			return (matrix1 == null && matrix2 == null) || (matrix1 != null && matrix2 != null && (matrix1.a == matrix2.a && matrix1.b == matrix2.b && matrix1.c == matrix2.c && matrix1.d == matrix2.d && matrix1.e == matrix2.e) && matrix1.f == matrix2.f);
		}
	}
}
