using System;
using System.IO;
using System.Security.Cryptography;

namespace pdfconverter.Utils
{
	// Token: 0x0200003D RID: 61
	public static class EncryptUtils
	{
		// Token: 0x060004D2 RID: 1234 RVA: 0x000138C2 File Offset: 0x00011AC2
		public static string EncryptStringToBase64_Aes(string plainText)
		{
			return EncryptUtils.EncryptStringToBase64_Aes(plainText, EncryptUtils.key, EncryptUtils.iv);
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x000138D4 File Offset: 0x00011AD4
		public static string EncryptStringToBase64_Aes(string plainText, byte[] Key, byte[] IV)
		{
			if (plainText == null || plainText.Length <= 0)
			{
				throw new ArgumentNullException("plainText");
			}
			if (Key == null || Key.Length == 0)
			{
				throw new ArgumentNullException("Key");
			}
			if (IV == null || IV.Length == 0)
			{
				throw new ArgumentNullException("IV");
			}
			byte[] array;
			using (Aes aes = Aes.Create())
			{
				aes.Key = Key;
				aes.IV = IV;
				ICryptoTransform cryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
						{
							streamWriter.Write(plainText);
						}
						array = memoryStream.ToArray();
					}
				}
			}
			return Convert.ToBase64String(array);
		}

		// Token: 0x04000268 RID: 616
		public static readonly byte[] key = new byte[]
		{
			114, 68, 134, 72, 70, 59, 165, 226, 82, 97,
			150, 72, 128, 164, 78, 227
		};

		// Token: 0x04000269 RID: 617
		public static readonly byte[] iv = new byte[]
		{
			162, 52, 182, 120, 71, 75, 149, 228, 37, 98,
			138, 44, 46, 180, 77, 179
		};
	}
}
