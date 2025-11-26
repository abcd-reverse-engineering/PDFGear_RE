using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using CommonLib.Common;
using Microsoft.Win32;

namespace RegExt.FileAssociations
{
	// Token: 0x02000005 RID: 5
	internal static class DefaultAppHelper
	{
		// Token: 0x06000012 RID: 18 RVA: 0x00003128 File Offset: 0x00001328
		public static string GetDefaultAppProgId(string ext)
		{
			return AppIdHelper.GetDefaultAppProgId(ext);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00003130 File Offset: 0x00001330
		public static void ResetDefaultApp(string ext)
		{
			if (string.IsNullOrEmpty(ext))
			{
				return;
			}
			ext = ext.Trim();
			string text;
			if (ext.Contains("."))
			{
				text = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\" + ext;
			}
			else
			{
				text = "Software\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\" + ext;
			}
			try
			{
				string text2 = WindowsIdentity.GetCurrent().User.ToString();
				string text3 = "\\Registry\\User\\" + text2 + "\\" + text;
				RegistryBatchHelper.DeleteRegistry(new string[] { text3 + "\\UserChoice" });
				RegistryBatchHelper.DeleteRegistry(new string[] { text3 + "\\UserChoiceLatest\\ProgId" });
				RegistryBatchHelper.DeleteRegistry(new string[] { text3 + "\\UserChoiceLatest" });
			}
			catch
			{
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00003200 File Offset: 0x00001400
		public static void SetDefaultApp(string progId, string ext)
		{
			if (string.IsNullOrEmpty(progId) || string.IsNullOrEmpty(ext))
			{
				return;
			}
			progId = progId.Trim();
			ext = ext.Trim();
			DefaultAppHelper.WriteRequiredApplicationAssociationToasts(progId, ext);
			if (ext.Contains("."))
			{
				DefaultAppHelper.ResetUserChoice("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\" + ext, progId, ext);
				return;
			}
			DefaultAppHelper.ResetUserChoice("Software\\Microsoft\\Windows\\Shell\\Associations\\UrlAssociations\\" + ext, progId, ext);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00003268 File Offset: 0x00001468
		private static void ResetUserChoice(string keyPath, string progId, string ext)
		{
			try
			{
				string text = WindowsIdentity.GetCurrent().User.ToString();
				string text2 = "\\Registry\\User\\" + text + "\\" + keyPath;
				RegistryBatchHelper.DeleteRegistry(new string[] { text2 + "\\UserChoice" });
				string hash = DefaultAppHashHelper.GetHash(progId, ext);
				string text3 = text2 + "\\UserChoice";
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["Hash"] = hash;
				dictionary["ProgId"] = progId;
				RegistryBatchHelper.SetRegistryKeyValue(text3, dictionary, "");
			}
			catch
			{
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00003300 File Offset: 0x00001500
		private static void WriteRequiredApplicationAssociationToasts(string progId, string ext)
		{
			DefaultAppHelper.WriteRequiredApplicationAssociationToasts(new global::System.ValueTuple<string, string>[]
			{
				new global::System.ValueTuple<string, string>(progId, ext)
			});
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000331C File Offset: 0x0000151C
		private static void WriteRequiredApplicationAssociationToasts([global::System.Runtime.CompilerServices.TupleElementNames(new string[] { "progId", "ext" })] IReadOnlyList<global::System.ValueTuple<string, string>> exts)
		{
			List<global::System.ValueTuple<RegistryKey, IReadOnlyList<string>>> list = new List<global::System.ValueTuple<RegistryKey, IReadOnlyList<string>>>();
			foreach (global::System.ValueTuple<string, string> valueTuple in exts)
			{
				string item = valueTuple.Item1;
				string item2 = valueTuple.Item2;
				RegistryKey registryKey;
				IReadOnlyList<string> readOnlyList;
				if (DefaultAppHelper.GetRequiredApplicationAssociationToasts(item, item2, out registryKey, out readOnlyList))
				{
					list.Add(new global::System.ValueTuple<RegistryKey, IReadOnlyList<string>>(registryKey, readOnlyList));
				}
			}
			try
			{
				for (int i = 0; i < list.Count; i++)
				{
					for (int j = 0; j < list[i].Item2.Count; j++)
					{
						list[i].Item1.SetValue(list[i].Item2[j], 0);
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00003400 File Offset: 0x00001600
		private static bool GetRequiredApplicationAssociationToasts(string progId, string ext, out RegistryKey registryKey, out IReadOnlyList<string> apps)
		{
			registryKey = null;
			apps = null;
			try
			{
				registryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default).OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ApplicationAssociationToasts", true);
			}
			catch
			{
			}
			if (registryKey == null)
			{
				return false;
			}
			List<string> list = new List<string>();
			list.Add(progId);
			RegistryKey registryKey2 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
			RegistryKey registryKey3 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
			list.AddRange(DefaultAppHelper.GetAppsFromClasses(registryKey2, ext));
			list.AddRange(DefaultAppHelper.GetAppsFromClasses(registryKey3, ext));
			list.AddRange(DefaultAppHelper.GetAppsFromSoftwareCapabilities(registryKey2, ext));
			list.AddRange(DefaultAppHelper.GetAppsFromSoftwareCapabilities(registryKey3, ext));
			list.AddRange(DefaultAppHelper.GetStartMenuInternetApps(registryKey2, ext));
			list.AddRange(DefaultAppHelper.GetStartMenuInternetApps(registryKey3, ext));
			if (registryKey2 != null)
			{
				registryKey2.Dispose();
			}
			if (registryKey3 != null)
			{
				registryKey3.Dispose();
			}
			list.AddRange(DefaultAppHelper.GetAppsFromAppModel(ext));
			try
			{
				for (int i = list.Count - 1; i >= 0; i--)
				{
					string text = list[i];
					if (string.IsNullOrEmpty(text))
					{
						list.RemoveAt(i);
					}
					else
					{
						string text2 = text + "_" + ext;
						if (registryKey.GetValue(text2) != null)
						{
							list.RemoveAt(i);
						}
						else if (i == list.Count + 1)
						{
							list[i] = text2;
						}
						else
						{
							list.Add(text2);
						}
					}
				}
			}
			catch (Exception)
			{
			}
			list = list.Distinct<string>().ToList<string>();
			if (list.Count == 0)
			{
				registryKey.Dispose();
				return false;
			}
			apps = list;
			return true;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00003578 File Offset: 0x00001778
		private static IEnumerable<string> GetStartMenuInternetApps(RegistryKey baseKey, string ext)
		{
			List<string> list = new List<string>();
			try
			{
				RegistryKey registryKey = ((baseKey != null) ? baseKey.OpenSubKey("SOFTWARE\\Clients\\StartMenuInternet") : null);
				if (registryKey != null)
				{
					foreach (string text in registryKey.GetSubKeyNames())
					{
						RegistryKey registryKey2 = registryKey.OpenSubKey(text + "\\Capabilities\\" + (ext.Contains(".") ? "FileAssociations" : "URLAssociations"));
						if (!string.IsNullOrEmpty(((registryKey2 != null) ? registryKey2.GetValue(ext) : null) as string))
						{
							list.Add(text);
						}
					}
				}
			}
			catch
			{
			}
			return list;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x0000361C File Offset: 0x0000181C
		private static IEnumerable<string> GetAppsFromAppModel(string ext)
		{
			List<string> list = new List<string>();
			try
			{
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default))
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey("Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\AppModel\\Repository\\Packages"))
					{
						if (registryKey2 != null)
						{
							foreach (string text in registryKey2.GetSubKeyNames())
							{
								try
								{
									using (RegistryKey registryKey3 = registryKey2.OpenSubKey(text))
									{
										if (registryKey3 != null)
										{
											string[] subKeyNames2 = registryKey3.GetSubKeyNames();
											string text2 = "";
											if (subKeyNames2.Length == 1)
											{
												text2 = subKeyNames2[0];
											}
											else if (text2.Contains("App"))
											{
												text2 = "App";
											}
											else if (subKeyNames2.Length > 1)
											{
												text2 = subKeyNames2[0];
											}
											string text3 = text2 + "\\Capabilities\\FileAssociations";
											if (!ext.Contains('.'))
											{
												text3 = text2 + "\\Capabilities\\URLAssociations";
											}
											using (RegistryKey registryKey4 = registryKey3.OpenSubKey(text3))
											{
												if (registryKey4 != null)
												{
													foreach (string text4 in registryKey4.GetValueNames())
													{
														if (string.Equals(text4, ext, StringComparison.OrdinalIgnoreCase))
														{
															string text5 = registryKey4.GetValue(text4) as string;
															if (text5 != null)
															{
																list.Add(text5);
															}
														}
													}
												}
											}
										}
									}
								}
								catch
								{
								}
							}
						}
					}
				}
			}
			catch
			{
			}
			return list;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00003818 File Offset: 0x00001A18
		private static IEnumerable<string> GetAppsFromClasses(RegistryKey baseKey, string ext)
		{
			List<string> list = new List<string>();
			try
			{
				RegistryKey registryKey = baseKey.OpenSubKey("SOFTWARE\\Classes\\" + ext);
				string text = ((registryKey != null) ? registryKey.GetValue("") : null) as string;
				if (text != null)
				{
					list.Add(text);
				}
			}
			catch
			{
			}
			try
			{
				RegistryKey registryKey2 = baseKey.OpenSubKey("SOFTWARE\\Classes\\" + ext + "\\OpenWithList");
				IEnumerable<string> enumerable;
				if (registryKey2 == null)
				{
					enumerable = null;
				}
				else
				{
					enumerable = from c in registryKey2.GetSubKeyNames()
						select "Applications\\" + c;
				}
				IEnumerable<string> enumerable2 = enumerable;
				if (enumerable2 != null)
				{
					list.AddRange(enumerable2);
				}
			}
			catch
			{
			}
			try
			{
				RegistryKey registryKey3 = baseKey.OpenSubKey("SOFTWARE\\Classes\\" + ext + "\\OpenWithProgids");
				string[] array = ((registryKey3 != null) ? registryKey3.GetValueNames() : null);
				if (array != null)
				{
					list.AddRange(array);
				}
			}
			catch
			{
			}
			return list;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00003918 File Offset: 0x00001B18
		private static IEnumerable<string> GetAppsFromSoftwareCapabilities(RegistryKey baseKey, string ext)
		{
			List<string> list = new List<string>();
			try
			{
				using (RegistryKey registryKey = baseKey.OpenSubKey("SOFTWARE"))
				{
					foreach (string text in registryKey.GetSubKeyNames())
					{
						try
						{
							string text2 = text + "\\Capabilities\\" + (ext.Contains(".") ? "FileAssociations" : "URLAssociations");
							using (RegistryKey registryKey2 = registryKey.OpenSubKey(text2))
							{
								if (registryKey2 != null)
								{
									foreach (string text3 in registryKey2.GetValueNames())
									{
										if (string.Equals(text3, ext, StringComparison.OrdinalIgnoreCase))
										{
											string text4 = registryKey2.GetValue(text3) as string;
											if (text4 != null)
											{
												list.Add(text4);
											}
										}
									}
								}
							}
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
			return list;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00003A30 File Offset: 0x00001C30
		private static void RemoveUserChoiceKey(string key)
		{
			try
			{
				DefaultAppHelper.DeleteKey(key);
			}
			catch
			{
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00003A58 File Offset: 0x00001C58
		private static void SetUserChoiceKeyAccessControl(RegistryKey extensionKey, string userChoiceKeyName = "UserChoice")
		{
			using (RegistryKey registryKey = extensionKey.OpenSubKey(userChoiceKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions))
			{
				if (registryKey != null)
				{
					string name = WindowsIdentity.GetCurrent().Name;
					RegistrySecurity accessControl = registryKey.GetAccessControl();
					foreach (object obj in accessControl.GetAccessRules(true, true, typeof(NTAccount)))
					{
						RegistryAccessRule registryAccessRule = (RegistryAccessRule)obj;
						if (registryAccessRule.IdentityReference.Value == name && registryAccessRule.AccessControlType == AccessControlType.Deny)
						{
							accessControl.RemoveAccessRuleSpecific(registryAccessRule);
						}
					}
					registryKey.SetAccessControl(accessControl);
				}
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003B28 File Offset: 0x00001D28
		public static void Refresh()
		{
			DefaultAppHelper.SHChangeNotify(134217728, 0, IntPtr.Zero, IntPtr.Zero);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003B40 File Offset: 0x00001D40
		private static void DeleteKey(string key)
		{
			UIntPtr zero = UIntPtr.Zero;
			DefaultAppHelper.RegOpenKeyEx((UIntPtr)2147483649U, key, 0, 131097, out zero);
			DefaultAppHelper.RegDeleteKey((UIntPtr)2147483649U, key);
		}

		// Token: 0x06000021 RID: 33
		[DllImport("Shell32.dll")]
		private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

		// Token: 0x06000022 RID: 34
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern int RegOpenKeyEx(UIntPtr hKey, string subKey, int ulOptions, int samDesired, out UIntPtr hkResult);

		// Token: 0x06000023 RID: 35
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern uint RegDeleteKey(UIntPtr hKey, string subKey);
	}
}
