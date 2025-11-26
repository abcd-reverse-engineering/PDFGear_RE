using System;
using pdfeditor.Models.Menus;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x0200019D RID: 413
	public class CircleAnnotation : BaseFigureAnnotation
	{
		// Token: 0x060017AB RID: 6059 RVA: 0x0005AC19 File Offset: 0x00058E19
		protected override bool EqualsCore(BaseAnnotation other)
		{
			return base.EqualsCore(other) && other is CircleAnnotation;
		}

		// Token: 0x060017AC RID: 6060 RVA: 0x0005AC30 File Offset: 0x00058E30
		public override object GetValue(AnnotationMode mode, ContextMenuItemType type)
		{
			if (mode != AnnotationMode.Ellipse)
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
