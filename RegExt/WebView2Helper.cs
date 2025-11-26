using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace RegExt
{
	// Token: 0x02000003 RID: 3
	internal static class WebView2Helper
	{
		// Token: 0x06000008 RID: 8 RVA: 0x00002590 File Offset: 0x00000790
		public static bool IsWebView2Installed(out Version version)
		{
			version = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Microsoft\\EdgeUpdate\\Clients\\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}", false))
				{
					if (registryKey != null && Version.TryParse(registryKey.GetValue("pv") as string, out version))
					{
						return version > new Version(0, 0, 0, 0);
					}
				}
			}
			catch
			{
			}
			try
			{
				using (RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\EdgeUpdate\\Clients\\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}", false))
				{
					if (registryKey2 != null && Version.TryParse(registryKey2.GetValue("pv") as string, out version))
					{
						return version > new Version(0, 0, 0, 0);
					}
				}
			}
			catch
			{
			}
			return false;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002678 File Offset: 0x00000878
		public static Task<bool> StartInstall()
		{
			string text = Path.Combine(AppContext.BaseDirectory, "MicrosoftEdgeWebview2Setup.exe");
			if (File.Exists(text))
			{
				try
				{
					ProcessStartInfo processStartInfo = new ProcessStartInfo(text, "/silent /install")
					{
						UseShellExecute = false
					};
					Process process = Process.Start(processStartInfo);
					return Task.Run<bool>(delegate
					{
						try
						{
							process.WaitForExit();
							return process.ExitCode == 0;
						}
						catch
						{
						}
						return false;
					});
				}
				catch
				{
				}
			}
			return Task.FromResult<bool>(false);
		}
	}
}
