using System;
using System.Globalization;
using Sharpen;

namespace XmpCore.Impl
{
	// Token: 0x02000036 RID: 54
	public sealed class XmpDateTime : IXmpDateTime, IComparable
	{
		// Token: 0x060001F2 RID: 498 RVA: 0x000072E1 File Offset: 0x000054E1
		public XmpDateTime()
		{
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x000072EC File Offset: 0x000054EC
		public XmpDateTime(global::Sharpen.Calendar calendar)
		{
			DateTime time = calendar.GetTime();
			TimeZoneInfo timeZone = calendar.GetTimeZone();
			global::Sharpen.GregorianCalendar gregorianCalendar = (global::Sharpen.GregorianCalendar)global::Sharpen.Calendar.GetInstance(CultureInfo.InvariantCulture);
			gregorianCalendar.SetGregorianChange(XmpDateTime.UnixTimeToDateTime(long.MinValue));
			gregorianCalendar.SetTimeZone(timeZone);
			gregorianCalendar.SetTime(time);
			this._year = gregorianCalendar.Get(CalendarEnum.Year);
			this._month = gregorianCalendar.Get(CalendarEnum.Month) + 1;
			this._day = gregorianCalendar.Get(CalendarEnum.DayOfMonth);
			this._hour = gregorianCalendar.Get(CalendarEnum.HourOfDay);
			this._minute = gregorianCalendar.Get(CalendarEnum.Minute);
			this._second = gregorianCalendar.Get(CalendarEnum.Second);
			this._nanoseconds = gregorianCalendar.Get(CalendarEnum.Millisecond) * 1000000;
			this._timeZone = gregorianCalendar.GetTimeZone();
			this.HasDate = (this.HasTime = (this.HasTimeZone = true));
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x000073CC File Offset: 0x000055CC
		public XmpDateTime(DateTime date, TimeZoneInfo timeZone)
		{
			global::Sharpen.GregorianCalendar gregorianCalendar = new global::Sharpen.GregorianCalendar(timeZone);
			gregorianCalendar.SetTime(date);
			this._year = gregorianCalendar.Get(CalendarEnum.Year);
			this._month = gregorianCalendar.Get(CalendarEnum.Month) + 1;
			this._day = gregorianCalendar.Get(CalendarEnum.DayOfMonth);
			this._hour = gregorianCalendar.Get(CalendarEnum.HourOfDay);
			this._minute = gregorianCalendar.Get(CalendarEnum.Minute);
			this._second = gregorianCalendar.Get(CalendarEnum.Second);
			this._nanoseconds = gregorianCalendar.Get(CalendarEnum.Millisecond) * 1000000;
			this._timeZone = timeZone;
			this.HasDate = (this.HasTime = (this.HasTimeZone = true));
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00007470 File Offset: 0x00005670
		public XmpDateTime(string strValue)
		{
			Iso8601Converter.Parse(strValue, this);
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x00007480 File Offset: 0x00005680
		// (set) Token: 0x060001F7 RID: 503 RVA: 0x00007488 File Offset: 0x00005688
		public int Year
		{
			get
			{
				return this._year;
			}
			set
			{
				this._year = Math.Min(Math.Abs(value), 9999);
				this.HasDate = true;
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060001F8 RID: 504 RVA: 0x000074A7 File Offset: 0x000056A7
		// (set) Token: 0x060001F9 RID: 505 RVA: 0x000074AF File Offset: 0x000056AF
		public int Month
		{
			get
			{
				return this._month;
			}
			set
			{
				this._month = ((value < 1) ? 1 : ((value > 12) ? 12 : value));
				this.HasDate = true;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060001FA RID: 506 RVA: 0x000074CF File Offset: 0x000056CF
		// (set) Token: 0x060001FB RID: 507 RVA: 0x000074D7 File Offset: 0x000056D7
		public int Day
		{
			get
			{
				return this._day;
			}
			set
			{
				this._day = ((value < 1) ? 1 : ((value > 31) ? 31 : value));
				this.HasDate = true;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060001FC RID: 508 RVA: 0x000074F7 File Offset: 0x000056F7
		// (set) Token: 0x060001FD RID: 509 RVA: 0x000074FF File Offset: 0x000056FF
		public int Hour
		{
			get
			{
				return this._hour;
			}
			set
			{
				this._hour = Math.Min(Math.Abs(value), 23);
				this.HasTime = true;
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060001FE RID: 510 RVA: 0x0000751B File Offset: 0x0000571B
		// (set) Token: 0x060001FF RID: 511 RVA: 0x00007523 File Offset: 0x00005723
		public int Minute
		{
			get
			{
				return this._minute;
			}
			set
			{
				this._minute = Math.Min(Math.Abs(value), 59);
				this.HasTime = true;
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000200 RID: 512 RVA: 0x0000753F File Offset: 0x0000573F
		// (set) Token: 0x06000201 RID: 513 RVA: 0x00007547 File Offset: 0x00005747
		public int Second
		{
			get
			{
				return this._second;
			}
			set
			{
				this._second = Math.Min(Math.Abs(value), 59);
				this.HasTime = true;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x06000202 RID: 514 RVA: 0x00007563 File Offset: 0x00005763
		// (set) Token: 0x06000203 RID: 515 RVA: 0x0000756B File Offset: 0x0000576B
		public int Nanosecond
		{
			get
			{
				return this._nanoseconds;
			}
			set
			{
				this._nanoseconds = value;
				this.HasTime = true;
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000757C File Offset: 0x0000577C
		public int CompareTo(object dt)
		{
			IXmpDateTime xmpDateTime = (IXmpDateTime)dt;
			long num = this.Calendar.GetTimeInMillis() - xmpDateTime.Calendar.GetTimeInMillis();
			if (num != 0L)
			{
				return Math.Sign(num);
			}
			num = (long)(this._nanoseconds - xmpDateTime.Nanosecond);
			return Math.Sign(num);
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000205 RID: 517 RVA: 0x000075C7 File Offset: 0x000057C7
		// (set) Token: 0x06000206 RID: 518 RVA: 0x000075CF File Offset: 0x000057CF
		public TimeZoneInfo TimeZone
		{
			get
			{
				return this._timeZone;
			}
			set
			{
				this._timeZone = value;
				this.HasTime = true;
				this.HasTimeZone = true;
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x06000207 RID: 519 RVA: 0x000075E6 File Offset: 0x000057E6
		// (set) Token: 0x06000208 RID: 520 RVA: 0x000075EE File Offset: 0x000057EE
		public TimeSpan Offset { get; set; }

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x06000209 RID: 521 RVA: 0x000075F7 File Offset: 0x000057F7
		// (set) Token: 0x0600020A RID: 522 RVA: 0x000075FF File Offset: 0x000057FF
		public bool HasDate { get; private set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600020B RID: 523 RVA: 0x00007608 File Offset: 0x00005808
		// (set) Token: 0x0600020C RID: 524 RVA: 0x00007610 File Offset: 0x00005810
		public bool HasTime { get; private set; }

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x0600020D RID: 525 RVA: 0x00007619 File Offset: 0x00005819
		// (set) Token: 0x0600020E RID: 526 RVA: 0x00007621 File Offset: 0x00005821
		public bool HasTimeZone { get; private set; }

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x0600020F RID: 527 RVA: 0x0000762C File Offset: 0x0000582C
		public global::Sharpen.Calendar Calendar
		{
			get
			{
				global::Sharpen.GregorianCalendar gregorianCalendar = (global::Sharpen.GregorianCalendar)global::Sharpen.Calendar.GetInstance(CultureInfo.InvariantCulture);
				gregorianCalendar.SetGregorianChange(XmpDateTime.UnixTimeToDateTime(long.MinValue));
				if (this.HasTimeZone)
				{
					gregorianCalendar.SetTimeZone(this._timeZone);
				}
				gregorianCalendar.Set(CalendarEnum.Year, this._year);
				gregorianCalendar.Set(CalendarEnum.Month, this._month - 1);
				gregorianCalendar.Set(CalendarEnum.DayOfMonth, this._day);
				gregorianCalendar.Set(CalendarEnum.HourOfDay, this._hour);
				gregorianCalendar.Set(CalendarEnum.Minute, this._minute);
				gregorianCalendar.Set(CalendarEnum.Second, this._second);
				gregorianCalendar.Set(CalendarEnum.Millisecond, this._nanoseconds / 1000000);
				return gregorianCalendar;
			}
		}

		// Token: 0x06000210 RID: 528 RVA: 0x000076D5 File Offset: 0x000058D5
		public string ToIso8601String()
		{
			return Iso8601Converter.Render(this);
		}

		// Token: 0x06000211 RID: 529 RVA: 0x000076DD File Offset: 0x000058DD
		public override string ToString()
		{
			return this.ToIso8601String();
		}

		// Token: 0x06000212 RID: 530 RVA: 0x000076E8 File Offset: 0x000058E8
		internal static DateTime UnixTimeToDateTime(long unixTime)
		{
			return new DateTime(XmpDateTime._unixEpoch.Ticks + unixTime * 10000L);
		}

		// Token: 0x06000213 RID: 531 RVA: 0x00007710 File Offset: 0x00005910
		public static DateTimeOffset UnixTimeToDateTimeOffset(long unixTime)
		{
			return new DateTimeOffset(XmpDateTime._unixEpoch.Ticks + unixTime * 10000L, TimeSpan.Zero);
		}

		// Token: 0x040000EA RID: 234
		private int _year;

		// Token: 0x040000EB RID: 235
		private int _month;

		// Token: 0x040000EC RID: 236
		private int _day;

		// Token: 0x040000ED RID: 237
		private int _hour;

		// Token: 0x040000EE RID: 238
		private int _minute;

		// Token: 0x040000EF RID: 239
		private int _second;

		// Token: 0x040000F0 RID: 240
		private TimeZoneInfo _timeZone;

		// Token: 0x040000F1 RID: 241
		private int _nanoseconds;

		// Token: 0x040000F6 RID: 246
		private static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}
}
