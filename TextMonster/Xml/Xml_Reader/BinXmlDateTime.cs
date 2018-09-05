using System;
using System.Globalization;
using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class BinXmlDateTime
  {

    const int MaxFractionDigits = 7;

    static internal int[] KatmaiTimeScaleMultiplicator = new int[8] {
      10000000,  
      1000000,  
      100000,  
      10000,  
      1000,  
      100,  
      10,  
      1,  
    };

    static void Write2Dig(StringBuilder sb, int val)
    {
      sb.Append((char)('0' + (val / 10)));
      sb.Append((char)('0' + (val % 10)));
    }
    static void Write4DigNeg(StringBuilder sb, int val)
    {
      if (val < 0)
      {
        val = -val;
        sb.Append('-');
      }
      Write2Dig(sb, val / 100);
      Write2Dig(sb, val % 100);
    }

    static void Write3Dec(StringBuilder sb, int val)
    {
      int c3 = val % 10;
      val /= 10;
      int c2 = val % 10;
      val /= 10;
      int c1 = val;
      sb.Append('.');
      sb.Append((char)('0' + c1));
      sb.Append((char)('0' + c2));
      sb.Append((char)('0' + c3));
    }

    static void WriteDate(StringBuilder sb, int yr, int mnth, int day)
    {
      Write4DigNeg(sb, yr);
      sb.Append('-');
      Write2Dig(sb, mnth);
      sb.Append('-');
      Write2Dig(sb, day);
    }

    static void WriteTime(StringBuilder sb, int hr, int min, int sec, int ms)
    {
      Write2Dig(sb, hr);
      sb.Append(':');
      Write2Dig(sb, min);
      sb.Append(':');
      Write2Dig(sb, sec);
      if (ms != 0)
      {
        Write3Dec(sb, ms);
      }
    }

    static void WriteTimeFullPrecision(StringBuilder sb, int hr, int min, int sec, int fraction)
    {
      Write2Dig(sb, hr);
      sb.Append(':');
      Write2Dig(sb, min);
      sb.Append(':');
      Write2Dig(sb, sec);
      if (fraction != 0)
      {
        int fractionDigits = MaxFractionDigits;
        while (fraction % 10 == 0)
        {
          fractionDigits--;
          fraction /= 10;
        }
        char[] charArray = new char[fractionDigits];
        while (fractionDigits > 0)
        {
          fractionDigits--;
          charArray[fractionDigits] = (char)(fraction % 10 + '0');
          fraction /= 10;
        }
        sb.Append('.');
        sb.Append(charArray);
      }
    }

    static void WriteTimeZone(StringBuilder sb, TimeSpan zone)
    {
      bool negTimeZone = true;
      if (zone.Ticks < 0)
      {
        negTimeZone = false;
        zone = zone.Negate();
      }
      WriteTimeZone(sb, negTimeZone, zone.Hours, zone.Minutes);
    }

    static void WriteTimeZone(StringBuilder sb, bool negTimeZone, int hr, int min)
    {
      if (hr == 0 && min == 0)
      {
        sb.Append('Z');
      }
      else
      {
        sb.Append(negTimeZone ? '+' : '-');
        Write2Dig(sb, hr);
        sb.Append(':');
        Write2Dig(sb, min);
      }
    }

    static void BreakDownXsdDateTime(long val, out int yr, out int mnth, out int day, out int hr, out int min, out int sec, out int ms)
    {
      if (val < 0)
        goto Error;
      long date = val / 4; // trim indicator bits
      ms = (int)(date % 1000);
      date /= 1000;
      sec = (int)(date % 60);
      date /= 60;
      min = (int)(date % 60);
      date /= 60;
      hr = (int)(date % 24);
      date /= 24;
      day = (int)(date % 31) + 1;
      date /= 31;
      mnth = (int)(date % 12) + 1;
      date /= 12;
      yr = (int)(date - 9999);
      if (yr < -9999 || yr > 9999)
        goto Error;
      return;
      Error:
      throw new XmlException(Res.SqlTypes_ArithOverflow, (string)null);
    }

    static void BreakDownXsdDate(long val, out int yr, out int mnth, out int day, out bool negTimeZone, out int hr, out int min)
    {
      if (val < 0)
        goto Error;
      val = val / 4; // trim indicator bits
      int totalMin = (int)(val % (29 * 60)) - 60 * 14;
      long totalDays = val / (29 * 60);

      if (negTimeZone = (totalMin < 0))
        totalMin = -totalMin;

      min = totalMin % 60;
      hr = totalMin / 60;

      day = (int)(totalDays % 31) + 1;
      totalDays /= 31;
      mnth = (int)(totalDays % 12) + 1;
      yr = (int)(totalDays / 12) - 9999;
      if (yr < -9999 || yr > 9999)
        goto Error;
      return;
      Error:
      throw new XmlException(Res.SqlTypes_ArithOverflow, (string)null);
    }

    static void BreakDownXsdTime(long val, out int hr, out int min, out int sec, out int ms)
    {
      if (val < 0)
        goto Error;
      val = val / 4; // trim indicator bits
      ms = (int)(val % 1000);
      val /= 1000;
      sec = (int)(val % 60);
      val /= 60;
      min = (int)(val % 60);
      hr = (int)(val / 60);
      if (0 > hr || hr > 23)
        goto Error;
      return;
      Error:
      throw new XmlException(Res.SqlTypes_ArithOverflow, (string)null);
    }

    public static string XsdDateTimeToString(long val)
    {
      int yr; int mnth; int day; int hr; int min; int sec; int ms;
      BreakDownXsdDateTime(val, out yr, out mnth, out day, out hr, out min, out sec, out ms);
      StringBuilder sb = new StringBuilder(20);
      WriteDate(sb, yr, mnth, day);
      sb.Append('T');
      WriteTime(sb, hr, min, sec, ms);
      sb.Append('Z');
      return sb.ToString();
    }
    public static DateTime XsdDateTimeToDateTime(long val)
    {
      int yr; int mnth; int day; int hr; int min; int sec; int ms;
      BreakDownXsdDateTime(val, out yr, out mnth, out day, out hr, out min, out sec, out ms);
      return new DateTime(yr, mnth, day, hr, min, sec, ms, DateTimeKind.Utc);
    }

    public static string XsdDateToString(long val)
    {
      int yr; int mnth; int day; int hr; int min; bool negTimeZ;
      BreakDownXsdDate(val, out yr, out mnth, out day, out negTimeZ, out hr, out min);
      StringBuilder sb = new StringBuilder(20);
      WriteDate(sb, yr, mnth, day);
      WriteTimeZone(sb, negTimeZ, hr, min);
      return sb.ToString();
    }
    public static DateTime XsdDateToDateTime(long val)
    {
      int yr; int mnth; int day; int hr; int min; bool negTimeZ;
      BreakDownXsdDate(val, out yr, out mnth, out day, out negTimeZ, out hr, out min);
      DateTime d = new DateTime(yr, mnth, day, 0, 0, 0, DateTimeKind.Utc);
      // adjust for timezone
      int adj = (negTimeZ ? -1 : 1) * ((hr * 60) + min);
      return TimeZone.CurrentTimeZone.ToLocalTime(d.AddMinutes(adj));
    }

    public static string XsdTimeToString(long val)
    {
      int hr; int min; int sec; int ms;
      BreakDownXsdTime(val, out hr, out min, out sec, out ms);
      StringBuilder sb = new StringBuilder(16);
      WriteTime(sb, hr, min, sec, ms);
      sb.Append('Z');
      return sb.ToString();
    }
    public static DateTime XsdTimeToDateTime(long val)
    {
      int hr; int min; int sec; int ms;
      BreakDownXsdTime(val, out hr, out min, out sec, out ms);
      return new DateTime(1, 1, 1, hr, min, sec, ms, DateTimeKind.Utc);
    }

    public static string SqlDateTimeToString(int dateticks, uint timeticks)
    {
      DateTime dateTime = SqlDateTimeToDateTime(dateticks, timeticks);
      string format = (dateTime.Millisecond != 0) ? "yyyy/MM/dd\\THH:mm:ss.ffff" : "yyyy/MM/dd\\THH:mm:ss";
      return dateTime.ToString(format, CultureInfo.InvariantCulture);
    }
    public static DateTime SqlDateTimeToDateTime(int dateticks, uint timeticks)
    {
      DateTime SQLBaseDate = new DateTime(1900, 1, 1);
      //long millisecond = (long)(((ulong)timeticks * 20 + (ulong)3) / (ulong)6);
      long millisecond = (long)(timeticks / SQLTicksPerMillisecond + 0.5);
      return SQLBaseDate.Add(new TimeSpan(dateticks * TimeSpan.TicksPerDay +
                                          millisecond * TimeSpan.TicksPerMillisecond));
    }

    // Number of (100ns) ticks per time unit
    private static readonly double SQLTicksPerMillisecond = 0.3;
    public static readonly int SQLTicksPerSecond = 300;
    public static readonly int SQLTicksPerMinute = SQLTicksPerSecond * 60;
    public static readonly int SQLTicksPerHour = SQLTicksPerMinute * 60;
    private static readonly int SQLTicksPerDay = SQLTicksPerHour * 24;


    public static string SqlSmallDateTimeToString(short dateticks, ushort timeticks)
    {
      DateTime dateTime = SqlSmallDateTimeToDateTime(dateticks, timeticks);
      return dateTime.ToString("yyyy/MM/dd\\THH:mm:ss", CultureInfo.InvariantCulture);
    }
    public static DateTime SqlSmallDateTimeToDateTime(short dateticks, ushort timeticks)
    {
      return SqlDateTimeToDateTime((int)dateticks, (uint)(timeticks * SQLTicksPerMinute));
    }

    // Conversions of the Katmai date & time types to DateTime
    public static DateTime XsdKatmaiDateToDateTime(byte[] data, int offset)
    {
      // Katmai SQL type "DATE"
      long dateTicks = GetKatmaiDateTicks(data, ref offset);
      DateTime dt = new DateTime(dateTicks);
      return dt;
    }

    public static DateTime XsdKatmaiDateTimeToDateTime(byte[] data, int offset)
    {
      // Katmai SQL type "DATETIME2"
      long timeTicks = GetKatmaiTimeTicks(data, ref offset);
      long dateTicks = GetKatmaiDateTicks(data, ref offset);
      DateTime dt = new DateTime(dateTicks + timeTicks);
      return dt;
    }

    public static DateTime XsdKatmaiTimeToDateTime(byte[] data, int offset)
    {
      // TIME without zone is stored as DATETIME2
      return XsdKatmaiDateTimeToDateTime(data, offset);
    }

    public static DateTime XsdKatmaiDateOffsetToDateTime(byte[] data, int offset)
    {
      // read the timezoned value into DateTimeOffset and then convert to local time
      return XsdKatmaiDateOffsetToDateTimeOffset(data, offset).LocalDateTime;
    }

    public static DateTime XsdKatmaiDateTimeOffsetToDateTime(byte[] data, int offset)
    {
      // read the timezoned value into DateTimeOffset and then convert to local time
      return XsdKatmaiDateTimeOffsetToDateTimeOffset(data, offset).LocalDateTime;
    }

    public static DateTime XsdKatmaiTimeOffsetToDateTime(byte[] data, int offset)
    {
      // read the timezoned value into DateTimeOffset and then convert to local time
      return XsdKatmaiTimeOffsetToDateTimeOffset(data, offset).LocalDateTime;
    }

    // Conversions of the Katmai date & time types to DateTimeOffset
    public static DateTimeOffset XsdKatmaiDateToDateTimeOffset(byte[] data, int offset)
    {
      // read the value into DateTime and then convert it to DateTimeOffset, which adds local time zone
      return (DateTimeOffset)XsdKatmaiDateToDateTime(data, offset);
    }

    public static DateTimeOffset XsdKatmaiDateTimeToDateTimeOffset(byte[] data, int offset)
    {
      // read the value into DateTime and then convert it to DateTimeOffset, which adds local time zone
      return (DateTimeOffset)XsdKatmaiDateTimeToDateTime(data, offset);
    }

    public static DateTimeOffset XsdKatmaiTimeToDateTimeOffset(byte[] data, int offset)
    {
      // read the value into DateTime and then convert it to DateTimeOffset, which adds local time zone
      return (DateTimeOffset)XsdKatmaiTimeToDateTime(data, offset);
    }

    public static DateTimeOffset XsdKatmaiDateOffsetToDateTimeOffset(byte[] data, int offset)
    {
      // DATE with zone is stored as DATETIMEOFFSET
      return XsdKatmaiDateTimeOffsetToDateTimeOffset(data, offset);
    }

    public static DateTimeOffset XsdKatmaiDateTimeOffsetToDateTimeOffset(byte[] data, int offset)
    {
      // Katmai SQL type "DATETIMEOFFSET"
      long timeTicks = GetKatmaiTimeTicks(data, ref offset);
      long dateTicks = GetKatmaiDateTicks(data, ref offset);
      long zoneTicks = GetKatmaiTimeZoneTicks(data, offset);
      // The DATETIMEOFFSET values are serialized in UTC, but DateTimeOffset takes adjusted time -> we need to add zoneTicks
      DateTimeOffset dto = new DateTimeOffset(dateTicks + timeTicks + zoneTicks, new TimeSpan(zoneTicks));
      return dto;
    }

    public static DateTimeOffset XsdKatmaiTimeOffsetToDateTimeOffset(byte[] data, int offset)
    {
      // TIME with zone is stored as DATETIMEOFFSET
      return XsdKatmaiDateTimeOffsetToDateTimeOffset(data, offset);
    }

    // Conversions of the Katmai date & time types to string
    public static string XsdKatmaiDateToString(byte[] data, int offset)
    {
      DateTime dt = XsdKatmaiDateToDateTime(data, offset);
      StringBuilder sb = new StringBuilder(10);
      WriteDate(sb, dt.Year, dt.Month, dt.Day);
      return sb.ToString();
    }

    public static string XsdKatmaiDateTimeToString(byte[] data, int offset)
    {
      DateTime dt = XsdKatmaiDateTimeToDateTime(data, offset);
      StringBuilder sb = new StringBuilder(33);
      WriteDate(sb, dt.Year, dt.Month, dt.Day);
      sb.Append('T');
      WriteTimeFullPrecision(sb, dt.Hour, dt.Minute, dt.Second, GetFractions(dt));
      return sb.ToString();
    }

    public static string XsdKatmaiTimeToString(byte[] data, int offset)
    {
      DateTime dt = XsdKatmaiTimeToDateTime(data, offset);
      StringBuilder sb = new StringBuilder(16);
      WriteTimeFullPrecision(sb, dt.Hour, dt.Minute, dt.Second, GetFractions(dt));
      return sb.ToString();
    }

    public static string XsdKatmaiDateOffsetToString(byte[] data, int offset)
    {
      DateTimeOffset dto = XsdKatmaiDateOffsetToDateTimeOffset(data, offset);
      StringBuilder sb = new StringBuilder(16);
      WriteDate(sb, dto.Year, dto.Month, dto.Day);
      WriteTimeZone(sb, dto.Offset);
      return sb.ToString();
    }

    public static string XsdKatmaiDateTimeOffsetToString(byte[] data, int offset)
    {
      DateTimeOffset dto = XsdKatmaiDateTimeOffsetToDateTimeOffset(data, offset);
      StringBuilder sb = new StringBuilder(39);
      WriteDate(sb, dto.Year, dto.Month, dto.Day);
      sb.Append('T');
      WriteTimeFullPrecision(sb, dto.Hour, dto.Minute, dto.Second, GetFractions(dto));
      WriteTimeZone(sb, dto.Offset);
      return sb.ToString();
    }

    public static string XsdKatmaiTimeOffsetToString(byte[] data, int offset)
    {
      DateTimeOffset dto = XsdKatmaiTimeOffsetToDateTimeOffset(data, offset);
      StringBuilder sb = new StringBuilder(22);
      WriteTimeFullPrecision(sb, dto.Hour, dto.Minute, dto.Second, GetFractions(dto));
      WriteTimeZone(sb, dto.Offset);
      return sb.ToString();
    }

    // Helper methods for the Katmai date & time types
    static long GetKatmaiDateTicks(byte[] data, ref int pos)
    {
      int p = pos;
      pos = p + 3;
      return (data[p] | data[p + 1] << 8 | data[p + 2] << 16) * TimeSpan.TicksPerDay;
    }

    static long GetKatmaiTimeTicks(byte[] data, ref int pos)
    {
      int p = pos;
      byte scale = data[p];
      long timeTicks;
      p++;
      if (scale <= 2)
      {
        timeTicks = data[p] | (data[p + 1] << 8) | (data[p + 2] << 16);
        pos = p + 3;
      }
      else if (scale <= 4)
      {
        timeTicks = data[p] | (data[p + 1] << 8) | (data[p + 2] << 16);
        timeTicks |= ((long)data[p + 3] << 24);
        pos = p + 4;
      }
      else if (scale <= 7)
      {
        timeTicks = data[p] | (data[p + 1] << 8) | (data[p + 2] << 16);
        timeTicks |= ((long)data[p + 3] << 24) | ((long)data[p + 4] << 32);
        pos = p + 5;
      }
      else
      {
        throw new XmlException(Res.SqlTypes_ArithOverflow, (string)null);
      }
      return timeTicks * KatmaiTimeScaleMultiplicator[scale];
    }

    static long GetKatmaiTimeZoneTicks(byte[] data, int pos)
    {
      return (short)(data[pos] | data[pos + 1] << 8) * TimeSpan.TicksPerMinute;
    }

    static int GetFractions(DateTime dt)
    {
      return (int)(dt.Ticks - new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second).Ticks);
    }

    static int GetFractions(DateTimeOffset dt)
    {
      return (int)(dt.Ticks - new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second).Ticks);
    }
  }
}