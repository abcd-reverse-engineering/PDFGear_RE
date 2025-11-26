using System;
using System.IO;

namespace pdfeditor.Utils
{
	// Token: 0x020000A1 RID: 161
	public class SDKUtils
	{
		// Token: 0x06000A1E RID: 2590 RVA: 0x00033699 File Offset: 0x00031899
		internal static string GetLinceseKey()
		{
			return "EEF63308-0101E907-0B180B50-44464955-4D5F434F-52500E00-67704070-64666765-61722E63-6F6D4000-02BC5934-1A0899CC-145E1B58-2FC3B806-9B438CF6-30D7ABF0-9A2F0E5B-89A54860-5A2BA77C-A3865E68-66BE97F1-28A90A9B-D618DD16-F46FAA70-63E224F4-B6F4D1B6";
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x000336A0 File Offset: 0x000318A0
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
