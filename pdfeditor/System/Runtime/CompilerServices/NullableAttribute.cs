using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	// Token: 0x0200000A RID: 10
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = false)]
	internal sealed class NullableAttribute : Attribute
	{
		// Token: 0x0600002A RID: 42 RVA: 0x0000283A File Offset: 0x00000A3A
		public NullableAttribute(byte A_1)
		{
			this.NullableFlags = new byte[] { A_1 };
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002852 File Offset: 0x00000A52
		public NullableAttribute(byte[] A_1)
		{
			this.NullableFlags = A_1;
		}

		// Token: 0x04000010 RID: 16
		public readonly byte[] NullableFlags;
	}
}
