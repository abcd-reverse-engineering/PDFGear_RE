using System;

namespace XmpCore
{
	// Token: 0x0200001B RID: 27
	public interface IXmpPropertyInfo : IXmpProperty
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000BA RID: 186
		string Namespace { get; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000BB RID: 187
		string Path { get; }
	}
}
