using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  ///   The <see cref="XmlCustomFormatter"/> class provides a set of static methods for converting
  ///   primitive type values to and from their XML string representations.</summary>
  internal class XmlCustomFormatter
  {
    private XmlCustomFormatter() { }
    internal static string FromDefaultValue(object value, string formatter)
    {
      if (value == null) return null;
      Type type = value.GetType();
      if (type == typeof(DateTime))
      {
        if (formatter == "DateTime")
        {
          return FromDateTime((DateTime)value);
        }
        if (formatter == "Date")
        {
          return FromDate((DateTime)value);
        }
        if (formatter == "Time")
        {
          return FromTime((DateTime)value);
        }
      }
      else if (type == typeof(string))
      {
        if (formatter == "XmlName")
        {
          return FromXmlName((string)value);
        }
        if (formatter == "XmlNCName")
        {
          return FromXmlNCName((string)value);
        }
        if (formatter == "XmlNmToken")
        {
          return FromXmlNmToken((string)value);
        }
        if (formatter == "XmlNmTokens")
        {
          return FromXmlNmTokens((string)value);
        }
      }
      throw new Exception(Res.GetString(Res.XmlUnsupportedDefaultType, type.FullName));
    }

    internal static string FromDate(DateTime value)
    {
      return XmlConvert.ToString(value, "yyyy-MM-dd");
    }

    internal static string FromTime(DateTime value)
    {
      return XmlConvert.ToString(DateTime.MinValue + value.TimeOfDay, "HH:mm:ss.fffffffzzzzzz");
    }

    internal static string FromDateTime(DateTime value)
    {
      return XmlConvert.ToString(value, "yyyy-MM-ddTHH:mm:ss.fffffffzzzzzz");
    }

    internal static string FromChar(char value)
    {
      return XmlConvert.ToString((UInt16)value);
    }

    internal static string FromXmlName(string name)
    {
      return XmlConvert.EncodeName(name);
    }

    internal static string FromXmlNCName(string ncName)
    {
      return XmlConvert.EncodeLocalName(ncName);
    }

    internal static string FromXmlNmToken(string nmToken)
    {
      return XmlConvert.EncodeNmToken(nmToken);
    }

    internal static string FromXmlNmTokens(string nmTokens)
    {
      if (nmTokens == null)
        return null;
      if (nmTokens.IndexOf(' ') < 0)
        return FromXmlNmToken(nmTokens);
      else
      {
        string[] toks = nmTokens.Split(new char[] { ' ' });
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < toks.Length; i++)
        {
          if (i > 0) sb.Append(' ');
          sb.Append(FromXmlNmToken(toks[i]));
        }
        return sb.ToString();
      }
    }

    internal static void WriteArrayBase64(XmlWriter writer, byte[] inData, int start, int count)
    {
      if (inData == null || count == 0)
      {
        return;
      }
      writer.WriteBase64(inData, start, count);
    }

    internal static string FromEnum(long val, string[] vals, long[] ids, string typeName)
    {
      long originalValue = val;
      StringBuilder sb = new StringBuilder();
      int iZero = -1;

      for (int i = 0; i < ids.Length; i++)
      {
        if (ids[i] == 0)
        {
          iZero = i;
          continue;
        }
        if (val == 0)
        {
          break;
        }
        if ((ids[i] & originalValue) == ids[i])
        {
          if (sb.Length != 0)
            sb.Append(" ");
          sb.Append(vals[i]);
          val &= ~ids[i];
        }
      }
      if (val != 0)
      {
        // failed to parse the enum value
        throw new InvalidOperationException(Res.GetString(Res.XmlUnknownConstant, originalValue, typeName == null ? "enum" : typeName));
      }
      if (sb.Length == 0 && iZero >= 0)
      {
        sb.Append(vals[iZero]);
      }
      return sb.ToString();
    }

    internal static object ToDefaultValue(string value, string formatter)
    {
      if (formatter == "DateTime")
      {
        return ToDateTime(value);
      }
      if (formatter == "Date")
      {
        return ToDate(value);
      }
      if (formatter == "Time")
      {
        return ToTime(value);
      }
      if (formatter == "XmlName")
      {
        return ToXmlName(value);
      }
      if (formatter == "XmlNCName")
      {
        return ToXmlNCName(value);
      }
      if (formatter == "XmlNmToken")
      {
        return ToXmlNmToken(value);
      }
      if (formatter == "XmlNmTokens")
      {
        return ToXmlNmTokens(value);
      }
      throw new Exception(Res.GetString(Res.XmlUnsupportedDefaultValue, formatter));
      //            Debug.WriteLineIf(CompModSwitches.XmlSerialization.TraceVerbose, "XmlSerialization::Unhandled default value " + value + " formatter " + formatter);
      //            return DBNull.Value;
    }

    static string[] allDateFormats = new string[] {
      "yyyy-MM-ddzzzzzz",
      "yyyy-MM-dd",
      "yyyy-MM-ddZ",
      "yyyy",
      "---dd",
      "---ddZ",
      "---ddzzzzzz",
      "--MM-dd",
      "--MM-ddZ",
      "--MM-ddzzzzzz",
      "--MM--",
      "--MM--Z",
      "--MM--zzzzzz",
      "yyyy-MM",
      "yyyy-MMZ",
      "yyyy-MMzzzzzz",
      "yyyyzzzzzz",
    };

    static string[] allTimeFormats = new string[] {
      "HH:mm:ss.fffffffzzzzzz",
      "HH:mm:ss",
      "HH:mm:ss.f",
      "HH:mm:ss.ff",
      "HH:mm:ss.fff",
      "HH:mm:ss.ffff",
      "HH:mm:ss.fffff",
      "HH:mm:ss.ffffff",
      "HH:mm:ss.fffffff",
      "HH:mm:ssZ",
      "HH:mm:ss.fZ",
      "HH:mm:ss.ffZ",
      "HH:mm:ss.fffZ",
      "HH:mm:ss.ffffZ",
      "HH:mm:ss.fffffZ",
      "HH:mm:ss.ffffffZ",
      "HH:mm:ss.fffffffZ",
      "HH:mm:sszzzzzz",
      "HH:mm:ss.fzzzzzz",
      "HH:mm:ss.ffzzzzzz",
      "HH:mm:ss.fffzzzzzz",
      "HH:mm:ss.ffffzzzzzz",
      "HH:mm:ss.fffffzzzzzz",
      "HH:mm:ss.ffffffzzzzzz",
    };

    internal static DateTime ToDateTime(string value)
    {
      // for mode DateTimeSerializationMode.Roundtrip and DateTimeSerializationMode.Default
      return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind);
    }

    internal static DateTime ToDateTime(string value, string[] formats)
    {
      return XmlConvert.ToDateTime(value, formats);
    }

    internal static DateTime ToDate(string value)
    {
      return ToDateTime(value, allDateFormats);
    }

    internal static DateTime ToTime(string value)
    {
      return DateTime.ParseExact(value, allTimeFormats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.RoundtripKind);
    }

    internal static string ToXmlName(string value)
    {
      return XmlConvert.DecodeName(CollapseWhitespace(value));
    }

    internal static string ToXmlNCName(string value)
    {
      return XmlConvert.DecodeName(CollapseWhitespace(value));
    }

    internal static string ToXmlNmToken(string value)
    {
      return XmlConvert.DecodeName(CollapseWhitespace(value));
    }

    internal static string ToXmlNmTokens(string value)
    {
      return XmlConvert.DecodeName(CollapseWhitespace(value));
    }

    internal static long ToEnum(string val, Hashtable vals, string typeName, bool validate)
    {
      long value = 0;
      string[] parts = val.Split(null);
      for (int i = 0; i < parts.Length; i++)
      {
        object id = vals[parts[i]];
        if (id != null)
          value |= (long)id;
        else if (validate && parts[i].Length > 0)
          throw new InvalidOperationException(Res.GetString(Res.XmlUnknownConstant, parts[i], typeName));
      }
      return value;
    }

    static string CollapseWhitespace(string value)
    {
      if (value == null)
        return null;
      return value.Trim();
    }
  }
}