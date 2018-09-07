using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class ListFacetsChecker : FacetsChecker
  {

    internal override Exception CheckValueFacets(object value, XmlSchemaDatatype datatype)
    {
      //Check for facets allowed on lists - Length, MinLength, MaxLength
      Array values = value as Array;

      RestrictionFacets restriction = datatype.Restriction;
      RestrictionFlags flags = restriction != null ? restriction.Flags : 0;

      if ((flags & (RestrictionFlags.Length | RestrictionFlags.MinLength | RestrictionFlags.MaxLength)) != 0)
      {
        int length = values.Length;
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
      }
      if ((flags & RestrictionFlags.Enumeration) != 0)
      {
        if (!MatchEnumeration(value, restriction.Enumeration, datatype))
        {
          return new XmlSchemaException(Res.Sch_EnumerationConstraintFailed, string.Empty);
        }
      }
      return null;
    }

    internal virtual bool MatchEnumeration(object value, ArrayList enumeration, XmlSchemaDatatype datatype)
    {
      for (int i = 0; i < enumeration.Count; ++i)
      {
        if (datatype.Compare(value, enumeration[i]) == 0)
        {
          return true;
        }
      }
      return false;
    }
  }
}
