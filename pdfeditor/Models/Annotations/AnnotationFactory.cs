using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Annotations;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x02000194 RID: 404
	public static class AnnotationFactory
	{
		// Token: 0x17000937 RID: 2359
		// (get) Token: 0x0600173C RID: 5948 RVA: 0x00059090 File Offset: 0x00057290
		private static Dictionary<Type, AnnotationFactory.AnnotationTypeData> AnnotTypes
		{
			get
			{
				if (AnnotationFactory.annotTypes == null)
				{
					Type typeFromHandle = typeof(AnnotationFactory);
					lock (typeFromHandle)
					{
						if (AnnotationFactory.annotTypes == null)
						{
							AnnotationFactory.InitAnnotTypes();
						}
					}
				}
				return AnnotationFactory.annotTypes;
			}
		}

		// Token: 0x17000938 RID: 2360
		// (get) Token: 0x0600173D RID: 5949 RVA: 0x000590E8 File Offset: 0x000572E8
		private static Dictionary<Type, AnnotationFactory.AnnotationTypeData> PdfAnnotTypes
		{
			get
			{
				if (AnnotationFactory.pdfAnnotTypes == null)
				{
					Type typeFromHandle = typeof(AnnotationFactory);
					lock (typeFromHandle)
					{
						if (AnnotationFactory.pdfAnnotTypes == null)
						{
							AnnotationFactory.InitAnnotTypes();
						}
					}
				}
				return AnnotationFactory.pdfAnnotTypes;
			}
		}

		// Token: 0x17000939 RID: 2361
		// (get) Token: 0x0600173E RID: 5950 RVA: 0x00059140 File Offset: 0x00057340
		private static Dictionary<AnnotationMode, AnnotationFactory.AnnotationTypeData> AnnotModeData
		{
			get
			{
				if (AnnotationFactory.annotModeData == null)
				{
					Type typeFromHandle = typeof(AnnotationFactory);
					lock (typeFromHandle)
					{
						if (AnnotationFactory.annotModeData == null)
						{
							AnnotationFactory.InitAnnotTypes();
						}
					}
				}
				return AnnotationFactory.annotModeData;
			}
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x00059198 File Offset: 0x00057398
		private static void InitAnnotTypes()
		{
			if (AnnotationFactory.annotTypes == null)
			{
				Dictionary<Type, AnnotationFactory.AnnotationTypeData> dictionary = new Dictionary<Type, AnnotationFactory.AnnotationTypeData>();
				Type typeFromHandle = typeof(PopupAnnotation);
				dictionary[typeFromHandle] = new AnnotationFactory.AnnotationTypeData(typeof(PopupAnnotation), typeof(PdfPopupAnnotation), AnnotationMode.Popup, () => AnnotationFactory.CreateInstance<PopupAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfPopupAnnotation(p));
				Type typeFromHandle2 = typeof(UnderlineAnnotation);
				dictionary[typeFromHandle2] = new AnnotationFactory.AnnotationTypeData(typeof(UnderlineAnnotation), typeof(PdfUnderlineAnnotation), AnnotationMode.Underline, () => AnnotationFactory.CreateInstance<UnderlineAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfUnderlineAnnotation(p));
				Type typeFromHandle3 = typeof(StrikeoutAnnotation);
				dictionary[typeFromHandle3] = new AnnotationFactory.AnnotationTypeData(typeof(StrikeoutAnnotation), typeof(PdfStrikeoutAnnotation), AnnotationMode.Strike, () => AnnotationFactory.CreateInstance<StrikeoutAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfStrikeoutAnnotation(p));
				Type typeFromHandle4 = typeof(HighlightAnnotation);
				dictionary[typeFromHandle4] = new AnnotationFactory.AnnotationTypeData(typeof(HighlightAnnotation), typeof(PdfHighlightAnnotation), new AnnotationMode[]
				{
					AnnotationMode.Highlight,
					AnnotationMode.HighlightArea
				}, () => AnnotationFactory.CreateInstance<HighlightAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfHighlightAnnotation(p), delegate(PdfAnnotation a)
				{
					PdfMarkupAnnotation pdfMarkupAnnotation = a as PdfMarkupAnnotation;
					if (pdfMarkupAnnotation == null || !(pdfMarkupAnnotation.Subject == "AreaHighlight"))
					{
						return AnnotationMode.Highlight;
					}
					return AnnotationMode.HighlightArea;
				}, delegate(BaseAnnotation a)
				{
					BaseMarkupAnnotation baseMarkupAnnotation = a as BaseMarkupAnnotation;
					if (baseMarkupAnnotation == null || !(baseMarkupAnnotation.Subject == "AreaHighlight"))
					{
						return AnnotationMode.Highlight;
					}
					return AnnotationMode.HighlightArea;
				});
				Type typeFromHandle5 = typeof(LineAnnotation);
				dictionary[typeFromHandle5] = new AnnotationFactory.AnnotationTypeData(typeof(LineAnnotation), typeof(PdfLineAnnotation), AnnotationMode.Line, () => AnnotationFactory.CreateInstance<LineAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfLineAnnotation(p));
				Type typeFromHandle6 = typeof(FreeTextAnnotation);
				dictionary[typeFromHandle6] = new AnnotationFactory.AnnotationTypeData(typeof(FreeTextAnnotation), typeof(PdfFreeTextAnnotation), new AnnotationMode[]
				{
					AnnotationMode.TextBox,
					AnnotationMode.Text
				}, () => AnnotationFactory.CreateInstance<FreeTextAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfFreeTextAnnotation(p), delegate(PdfAnnotation a)
				{
					PdfFreeTextAnnotation pdfFreeTextAnnotation = a as PdfFreeTextAnnotation;
					if (pdfFreeTextAnnotation == null || pdfFreeTextAnnotation.Intent != AnnotationIntent.FreeTextTypeWriter)
					{
						return AnnotationMode.TextBox;
					}
					return AnnotationMode.Text;
				}, delegate(BaseAnnotation a)
				{
					FreeTextAnnotation freeTextAnnotation = a as FreeTextAnnotation;
					if (freeTextAnnotation == null || freeTextAnnotation.Intent != AnnotationIntent.FreeTextTypeWriter)
					{
						return AnnotationMode.TextBox;
					}
					return AnnotationMode.Text;
				});
				Type typeFromHandle7 = typeof(InkAnnotation);
				dictionary[typeFromHandle7] = new AnnotationFactory.AnnotationTypeData(typeof(InkAnnotation), typeof(PdfInkAnnotation), AnnotationMode.Ink, () => AnnotationFactory.CreateInstance<InkAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfInkAnnotation(p));
				Type typeFromHandle8 = typeof(SquareAnnotation);
				dictionary[typeFromHandle8] = new AnnotationFactory.AnnotationTypeData(typeof(SquareAnnotation), typeof(PdfSquareAnnotation), AnnotationMode.Shape, () => AnnotationFactory.CreateInstance<SquareAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfSquareAnnotation(p));
				Type typeFromHandle9 = typeof(LinkAnnotation);
				dictionary[typeFromHandle9] = new AnnotationFactory.AnnotationTypeData(typeof(LinkAnnotation), typeof(PdfLinkAnnotation), AnnotationMode.Link, () => AnnotationFactory.CreateInstance<LinkAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfLinkAnnotation(p));
				Type typeFromHandle10 = typeof(CircleAnnotation);
				dictionary[typeFromHandle10] = new AnnotationFactory.AnnotationTypeData(typeof(CircleAnnotation), typeof(PdfCircleAnnotation), AnnotationMode.Ellipse, () => AnnotationFactory.CreateInstance<CircleAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfCircleAnnotation(p));
				Type typeFromHandle11 = typeof(TextAnnotation);
				dictionary[typeFromHandle11] = new AnnotationFactory.AnnotationTypeData(typeof(TextAnnotation), typeof(PdfTextAnnotation), AnnotationMode.Note, () => AnnotationFactory.CreateInstance<TextAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfTextAnnotation(p));
				Type typeFromHandle12 = typeof(StampAnnotation);
				dictionary[typeFromHandle12] = new AnnotationFactory.AnnotationTypeData(typeof(StampAnnotation), typeof(PdfStampAnnotation), new AnnotationMode[]
				{
					AnnotationMode.Stamp,
					AnnotationMode.Signature
				}, () => AnnotationFactory.CreateInstance<StampAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfStampAnnotation(p), (PdfAnnotation a) => AnnotationMode.Stamp, (BaseAnnotation a) => AnnotationMode.Stamp);
				Type typeFromHandle13 = typeof(WatermarkAnnotation);
				dictionary[typeFromHandle13] = new AnnotationFactory.AnnotationTypeData(typeof(WatermarkAnnotation), typeof(PdfWatermarkAnnotation), AnnotationMode.Watermark, () => AnnotationFactory.CreateInstance<WatermarkAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfWatermarkAnnotation(p));
				Type typeFromHandle14 = typeof(AttachmentAnnotation);
				dictionary[typeFromHandle14] = new AnnotationFactory.AnnotationTypeData(typeof(AttachmentAnnotation), typeof(PdfFileAttachmentAnnotation), AnnotationMode.Attachment, () => AnnotationFactory.CreateInstance<AttachmentAnnotation>(), (PdfPage p, BaseAnnotation a) => new PdfFileAttachmentAnnotation(p));
				AnnotationFactory.annotTypes = dictionary;
			}
			if (AnnotationFactory.pdfAnnotTypes == null)
			{
				AnnotationFactory.pdfAnnotTypes = (from c in AnnotationFactory.annotTypes
					select c.Value into c
					where c.PdfAnnotationType != null
					group c by c.PdfAnnotationType into c
					where c.Count<AnnotationFactory.AnnotationTypeData>() > 0
					select c).ToDictionary((IGrouping<Type, AnnotationFactory.AnnotationTypeData> c) => c.Key, (IGrouping<Type, AnnotationFactory.AnnotationTypeData> c) => c.First<AnnotationFactory.AnnotationTypeData>());
			}
			if (AnnotationFactory.annotModeData == null)
			{
				AnnotationFactory.annotModeData = (from c in AnnotationFactory.annotTypes
					from mode in c.Value.Modes
					select new global::System.ValueTuple<AnnotationMode, AnnotationFactory.AnnotationTypeData>(mode, c.Value) into c
					group c.Item2 by c.Item1 into c
					where c.Count<AnnotationFactory.AnnotationTypeData>() > 0
					select c).ToDictionary((IGrouping<AnnotationMode, AnnotationFactory.AnnotationTypeData> c) => c.Key, (IGrouping<AnnotationMode, AnnotationFactory.AnnotationTypeData> c) => c.First<AnnotationFactory.AnnotationTypeData>());
			}
		}

		// Token: 0x06001740 RID: 5952 RVA: 0x00059A70 File Offset: 0x00057C70
		private static bool TryGetAnnotationDataCore(Type annotModeType, Type pdfAnnotType, out AnnotationFactory.AnnotationTypeData data)
		{
			data = null;
			if (annotModeType == null && pdfAnnotType == null)
			{
				return false;
			}
			if (annotModeType != null)
			{
				return AnnotationFactory.AnnotTypes.TryGetValue(annotModeType, out data);
			}
			return pdfAnnotType != null && AnnotationFactory.PdfAnnotTypes.TryGetValue(pdfAnnotType, out data);
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x00059AC1 File Offset: 0x00057CC1
		private static bool TryGetAnnotationDataFromModel<T>(out AnnotationFactory.AnnotationTypeData data) where T : BaseAnnotation
		{
			return AnnotationFactory.TryGetAnnotationDataCore(typeof(T), null, out data);
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x00059AD4 File Offset: 0x00057CD4
		private static bool TryGetAnnotationDataFromPdfType<T>(out AnnotationFactory.AnnotationTypeData data) where T : PdfAnnotation
		{
			return AnnotationFactory.TryGetAnnotationDataCore(null, typeof(T), out data);
		}

		// Token: 0x06001743 RID: 5955 RVA: 0x00059AE8 File Offset: 0x00057CE8
		public static BaseAnnotation Create(PdfAnnotation pdfAnnotation)
		{
			if (pdfAnnotation == null)
			{
				return null;
			}
			BaseAnnotation baseAnnotation = AnnotationFactory.CreateInstance(pdfAnnotation);
			BaseAnnotation.InitModelProperties(pdfAnnotation, baseAnnotation);
			return baseAnnotation;
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x00059B10 File Offset: 0x00057D10
		public static PdfAnnotation Create(PdfPage page, BaseAnnotation annotation)
		{
			if (annotation == null)
			{
				return null;
			}
			PdfAnnotation pdfAnnotation = null;
			AnnotationFactory.AnnotationTypeData annotationTypeData;
			if (AnnotationFactory.TryGetAnnotationDataCore(annotation.GetType(), null, out annotationTypeData))
			{
				pdfAnnotation = annotationTypeData.Create(page, annotation);
			}
			pdfAnnotation == null;
			if (pdfAnnotation != null)
			{
				annotation.Apply(pdfAnnotation);
			}
			return pdfAnnotation;
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x00059B58 File Offset: 0x00057D58
		public static global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> Create(PdfPage page)
		{
			if (page == null)
			{
				throw new ArgumentNullException("page");
			}
			if (page.Annots == null)
			{
				page.CreateAnnotations();
			}
			if (page.Annots == null || page.Annots.Count == 0)
			{
				return Array.Empty<BaseAnnotation>();
			}
			BaseAnnotation[] array = page.Annots.Select((PdfAnnotation c) => AnnotationFactory.Create(c)).ToArray<BaseAnnotation>();
			AnnotationFactory.CreateRelations(page.Annots, array);
			return array;
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x00059BDC File Offset: 0x00057DDC
		public static global::System.Collections.Generic.IReadOnlyList<AnnotationMode> GetAnnotationModes(BaseAnnotation baseAnnotation)
		{
			AnnotationFactory.AnnotationTypeData annotationTypeData;
			if (!AnnotationFactory.TryGetAnnotationDataCore((baseAnnotation != null) ? baseAnnotation.GetType() : null, null, out annotationTypeData))
			{
				return Array.Empty<AnnotationMode>();
			}
			AnnotationMode mode = annotationTypeData.GetAnnotationMode(baseAnnotation);
			if (mode == AnnotationMode.None)
			{
				return annotationTypeData.Modes;
			}
			List<AnnotationMode> list = annotationTypeData.Modes.Where((AnnotationMode c) => c != mode).ToList<AnnotationMode>();
			list.Insert(0, mode);
			return list;
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x00059C54 File Offset: 0x00057E54
		public static global::System.Collections.Generic.IReadOnlyList<AnnotationMode> GetAnnotationModes(PdfAnnotation selectedAnnotation)
		{
			AnnotationFactory.AnnotationTypeData annotationTypeData;
			if (!AnnotationFactory.TryGetAnnotationDataCore(null, (selectedAnnotation != null) ? selectedAnnotation.GetType() : null, out annotationTypeData))
			{
				return Array.Empty<AnnotationMode>();
			}
			AnnotationMode mode = annotationTypeData.GetAnnotationMode(selectedAnnotation);
			if (mode == AnnotationMode.None)
			{
				return annotationTypeData.Modes;
			}
			List<AnnotationMode> list = annotationTypeData.Modes.Where((AnnotationMode c) => c != mode).ToList<AnnotationMode>();
			list.Insert(0, mode);
			return list;
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x00059CCC File Offset: 0x00057ECC
		private static void CreateRelations(PdfAnnotationCollection source, BaseAnnotation[] annotations)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (annotations == null)
			{
				throw new ArgumentNullException("annotations");
			}
			if (source.Count != annotations.Length)
			{
				throw new ArgumentException();
			}
			if (annotations.Length == 0)
			{
				return;
			}
			for (int i = 0; i < source.Count; i++)
			{
				PdfMarkupAnnotation pdfMarkupAnnotation = source[i] as PdfMarkupAnnotation;
				if (pdfMarkupAnnotation != null)
				{
					if (pdfMarkupAnnotation.RelationshipAnnotation != null)
					{
						int num = source.IndexOf(pdfMarkupAnnotation.RelationshipAnnotation);
						if (num >= 0)
						{
							(annotations[i] as BaseMarkupAnnotation).RelationshipAnnotation = annotations[num];
						}
					}
					if (pdfMarkupAnnotation.Popup != null)
					{
						int num2 = source.IndexOf(pdfMarkupAnnotation.Popup);
						if (num2 >= 0)
						{
							(annotations[i] as BaseMarkupAnnotation).Popup = (PopupAnnotation)annotations[num2];
							(annotations[num2] as PopupAnnotation).Parent = annotations[i];
						}
					}
				}
			}
		}

		// Token: 0x06001749 RID: 5961 RVA: 0x00059DA8 File Offset: 0x00057FA8
		private static BaseAnnotation CreateInstance(PdfAnnotation pdfAnnotation)
		{
			if (pdfAnnotation == null)
			{
				return null;
			}
			AnnotationFactory.AnnotationTypeData annotationTypeData;
			if (AnnotationFactory.TryGetAnnotationDataCore(null, (pdfAnnotation != null) ? pdfAnnotation.GetType() : null, out annotationTypeData))
			{
				return annotationTypeData.CreateModel();
			}
			if (pdfAnnotation is PdfMarkupAnnotation)
			{
				return AnnotationFactory.CreateInstance<NotImplementedMarkupAnnotation>();
			}
			return AnnotationFactory.CreateInstance<NotImplementedAnnotation>();
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x00059DF0 File Offset: 0x00057FF0
		private static T CreateInstance<T>() where T : new()
		{
			return new T();
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x00059DF8 File Offset: 0x00057FF8
		[Conditional("DEBUG")]
		public static void ValidRelationship(global::System.Collections.Generic.IReadOnlyList<BaseAnnotation> annotations)
		{
			if (annotations == null)
			{
				return;
			}
			for (int i = 0; i < annotations.Count; i++)
			{
				BaseAnnotation baseAnnotation = annotations[i];
				BaseMarkupAnnotation baseMarkupAnnotation = baseAnnotation as BaseMarkupAnnotation;
				if (baseMarkupAnnotation != null)
				{
					if (baseMarkupAnnotation.RelationshipAnnotation != null && !annotations.Contains(baseMarkupAnnotation.RelationshipAnnotation))
					{
						throw new ArgumentException();
					}
					if (baseMarkupAnnotation.Popup != null && !annotations.Contains(baseMarkupAnnotation.Popup))
					{
						throw new ArgumentException();
					}
				}
				PopupAnnotation popupAnnotation = baseAnnotation as PopupAnnotation;
				if (popupAnnotation != null && popupAnnotation.Parent != null)
				{
					annotations.Contains(popupAnnotation.Parent);
				}
			}
		}

		// Token: 0x040007BE RID: 1982
		private static Dictionary<Type, AnnotationFactory.AnnotationTypeData> annotTypes;

		// Token: 0x040007BF RID: 1983
		private static Dictionary<Type, AnnotationFactory.AnnotationTypeData> pdfAnnotTypes;

		// Token: 0x040007C0 RID: 1984
		private static Dictionary<AnnotationMode, AnnotationFactory.AnnotationTypeData> annotModeData;

		// Token: 0x020005AA RID: 1450
		private class AnnotationTypeData
		{
			// Token: 0x060031D3 RID: 12755 RVA: 0x000F39FC File Offset: 0x000F1BFC
			public AnnotationTypeData(Type annotationModeType, Type pdfAnnotationType, AnnotationMode mode, Func<BaseAnnotation> createModelFunc, Func<PdfPage, BaseAnnotation, PdfAnnotation> createFunc)
			{
				this.AnnotationModeType = annotationModeType;
				this.PdfAnnotationType = pdfAnnotationType;
				this.Modes = new AnnotationMode[] { mode };
				this.createModelFunc = createModelFunc;
				this.createFunc = createFunc;
			}

			// Token: 0x060031D4 RID: 12756 RVA: 0x000F3A32 File Offset: 0x000F1C32
			public AnnotationTypeData(Type annotationModeType, Type pdfAnnotationType, AnnotationMode[] modes, Func<BaseAnnotation> createModelFunc, Func<PdfPage, BaseAnnotation, PdfAnnotation> createFunc, Func<PdfAnnotation, AnnotationMode> getAnnotModeFunc1, Func<BaseAnnotation, AnnotationMode> getAnnotModeFunc2)
			{
				this.AnnotationModeType = annotationModeType;
				this.PdfAnnotationType = pdfAnnotationType;
				this.Modes = modes;
				this.createModelFunc = createModelFunc;
				this.createFunc = createFunc;
				this.getAnnotModeFunc1 = getAnnotModeFunc1;
				this.getAnnotModeFunc2 = getAnnotModeFunc2;
			}

			// Token: 0x17000D34 RID: 3380
			// (get) Token: 0x060031D5 RID: 12757 RVA: 0x000F3A6F File Offset: 0x000F1C6F
			public Type AnnotationModeType { get; }

			// Token: 0x17000D35 RID: 3381
			// (get) Token: 0x060031D6 RID: 12758 RVA: 0x000F3A77 File Offset: 0x000F1C77
			public Type PdfAnnotationType { get; }

			// Token: 0x060031D7 RID: 12759 RVA: 0x000F3A7F File Offset: 0x000F1C7F
			public BaseAnnotation CreateModel()
			{
				return this.createModelFunc();
			}

			// Token: 0x060031D8 RID: 12760 RVA: 0x000F3A8C File Offset: 0x000F1C8C
			public PdfAnnotation Create(PdfPage page, BaseAnnotation annotation)
			{
				return this.createFunc(page, annotation);
			}

			// Token: 0x17000D36 RID: 3382
			// (get) Token: 0x060031D9 RID: 12761 RVA: 0x000F3A9B File Offset: 0x000F1C9B
			public global::System.Collections.Generic.IReadOnlyList<AnnotationMode> Modes { get; }

			// Token: 0x060031DA RID: 12762 RVA: 0x000F3AA4 File Offset: 0x000F1CA4
			public AnnotationMode GetAnnotationMode(PdfAnnotation instance)
			{
				if (this.Modes != null && this.Modes.Count == 1 && this.Modes[0] != AnnotationMode.None)
				{
					return this.Modes[0];
				}
				if (this.getAnnotModeFunc1 != null)
				{
					return this.getAnnotModeFunc1(instance);
				}
				return this.GetDefaultAnnotationModeCore();
			}

			// Token: 0x060031DB RID: 12763 RVA: 0x000F3B00 File Offset: 0x000F1D00
			public AnnotationMode GetAnnotationMode(BaseAnnotation instance)
			{
				if (this.Modes != null && this.Modes.Count == 1 && this.Modes[0] != AnnotationMode.None)
				{
					return this.Modes[0];
				}
				if (this.getAnnotModeFunc2 != null)
				{
					return this.getAnnotModeFunc2(instance);
				}
				return this.GetDefaultAnnotationModeCore();
			}

			// Token: 0x060031DC RID: 12764 RVA: 0x000F3B5C File Offset: 0x000F1D5C
			private AnnotationMode GetDefaultAnnotationModeCore()
			{
				if (this.Modes != null)
				{
					foreach (AnnotationMode annotationMode in this.Modes)
					{
						if (annotationMode != AnnotationMode.None)
						{
							return annotationMode;
						}
					}
					return AnnotationMode.None;
				}
				return AnnotationMode.None;
			}

			// Token: 0x04001EB5 RID: 7861
			private Func<BaseAnnotation> createModelFunc;

			// Token: 0x04001EB6 RID: 7862
			private Func<PdfPage, BaseAnnotation, PdfAnnotation> createFunc;

			// Token: 0x04001EB7 RID: 7863
			private readonly Func<PdfAnnotation, AnnotationMode> getAnnotModeFunc1;

			// Token: 0x04001EB8 RID: 7864
			private readonly Func<BaseAnnotation, AnnotationMode> getAnnotModeFunc2;
		}
	}
}
