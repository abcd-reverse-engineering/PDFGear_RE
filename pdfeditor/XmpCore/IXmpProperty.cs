using System;
using XmpCore.Options;

namespace XmpCore
{
	// Token: 0x0200001A RID: 26
	public interface IXmpProperty
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000B7 RID: 183
		string Value { get; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000B8 RID: 184
		PropertyOptions Options { get; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000B9 RID: 185
		string Language { get; }
	}
}
