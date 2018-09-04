using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class DateTimeFacetsChecker : FacetsChecker
  {

    internal override Exception CheckValueFacets(object value, XmlSchemaDatatype datatype)
    {
      DateTime dateTimeValue = datatype.ValueConverter.ToDateTime(value);
      return CheckValueFacets(dateTimeValue, datatype);
    }

    internal override Exception CheckValueFacets(DateTime value, XmlSchemaDatatype datatype)
    {
      RestrictionFacets restriction = datatype.Restriction;
      RestrictionFlags flags = restriction != null ? restriction.Flags : 0;

      if ((flags & RestrictionFlags.MaxInclusive) != 0)
      {
        if (datatype.Compare(value, (DateTime)restriction.MaxInclusive) > 0)
        {
          return new XmlSchemaException(Res.Sch_MaxInclusiveConstraintFailed, string.Empty);
        }
      }

      if ((flags & RestrictionFlags.MaxExclusive) != 0)
      {
        if (datatype.Compare(value, (DateTime)restriction.MaxExclusive) >= 0)
        {
          return new XmlSchemaException(Res.Sch_MaxExclusiveConstraintFailed, string.Empty);
        }
      }

      if ((flags & RestrictionFlags.MinInclusive) != 0)
      {
        if (datatype.Compare(value, (DateTime)restriction.MinInclusive) < 0)
        {
          return new XmlSchemaException(Res.Sch_MinInclusiveConstraintFailed, string.Empty);
        }
      }

      if ((flags & RestrictionFlags.MinExclusive) != 0)
      {
        if (datatype.Compare(value, (DateTime)restriction.MinExclusive) <= 0)
        {
          return new XmlSchemaException(Res.Sch_MinExclusiveConstraintFailed, string.Empty);
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

    internal override bool MatchEnumeration(object value, ArrayList enumeration, XmlSchemaDatatype datatype)
    {
      return MatchEnumeration(datatype.ValueConverter.ToDateTime(value), enumeration, datatype);
    }

    private bool MatchEnumeration(DateTime value, ArrayList enumeration, XmlSchemaDatatype datatype)
    {
      for (int i = 0; i < enumeration.Count; ++i)
      {
        if (datatype.Compare(value, (DateTime)enumeration[i]) == 0)
        {
          return true;
        }
      }
      return false;
    }
  }
}
