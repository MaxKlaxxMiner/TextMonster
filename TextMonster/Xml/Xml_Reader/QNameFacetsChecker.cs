using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class QNameFacetsChecker : FacetsChecker
  {

    internal override Exception CheckValueFacets(object value, XmlSchemaDatatype datatype)
    {
      XmlQualifiedName qualifiedNameValue = (XmlQualifiedName)datatype.ValueConverter.ChangeType(value, typeof(XmlQualifiedName));
      return CheckValueFacets(qualifiedNameValue, datatype);
    }

    internal override Exception CheckValueFacets(XmlQualifiedName value, XmlSchemaDatatype datatype)
    {
      RestrictionFacets restriction = datatype.Restriction;
      RestrictionFlags flags = restriction != null ? restriction.Flags : 0;
      if (flags != 0)
      { //If there are facets defined
        string strValue = value.ToString();
        int length = strValue.Length;
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
          if (!MatchEnumeration(value, restriction.Enumeration))
          {
            return new XmlSchemaException(Res.Sch_EnumerationConstraintFailed, string.Empty);
          }
        }
      }
      return null;
    }
    internal override bool MatchEnumeration(object value, ArrayList enumeration, XmlSchemaDatatype datatype)
    {
      return MatchEnumeration((XmlQualifiedName)datatype.ValueConverter.ChangeType(value, typeof(XmlQualifiedName)), enumeration);
    }

    private bool MatchEnumeration(XmlQualifiedName value, ArrayList enumeration)
    {
      for (int i = 0; i < enumeration.Count; ++i)
      {
        if (value.Equals((XmlQualifiedName)enumeration[i]))
        {
          return true;
        }
      }
      return false;
    }
  }
}
