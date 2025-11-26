using System;
using CommonLib.Common;
using Newtonsoft.Json;

namespace pdfeditor.Models.Print
{
	// Token: 0x0200014C RID: 332
	public static class PrintService
	{
		// Token: 0x060013E9 RID: 5097 RVA: 0x0004F8B4 File Offset: 0x0004DAB4
		public static PrintSettings LoadSettings(string deviceId)
		{
			string text = "device-" + deviceId;
			try
			{
				string printSetting = ConfigManager.GetPrintSetting(text);
				if (!string.IsNullOrEmpty(printSetting))
				{
					return JsonConvert.DeserializeObject<PrintSettings>(printSetting);
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

		// Token: 0x060013EA RID: 5098 RVA: 0x0004F900 File Offset: 0x0004DB00
		public static void SaveSettings(PrintSettings settings)
		{
			string text = "device-" + settings.Device;
			try
			{
				string text2 = JsonConvert.SerializeObject(settings);
				if (!string.IsNullOrEmpty(text2))
				{
					ConfigManager.SetPrintSetting(text2, text);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
