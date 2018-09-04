using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class DurationFacetsChecker : FacetsChecker
  {

    internal override Exception CheckValueFacets(object value, XmlSchemaDatatype datatype)
    {
      TimeSpan timeSpanValue = (TimeSpan)datatype.ValueConverter.ChangeType(value, typeof(TimeSpan));
      return CheckValueFacets(timeSpanValue, datatype);
    }

    internal override Exception CheckValueFacets(TimeSpan value, XmlSchemaDatatype datatype)
    {
      RestrictionFacets restriction = datatype.Restriction;
      RestrictionFlags flags = restriction != null ? restriction.Flags : 0;

      if ((flags & RestrictionFlags.MaxInclusive) != 0)
      {
        if (TimeSpan.Compare(value, (TimeSpan)restriction.MaxInclusive) > 0)
        {
          return new XmlSchemaException(Res.Sch_MaxInclusiveConstraintFailed, string.Empty);
        }
      }

      if ((flags & RestrictionFlags.MaxExclusive) != 0)
      {
        if (TimeSpan.Compare(value, (TimeSpan)restriction.MaxExclusive) >= 0)
        {
          return new XmlSchemaException(Res.Sch_MaxExclusiveConstraintFailed, string.Empty);
        }
      }

      if ((flags & RestrictionFlags.MinInclusive) != 0)
      {
        if (TimeSpan.Compare(value, (TimeSpan)restriction.MinInclusive) < 0)
        {
          return new XmlSchemaException(Res.Sch_MinInclusiveConstraintFailed, string.Empty);
        }
      }

      if ((flags & RestrictionFlags.MinExclusive) != 0)
      {
        if (TimeSpan.Compare(value, (TimeSpan)restriction.MinExclusive) <= 0)
        {
          return new XmlSchemaException(Res.Sch_MinExclusiveConstraintFailed, string.Empty);
        }
      }
      if ((flags & RestrictionFlags.Enumeration) != 0)
      {
        if (!MatchEnumeration(value, restriction.Enumeration))
        {
          return new XmlSchemaException(Res.Sch_EnumerationConstraintFailed, string.Empty);
        }
      }
      return null;
    }
    internal override bool MatchEnumeration(object value, ArrayList enumeration, XmlSchemaDatatype datatype)
    {
      return MatchEnumeration((TimeSpan)value, enumeration);
    }

    private bool MatchEnumeration(TimeSpan value, ArrayList enumeration)
    {
      for (int i = 0; i < enumeration.Count; ++i)
      {
        if (TimeSpan.Compare(value, (TimeSpan)enumeration[i]) == 0)
        {
          return true;
        }
      }
      return false;
    }
  }
}
