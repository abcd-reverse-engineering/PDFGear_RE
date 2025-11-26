using System;

namespace pdfconverter
{
	// Token: 0x02000007 RID: 7
	public enum FileCovertStatus
	{
		// Token: 0x04000097 RID: 151
		ConvertInit,
		// Token: 0x04000098 RID: 152
		ConvertLoading,
		// Token: 0x04000099 RID: 153
		ConvertLoaded,
		// Token: 0x0400009A RID: 154
		ConvertLoadedFailed,
		// Token: 0x0400009B RID: 155
		ConvertUnsupport,
		// Token: 0x0400009C RID: 156
		ConvertCoverting,
		// Token: 0x0400009D RID: 157
		ConvertFail,
		// Token: 0x0400009E RID: 158
		ConvertSucc
	}
}
