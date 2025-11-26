using System;
using pdfeditor.Models.Menus;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001A9 RID: 425
	public class HighlightAnnotation : BaseTextMarkupAnnotation
	{
		// Token: 0x06001820 RID: 6176 RVA: 0x0005C1E9 File Offset: 0x0005A3E9
		protected override bool EqualsCore(BaseAnnotation other)
		{
			return base.EqualsCore(other) && other is HighlightAnnotation;
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x0005C1FF File Offset: 0x0005A3FF
		public override object GetValue(AnnotationMode mode, ContextMenuItemType type)
		{
			if (mode != AnnotationMode.Highlight && mode != AnnotationMode.HighlightArea)
			{
				return base.GetValue(mode, type);
			}
			if (type == ContextMenuItemType.StrokeColor)
			{
				return base.Color;
			}
			return null;
		}
	}
}
