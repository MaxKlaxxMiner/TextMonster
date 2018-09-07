using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Contains various static functions and methods for parsing and validating:
  ///     NCName (not namespace-aware, no colons allowed)
  ///     QName (prefix:local-name)
  /// </summary>
  internal static class ValidateNames
  {
    static XmlCharType xmlCharType = XmlCharType.Instance;

    //-----------------------------------------------
    // Nmtoken parsing
    //-----------------------------------------------
    /// <summary>
    /// Attempts to parse the input string as an Nmtoken (see the XML spec production [7] && XML Namespaces spec).
    /// Quits parsing when an invalid Nmtoken char is reached or the end of string is reached.
    /// Returns the number of valid Nmtoken chars that were parsed.
    /// </summary>
    internal static unsafe int ParseNmtoken(string s, int offset)
    {
      // Keep parsing until the end of string or an invalid NCName character is reached
      int i = offset;
      while (i < s.Length)
      {
        if ((xmlCharType.charProperties[s[i]] & XmlCharType.fNCNameSC) != 0)
        { // if (xmlCharType.IsNCNameSingleChar(s[i])) {
          i++;
        }
        else
        {
          break;
        }
      }

      return i - offset;
    }

    //-----------------------------------------------
    // Nmtoken parsing (no XML namespaces support)
    //-----------------------------------------------
    /// <summary>
    /// Attempts to parse the input string as an Nmtoken (see the XML spec production [7]) without taking 
    /// into account the XML Namespaces spec. What it means is that the ':' character is allowed at any 
    /// position and any number of times in the token.
    /// Quits parsing when an invalid Nmtoken char is reached or the end of string is reached.
    /// Returns the number of valid Nmtoken chars that were parsed.
    /// </summary>
    internal static unsafe int ParseNmtokenNoNamespaces(string s, int offset)
    {
      // Keep parsing until the end of string or an invalid Name character is reached
      int i = offset;
      while (i < s.Length)
      {
        if ((xmlCharType.charProperties[s[i]] & XmlCharType.fNCNameSC) != 0 || s[i] == ':')
        { // if (xmlCharType.IsNameSingleChar(s[i])) {
          i++;
        }
        else
        {
          break;
        }
      }

      return i - offset;
    }

    /// <summary>
    /// Attempts to parse the input string as a Name without taking into account the XML Namespaces spec.
    /// What it means is that the ':' character does not delimiter prefix and local name, but it is a regular
    /// name character, which is allowed to appear at any position and any number of times in the name.
    /// Quits parsing when an invalid Name char is reached or the end of string is reached.
    /// Returns the number of valid Name chars that were parsed.
    /// </summary>
    internal static unsafe int ParseNameNoNamespaces(string s, int offset)
    {
      // Quit if the first character is not a valid NCName starting character
      int i = offset;
      if (i < s.Length)
      {
        if ((xmlCharType.charProperties[s[i]] & XmlCharType.fNCStartNameSC) != 0 || s[i] == ':')
        { // xmlCharType.IsStartNCNameSingleChar(s[i])) {
          i++;
        }
        else
        {
          return 0; // no valid StartNCName char
        }

        // Keep parsing until the end of string or an invalid NCName character is reached
        while (i < s.Length)
        {
          if ((xmlCharType.charProperties[s[i]] & XmlCharType.fNCNameSC) != 0 || s[i] == ':')
          { // if (xmlCharType.IsNCNameSingleChar(s[i]))
            i++;
          }
          else
          {
            break;
          }
        }
      }

      return i - offset;
    }

    // helper methods
    internal static bool IsNameNoNamespaces(string s)
    {
      int endPos = ParseNameNoNamespaces(s, 0);
      return endPos > 0 && endPos == s.Length;
    }

    //-----------------------------------------------
    // NCName parsing
    //-----------------------------------------------

    /// <summary>
    /// Attempts to parse the input string as an NCName (see the XML Namespace spec).
    /// Quits parsing when an invalid NCName char is reached or the end of string is reached.
    /// Returns the number of valid NCName chars that were parsed.
    /// </summary>
    internal static unsafe int ParseNCName(string s, int offset)
    {
      // Quit if the first character is not a valid NCName starting character
      int i = offset;
      if (i < s.Length)
      {
        if ((xmlCharType.charProperties[s[i]] & XmlCharType.fNCStartNameSC) != 0)
        { // xmlCharType.IsStartNCNameSingleChar(s[i])) {
          i++;
        }
        else
        {
          return 0; // no valid StartNCName char
        }

        // Keep parsing until the end of string or an invalid NCName character is reached
        while (i < s.Length)
        {
          if ((xmlCharType.charProperties[s[i]] & XmlCharType.fNCNameSC) != 0)
          { // if (xmlCharType.IsNCNameSingleChar(s[i]))
            i++;
          }
          else
          {
            break;
          }
        }
      }

      return i - offset;
    }

    internal static int ParseNCName(string s)
    {
      return ParseNCName(s, 0);
    }

    /// <summary>
    /// Attempts to parse the input string as a QName (see the XML Namespace spec).
    /// Quits parsing when an invalid QName char is reached or the end of string is reached.
    /// Returns the number of valid QName chars that were parsed.
    /// Sets colonOffset to the offset of a colon character if it exists, or 0 otherwise.
    /// </summary>
    internal static int ParseQName(string s, int offset, out int colonOffset)
    {
      int len, lenLocal;

      // Assume no colon
      colonOffset = 0;

      // Parse NCName (may be prefix, may be local name)
      len = ParseNCName(s, offset);
      if (len != 0)
      {

        // Non-empty NCName, so look for colon if there are any characters left
        offset += len;
        if (offset < s.Length && s[offset] == ':')
        {

          // First NCName was prefix, so look for local name part
          lenLocal = ParseNCName(s, offset + 1);
          if (lenLocal != 0)
          {
            // Local name part found, so increase total QName length (add 1 for colon)
            colonOffset = offset;
            len += lenLocal + 1;
          }
        }
      }

      return len;
    }

    /// <summary>
    /// Calls parseQName and throws exception if the resulting name is not a valid QName.
    /// Returns the prefix and local name parts.
    /// </summary>
    internal static void ParseQNameThrow(string s, out string prefix, out string localName)
    {
      int colonOffset;
      int len = ParseQName(s, 0, out colonOffset);

      if (len == 0 || len != s.Length)
      {
        // If the string is not a valid QName, then throw
        ThrowInvalidName(s, 0, len);
      }

      if (colonOffset != 0)
      {
        prefix = s.Substring(0, colonOffset);
        localName = s.Substring(colonOffset + 1);
      }
      else
      {
        prefix = "";
        localName = s;
      }
    }

    /// <summary>
    /// Throws an invalid name exception.
    /// </summary>
    /// <param name="s">String that was parsed.</param>
    /// <param name="offsetStartChar">Offset in string where parsing began.</param>
    /// <param name="offsetBadChar">Offset in string where parsing failed.</param>
    internal static void ThrowInvalidName(string s, int offsetStartChar, int offsetBadChar)
    {
      // If the name is empty, throw an exception
      if (offsetStartChar >= s.Length)
        throw new XmlException(Res.Xml_EmptyName, string.Empty);

      if (xmlCharType.IsNCNameSingleChar(s[offsetBadChar]) && !XmlCharType.Instance.IsStartNCNameSingleChar(s[offsetBadChar]))
      {
        // The error character is a valid name character, but is not a valid start name character
        throw new XmlException(Res.Xml_BadStartNameChar, XmlException.BuildCharExceptionArgs(s, offsetBadChar));
      }
      else
      {
        // The error character is an invalid name character
        throw new XmlException(Res.Xml_BadNameChar, XmlException.BuildCharExceptionArgs(s, offsetBadChar));
      }
    }

    internal static Exception GetInvalidNameException(string s, int offsetStartChar, int offsetBadChar)
    {
      // If the name is empty, throw an exception
      if (offsetStartChar >= s.Length)
        return new XmlException(Res.Xml_EmptyName, string.Empty);

      if (xmlCharType.IsNCNameSingleChar(s[offsetBadChar]) && !xmlCharType.IsStartNCNameSingleChar(s[offsetBadChar]))
      {
        // The error character is a valid name character, but is not a valid start name character
        return new XmlException(Res.Xml_BadStartNameChar, XmlException.BuildCharExceptionArgs(s, offsetBadChar));
      }
      else
      {
        // The error character is an invalid name character
        return new XmlException(Res.Xml_BadNameChar, XmlException.BuildCharExceptionArgs(s, offsetBadChar));
      }
    }


    /// <summary>
    /// Split a QualifiedName into prefix and localname, w/o any checking.
    /// (Used for XmlReader/XPathNavigator MoveTo(name) methods)
    /// </summary>
    internal static void SplitQName(string name, out string prefix, out string lname)
    {
      int colonPos = name.IndexOf(':');
      if (-1 == colonPos)
      {
        prefix = string.Empty;
        lname = name;
      }
      else if (0 == colonPos || (name.Length - 1) == colonPos)
      {
        throw new ArgumentException(Res.GetString(Res.Xml_BadNameChar, XmlException.BuildCharExceptionArgs(':', '\0')), "name");
      }
      else
      {
        prefix = name.Substring(0, colonPos);
        colonPos++; // move after colon
        lname = name.Substring(colonPos, name.Length - colonPos);
      }
    }
  }
}
