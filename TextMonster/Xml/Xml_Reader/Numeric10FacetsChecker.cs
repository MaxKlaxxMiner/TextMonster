using System;
using System.Collections;
using System.Globalization;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Numeric10FacetsChecker : FacetsChecker
  {
    decimal maxValue;
    decimal minValue;

    internal Numeric10FacetsChecker(decimal minVal, decimal maxVal)
    {
      minValue = minVal;
      maxValue = maxVal;
    }

    internal override Exception CheckValueFacets(object value, XmlSchemaDatatype datatype)
    {

      decimal decimalValue = datatype.ValueConverter.ToDecimal(value);
      return CheckValueFacets(decimalValue, datatype);
    }

    internal override Exception CheckValueFacets(decimal value, XmlSchemaDatatype datatype)
    {
      RestrictionFacets restriction = datatype.Restriction;
      RestrictionFlags flags = restriction != null ? restriction.Flags : 0;
      XmlValueConverter valueConverter = datatype.ValueConverter;

      //Check built-in facets
      if (value > maxValue || value < minValue)
      {
        return new OverflowException(Res.GetString(Res.XmlConvert_Overflow, value.ToString(CultureInfo.InvariantCulture), datatype.TypeCodeString));
      }
      //Check user-defined facets
      if (flags != 0)
      {
        if ((flags & RestrictionFlags.MaxInclusive) != 0)
        {
          if (value > valueConverter.ToDecimal(restriction.MaxInclusive))
          {
            return new XmlSchemaException(Res.Sch_MaxInclusiveConstraintFailed, string.Empty);
          }
        }

        if ((flags & RestrictionFlags.MaxExclusive) != 0)
        {
          if (value >= valueConverter.ToDecimal(restriction.MaxExclusive))
          {
            return new XmlSchemaException(Res.Sch_MaxExclusiveConstraintFailed, string.Empty);
          }
        }

        if ((flags & RestrictionFlags.MinInclusive) != 0)
        {
          if (value < valueConverter.ToDecimal(restriction.MinInclusive))
          {
            return new XmlSchemaException(Res.Sch_MinInclusiveConstraintFailed, string.Empty);
          }
        }

        if ((flags & RestrictionFlags.MinExclusive) != 0)
        {
          if (value <= valueConverter.ToDecimal(restriction.MinExclusive))
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
        return CheckTotalAndFractionDigits(value, restriction.TotalDigits, restriction.FractionDigits, ((flags & RestrictionFlags.TotalDigits) != 0), ((flags & RestrictionFlags.FractionDigits) != 0));
      }
      return null;
    }

    internal override Exception CheckValueFacets(Int64 value, XmlSchemaDatatype datatype)
    {
      decimal decimalValue = (decimal)value;
      return CheckValueFacets(decimalValue, datatype);
    }

    internal override Exception CheckValueFacets(Int32 value, XmlSchemaDatatype datatype)
    {
      decimal decimalValue = (decimal)value;
      return CheckValueFacets(decimalValue, datatype);
    }
    internal override Exception CheckValueFacets(Int16 value, XmlSchemaDatatype datatype)
    {
      decimal decimalValue = (decimal)value;
      return CheckValueFacets(decimalValue, datatype);
    }

    internal override bool MatchEnumeration(object value, ArrayList enumeration, XmlSchemaDatatype datatype)
    {
      return MatchEnumeration(datatype.ValueConverter.ToDecimal(value), enumeration, datatype.ValueConverter);
    }

    internal bool MatchEnumeration(decimal value, ArrayList enumeration, XmlValueConverter valueConverter)
    {
      for (int i = 0; i < enumeration.Count; ++i)
      {
        if (value == valueConverter.ToDecimal(enumeration[i]))
        {
          return true;
        }
      }
      return false;
    }
    internal Exception CheckTotalAndFractionDigits(decimal value, int totalDigits, int fractionDigits, bool checkTotal, bool checkFraction)
    {
      decimal maxValue = FacetsChecker.Power(10, totalDigits) - 1; //(decimal)Math.Pow(10, totalDigits) - 1 ;
      int powerCnt = 0;
      if (value < 0)
      {
        value = Decimal.Negate(value); //Need to compare maxValue allowed against the absolute value
      }
      while (Decimal.Truncate(value) != value)
      { //Till it has a fraction
        value = value * 10;
        powerCnt++;
      }

      if (checkTotal && (value > maxValue || powerCnt > totalDigits))
      {
        return new XmlSchemaException(Res.Sch_TotalDigitsConstraintFailed, string.Empty);
      }
      if (checkFraction && powerCnt > fractionDigits)
      {
        return new XmlSchemaException(Res.Sch_FractionDigitsConstraintFailed, string.Empty);
      }
      return null;
    }
  }
}
