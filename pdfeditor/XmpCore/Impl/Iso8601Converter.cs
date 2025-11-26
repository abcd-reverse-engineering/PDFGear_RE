using System;
using System.Text;

namespace XmpCore.Impl
{
	// Token: 0x0200002E RID: 46
	public static class Iso8601Converter
	{
		// Token: 0x060001AF RID: 431 RVA: 0x00004893 File Offset: 0x00002A93
		public static IXmpDateTime Parse(string iso8601String)
		{
			return Iso8601Converter.Parse(iso8601String, new XmpDateTime());
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x000048A0 File Offset: 0x00002AA0
		public static IXmpDateTime Parse(string iso8601String, IXmpDateTime binValue)
		{
			if (iso8601String == null)
			{
				throw new XmpException("Parameter must not be null", XmpErrorCode.BadParam);
			}
			if (iso8601String.Length == 0)
			{
				return binValue;
			}
			ParseState parseState = new ParseState(iso8601String);
			if (parseState.Ch(0) == '-')
			{
				parseState.Skip();
			}
			int num = parseState.GatherInt("Invalid year in date string", 9999);
			if (parseState.HasNext && parseState.Ch() != '-')
			{
				throw new XmpException("Invalid date string, after year", XmpErrorCode.BadValue);
			}
			if (parseState.Ch(0) == '-')
			{
				num = -num;
			}
			binValue.Year = num;
			if (!parseState.HasNext)
			{
				return binValue;
			}
			parseState.Skip();
			num = parseState.GatherInt("Invalid month in date string", 12);
			if (parseState.HasNext && parseState.Ch() != '-')
			{
				throw new XmpException("Invalid date string, after month", XmpErrorCode.BadValue);
			}
			binValue.Month = num;
			if (!parseState.HasNext)
			{
				return binValue;
			}
			parseState.Skip();
			num = parseState.GatherInt("Invalid day in date string", 31);
			if (parseState.HasNext && parseState.Ch() != 'T')
			{
				throw new XmpException("Invalid date string, after day", XmpErrorCode.BadValue);
			}
			binValue.Day = num;
			if (!parseState.HasNext)
			{
				return binValue;
			}
			parseState.Skip();
			num = parseState.GatherInt("Invalid hour in date string", 23);
			binValue.Hour = num;
			if (!parseState.HasNext)
			{
				return binValue;
			}
			if (parseState.Ch() == ':')
			{
				parseState.Skip();
				num = parseState.GatherInt("Invalid minute in date string", 59);
				if (parseState.HasNext && parseState.Ch() != ':' && parseState.Ch() != 'Z' && parseState.Ch() != '+' && parseState.Ch() != '-')
				{
					throw new XmpException("Invalid date string, after minute", XmpErrorCode.BadValue);
				}
				binValue.Minute = num;
			}
			if (!parseState.HasNext)
			{
				return binValue;
			}
			if (parseState.HasNext && parseState.Ch() == ':')
			{
				parseState.Skip();
				num = parseState.GatherInt("Invalid whole seconds in date string", 59);
				if (parseState.HasNext && parseState.Ch() != '.' && parseState.Ch() != 'Z' && parseState.Ch() != '+' && parseState.Ch() != '-')
				{
					throw new XmpException("Invalid date string, after whole seconds", XmpErrorCode.BadValue);
				}
				binValue.Second = num;
				if (parseState.Ch() == '.')
				{
					parseState.Skip();
					int i = parseState.Pos;
					num = parseState.GatherInt("Invalid fractional seconds in date string", 999999999);
					if (parseState.HasNext && parseState.Ch() != 'Z' && parseState.Ch() != '+' && parseState.Ch() != '-')
					{
						throw new XmpException("Invalid date string, after fractional second", XmpErrorCode.BadValue);
					}
					for (i = parseState.Pos - i; i > 9; i--)
					{
						num /= 10;
					}
					while (i < 9)
					{
						num *= 10;
						i++;
					}
					binValue.Nanosecond = num;
				}
			}
			else if (parseState.Ch() != 'Z' && parseState.Ch() != '+' && parseState.Ch() != '-')
			{
				throw new XmpException("Invalid date string, after time", XmpErrorCode.BadValue);
			}
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (!parseState.HasNext)
			{
				return binValue;
			}
			if (parseState.Ch() == 'Z')
			{
				parseState.Skip();
			}
			else if (parseState.HasNext)
			{
				char c = parseState.Ch();
				if (c != '+')
				{
					if (c != '-')
					{
						throw new XmpException("Time zone must begin with 'Z', '+', or '-'", XmpErrorCode.BadValue);
					}
					num2 = -1;
				}
				else
				{
					num2 = 1;
				}
				parseState.Skip();
				num3 = parseState.GatherInt("Invalid time zone hour in date string", 23);
				if (parseState.HasNext)
				{
					if (parseState.Ch() != ':')
					{
						throw new XmpException("Invalid date string, after time zone hour", XmpErrorCode.BadValue);
					}
					parseState.Skip();
					num4 = parseState.GatherInt("Invalid time zone minute in date string", 59);
				}
			}
			TimeSpan timeSpan = TimeSpan.FromHours((double)num3) + TimeSpan.FromMinutes((double)num4);
			if (num2 < 0)
			{
				timeSpan = -timeSpan;
			}
			binValue.TimeZone = TimeZoneInfo.Local;
			binValue.Offset = timeSpan;
			if (parseState.HasNext)
			{
				throw new XmpException("Invalid date string, extra chars at end", XmpErrorCode.BadValue);
			}
			return binValue;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00004C5C File Offset: 0x00002E5C
		public static string Render(IXmpDateTime dateTime)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (dateTime.HasDate)
			{
				stringBuilder.Append(dateTime.Year.ToString("0000"));
				if (dateTime.Month == 0)
				{
					return stringBuilder.ToString();
				}
				stringBuilder.Append('-');
				stringBuilder.Append(dateTime.Month.ToString("00"));
				if (dateTime.Day == 0)
				{
					return stringBuilder.ToString();
				}
				stringBuilder.Append('-');
				stringBuilder.Append(dateTime.Day.ToString("00"));
				if (dateTime.HasTime)
				{
					stringBuilder.Append('T');
					stringBuilder.Append(dateTime.Hour.ToString("00"));
					stringBuilder.Append(':');
					stringBuilder.Append(dateTime.Minute.ToString("00"));
					if (dateTime.Second != 0 || dateTime.Nanosecond != 0)
					{
						stringBuilder.Append(':');
						double num = (double)dateTime.Second + (double)dateTime.Nanosecond / 1000000000.0;
						stringBuilder.AppendFormat("{0:00.#########}", num);
					}
					if (dateTime.HasTimeZone)
					{
						long timeInMillis = dateTime.Calendar.GetTimeInMillis();
						int num2 = (int)dateTime.TimeZone.GetUtcOffset(XmpDateTime.UnixTimeToDateTimeOffset(timeInMillis).DateTime).TotalMilliseconds;
						if (num2 == 0)
						{
							stringBuilder.Append('Z');
						}
						else
						{
							int num3 = num2 / 3600000;
							int num4 = Math.Abs(num2 % 3600000 / 60000);
							stringBuilder.Append(num3.ToString("+00;-00"));
							stringBuilder.Append(num4.ToString(":00"));
						}
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
