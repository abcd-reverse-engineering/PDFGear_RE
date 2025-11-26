using System;

namespace XmpCore.Options
{
	// Token: 0x02000026 RID: 38
	public sealed class IteratorOptions : Options
	{
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000113 RID: 275 RVA: 0x000037D2 File Offset: 0x000019D2
		// (set) Token: 0x06000114 RID: 276 RVA: 0x000037DF File Offset: 0x000019DF
		public bool IsJustChildren
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

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000115 RID: 277 RVA: 0x000037ED File Offset: 0x000019ED
		// (set) Token: 0x06000116 RID: 278 RVA: 0x000037FA File Offset: 0x000019FA
		public bool IsJustLeafName
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

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000117 RID: 279 RVA: 0x00003808 File Offset: 0x00001A08
		// (set) Token: 0x06000118 RID: 280 RVA: 0x00003815 File Offset: 0x00001A15
		public bool IsJustLeafNodes
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

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000119 RID: 281 RVA: 0x00003823 File Offset: 0x00001A23
		// (set) Token: 0x0600011A RID: 282 RVA: 0x00003830 File Offset: 0x00001A30
		public bool IsOmitQualifiers
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

		// Token: 0x0600011B RID: 283 RVA: 0x00003840 File Offset: 0x00001A40
		protected override string DefineOptionName(int option)
		{
			if (option <= 512)
			{
				if (option == 256)
				{
					return "JUST_CHILDREN";
				}
				if (option == 512)
				{
					return "JUST_LEAFNODES";
				}
			}
			else
			{
				if (option == 1024)
				{
					return "JUST_LEAFNAME";
				}
				if (option == 4096)
				{
					return "OMIT_QUALIFIERS";
				}
			}
			return null;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00003892 File Offset: 0x00001A92
		protected override int GetValidOptions()
		{
			return 5888;
		}

		// Token: 0x04000085 RID: 133
		public const int JustChildren = 256;

		// Token: 0x04000086 RID: 134
		public const int JustLeafNodes = 512;

		// Token: 0x04000087 RID: 135
		public const int JustLeafName = 1024;

		// Token: 0x04000088 RID: 136
		public const int OmitQualifiers = 4096;
	}
}
