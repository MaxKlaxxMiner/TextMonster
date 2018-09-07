using System;
using System.Text.RegularExpressions;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaFacet.uex' path='docs/doc[@for="XmlSchemaFacet"]/*' />
  internal abstract class FacetsChecker
  {
    internal Exception CheckLexicalFacets(ref string parseString, XmlSchemaDatatype datatype)
    {
      CheckWhitespaceFacets(ref parseString, datatype);
      return CheckPatternFacets(datatype.Restriction, parseString);
    }
    internal virtual Exception CheckValueFacets(object value, XmlSchemaDatatype datatype)
    {
      return null;
    }
    internal virtual Exception CheckValueFacets(decimal value, XmlSchemaDatatype datatype)
    {
      return null;
    }
    internal virtual Exception CheckValueFacets(Int64 value, XmlSchemaDatatype datatype)
    {
      return null;
    }
    internal virtual Exception CheckValueFacets(Int32 value, XmlSchemaDatatype datatype)
    {
      return null;
    }
    internal virtual Exception CheckValueFacets(Int16 value, XmlSchemaDatatype datatype)
    {
      return null;
    }

    internal virtual Exception CheckValueFacets(DateTime value, XmlSchemaDatatype datatype)
    {
      return null;
    }
    internal virtual Exception CheckValueFacets(double value, XmlSchemaDatatype datatype)
    {
      return null;
    }
    internal virtual Exception CheckValueFacets(float value, XmlSchemaDatatype datatype)
    {
      return null;
    }
    internal virtual Exception CheckValueFacets(string value, XmlSchemaDatatype datatype)
    {
      return null;
    }
    internal virtual Exception CheckValueFacets(byte[] value, XmlSchemaDatatype datatype)
    {
      return null;
    }
    internal virtual Exception CheckValueFacets(TimeSpan value, XmlSchemaDatatype datatype)
    {
      return null;
    }
    internal virtual Exception CheckValueFacets(XmlQualifiedName value, XmlSchemaDatatype datatype)
    {
      return null;
    }

    internal void CheckWhitespaceFacets(ref string s, XmlSchemaDatatype datatype)
    {
      // before parsing, check whitespace facet
      RestrictionFacets restriction = datatype.Restriction;

      switch (datatype.Variety)
      {
        case XmlSchemaDatatypeVariety.List:
        s = s.Trim();
        break;

        case XmlSchemaDatatypeVariety.Atomic:
        if (datatype.BuiltInWhitespaceFacet == XmlSchemaWhiteSpace.Collapse)
        {
          s = XmlComplianceUtil.NonCDataNormalize(s);
        }
        else if (datatype.BuiltInWhitespaceFacet == XmlSchemaWhiteSpace.Replace)
        {
          s = XmlComplianceUtil.CDataNormalize(s);
        }
        else if (restriction != null && (restriction.Flags & RestrictionFlags.WhiteSpace) != 0)
        { //Restriction has whitespace facet specified
          if (restriction.WhiteSpace == XmlSchemaWhiteSpace.Replace)
          {
            s = XmlComplianceUtil.CDataNormalize(s);
          }
          else if (restriction.WhiteSpace == XmlSchemaWhiteSpace.Collapse)
          {
            s = XmlComplianceUtil.NonCDataNormalize(s);
          }
        }
        break;

        default:
        break;

      }
    }
    internal Exception CheckPatternFacets(RestrictionFacets restriction, string value)
    {
      if (restriction != null && (restriction.Flags & RestrictionFlags.Pattern) != 0)
      {
        for (int i = 0; i < restriction.Patterns.Count; ++i)
        {
          Regex regex = (Regex)restriction.Patterns[i];
          if (!regex.IsMatch(value))
          {
            return new XmlSchemaException(Res.Sch_PatternConstraintFailed, string.Empty);
          }
        }
      }
      return null;
    }

    internal static decimal Power(int x, int y)
    {
      //Returns X raised to the power Y
      decimal returnValue = 1m;
      decimal decimalValue = (decimal)x;
      if (y > 28)
      { //CLR decimal cannot handle more than 29 digits (10 power 28.)
        return decimal.MaxValue;
      }
      for (int i = 0; i < y; i++)
      {
        returnValue = returnValue * decimalValue;
      }
      return returnValue;
    }
  }
}
