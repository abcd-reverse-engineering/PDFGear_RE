using System;
using System.Text;

namespace XmpCore.Options
{
	// Token: 0x0200002A RID: 42
	public sealed class SerializeOptions : Options
	{
		// Token: 0x06000167 RID: 359 RVA: 0x00003EC2 File Offset: 0x000020C2
		public SerializeOptions()
		{
			this.Padding = 2048;
			this.Newline = "\n";
			this.Indent = "  ";
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00003EEB File Offset: 0x000020EB
		public SerializeOptions(int options)
			: base(options)
		{
			this.Padding = 2048;
			this.Newline = "\n";
			this.Indent = "  ";
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000169 RID: 361 RVA: 0x00003F15 File Offset: 0x00002115
		// (set) Token: 0x0600016A RID: 362 RVA: 0x00003F1F File Offset: 0x0000211F
		public bool OmitPacketWrapper
		{
			get
			{
				return base.GetOption(16);
			}
			set
			{
				base.SetOption(16, value);
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600016B RID: 363 RVA: 0x00003F2A File Offset: 0x0000212A
		// (set) Token: 0x0600016C RID: 364 RVA: 0x00003F37 File Offset: 0x00002137
		public bool OmitXmpMetaElement
		{
			get
			{
				return base.GetOption(4096);
			}
			set
			{
				base.SetOption(4096, value);
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600016D RID: 365 RVA: 0x00003F45 File Offset: 0x00002145
		// (set) Token: 0x0600016E RID: 366 RVA: 0x00003F4F File Offset: 0x0000214F
		public bool ReadOnlyPacket
		{
			get
			{
				return base.GetOption(32);
			}
			set
			{
				base.SetOption(32, value);
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600016F RID: 367 RVA: 0x00003F5A File Offset: 0x0000215A
		// (set) Token: 0x06000170 RID: 368 RVA: 0x00003F64 File Offset: 0x00002164
		public bool UseCompactFormat
		{
			get
			{
				return base.GetOption(64);
			}
			set
			{
				base.SetOption(64, value);
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x06000171 RID: 369 RVA: 0x00003F6F File Offset: 0x0000216F
		// (set) Token: 0x06000172 RID: 370 RVA: 0x00003F7C File Offset: 0x0000217C
		public bool UseCanonicalFormat
		{
			get
			{
				return base.GetOption(128);
			}
			set
			{
				base.SetOption(128, value);
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x06000173 RID: 371 RVA: 0x00003F8A File Offset: 0x0000218A
		// (set) Token: 0x06000174 RID: 372 RVA: 0x00003F97 File Offset: 0x00002197
		public bool UsePlainXmp
		{
			get
			{
				return base.GetOption(1024);
			}
			set
			{
				base.SetOption(1024, value);
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000175 RID: 373 RVA: 0x00003FA5 File Offset: 0x000021A5
		// (set) Token: 0x06000176 RID: 374 RVA: 0x00003FB2 File Offset: 0x000021B2
		public bool IncludeThumbnailPad
		{
			get
			{
				return base.GetOption(256);
			}
			set
			{
				base.SetOption(256, value);
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x06000177 RID: 375 RVA: 0x00003FC0 File Offset: 0x000021C0
		// (set) Token: 0x06000178 RID: 376 RVA: 0x00003FCD File Offset: 0x000021CD
		public bool ExactPacketLength
		{
			get
			{
				return base.GetOption(512);
			}
			set
			{
				base.SetOption(512, value);
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000179 RID: 377 RVA: 0x00003FDB File Offset: 0x000021DB
		// (set) Token: 0x0600017A RID: 378 RVA: 0x00003FE8 File Offset: 0x000021E8
		public bool Sort
		{
			get
			{
				return base.GetOption(8192);
			}
			set
			{
				base.SetOption(8192, value);
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x0600017B RID: 379 RVA: 0x00003FF6 File Offset: 0x000021F6
		// (set) Token: 0x0600017C RID: 380 RVA: 0x00004004 File Offset: 0x00002204
		public bool EncodeUtf16Be
		{
			get
			{
				return (base.GetOptions() & 11) == 2;
			}
			set
			{
				base.SetOption(11, false);
				base.SetOption(2, value);
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00004017 File Offset: 0x00002217
		// (set) Token: 0x0600017E RID: 382 RVA: 0x00004025 File Offset: 0x00002225
		public bool EncodeUtf16Le
		{
			get
			{
				return (base.GetOptions() & 11) == 3;
			}
			set
			{
				base.SetOption(11, false);
				base.SetOption(3, value);
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x0600017F RID: 383 RVA: 0x00004038 File Offset: 0x00002238
		// (set) Token: 0x06000180 RID: 384 RVA: 0x00004046 File Offset: 0x00002246
		public bool EncodeUtf8WithBom
		{
			get
			{
				return (base.GetOptions() & 11) == 8;
			}
			set
			{
				base.SetOption(11, false);
				base.SetOption(8, value);
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06000182 RID: 386 RVA: 0x00004062 File Offset: 0x00002262
		// (set) Token: 0x06000181 RID: 385 RVA: 0x00004059 File Offset: 0x00002259
		public int BaseIndent { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000184 RID: 388 RVA: 0x00004073 File Offset: 0x00002273
		// (set) Token: 0x06000183 RID: 387 RVA: 0x0000406A File Offset: 0x0000226A
		public string Indent { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000185 RID: 389 RVA: 0x0000407B File Offset: 0x0000227B
		// (set) Token: 0x06000186 RID: 390 RVA: 0x00004083 File Offset: 0x00002283
		public string Newline { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000187 RID: 391 RVA: 0x0000408C File Offset: 0x0000228C
		// (set) Token: 0x06000188 RID: 392 RVA: 0x00004094 File Offset: 0x00002294
		public int Padding { get; set; }

		// Token: 0x06000189 RID: 393 RVA: 0x000040A0 File Offset: 0x000022A0
		public Encoding GetEncoding()
		{
			if (this.EncodeUtf16Be)
			{
				return Encoding.BigEndianUnicode;
			}
			if (this.EncodeUtf16Le)
			{
				return Encoding.Unicode;
			}
			if (this.EncodeUtf8WithBom)
			{
				return Encoding.UTF8;
			}
			if (SerializeOptions.utf8Encoding == null)
			{
				SerializeOptions.utf8Encoding = new UTF8Encoding(false);
			}
			return SerializeOptions.utf8Encoding;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x000040EE File Offset: 0x000022EE
		public object Clone()
		{
			return new SerializeOptions(base.GetOptions())
			{
				BaseIndent = this.BaseIndent,
				Indent = this.Indent,
				Newline = this.Newline,
				Padding = this.Padding
			};
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000412C File Offset: 0x0000232C
		protected override string DefineOptionName(int option)
		{
			if (option <= 256)
			{
				if (option <= 32)
				{
					if (option == 16)
					{
						return "OMIT_PACKET_WRAPPER";
					}
					if (option == 32)
					{
						return "READONLY_PACKET";
					}
				}
				else
				{
					if (option == 64)
					{
						return "USE_COMPACT_FORMAT";
					}
					if (option == 256)
					{
						return "INCLUDE_THUMBNAIL_PAD";
					}
				}
			}
			else if (option <= 1024)
			{
				if (option == 512)
				{
					return "EXACT_PACKET_LENGTH";
				}
				if (option == 1024)
				{
					return "USE_PLAIN_XMP";
				}
			}
			else
			{
				if (option == 4096)
				{
					return "OMIT_XMPMETA_ELEMENT";
				}
				if (option == 8192)
				{
					return "NORMALIZED";
				}
			}
			return null;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x000041BE File Offset: 0x000023BE
		protected override int GetValidOptions()
		{
			return 14192;
		}

		// Token: 0x040000A0 RID: 160
		private static Encoding utf8Encoding;

		// Token: 0x040000A1 RID: 161
		public const int OmitPacketWrapperFlag = 16;

		// Token: 0x040000A2 RID: 162
		public const int ReadonlyPacketFlag = 32;

		// Token: 0x040000A3 RID: 163
		public const int UseCompactFormatFlag = 64;

		// Token: 0x040000A4 RID: 164
		public const int UseCanonicalFormatFlag = 128;

		// Token: 0x040000A5 RID: 165
		public const int UsePlainXmpFlag = 1024;

		// Token: 0x040000A6 RID: 166
		public const int IncludeThumbnailPadFlag = 256;

		// Token: 0x040000A7 RID: 167
		public const int ExactPacketLengthFlag = 512;

		// Token: 0x040000A8 RID: 168
		public const int OmitXmpmetaElementFlag = 4096;

		// Token: 0x040000A9 RID: 169
		public const int SortFlag = 8192;

		// Token: 0x040000AA RID: 170
		private const int LittleEndianBit = 1;

		// Token: 0x040000AB RID: 171
		private const int Utf16Bit = 2;

		// Token: 0x040000AC RID: 172
		private const int Utf8BomBit = 8;

		// Token: 0x040000AD RID: 173
		public const int EncodeUtf8 = 0;

		// Token: 0x040000AE RID: 174
		public const int EncodeUtf8WithBomFlag = 8;

		// Token: 0x040000AF RID: 175
		public const int EncodeUtf16BeFlag = 2;

		// Token: 0x040000B0 RID: 176
		public const int EncodeUtf16LeFlag = 3;

		// Token: 0x040000B1 RID: 177
		private const int EncodingMask = 11;
	}
}
