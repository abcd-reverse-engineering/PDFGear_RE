using System;

namespace XmpCore.Options
{
	// Token: 0x02000025 RID: 37
	public sealed class AliasOptions : Options
	{
		// Token: 0x06000105 RID: 261 RVA: 0x000036DB File Offset: 0x000018DB
		public AliasOptions()
		{
		}

		// Token: 0x06000106 RID: 262 RVA: 0x000036E3 File Offset: 0x000018E3
		public AliasOptions(int options)
			: base(options)
		{
		}

		// Token: 0x06000107 RID: 263 RVA: 0x000036EC File Offset: 0x000018EC
		public bool IsSimple()
		{
			return base.GetOptions() == 0;
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000108 RID: 264 RVA: 0x000036F7 File Offset: 0x000018F7
		// (set) Token: 0x06000109 RID: 265 RVA: 0x00003704 File Offset: 0x00001904
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

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600010A RID: 266 RVA: 0x00003712 File Offset: 0x00001912
		// (set) Token: 0x0600010B RID: 267 RVA: 0x0000371F File Offset: 0x0000191F
		public bool IsArrayOrdered
		{
			get
			{
				return base.GetOption(1024);
			}
			set
			{
				base.SetOption(1536, value);
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600010C RID: 268 RVA: 0x0000372D File Offset: 0x0000192D
		// (set) Token: 0x0600010D RID: 269 RVA: 0x0000373A File Offset: 0x0000193A
		public bool IsArrayAlternate
		{
			get
			{
				return base.GetOption(2048);
			}
			set
			{
				base.SetOption(3584, value);
			}
		}

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x0600010E RID: 270 RVA: 0x00003748 File Offset: 0x00001948
		// (set) Token: 0x0600010F RID: 271 RVA: 0x00003755 File Offset: 0x00001955
		public bool IsArrayAltText
		{
			get
			{
				return base.GetOption(4096);
			}
			set
			{
				base.SetOption(7680, value);
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00003763 File Offset: 0x00001963
		public PropertyOptions ToPropertyOptions()
		{
			return new PropertyOptions(base.GetOptions());
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00003770 File Offset: 0x00001970
		protected override string DefineOptionName(int option)
		{
			if (option <= 512)
			{
				if (option == 0)
				{
					return "PROP_DIRECT";
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

		// Token: 0x06000112 RID: 274 RVA: 0x000037CB File Offset: 0x000019CB
		protected override int GetValidOptions()
		{
			return 7680;
		}

		// Token: 0x04000080 RID: 128
		public const int PropDirect = 0;

		// Token: 0x04000081 RID: 129
		public const int PropArray = 512;

		// Token: 0x04000082 RID: 130
		public const int PropArrayOrdered = 1024;

		// Token: 0x04000083 RID: 131
		public const int PropArrayAlternate = 2048;

		// Token: 0x04000084 RID: 132
		public const int PropArrayAltText = 4096;
	}
}
