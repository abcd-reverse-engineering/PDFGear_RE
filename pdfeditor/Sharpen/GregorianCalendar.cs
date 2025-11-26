using System;

namespace Sharpen
{
	// Token: 0x02000012 RID: 18
	public class GregorianCalendar : Calendar
	{
		// Token: 0x06000049 RID: 73 RVA: 0x00002DE5 File Offset: 0x00000FE5
		public GregorianCalendar()
		{
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002DED File Offset: 0x00000FED
		public GregorianCalendar(TimeZoneInfo timeZoneInfo)
			: base(timeZoneInfo)
		{
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002DF6 File Offset: 0x00000FF6
		public GregorianCalendar(int year, int month, int day)
			: base(year, month, day)
		{
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002E01 File Offset: 0x00001001
		public GregorianCalendar(int year, int month, int dayOfMonth, int hourOfDay, int minute, int second)
			: base(year, month, dayOfMonth, hourOfDay, minute, second)
		{
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002E12 File Offset: 0x00001012
		public void SetGregorianChange(DateTime date)
		{
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002E14 File Offset: 0x00001014
		public override int GetMaximum(CalendarEnum field)
		{
			switch (field)
			{
			case CalendarEnum.Year:
				return DateTime.MaxValue.Year;
			case CalendarEnum.Month:
				return 11;
			case CalendarEnum.MonthOneBased:
				return 12;
			case CalendarEnum.DayOfMonth:
				return DateTime.DaysInMonth(base.GetTime().Year, base.GetTime().Month);
			case CalendarEnum.Hour:
				return 23;
			case CalendarEnum.HourOfDay:
				return 23;
			case CalendarEnum.Minute:
				return 60;
			case CalendarEnum.Second:
				return 60;
			case CalendarEnum.Millisecond:
				return 999;
			default:
				throw new NotSupportedException();
			}
		}

		// Token: 0x04000026 RID: 38
		public const int January = 0;
	}
}
