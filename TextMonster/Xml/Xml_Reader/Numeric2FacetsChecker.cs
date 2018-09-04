using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Numeric2FacetsChecker : FacetsChecker
  {

    internal override Exception CheckValueFacets(object value, XmlSchemaDatatype datatype)
    {
      double doubleValue = datatype.ValueConverter.ToDouble(value);
      return CheckValueFacets(doubleValue, datatype);
    }

    internal override Exception CheckValueFacets(double value, XmlSchemaDatatype datatype)
    {
      RestrictionFacets restriction = datatype.Restriction;
      RestrictionFlags flags = restriction != null ? restriction.Flags : 0;
      XmlValueConverter valueConverter = datatype.ValueConverter;

      if ((flags & RestrictionFlags.MaxInclusive) != 0)
      {
        if (value > valueConverter.ToDouble(restriction.MaxInclusive))
        {
          return new XmlSchemaException(Res.Sch_MaxInclusiveConstraintFailed, string.Empty);
        }
      }
      if ((flags & RestrictionFlags.MaxExclusive) != 0)
      {
        if (value >= valueConverter.ToDouble(restriction.MaxExclusive))
        {
          return new XmlSchemaException(Res.Sch_MaxExclusiveConstraintFailed, string.Empty);
        }
      }

      if ((flags & RestrictionFlags.MinInclusive) != 0)
      {
        if (value < (valueConverter.ToDouble(restriction.MinInclusive)))
        {
          return new XmlSchemaException(Res.Sch_MinInclusiveConstraintFailed, string.Empty);
        }
      }

      if ((flags & RestrictionFlags.MinExclusive) != 0)
      {
        if (value <= valueConverter.ToDouble(restriction.MinExclusive))
        {
          return new XmlSchemaException(Res.Sch_MinExclusiveConstraintFailed, string.Empty);
        }
      }
      if ((flags & RestrictionFlags.Enumeration) != 0)
      {
        if (!MatchEnumeration(value, restriction.Enumeration, valueConverter))
        {
          return new XmlSchemaException(Res.Sch_EnumerationConstraintFailed, string.Empty);
        }
      }
      return null;
    }

    internal override Exception CheckValueFacets(float value, XmlSchemaDatatype datatype)
    {
      double doubleValue = (double)value;
      return CheckValueFacets(doubleValue, datatype);
    }
    internal override bool MatchEnumeration(object value, ArrayList enumeration, XmlSchemaDatatype datatype)
    {
      return MatchEnumeration(datatype.ValueConverter.ToDouble(value), enumeration, datatype.ValueConverter);
    }
    private bool MatchEnumeration(double value, ArrayList enumeration, XmlValueConverter valueConverter)
    {
      for (int i = 0; i < enumeration.Count; ++i)
      {
        if (value == valueConverter.ToDouble(enumeration[i]))
        {
          return true;
        }
      }
      return false;
    }
  }
}
