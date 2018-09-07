using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class BinaryFacetsChecker : FacetsChecker
  { //hexBinary & Base64Binary

    internal override Exception CheckValueFacets(object value, XmlSchemaDatatype datatype)
    {
      byte[] byteArrayValue = (byte[])value;
      return CheckValueFacets(byteArrayValue, datatype);
    }

    internal override Exception CheckValueFacets(byte[] value, XmlSchemaDatatype datatype)
    {
      //Length, MinLength, MaxLength
      RestrictionFacets restriction = datatype.Restriction;
      int length = value.Length;
      RestrictionFlags flags = restriction != null ? restriction.Flags : 0;
      if (flags != 0)
      { //if it has facets defined
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

    private bool MatchEnumeration(byte[] value, ArrayList enumeration, XmlSchemaDatatype datatype)
    {
      for (int i = 0; i < enumeration.Count; ++i)
      {
        if (datatype.Compare(value, (byte[])enumeration[i]) == 0)
        {
          return true;
        }
      }
      return false;
    }
  }
}
