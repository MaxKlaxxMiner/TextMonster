using System.Text;

namespace TextMonster.Xml.XmlReader
{
  /// <include file='doc\XmlSchemaFacet.uex' path='docs/doc[@for="XmlSchemaFacet"]/*' />
  internal abstract class FacetsChecker
  {

    private struct FacetsCompiler
    {
      DatatypeImplementation datatype;
      RestrictionFacets derivedRestriction;

      RestrictionFlags baseFlags;
      RestrictionFlags baseFixedFlags;
      RestrictionFlags validRestrictionFlags;

      //Helpers
      XmlSchemaDatatype nonNegativeInt;
      XmlSchemaDatatype builtInType;
      XmlTypeCode builtInEnum;

      bool firstPattern;
      StringBuilder regStr;
      XmlSchemaPatternFacet pattern_facet;

      public FacetsCompiler(DatatypeImplementation baseDatatype, RestrictionFacets restriction)
      {
        firstPattern = true;
        regStr = null;
        pattern_facet = null;
        datatype = baseDatatype;
        derivedRestriction = restriction;
        baseFlags = datatype.Restriction != null ? datatype.Restriction.Flags : 0;
        baseFixedFlags = datatype.Restriction != null ? datatype.Restriction.FixedFlags : 0;
        validRestrictionFlags = datatype.ValidRestrictionFlags;
        nonNegativeInt = DatatypeImplementation.GetSimpleTypeFromTypeCode(XmlTypeCode.NonNegativeInteger).Datatype;
        builtInEnum = !(datatype is Datatype_union || datatype is Datatype_List) ? datatype.TypeCode : 0;
        builtInType = (int)builtInEnum > 0 ? DatatypeImplementation.GetSimpleTypeFromTypeCode(builtInEnum).Datatype : datatype;
      }

      internal void CompileLengthFacet(XmlSchemaFacet facet)
      {
        CheckProhibitedFlag(facet, RestrictionFlags.Length, Res.Sch_LengthFacetProhibited);
        CheckDupFlag(facet, RestrictionFlags.Length, Res.Sch_DupLengthFacet);
        derivedRestriction.Length = XmlBaseConverter.DecimalToInt32((decimal)ParseFacetValue(nonNegativeInt, facet, Res.Sch_LengthFacetInvalid, null, null));

        if ((baseFixedFlags & RestrictionFlags.Length) != 0)
        {
          if (!datatype.IsEqual(datatype.Restriction.Length, derivedRestriction.Length))
          {
            throw new XmlSchemaException(Res.Sch_FacetBaseFixed, facet);
          }
        }
        if ((baseFlags & RestrictionFlags.Length) != 0)
        {
          if (datatype.Restriction.Length < derivedRestriction.Length)
          {
            throw new XmlSchemaException(Res.Sch_LengthGtBaseLength, facet);
          }
        }
        // If the base has the MinLength facet, check that our derived length is not violating it
        if ((baseFlags & RestrictionFlags.MinLength) != 0)
        {
          if (datatype.Restriction.MinLength > derivedRestriction.Length)
          {
            throw new XmlSchemaException(Res.Sch_MaxMinLengthBaseLength, facet);
          }
        }
        // If the base has the MaxLength facet, check that our derived length is not violating it
        if ((baseFlags & RestrictionFlags.MaxLength) != 0)
        {
          if (datatype.Restriction.MaxLength < derivedRestriction.Length)
          {
            throw new XmlSchemaException(Res.Sch_MaxMinLengthBaseLength, facet);
          }
        }
        SetFlag(facet, RestrictionFlags.Length);
      }

      internal void CompileMinLengthFacet(XmlSchemaFacet facet)
      {
        CheckProhibitedFlag(facet, RestrictionFlags.MinLength, Res.Sch_MinLengthFacetProhibited);
        CheckDupFlag(facet, RestrictionFlags.MinLength, Res.Sch_DupMinLengthFacet);
        derivedRestriction.MinLength = XmlBaseConverter.DecimalToInt32((decimal)ParseFacetValue(nonNegativeInt, facet, Res.Sch_MinLengthFacetInvalid, null, null));

        if ((baseFixedFlags & RestrictionFlags.MinLength) != 0)
        {
          if (!datatype.IsEqual(datatype.Restriction.MinLength, derivedRestriction.MinLength))
          {
            throw new XmlSchemaException(Res.Sch_FacetBaseFixed, facet);
          }
        }
        if ((baseFlags & RestrictionFlags.MinLength) != 0)
        {
          if (datatype.Restriction.MinLength > derivedRestriction.MinLength)
          {
            throw new XmlSchemaException(Res.Sch_MinLengthGtBaseMinLength, facet);
          }
        }
        if ((baseFlags & RestrictionFlags.Length) != 0)
        {
          if (datatype.Restriction.Length < derivedRestriction.MinLength)
          {
            throw new XmlSchemaException(Res.Sch_MaxMinLengthBaseLength, facet);
          }
        }
        SetFlag(facet, RestrictionFlags.MinLength);
      }

      internal void CompileMaxLengthFacet(XmlSchemaFacet facet)
      {
        CheckProhibitedFlag(facet, RestrictionFlags.MaxLength, Res.Sch_MaxLengthFacetProhibited);
        CheckDupFlag(facet, RestrictionFlags.MaxLength, Res.Sch_DupMaxLengthFacet);
        derivedRestriction.MaxLength = XmlBaseConverter.DecimalToInt32((decimal)ParseFacetValue(nonNegativeInt, facet, Res.Sch_MaxLengthFacetInvalid, null, null));

        if ((baseFixedFlags & RestrictionFlags.MaxLength) != 0)
        {
          if (!datatype.IsEqual(datatype.Restriction.MaxLength, derivedRestriction.MaxLength))
          {
            throw new XmlSchemaException(Res.Sch_FacetBaseFixed, facet);
          }
        }
        if ((baseFlags & RestrictionFlags.MaxLength) != 0)
        {
          if (datatype.Restriction.MaxLength < derivedRestriction.MaxLength)
          {
            throw new XmlSchemaException(Res.Sch_MaxLengthGtBaseMaxLength, facet);
          }
        }
        if ((baseFlags & RestrictionFlags.Length) != 0)
        {
          if (datatype.Restriction.Length > derivedRestriction.MaxLength)
          {
            throw new XmlSchemaException(Res.Sch_MaxMinLengthBaseLength, facet);
          }
        }
        SetFlag(facet, RestrictionFlags.MaxLength);
      }

      internal void CompilePatternFacet(XmlSchemaPatternFacet facet)
      {
        CheckProhibitedFlag(facet, RestrictionFlags.Pattern, Res.Sch_PatternFacetProhibited);
        if (firstPattern == true)
        {
          regStr = new StringBuilder();
          regStr.Append("(");
          regStr.Append(facet.Value);
          pattern_facet = facet;
          firstPattern = false;
        }
        else
        {
          regStr.Append(")|(");
          regStr.Append(facet.Value);
        }
        SetFlag(facet, RestrictionFlags.Pattern);
      }

      internal void CompileEnumerationFacet(XmlSchemaFacet facet, IXmlNamespaceResolver nsmgr, XmlNameTable nameTable)
      {
        CheckProhibitedFlag(facet, RestrictionFlags.Enumeration, Res.Sch_EnumerationFacetProhibited);
        if (derivedRestriction.Enumeration == null)
        {
          derivedRestriction.Enumeration = new ArrayList();
        }
        derivedRestriction.Enumeration.Add(ParseFacetValue(datatype, facet, Res.Sch_EnumerationFacetInvalid, nsmgr, nameTable));
        SetFlag(facet, RestrictionFlags.Enumeration);
      }

      internal void CompileWhitespaceFacet(XmlSchemaFacet facet)
      {
        CheckProhibitedFlag(facet, RestrictionFlags.WhiteSpace, Res.Sch_WhiteSpaceFacetProhibited);
        CheckDupFlag(facet, RestrictionFlags.WhiteSpace, Res.Sch_DupWhiteSpaceFacet);
        if (facet.Value == "preserve")
        {
          derivedRestriction.WhiteSpace = XmlSchemaWhiteSpace.Preserve;
        }
        else if (facet.Value == "replace")
        {
          derivedRestriction.WhiteSpace = XmlSchemaWhiteSpace.Replace;
        }
        else if (facet.Value == "collapse")
        {
          derivedRestriction.WhiteSpace = XmlSchemaWhiteSpace.Collapse;
        }
        else
        {
          throw new XmlSchemaException(Res.Sch_InvalidWhiteSpace, facet.Value, facet);
        }
        if ((baseFixedFlags & RestrictionFlags.WhiteSpace) != 0)
        {
          if (!datatype.IsEqual(datatype.Restriction.WhiteSpace, derivedRestriction.WhiteSpace))
          {
            throw new XmlSchemaException(Res.Sch_FacetBaseFixed, facet);
          }
        }
        //Check base and derived whitespace facets
        XmlSchemaWhiteSpace baseWhitespace;
        if ((baseFlags & RestrictionFlags.WhiteSpace) != 0)
        {
          baseWhitespace = datatype.Restriction.WhiteSpace;
        }
        else
        {
          baseWhitespace = datatype.BuiltInWhitespaceFacet;
        }
        if (baseWhitespace == XmlSchemaWhiteSpace.Collapse &&
            (derivedRestriction.WhiteSpace == XmlSchemaWhiteSpace.Replace || derivedRestriction.WhiteSpace == XmlSchemaWhiteSpace.Preserve)
        )
        {
          throw new XmlSchemaException(Res.Sch_WhiteSpaceRestriction1, facet);
        }
        if (baseWhitespace == XmlSchemaWhiteSpace.Replace &&
            derivedRestriction.WhiteSpace == XmlSchemaWhiteSpace.Preserve
        )
        {
          throw new XmlSchemaException(Res.Sch_WhiteSpaceRestriction2, facet);
        }
        SetFlag(facet, RestrictionFlags.WhiteSpace);
      }

      internal void CompileMaxInclusiveFacet(XmlSchemaFacet facet)
      {
        CheckProhibitedFlag(facet, RestrictionFlags.MaxInclusive, Res.Sch_MaxInclusiveFacetProhibited);
        CheckDupFlag(facet, RestrictionFlags.MaxInclusive, Res.Sch_DupMaxInclusiveFacet);
        derivedRestriction.MaxInclusive = ParseFacetValue(builtInType, facet, Res.Sch_MaxInclusiveFacetInvalid, null, null);

        if ((baseFixedFlags & RestrictionFlags.MaxInclusive) != 0)
        {
          if (!datatype.IsEqual(datatype.Restriction.MaxInclusive, derivedRestriction.MaxInclusive))
          {
            throw new XmlSchemaException(Res.Sch_FacetBaseFixed, facet);
          }
        }
        CheckValue(derivedRestriction.MaxInclusive, facet);
        SetFlag(facet, RestrictionFlags.MaxInclusive);
      }

      internal void CompileMaxExclusiveFacet(XmlSchemaFacet facet)
      {
        CheckProhibitedFlag(facet, RestrictionFlags.MaxExclusive, Res.Sch_MaxExclusiveFacetProhibited);
        CheckDupFlag(facet, RestrictionFlags.MaxExclusive, Res.Sch_DupMaxExclusiveFacet);
        derivedRestriction.MaxExclusive = ParseFacetValue(builtInType, facet, Res.Sch_MaxExclusiveFacetInvalid, null, null);

        if ((baseFixedFlags & RestrictionFlags.MaxExclusive) != 0)
        {
          if (!datatype.IsEqual(datatype.Restriction.MaxExclusive, derivedRestriction.MaxExclusive))
          {
            throw new XmlSchemaException(Res.Sch_FacetBaseFixed, facet);
          }
        }
        CheckValue(derivedRestriction.MaxExclusive, facet);
        SetFlag(facet, RestrictionFlags.MaxExclusive);
      }

      internal void CompileMinInclusiveFacet(XmlSchemaFacet facet)
      {
        CheckProhibitedFlag(facet, RestrictionFlags.MinInclusive, Res.Sch_MinInclusiveFacetProhibited);
        CheckDupFlag(facet, RestrictionFlags.MinInclusive, Res.Sch_DupMinInclusiveFacet);
        derivedRestriction.MinInclusive = ParseFacetValue(builtInType, facet, Res.Sch_MinInclusiveFacetInvalid, null, null);

        if ((baseFixedFlags & RestrictionFlags.MinInclusive) != 0)
        {
          if (!datatype.IsEqual(datatype.Restriction.MinInclusive, derivedRestriction.MinInclusive))
          {
            throw new XmlSchemaException(Res.Sch_FacetBaseFixed, facet);
          }
        }
        CheckValue(derivedRestriction.MinInclusive, facet);
        SetFlag(facet, RestrictionFlags.MinInclusive);
      }

      internal void CompileMinExclusiveFacet(XmlSchemaFacet facet)
      {
        CheckProhibitedFlag(facet, RestrictionFlags.MinExclusive, Res.Sch_MinExclusiveFacetProhibited);
        CheckDupFlag(facet, RestrictionFlags.MinExclusive, Res.Sch_DupMinExclusiveFacet);
        derivedRestriction.MinExclusive = ParseFacetValue(builtInType, facet, Res.Sch_MinExclusiveFacetInvalid, null, null);

        if ((baseFixedFlags & RestrictionFlags.MinExclusive) != 0)
        {
          if (!datatype.IsEqual(datatype.Restriction.MinExclusive, derivedRestriction.MinExclusive))
          {
            throw new XmlSchemaException(Res.Sch_FacetBaseFixed, facet);
          }
        }
        CheckValue(derivedRestriction.MinExclusive, facet);
        SetFlag(facet, RestrictionFlags.MinExclusive);
      }

      internal void CompileTotalDigitsFacet(XmlSchemaFacet facet)
      {
        CheckProhibitedFlag(facet, RestrictionFlags.TotalDigits, Res.Sch_TotalDigitsFacetProhibited);
        CheckDupFlag(facet, RestrictionFlags.TotalDigits, Res.Sch_DupTotalDigitsFacet);
        XmlSchemaDatatype positiveInt = DatatypeImplementation.GetSimpleTypeFromTypeCode(XmlTypeCode.PositiveInteger).Datatype;
        derivedRestriction.TotalDigits = XmlBaseConverter.DecimalToInt32((decimal)ParseFacetValue(positiveInt, facet, Res.Sch_TotalDigitsFacetInvalid, null, null));

        if ((baseFixedFlags & RestrictionFlags.TotalDigits) != 0)
        {
          if (!datatype.IsEqual(datatype.Restriction.TotalDigits, derivedRestriction.TotalDigits))
          {
            throw new XmlSchemaException(Res.Sch_FacetBaseFixed, facet);
          }
        }
        if ((baseFlags & RestrictionFlags.TotalDigits) != 0)
        {
          if (derivedRestriction.TotalDigits > datatype.Restriction.TotalDigits)
          {
            throw new XmlSchemaException(Res.Sch_TotalDigitsMismatch, string.Empty);
          }
        }
        SetFlag(facet, RestrictionFlags.TotalDigits);
      }

      internal void CompileFractionDigitsFacet(XmlSchemaFacet facet)
      {
        CheckProhibitedFlag(facet, RestrictionFlags.FractionDigits, Res.Sch_FractionDigitsFacetProhibited);
        CheckDupFlag(facet, RestrictionFlags.FractionDigits, Res.Sch_DupFractionDigitsFacet);
        derivedRestriction.FractionDigits = XmlBaseConverter.DecimalToInt32((decimal)ParseFacetValue(nonNegativeInt, facet, Res.Sch_FractionDigitsFacetInvalid, null, null));

        if ((derivedRestriction.FractionDigits != 0) && (datatype.TypeCode != XmlTypeCode.Decimal))
        {
          throw new XmlSchemaException(Res.Sch_FractionDigitsFacetInvalid, Res.GetString(Res.Sch_FractionDigitsNotOnDecimal), facet);
        }
        if ((baseFlags & RestrictionFlags.FractionDigits) != 0)
        {
          if (derivedRestriction.FractionDigits > datatype.Restriction.FractionDigits)
          {
            throw new XmlSchemaException(Res.Sch_TotalDigitsMismatch, string.Empty);
          }
        }
        SetFlag(facet, RestrictionFlags.FractionDigits);
      }

      internal void FinishFacetCompile()
      {
        //Additional check for pattern facet
        //If facet is XMLSchemaPattern, then the String built inside the loop
        //needs to be converted to a RegEx
        if (firstPattern == false)
        {
          if (derivedRestriction.Patterns == null)
          {
            derivedRestriction.Patterns = new ArrayList();
          }
          try
          {
            regStr.Append(")");
            string tempStr = regStr.ToString();
            if (tempStr.IndexOf('|') != -1)
            { // ordinal compare
              regStr.Insert(0, "(");
              regStr.Append(")");
            }
            derivedRestriction.Patterns.Add(new Regex(Preprocess(regStr.ToString()), RegexOptions.None));

          }
          catch (Exception e)
          {
            throw new XmlSchemaException(Res.Sch_PatternFacetInvalid, new string[] { e.Message }, e, pattern_facet.SourceUri, pattern_facet.LineNumber, pattern_facet.LinePosition, pattern_facet);
          }
        }
      }

      private void CheckValue(object value, XmlSchemaFacet facet)
      {
        RestrictionFacets restriction = datatype.Restriction;
        switch (facet.FacetType)
        {
          case FacetType.MaxInclusive:
          if ((baseFlags & RestrictionFlags.MaxInclusive) != 0)
          { //Base facet has maxInclusive
            if (datatype.Compare(value, restriction.MaxInclusive) > 0)
            {
              throw new XmlSchemaException(Res.Sch_MaxInclusiveMismatch, string.Empty);
            }
          }
          if ((baseFlags & RestrictionFlags.MaxExclusive) != 0)
          { //Base facet has maxExclusive
            if (datatype.Compare(value, restriction.MaxExclusive) >= 0)
            {
              throw new XmlSchemaException(Res.Sch_MaxIncExlMismatch, string.Empty);
            }
          }
          break;

          case FacetType.MaxExclusive:
          if ((baseFlags & RestrictionFlags.MaxExclusive) != 0)
          { //Base facet has maxExclusive
            if (datatype.Compare(value, restriction.MaxExclusive) > 0)
            {
              throw new XmlSchemaException(Res.Sch_MaxExclusiveMismatch, string.Empty);
            }
          }
          if ((baseFlags & RestrictionFlags.MaxInclusive) != 0)
          { //Base facet has maxInclusive
            if (datatype.Compare(value, restriction.MaxInclusive) > 0)
            {
              throw new XmlSchemaException(Res.Sch_MaxExlIncMismatch, string.Empty);
            }
          }
          break;

          case FacetType.MinInclusive:
          if ((baseFlags & RestrictionFlags.MinInclusive) != 0)
          { //Base facet has minInclusive
            if (datatype.Compare(value, restriction.MinInclusive) < 0)
            {
              throw new XmlSchemaException(Res.Sch_MinInclusiveMismatch, string.Empty);
            }
          }
          if ((baseFlags & RestrictionFlags.MinExclusive) != 0)
          { //Base facet has minExclusive
            if (datatype.Compare(value, restriction.MinExclusive) < 0)
            {
              throw new XmlSchemaException(Res.Sch_MinIncExlMismatch, string.Empty);
            }
          }
          if ((baseFlags & RestrictionFlags.MaxExclusive) != 0)
          { //Base facet has maxExclusive
            if (datatype.Compare(value, restriction.MaxExclusive) >= 0)
            {
              throw new XmlSchemaException(Res.Sch_MinIncMaxExlMismatch, string.Empty);
            }
          }
          break;

          case FacetType.MinExclusive:
          if ((baseFlags & RestrictionFlags.MinExclusive) != 0)
          { //Base facet has minExclusive
            if (datatype.Compare(value, restriction.MinExclusive) < 0)
            {
              throw new XmlSchemaException(Res.Sch_MinExclusiveMismatch, string.Empty);
            }
          }
          if ((baseFlags & RestrictionFlags.MinInclusive) != 0)
          { //Base facet has minInclusive
            if (datatype.Compare(value, restriction.MinInclusive) < 0)
            {
              throw new XmlSchemaException(Res.Sch_MinExlIncMismatch, string.Empty);
            }
          }
          if ((baseFlags & RestrictionFlags.MaxExclusive) != 0)
          { //Base facet has maxExclusive
            if (datatype.Compare(value, restriction.MaxExclusive) >= 0)
            {
              throw new XmlSchemaException(Res.Sch_MinExlMaxExlMismatch, string.Empty);
            }
          }
          break;

          default:
          Debug.Assert(false);
          break;
        }
      }

      internal void CompileFacetCombinations()
      {
        RestrictionFacets baseRestriction = datatype.Restriction;
        //They are not allowed on the same type but allowed on derived types.
        if (
            (derivedRestriction.Flags & RestrictionFlags.MaxInclusive) != 0 &&
            (derivedRestriction.Flags & RestrictionFlags.MaxExclusive) != 0
        )
        {
          throw new XmlSchemaException(Res.Sch_MaxInclusiveExclusive, string.Empty);
        }
        if (
            (derivedRestriction.Flags & RestrictionFlags.MinInclusive) != 0 &&
            (derivedRestriction.Flags & RestrictionFlags.MinExclusive) != 0
        )
        {
          throw new XmlSchemaException(Res.Sch_MinInclusiveExclusive, string.Empty);
        }
        if (
            (derivedRestriction.Flags & RestrictionFlags.Length) != 0 &&
            (derivedRestriction.Flags & (RestrictionFlags.MinLength | RestrictionFlags.MaxLength)) != 0
        )
        {
          throw new XmlSchemaException(Res.Sch_LengthAndMinMax, string.Empty);
        }

        CopyFacetsFromBaseType();

        // Check combinations
        if (
            (derivedRestriction.Flags & RestrictionFlags.MinLength) != 0 &&
            (derivedRestriction.Flags & RestrictionFlags.MaxLength) != 0
        )
        {
          if (derivedRestriction.MinLength > derivedRestriction.MaxLength)
          {
            throw new XmlSchemaException(Res.Sch_MinLengthGtMaxLength, string.Empty);
          }
        }

        //
        if (
            (derivedRestriction.Flags & RestrictionFlags.MinInclusive) != 0 &&
            (derivedRestriction.Flags & RestrictionFlags.MaxInclusive) != 0
        )
        {
          if (datatype.Compare(derivedRestriction.MinInclusive, derivedRestriction.MaxInclusive) > 0)
          {
            throw new XmlSchemaException(Res.Sch_MinInclusiveGtMaxInclusive, string.Empty);
          }
        }
        if (
            (derivedRestriction.Flags & RestrictionFlags.MinInclusive) != 0 &&
            (derivedRestriction.Flags & RestrictionFlags.MaxExclusive) != 0
        )
        {
          if (datatype.Compare(derivedRestriction.MinInclusive, derivedRestriction.MaxExclusive) > 0)
          {
            throw new XmlSchemaException(Res.Sch_MinInclusiveGtMaxExclusive, string.Empty);
          }
        }
        if (
            (derivedRestriction.Flags & RestrictionFlags.MinExclusive) != 0 &&
            (derivedRestriction.Flags & RestrictionFlags.MaxExclusive) != 0
        )
        {
          if (datatype.Compare(derivedRestriction.MinExclusive, derivedRestriction.MaxExclusive) > 0)
          {
            throw new XmlSchemaException(Res.Sch_MinExclusiveGtMaxExclusive, string.Empty);
          }
        }
        if (
            (derivedRestriction.Flags & RestrictionFlags.MinExclusive) != 0 &&
            (derivedRestriction.Flags & RestrictionFlags.MaxInclusive) != 0
        )
        {
          if (datatype.Compare(derivedRestriction.MinExclusive, derivedRestriction.MaxInclusive) > 0)
          {
            throw new XmlSchemaException(Res.Sch_MinExclusiveGtMaxInclusive, string.Empty);
          }
        }
        if ((derivedRestriction.Flags & (RestrictionFlags.TotalDigits | RestrictionFlags.FractionDigits)) == (RestrictionFlags.TotalDigits | RestrictionFlags.FractionDigits))
        {
          if (derivedRestriction.FractionDigits > derivedRestriction.TotalDigits)
          {
            throw new XmlSchemaException(Res.Sch_FractionDigitsGtTotalDigits, string.Empty);
          }
        }
      }

      private void CopyFacetsFromBaseType()
      {
        RestrictionFacets baseRestriction = datatype.Restriction;
        // Copy additional facets from the base type
        if (
            (derivedRestriction.Flags & RestrictionFlags.Length) == 0 &&
            (baseFlags & RestrictionFlags.Length) != 0
        )
        {
          derivedRestriction.Length = baseRestriction.Length;
          SetFlag(RestrictionFlags.Length);
        }
        if (
            (derivedRestriction.Flags & RestrictionFlags.MinLength) == 0 &&
            (baseFlags & RestrictionFlags.MinLength) != 0
        )
        {
          derivedRestriction.MinLength = baseRestriction.MinLength;
          SetFlag(RestrictionFlags.MinLength);
        }
        if (
            (derivedRestriction.Flags & RestrictionFlags.MaxLength) == 0 &&
            (baseFlags & RestrictionFlags.MaxLength) != 0
        )
        {
          derivedRestriction.MaxLength = baseRestriction.MaxLength;
          SetFlag(RestrictionFlags.MaxLength);
        }
        if ((baseFlags & RestrictionFlags.Pattern) != 0)
        {
          if (derivedRestriction.Patterns == null)
          {
            derivedRestriction.Patterns = baseRestriction.Patterns;
          }
          else
          {
            derivedRestriction.Patterns.AddRange(baseRestriction.Patterns);
          }
          SetFlag(RestrictionFlags.Pattern);
        }

        if ((baseFlags & RestrictionFlags.Enumeration) != 0)
        {
          if (derivedRestriction.Enumeration == null)
          {
            derivedRestriction.Enumeration = baseRestriction.Enumeration;
          }
          SetFlag(RestrictionFlags.Enumeration);
        }

        if (
            (derivedRestriction.Flags & RestrictionFlags.WhiteSpace) == 0 &&
            (baseFlags & RestrictionFlags.WhiteSpace) != 0
        )
        {
          derivedRestriction.WhiteSpace = baseRestriction.WhiteSpace;
          SetFlag(RestrictionFlags.WhiteSpace);
        }
        if (
            (derivedRestriction.Flags & RestrictionFlags.MaxInclusive) == 0 &&
            (baseFlags & RestrictionFlags.MaxInclusive) != 0
        )
        {
          derivedRestriction.MaxInclusive = baseRestriction.MaxInclusive;
          SetFlag(RestrictionFlags.MaxInclusive);
        }
        if (
            (derivedRestriction.Flags & RestrictionFlags.MaxExclusive) == 0 &&
            (baseFlags & RestrictionFlags.MaxExclusive) != 0
        )
        {
          derivedRestriction.MaxExclusive = baseRestriction.MaxExclusive;
          SetFlag(RestrictionFlags.MaxExclusive);
        }
        if (
            (derivedRestriction.Flags & RestrictionFlags.MinInclusive) == 0 &&
            (baseFlags & RestrictionFlags.MinInclusive) != 0
        )
        {
          derivedRestriction.MinInclusive = baseRestriction.MinInclusive;
          SetFlag(RestrictionFlags.MinInclusive);
        }
        if (
            (derivedRestriction.Flags & RestrictionFlags.MinExclusive) == 0 &&
            (baseFlags & RestrictionFlags.MinExclusive) != 0
        )
        {
          derivedRestriction.MinExclusive = baseRestriction.MinExclusive;
          SetFlag(RestrictionFlags.MinExclusive);
        }
        if (
            (derivedRestriction.Flags & RestrictionFlags.TotalDigits) == 0 &&
            (baseFlags & RestrictionFlags.TotalDigits) != 0
        )
        {
          derivedRestriction.TotalDigits = baseRestriction.TotalDigits;
          SetFlag(RestrictionFlags.TotalDigits);
        }
        if (
            (derivedRestriction.Flags & RestrictionFlags.FractionDigits) == 0 &&
            (baseFlags & RestrictionFlags.FractionDigits) != 0
        )
        {
          derivedRestriction.FractionDigits = baseRestriction.FractionDigits;
          SetFlag(RestrictionFlags.FractionDigits);
        }
      }

      private object ParseFacetValue(XmlSchemaDatatype datatype, XmlSchemaFacet facet, string code, IXmlNamespaceResolver nsmgr, XmlNameTable nameTable)
      {
        object typedValue;
        Exception ex = datatype.TryParseValue(facet.Value, nameTable, nsmgr, out typedValue);
        if (ex == null)
        {
          return typedValue;
        }
        else
        {
          throw new XmlSchemaException(code, new string[] { ex.Message }, ex, facet.SourceUri, facet.LineNumber, facet.LinePosition, facet);
        }
      }

      private struct Map
      {
        internal Map(char m, string r)
        {
          match = m;
          replacement = r;
        }
        internal char match;
        internal string replacement;
      };

      private static readonly Map[] c_map = {
            new Map('c', "\\p{_xmlC}"),
            new Map('C', "\\P{_xmlC}"),
            new Map('d', "\\p{_xmlD}"),
            new Map('D', "\\P{_xmlD}"),
            new Map('i', "\\p{_xmlI}"),
            new Map('I', "\\P{_xmlI}"),
            new Map('w', "\\p{_xmlW}"),
            new Map('W', "\\P{_xmlW}"),
        };
      private static string Preprocess(string pattern)
      {
        StringBuilder bufBld = new StringBuilder();
        bufBld.Append("^");

        char[] source = pattern.ToCharArray();
        int length = pattern.Length;
        int copyPosition = 0;
        for (int position = 0; position < length - 2; position++)
        {
          if (source[position] == '\\')
          {
            if (source[position + 1] == '\\')
            {
              position++; // skip it
            }
            else
            {
              char ch = source[position + 1];
              for (int i = 0; i < c_map.Length; i++)
              {
                if (c_map[i].match == ch)
                {
                  if (copyPosition < position)
                  {
                    bufBld.Append(source, copyPosition, position - copyPosition);
                  }
                  bufBld.Append(c_map[i].replacement);
                  position++;
                  copyPosition = position + 1;
                  break;
                }
              }
            }
          }
        }
        if (copyPosition < length)
        {
          bufBld.Append(source, copyPosition, length - copyPosition);
        }

        bufBld.Append("$");
        return bufBld.ToString();
      }

      private void CheckProhibitedFlag(XmlSchemaFacet facet, RestrictionFlags flag, string errorCode)
      {
        if ((validRestrictionFlags & flag) == 0)
        {
          throw new XmlSchemaException(errorCode, datatype.TypeCodeString, facet);
        }
      }

      private void CheckDupFlag(XmlSchemaFacet facet, RestrictionFlags flag, string errorCode)
      {
        if ((derivedRestriction.Flags & flag) != 0)
        {
          throw new XmlSchemaException(errorCode, facet);
        }
      }

      private void SetFlag(XmlSchemaFacet facet, RestrictionFlags flag)
      {
        derivedRestriction.Flags |= flag;
        if (facet.IsFixed)
        {
          derivedRestriction.FixedFlags |= flag;
        }
      }

      private void SetFlag(RestrictionFlags flag)
      {
        derivedRestriction.Flags |= flag;
        if ((baseFixedFlags & flag) != 0)
        {
          derivedRestriction.FixedFlags |= flag;
        }
      }

    }

    internal virtual Exception CheckLexicalFacets(ref string parseString, XmlSchemaDatatype datatype)
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
    internal virtual Exception CheckValueFacets(byte value, XmlSchemaDatatype datatype)
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

    internal virtual bool MatchEnumeration(object value, ArrayList enumeration, XmlSchemaDatatype datatype)
    {
      return false;
    }

    //Compile-time Facet Checking
    internal virtual RestrictionFacets ConstructRestriction(DatatypeImplementation datatype, XmlSchemaObjectCollection facets, XmlNameTable nameTable)
    {
      //Datatype is the type on which this method is called
      RestrictionFacets derivedRestriction = new RestrictionFacets();
      FacetsCompiler facetCompiler = new FacetsCompiler(datatype, derivedRestriction);

      for (int i = 0; i < facets.Count; ++i)
      {
        XmlSchemaFacet facet = (XmlSchemaFacet)facets[i];
        if (facet.Value == null)
        {
          throw new XmlSchemaException(Res.Sch_InvalidFacet, facet);
        }
        IXmlNamespaceResolver nsmgr = new SchemaNamespaceManager(facet);
        switch (facet.FacetType)
        {
          case FacetType.Length:
          facetCompiler.CompileLengthFacet(facet);
          break;

          case FacetType.MinLength:
          facetCompiler.CompileMinLengthFacet(facet);
          break;

          case FacetType.MaxLength:
          facetCompiler.CompileMaxLengthFacet(facet);
          break;

          case FacetType.Pattern:
          facetCompiler.CompilePatternFacet(facet as XmlSchemaPatternFacet);
          break;

          case FacetType.Enumeration:
          facetCompiler.CompileEnumerationFacet(facet, nsmgr, nameTable);
          break;

          case FacetType.Whitespace:
          facetCompiler.CompileWhitespaceFacet(facet);
          break;

          case FacetType.MinInclusive:
          facetCompiler.CompileMinInclusiveFacet(facet);
          break;

          case FacetType.MinExclusive:
          facetCompiler.CompileMinExclusiveFacet(facet);
          break;

          case FacetType.MaxInclusive:
          facetCompiler.CompileMaxInclusiveFacet(facet);
          break;

          case FacetType.MaxExclusive:
          facetCompiler.CompileMaxExclusiveFacet(facet);
          break;

          case FacetType.TotalDigits:
          facetCompiler.CompileTotalDigitsFacet(facet);
          break;

          case FacetType.FractionDigits:
          facetCompiler.CompileFractionDigitsFacet(facet);
          break;

          default:
          throw new XmlSchemaException(Res.Sch_UnknownFacet, facet);
        }
      }
      facetCompiler.FinishFacetCompile();
      facetCompiler.CompileFacetCombinations();
      return derivedRestriction;
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
