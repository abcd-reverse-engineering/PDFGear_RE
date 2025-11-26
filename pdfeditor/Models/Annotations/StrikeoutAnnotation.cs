using System;
using pdfeditor.Models.Menus;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001A7 RID: 423
	public class StrikeoutAnnotation : BaseTextMarkupAnnotation
	{
		// Token: 0x06001817 RID: 6167 RVA: 0x0005C094 File Offset: 0x0005A294
		protected override bool EqualsCore(BaseAnnotation other)
		{
			return base.EqualsCore(other) && other is StrikeoutAnnotation;
		}

		// Token: 0x06001818 RID: 6168 RVA: 0x0005C0AA File Offset: 0x0005A2AA
		public override object GetValue(AnnotationMode mode, ContextMenuItemType type)
		{
			if (mode != AnnotationMode.Strike)
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
