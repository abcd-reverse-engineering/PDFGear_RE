using System;

namespace XmpCore
{
	// Token: 0x02000021 RID: 33
	public sealed class XmpException : Exception
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x000031E2 File Offset: 0x000013E2
		public XmpErrorCode ErrorCode { get; }

		// Token: 0x060000D6 RID: 214 RVA: 0x000031EA File Offset: 0x000013EA
		public XmpException(string message, XmpErrorCode errorCode)
			: base(message)
		{
			this.ErrorCode = errorCode;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x000031FA File Offset: 0x000013FA
		public XmpException(string message, XmpErrorCode errorCode, Exception innerException)
			: base(message, innerException)
		{
			this.ErrorCode = errorCode;
		}
	}
}
