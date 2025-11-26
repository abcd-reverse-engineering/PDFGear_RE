using System;
using Sharpen;
using XmpCore.Impl;

namespace XmpCore
{
	// Token: 0x0200001F RID: 31
	public static class XmpDateTimeFactory
	{
		// Token: 0x060000CC RID: 204 RVA: 0x000030E0 File Offset: 0x000012E0
		public static IXmpDateTime CreateFromCalendar(Calendar calendar)
		{
			return new XmpDateTime(calendar);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x000030E8 File Offset: 0x000012E8
		public static IXmpDateTime Create()
		{
			return new XmpDateTime();
		}

		// Token: 0x060000CE RID: 206 RVA: 0x000030EF File Offset: 0x000012EF
		public static IXmpDateTime Create(int year, int month, int day)
		{
			return new XmpDateTime
			{
				Year = year,
				Month = month,
				Day = day
			};
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0000310B File Offset: 0x0000130B
		public static IXmpDateTime Create(int year, int month, int day, int hour, int minute, int second, int nanoSecond)
		{
			return new XmpDateTime
			{
				Year = year,
				Month = month,
				Day = day,
				Hour = hour,
				Minute = minute,
				Second = second,
				Nanosecond = nanoSecond
			};
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00003146 File Offset: 0x00001346
		public static IXmpDateTime CreateFromIso8601(string strValue)
		{
			return new XmpDateTime(strValue);
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000314E File Offset: 0x0000134E
		public static IXmpDateTime GetCurrentDateTime()
		{
			return new XmpDateTime(new GregorianCalendar());
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000315A File Offset: 0x0000135A
		public static IXmpDateTime SetLocalTimeZone(IXmpDateTime dateTime)
		{
			Calendar calendar = dateTime.Calendar;
			calendar.SetTimeZone(TimeZoneInfo.Local);
			return new XmpDateTime(calendar);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00003174 File Offset: 0x00001374
		public static IXmpDateTime ConvertToUtcTime(IXmpDateTime dateTime)
		{
			long timeInMillis = dateTime.Calendar.GetTimeInMillis();
			GregorianCalendar gregorianCalendar = new GregorianCalendar(TimeZoneInfo.Utc);
			gregorianCalendar.SetGregorianChange(XmpDateTime.UnixTimeToDateTime(long.MinValue));
			gregorianCalendar.SetTimeInMillis(timeInMillis);
			return new XmpDateTime(gregorianCalendar);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x000031B8 File Offset: 0x000013B8
		public static IXmpDateTime ConvertToLocalTime(IXmpDateTime dateTime)
		{
			long timeInMillis = dateTime.Calendar.GetTimeInMillis();
			GregorianCalendar gregorianCalendar = new GregorianCalendar();
			gregorianCalendar.SetTimeInMillis(timeInMillis);
			return new XmpDateTime(gregorianCalendar);
		}
	}
}
