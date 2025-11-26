using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using CommonLib.Config;

namespace RegExt.FileAssociations
{
	// Token: 0x02000004 RID: 4
	internal static class DefaultAppHashHelper
	{
		// Token: 0x0600000A RID: 10 RVA: 0x000026F0 File Offset: 0x000008F0
		public static string GetHash(string progId, string ext)
		{
			if (string.IsNullOrEmpty(progId) || string.IsNullOrEmpty(ext))
			{
				return null;
			}
			string userSid = DefaultAppHashHelper.GetUserSid();
			if (string.IsNullOrEmpty(userSid))
			{
				return null;
			}
			string userExperienceString = DefaultAppHashHelper.GetUserExperienceString();
			if (string.IsNullOrEmpty(userExperienceString))
			{
				return null;
			}
			string hexDateTime = DefaultAppHashHelper.GetHexDateTime();
			return DefaultAppHashHelper.GetHashCore(string.Concat(new string[] { ext, userSid, progId, hexDateTime, userExperienceString }).ToLowerInvariant());
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002760 File Offset: 0x00000960
		private static string GetHashCore(string baseInfo)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(baseInfo);
			Array.Resize<byte>(ref bytes, bytes.Length + 2);
			bytes[bytes.Length - 2] = 0;
			bytes[bytes.Length - 1] = 0;
			byte[] array;
			using (MD5 md = MD5.Create())
			{
				array = md.ComputeHash(bytes);
			}
			int num = baseInfo.Length * 2 + 2;
			long num2 = (((num & 4) <= 1) ? 1L : 0L) + DefaultAppHashHelper.GetShiftRight((long)num, 2) - 1L;
			string text = "";
			if (num2 > 1L)
			{
				DefaultAppHashHelper.Map map = new DefaultAppHashHelper.Map
				{
					CACHE = 0L,
					OUTHASH1 = 0,
					PDATA = 0,
					MD51 = (DefaultAppHashHelper.GetLong(array, 0) | 1L) + 1778057216L,
					MD52 = (DefaultAppHashHelper.GetLong(array, 4) | 1L) + 333119488L,
					INDEX = DefaultAppHashHelper.GetShiftRight(num2 - 2L, 1)
				};
				map.COUNTER = map.INDEX + 1L;
				while (map.COUNTER > 0L)
				{
					map.R0 = DefaultAppHashHelper.ConvertInt32(DefaultAppHashHelper.GetLong(bytes, map.PDATA) + (long)map.OUTHASH1);
					map.R1.V1 = (long)DefaultAppHashHelper.ConvertInt32(DefaultAppHashHelper.GetLong(bytes, map.PDATA + 4));
					map.PDATA += 8;
					map.R2.V1 = (long)DefaultAppHashHelper.ConvertInt32((long)map.R0 * map.MD51 - 284857861L * DefaultAppHashHelper.GetShiftRight((long)map.R0, 16));
					map.R2.V2 = (long)DefaultAppHashHelper.ConvertInt32(2046337941L * map.R2.V1 + 1755016095L * DefaultAppHashHelper.GetShiftRight(map.R2.V1, 16));
					map.R3 = DefaultAppHashHelper.ConvertInt32((long)((ulong)(-359202815) * (ulong)map.R2.V2 - (ulong)(1007687017L * DefaultAppHashHelper.GetShiftRight(map.R2.V2, 16))));
					map.R4.V1 = (long)DefaultAppHashHelper.ConvertInt32((long)map.R3 + map.R1.V1);
					map.R5.V1 = (long)DefaultAppHashHelper.ConvertInt32(map.CACHE + (long)map.R3);
					map.R6.V1 = (long)DefaultAppHashHelper.ConvertInt32(map.R4.V1 * map.MD52 - 1021897765L * DefaultAppHashHelper.GetShiftRight(map.R4.V1, 16));
					map.R6.V2 = (long)DefaultAppHashHelper.ConvertInt32(1505996589L * map.R6.V1 - 573759729L * DefaultAppHashHelper.GetShiftRight(map.R6.V1, 16));
					map.OUTHASH1 = DefaultAppHashHelper.ConvertInt32(516489217L * map.R6.V2 + 901586633L * DefaultAppHashHelper.GetShiftRight(map.R6.V2, 16));
					map.OUTHASH2 = DefaultAppHashHelper.ConvertInt32(map.R5.V1 + (long)map.OUTHASH1);
					map.CACHE = (long)map.OUTHASH2;
					map.COUNTER -= 1L;
				}
				byte[] array2 = new byte[16];
				BitConverter.GetBytes(map.OUTHASH1).CopyTo(array2, 0);
				BitConverter.GetBytes(map.OUTHASH2).CopyTo(array2, 4);
				map = new DefaultAppHashHelper.Map
				{
					CACHE = 0L,
					OUTHASH1 = 0,
					PDATA = 0,
					MD51 = (DefaultAppHashHelper.GetLong(array, 0) | 1L),
					MD52 = (DefaultAppHashHelper.GetLong(array, 4) | 1L),
					INDEX = DefaultAppHashHelper.GetShiftRight(num2 - 2L, 1)
				};
				map.COUNTER = map.INDEX + 1L;
				while (map.COUNTER > 0L)
				{
					map.R0 = DefaultAppHashHelper.ConvertInt32(DefaultAppHashHelper.GetLong(bytes, map.PDATA) + (long)map.OUTHASH1);
					map.PDATA += 8;
					map.R1.V1 = (long)DefaultAppHashHelper.ConvertInt32((long)map.R0 * map.MD51);
					map.R1.V2 = (long)DefaultAppHashHelper.ConvertInt32((long)((ulong)(-1324285952) * (ulong)map.R1.V1 - (ulong)(812076783L * DefaultAppHashHelper.GetShiftRight(map.R1.V1, 16))));
					map.R2.V1 = (long)DefaultAppHashHelper.ConvertInt32(1537146880L * map.R1.V2 - 2029495393L * DefaultAppHashHelper.GetShiftRight(map.R1.V2, 16));
					map.R2.V2 = (long)DefaultAppHashHelper.ConvertInt32(315537773L * DefaultAppHashHelper.GetShiftRight(map.R2.V1, 16) - 1184038912L * map.R2.V1);
					map.R3 = DefaultAppHashHelper.ConvertInt32(495124480L * map.R2.V2 + 629022083L * DefaultAppHashHelper.GetShiftRight(map.R2.V2, 16));
					map.R4.V1 = (long)DefaultAppHashHelper.ConvertInt32(map.MD52 * ((long)map.R3 + DefaultAppHashHelper.GetLong(bytes, map.PDATA - 4)));
					map.R4.V2 = (long)DefaultAppHashHelper.ConvertInt32(385155072L * map.R4.V1 - 1569450251L * DefaultAppHashHelper.GetShiftRight(map.R4.V1, 16));
					map.R5.V1 = (long)DefaultAppHashHelper.ConvertInt32((long)((ulong)(-1761673216) * (ulong)map.R4.V2 - (ulong)(746350849L * DefaultAppHashHelper.GetShiftRight(map.R4.V2, 16))));
					map.R5.V2 = (long)DefaultAppHashHelper.ConvertInt32(730398720L * map.R5.V1 + 2090019721L * DefaultAppHashHelper.GetShiftRight(map.R5.V1, 16));
					map.OUTHASH1 = DefaultAppHashHelper.ConvertInt32((long)((ulong)(-1620508672) * (ulong)map.R5.V2 - (ulong)(1079730327L * DefaultAppHashHelper.GetShiftRight(map.R5.V2, 16))));
					map.OUTHASH2 = DefaultAppHashHelper.ConvertInt32((long)map.OUTHASH1 + map.CACHE + (long)map.R3);
					map.CACHE = (long)map.OUTHASH2;
					map.COUNTER -= 1L;
				}
				BitConverter.GetBytes(map.OUTHASH1).CopyTo(array2, 8);
				BitConverter.GetBytes(map.OUTHASH2).CopyTo(array2, 12);
				byte[] array3 = new byte[8];
				int num3 = (int)(DefaultAppHashHelper.GetLong(array2, 8) ^ DefaultAppHashHelper.GetLong(array2, 0));
				long num4 = DefaultAppHashHelper.GetLong(array2, 12) ^ DefaultAppHashHelper.GetLong(array2, 4);
				BitConverter.GetBytes(num3).CopyTo(array3, 0);
				BitConverter.GetBytes((int)num4).CopyTo(array3, 4);
				text = Convert.ToBase64String(array3);
			}
			return text;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002EA8 File Offset: 0x000010A8
		private static long GetShiftRight(long value, int count)
		{
			if ((value & (long)((ulong)(-2147483648))) != 0L)
			{
				return (value >> count) ^ (long)((ulong)(-65536));
			}
			return value >> count;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002EC8 File Offset: 0x000010C8
		private static long GetLong(byte[] bytes, int index = 0)
		{
			return (long)BitConverter.ToInt32(bytes, index);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002ED2 File Offset: 0x000010D2
		private static int ConvertInt32(long value)
		{
			return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002EE0 File Offset: 0x000010E0
		private static string GetUserSid()
		{
			return new NTAccount(Environment.UserName).Translate(typeof(SecurityIdentifier)).Value.ToLowerInvariant();
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002F08 File Offset: 0x00001108
		private static string GetUserExperienceString()
		{
			try
			{
				string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "shell32.dll");
				if (File.Exists(text))
				{
					FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(text);
					string text2 = "shell32_ue_" + versionInfo.FileVersion.Replace(" ", "").Trim();
					string text3;
					if (ConfigUtils.TryGet<string>(text2, out text3) && !string.IsNullOrEmpty(text3))
					{
						return text3;
					}
					string text4 = "User Choice set via Windows User Experience";
					byte[] array;
					using (FileStream fileStream = File.Open(text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						using (BinaryReader binaryReader = new BinaryReader(fileStream, new UTF8Encoding(false), true))
						{
							array = binaryReader.ReadBytes(5242880);
						}
					}
					string text5 = "";
					int num = 0;
					do
					{
						try
						{
							text5 = Encoding.Unicode.GetString(array, 0, array.Length - num);
						}
						catch
						{
						}
						num++;
					}
					while (string.IsNullOrEmpty(text5) && num < array.Length);
					int num2 = text5.IndexOf(text4);
					if (num2 >= 0)
					{
						int num3 = text5.IndexOf("}", num2);
						if (num3 == -1)
						{
						}
						string text6 = text5.Substring(num2, num3 - num2 + 1);
						ConfigUtils.TrySet<string>(text2, text6);
						return text6;
					}
				}
			}
			catch
			{
			}
			return string.Empty;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000030C4 File Offset: 0x000012C4
		private static string GetHexDateTime()
		{
			DateTime now = DateTime.Now;
			DateTime dateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
			long num = dateTime.ToFileTime();
			long num2 = num >> 32;
			long num3 = num & (long)((ulong)(-1));
			return string.Format("{0:x8}{1:x8}", num2, num3);
		}

		// Token: 0x0200000C RID: 12
		private struct Map
		{
			// Token: 0x04000013 RID: 19
			public int PDATA;

			// Token: 0x04000014 RID: 20
			public long CACHE;

			// Token: 0x04000015 RID: 21
			public long COUNTER;

			// Token: 0x04000016 RID: 22
			public long INDEX;

			// Token: 0x04000017 RID: 23
			public long MD51;

			// Token: 0x04000018 RID: 24
			public long MD52;

			// Token: 0x04000019 RID: 25
			public int OUTHASH1;

			// Token: 0x0400001A RID: 26
			public int OUTHASH2;

			// Token: 0x0400001B RID: 27
			public int R0;

			// Token: 0x0400001C RID: 28
			public DefaultAppHashHelper.Map.R R1;

			// Token: 0x0400001D RID: 29
			public DefaultAppHashHelper.Map.R R2;

			// Token: 0x0400001E RID: 30
			public int R3;

			// Token: 0x0400001F RID: 31
			public DefaultAppHashHelper.Map.R R4;

			// Token: 0x04000020 RID: 32
			public DefaultAppHashHelper.Map.R R5;

			// Token: 0x04000021 RID: 33
			public DefaultAppHashHelper.Map.R R6;

			// Token: 0x04000022 RID: 34
			public DefaultAppHashHelper.Map.R R7;

			// Token: 0x0200000E RID: 14
			public struct R
			{
				// Token: 0x04000025 RID: 37
				public long V1;

				// Token: 0x04000026 RID: 38
				public long V2;
			}
		}
	}
}
