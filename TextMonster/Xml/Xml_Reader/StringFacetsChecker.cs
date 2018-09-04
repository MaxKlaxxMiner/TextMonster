using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace TextMonster.Xml.Xml_Reader
{
  internal class StringFacetsChecker : FacetsChecker
  { //All types derived from string & anyURI
    static Regex languagePattern;

    static Regex LanguagePattern
    {
      get
      {
        if (languagePattern == null)
        {
          Regex langRegex = new Regex("^([a-zA-Z]{1,8})(-[a-zA-Z0-9]{1,8})*$", RegexOptions.None);
          Interlocked.CompareExchange(ref languagePattern, langRegex, null);
        }
        return languagePattern;
      }
    }

    internal override Exception CheckValueFacets(object value, XmlSchemaDatatype datatype)
    {
      string stringValue = datatype.ValueConverter.ToString(value);
      return CheckValueFacets(stringValue, datatype, true);
    }

    internal override Exception CheckValueFacets(string value, XmlSchemaDatatype datatype)
    {
      return CheckValueFacets(value, datatype, true);
    }

    internal Exception CheckValueFacets(string value, XmlSchemaDatatype datatype, bool verifyUri)
    {
      //Length, MinLength, MaxLength
      int length = value.Length;
      RestrictionFacets restriction = datatype.Restriction;
      RestrictionFlags flags = restriction != null ? restriction.Flags : 0;
      Exception exception;

      exception = CheckBuiltInFacets(value, datatype.TypeCode, verifyUri);
      if (exception != null) return exception;

      if (flags != 0)
      {
        if ((flags & RestrictionFlags.Length) != 0)
        {
          if (restriction.Length != length)
          {
            return new XmlSchemaException(Res.Sch_LengthConstraintFailed, string.Empty);
          }
        }
        if ((flags & RestrictionFlags.MinLength) != 0)
        {
          if (length < restriction.MinLength)
          {
            return new XmlSchemaException(Res.Sch_MinLengthConstraintFailed, string.Empty);
          }
        }
        if ((flags & RestrictionFlags.MaxLength) != 0)
        {
          if (restriction.MaxLength < length)
          {
            return new XmlSchemaException(Res.Sch_MaxLengthConstraintFailed, string.Empty);
          }
        }
        if ((flags & RestrictionFlags.Enumeration) != 0)
        {
          if (!MatchEnumeration(value, restriction.Enumeration, datatype))
          {
            return new XmlSchemaException(Res.Sch_EnumerationConstraintFailed, string.Empty);
          }
        }
      }
      return null;
    }

    internal override bool MatchEnumeration(object value, ArrayList enumeration, XmlSchemaDatatype datatype)
    {
      return MatchEnumeration(datatype.ValueConverter.ToString(value), enumeration, datatype);
    }

    private bool MatchEnumeration(string value, ArrayList enumeration, XmlSchemaDatatype datatype)
    {
      if (datatype.TypeCode == XmlTypeCode.AnyUri)
      {
        for (int i = 0; i < enumeration.Count; ++i)
        {
          if (value.Equals(((Uri)enumeration[i]).OriginalString))
          {
            return true;
          }
        }
      }
      else
      {
        for (int i = 0; i < enumeration.Count; ++i)
        {
          if (value.Equals((string)enumeration[i]))
          {
            return true;
          }
        }
      }
      return false;
    }

    private Exception CheckBuiltInFacets(string s, XmlTypeCode typeCode, bool verifyUri)
    {
      Exception exception = null;

      switch (typeCode)
      {

        case XmlTypeCode.AnyUri:
        if (verifyUri)
        {
          Uri uri;
          exception = XmlConvert.TryToUri(s, out uri);
        }
        break;

        case XmlTypeCode.NormalizedString:
        exception = XmlConvert.TryVerifyNormalizedString(s);
        break;

        case XmlTypeCode.Token:
        exception = XmlConvert.TryVerifyTOKEN(s);
        break;

        case XmlTypeCode.Language:
        if (s == null || s.Length == 0)
        {
          return new XmlSchemaException(Res.Sch_EmptyAttributeValue, string.Empty);
        }
        if (!LanguagePattern.IsMatch(s))
        {
          return new XmlSchemaException(Res.Sch_InvalidLanguageId, string.Empty);
        }
        break;

        case XmlTypeCode.NmToken:
        exception = XmlConvert.TryVerifyNMTOKEN(s);
        break;

        case XmlTypeCode.Name:
        exception = XmlConvert.TryVerifyName(s);
        break;

        case XmlTypeCode.NCName:
        case XmlTypeCode.Id:
        case XmlTypeCode.Idref:
        case XmlTypeCode.Entity:
        exception = XmlConvert.TryVerifyNCName(s);
        break;
        default:
        break;
      }
      return exception;
    }
  }
}
