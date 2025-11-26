using System;
using System.Collections.Generic;
using System.Linq;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.BasicTypes;
using pdfeditor.Models.Menus;
using pdfeditor.ViewModels;
using PDFKit.Utils;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x0200019A RID: 410
	public abstract class BaseAnnotation : IEquatable<BaseAnnotation>
	{
		// Token: 0x17000946 RID: 2374
		// (get) Token: 0x0600176A RID: 5994 RVA: 0x0005A2AC File Offset: 0x000584AC
		// (set) Token: 0x0600176B RID: 5995 RVA: 0x0005A2B4 File Offset: 0x000584B4
		public string AnnotationType { get; protected set; }

		// Token: 0x17000947 RID: 2375
		// (get) Token: 0x0600176C RID: 5996 RVA: 0x0005A2BD File Offset: 0x000584BD
		// (set) Token: 0x0600176D RID: 5997 RVA: 0x0005A2C5 File Offset: 0x000584C5
		public int PageIndex { get; protected set; }

		// Token: 0x17000948 RID: 2376
		// (get) Token: 0x0600176E RID: 5998 RVA: 0x0005A2CE File Offset: 0x000584CE
		// (set) Token: 0x0600176F RID: 5999 RVA: 0x0005A2D6 File Offset: 0x000584D6
		public int AnnotIndex { get; internal set; }

		// Token: 0x17000949 RID: 2377
		// (get) Token: 0x06001770 RID: 6000 RVA: 0x0005A2DF File Offset: 0x000584DF
		// (set) Token: 0x06001771 RID: 6001 RVA: 0x0005A2E7 File Offset: 0x000584E7
		public AnnotationFlags Flags { get; protected set; }

		// Token: 0x1700094A RID: 2378
		// (get) Token: 0x06001772 RID: 6002 RVA: 0x0005A2F0 File Offset: 0x000584F0
		// (set) Token: 0x06001773 RID: 6003 RVA: 0x0005A2F8 File Offset: 0x000584F8
		public FS_COLOR Color { get; protected set; }

		// Token: 0x1700094B RID: 2379
		// (get) Token: 0x06001774 RID: 6004 RVA: 0x0005A301 File Offset: 0x00058501
		// (set) Token: 0x06001775 RID: 6005 RVA: 0x0005A309 File Offset: 0x00058509
		public string Contents { get; protected set; }

		// Token: 0x1700094C RID: 2380
		// (get) Token: 0x06001776 RID: 6006 RVA: 0x0005A312 File Offset: 0x00058512
		// (set) Token: 0x06001777 RID: 6007 RVA: 0x0005A31A File Offset: 0x0005851A
		public string Name { get; protected set; }

		// Token: 0x1700094D RID: 2381
		// (get) Token: 0x06001778 RID: 6008 RVA: 0x0005A323 File Offset: 0x00058523
		// (set) Token: 0x06001779 RID: 6009 RVA: 0x0005A32B File Offset: 0x0005852B
		public string ModificationDate { get; protected set; }

		// Token: 0x1700094E RID: 2382
		// (get) Token: 0x0600177A RID: 6010 RVA: 0x0005A334 File Offset: 0x00058534
		// (set) Token: 0x0600177B RID: 6011 RVA: 0x0005A33C File Offset: 0x0005853C
		public FS_RECTF Rectangle { get; internal set; }

		// Token: 0x0600177C RID: 6012 RVA: 0x0005A348 File Offset: 0x00058548
		protected virtual void Init(PdfAnnotation pdfAnnotation)
		{
			PdfTypeBase pdfTypeBase;
			if (pdfAnnotation.Dictionary.TryGetValue("Subtype", out pdfTypeBase))
			{
				PdfTypeName pdfTypeName = pdfTypeBase as PdfTypeName;
				if (pdfTypeName != null)
				{
					this.AnnotationType = pdfTypeName.Value;
					goto IL_0054;
				}
			}
			this.AnnotationType = pdfAnnotation.GetType().FullName;
			IL_0054:
			this.PageIndex = pdfAnnotation.Page.PageIndex;
			this.AnnotIndex = pdfAnnotation.Page.Annots.IndexOf(pdfAnnotation);
			this.Flags = BaseAnnotation.ReturnValueOrDefault<AnnotationFlags>(() => pdfAnnotation.Flags);
			this.Color = BaseAnnotation.ReturnValueOrDefault<FS_COLOR>(() => pdfAnnotation.Color);
			this.Contents = BaseAnnotation.ReturnValueOrDefault<string>(() => pdfAnnotation.Contents);
			this.Name = BaseAnnotation.ReturnValueOrDefault<string>(() => pdfAnnotation.Name);
			this.ModificationDate = BaseAnnotation.ReturnValueOrDefault<string>(() => pdfAnnotation.ModificationDate);
			this.Rectangle = BaseAnnotation.ReturnValueOrDefault<FS_RECTF>(() => pdfAnnotation.GetRECT());
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x0005A46C File Offset: 0x0005866C
		public void Apply(PdfAnnotation annot)
		{
			if (annot == null)
			{
				throw new ArgumentNullException("annot");
			}
			annot.Flags = this.Flags;
			annot.Color = this.Color;
			annot.Contents = this.Contents;
			annot.Name = this.Name;
			annot.ModificationDate = this.ModificationDate;
			this.ApplyCore(annot);
		}

		// Token: 0x0600177E RID: 6014 RVA: 0x0005A4CA File Offset: 0x000586CA
		protected virtual void ApplyCore(PdfAnnotation annot)
		{
		}

		// Token: 0x0600177F RID: 6015 RVA: 0x0005A4CC File Offset: 0x000586CC
		public virtual object GetValue(AnnotationMode mode, ContextMenuItemType type)
		{
			return null;
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x0005A4CF File Offset: 0x000586CF
		public static void InitModelProperties(PdfAnnotation annot, BaseAnnotation model)
		{
			if (annot == null || model == null)
			{
				return;
			}
			model.Init(annot);
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x0005A4E8 File Offset: 0x000586E8
		public bool Equals(BaseAnnotation other)
		{
			return other != null && (this.Flags == other.Flags && this.Color == other.Color && this.Contents == other.Contents && this.Name == other.Name && this.ModificationDate == other.ModificationDate) && this.EqualsCore(other);
		}

		// Token: 0x06001782 RID: 6018 RVA: 0x0005A55F File Offset: 0x0005875F
		protected virtual bool EqualsCore(BaseAnnotation other)
		{
			return true;
		}

		// Token: 0x06001783 RID: 6019 RVA: 0x0005A564 File Offset: 0x00058764
		public static bool CollectionEqual<T>(IReadOnlyList<T> first, IReadOnlyList<T> second)
		{
			if (first == second)
			{
				return true;
			}
			bool flag = first == null || first.Count == 0;
			bool flag2 = second == null || second.Count == 0;
			return (flag && flag2) || (!flag && !flag2 && first.Count == second.Count && first.SequenceEqual(second));
		}

		// Token: 0x06001784 RID: 6020 RVA: 0x0005A5BC File Offset: 0x000587BC
		public static bool CollectionEqual<TCollection, T>(IReadOnlyList<TCollection> first, IReadOnlyList<TCollection> second) where TCollection : IReadOnlyList<T>
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
				flag3 &= BaseAnnotation.CollectionEqual<T>(first[i], second[i]);
				if (!flag3)
				{
					break;
				}
			}
			return flag3;
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x0005A644 File Offset: 0x00058844
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

		// Token: 0x06001786 RID: 6022 RVA: 0x0005A684 File Offset: 0x00058884
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
	}
}
