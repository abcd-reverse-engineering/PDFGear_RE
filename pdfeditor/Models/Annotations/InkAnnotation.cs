using System;
using System.Collections.Generic;
using System.Linq;
using Patagames.Pdf;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Models.Menus;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x0200019E RID: 414
	public class InkAnnotation : BaseMarkupAnnotation
	{
		// Token: 0x1700095D RID: 2397
		// (get) Token: 0x060017AE RID: 6062 RVA: 0x0005AC9E File Offset: 0x00058E9E
		// (set) Token: 0x060017AF RID: 6063 RVA: 0x0005ACA6 File Offset: 0x00058EA6
		public PdfBorderStyleModel LineStyle { get; protected set; }

		// Token: 0x1700095E RID: 2398
		// (get) Token: 0x060017B0 RID: 6064 RVA: 0x0005ACAF File Offset: 0x00058EAF
		// (set) Token: 0x060017B1 RID: 6065 RVA: 0x0005ACB7 File Offset: 0x00058EB7
		public IReadOnlyList<IReadOnlyList<FS_POINTF>> InkList { get; internal set; }

		// Token: 0x060017B2 RID: 6066 RVA: 0x0005ACC0 File Offset: 0x00058EC0
		protected override void Init(PdfAnnotation pdfAnnotation)
		{
			base.Init(pdfAnnotation);
			PdfInkAnnotation annot = pdfAnnotation as PdfInkAnnotation;
			if (annot != null)
			{
				this.LineStyle = BaseAnnotation.ReturnValueOrDefault<PdfBorderStyleModel>(delegate
				{
					if (!(annot.LineStyle != null))
					{
						return null;
					}
					return new PdfBorderStyleModel(annot.LineStyle.Width, annot.LineStyle.Style, annot.LineStyle.DashPattern);
				});
				this.InkList = BaseAnnotation.ReturnArrayOrEmpty<FS_POINTF[]>(delegate
				{
					PdfInkPointCollection inkList = annot.InkList;
					if (inkList == null)
					{
						return null;
					}
					return inkList.Select((PdfLinePointCollection<PdfInkAnnotation> c) => BaseAnnotation.ReturnArrayOrEmpty<FS_POINTF>(() => c.ToArray<FS_POINTF>())).ToArray<FS_POINTF[]>();
				});
			}
		}

		// Token: 0x060017B3 RID: 6067 RVA: 0x0005AD1C File Offset: 0x00058F1C
		protected override void ApplyCore(PdfAnnotation annot)
		{
			base.ApplyCore(annot);
			PdfInkAnnotation pdfInkAnnotation = annot as PdfInkAnnotation;
			if (pdfInkAnnotation != null)
			{
				if (this.LineStyle == null)
				{
					pdfInkAnnotation.LineStyle = null;
				}
				else
				{
					if (pdfInkAnnotation.LineStyle == null)
					{
						pdfInkAnnotation.LineStyle = new PdfBorderStyle();
					}
					pdfInkAnnotation.LineStyle.Width = this.LineStyle.Width;
					pdfInkAnnotation.LineStyle.DashPattern = this.LineStyle.DashPattern.ToArray<float>();
					pdfInkAnnotation.LineStyle.Style = this.LineStyle.Style;
				}
				if (pdfInkAnnotation.InkList == null)
				{
					pdfInkAnnotation.InkList = new PdfInkPointCollection();
				}
				else
				{
					pdfInkAnnotation.InkList.Clear();
				}
				if (this.InkList != null)
				{
					foreach (IEnumerable<FS_POINTF> enumerable in this.InkList)
					{
						PdfLinePointCollection<PdfInkAnnotation> pdfLinePointCollection = new PdfLinePointCollection<PdfInkAnnotation>();
						pdfInkAnnotation.InkList.Add(pdfLinePointCollection);
						foreach (FS_POINTF fs_POINTF in enumerable)
						{
							pdfLinePointCollection.Add(fs_POINTF);
						}
					}
				}
			}
		}

		// Token: 0x060017B4 RID: 6068 RVA: 0x0005AE58 File Offset: 0x00059058
		protected override bool EqualsCore(BaseAnnotation other)
		{
			if (base.EqualsCore(other))
			{
				InkAnnotation inkAnnotation = other as InkAnnotation;
				if (inkAnnotation != null && EqualityComparer<PdfBorderStyleModel>.Default.Equals(inkAnnotation.LineStyle, this.LineStyle))
				{
					return BaseAnnotation.CollectionEqual<IReadOnlyList<FS_POINTF>, FS_POINTF>(inkAnnotation.InkList, this.InkList);
				}
			}
			return false;
		}

		// Token: 0x060017B5 RID: 6069 RVA: 0x0005AEA4 File Offset: 0x000590A4
		public override object GetValue(AnnotationMode mode, ContextMenuItemType type)
		{
			if (mode != AnnotationMode.Ink)
			{
				return base.GetValue(mode, type);
			}
			if (type == ContextMenuItemType.StrokeColor)
			{
				return base.Color;
			}
			if (type != ContextMenuItemType.StrokeThickness)
			{
				return null;
			}
			PdfBorderStyleModel lineStyle = this.LineStyle;
			return (lineStyle != null) ? lineStyle.Width : 1f;
		}
	}
}
