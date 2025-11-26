using System;
using System.IO;

namespace pdfeditor.Utils
{
	// Token: 0x020000AF RID: 175
	public static class UserGuideUtils
	{
		// Token: 0x06000ADC RID: 2780 RVA: 0x0003880D File Offset: 0x00036A0D
		public static void OpenUserGuide()
		{
		}

		// Token: 0x040004BE RID: 1214
		private static readonly string userGuidePath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Doc", "User Guide.pdf");
	}
}
