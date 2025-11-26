using System;
using XmpCore.Options;

namespace XmpCore
{
	// Token: 0x02000016 RID: 22
	public interface IXmpAliasInfo
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000060 RID: 96
		string Namespace { get; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000061 RID: 97
		string Prefix { get; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000062 RID: 98
		string PropName { get; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000063 RID: 99
		AliasOptions AliasForm { get; }
	}
}
