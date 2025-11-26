using System;
using pdfeditor.Models.Menus;
using pdfeditor.ViewModels;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001AC RID: 428
	public class UnderlineAnnotation : BaseTextMarkupAnnotation
	{
		// Token: 0x06001848 RID: 6216 RVA: 0x0005CA06 File Offset: 0x0005AC06
		protected override bool EqualsCore(BaseAnnotation other)
		{
			return base.EqualsCore(other) && other is UnderlineAnnotation;
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x0005CA1C File Offset: 0x0005AC1C
		public override object GetValue(AnnotationMode mode, ContextMenuItemType type)
		{
			if (mode != AnnotationMode.Underline)
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
