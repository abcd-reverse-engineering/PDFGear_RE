using System;
using pdfeditor.Models.Menus;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001A4 RID: 420
	public class SquareAnnotation : BaseFigureAnnotation
	{
		// Token: 0x060017F2 RID: 6130 RVA: 0x0005B828 File Offset: 0x00059A28
		protected override bool EqualsCore(BaseAnnotation other)
		{
			return base.EqualsCore(other) && other is SquareAnnotation;
		}

		// Token: 0x060017F3 RID: 6131 RVA: 0x0005B840 File Offset: 0x00059A40
		public override object GetValue(AnnotationMode mode, ContextMenuItemType type)
		{
			if (mode != AnnotationMode.Shape)
			{
				return base.GetValue(mode, type);
			}
			switch (type)
			{
			case ContextMenuItemType.StrokeColor:
				return base.Color;
			case ContextMenuItemType.FillColor:
				return base.InteriorColor;
			case ContextMenuItemType.StrokeThickness:
			{
				PdfBorderStyleModel borderStyle = base.BorderStyle;
				return (borderStyle != null) ? borderStyle.Width : 1f;
			}
			default:
				return null;
			}
		}
	}
}
