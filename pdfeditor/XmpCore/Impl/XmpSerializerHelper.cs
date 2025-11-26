using System;
using System.IO;
using XmpCore.Options;

namespace XmpCore.Impl
{
	// Token: 0x0200003E RID: 62
	public static class XmpSerializerHelper
	{
		// Token: 0x060002D2 RID: 722 RVA: 0x0000BA88 File Offset: 0x00009C88
		public static void Serialize(XmpMeta xmp, Stream stream, SerializeOptions options)
		{
			options = options ?? new SerializeOptions();
			if (options.Sort)
			{
				xmp.Sort();
			}
			new XmpSerializerRdf().Serialize(xmp, stream, options);
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x0000BAB4 File Offset: 0x00009CB4
		public static string SerializeToString(XmpMeta xmp, SerializeOptions options)
		{
			options = options ?? new SerializeOptions();
			MemoryStream memoryStream = new MemoryStream(2048);
			XmpSerializerHelper.Serialize(xmp, memoryStream, options);
			string text;
			try
			{
				text = options.GetEncoding().GetString(memoryStream.ToArray(), 0, (int)memoryStream.Length);
			}
			catch
			{
				text = memoryStream.ToString();
			}
			return text;
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x0000BB18 File Offset: 0x00009D18
		public static byte[] SerializeToBuffer(XmpMeta xmp, SerializeOptions options)
		{
			MemoryStream memoryStream = new MemoryStream(2048);
			XmpSerializerHelper.Serialize(xmp, memoryStream, options);
			return memoryStream.ToArray();
		}
	}
}
