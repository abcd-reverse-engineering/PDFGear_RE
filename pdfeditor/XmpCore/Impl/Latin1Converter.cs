using System;
using System.Text;

namespace XmpCore.Impl
{
	// Token: 0x02000030 RID: 48
	public static class Latin1Converter
	{
		// Token: 0x060001BA RID: 442 RVA: 0x00004F2C File Offset: 0x0000312C
		public static ByteBuffer Convert(ByteBuffer buffer)
		{
			if (buffer.GetEncoding() != Encoding.UTF8)
			{
				return buffer;
			}
			byte[] array = new byte[8];
			int num = 0;
			int num2 = 0;
			ByteBuffer byteBuffer = new ByteBuffer(buffer.Length * 4 / 3);
			int num3 = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				int num4 = buffer.CharAt(i);
				if (num3 == 0 || num3 != 11)
				{
					if (num4 < 127)
					{
						byteBuffer.Append((byte)num4);
					}
					else if (num4 >= 192)
					{
						num2 = -1;
						int num5 = num4;
						while (num2 < 8 && (num5 & 128) == 128)
						{
							num2++;
							num5 <<= 1;
						}
						array[num++] = (byte)num4;
						num3 = 11;
					}
					else
					{
						byte[] array2 = Latin1Converter.ConvertToUtf8((byte)num4);
						byteBuffer.Append(array2);
					}
				}
				else if (num2 > 0 && (num4 & 192) == 128)
				{
					array[num++] = (byte)num4;
					num2--;
					if (num2 == 0)
					{
						byteBuffer.Append(array, 0, num);
						num = 0;
						num3 = 0;
					}
				}
				else
				{
					byte[] array3 = Latin1Converter.ConvertToUtf8(array[0]);
					byteBuffer.Append(array3);
					i -= num;
					num = 0;
					num3 = 0;
				}
			}
			if (num3 == 11)
			{
				for (int j = 0; j < num; j++)
				{
					byte[] array4 = Latin1Converter.ConvertToUtf8(array[j]);
					byteBuffer.Append(array4);
				}
			}
			return byteBuffer;
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00005078 File Offset: 0x00003278
		private static byte[] ConvertToUtf8(byte ch)
		{
			int num = (int)(ch & byte.MaxValue);
			if (num < 128)
			{
				return new byte[] { ch };
			}
			if (num == 129 || num == 141 || num == 143 || num == 144 || num == 157)
			{
				return new byte[] { 32 };
			}
			return Encoding.UTF8.GetBytes(Encoding.GetEncoding("windows-1252").GetString(new byte[] { ch }, 0, 1));
		}

		// Token: 0x040000CA RID: 202
		private const int StateStart = 0;

		// Token: 0x040000CB RID: 203
		private const int StateUtf8Char = 11;
	}
}
