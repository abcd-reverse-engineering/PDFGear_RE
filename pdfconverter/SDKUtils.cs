using System;
using System.IO;

namespace pdfconverter
{
	// Token: 0x0200001E RID: 30
	public class SDKUtils
	{
		// Token: 0x060000D7 RID: 215 RVA: 0x00003C6A File Offset: 0x00001E6A
		internal static string GetLinceseKey()
		{
			return "EEF63308-0101E907-0B180B50-44464955-4D5F434F-52500E00-67704070-64666765-61722E63-6F6D4000-02BC5934-1A0899CC-145E1B58-2FC3B806-9B438CF6-30D7ABF0-9A2F0E5B-89A54860-5A2BA77C-A3865E68-66BE97F1-28A90A9B-D618DD16-F46FAA70-63E224F4-B6F4D1B6";
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00003C74 File Offset: 0x00001E74
		internal static string GetLibPath()
		{
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pdfium.dll");
			if (!string.IsNullOrWhiteSpace(text) && File.Exists(text))
			{
				return text;
			}
			return "";
		}
	}
}
