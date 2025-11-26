using System;
using System.Linq;
using Microsoft.Win32;

namespace pdfeditor.Utils.Email
{
	// Token: 0x020000CE RID: 206
	internal static class MailApiProvider
	{
		// Token: 0x06000BB3 RID: 2995 RVA: 0x0003DD98 File Offset: 0x0003BF98
		public static bool SendMessage(EmailMessage message)
		{
			MAPIHelper mapihelper = new MAPIHelper();
			foreach (string text in message.To)
			{
				mapihelper.AddRecipientTo(text);
			}
			foreach (string text2 in message.Cc)
			{
				mapihelper.AddRecipientCc(text2);
			}
			foreach (string text3 in message.Bcc)
			{
				mapihelper.AddRecipientBcc(text3);
			}
			foreach (string text4 in message.AttachmentFilePath)
			{
				mapihelper.AddAttachment(text4);
			}
			return mapihelper.SendMailPopup(message.Subject, message.Body);
		}

		// Token: 0x06000BB4 RID: 2996 RVA: 0x0003DEB8 File Offset: 0x0003C0B8
		public static bool HasThirdPartyClient()
		{
			if (!string.IsNullOrEmpty(MailApiProvider.GetDefaultClientName()))
			{
				return true;
			}
			string[] names = MailApiProvider.GetNames();
			return names != null && names.Length != 0 && (names.Length != 1 || !string.Equals(names[0], "Hotmail", StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x06000BB5 RID: 2997 RVA: 0x0003DEFC File Offset: 0x0003C0FC
		private static string GetDefaultClientName()
		{
			string text2;
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Clients\\Mail", false))
			{
				string text;
				if (registryKey == null)
				{
					text = null;
				}
				else
				{
					object value = registryKey.GetValue(null);
					text = ((value != null) ? value.ToString() : null);
				}
				text2 = text ?? string.Empty;
			}
			return text2;
		}

		// Token: 0x06000BB6 RID: 2998 RVA: 0x0003DF5C File Offset: 0x0003C15C
		private static string[] GetNames()
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Clients\\Mail", false))
				{
					string[] array;
					if (registryKey == null)
					{
						array = null;
					}
					else
					{
						array = registryKey.GetSubKeyNames().Where(delegate(string clientName)
						{
							bool flag;
							try
							{
								using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Clients\\Mail\\" + clientName))
								{
									flag = ((registryKey2 != null) ? registryKey2.GetValue("DllPath") : null) != null;
								}
							}
							catch
							{
								flag = false;
							}
							return flag;
						}).ToArray<string>();
					}
					return array ?? Array.Empty<string>();
				}
			}
			catch
			{
			}
			return Array.Empty<string>();
		}
	}
}
