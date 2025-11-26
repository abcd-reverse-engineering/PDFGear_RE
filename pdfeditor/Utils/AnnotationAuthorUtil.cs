using System;
using CommonLib.Common;

namespace pdfeditor.Utils
{
	// Token: 0x0200006F RID: 111
	public static class AnnotationAuthorUtil
	{
		// Token: 0x06000887 RID: 2183 RVA: 0x00028F18 File Offset: 0x00027118
		public static string GetAuthorName()
		{
			string annotationAuthorName = ConfigManager.GetAnnotationAuthorName();
			if (!string.IsNullOrEmpty(annotationAuthorName))
			{
				return annotationAuthorName;
			}
			return Environment.UserName;
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x00028F3A File Offset: 0x0002713A
		public static void SetAuthorName(string authorName)
		{
			ConfigManager.SetAnnotationAuthorName(authorName ?? string.Empty);
		}
	}
}
