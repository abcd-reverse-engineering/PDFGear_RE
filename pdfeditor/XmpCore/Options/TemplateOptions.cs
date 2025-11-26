using System;

namespace XmpCore.Options
{
	// Token: 0x0200002B RID: 43
	public sealed class TemplateOptions : Options
	{
		// Token: 0x0600018D RID: 397 RVA: 0x000041C5 File Offset: 0x000023C5
		public TemplateOptions()
		{
		}

		// Token: 0x0600018E RID: 398 RVA: 0x000041CD File Offset: 0x000023CD
		public TemplateOptions(int options)
			: base(options)
		{
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x0600018F RID: 399 RVA: 0x000041D6 File Offset: 0x000023D6
		// (set) Token: 0x06000190 RID: 400 RVA: 0x000041DF File Offset: 0x000023DF
		public bool ClearUnnamedProperties
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

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000191 RID: 401 RVA: 0x000041E9 File Offset: 0x000023E9
		// (set) Token: 0x06000192 RID: 402 RVA: 0x000041F3 File Offset: 0x000023F3
		public bool ReplaceExistingProperties
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

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000193 RID: 403 RVA: 0x000041FE File Offset: 0x000023FE
		// (set) Token: 0x06000194 RID: 404 RVA: 0x00004208 File Offset: 0x00002408
		public bool IncludeInternalProperties
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

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000195 RID: 405 RVA: 0x00004213 File Offset: 0x00002413
		// (set) Token: 0x06000196 RID: 406 RVA: 0x0000421D File Offset: 0x0000241D
		public bool AddNewProperties
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

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x06000197 RID: 407 RVA: 0x00004228 File Offset: 0x00002428
		// (set) Token: 0x06000198 RID: 408 RVA: 0x00004235 File Offset: 0x00002435
		public bool ReplaceWithDeleteEmpty
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

		// Token: 0x06000199 RID: 409 RVA: 0x00004243 File Offset: 0x00002443
		public object Clone()
		{
			return new TemplateOptions(base.GetOptions());
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00004250 File Offset: 0x00002450
		protected override string DefineOptionName(int option)
		{
			if (option <= 16)
			{
				if (option == 2)
				{
					return "CLEAR_UNNAMED_PROPERTIES";
				}
				if (option == 16)
				{
					return "REPLACE_EXISTING_PROPERTIES";
				}
			}
			else
			{
				if (option == 32)
				{
					return "INCLUDE_INTERNAL_PROPERTIES";
				}
				if (option == 64)
				{
					return "ADD_NEW_PROPERTIES";
				}
				if (option == 128)
				{
					return "REPLACE_WITH_DELETE_EMPTY";
				}
			}
			return null;
		}

		// Token: 0x0600019B RID: 411 RVA: 0x000042A0 File Offset: 0x000024A0
		protected override int GetValidOptions()
		{
			return 242;
		}

		// Token: 0x040000B6 RID: 182
		public const int ClearUnnamedPropertiesFlag = 2;

		// Token: 0x040000B7 RID: 183
		public const int ReplaceExistingPropertiesFlag = 16;

		// Token: 0x040000B8 RID: 184
		public const int IncludeInternalPropertiesFlag = 32;

		// Token: 0x040000B9 RID: 185
		public const int AddNewPropertiesFlag = 64;

		// Token: 0x040000BA RID: 186
		public const int ReplaceWithDeleteEmptyFlag = 128;
	}
}
