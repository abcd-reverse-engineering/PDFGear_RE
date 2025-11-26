using System;
using Microsoft.Win32;

namespace RegExt.FileAssociations
{
	// Token: 0x02000006 RID: 6
	internal static class RegisterExtensions
	{
		// Token: 0x06000024 RID: 36 RVA: 0x00003B7D File Offset: 0x00001D7D
		internal static void Write32(this RegistryHive hive, string subKey, string value)
		{
			hive.Write32(subKey, "", value);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00003B8C File Offset: 0x00001D8C
		internal static void Write32(this RegistryHive hive, string subKey, string key, string value)
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default))
			{
				using (RegistryKey registryKey2 = registryKey.CreateSubKey(subKey, true))
				{
					registryKey2.SetValue(key, value);
				}
			}
		}
	}
}
