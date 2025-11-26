using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	// Token: 0x0200000B RID: 11
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	internal sealed class NullableContextAttribute : Attribute
	{
		// Token: 0x0600002C RID: 44 RVA: 0x00002861 File Offset: 0x00000A61
		public NullableContextAttribute(byte A_1)
		{
			this.Flag = A_1;
		}

		// Token: 0x04000011 RID: 17
		public readonly byte Flag;
	}
}
