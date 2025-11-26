using System;
using System.Collections.Generic;
using System.Linq;
using Patagames.Pdf;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net.Annotations;
using Patagames.Pdf.Net.Wrappers;
using pdfeditor.Models.Menus;
using pdfeditor.ViewModels;
using PDFKit.Utils;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x0200019F RID: 415
	public class LineAnnotation : BaseMarkupAnnotation
	{
		// Token: 0x1700095F RID: 2399
		// (get) Token: 0x060017B7 RID: 6071 RVA: 0x0005AEF9 File Offset: 0x000590F9
		// (set) Token: 0x060017B8 RID: 6072 RVA: 0x0005AF01 File Offset: 0x00059101
		public IReadOnlyList<FS_POINTF> Line { get; internal set; }

		// Token: 0x17000960 RID: 2400
		// (get) Token: 0x060017B9 RID: 6073 RVA: 0x0005AF0A File Offset: 0x0005910A
		// (set) Token: 0x060017BA RID: 6074 RVA: 0x0005AF12 File Offset: 0x00059112
		public IReadOnlyList<LineEndingStyles> LineEnding { get; protected set; }

		// Token: 0x17000961 RID: 2401
		// (get) Token: 0x060017BB RID: 6075 RVA: 0x0005AF1B File Offset: 0x0005911B
		// (set) Token: 0x060017BC RID: 6076 RVA: 0x0005AF23 File Offset: 0x00059123
		public PdfBorderStyleModel LineStyle { get; protected set; }

		// Token: 0x17000962 RID: 2402
		// (get) Token: 0x060017BD RID: 6077 RVA: 0x0005AF2C File Offset: 0x0005912C
		// (set) Token: 0x060017BE RID: 6078 RVA: 0x0005AF34 File Offset: 0x00059134
		public FS_COLOR InteriorColor { get; protected set; }

		// Token: 0x17000963 RID: 2403
		// (get) Token: 0x060017BF RID: 6079 RVA: 0x0005AF3D File Offset: 0x0005913D
		// (set) Token: 0x060017C0 RID: 6080 RVA: 0x0005AF45 File Offset: 0x00059145
		public AnnotationIntent Intent { get; protected set; }

		// Token: 0x17000964 RID: 2404
		// (get) Token: 0x060017C1 RID: 6081 RVA: 0x0005AF4E File Offset: 0x0005914E
		// (set) Token: 0x060017C2 RID: 6082 RVA: 0x0005AF56 File Offset: 0x00059156
		public float LeaderLineLenght { get; protected set; }

		// Token: 0x17000965 RID: 2405
		// (get) Token: 0x060017C3 RID: 6083 RVA: 0x0005AF5F File Offset: 0x0005915F
		// (set) Token: 0x060017C4 RID: 6084 RVA: 0x0005AF67 File Offset: 0x00059167
		public float LeaderLineExtension { get; protected set; }

		// Token: 0x17000966 RID: 2406
		// (get) Token: 0x060017C5 RID: 6085 RVA: 0x0005AF70 File Offset: 0x00059170
		// (set) Token: 0x060017C6 RID: 6086 RVA: 0x0005AF78 File Offset: 0x00059178
		public bool Cap { get; protected set; }

		// Token: 0x17000967 RID: 2407
		// (get) Token: 0x060017C7 RID: 6087 RVA: 0x0005AF81 File Offset: 0x00059181
		// (set) Token: 0x060017C8 RID: 6088 RVA: 0x0005AF89 File Offset: 0x00059189
		public float LeaderLineOffset { get; protected set; }

		// Token: 0x17000968 RID: 2408
		// (get) Token: 0x060017C9 RID: 6089 RVA: 0x0005AF92 File Offset: 0x00059192
		// (set) Token: 0x060017CA RID: 6090 RVA: 0x0005AF9A File Offset: 0x0005919A
		public CaptionPositions CaptionPosition { get; protected set; }

		// Token: 0x17000969 RID: 2409
		// (get) Token: 0x060017CB RID: 6091 RVA: 0x0005AFA3 File Offset: 0x000591A3
		// (set) Token: 0x060017CC RID: 6092 RVA: 0x0005AFAB File Offset: 0x000591AB
		public FS_SIZEF CaptionOffset { get; protected set; }

		// Token: 0x060017CD RID: 6093 RVA: 0x0005AFB4 File Offset: 0x000591B4
		protected override void Init(PdfAnnotation pdfAnnotation)
		{
			base.Init(pdfAnnotation);
			PdfLineAnnotation annot = pdfAnnotation as PdfLineAnnotation;
			if (annot != null)
			{
				this.Line = BaseAnnotation.ReturnArrayOrEmpty<FS_POINTF>(delegate
				{
					IReadOnlyList<FS_POINTF> line = annot.GetLine();
					if (line == null)
					{
						return null;
					}
					return line.ToArray<FS_POINTF>();
				});
				this.LineEnding = BaseAnnotation.ReturnArrayOrEmpty<LineEndingStyles>(delegate
				{
					PdfLineEndingCollection lineEnding = annot.LineEnding;
					if (lineEnding == null)
					{
						return null;
					}
					return lineEnding.ToArray<LineEndingStyles>();
				});
				this.LineStyle = BaseAnnotation.ReturnValueOrDefault<PdfBorderStyleModel>(delegate
				{
					if (!(annot.LineStyle != null))
					{
						return null;
					}
					return new PdfBorderStyleModel(annot.LineStyle.Width, annot.LineStyle.Style, annot.LineStyle.DashPattern);
				});
				this.InteriorColor = BaseAnnotation.ReturnValueOrDefault<FS_COLOR>(() => annot.InteriorColor);
				this.Intent = BaseAnnotation.ReturnValueOrDefault<AnnotationIntent>(() => annot.Intent);
				this.LeaderLineLenght = BaseAnnotation.ReturnValueOrDefault<float>(() => annot.LeaderLineLenght);
				this.LeaderLineExtension = BaseAnnotation.ReturnValueOrDefault<float>(() => annot.LeaderLineExtension);
				this.Cap = BaseAnnotation.ReturnValueOrDefault<bool>(() => annot.Cap);
				this.LeaderLineOffset = BaseAnnotation.ReturnValueOrDefault<float>(() => annot.LeaderLineOffset);
				this.CaptionPosition = BaseAnnotation.ReturnValueOrDefault<CaptionPositions>(() => annot.CaptionPosition);
				this.CaptionOffset = BaseAnnotation.ReturnValueOrDefault<FS_SIZEF>(() => annot.CaptionOffset);
			}
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x0005B0E4 File Offset: 0x000592E4
		protected override void ApplyCore(PdfAnnotation annot)
		{
			base.ApplyCore(annot);
			PdfLineAnnotation pdfLineAnnotation = annot as PdfLineAnnotation;
			if (pdfLineAnnotation != null)
			{
				if (pdfLineAnnotation.Line == null)
				{
					pdfLineAnnotation.Line = new PdfLinePointCollection<PdfLineAnnotation>();
				}
				else
				{
					pdfLineAnnotation.Line.Clear();
				}
				foreach (FS_POINTF fs_POINTF in this.Line)
				{
					pdfLineAnnotation.Line.Add(fs_POINTF);
				}
				if (pdfLineAnnotation.LineEnding == null)
				{
					pdfLineAnnotation.LineEnding = new PdfLineEndingCollection(LineEndingStyles.None, LineEndingStyles.None);
					pdfLineAnnotation.LineEnding.Clear();
				}
				else
				{
					pdfLineAnnotation.LineEnding.Clear();
				}
				foreach (LineEndingStyles lineEndingStyles in this.LineEnding)
				{
					pdfLineAnnotation.LineEnding.Add(lineEndingStyles);
				}
				if (this.LineStyle == null)
				{
					pdfLineAnnotation.LineStyle = null;
				}
				else
				{
					if (pdfLineAnnotation.LineStyle == null)
					{
						pdfLineAnnotation.LineStyle = new PdfBorderStyle();
					}
					pdfLineAnnotation.LineStyle.Width = this.LineStyle.Width;
					pdfLineAnnotation.LineStyle.DashPattern = this.LineStyle.DashPattern.ToArray<float>();
					pdfLineAnnotation.LineStyle.Style = this.LineStyle.Style;
				}
				pdfLineAnnotation.InteriorColor = this.InteriorColor;
				pdfLineAnnotation.Intent = this.Intent;
				pdfLineAnnotation.LeaderLineLenght = this.LeaderLineLenght;
				pdfLineAnnotation.LeaderLineExtension = this.LeaderLineExtension;
				pdfLineAnnotation.Cap = this.Cap;
				pdfLineAnnotation.LeaderLineOffset = this.LeaderLineOffset;
				pdfLineAnnotation.CaptionPosition = this.CaptionPosition;
				pdfLineAnnotation.CaptionOffset = this.CaptionOffset;
			}
		}

		// Token: 0x060017CF RID: 6095 RVA: 0x0005B2AC File Offset: 0x000594AC
		protected override bool EqualsCore(BaseAnnotation other)
		{
			if (base.EqualsCore(other))
			{
				LineAnnotation lineAnnotation = other as LineAnnotation;
				if (lineAnnotation != null && lineAnnotation.InteriorColor == this.InteriorColor && lineAnnotation.Intent == this.Intent && lineAnnotation.LeaderLineLenght == this.LeaderLineLenght && lineAnnotation.LeaderLineExtension == this.LeaderLineExtension && lineAnnotation.Cap == this.Cap && lineAnnotation.LeaderLineOffset == this.LeaderLineOffset && lineAnnotation.CaptionPosition == this.CaptionPosition && lineAnnotation.CaptionOffset == this.CaptionOffset && EqualityComparer<PdfBorderStyleModel>.Default.Equals(lineAnnotation.LineStyle, this.LineStyle) && BaseAnnotation.CollectionEqual<FS_POINTF>(lineAnnotation.Line, this.Line))
				{
					return BaseAnnotation.CollectionEqual<LineEndingStyles>(lineAnnotation.LineEnding, this.LineEnding);
				}
			}
			return false;
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x0005B393 File Offset: 0x00059593
		public override object GetValue(AnnotationMode mode, ContextMenuItemType type)
		{
			if (mode == AnnotationMode.Line)
			{
				if (type == ContextMenuItemType.StrokeColor)
				{
					return base.Color;
				}
				if (type == ContextMenuItemType.StrokeThickness)
				{
					PdfBorderStyleModel lineStyle = this.LineStyle;
					return (lineStyle != null) ? lineStyle.Width : 1f;
				}
			}
			return base.GetValue(mode, type);
		}
	}
}
