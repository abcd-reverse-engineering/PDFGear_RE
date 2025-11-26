using System;

namespace XmpCore
{
	// Token: 0x02000020 RID: 32
	public enum XmpErrorCode
	{
		// Token: 0x04000071 RID: 113
		Unknown,
		// Token: 0x04000072 RID: 114
		BadParam = 4,
		// Token: 0x04000073 RID: 115
		BadValue,
		// Token: 0x04000074 RID: 116
		InternalFailure = 9,
		// Token: 0x04000075 RID: 117
		BadSchema = 101,
		// Token: 0x04000076 RID: 118
		BadXPath,
		// Token: 0x04000077 RID: 119
		BadOptions,
		// Token: 0x04000078 RID: 120
		BadIndex,
		// Token: 0x04000079 RID: 121
		BadSerialize = 107,
		// Token: 0x0400007A RID: 122
		BadXml = 201,
		// Token: 0x0400007B RID: 123
		BadRdf,
		// Token: 0x0400007C RID: 124
		BadXmp,
		// Token: 0x0400007D RID: 125
		BadStream
	}
}
