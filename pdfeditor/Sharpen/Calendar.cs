using System;
using System.Globalization;

namespace Sharpen
{
	// Token: 0x0200000E RID: 14
	public abstract class Calendar
	{
		// Token: 0x06000033 RID: 51 RVA: 0x000028E6 File Offset: 0x00000AE6
		protected Calendar(TimeZoneInfo value)
		{
			this._mTz = value;
			this._mCalendarDate = TimeZoneInfo.ConvertTime(DateTime.Now, this._mTz);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x0000290C File Offset: 0x00000B0C
		protected Calendar()
		{
			this._mTz = Calendar.DefaultTimeZone;
			this._mCalendarDate = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002943 File Offset: 0x00000B43
		protected Calendar(int year, int month, int dayOfMonth)
		{
			this._mTz = Calendar.DefaultTimeZone;
			this._mCalendarDate = new DateTime(year, month + 1, dayOfMonth);
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002968 File Offset: 0x00000B68
		protected Calendar(int year, int month, int dayOfMonth, int hourOfDay, int minute, int second)
		{
			this._mTz = Calendar.DefaultTimeZone;
			bool flag = false;
			if (hourOfDay == 24)
			{
				hourOfDay = 0;
				flag = true;
			}
			this._mCalendarDate = new DateTime(year, month + 1, dayOfMonth, hourOfDay, minute, second);
			if (flag)
			{
				this._mCalendarDate = this._mCalendarDate.AddDays(1.0);
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x000029C8 File Offset: 0x00000BC8
		public long GetTimeInMillis()
		{
			return this.GetTime().Ticks / 10000L;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000029EC File Offset: 0x00000BEC
		public void SetTimeInMillis(long millis)
		{
			long num = millis * 10000L;
			this._mCalendarDate = new DateTime(num, DateTimeKind.Unspecified);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002A0F File Offset: 0x00000C0F
		public DateTime GetTime()
		{
			return TimeZoneInfo.ConvertTime(this._mCalendarDate, Calendar.DefaultTimeZone);
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002A21 File Offset: 0x00000C21
		public void SetTime(DateTime date)
		{
			this._mCalendarDate = date;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002A2A File Offset: 0x00000C2A
		public TimeZoneInfo GetTimeZone()
		{
			return this._mTz;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002A32 File Offset: 0x00000C32
		public void SetTimeZone(TimeZoneInfo value)
		{
			this._mTz = value;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002A3C File Offset: 0x00000C3C
		public int Get(CalendarEnum field)
		{
			switch (field)
			{
			case CalendarEnum.Year:
				return this._mCalendarDate.Year;
			case CalendarEnum.Month:
				return this._mCalendarDate.Month - 1;
			case CalendarEnum.MonthOneBased:
				return this._mCalendarDate.Month;
			case CalendarEnum.DayOfMonth:
				return this._mCalendarDate.Day;
			case CalendarEnum.Hour:
			case CalendarEnum.HourOfDay:
				return this._mCalendarDate.Hour;
			case CalendarEnum.Minute:
				return this._mCalendarDate.Minute;
			case CalendarEnum.Second:
				return this._mCalendarDate.Second;
			case CalendarEnum.Millisecond:
				return this._mCalendarDate.Millisecond;
			default:
				throw new NotSupportedException();
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002ADC File Offset: 0x00000CDC
		public void Set(CalendarEnum field, int value)
		{
			int num = this.GetMaximum(field) + 1;
			switch (field)
			{
			case CalendarEnum.Year:
				value %= num;
				this._mCalendarDate = this._mCalendarDate.AddYears(value - this._mCalendarDate.Year);
				return;
			case CalendarEnum.Month:
				this._mCalendarDate = this._mCalendarDate.AddMonths(value + 1 - this._mCalendarDate.Month);
				return;
			case CalendarEnum.MonthOneBased:
				this._mCalendarDate = this._mCalendarDate.AddMonths(value - this._mCalendarDate.Month);
				return;
			case CalendarEnum.DayOfMonth:
				this._mCalendarDate = this._mCalendarDate.AddDays((double)(value - this._mCalendarDate.Day));
				return;
			case CalendarEnum.Hour:
				this._mCalendarDate = this._mCalendarDate.AddHours((double)(value - this._mCalendarDate.Hour));
				return;
			case CalendarEnum.HourOfDay:
				if (value == 24)
				{
					this.Set(CalendarEnum.Hour, 0);
					this._mCalendarDate = this._mCalendarDate.AddDays(1.0);
					return;
				}
				this.Set(CalendarEnum.Hour, value);
				return;
			case CalendarEnum.Minute:
				this._mCalendarDate = this._mCalendarDate.AddMinutes((double)(value - this._mCalendarDate.Minute));
				return;
			case CalendarEnum.Second:
				this._mCalendarDate = this._mCalendarDate.AddSeconds((double)(value - this._mCalendarDate.Second));
				return;
			case CalendarEnum.Millisecond:
				this._mCalendarDate = new DateTime(this._mCalendarDate.Year, this._mCalendarDate.Month, this._mCalendarDate.Day, this._mCalendarDate.Hour, this._mCalendarDate.Minute, this._mCalendarDate.Second, value, this._mCalendarDate.Kind);
				return;
			default:
				throw new NotSupportedException();
			}
		}

		// Token: 0x0600003F RID: 63
		public abstract int GetMaximum(CalendarEnum field);

		// Token: 0x06000040 RID: 64 RVA: 0x00002C96 File Offset: 0x00000E96
		public void Set(int year, int month, int day, int hourOfDay, int minute, int second)
		{
			this.Set(CalendarEnum.Year, year);
			this.Set(CalendarEnum.Month, month);
			this.Set(CalendarEnum.DayOfMonth, day);
			this.Set(CalendarEnum.HourOfDay, hourOfDay);
			this.Set(CalendarEnum.Minute, minute);
			this.Set(CalendarEnum.Second, second);
			this.Set(CalendarEnum.Millisecond, 0);
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002CD3 File Offset: 0x00000ED3
		public static Calendar GetInstance(CultureInfo culture)
		{
			return new GregorianCalendar();
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002CDA File Offset: 0x00000EDA
		public static Calendar GetInstance(TimeZoneInfo value)
		{
			return new GregorianCalendar(value);
		}

		// Token: 0x0400001C RID: 28
		private static readonly TimeZoneInfo DefaultTimeZone = TimeZoneInfo.Local;

		// Token: 0x0400001D RID: 29
		private DateTime _mCalendarDate;

		// Token: 0x0400001E RID: 30
		private TimeZoneInfo _mTz;
	}
}
