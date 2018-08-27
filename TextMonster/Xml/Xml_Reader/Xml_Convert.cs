using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Codiert und decodiert XML-Namen und stellt Methoden für das Konvertieren zwischen Typen der Common Language Runtime und XSD-Typen (XML Schema Definition) bereit.Bei der Konvertierung von Datentypen sind die zurückgegebenen Werte vom Gebietsschema unabhängig.
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class Xml_Convert
  {
    private static Xml_CharType xmlCharType = Xml_CharType.Instance;
    internal static char[] crt = new char[3]
    {
      '\n',
      '\r',
      '\t'
    };
    private static readonly int c_EncodedCharLength = 7;
    internal static readonly char[] WhitespaceChars = new char[4]
    {
      ' ',
      '\t',
      '\n',
      '\r'
    };
    private static volatile Regex c_EncodeCharPattern;
    private static volatile Regex c_DecodeCharPattern;
    private static volatile string[] s_allDateTimeFormats;

    private static string[] AllDateTimeFormats
    {
      get
      {
        if (Xml_Convert.s_allDateTimeFormats == null)
          Xml_Convert.CreateAllDateTimeFormats();
        return Xml_Convert.s_allDateTimeFormats;
      }
    }

    /// <summary>
    /// Konvertiert den Namen in einen gültigen XML-Namen.
    /// </summary>
    /// 
    /// <returns>
    /// Gibt den Namen zurück, wobei ungültige Zeichen durch eine Escapezeichenfolge ersetzt wurden.
    /// </returns>
    /// <param name="name">Ein zu übersetzender Name. </param>
    public static string EncodeName(string name)
    {
      return Xml_Convert.EncodeName(name, true, false);
    }

    /// <summary>
    /// Überprüft, ob der Name entsprechend der XML-Spezifikation gültig ist.
    /// </summary>
    /// 
    /// <returns>
    /// Der codierte Name.
    /// </returns>
    /// <param name="name">Der zu codierende Name. </param>
    public static string EncodeNmToken(string name)
    {
      return Xml_Convert.EncodeName(name, false, false);
    }

    /// <summary>
    /// Konvertiert den Namen in einen gültigen lokalen XML-Namen.
    /// </summary>
    /// 
    /// <returns>
    /// Der codierte Name.
    /// </returns>
    /// <param name="name">Der zu codierende Name. </param>
    public static string EncodeLocalName(string name)
    {
      return Xml_Convert.EncodeName(name, true, true);
    }

    /// <summary>
    /// Decodiert einen Namen.Diese Methode ist die Umkehrung der <see cref="M:System.Xml.XmlConvert.EncodeName(System.String)"/>-Methode und der <see cref="M:System.Xml.XmlConvert.EncodeLocalName(System.String)"/>-Methode.
    /// </summary>
    /// 
    /// <returns>
    /// Der decodierte Name.
    /// </returns>
    /// <param name="name">Der umzuwandelnde Name. </param>
    public static string DecodeName(string name)
    {
      if (name == null || name.Length == 0)
        return name;
      StringBuilder stringBuilder = (StringBuilder)null;
      int length = name.Length;
      int startIndex = 0;
      int startat = name.IndexOf('_');
      if (startat < 0)
        return name;
      if (Xml_Convert.c_DecodeCharPattern == null)
        Xml_Convert.c_DecodeCharPattern = new Regex("_[Xx]([0-9a-fA-F]{4}|[0-9a-fA-F]{8})_");
      IEnumerator enumerator = Xml_Convert.c_DecodeCharPattern.Matches(name, startat).GetEnumerator();
      int num = -1;
      if (enumerator != null && enumerator.MoveNext())
        num = ((Capture)enumerator.Current).Index;
      for (int index = 0; index < length - Xml_Convert.c_EncodedCharLength + 1; ++index)
      {
        if (index == num)
        {
          if (enumerator.MoveNext())
            num = ((Capture)enumerator.Current).Index;
          if (stringBuilder == null)
            stringBuilder = new StringBuilder(length + 20);
          stringBuilder.Append(name, startIndex, index - startIndex);
          if ((int)name[index + 6] != 95)
          {
            int combinedChar = Xml_Convert.FromHex(name[index + 2]) * 268435456 + Xml_Convert.FromHex(name[index + 3]) * 16777216 + Xml_Convert.FromHex(name[index + 4]) * 1048576 + Xml_Convert.FromHex(name[index + 5]) * 65536 + Xml_Convert.FromHex(name[index + 6]) * 4096 + Xml_Convert.FromHex(name[index + 7]) * 256 + Xml_Convert.FromHex(name[index + 8]) * 16 + Xml_Convert.FromHex(name[index + 9]);
            if (combinedChar >= 65536)
            {
              if (combinedChar <= 1114111)
              {
                startIndex = index + Xml_Convert.c_EncodedCharLength + 4;
                char lowChar;
                char highChar;
                Xml_CharType.SplitSurrogateChar(combinedChar, out lowChar, out highChar);
                stringBuilder.Append(highChar);
                stringBuilder.Append(lowChar);
              }
            }
            else
            {
              startIndex = index + Xml_Convert.c_EncodedCharLength + 4;
              stringBuilder.Append((char)combinedChar);
            }
            index += Xml_Convert.c_EncodedCharLength - 1 + 4;
          }
          else
          {
            startIndex = index + Xml_Convert.c_EncodedCharLength;
            stringBuilder.Append((char)(Xml_Convert.FromHex(name[index + 2]) * 4096 + Xml_Convert.FromHex(name[index + 3]) * 256 + Xml_Convert.FromHex(name[index + 4]) * 16 + Xml_Convert.FromHex(name[index + 5])));
            index += Xml_Convert.c_EncodedCharLength - 1;
          }
        }
      }
      if (startIndex == 0)
        return name;
      if (startIndex < length)
        stringBuilder.Append(name, startIndex, length - startIndex);
      return stringBuilder.ToString();
    }

    private static string EncodeName(string name, bool first, bool local)
    {
      if (string.IsNullOrEmpty(name))
        return name;
      StringBuilder stringBuilder = (StringBuilder)null;
      int length = name.Length;
      int startIndex = 0;
      int index = 0;
      int startat = name.IndexOf('_');
      IEnumerator enumerator = (IEnumerator)null;
      if (startat >= 0)
      {
        if (Xml_Convert.c_EncodeCharPattern == null)
          Xml_Convert.c_EncodeCharPattern = new Regex("(?<=_)[Xx]([0-9a-fA-F]{4}|[0-9a-fA-F]{8})_");
        enumerator = Xml_Convert.c_EncodeCharPattern.Matches(name, startat).GetEnumerator();
      }
      int num1 = -1;
      if (enumerator != null && enumerator.MoveNext())
        num1 = ((Capture)enumerator.Current).Index - 1;
      if (first && (!Xml_Convert.xmlCharType.IsStartNCNameCharXml4e(name[0]) && (local || !local && (int)name[0] != 58) || num1 == 0))
      {
        if (stringBuilder == null)
          stringBuilder = new StringBuilder(length + 20);
        stringBuilder.Append("_x");
        if (length > 1 && Xml_CharType.IsHighSurrogate((int)name[0]) && Xml_CharType.IsLowSurrogate((int)name[1]))
        {
          int highChar = (int)name[0];
          int num2 = Xml_CharType.CombineSurrogateChar((int)name[1], highChar);
          stringBuilder.Append(num2.ToString("X8", (IFormatProvider)CultureInfo.InvariantCulture));
          ++index;
          startIndex = 2;
        }
        else
        {
          stringBuilder.Append(((int)name[0]).ToString("X4", (IFormatProvider)CultureInfo.InvariantCulture));
          startIndex = 1;
        }
        stringBuilder.Append("_");
        ++index;
        if (num1 == 0 && enumerator.MoveNext())
          num1 = ((Capture)enumerator.Current).Index - 1;
      }
      for (; index < length; ++index)
      {
        if (local && !Xml_Convert.xmlCharType.IsNCNameCharXml4e(name[index]) || !local && !Xml_Convert.xmlCharType.IsNameCharXml4e(name[index]) || num1 == index)
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder(length + 20);
          if (num1 == index && enumerator.MoveNext())
            num1 = ((Capture)enumerator.Current).Index - 1;
          stringBuilder.Append(name, startIndex, index - startIndex);
          stringBuilder.Append("_x");
          if (length > index + 1 && Xml_CharType.IsHighSurrogate((int)name[index]) && Xml_CharType.IsLowSurrogate((int)name[index + 1]))
          {
            int highChar = (int)name[index];
            int num2 = Xml_CharType.CombineSurrogateChar((int)name[index + 1], highChar);
            stringBuilder.Append(num2.ToString("X8", (IFormatProvider)CultureInfo.InvariantCulture));
            startIndex = index + 2;
            ++index;
          }
          else
          {
            stringBuilder.Append(((int)name[index]).ToString("X4", (IFormatProvider)CultureInfo.InvariantCulture));
            startIndex = index + 1;
          }
          stringBuilder.Append("_");
        }
      }
      if (startIndex == 0)
        return name;
      if (startIndex < length)
        stringBuilder.Append(name, startIndex, length - startIndex);
      return stringBuilder.ToString();
    }

    private static int FromHex(char digit)
    {
      if ((int)digit > 57)
        return ((int)digit <= 70 ? (int)digit - 65 : (int)digit - 97) + 10;
      return (int)digit - 48;
    }

    internal static byte[] FromBinHexString(string s)
    {
      return Xml_Convert.FromBinHexString(s, true);
    }

    internal static byte[] FromBinHexString(string s, bool allowOddCount)
    {
      if (s == null)
        throw new ArgumentNullException("s");
      return Xml_BinHexDecoder.Decode(s.ToCharArray(), allowOddCount);
    }

    internal static string ToBinHexString(byte[] inArray)
    {
      if (inArray == null)
        throw new ArgumentNullException("inArray");
      return BinHexEncoder.Encode(inArray, 0, inArray.Length);
    }

    /// <summary>
    /// Überprüft, ob der Name ein gültiger Name gemäß der W3C-Empfehlung für XML (Extended Markup Language) ist.
    /// </summary>
    /// 
    /// <returns>
    /// Der Name, wenn dieser ein gültiger XML-Name ist.
    /// </returns>
    /// <param name="name">Der zu überprüfende Name. </param><exception cref="T:System.Xml.XmlException"><paramref name="name"/> ist kein gültiger XML-Name. </exception><exception cref="T:System.ArgumentNullException"><paramref name="name"/> ist null oder String.Empty. </exception>
    public static string VerifyName(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (name.Length == 0)
        throw new ArgumentNullException("name", Res.GetString("Xml_EmptyName"));
      int index = ValidateNames.ParseNameNoNamespaces(name, 0);
      if (index != name.Length)
        throw Xml_Convert.CreateInvalidNameCharException(name, index, ExceptionType.XmlException);
      return name;
    }

    internal static Exception TryVerifyName(string name)
    {
      if (name == null || name.Length == 0)
        return (Exception)new XmlException("Xml_EmptyName", string.Empty);
      int invCharIndex = ValidateNames.ParseNameNoNamespaces(name, 0);
      if (invCharIndex != name.Length)
        return (Exception)new XmlException(invCharIndex == 0 ? "Xml_BadStartNameChar" : "Xml_BadNameChar", XmlException.BuildCharExceptionArgs(name, invCharIndex));
      return (Exception)null;
    }

    internal static string VerifyQName(string name)
    {
      return Xml_Convert.VerifyQName(name, ExceptionType.XmlException);
    }

    internal static string VerifyQName(string name, ExceptionType exceptionType)
    {
      if (name == null || name.Length == 0)
        throw new ArgumentNullException("name");
      int colonOffset = -1;
      int invCharIndex = ValidateNames.ParseQName(name, 0, out colonOffset);
      if (invCharIndex != name.Length)
        throw Xml_Convert.CreateException("Xml_BadNameChar", XmlException.BuildCharExceptionArgs(name, invCharIndex), exceptionType, 0, invCharIndex + 1);
      return name;
    }

    /// <summary>
    /// Überprüft, ob der Name ein gültiger NCName gemäß der W3C-Empfehlung für XML (Extended Markup Language) ist.Ein NCName darf keinen Doppelpunkt enthalten.
    /// </summary>
    /// 
    /// <returns>
    /// Der Name, wenn dieser ein gültiger NCName ist.
    /// </returns>
    /// <param name="name">Der zu überprüfende Name. </param><exception cref="T:System.ArgumentNullException"><paramref name="name"/> ist null oder String.Empty. </exception><exception cref="T:System.Xml.XmlException"><paramref name="name"/> ist kein gültiger nicht-Doppelpunkt-Name. </exception>
    public static string VerifyNCName(string name)
    {
      return Xml_Convert.VerifyNCName(name, ExceptionType.XmlException);
    }

    internal static string VerifyNCName(string name, ExceptionType exceptionType)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (name.Length == 0)
        throw new ArgumentNullException("name", Res.GetString("Xml_EmptyLocalName"));
      int index = ValidateNames.ParseNCName(name, 0);
      if (index != name.Length)
        throw Xml_Convert.CreateInvalidNameCharException(name, index, exceptionType);
      return name;
    }

    internal static Exception TryVerifyNCName(string name)
    {
      int offsetBadChar = ValidateNames.ParseNCName(name);
      if (offsetBadChar == 0 || offsetBadChar != name.Length)
        return ValidateNames.GetInvalidNameException(name, 0, offsetBadChar);
      return (Exception)null;
    }

    /// <summary>
    /// Überprüft, ob die Zeichenfolge ein gültiges Token gemäß der Empfehlung in W3C XML Schema Teil 2: „Datentypen“ ist.
    /// </summary>
    /// 
    /// <returns>
    /// Das Token, wenn es sich um ein gültiges Token handelt.
    /// </returns>
    /// <param name="token">Der Zeichenfolgenwert, der überprüft werden soll.</param><exception cref="T:System.Xml.XmlException">Der Zeichenfolgenwert ist kein gültiges Token.</exception>
    public static string VerifyTOKEN(string token)
    {
      if (token == null || token.Length == 0)
        return token;
      if ((int)token[0] != 32)
      {
        string str = token;
        int index = str.Length - 1;
        if ((int)str[index] != 32 && token.IndexOfAny(Xml_Convert.crt) == -1 && token.IndexOf("  ", StringComparison.Ordinal) == -1)
          return token;
      }
      throw new XmlException("Sch_NotTokenString", token);
    }

    internal static Exception TryVerifyTOKEN(string token)
    {
      if (token == null || token.Length == 0)
        return (Exception)null;
      if ((int)token[0] != 32)
      {
        string str = token;
        int index = str.Length - 1;
        if ((int)str[index] != 32 && token.IndexOfAny(Xml_Convert.crt) == -1 && token.IndexOf("  ", StringComparison.Ordinal) == -1)
          return (Exception)null;
      }
      return (Exception)new XmlException("Sch_NotTokenString", token);
    }

    /// <summary>
    /// Überprüft, ob die Zeichenfolge ein gültiges NMTOKEN gemäß der Empfehlung in W3C XML Schema, Teil 2: „Datentypen“, ist.
    /// </summary>
    /// 
    /// <returns>
    /// Das Namenstoken, wenn es sich um ein gültiges NMTOKEN handelt.
    /// </returns>
    /// <param name="name">Die Zeichenfolge, die überprüft werden soll.</param><exception cref="T:System.Xml.XmlException">Die Zeichenfolge ist kein gültiger Namenstoken.</exception><exception cref="T:System.ArgumentNullException"><paramref name="name"/> ist null.</exception>
    public static string VerifyNMTOKEN(string name)
    {
      return Xml_Convert.VerifyNMTOKEN(name, ExceptionType.XmlException);
    }

    internal static string VerifyNMTOKEN(string name, ExceptionType exceptionType)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (name.Length == 0)
        throw Xml_Convert.CreateException("Xml_InvalidNmToken", name, exceptionType);
      int invCharIndex = ValidateNames.ParseNmtokenNoNamespaces(name, 0);
      if (invCharIndex != name.Length)
        throw Xml_Convert.CreateException("Xml_BadNameChar", XmlException.BuildCharExceptionArgs(name, invCharIndex), exceptionType, 0, invCharIndex + 1);
      return name;
    }

    internal static Exception TryVerifyNMTOKEN(string name)
    {
      if (name == null || name.Length == 0)
        return (Exception)new XmlException("Xml_EmptyName", string.Empty);
      int invCharIndex = ValidateNames.ParseNmtokenNoNamespaces(name, 0);
      if (invCharIndex != name.Length)
        return (Exception)new XmlException("Xml_BadNameChar", XmlException.BuildCharExceptionArgs(name, invCharIndex));
      return (Exception)null;
    }

    internal static string VerifyNormalizedString(string str)
    {
      if (str.IndexOfAny(Xml_Convert.crt) != -1)
        throw new XmlSchemaException("Sch_NotNormalizedString", str);
      return str;
    }

    internal static Exception TryVerifyNormalizedString(string str)
    {
      if (str.IndexOfAny(Xml_Convert.crt) != -1)
        return (Exception)new XmlSchemaException("Sch_NotNormalizedString", str);
      return (Exception)null;
    }

    /// <summary>
    /// Gibt die übergebene Zeichenfolge zurück, wenn alle Zeichen und Ersatzzeichenpaare im Zeichenfolgenargument gültige XML-Zeichen sind, andernfalls wird eine XmlException mit Informationen zum ersten ungültigen Zeichen ausgelöst.
    /// </summary>
    /// 
    /// <returns>
    /// Gibt die übergebene Zeichenfolge zurück, wenn alle Zeichen und Ersatzzeichenpaare im Zeichenfolgenargument gültige XML-Zeichen sind, andernfalls wird eine XmlException mit Informationen zum ersten ungültigen Zeichen ausgelöst.
    /// </returns>
    /// <param name="content">Der <see cref="T:System.String"/> mit den zu überprüfenden Zeichen.</param>
    public static string VerifyXmlChars(string content)
    {
      if (content == null)
        throw new ArgumentNullException("content");
      Xml_Convert.VerifyCharData(content, ExceptionType.XmlException);
      return content;
    }

    /// <summary>
    /// Gibt die übergebene Zeichenfolgeninstanz zurück, wenn alle Zeichen im Zeichenfolgenargument gültige Zeichen für eine öffentliche ID sind.
    /// </summary>
    /// 
    /// <returns>
    /// Gibt die übergebene Zeichenfolge zurück, wenn alle Zeichen im Argument gültige Zeichen für eine öffentliche ID sind.
    /// </returns>
    /// <param name="publicId"><see cref="T:System.String"/>, der die zu überprüfende ID enthält.</param>
    public static string VerifyPublicId(string publicId)
    {
      if (publicId == null)
        throw new ArgumentNullException("publicId");
      int invCharPos = Xml_Convert.xmlCharType.IsPublicId(publicId);
      if (invCharPos != -1)
        throw Xml_Convert.CreateInvalidCharException(publicId, invCharPos, ExceptionType.XmlException);
      return publicId;
    }

    /// <summary>
    /// Gibt die übergebene Zeichenfolgeninstanz zurück, wenn alle Zeichen im Zeichenfolgenargument gültige Leerraumzeichen sind.
    /// </summary>
    /// 
    /// <returns>
    /// Gibt die übergebene Zeichenfolgeninstanz zurück, wenn alle Zeichen im Zeichenfolgenargument gültige Leerraumzeichen sind, andernfalls null.
    /// </returns>
    /// <param name="content">Der zu überprüfende <see cref="T:System.String"/>.</param>
    public static string VerifyWhitespace(string content)
    {
      if (content == null)
        throw new ArgumentNullException("content");
      int invCharIndex = Xml_Convert.xmlCharType.IsOnlyWhitespaceWithPos(content);
      if (invCharIndex != -1)
        throw new XmlException("Xml_InvalidWhitespaceCharacter", XmlException.BuildCharExceptionArgs(content, invCharIndex), 0, invCharIndex + 1);
      return content;
    }

    /// <summary>
    /// Überprüft, ob das übergebene Zeichen ein gültiger Startnamenszeichen-Typ ist.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn das Zeichen ein gültiger Startnamenszeichen-Typ ist, andernfalls false.
    /// </returns>
    /// <param name="ch">Das Zeichen, das validiert werden soll.</param>
    public static unsafe bool IsStartNCNameChar(char ch)
    {
      return ((uint)Xml_Convert.xmlCharType.charProperties[(int)ch] & 4U) > 0U;
    }

    /// <summary>
    /// Überprüft, ob das übergebene Zeichen ein gültiger Nicht-Doppelpunkt-Zeichentyp ist.
    /// </summary>
    /// 
    /// <returns>
    /// Gibt true zurück, wenn ein Zeichen ein gültiger Nicht-Doppelpunkt-Zeichentyp ist, andernfalls false.
    /// </returns>
    /// <param name="ch">Das Zeichen, das als Nicht-Doppelpunkt-Zeichen überprüft werden soll.</param>
    public static unsafe bool IsNCNameChar(char ch)
    {
      return ((uint)Xml_Convert.xmlCharType.charProperties[(int)ch] & 8U) > 0U;
    }

    /// <summary>
    /// Überprüft, ob das übergebene Zeichen ein gültiges XML-Zeichen ist.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn das übergebene Zeichen ein gültiges XML-Zeichen ist, andernfalls false.
    /// </returns>
    /// <param name="ch">Das Zeichen, das validiert werden soll.</param>
    public static unsafe bool IsXmlChar(char ch)
    {
      return ((uint)Xml_Convert.xmlCharType.charProperties[(int)ch] & 16U) > 0U;
    }

    /// <summary>
    /// Überprüft, ob das übergebene Ersatzzeichenpaar ein gültiges XML-Zeichen ist.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn das übergebene Ersatzzeichenpaar ein gültiges XML-Zeichen ist, andernfalls false.
    /// </returns>
    /// <param name="lowChar">Das zu validierende Ersatzzeichen.</param><param name="highChar">Das zu validierende Ersatzzeichen.</param>
    public static bool IsXmlSurrogatePair(char lowChar, char highChar)
    {
      if (Xml_CharType.IsHighSurrogate((int)highChar))
        return Xml_CharType.IsLowSurrogate((int)lowChar);
      return false;
    }

    /// <summary>
    /// Gibt die übergebene Zeicheninstanz zurück, wenn das Zeichen im Argument ein gültiges Zeichen für eine öffentliche ID ist, andernfalls null.
    /// </summary>
    /// 
    /// <returns>
    /// Gibt das übergebene Zeichen zurück, wenn das Zeichen ein gültiges Zeichen für eine öffentliche ID ist, andernfalls null.
    /// </returns>
    /// <param name="ch">Das zu überprüfende <see cref="T:System.Char"/>-Objekt.</param>
    public static bool IsPublicIdChar(char ch)
    {
      return Xml_Convert.xmlCharType.IsPubidChar(ch);
    }

    /// <summary>
    /// Überprüft, ob das übergebene Zeichen ein gültiges XML-Leerraumzeichen ist.
    /// </summary>
    /// 
    /// <returns>
    /// true, wenn das übergebene Zeichen ein gültiges XML-Leerraumzeichen ist, andernfalls false.
    /// </returns>
    /// <param name="ch">Das Zeichen, das validiert werden soll.</param>
    public static unsafe bool IsWhitespaceChar(char ch)
    {
      return ((uint)Xml_Convert.xmlCharType.charProperties[(int)ch] & 1U) > 0U;
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.Boolean"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung von Boolean, d. h. „true“ oder „false“.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(bool value)
    {
      return !value ? "false" : "true";
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.Char"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des Char.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(char value)
    {
      return value.ToString((IFormatProvider)null);
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.Decimal"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des Decimal.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(Decimal value)
    {
      return value.ToString((string)null, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.SByte"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des SByte.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(sbyte value)
    {
      return value.ToString((string)null, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.Int16"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des Int16.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(short value)
    {
      return value.ToString((string)null, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.Int32"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des Int32.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(int value)
    {
      return value.ToString((string)null, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.Int64"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des Int64.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(long value)
    {
      return value.ToString((string)null, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.Byte"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des Byte.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(byte value)
    {
      return value.ToString((string)null, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.UInt16"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des UInt16.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(ushort value)
    {
      return value.ToString((string)null, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.UInt32"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des UInt32.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(uint value)
    {
      return value.ToString((string)null, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.UInt64"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des UInt64.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(ulong value)
    {
      return value.ToString((string)null, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.Single"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des Single.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(float value)
    {
      if (float.IsNegativeInfinity(value))
        return "-INF";
      if (float.IsPositiveInfinity(value))
        return "INF";
      if (Xml_Convert.IsNegativeZero((double)value))
        return "-0";
      return value.ToString("R", (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.Double"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des Double.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(double value)
    {
      if (double.IsNegativeInfinity(value))
        return "-INF";
      if (double.IsPositiveInfinity(value))
        return "INF";
      if (Xml_Convert.IsNegativeZero(value))
        return "-0";
      return value.ToString("R", (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.TimeSpan"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des TimeSpan.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(TimeSpan value)
    {
      return new XsdDuration(value).ToString();
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.DateTime"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung von DateTime im Format „yyyy-MM-ddTHH:mm:ss , wobei „T“ ein konstantes Literal ist.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    [Obsolete("Use XmlConvert.ToString() that takes in XmlDateTimeSerializationMode")]
    public static string ToString(DateTime value)
    {
      return Xml_Convert.ToString(value, "yyyy-MM-ddTHH:mm:ss.fffffffzzzzzz");
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.DateTime"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung von DateTime im angegebenen Format.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param><param name="format">Die Formatstruktur, die die Anzeige der konvertierten Zeichenfolge definiert.Zu den gültigen Format zählt „yyyy-MM-ddTHH:mm:sszzzzzz“ und dessen Teilmengen.</param>
    public static string ToString(DateTime value, string format)
    {
      return value.ToString(format, (IFormatProvider)DateTimeFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Konvertiert die <see cref="T:System.DateTime"/>-Struktur mithilfe von <see cref="T:System.Xml.XmlDateTimeSerializationMode"/> in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.String"/>-Äquivalent von <see cref="T:System.DateTime"/>.
    /// </returns>
    /// <param name="value">Der zu konvertierende <see cref="T:System.DateTime"/>-Wert.</param><param name="dateTimeOption">Einer der <see cref="T:System.Xml.XmlDateTimeSerializationMode"/>-Werte, die angeben, wie der <see cref="T:System.DateTime"/>-Wert behandelt wird.</param><exception cref="T:System.ArgumentException">Der <paramref name="dateTimeOption"/> -Wert ist ungültig.</exception><exception cref="T:System.ArgumentNullException">Die <paramref name="value"/> oder <paramref name="dateTimeOption"/> Wert ist null.</exception>
    public static string ToString(DateTime value, XmlDateTimeSerializationMode dateTimeOption)
    {
      switch (dateTimeOption)
      {
        case XmlDateTimeSerializationMode.Local:
        value = Xml_Convert.SwitchToLocalTime(value);
        goto case 3;
        case XmlDateTimeSerializationMode.Utc:
        value = Xml_Convert.SwitchToUtcTime(value);
        goto case 3;
        case XmlDateTimeSerializationMode.Unspecified:
        value = new DateTime(value.Ticks, DateTimeKind.Unspecified);
        goto case 3;
        case XmlDateTimeSerializationMode.RoundtripKind:
        return new XsdDateTime(value, XsdDateTimeFlags.DateTime).ToString();
        default:
        throw new ArgumentException(Res.GetString("Sch_InvalidDateTimeOption", (object)dateTimeOption, (object)"dateTimeOption"));
      }
    }

    /// <summary>
    /// Konvertiert den angegebenen <see cref="T:System.DateTimeOffset"/> in einen <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine <see cref="T:System.String"/>-Darstellung des angegebenen <see cref="T:System.DateTimeOffset"/>.
    /// </returns>
    /// <param name="value">Der zu konvertierende <see cref="T:System.DateTimeOffset"/>.</param>
    public static string ToString(DateTimeOffset value)
    {
      return new XsdDateTime(value).ToString();
    }

    /// <summary>
    /// Konvertiert den angegebenen <see cref="T:System.DateTimeOffset"/> in einen <see cref="T:System.String"/> im angegebenen Format.
    /// </summary>
    /// 
    /// <returns>
    /// Eine <see cref="T:System.String"/>-Darstellung im angegebenen Format des bereitgestellten <see cref="T:System.DateTimeOffset"/>.
    /// </returns>
    /// <param name="value">Der zu konvertierende <see cref="T:System.DateTimeOffset"/>.</param><param name="format">Das Format, in das <paramref name="s"/> konvertiert wird.Der Formatparameter kann eine beliebige Teilmenge der W3C-Empfehlung für den XML-DateTime-Typ sein.(Weitere Informationen finden Sie unter „http://www.w3.org/TR/xmlschema-2/#dateTime“.)</param>
    public static string ToString(DateTimeOffset value, string format)
    {
      return value.ToString(format, (IFormatProvider)DateTimeFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Konvertiert das <see cref="T:System.Guid"/>-Element in eine <see cref="T:System.String"/>.
    /// </summary>
    /// 
    /// <returns>
    /// Eine Zeichenfolgendarstellung des Guid.
    /// </returns>
    /// <param name="value">Der zu konvertierende Wert. </param>
    public static string ToString(Guid value)
    {
      return value.ToString();
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.Boolean"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Boolean-Wert, d. h. true oder false.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> keine dar eine Boolean Wert. </exception>
    public static bool ToBoolean(string s)
    {
      s = Xml_Convert.TrimString(s);
      if (s == "1" || s == "true")
        return true;
      if (s == "0" || s == "false")
        return false;
      throw new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Boolean"));
    }

    internal static Exception TryToBoolean(string s, out bool result)
    {
      s = Xml_Convert.TrimString(s);
      if (s == "0" || s == "false")
      {
        result = false;
        return (Exception)null;
      }
      if (s == "1" || s == "true")
      {
        result = true;
        return (Exception)null;
      }
      result = false;
      return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Boolean"));
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.Char"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Char, das für das einzelne Zeichen steht.
    /// </returns>
    /// <param name="s">Die Zeichenfolge, die ein einzelnes zu konvertierendes Zeichen enthält. </param><exception cref="T:System.ArgumentNullException">Der Wert des <paramref name="s"/>-Parameters ist null. </exception><exception cref="T:System.FormatException">Die <paramref name="s"/> Parameter enthält mehr als ein Zeichen. </exception>
    public static char ToChar(string s)
    {
      if (s == null)
        throw new ArgumentNullException("s");
      if (s.Length != 1)
        throw new FormatException(Res.GetString("XmlConvert_NotOneCharString"));
      return s[0];
    }

    internal static Exception TryToChar(string s, out char result)
    {
      if (char.TryParse(s, out result))
        return (Exception)null;
      return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Char"));
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.Decimal"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Decimal-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> ist nicht im richtigen Format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> Stellt eine Zahl kleiner als <see cref="F:System.Decimal.MinValue"/> oder größer als <see cref="F:System.Decimal.MaxValue"/>. </exception>
    public static Decimal ToDecimal(string s)
    {
      return Decimal.Parse(s, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToDecimal(string s, out Decimal result)
    {
      if (Decimal.TryParse(s, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, (IFormatProvider)NumberFormatInfo.InvariantInfo, out result))
        return (Exception)null;
      return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Decimal"));
    }

    internal static Decimal ToInteger(string s)
    {
      return Decimal.Parse(s, NumberStyles.Integer, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToInteger(string s, out Decimal result)
    {
      if (Decimal.TryParse(s, NumberStyles.Integer, (IFormatProvider)NumberFormatInfo.InvariantInfo, out result))
        return (Exception)null;
      return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Integer"));
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.SByte"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein SByte-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> ist nicht im richtigen Format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> Stellt eine Zahl kleiner als <see cref="F:System.SByte.MinValue"/> oder größer als <see cref="F:System.SByte.MaxValue"/>. </exception>
    public static sbyte ToSByte(string s)
    {
      return sbyte.Parse(s, NumberStyles.Integer, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToSByte(string s, out sbyte result)
    {
      if (sbyte.TryParse(s, NumberStyles.Integer, (IFormatProvider)NumberFormatInfo.InvariantInfo, out result))
        return (Exception)null;
      return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"SByte"));
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.Int16"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Int16-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> ist nicht im richtigen Format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> Stellt eine Zahl kleiner als <see cref="F:System.Int16.MinValue"/> oder größer als <see cref="F:System.Int16.MaxValue"/>. </exception>
    public static short ToInt16(string s)
    {
      return short.Parse(s, NumberStyles.Integer, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToInt16(string s, out short result)
    {
      if (short.TryParse(s, NumberStyles.Integer, (IFormatProvider)NumberFormatInfo.InvariantInfo, out result))
        return (Exception)null;
      return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Int16"));
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.Int32"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Int32-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> ist nicht im richtigen Format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> Stellt eine Zahl kleiner als <see cref="F:System.Int32.MinValue"/> oder größer als <see cref="F:System.Int32.MaxValue"/>. </exception>
    public static int ToInt32(string s)
    {
      return int.Parse(s, NumberStyles.Integer, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToInt32(string s, out int result)
    {
      if (int.TryParse(s, NumberStyles.Integer, (IFormatProvider)NumberFormatInfo.InvariantInfo, out result))
        return (Exception)null;
      return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Int32"));
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.Int64"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Int64-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> ist nicht im richtigen Format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> Stellt eine Zahl kleiner als <see cref="F:System.Int64.MinValue"/> oder größer als <see cref="F:System.Int64.MaxValue"/>. </exception>
    public static long ToInt64(string s)
    {
      return long.Parse(s, NumberStyles.Integer, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToInt64(string s, out long result)
    {
      if (long.TryParse(s, NumberStyles.Integer, (IFormatProvider)NumberFormatInfo.InvariantInfo, out result))
        return (Exception)null;
      return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Int64"));
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.Byte"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Byte-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> ist nicht im richtigen Format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> Stellt eine Zahl kleiner als <see cref="F:System.Byte.MinValue"/> oder größer als <see cref="F:System.Byte.MaxValue"/>. </exception>
    public static byte ToByte(string s)
    {
      return byte.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToByte(string s, out byte result)
    {
      if (byte.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, (IFormatProvider)NumberFormatInfo.InvariantInfo, out result))
        return (Exception)null;
      return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Byte"));
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.UInt16"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein UInt16-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> ist nicht im richtigen Format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> Stellt eine Zahl kleiner als <see cref="F:System.UInt16.MinValue"/> oder größer als <see cref="F:System.UInt16.MaxValue"/>. </exception>
    public static ushort ToUInt16(string s)
    {
      return ushort.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToUInt16(string s, out ushort result)
    {
      if (ushort.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, (IFormatProvider)NumberFormatInfo.InvariantInfo, out result))
        return (Exception)null;
      return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"UInt16"));
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.UInt32"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein UInt32-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> ist nicht im richtigen Format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> Stellt eine Zahl kleiner als <see cref="F:System.UInt32.MinValue"/> oder größer als <see cref="F:System.UInt32.MaxValue"/>. </exception>
    public static uint ToUInt32(string s)
    {
      return uint.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToUInt32(string s, out uint result)
    {
      if (uint.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, (IFormatProvider)NumberFormatInfo.InvariantInfo, out result))
        return (Exception)null;
      return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"UInt32"));
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.UInt64"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein UInt64-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> ist nicht im richtigen Format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> Stellt eine Zahl kleiner als <see cref="F:System.UInt64.MinValue"/> oder größer als <see cref="F:System.UInt64.MaxValue"/>. </exception>
    public static ulong ToUInt64(string s)
    {
      return ulong.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, (IFormatProvider)NumberFormatInfo.InvariantInfo);
    }

    internal static Exception TryToUInt64(string s, out ulong result)
    {
      if (ulong.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, (IFormatProvider)NumberFormatInfo.InvariantInfo, out result))
        return (Exception)null;
      return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"UInt64"));
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.Single"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Single-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> ist nicht im richtigen Format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> Stellt eine Zahl kleiner als <see cref="F:System.Single.MinValue"/> oder größer als <see cref="F:System.Single.MaxValue"/>. </exception>
    public static float ToSingle(string s)
    {
      s = Xml_Convert.TrimString(s);
      if (s == "-INF")
        return float.NegativeInfinity;
      if (s == "INF")
        return float.PositiveInfinity;
      float num = float.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, (IFormatProvider)NumberFormatInfo.InvariantInfo);
      if ((double)num == 0.0 && (int)s[0] == 45)
        return -0.0f;
      return num;
    }

    internal static Exception TryToSingle(string s, out float result)
    {
      s = Xml_Convert.TrimString(s);
      if (s == "-INF")
      {
        result = float.NegativeInfinity;
        return (Exception)null;
      }
      if (s == "INF")
      {
        result = float.PositiveInfinity;
        return (Exception)null;
      }
      if (!float.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, (IFormatProvider)NumberFormatInfo.InvariantInfo, out result))
        return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Single"));
      if ((double)result == 0.0 && (int)s[0] == 45)
        result = -0.0f;
      return (Exception)null;
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.Double"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Double-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> ist nicht im richtigen Format. </exception><exception cref="T:System.OverflowException"><paramref name="s"/> Stellt eine Zahl kleiner als <see cref="F:System.Double.MinValue"/> oder größer als <see cref="F:System.Double.MaxValue"/>. </exception>
    public static double ToDouble(string s)
    {
      s = Xml_Convert.TrimString(s);
      if (s == "-INF")
        return double.NegativeInfinity;
      if (s == "INF")
        return double.PositiveInfinity;
      double num = double.Parse(s, NumberStyles.Float, (IFormatProvider)NumberFormatInfo.InvariantInfo);
      if (num == 0.0 && (int)s[0] == 45)
        return -0.0;
      return num;
    }

    internal static Exception TryToDouble(string s, out double result)
    {
      s = Xml_Convert.TrimString(s);
      if (s == "-INF")
      {
        result = double.NegativeInfinity;
        return (Exception)null;
      }
      if (s == "INF")
      {
        result = double.PositiveInfinity;
        return (Exception)null;
      }
      if (!double.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, (IFormatProvider)NumberFormatInfo.InvariantInfo, out result))
        return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Double"));
      if (result == 0.0 && (int)s[0] == 45)
        result = -0.0;
      return (Exception)null;
    }

    internal static double ToXPathDouble(object o)
    {
      string str = o as string;
      if (str != null)
      {
        string s = Xml_Convert.TrimString(str);
        double result;
        if (s.Length != 0 && (int)s[0] != 43 && double.TryParse(s, NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, (IFormatProvider)NumberFormatInfo.InvariantInfo, out result))
          return result;
        return double.NaN;
      }
      if (o is double)
        return (double)o;
      if (o is bool)
        return !(bool)o ? 0.0 : 1.0;
      try
      {
        return Convert.ToDouble(o, (IFormatProvider)NumberFormatInfo.InvariantInfo);
      }
      catch (FormatException ex)
      {
      }
      catch (OverflowException ex)
      {
      }
      catch (ArgumentNullException ex)
      {
      }
      return double.NaN;
    }

    internal static string ToXPathString(object value)
    {
      string str = value as string;
      if (str != null)
        return str;
      if (value is double)
        return ((double)value).ToString("R", (IFormatProvider)NumberFormatInfo.InvariantInfo);
      if (!(value is bool))
        return Convert.ToString(value, (IFormatProvider)NumberFormatInfo.InvariantInfo);
      return !(bool)value ? "false" : "true";
    }

    internal static double XPathRound(double value)
    {
      double num = Math.Round(value);
      if (value - num != 0.5)
        return num;
      return num + 1.0;
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.TimeSpan"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein TimeSpan-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge.Das Zeichenfolgenformat muss dem W3C-XML-Schema Teil 2 entsprechen: Empfehlung für Datentypen für Dauer.</param><exception cref="T:System.FormatException"><paramref name="s"/> befindet sich nicht im richtigen Format zum Darstellen einer TimeSpan Wert. </exception>
    public static TimeSpan ToTimeSpan(string s)
    {
      XsdDuration xsdDuration;
      try
      {
        xsdDuration = new XsdDuration(s);
      }
      catch (Exception ex)
      {
        throw new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"TimeSpan"));
      }
      return xsdDuration.ToTimeSpan();
    }

    internal static Exception TryToTimeSpan(string s, out TimeSpan result)
    {
      XsdDuration result1;
      Exception exception = XsdDuration.TryParse(s, out result1);
      if (exception == null)
        return result1.TryToTimeSpan(out result);
      result = TimeSpan.MinValue;
      return exception;
    }

    private static void CreateAllDateTimeFormats()
    {
      if (Xml_Convert.s_allDateTimeFormats != null)
        return;
      Xml_Convert.s_allDateTimeFormats = new string[24]
      {
        "yyyy-MM-ddTHH:mm:ss.FFFFFFFzzzzzz",
        "yyyy-MM-ddTHH:mm:ss.FFFFFFF",
        "yyyy-MM-ddTHH:mm:ss.FFFFFFFZ",
        "HH:mm:ss.FFFFFFF",
        "HH:mm:ss.FFFFFFFZ",
        "HH:mm:ss.FFFFFFFzzzzzz",
        "yyyy-MM-dd",
        "yyyy-MM-ddZ",
        "yyyy-MM-ddzzzzzz",
        "yyyy-MM",
        "yyyy-MMZ",
        "yyyy-MMzzzzzz",
        "yyyy",
        "yyyyZ",
        "yyyyzzzzzz",
        "--MM-dd",
        "--MM-ddZ",
        "--MM-ddzzzzzz",
        "---dd",
        "---ddZ",
        "---ddzzzzzz",
        "--MM--",
        "--MM--Z",
        "--MM--zzzzzz"
      };
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.DateTime"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein DateTime-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> ist eine leere Zeichenfolge oder ist nicht im richtigen Format. </exception>
    [Obsolete("Use XmlConvert.ToDateTime() that takes in XmlDateTimeSerializationMode")]
    public static DateTime ToDateTime(string s)
    {
      return Xml_Convert.ToDateTime(s, Xml_Convert.AllDateTimeFormats);
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.DateTime"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein DateTime-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><param name="format">Die auf das konvertierte DateTime anzuwendende Formatstruktur.Zu den gültigen Format zählt „yyyy-MM-ddTHH:mm:sszzzzzz“ und dessen Teilmengen.Es wird validiert, ob die Zeichenfolge dieses Format aufweist.</param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> oder <paramref name="format"/> ist String.Empty. - oder -  <paramref name="s"/> enthält Datum und Uhrzeit, die entspricht, nicht <paramref name="format"/>. </exception>
    public static DateTime ToDateTime(string s, string format)
    {
      return DateTime.ParseExact(s, format, (IFormatProvider)DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite);
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.DateTime"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein DateTime-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param><param name="formats">Ein Array, das die Formatstrukturen enthält, die auf das konvertierte DateTime angewendet werden sollen.Zu den gültigen Format zählt „yyyy-MM-ddTHH:mm:sszzzzzz“ und dessen Teilmengen.</param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> oder ein Element eines <paramref name="formats"/> ist String.Empty. - oder -  <paramref name="s"/> enthält Datum und Uhrzeit, die einem der Elemente des entsprechen, keine <paramref name="formats"/>. </exception>
    public static DateTime ToDateTime(string s, string[] formats)
    {
      return DateTime.ParseExact(s, formats, (IFormatProvider)DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite);
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> mithilfe von <see cref="T:System.Xml.XmlDateTimeSerializationMode"/> in eine <see cref="T:System.DateTime"/>-Struktur.
    /// </summary>
    /// 
    /// <returns>
    /// Ein <see cref="T:System.DateTime"/>-Äquivalent von <see cref="T:System.String"/>.
    /// </returns>
    /// <param name="s">Der zu konvertierende <see cref="T:System.String"/>-Wert.</param><param name="dateTimeOption">Einer der <see cref="T:System.Xml.XmlDateTimeSerializationMode"/>-Werte, die angeben, ob das Datum in die Ortszeit konvertiert oder als UTC-Zeit (Coordinated Universal Time) beibehalten werden soll, falls es sich um ein UTC-Datum handelt.</param><exception cref="T:System.NullReferenceException"><paramref name="s"/> ist null.</exception><exception cref="T:System.ArgumentNullException">Der <paramref name="dateTimeOption"/> Wert ist null.</exception><exception cref="T:System.FormatException"><paramref name="s"/> ist eine leere Zeichenfolge oder ist nicht in einem gültigen Format.</exception>
    public static DateTime ToDateTime(string s, XmlDateTimeSerializationMode dateTimeOption)
    {
      DateTime dateTime = (DateTime)new XsdDateTime(s, XsdDateTimeFlags.AllXsd);
      switch (dateTimeOption)
      {
        case XmlDateTimeSerializationMode.Local:
        dateTime = Xml_Convert.SwitchToLocalTime(dateTime);
        goto case 3;
        case XmlDateTimeSerializationMode.Utc:
        dateTime = Xml_Convert.SwitchToUtcTime(dateTime);
        goto case 3;
        case XmlDateTimeSerializationMode.Unspecified:
        dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Unspecified);
        goto case 3;
        case XmlDateTimeSerializationMode.RoundtripKind:
        return dateTime;
        default:
        throw new ArgumentException(Res.GetString("Sch_InvalidDateTimeOption", (object)dateTimeOption, (object)"dateTimeOption"));
      }
    }

    /// <summary>
    /// Konvertiert den angegebenen <see cref="T:System.String"/> in ein <see cref="T:System.DateTimeOffset"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Das <see cref="T:System.DateTimeOffset"/>-Äquivalent der angegebenen Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge.Hinweis   Die Zeichenfolge muss einer Teilmenge der W3C-Empfehlung für den XML-dateTime-Typ entsprechen.Weitere Informationen finden Sie unter „http://www.w3.org/TR/xmlschema-2/#dateTime“.</param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.ArgumentOutOfRangeException">Das an diese Methode übergebene Argument liegt außerhalb des Bereichs der zulässigen Werte.Weitere Informationen zu zulässigen Werten finden Sie unter <see cref="T:System.DateTimeOffset"/>.</exception><exception cref="T:System.FormatException">Das an diese Methode übergebene Argument entspricht nicht auf eine Teilmenge der W3C-Empfehlungen für den XML-DateTime-Typ.Weitere Informationen finden Sie unter http://www.w3.org/TR/xmlschema-2/#dateTime.</exception>
    public static DateTimeOffset ToDateTimeOffset(string s)
    {
      if (s == null)
        throw new ArgumentNullException("s");
      return (DateTimeOffset)new XsdDateTime(s, XsdDateTimeFlags.AllXsd);
    }

    /// <summary>
    /// Konvertiert den angegebenen <see cref="T:System.String"/> in ein <see cref="T:System.DateTimeOffset"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Das <see cref="T:System.DateTimeOffset"/>-Äquivalent der angegebenen Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge.</param><param name="format">Das Format, aus dem <paramref name="s"/> konvertiert wird.Der Formatparameter kann eine beliebige Teilmenge der W3C-Empfehlung für den XML-DateTime-Typ sein.(Weitere Informationen finden Sie unter „http://www.w3.org/TR/xmlschema-2/#dateTime“.) Die Gültigkeit der Zeichenfolge <paramref name="s"/> wird anhand dieses Formats überprüft.</param><exception cref="T:System.ArgumentNullException"><paramref name="s"/> ist null. </exception><exception cref="T:System.FormatException"><paramref name="s"/> oder <paramref name="format"/> ist eine leere Zeichenfolge oder ist nicht im angegebenen Format.</exception>
    public static DateTimeOffset ToDateTimeOffset(string s, string format)
    {
      if (s == null)
        throw new ArgumentNullException("s");
      return DateTimeOffset.ParseExact(s, format, (IFormatProvider)DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite);
    }

    /// <summary>
    /// Konvertiert den angegebenen <see cref="T:System.String"/> in ein <see cref="T:System.DateTimeOffset"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Das <see cref="T:System.DateTimeOffset"/>-Äquivalent der angegebenen Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge.</param><param name="formats">Ein Array von Formaten, aus denen <paramref name="s"/> konvertiert werden kann.Jedes Format in <paramref name="formats"/> kann eine beliebige Teilmenge der W3C-Empfehlung für den XML-DateTime-Typ sein.(Weitere Informationen finden Sie unter „http://www.w3.org/TR/xmlschema-2/#dateTime“.) Die Gültigkeit der Zeichenfolge <paramref name="s"/> wird im Vergleich mit einem dieser Formate überprüft.</param>
    public static DateTimeOffset ToDateTimeOffset(string s, string[] formats)
    {
      if (s == null)
        throw new ArgumentNullException("s");
      return DateTimeOffset.ParseExact(s, formats, (IFormatProvider)DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite);
    }

    /// <summary>
    /// Konvertiert den <see cref="T:System.String"/> in ein <see cref="T:System.Guid"/>-Äquivalent.
    /// </summary>
    /// 
    /// <returns>
    /// Ein Guid-Äquivalent der Zeichenfolge.
    /// </returns>
    /// <param name="s">Die zu konvertierende Zeichenfolge. </param>
    public static Guid ToGuid(string s)
    {
      return new Guid(s);
    }

    internal static Exception TryToGuid(string s, out Guid result)
    {
      Exception exception = (Exception)null;
      result = Guid.Empty;
      try
      {
        result = new Guid(s);
      }
      catch (ArgumentException ex)
      {
        exception = (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Guid"));
      }
      catch (FormatException ex)
      {
        exception = (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Guid"));
      }
      return exception;
    }

    private static DateTime SwitchToLocalTime(DateTime value)
    {
      switch (value.Kind)
      {
        case DateTimeKind.Unspecified:
        return new DateTime(value.Ticks, DateTimeKind.Local);
        case DateTimeKind.Utc:
        return value.ToLocalTime();
        case DateTimeKind.Local:
        return value;
        default:
        return value;
      }
    }

    private static DateTime SwitchToUtcTime(DateTime value)
    {
      switch (value.Kind)
      {
        case DateTimeKind.Unspecified:
        return new DateTime(value.Ticks, DateTimeKind.Utc);
        case DateTimeKind.Utc:
        return value;
        case DateTimeKind.Local:
        return value.ToUniversalTime();
        default:
        return value;
      }
    }

    internal static Uri ToUri(string s)
    {
      if (s != null && s.Length > 0)
      {
        s = Xml_Convert.TrimString(s);
        if (s.Length == 0 || s.IndexOf("##", StringComparison.Ordinal) != -1)
          throw new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Uri"));
      }
      Uri result;
      if (!Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out result))
        throw new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Uri"));
      return result;
    }

    internal static Exception TryToUri(string s, out Uri result)
    {
      result = (Uri)null;
      if (s != null && s.Length > 0)
      {
        s = Xml_Convert.TrimString(s);
        if (s.Length == 0 || s.IndexOf("##", StringComparison.Ordinal) != -1)
          return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Uri"));
      }
      if (Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out result))
        return (Exception)null;
      return (Exception)new FormatException(Res.GetString("XmlConvert_BadFormat", (object)s, (object)"Uri"));
    }

    internal static bool StrEqual(char[] chars, int strPos1, int strLen1, string str2)
    {
      if (strLen1 != str2.Length)
        return false;
      int index = 0;
      while (index < strLen1 && (int)chars[strPos1 + index] == (int)str2[index])
        ++index;
      return index == strLen1;
    }

    internal static string TrimString(string value)
    {
      return value.Trim(Xml_Convert.WhitespaceChars);
    }

    internal static string TrimStringStart(string value)
    {
      return value.TrimStart(Xml_Convert.WhitespaceChars);
    }

    internal static string TrimStringEnd(string value)
    {
      return value.TrimEnd(Xml_Convert.WhitespaceChars);
    }

    internal static string[] SplitString(string value)
    {
      return value.Split(Xml_Convert.WhitespaceChars, StringSplitOptions.RemoveEmptyEntries);
    }

    internal static string[] SplitString(string value, StringSplitOptions splitStringOptions)
    {
      return value.Split(Xml_Convert.WhitespaceChars, splitStringOptions);
    }

    internal static bool IsNegativeZero(double value)
    {
      return value == 0.0 && Xml_Convert.DoubleToInt64Bits(value) == Xml_Convert.DoubleToInt64Bits(-0.0);
    }

    private static unsafe long DoubleToInt64Bits(double value)
    {
      return *(long*)&value;
    }

    internal static void VerifyCharData(string data, ExceptionType exceptionType)
    {
      string data1 = data;
      int num = (int)exceptionType;
      Xml_Convert.VerifyCharData(data1, (ExceptionType)num, (ExceptionType)num);
    }

    internal static unsafe void VerifyCharData(string data, ExceptionType invCharExceptionType, ExceptionType invSurrogateExceptionType)
    {
      if (data == null || data.Length == 0)
        return;
      int invCharPos = 0;
      int length = data.Length;
      while (true)
      {
        while (invCharPos >= length || ((int)Xml_Convert.xmlCharType.charProperties[(int)data[invCharPos]] & 16) == 0)
        {
          if (invCharPos == length)
            return;
          if (!Xml_CharType.IsHighSurrogate((int)data[invCharPos]))
            throw Xml_Convert.CreateInvalidCharException(data, invCharPos, invCharExceptionType);
          if (invCharPos + 1 == length)
            throw Xml_Convert.CreateException("Xml_InvalidSurrogateMissingLowChar", invSurrogateExceptionType, 0, invCharPos + 1);
          if (!Xml_CharType.IsLowSurrogate((int)data[invCharPos + 1]))
            throw Xml_Convert.CreateInvalidSurrogatePairException(data[invCharPos + 1], data[invCharPos], invSurrogateExceptionType, 0, invCharPos + 1);
          invCharPos += 2;
        }
        ++invCharPos;
      }
    }

    internal static unsafe void VerifyCharData(char[] data, int offset, int len, ExceptionType exceptionType)
    {
      if (data == null || len == 0)
        return;
      int invCharPos = offset;
      int num = offset + len;
      while (true)
      {
        while (invCharPos >= num || ((int)Xml_Convert.xmlCharType.charProperties[(int)data[invCharPos]] & 16) == 0)
        {
          if (invCharPos == num)
            return;
          if (!Xml_CharType.IsHighSurrogate((int)data[invCharPos]))
            throw Xml_Convert.CreateInvalidCharException(data, len, invCharPos, exceptionType);
          if (invCharPos + 1 == num)
            throw Xml_Convert.CreateException("Xml_InvalidSurrogateMissingLowChar", exceptionType, 0, offset - invCharPos + 1);
          if (!Xml_CharType.IsLowSurrogate((int)data[invCharPos + 1]))
            throw Xml_Convert.CreateInvalidSurrogatePairException(data[invCharPos + 1], data[invCharPos], exceptionType, 0, offset - invCharPos + 1);
          invCharPos += 2;
        }
        ++invCharPos;
      }
    }

    internal static string EscapeValueForDebuggerDisplay(string value)
    {
      StringBuilder stringBuilder = (StringBuilder)null;
      int index = 0;
      int startIndex = 0;
      for (; index < value.Length; ++index)
      {
        char ch = value[index];
        if ((int)ch < 32 || (int)ch == 34)
        {
          if (stringBuilder == null)
            stringBuilder = new StringBuilder(value.Length + 4);
          if (index - startIndex > 0)
            stringBuilder.Append(value, startIndex, index - startIndex);
          startIndex = index + 1;
          switch (ch)
          {
            case '\t':
            stringBuilder.Append("\\t");
            continue;
            case '\n':
            stringBuilder.Append("\\n");
            continue;
            case '\r':
            stringBuilder.Append("\\r");
            continue;
            case '"':
            stringBuilder.Append("\\\"");
            continue;
            default:
            stringBuilder.Append(ch);
            continue;
          }
        }
      }
      if (stringBuilder == null)
        return value;
      if (index - startIndex > 0)
        stringBuilder.Append(value, startIndex, index - startIndex);
      return stringBuilder.ToString();
    }

    internal static Exception CreateException(string res, ExceptionType exceptionType)
    {
      return Xml_Convert.CreateException(res, exceptionType, 0, 0);
    }

    internal static Exception CreateException(string res, ExceptionType exceptionType, int lineNo, int linePos)
    {
      if (exceptionType == ExceptionType.ArgumentException)
        return (Exception)new ArgumentException(Res.GetString(res));
      if (exceptionType == ExceptionType.XmlException)
        ;
      return (Exception)new XmlException(res, string.Empty, lineNo, linePos);
    }

    internal static Exception CreateException(string res, string arg, ExceptionType exceptionType)
    {
      return Xml_Convert.CreateException(res, arg, exceptionType, 0, 0);
    }

    internal static Exception CreateException(string res, string arg, ExceptionType exceptionType, int lineNo, int linePos)
    {
      if (exceptionType != ExceptionType.ArgumentException)
      {
        if (exceptionType == ExceptionType.XmlException)
          ;
        return (Exception)new XmlException(res, arg, lineNo, linePos);
      }
      return (Exception)new ArgumentException(Res.GetString(res, new object[1]
      {
        (object) arg
      }));
    }

    internal static Exception CreateException(string res, string[] args, ExceptionType exceptionType)
    {
      return Xml_Convert.CreateException(res, args, exceptionType, 0, 0);
    }

    internal static Exception CreateException(string res, string[] args, ExceptionType exceptionType, int lineNo, int linePos)
    {
      if (exceptionType == ExceptionType.ArgumentException)
        return (Exception)new ArgumentException(Res.GetString(res, (object[])args));
      if (exceptionType == ExceptionType.XmlException)
        ;
      return (Exception)new XmlException(res, args, lineNo, linePos);
    }

    internal static Exception CreateInvalidSurrogatePairException(char low, char hi)
    {
      return Xml_Convert.CreateInvalidSurrogatePairException(low, hi, ExceptionType.ArgumentException);
    }

    internal static Exception CreateInvalidSurrogatePairException(char low, char hi, ExceptionType exceptionType)
    {
      return Xml_Convert.CreateInvalidSurrogatePairException(low, hi, exceptionType, 0, 0);
    }

    internal static Exception CreateInvalidSurrogatePairException(char low, char hi, ExceptionType exceptionType, int lineNo, int linePos)
    {
      return Xml_Convert.CreateException("Xml_InvalidSurrogatePairWithArgs", new string[2]
      {
        ((uint) hi).ToString("X", (IFormatProvider) CultureInfo.InvariantCulture),
        ((uint) low).ToString("X", (IFormatProvider) CultureInfo.InvariantCulture)
      }, exceptionType, lineNo, linePos);
    }

    internal static Exception CreateInvalidHighSurrogateCharException(char hi)
    {
      return Xml_Convert.CreateInvalidHighSurrogateCharException(hi, ExceptionType.ArgumentException);
    }

    internal static Exception CreateInvalidHighSurrogateCharException(char hi, ExceptionType exceptionType)
    {
      return Xml_Convert.CreateInvalidHighSurrogateCharException(hi, exceptionType, 0, 0);
    }

    internal static Exception CreateInvalidHighSurrogateCharException(char hi, ExceptionType exceptionType, int lineNo, int linePos)
    {
      return Xml_Convert.CreateException("Xml_InvalidSurrogateHighChar", ((uint)hi).ToString("X", (IFormatProvider)CultureInfo.InvariantCulture), exceptionType, lineNo, linePos);
    }

    internal static Exception CreateInvalidCharException(char[] data, int length, int invCharPos)
    {
      return Xml_Convert.CreateInvalidCharException(data, length, invCharPos, ExceptionType.ArgumentException);
    }

    internal static Exception CreateInvalidCharException(char[] data, int length, int invCharPos, ExceptionType exceptionType)
    {
      return Xml_Convert.CreateException("Xml_InvalidCharacter", XmlException.BuildCharExceptionArgs(data, length, invCharPos), exceptionType, 0, invCharPos + 1);
    }

    internal static Exception CreateInvalidCharException(string data, int invCharPos)
    {
      return Xml_Convert.CreateInvalidCharException(data, invCharPos, ExceptionType.ArgumentException);
    }

    internal static Exception CreateInvalidCharException(string data, int invCharPos, ExceptionType exceptionType)
    {
      return Xml_Convert.CreateException("Xml_InvalidCharacter", XmlException.BuildCharExceptionArgs(data, invCharPos), exceptionType, 0, invCharPos + 1);
    }

    internal static Exception CreateInvalidCharException(char invChar, char nextChar)
    {
      return Xml_Convert.CreateInvalidCharException(invChar, nextChar, ExceptionType.ArgumentException);
    }

    internal static Exception CreateInvalidCharException(char invChar, char nextChar, ExceptionType exceptionType)
    {
      return Xml_Convert.CreateException("Xml_InvalidCharacter", XmlException.BuildCharExceptionArgs(invChar, nextChar), exceptionType);
    }

    internal static Exception CreateInvalidNameCharException(string name, int index, ExceptionType exceptionType)
    {
      return Xml_Convert.CreateException(index == 0 ? "Xml_BadStartNameChar" : "Xml_BadNameChar", XmlException.BuildCharExceptionArgs(name, index), exceptionType, 0, index + 1);
    }

    internal static ArgumentException CreateInvalidNameArgumentException(string name, string argumentName)
    {
      if (name != null)
        return new ArgumentException(Res.GetString("Xml_EmptyName"), argumentName);
      return (ArgumentException)new ArgumentNullException(argumentName);
    }
  }
}
