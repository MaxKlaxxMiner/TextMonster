using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert"]/*' />
  /// <devdoc>
  ///    Encodes and decodes XML names according to
  ///    the "Encoding of arbitrary Unicode Characters in XML Names" specification.
  /// </devdoc>
  public class XmlConvert
  {

    //
    // Static fields with implicit initialization
    //
    static XmlCharType xmlCharType = XmlCharType.Instance;

    internal static char[] crt = new char[] { '\n', '\r', '\t' };

    /// <include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.EncodeName"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Converts names, such
    ///       as DataTable or
    ///       DataColumn names, that contain characters that are not permitted in
    ///       XML names to valid names.</para>
    /// </devdoc>
    public static string EncodeName(string name)
    {
      return EncodeName(name, true/*Name_not_NmToken*/, false/*Local?*/);
    }

    /// <include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.EncodeNmToken"]/*' />
    /// <devdoc>
    ///    <para> Verifies the name is valid
    ///       according to production [7] in the XML spec.</para>
    /// </devdoc>
    public static string EncodeNmToken(string name)
    {
      return EncodeName(name, false/*Name_not_NmToken*/, false/*Local?*/);
    }

    /// <include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.EncodeLocalName"]/*' />
    /// <devdoc>
    ///    <para>Converts names, such as DataTable or DataColumn names, that contain
    ///       characters that are not permitted in XML names to valid names.</para>
    /// </devdoc>
    public static string EncodeLocalName(string name)
    {
      return EncodeName(name, true/*Name_not_NmToken*/, true/*Local?*/);
    }

    /// <include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.DecodeName"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Transforms an XML name into an object name (such as DataTable or DataColumn).</para>
    /// </devdoc>

    public static string DecodeName(string name)
    {
      if (name == null || name.Length == 0)
        return name;

      StringBuilder bufBld = null;

      int length = name.Length;
      int copyPosition = 0;

      int underscorePos = name.IndexOf('_');
      MatchCollection mc = null;
      IEnumerator en = null;
      if (underscorePos >= 0)
      {
        if (c_DecodeCharPattern == null)
        {
          c_DecodeCharPattern = new Regex("_[Xx]([0-9a-fA-F]{4}|[0-9a-fA-F]{8})_");
        }
        mc = c_DecodeCharPattern.Matches(name, underscorePos);
        en = mc.GetEnumerator();
      }
      else
      {
        return name;
      }
      int matchPos = -1;
      if (en != null && en.MoveNext())
      {
        Match m = (Match)en.Current;
        matchPos = m.Index;
      }

      for (int position = 0; position < length - c_EncodedCharLength + 1; position++)
      {
        if (position == matchPos)
        {
          if (en.MoveNext())
          {
            Match m = (Match)en.Current;
            matchPos = m.Index;
          }

          if (bufBld == null)
          {
            bufBld = new StringBuilder(length + 20);
          }
          bufBld.Append(name, copyPosition, position - copyPosition);

          if (name[position + 6] != '_')
          { //_x1234_

            Int32 u =
                FromHex(name[position + 2]) * 0x10000000 +
                FromHex(name[position + 3]) * 0x1000000 +
                FromHex(name[position + 4]) * 0x100000 +
                FromHex(name[position + 5]) * 0x10000 +

                FromHex(name[position + 6]) * 0x1000 +
                FromHex(name[position + 7]) * 0x100 +
                FromHex(name[position + 8]) * 0x10 +
                FromHex(name[position + 9]);

            if (u >= 0x00010000)
            {
              if (u <= 0x0010ffff)
              { //convert to two chars
                copyPosition = position + c_EncodedCharLength + 4;
                char lowChar, highChar;
                XmlCharType.SplitSurrogateChar(u, out lowChar, out highChar);
                bufBld.Append(highChar);
                bufBld.Append(lowChar);
              }
              //else bad ucs-4 char dont convert
            }
            else
            { //convert to single char
              copyPosition = position + c_EncodedCharLength + 4;
              bufBld.Append((char)u);
            }
            position += c_EncodedCharLength - 1 + 4; //just skip

          }
          else
          {
            copyPosition = position + c_EncodedCharLength;
            bufBld.Append((char)(
                FromHex(name[position + 2]) * 0x1000 +
                FromHex(name[position + 3]) * 0x100 +
                FromHex(name[position + 4]) * 0x10 +
                FromHex(name[position + 5])));
            position += c_EncodedCharLength - 1;
          }
        }
      }
      if (copyPosition == 0)
      {
        return name;
      }
      else
      {
        if (copyPosition < length)
        {
          bufBld.Append(name, copyPosition, length - copyPosition);
        }
        return bufBld.ToString();
      }
    }

    private static string EncodeName(string name, /*Name_not_NmToken*/ bool first, bool local)
    {
      if (string.IsNullOrEmpty(name))
      {
        return name;
      }

      StringBuilder bufBld = null;
      int length = name.Length;
      int copyPosition = 0;
      int position = 0;

      int underscorePos = name.IndexOf('_');
      MatchCollection mc = null;
      IEnumerator en = null;
      if (underscorePos >= 0)
      {
        if (c_EncodeCharPattern == null)
        {
          c_EncodeCharPattern = new Regex("(?<=_)[Xx]([0-9a-fA-F]{4}|[0-9a-fA-F]{8})_");
        }
        mc = c_EncodeCharPattern.Matches(name, underscorePos);
        en = mc.GetEnumerator();
      }

      int matchPos = -1;
      if (en != null && en.MoveNext())
      {
        Match m = (Match)en.Current;
        matchPos = m.Index - 1;
      }
      if (first)
      {
        if ((!xmlCharType.IsStartNCNameCharXml4e(name[0]) && (local || (!local && name[0] != ':'))) ||
             matchPos == 0)
        {

          if (bufBld == null)
          {
            bufBld = new StringBuilder(length + 20);
          }
          bufBld.Append("_x");
          if (length > 1 && XmlCharType.IsHighSurrogate(name[0]) && XmlCharType.IsLowSurrogate(name[1]))
          {
            int x = name[0];
            int y = name[1];
            Int32 u = XmlCharType.CombineSurrogateChar(y, x);
            bufBld.Append(u.ToString("X8", CultureInfo.InvariantCulture));
            position++;
            copyPosition = 2;
          }
          else
          {
            bufBld.Append(((Int32)name[0]).ToString("X4", CultureInfo.InvariantCulture));
            copyPosition = 1;
          }

          bufBld.Append("_");
          position++;

          if (matchPos == 0)
            if (en.MoveNext())
            {
              Match m = (Match)en.Current;
              matchPos = m.Index - 1;
            }
        }

      }
      for (; position < length; position++)
      {
        if ((local && !xmlCharType.IsNCNameCharXml4e(name[position])) ||
            (!local && !xmlCharType.IsNameCharXml4e(name[position])) ||
            (matchPos == position))
        {
          if (bufBld == null)
          {
            bufBld = new StringBuilder(length + 20);
          }
          if (matchPos == position)
            if (en.MoveNext())
            {
              Match m = (Match)en.Current;
              matchPos = m.Index - 1;
            }

          bufBld.Append(name, copyPosition, position - copyPosition);
          bufBld.Append("_x");
          if ((length > position + 1) && XmlCharType.IsHighSurrogate(name[position]) && XmlCharType.IsLowSurrogate(name[position + 1]))
          {
            int x = name[position];
            int y = name[position + 1];
            Int32 u = XmlCharType.CombineSurrogateChar(y, x);
            bufBld.Append(u.ToString("X8", CultureInfo.InvariantCulture));
            copyPosition = position + 2;
            position++;
          }
          else
          {
            bufBld.Append(((Int32)name[position]).ToString("X4", CultureInfo.InvariantCulture));
            copyPosition = position + 1;
          }
          bufBld.Append("_");
        }
      }
      if (copyPosition == 0)
      {
        return name;
      }
      else
      {
        if (copyPosition < length)
        {
          bufBld.Append(name, copyPosition, length - copyPosition);
        }
        return bufBld.ToString();
      }
    }

    private static readonly int c_EncodedCharLength = 7; // ("_xFFFF_".Length);
    private static volatile Regex c_EncodeCharPattern;
    private static volatile Regex c_DecodeCharPattern;
    private static int FromHex(char digit)
    {
      return (digit <= '9')
      ? ((int)digit - (int)'0')
      : (((digit <= 'F')
          ? ((int)digit - (int)'A')
          : ((int)digit - (int)'a'))
         + 10);
    }

    internal static byte[] FromBinHexString(string s, bool allowOddCount)
    {
      if (s == null)
      {
        throw new ArgumentNullException("s");
      }
      return BinHexDecoder.Decode(s.ToCharArray(), allowOddCount);
    }

    internal static string ToBinHexString(byte[] inArray)
    {
      if (inArray == null)
      {
        throw new ArgumentNullException("inArray");
      }
      return BinHexEncoder.Encode(inArray, 0, inArray.Length);
    }

    //
    // Verification methods for strings
    // 
    /// <include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.VerifyName"]/*' />
    /// <devdoc>
    ///    <para>
    ///    </para>
    /// </devdoc>
    public static string VerifyName(string name)
    {
      if (name == null)
      {
        throw new ArgumentNullException("name");
      }
      if (name.Length == 0)
      {
        throw new ArgumentNullException("name", Res.GetString(Res.Xml_EmptyName));
      }

      // parse name
      int endPos = ValidateNames.ParseNameNoNamespaces(name, 0);

      if (endPos != name.Length)
      {
        // did not parse to the end -> there is invalid character at endPos
        throw CreateInvalidNameCharException(name, endPos, ExceptionType.XmlException);
      }
      return name;
    }


    internal static Exception TryVerifyName(string name)
    {
      if (name == null || name.Length == 0)
      {
        return new XmlException(Res.Xml_EmptyName, string.Empty);
      }

      int endPos = ValidateNames.ParseNameNoNamespaces(name, 0);
      if (endPos != name.Length)
      {
        return new XmlException(endPos == 0 ? Res.Xml_BadStartNameChar : Res.Xml_BadNameChar, XmlException.BuildCharExceptionArgs(name, endPos));
      }
      return null;
    }

    internal static unsafe string VerifyQName(string name, ExceptionType exceptionType)
    {
      if (name == null || name.Length == 0)
      {
        throw new ArgumentNullException("name");
      }

      int colonPosition = -1;

      int endPos = ValidateNames.ParseQName(name, 0, out colonPosition);
      if (endPos != name.Length)
      {
        throw CreateException(Res.Xml_BadNameChar, XmlException.BuildCharExceptionArgs(name, endPos), exceptionType, 0, endPos + 1);
      }
      return name;
    }

    /// <include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.VerifyNCName"]/*' />
    /// <devdoc>
    ///    <para>
    ///    </para>
    /// </devdoc>
    public static string VerifyNCName(string name)
    {
      return VerifyNCName(name, ExceptionType.XmlException);
    }

    internal static string VerifyNCName(string name, ExceptionType exceptionType)
    {
      if (name == null)
      {
        throw new ArgumentNullException("name");
      }
      if (name.Length == 0)
      {
        throw new ArgumentNullException("name", Res.GetString(Res.Xml_EmptyLocalName));
      }

      int end = ValidateNames.ParseNCName(name, 0);

      if (end != name.Length)
      {
        // If the string is not a valid NCName, then throw or return false
        throw CreateInvalidNameCharException(name, end, exceptionType);
      }

      return name;
    }

    internal static Exception TryVerifyNCName(string name)
    {
      int len = ValidateNames.ParseNCName(name);

      if (len == 0 || len != name.Length)
      {
        return ValidateNames.GetInvalidNameException(name, 0, len);
      }
      return null;
    }

    /// <include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.VerifyTOKEN"]/*' />
    /// <devdoc>
    ///    <para>
    ///    </para>
    /// </devdoc>
    public static string VerifyTOKEN(string token)
    {
      if (token == null || token.Length == 0)
      {
        return token;
      }
      if (token[0] == ' ' || token[token.Length - 1] == ' ' || token.IndexOfAny(crt) != -1 || token.IndexOf("  ", StringComparison.Ordinal) != -1)
      {
        throw new XmlException(Res.Sch_NotTokenString, token);
      }
      return token;
    }

    internal static Exception TryVerifyTOKEN(string token)
    {
      if (token == null || token.Length == 0)
      {
        return null;
      }
      if (token[0] == ' ' || token[token.Length - 1] == ' ' || token.IndexOfAny(crt) != -1 || token.IndexOf("  ", StringComparison.Ordinal) != -1)
      {
        return new XmlException(Res.Sch_NotTokenString, token);
      }
      return null;
    }

    internal static Exception TryVerifyNMTOKEN(string name)
    {
      if (name == null || name.Length == 0)
      {
        return new XmlException(Res.Xml_EmptyName, string.Empty);
      }
      int endPos = ValidateNames.ParseNmtokenNoNamespaces(name, 0);
      if (endPos != name.Length)
      {
        return new XmlException(Res.Xml_BadNameChar, XmlException.BuildCharExceptionArgs(name, endPos));
      }
      return null;
    }


    internal static Exception TryVerifyNormalizedString(string str)
    {
      if (str.IndexOfAny(crt) != -1)
      {
        return new XmlSchemaException(Res.Sch_NotNormalizedString, str);
      }
      return null;
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static string ToString(Boolean value)
    {
      return value ? "true" : "false";
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString2"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static string ToString(Decimal value)
    {
      return value.ToString(null, NumberFormatInfo.InvariantInfo);
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString3"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [CLSCompliant(false)]
    public static string ToString(SByte value)
    {
      return value.ToString(null, NumberFormatInfo.InvariantInfo);
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString4"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static string ToString(Int16 value)
    {
      return value.ToString(null, NumberFormatInfo.InvariantInfo);
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString5"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static string ToString(Int32 value)
    {
      return value.ToString(null, NumberFormatInfo.InvariantInfo);
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString15"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static string ToString(Int64 value)
    {
      return value.ToString(null, NumberFormatInfo.InvariantInfo);
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString6"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static string ToString(Byte value)
    {
      return value.ToString(null, NumberFormatInfo.InvariantInfo);
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString7"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [CLSCompliant(false)]
    public static string ToString(UInt16 value)
    {
      return value.ToString(null, NumberFormatInfo.InvariantInfo);
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString8"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [CLSCompliant(false)]
    public static string ToString(UInt32 value)
    {
      return value.ToString(null, NumberFormatInfo.InvariantInfo);
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString16"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [CLSCompliant(false)]
    public static string ToString(UInt64 value)
    {
      return value.ToString(null, NumberFormatInfo.InvariantInfo);
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString9"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static string ToString(Single value)
    {
      if (Single.IsNegativeInfinity(value)) return "-INF";
      if (Single.IsPositiveInfinity(value)) return "INF";
      if (IsNegativeZero((double)value))
      {
        return ("-0");
      }
      return value.ToString("R", NumberFormatInfo.InvariantInfo);
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString10"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static string ToString(Double value)
    {
      if (Double.IsNegativeInfinity(value)) return "-INF";
      if (Double.IsPositiveInfinity(value)) return "INF";
      if (IsNegativeZero(value))
      {
        return ("-0");
      }
      return value.ToString("R", NumberFormatInfo.InvariantInfo);
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString13"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static string ToString(DateTime value, string format)
    {
      return value.ToString(format, DateTimeFormatInfo.InvariantInfo);
    }
    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToString14"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static string ToString(DateTime value, XmlDateTimeSerializationMode dateTimeOption)
    {
      switch (dateTimeOption)
      {
        case XmlDateTimeSerializationMode.Local:
        value = SwitchToLocalTime(value);
        break;

        case XmlDateTimeSerializationMode.Utc:
        value = SwitchToUtcTime(value);
        break;

        case XmlDateTimeSerializationMode.Unspecified:
        value = new DateTime(value.Ticks, DateTimeKind.Unspecified);
        break;

        case XmlDateTimeSerializationMode.RoundtripKind:
        break;

        default:
        throw new ArgumentException(Res.GetString(Res.Sch_InvalidDateTimeOption, dateTimeOption, "dateTimeOption"));
      }
      XsdDateTime xsdDateTime = new XsdDateTime(value, XsdDateTimeFlags.DateTime);
      return xsdDateTime.ToString();
    }

    public static string ToString(DateTimeOffset value)
    {
      XsdDateTime xsdDateTime = new XsdDateTime(value);
      return xsdDateTime.ToString();
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToBoolean"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static Boolean ToBoolean(string s)
    {
      s = TrimString(s);
      if (s == "1" || s == "true") return true;
      if (s == "0" || s == "false") return false;
      throw new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Boolean"));
    }

    internal static Exception TryToBoolean(string s, out Boolean result)
    {
      s = TrimString(s);
      if (s == "0" || s == "false")
      {
        result = false;
        return null;
      }
      else if (s == "1" || s == "true")
      {
        result = true;
        return null;
      }
      result = false;
      return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Boolean"));
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToChar"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static Char ToChar(string s)
    {
      if (s == null)
      {
        throw new ArgumentNullException("s");
      }
      if (s.Length != 1)
      {
        throw new FormatException(Res.GetString(Res.XmlConvert_NotOneCharString));
      }
      return s[0];
    }

    internal static Exception TryToChar(string s, out Char result)
    {
      if (!Char.TryParse(s, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Char"));
      }
      return null;
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToDecimal"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static Decimal ToDecimal(string s)
    {
      return Decimal.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToDecimal(string s, out Decimal result)
    {
      if (!Decimal.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Decimal"));
      }
      return null;
    }

    internal static Decimal ToInteger(string s)
    {
      return Decimal.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToInteger(string s, out Decimal result)
    {
      if (!Decimal.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Integer"));
      }
      return null;
    }

    internal static Exception TryToSByte(string s, out SByte result)
    {
      if (!SByte.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "SByte"));
      }
      return null;
    }

    internal static Exception TryToInt16(string s, out Int16 result)
    {
      if (!Int16.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Int16"));
      }
      return null;
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToInt32"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static Int32 ToInt32(string s)
    {
      return Int32.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToInt32(string s, out Int32 result)
    {
      if (!Int32.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Int32"));
      }
      return null;
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToInt64"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static Int64 ToInt64(string s)
    {
      return Int64.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToInt64(string s, out Int64 result)
    {
      if (!Int64.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Int64"));
      }
      return null;
    }

    internal static Exception TryToByte(string s, out Byte result)
    {
      if (!Byte.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Byte"));
      }
      return null;
    }

    internal static Exception TryToUInt16(string s, out UInt16 result)
    {
      if (!UInt16.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "UInt16"));
      }
      return null;
    }

    internal static Exception TryToUInt32(string s, out UInt32 result)
    {
      if (!UInt32.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "UInt32"));
      }
      return null;
    }

    internal static Exception TryToUInt64(string s, out UInt64 result)
    {
      if (!UInt64.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "UInt64"));
      }
      return null;
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToSingle"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static Single ToSingle(string s)
    {
      s = TrimString(s);
      if (s == "-INF") return Single.NegativeInfinity;
      if (s == "INF") return Single.PositiveInfinity;
      float f = Single.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo);
      if (f == 0 && s[0] == '-')
      {
        return -0f;
      }
      return f;
    }

    internal static Exception TryToSingle(string s, out Single result)
    {
      s = TrimString(s);
      if (s == "-INF")
      {
        result = Single.NegativeInfinity;
        return null;
      }
      else if (s == "INF")
      {
        result = Single.PositiveInfinity;
        return null;
      }
      else if (!Single.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Single"));
      }
      if (result == 0 && s[0] == '-')
      {
        result = -0f;
      }
      return null;
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToDouble"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static Double ToDouble(string s)
    {
      s = TrimString(s);
      if (s == "-INF") return Double.NegativeInfinity;
      if (s == "INF") return Double.PositiveInfinity;
      double dVal = Double.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo);
      if (dVal == 0 && s[0] == '-')
      {
        return -0d;
      }
      return dVal;
    }

    internal static Exception TryToDouble(string s, out double result)
    {
      s = TrimString(s);
      if (s == "-INF")
      {
        result = Double.NegativeInfinity;
        return null;
      }
      else if (s == "INF")
      {
        result = Double.PositiveInfinity;
        return null;
      }
      else if (!Double.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Double"));
      }
      if (result == 0 && s[0] == '-')
      {
        result = -0d;
      }
      return null;
    }

    internal static Double ToXPathDouble(Object o)
    {
      string str = o as string;
      if (str != null)
      {
        str = TrimString(str);
        if (str.Length != 0 && str[0] != '+')
        {
          double d;
          if (Double.TryParse(str, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out d))
          {
            return d;
          }
        }
        return Double.NaN;
      }
      if (o is double)
      {
        return (double)o;
      }
      if (o is bool)
      {
        return ((bool)o) ? 1.0 : 0.0;
      }
      try
      {
        return Convert.ToDouble(o, NumberFormatInfo.InvariantInfo);
      }
      catch (FormatException)
      {
      }
      catch (OverflowException)
      {
      }
      catch (ArgumentNullException) { }
      return Double.NaN;
    }

    internal static Exception TryToTimeSpan(string s, out TimeSpan result)
    {
      XsdDuration duration;
      Exception exception;

      exception = XsdDuration.TryParse(s, out duration);
      if (exception != null)
      {
        result = TimeSpan.MinValue;
        return exception;
      }
      else
      {
        return duration.TryToTimeSpan(out result);
      }
    }

    static volatile string[] s_allDateTimeFormats;

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToDateTime2"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static DateTime ToDateTime(string s, string[] formats)
    {
      return DateTime.ParseExact(s, formats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite);
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToDateTime3"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static DateTime ToDateTime(string s, XmlDateTimeSerializationMode dateTimeOption)
    {
      XsdDateTime xsdDateTime = new XsdDateTime(s, XsdDateTimeFlags.AllXsd);
      DateTime dt = (DateTime)xsdDateTime;

      switch (dateTimeOption)
      {
        case XmlDateTimeSerializationMode.Local:
        dt = SwitchToLocalTime(dt);
        break;

        case XmlDateTimeSerializationMode.Utc:
        dt = SwitchToUtcTime(dt);
        break;

        case XmlDateTimeSerializationMode.Unspecified:
        dt = new DateTime(dt.Ticks, DateTimeKind.Unspecified);
        break;

        case XmlDateTimeSerializationMode.RoundtripKind:
        break;

        default:
        throw new ArgumentException(Res.GetString(Res.Sch_InvalidDateTimeOption, dateTimeOption, "dateTimeOption"));
      }
      return dt;
    }

    ///<include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert.ToGuid"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public static Guid ToGuid(string s)
    {
      return new Guid(s);
    }

    internal static Exception TryToGuid(string s, out Guid result)
    {
      Exception exception = null;

      result = Guid.Empty;

      try
      {
        result = new Guid(s);
      }
      catch (ArgumentException)
      {
        exception = new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Guid"));
      }
      catch (FormatException)
      {
        exception = new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Guid"));
      }
      return exception;
    }

    private static DateTime SwitchToLocalTime(DateTime value)
    {
      switch (value.Kind)
      {
        case DateTimeKind.Local:
        return value;

        case DateTimeKind.Unspecified:
        return new DateTime(value.Ticks, DateTimeKind.Local);

        case DateTimeKind.Utc:
        return value.ToLocalTime();
      }
      return value;
    }

    private static DateTime SwitchToUtcTime(DateTime value)
    {
      switch (value.Kind)
      {
        case DateTimeKind.Utc:
        return value;

        case DateTimeKind.Unspecified:
        return new DateTime(value.Ticks, DateTimeKind.Utc);

        case DateTimeKind.Local:
        return value.ToUniversalTime();
      }
      return value;
    }

    internal static Uri ToUri(string s)
    {
      if (s != null && s.Length > 0)
      { //string.Empty is a valid uri but not "   "
        s = TrimString(s);
        if (s.Length == 0 || s.IndexOf("##", StringComparison.Ordinal) != -1)
        {
          throw new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Uri"));
        }
      }
      Uri uri;
      if (!Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out uri))
      {
        throw new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Uri"));
      }
      return uri;
    }

    internal static Exception TryToUri(string s, out Uri result)
    {
      result = null;

      if (s != null && s.Length > 0)
      { //string.Empty is a valid uri but not "   "
        s = TrimString(s);
        if (s.Length == 0 || s.IndexOf("##", StringComparison.Ordinal) != -1)
        {
          return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Uri"));
        }
      }
      if (!Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out result))
      {
        return new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Uri"));
      }
      return null;
    }

    // Compares the given character interval and string and returns true if the characters are identical
    internal static bool StrEqual(char[] chars, int strPos1, int strLen1, string str2)
    {
      if (strLen1 != str2.Length)
      {
        return false;
      }

      int i = 0;
      while (i < strLen1 && chars[strPos1 + i] == str2[i])
      {
        i++;
      }
      return i == strLen1;
    }

    // XML whitespace characters, <spec>http://www.w3.org/TR/REC-xml#NT-S</spec>
    internal static readonly char[] WhitespaceChars = new char[] { ' ', '\t', '\n', '\r' };

    // Trim a string using XML whitespace characters
    internal static string TrimString(string value)
    {
      return value.Trim(WhitespaceChars);
    }

    // Trim beginning of a string using XML whitespace characters
    internal static string TrimStringStart(string value)
    {
      return value.TrimStart(WhitespaceChars);
    }

    // Trim end of a string using XML whitespace characters
    internal static string TrimStringEnd(string value)
    {
      return value.TrimEnd(WhitespaceChars);
    }

    // Split a string into a whitespace-separated list of tokens
    internal static string[] SplitString(string value)
    {
      return value.Split(WhitespaceChars, StringSplitOptions.RemoveEmptyEntries);
    }

    internal static string[] SplitString(string value, StringSplitOptions splitStringOptions)
    {
      return value.Split(WhitespaceChars, splitStringOptions);
    }

    internal static bool IsNegativeZero(double value)
    {
      // Simple equals function will report that -0 is equal to +0, so compare bits instead
      if (value == 0 && DoubleToInt64Bits(value) == DoubleToInt64Bits(-0e0))
      {
        return true;
      }
      return false;
    }

    private static unsafe long DoubleToInt64Bits(double value)
    {
      // NOTE: BitConverter.DoubleToInt64Bits is missing in Silverlight
      return *((long*)&value);
    }

    internal static void VerifyCharData(string data, ExceptionType exceptionType)
    {
      VerifyCharData(data, exceptionType, exceptionType);
    }

    internal static unsafe void VerifyCharData(string data, ExceptionType invCharExceptionType, ExceptionType invSurrogateExceptionType)
    {
      if (data == null || data.Length == 0)
      {
        return;
      }

      int i = 0;
      int len = data.Length;
      for (; ; )
      {
        while (i < len && (xmlCharType.charProperties[data[i]] & XmlCharType.fCharData) != 0)
        {
          i++;
        }
        if (i == len)
        {
          return;
        }

        char ch = data[i];
        if (XmlCharType.IsHighSurrogate(ch))
        {
          if (i + 1 == len)
          {
            throw CreateException(Res.Xml_InvalidSurrogateMissingLowChar, invSurrogateExceptionType, 0, i + 1);
          }
          ch = data[i + 1];
          if (XmlCharType.IsLowSurrogate(ch))
          {
            i += 2;
            continue;
          }
          else
          {
            throw CreateInvalidSurrogatePairException(data[i + 1], data[i], invSurrogateExceptionType, 0, i + 1);
          }
        }
        throw CreateInvalidCharException(data, i, invCharExceptionType);
      }
    }

    internal static unsafe void VerifyCharData(char[] data, int offset, int len, ExceptionType exceptionType)
    {
      if (data == null || len == 0)
      {
        return;
      }

      int i = offset;
      int endPos = offset + len;
      for (; ; )
      {
        while (i < endPos && (xmlCharType.charProperties[data[i]] & XmlCharType.fCharData) != 0)
        {
          i++;
        }
        if (i == endPos)
        {
          return;
        }

        char ch = data[i];
        if (XmlCharType.IsHighSurrogate(ch))
        {
          if (i + 1 == endPos)
          {
            throw CreateException(Res.Xml_InvalidSurrogateMissingLowChar, exceptionType, 0, offset - i + 1);
          }
          ch = data[i + 1];
          if (XmlCharType.IsLowSurrogate(ch))
          {
            i += 2;
            continue;
          }
          else
          {
            throw CreateInvalidSurrogatePairException(data[i + 1], data[i], exceptionType, 0, offset - i + 1);
          }
        }
        throw CreateInvalidCharException(data, len, i, exceptionType);
      }
    }

    internal static string EscapeValueForDebuggerDisplay(string value)
    {
      StringBuilder sb = null;
      int i = 0;
      int start = 0;
      while (i < value.Length)
      {
        char ch = value[i];
        if ((int)ch < 0x20 || ch == '"')
        {
          if (sb == null)
          {
            sb = new StringBuilder(value.Length + 4);
          }
          if (i - start > 0)
          {
            sb.Append(value, start, i - start);
          }
          start = i + 1;
          switch (ch)
          {
            case '"':
            sb.Append("\\\"");
            break;
            case '\r':
            sb.Append("\\r");
            break;
            case '\n':
            sb.Append("\\n");
            break;
            case '\t':
            sb.Append("\\t");
            break;
            default:
            sb.Append(ch);
            break;
          }
        }
        i++;
      }
      if (sb == null)
      {
        return value;
      }
      if (i - start > 0)
      {
        sb.Append(value, start, i - start);
      }
      return sb.ToString();
    }

    internal static Exception CreateException(string res, ExceptionType exceptionType)
    {
      return CreateException(res, exceptionType, 0, 0);
    }

    internal static Exception CreateException(string res, ExceptionType exceptionType, int lineNo, int linePos)
    {
      switch (exceptionType)
      {
        case ExceptionType.ArgumentException:
        return new ArgumentException(Res.GetString(res));
        case ExceptionType.XmlException:
        default:
        return new XmlException(res, string.Empty, lineNo, linePos);
      }
    }

    internal static Exception CreateException(string res, string arg, ExceptionType exceptionType)
    {
      return CreateException(res, arg, exceptionType, 0, 0);
    }

    internal static Exception CreateException(string res, string arg, ExceptionType exceptionType, int lineNo, int linePos)
    {
      switch (exceptionType)
      {
        case ExceptionType.ArgumentException:
        return new ArgumentException(Res.GetString(res, arg));
        case ExceptionType.XmlException:
        default:
        return new XmlException(res, arg, lineNo, linePos);
      }
    }

    internal static Exception CreateException(string res, string[] args, ExceptionType exceptionType)
    {
      return CreateException(res, args, exceptionType, 0, 0);
    }

    internal static Exception CreateException(string res, string[] args, ExceptionType exceptionType, int lineNo, int linePos)
    {
      switch (exceptionType)
      {
        case ExceptionType.ArgumentException:
        return new ArgumentException(Res.GetString(res, args));
        case ExceptionType.XmlException:
        default:
        return new XmlException(res, args, lineNo, linePos);
      }
    }

    internal static Exception CreateInvalidSurrogatePairException(char low, char hi)
    {
      return CreateInvalidSurrogatePairException(low, hi, ExceptionType.ArgumentException);
    }

    internal static Exception CreateInvalidSurrogatePairException(char low, char hi, ExceptionType exceptionType)
    {
      return CreateInvalidSurrogatePairException(low, hi, exceptionType, 0, 0);
    }

    internal static Exception CreateInvalidSurrogatePairException(char low, char hi, ExceptionType exceptionType, int lineNo, int linePos)
    {
      string[] args = new string[] {
                ((uint)hi).ToString( "X", CultureInfo.InvariantCulture ),
                ((uint)low).ToString( "X", CultureInfo.InvariantCulture )
            };
      return CreateException(Res.Xml_InvalidSurrogatePairWithArgs, args, exceptionType, lineNo, linePos);
    }

    internal static Exception CreateInvalidHighSurrogateCharException(char hi)
    {
      return CreateInvalidHighSurrogateCharException(hi, ExceptionType.ArgumentException);
    }

    internal static Exception CreateInvalidHighSurrogateCharException(char hi, ExceptionType exceptionType)
    {
      return CreateInvalidHighSurrogateCharException(hi, exceptionType, 0, 0);
    }

    internal static Exception CreateInvalidHighSurrogateCharException(char hi, ExceptionType exceptionType, int lineNo, int linePos)
    {
      return CreateException(Res.Xml_InvalidSurrogateHighChar, ((uint)hi).ToString("X", CultureInfo.InvariantCulture), exceptionType, lineNo, linePos);
    }

    internal static Exception CreateInvalidCharException(char[] data, int length, int invCharPos)
    {
      return CreateInvalidCharException(data, length, invCharPos, ExceptionType.ArgumentException);
    }

    internal static Exception CreateInvalidCharException(char[] data, int length, int invCharPos, ExceptionType exceptionType)
    {
      return CreateException(Res.Xml_InvalidCharacter, XmlException.BuildCharExceptionArgs(data, length, invCharPos), exceptionType, 0, invCharPos + 1);
    }

    internal static Exception CreateInvalidCharException(string data, int invCharPos)
    {
      return CreateInvalidCharException(data, invCharPos, ExceptionType.ArgumentException);
    }

    internal static Exception CreateInvalidCharException(string data, int invCharPos, ExceptionType exceptionType)
    {
      return CreateException(Res.Xml_InvalidCharacter, XmlException.BuildCharExceptionArgs(data, invCharPos), exceptionType, 0, invCharPos + 1);
    }

    internal static Exception CreateInvalidCharException(char invChar, char nextChar)
    {
      return CreateInvalidCharException(invChar, nextChar, ExceptionType.ArgumentException);
    }

    internal static Exception CreateInvalidCharException(char invChar, char nextChar, ExceptionType exceptionType)
    {
      return CreateException(Res.Xml_InvalidCharacter, XmlException.BuildCharExceptionArgs(invChar, nextChar), exceptionType);
    }

    internal static Exception CreateInvalidNameCharException(string name, int index, ExceptionType exceptionType)
    {
      return CreateException(index == 0 ? Res.Xml_BadStartNameChar : Res.Xml_BadNameChar, XmlException.BuildCharExceptionArgs(name, index), exceptionType, 0, index + 1);
    }

    internal static ArgumentException CreateInvalidNameArgumentException(string name, string argumentName)
    {
      return (name == null) ? new ArgumentNullException(argumentName) : new ArgumentException(Res.GetString(Res.Xml_EmptyName), argumentName);
    }
  }
}
