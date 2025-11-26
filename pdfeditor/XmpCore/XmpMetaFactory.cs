using System;
using System.IO;
using System.Xml.Linq;
using XmpCore.Impl;
using XmpCore.Options;

namespace XmpCore
{
	// Token: 0x02000022 RID: 34
	public static class XmpMetaFactory
	{
		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x0000320B File Offset: 0x0000140B
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x00003212 File Offset: 0x00001412
		public static IXmpSchemaRegistry SchemaRegistry { get; private set; } = new XmpSchemaRegistry();

		// Token: 0x060000DA RID: 218 RVA: 0x0000321A File Offset: 0x0000141A
		public static IXmpMeta Create()
		{
			return new XmpMeta();
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00003221 File Offset: 0x00001421
		public static IXmpMeta Parse(Stream stream, ParseOptions options = null)
		{
			return XmpMetaParser.Parse(stream, options);
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000322A File Offset: 0x0000142A
		public static IXmpMeta ParseFromString(string packet, ParseOptions options = null)
		{
			return XmpMetaParser.Parse(packet, options);
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00003233 File Offset: 0x00001433
		public static IXmpMeta ParseFromBuffer(byte[] buffer, ParseOptions options = null)
		{
			return XmpMetaParser.Parse(buffer, options);
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000323C File Offset: 0x0000143C
		public static IXmpMeta ParseFromBuffer(byte[] buffer, int offset, int length, ParseOptions options = null)
		{
			return XmpMetaParser.Parse(new ByteBuffer(buffer, offset, length), options);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000324C File Offset: 0x0000144C
		public static IXmpMeta ParseFromXDocument(XDocument root, ParseOptions options = null)
		{
			return XmpMetaParser.Parse(root, options);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00003255 File Offset: 0x00001455
		public static XDocument ExtractXDocumentFromBuffer(byte[] buffer, ParseOptions options = null)
		{
			return XmpMetaParser.Extract(buffer, options);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000325E File Offset: 0x0000145E
		public static void Serialize(IXmpMeta xmp, Stream stream, SerializeOptions options = null)
		{
			XmpMetaFactory.AssertImplementation(xmp);
			XmpSerializerHelper.Serialize((XmpMeta)xmp, stream, options);
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00003273 File Offset: 0x00001473
		public static byte[] SerializeToBuffer(IXmpMeta xmp, SerializeOptions options)
		{
			XmpMetaFactory.AssertImplementation(xmp);
			return XmpSerializerHelper.SerializeToBuffer((XmpMeta)xmp, options);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00003287 File Offset: 0x00001487
		public static string SerializeToString(IXmpMeta xmp, SerializeOptions options)
		{
			XmpMetaFactory.AssertImplementation(xmp);
			return XmpSerializerHelper.SerializeToString((XmpMeta)xmp, options);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000329B File Offset: 0x0000149B
		private static void AssertImplementation(IXmpMeta xmp)
		{
			if (!(xmp is XmpMeta))
			{
				throw new NotSupportedException("The serializing service works only with the XmpMeta implementation of this library");
			}
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x000032B0 File Offset: 0x000014B0
		public static void Reset()
		{
			XmpMetaFactory.SchemaRegistry = new XmpSchemaRegistry();
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x000032BC File Offset: 0x000014BC
		public static IXmpVersionInfo VersionInfo
		{
			get
			{
				return new XmpMetaFactory.XmpVersionInfo(6, 1, 10, false, 3, "Adobe XMP Core 6.1.10");
			}
		}

		// Token: 0x020002C4 RID: 708
		private sealed class XmpVersionInfo : IXmpVersionInfo
		{
			// Token: 0x17000C58 RID: 3160
			// (get) Token: 0x0600289C RID: 10396 RVA: 0x000BF292 File Offset: 0x000BD492
			public int Major { get; }

			// Token: 0x17000C59 RID: 3161
			// (get) Token: 0x0600289D RID: 10397 RVA: 0x000BF29A File Offset: 0x000BD49A
			public int Minor { get; }

			// Token: 0x17000C5A RID: 3162
			// (get) Token: 0x0600289E RID: 10398 RVA: 0x000BF2A2 File Offset: 0x000BD4A2
			public int Micro { get; }

			// Token: 0x17000C5B RID: 3163
			// (get) Token: 0x0600289F RID: 10399 RVA: 0x000BF2AA File Offset: 0x000BD4AA
			public bool IsDebug { get; }

			// Token: 0x17000C5C RID: 3164
			// (get) Token: 0x060028A0 RID: 10400 RVA: 0x000BF2B2 File Offset: 0x000BD4B2
			public int Build { get; }

			// Token: 0x17000C5D RID: 3165
			// (get) Token: 0x060028A1 RID: 10401 RVA: 0x000BF2BA File Offset: 0x000BD4BA
			public string Message { get; }

			// Token: 0x060028A2 RID: 10402 RVA: 0x000BF2C2 File Offset: 0x000BD4C2
			public XmpVersionInfo(int major, int minor, int micro, bool debug, int engBuild, string message)
			{
				this.Major = major;
				this.Minor = minor;
				this.Micro = micro;
				this.IsDebug = debug;
				this.Build = engBuild;
				this.Message = message;
			}

			// Token: 0x060028A3 RID: 10403 RVA: 0x000BF2F7 File Offset: 0x000BD4F7
			public override string ToString()
			{
				return this.Message;
			}
		}
	}
}
