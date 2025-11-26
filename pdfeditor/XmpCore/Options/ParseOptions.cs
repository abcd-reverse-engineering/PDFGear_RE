using System;
using System.Collections.Generic;

namespace XmpCore.Options
{
	// Token: 0x02000028 RID: 40
	public sealed class ParseOptions : Options
	{
		// Token: 0x06000132 RID: 306 RVA: 0x00003A7D File Offset: 0x00001C7D
		public ParseOptions()
		{
			base.SetOption(88, true);
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00003A99 File Offset: 0x00001C99
		// (set) Token: 0x06000134 RID: 308 RVA: 0x00003AA2 File Offset: 0x00001CA2
		public bool RequireXmpMeta
		{
			get
			{
				return base.GetOption(1);
			}
			set
			{
				base.SetOption(1, value);
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000135 RID: 309 RVA: 0x00003AAC File Offset: 0x00001CAC
		// (set) Token: 0x06000136 RID: 310 RVA: 0x00003AB5 File Offset: 0x00001CB5
		public bool StrictAliasing
		{
			get
			{
				return base.GetOption(4);
			}
			set
			{
				base.SetOption(4, value);
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000137 RID: 311 RVA: 0x00003ABF File Offset: 0x00001CBF
		// (set) Token: 0x06000138 RID: 312 RVA: 0x00003AC8 File Offset: 0x00001CC8
		public bool FixControlChars
		{
			get
			{
				return base.GetOption(8);
			}
			set
			{
				base.SetOption(8, value);
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000139 RID: 313 RVA: 0x00003AD2 File Offset: 0x00001CD2
		// (set) Token: 0x0600013A RID: 314 RVA: 0x00003ADC File Offset: 0x00001CDC
		public bool AcceptLatin1
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

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00003AE7 File Offset: 0x00001CE7
		// (set) Token: 0x0600013C RID: 316 RVA: 0x00003AF1 File Offset: 0x00001CF1
		public bool OmitNormalization
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

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00003AFC File Offset: 0x00001CFC
		// (set) Token: 0x0600013E RID: 318 RVA: 0x00003B06 File Offset: 0x00001D06
		public bool DisallowDoctype
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

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600013F RID: 319 RVA: 0x00003B11 File Offset: 0x00001D11
		public bool AreXMPNodesLimited
		{
			get
			{
				return this.mXMPNodesToLimit.Count > 0;
			}
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00003B24 File Offset: 0x00001D24
		public ParseOptions SetXMPNodesToLimit(Dictionary<string, int> nodeMap)
		{
			foreach (KeyValuePair<string, int> keyValuePair in nodeMap)
			{
				this.mXMPNodesToLimit[keyValuePair.Key] = keyValuePair.Value;
			}
			return this;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00003B88 File Offset: 0x00001D88
		public Dictionary<string, int> GetXMPNodesToLimit()
		{
			return new Dictionary<string, int>(this.mXMPNodesToLimit);
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00003B98 File Offset: 0x00001D98
		protected override string DefineOptionName(int option)
		{
			if (option <= 8)
			{
				if (option == 1)
				{
					return "REQUIRE_XMP_META";
				}
				if (option == 4)
				{
					return "STRICT_ALIASING";
				}
				if (option == 8)
				{
					return "FIX_CONTROL_CHARS";
				}
			}
			else
			{
				if (option == 16)
				{
					return "ACCEPT_LATIN_1";
				}
				if (option == 32)
				{
					return "OMIT_NORMALIZATION";
				}
				if (option == 64)
				{
					return "DISALLOW_DOCTYPE";
				}
			}
			return null;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00003BED File Offset: 0x00001DED
		protected override int GetValidOptions()
		{
			return 125;
		}

		// Token: 0x0400008B RID: 139
		private const int RequireXmpMetaFlag = 1;

		// Token: 0x0400008C RID: 140
		private const int StrictAliasingFlag = 4;

		// Token: 0x0400008D RID: 141
		private const int FixControlCharsFlag = 8;

		// Token: 0x0400008E RID: 142
		private const int AcceptLatin1Flag = 16;

		// Token: 0x0400008F RID: 143
		private const int OmitNormalizationFlag = 32;

		// Token: 0x04000090 RID: 144
		public const int DisallowDoctypeFlag = 64;

		// Token: 0x04000091 RID: 145
		private readonly Dictionary<string, int> mXMPNodesToLimit = new Dictionary<string, int>();
	}
}
