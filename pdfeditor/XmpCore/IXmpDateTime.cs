using System;
using Sharpen;

namespace XmpCore
{
	// Token: 0x02000017 RID: 23
	public interface IXmpDateTime : IComparable
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000064 RID: 100
		// (set) Token: 0x06000065 RID: 101
		int Year { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000066 RID: 102
		// (set) Token: 0x06000067 RID: 103
		int Month { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000068 RID: 104
		// (set) Token: 0x06000069 RID: 105
		int Day { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600006A RID: 106
		// (set) Token: 0x0600006B RID: 107
		int Hour { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600006C RID: 108
		// (set) Token: 0x0600006D RID: 109
		int Minute { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600006E RID: 110
		// (set) Token: 0x0600006F RID: 111
		int Second { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000070 RID: 112
		// (set) Token: 0x06000071 RID: 113
		int Nanosecond { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000072 RID: 114
		// (set) Token: 0x06000073 RID: 115
		TimeZoneInfo TimeZone { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000074 RID: 116
		// (set) Token: 0x06000075 RID: 117
		TimeSpan Offset { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000076 RID: 118
		bool HasDate { get; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000077 RID: 119
		bool HasTime { get; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000078 RID: 120
		bool HasTimeZone { get; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000079 RID: 121
		Calendar Calendar { get; }

		// Token: 0x0600007A RID: 122
		string ToIso8601String();
	}
}
