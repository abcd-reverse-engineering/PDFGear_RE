using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RegExt.FileAssociations
{
	// Token: 0x02000009 RID: 9
	internal static class RegistryBatchHelper
	{
		// Token: 0x0600004C RID: 76 RVA: 0x000040B0 File Offset: 0x000022B0
		public static void DeleteRegistry(string[] paths)
		{
			string tempFileName = Path.GetTempFileName();
			try
			{
				using (FileStream fileStream = new FileStream(tempFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
				{
					using (StreamWriter streamWriter = new StreamWriter(fileStream, RegistryBatchHelper.UTF8WithoutBOMEncoding, 4096, true))
					{
						foreach (string text in paths)
						{
							if (!string.IsNullOrEmpty(text))
							{
								streamWriter.WriteLine(text + " [1 7 17]");
								streamWriter.WriteLine(text + " [DELETE]");
							}
						}
					}
				}
				RegistryBatchHelper.RunProcess("regini.exe", "\"" + tempFileName + "\"");
			}
			catch
			{
			}
			finally
			{
				try
				{
					File.Delete(tempFileName);
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000041AC File Offset: 0x000023AC
		public static void SetRegistryKeyValue(string path, Dictionary<string, string> keyValues, string role = "")
		{
			string tempFileName = Path.GetTempFileName();
			try
			{
				using (FileStream fileStream = new FileStream(tempFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
				{
					using (StreamWriter streamWriter = new StreamWriter(fileStream, RegistryBatchHelper.UTF8WithoutBOMEncoding, 4096, true))
					{
						streamWriter.WriteLine(path + " [1 7 17]");
						foreach (KeyValuePair<string, string> keyValuePair in keyValues)
						{
							streamWriter.WriteLine(keyValuePair.Key + " = \"" + keyValuePair.Value + "\"");
						}
						if (!string.IsNullOrEmpty(role))
						{
							streamWriter.WriteLine(path + " [ " + role + " ]");
						}
					}
				}
				RegistryBatchHelper.RunProcess("regini.exe", "\"" + tempFileName + "\"");
			}
			catch
			{
			}
			finally
			{
				try
				{
					File.Delete(tempFileName);
				}
				catch
				{
				}
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000042EC File Offset: 0x000024EC
		private static void RunProcess(string name, string args)
		{
			try
			{
				Process.Start(new ProcessStartInfo(name, args)
				{
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					CreateNoWindow = true
				}).WaitForExit(1000);
			}
			catch
			{
			}
		}

		// Token: 0x0400000F RID: 15
		private static Encoding UTF8WithoutBOMEncoding = new UTF8Encoding(false);
	}
}
