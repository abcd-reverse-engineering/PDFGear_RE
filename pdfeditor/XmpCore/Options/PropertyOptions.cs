using System;

namespace XmpCore.Options
{
	// Token: 0x02000029 RID: 41
	public sealed class PropertyOptions : Options
	{
		// Token: 0x06000144 RID: 324 RVA: 0x00003BF1 File Offset: 0x00001DF1
		public PropertyOptions()
		{
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00003C00 File Offset: 0x00001E00
		public PropertyOptions(int options)
			: base(options)
		{
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000146 RID: 326 RVA: 0x00003C10 File Offset: 0x00001E10
		// (set) Token: 0x06000147 RID: 327 RVA: 0x00003C19 File Offset: 0x00001E19
		public bool IsUri
		{
			get
			{
				return base.GetOption(2);
			}
			set
			{
				base.SetOption(2, value);
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000148 RID: 328 RVA: 0x00003C23 File Offset: 0x00001E23
		// (set) Token: 0x06000149 RID: 329 RVA: 0x00003C2D File Offset: 0x00001E2D
		public bool HasQualifiers
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

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600014A RID: 330 RVA: 0x00003C38 File Offset: 0x00001E38
		// (set) Token: 0x0600014B RID: 331 RVA: 0x00003C42 File Offset: 0x00001E42
		public bool IsQualifier
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

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600014C RID: 332 RVA: 0x00003C4D File Offset: 0x00001E4D
		// (set) Token: 0x0600014D RID: 333 RVA: 0x00003C57 File Offset: 0x00001E57
		public bool HasLanguage
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

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600014E RID: 334 RVA: 0x00003C62 File Offset: 0x00001E62
		// (set) Token: 0x0600014F RID: 335 RVA: 0x00003C6F File Offset: 0x00001E6F
		public bool HasType
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

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000150 RID: 336 RVA: 0x00003C7D File Offset: 0x00001E7D
		// (set) Token: 0x06000151 RID: 337 RVA: 0x00003C8A File Offset: 0x00001E8A
		public bool IsStruct
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

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000152 RID: 338 RVA: 0x00003C98 File Offset: 0x00001E98
		// (set) Token: 0x06000153 RID: 339 RVA: 0x00003CA5 File Offset: 0x00001EA5
		public bool IsArray
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

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000154 RID: 340 RVA: 0x00003CB3 File Offset: 0x00001EB3
		// (set) Token: 0x06000155 RID: 341 RVA: 0x00003CC0 File Offset: 0x00001EC0
		public bool IsArrayOrdered
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

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000156 RID: 342 RVA: 0x00003CCE File Offset: 0x00001ECE
		// (set) Token: 0x06000157 RID: 343 RVA: 0x00003CDB File Offset: 0x00001EDB
		public bool IsArrayAlternate
		{
			get
			{
				return base.GetOption(2048);
			}
			set
			{
				base.SetOption(2048, value);
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000158 RID: 344 RVA: 0x00003CE9 File Offset: 0x00001EE9
		// (set) Token: 0x06000159 RID: 345 RVA: 0x00003CF6 File Offset: 0x00001EF6
		public bool IsArrayAltText
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

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600015A RID: 346 RVA: 0x00003D04 File Offset: 0x00001F04
		public bool IsArrayLimited
		{
			get
			{
				return this.arrayElementsLimit != -1;
			}
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00003D12 File Offset: 0x00001F12
		public PropertyOptions SetArrayElementLimit(int arrayLimit)
		{
			this.arrayElementsLimit = arrayLimit;
			return this;
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600015C RID: 348 RVA: 0x00003D1C File Offset: 0x00001F1C
		public int ArrayElementsLimit
		{
			get
			{
				return this.arrayElementsLimit;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600015D RID: 349 RVA: 0x00003D24 File Offset: 0x00001F24
		// (set) Token: 0x0600015E RID: 350 RVA: 0x00003D31 File Offset: 0x00001F31
		public bool IsSchemaNode
		{
			get
			{
				return base.GetOption(int.MinValue);
			}
			set
			{
				base.SetOption(int.MinValue, value);
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600015F RID: 351 RVA: 0x00003D3F File Offset: 0x00001F3F
		public bool IsCompositeProperty
		{
			get
			{
				return (base.GetOptions() & 768) > 0;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000160 RID: 352 RVA: 0x00003D50 File Offset: 0x00001F50
		public bool IsSimple
		{
			get
			{
				return !this.IsCompositeProperty;
			}
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00003D5B File Offset: 0x00001F5B
		public bool EqualArrayTypes(PropertyOptions options)
		{
			return this.IsArray == options.IsArray && this.IsArrayOrdered == options.IsArrayOrdered && this.IsArrayAlternate == options.IsArrayAlternate && this.IsArrayAltText == options.IsArrayAltText;
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00003D97 File Offset: 0x00001F97
		public void MergeWith(PropertyOptions options)
		{
			if (options != null)
			{
				base.SetOptions(base.GetOptions() | options.GetOptions());
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000163 RID: 355 RVA: 0x00003DAF File Offset: 0x00001FAF
		public bool IsOnlyArrayOptions
		{
			get
			{
				return (base.GetOptions() & -7681) == 0;
			}
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00003DC0 File Offset: 0x00001FC0
		protected override int GetValidOptions()
		{
			return -1610604558;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00003DC8 File Offset: 0x00001FC8
		protected override string DefineOptionName(int option)
		{
			if (option <= 64)
			{
				if (option <= 2)
				{
					if (option == -2147483648)
					{
						return "SCHEMA_NODE";
					}
					if (option == 2)
					{
						return "URI";
					}
				}
				else
				{
					if (option == 16)
					{
						return "HAS_QUALIFIER";
					}
					if (option == 32)
					{
						return "QUALIFIER";
					}
					if (option == 64)
					{
						return "HAS_LANGUAGE";
					}
				}
			}
			else if (option <= 512)
			{
				if (option == 128)
				{
					return "HAS_TYPE";
				}
				if (option == 256)
				{
					return "STRUCT";
				}
				if (option == 512)
				{
					return "ARRAY";
				}
			}
			else
			{
				if (option == 1024)
				{
					return "ARRAY_ORDERED";
				}
				if (option == 2048)
				{
					return "ARRAY_ALTERNATE";
				}
				if (option == 4096)
				{
					return "ARRAY_ALT_TEXT";
				}
			}
			return null;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00003E82 File Offset: 0x00002082
		internal override void AssertConsistency(int options)
		{
			if ((options & 256) > 0 && (options & 512) > 0)
			{
				throw new XmpException("IsStruct and IsArray options are mutually exclusive", XmpErrorCode.BadOptions);
			}
			if ((options & 2) > 0 && (options & 768) > 0)
			{
				throw new XmpException("Structs and arrays can't have \"value\" options", XmpErrorCode.BadOptions);
			}
		}

		// Token: 0x04000092 RID: 146
		internal const int NoOptionsFlag = 0;

		// Token: 0x04000093 RID: 147
		internal const int IsUriFlag = 2;

		// Token: 0x04000094 RID: 148
		internal const int HasQualifiersFlag = 16;

		// Token: 0x04000095 RID: 149
		internal const int QualifierFlag = 32;

		// Token: 0x04000096 RID: 150
		internal const int HasLanguageFlag = 64;

		// Token: 0x04000097 RID: 151
		internal const int HasTypeFlag = 128;

		// Token: 0x04000098 RID: 152
		internal const int StructFlag = 256;

		// Token: 0x04000099 RID: 153
		internal const int ArrayFlag = 512;

		// Token: 0x0400009A RID: 154
		internal const int ArrayOrderedFlag = 1024;

		// Token: 0x0400009B RID: 155
		internal const int ArrayAlternateFlag = 2048;

		// Token: 0x0400009C RID: 156
		internal const int ArrayAltTextFlag = 4096;

		// Token: 0x0400009D RID: 157
		internal const int SchemaNodeFlag = -2147483648;

		// Token: 0x0400009E RID: 158
		internal const int DeleteExisting = 536870912;

		// Token: 0x0400009F RID: 159
		private int arrayElementsLimit = -1;
	}
}
