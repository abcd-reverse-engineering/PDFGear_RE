using System;

namespace pdfeditor.Models.Annotations
{
	// Token: 0x020001AD RID: 429
	public class WatermarkAnnotation : BaseAnnotation
	{
		// Token: 0x0600184B RID: 6219 RVA: 0x0005CA44 File Offset: 0x0005AC44
		protected override bool EqualsCore(BaseAnnotation other)
		{
			return other is WatermarkAnnotation;
		}
	}
}
