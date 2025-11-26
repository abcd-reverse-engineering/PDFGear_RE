using System;

namespace XmpCore
{
	// Token: 0x0200001D RID: 29
	public interface IXmpVersionInfo
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000C6 RID: 198
		int Major { get; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000C7 RID: 199
		int Minor { get; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000C8 RID: 200
		int Micro { get; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000C9 RID: 201
		int Build { get; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000CA RID: 202
		bool IsDebug { get; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000CB RID: 203
		string Message { get; }
	}
}
