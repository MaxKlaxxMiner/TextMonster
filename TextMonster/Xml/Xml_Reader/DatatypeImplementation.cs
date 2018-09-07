using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class DatatypeImplementation : XmlSchemaDatatype
  {
    private XmlSchemaDatatypeVariety variety = XmlSchemaDatatypeVariety.Atomic;
    private RestrictionFacets restriction = null;
    private DatatypeImplementation baseType = null;
    private XmlValueConverter valueConverter;
    private XmlSchemaType parentSchemaType;

    private static Hashtable builtinTypes = new Hashtable();
    private static XmlSchemaSimpleType[] enumToTypeCode = new XmlSchemaSimpleType[(int)XmlTypeCode.DayTimeDuration + 1];
    private static XmlSchemaSimpleType anySimpleType;
    private static XmlSchemaSimpleType anyAtomicType;
    private static XmlSchemaSimpleType untypedAtomicType;
    private static XmlSchemaSimpleType yearMonthDurationType;
    private static XmlSchemaSimpleType dayTimeDurationType;
    private static volatile XmlSchemaSimpleType normalizedStringTypeV1Compat;
    private static volatile XmlSchemaSimpleType tokenTypeV1Compat;

    private const int anySimpleTypeIndex = 11;

    internal static XmlQualifiedName QnAnyType = new XmlQualifiedName("anyType", XmlReservedNs.NsXs);

    //Create facet checkers
    internal static FacetsChecker stringFacetsChecker = new StringFacetsChecker();
    internal static FacetsChecker miscFacetsChecker = new MiscFacetsChecker();
    internal static FacetsChecker numeric2FacetsChecker = new Numeric2FacetsChecker();
    internal static FacetsChecker binaryFacetsChecker = new BinaryFacetsChecker();
    internal static FacetsChecker dateTimeFacetsChecker = new DateTimeFacetsChecker();
    internal static FacetsChecker durationFacetsChecker = new DurationFacetsChecker();
    internal static FacetsChecker listFacetsChecker = new ListFacetsChecker();
    internal static FacetsChecker qnameFacetsChecker = new QNameFacetsChecker();
    internal static FacetsChecker unionFacetsChecker = new UnionFacetsChecker();

    static DatatypeImplementation()
    {
      CreateBuiltinTypes();
    }

    internal static XmlSchemaSimpleType AnySimpleType { get { return anySimpleType; } }

    // Additional built-in XQuery simple types
    internal static XmlSchemaSimpleType UntypedAtomicType { get { return untypedAtomicType; } }

    internal new static DatatypeImplementation FromXmlTokenizedType(XmlTokenizedType token)
    {
      return c_tokenizedTypes[(int)token];
    }

    internal new static DatatypeImplementation FromXmlTokenizedTypeXsd(XmlTokenizedType token)
    {
      return c_tokenizedTypesXsd[(int)token];
    }

    internal new static DatatypeImplementation FromXdrName(string name)
    {
      int i = Array.BinarySearch(c_XdrTypes, name, null);
      return i < 0 ? null : (DatatypeImplementation)c_XdrTypes[i];
    }

    private static DatatypeImplementation FromTypeName(string name)
    {
      int i = Array.BinarySearch(c_XsdTypes, name, null);
      return i < 0 ? null : (DatatypeImplementation)c_XsdTypes[i];
    }

    /// <summary>
    /// Begin the creation of an XmlSchemaSimpleType object that will be used to represent a static built-in type.
    /// Once StartBuiltinType has been called for all built-in types, FinishBuiltinType should be called in order
    /// to create links between the types.
    /// </summary>
    internal static XmlSchemaSimpleType StartBuiltinType(XmlQualifiedName qname, XmlSchemaDatatype dataType)
    {
      XmlSchemaSimpleType simpleType;

      simpleType = new XmlSchemaSimpleType();
      simpleType.SetQualifiedName(qname);
      simpleType.SetDatatype(dataType);
      simpleType.ElementDecl = new SchemaElementDecl(dataType);
      simpleType.ElementDecl.SchemaType = simpleType;

      return simpleType;
    }

    /// <summary>
    /// Finish constructing built-in types by setting up derivation and list links.
    /// </summary>
    internal static void FinishBuiltinType(XmlSchemaSimpleType derivedType, XmlSchemaSimpleType baseType)
    {
      // Create link from the derived type to the base type
      derivedType.SetBaseSchemaType(baseType);
      derivedType.SetDerivedBy(XmlSchemaDerivationMethod.Restriction);
      if (derivedType.Datatype.Variety == XmlSchemaDatatypeVariety.Atomic)
      { //Content is restriction
        XmlSchemaSimpleTypeRestriction restContent = new XmlSchemaSimpleTypeRestriction();
        restContent.BaseTypeName = baseType.QualifiedName;
        derivedType.Content = restContent;
      }

      // Create link from a list type to its member type
      if (derivedType.Datatype.Variety == XmlSchemaDatatypeVariety.List)
      {
        XmlSchemaSimpleTypeList listContent = new XmlSchemaSimpleTypeList();
        derivedType.SetDerivedBy(XmlSchemaDerivationMethod.List);
        switch (derivedType.Datatype.TypeCode)
        {
          case XmlTypeCode.NmToken:
          listContent.ItemType = listContent.BaseItemType = enumToTypeCode[(int)XmlTypeCode.NmToken];
          break;

          case XmlTypeCode.Entity:
          listContent.ItemType = listContent.BaseItemType = enumToTypeCode[(int)XmlTypeCode.Entity];
          break;

          case XmlTypeCode.Idref:
          listContent.ItemType = listContent.BaseItemType = enumToTypeCode[(int)XmlTypeCode.Idref];
          break;
        }
        derivedType.Content = listContent;
      }
    }

    internal static void CreateBuiltinTypes()
    {
      XmlQualifiedName qname;

      //Build anySimpleType
      SchemaDatatypeMap sdm = c_XsdTypes[anySimpleTypeIndex]; //anySimpleType
      qname = new XmlQualifiedName(sdm.Name, XmlReservedNs.NsXs);
      DatatypeImplementation dt = FromTypeName(qname.Name);
      anySimpleType = StartBuiltinType(qname, dt);
      dt.parentSchemaType = anySimpleType;
      builtinTypes.Add(qname, anySimpleType);

      // Start construction of each built-in Xsd type
      XmlSchemaSimpleType simpleType;
      for (int i = 0; i < c_XsdTypes.Length; i++)
      { //Create all types
        if (i == anySimpleTypeIndex)
        { //anySimpleType
          continue;
        }
        sdm = c_XsdTypes[i];

        qname = new XmlQualifiedName(sdm.Name, XmlReservedNs.NsXs);
        dt = FromTypeName(qname.Name);
        simpleType = StartBuiltinType(qname, dt);
        dt.parentSchemaType = simpleType;

        builtinTypes.Add(qname, simpleType);
        if (dt.variety == XmlSchemaDatatypeVariety.Atomic)
        {
          enumToTypeCode[(int)dt.TypeCode] = simpleType;
        }
      }

      // Finish construction of each built-in Xsd type
      for (int i = 0; i < c_XsdTypes.Length; i++)
      {
        if (i == anySimpleTypeIndex)
        { //anySimpleType
          continue;
        }
        sdm = c_XsdTypes[i];
        XmlSchemaSimpleType derivedType = (XmlSchemaSimpleType)builtinTypes[new XmlQualifiedName(sdm.Name, XmlReservedNs.NsXs)];
        XmlSchemaSimpleType baseType;

        if (sdm.ParentIndex == anySimpleTypeIndex)
        {
          FinishBuiltinType(derivedType, anySimpleType);
        }
        else
        { //derived types whose index > 0
          baseType = (XmlSchemaSimpleType)builtinTypes[new XmlQualifiedName(((SchemaDatatypeMap)(c_XsdTypes[sdm.ParentIndex])).Name, XmlReservedNs.NsXs)];
          FinishBuiltinType(derivedType, baseType);
        }
      }

      // Construct xdt:anyAtomicType type (derived from xs:anySimpleType)
      qname = new XmlQualifiedName("anyAtomicType", XmlReservedNs.NsXQueryDataType);
      anyAtomicType = StartBuiltinType(qname, c_anyAtomicType);
      c_anyAtomicType.parentSchemaType = anyAtomicType;
      FinishBuiltinType(anyAtomicType, anySimpleType);
      builtinTypes.Add(qname, anyAtomicType);
      enumToTypeCode[(int)XmlTypeCode.AnyAtomicType] = anyAtomicType;

      // Construct xdt:untypedAtomic type (derived from xdt:anyAtomicType)
      qname = new XmlQualifiedName("untypedAtomic", XmlReservedNs.NsXQueryDataType);
      untypedAtomicType = StartBuiltinType(qname, c_untypedAtomicType);
      c_untypedAtomicType.parentSchemaType = untypedAtomicType;
      FinishBuiltinType(untypedAtomicType, anyAtomicType);
      builtinTypes.Add(qname, untypedAtomicType);
      enumToTypeCode[(int)XmlTypeCode.UntypedAtomic] = untypedAtomicType;

      // Construct xdt:yearMonthDuration type (derived from xs:duration)
      qname = new XmlQualifiedName("yearMonthDuration", XmlReservedNs.NsXQueryDataType);
      yearMonthDurationType = StartBuiltinType(qname, c_yearMonthDuration);
      c_yearMonthDuration.parentSchemaType = yearMonthDurationType;
      FinishBuiltinType(yearMonthDurationType, enumToTypeCode[(int)XmlTypeCode.Duration]);
      builtinTypes.Add(qname, yearMonthDurationType);
      enumToTypeCode[(int)XmlTypeCode.YearMonthDuration] = yearMonthDurationType;

      // Construct xdt:dayTimeDuration type (derived from xs:duration)
      qname = new XmlQualifiedName("dayTimeDuration", XmlReservedNs.NsXQueryDataType);
      dayTimeDurationType = StartBuiltinType(qname, c_dayTimeDuration);
      c_dayTimeDuration.parentSchemaType = dayTimeDurationType;
      FinishBuiltinType(dayTimeDurationType, enumToTypeCode[(int)XmlTypeCode.Duration]);
      builtinTypes.Add(qname, dayTimeDurationType);
      enumToTypeCode[(int)XmlTypeCode.DayTimeDuration] = dayTimeDurationType;
    }

    internal static XmlSchemaSimpleType GetSimpleTypeFromTypeCode(XmlTypeCode typeCode)
    {
      return enumToTypeCode[(int)typeCode];
    }

    internal static XmlSchemaSimpleType GetSimpleTypeFromXsdType(XmlQualifiedName qname)
    {
      return (XmlSchemaSimpleType)builtinTypes[qname];
    }

    internal static XmlTypeCode GetPrimitiveTypeCode(XmlTypeCode typeCode)
    {
      XmlSchemaSimpleType currentType = enumToTypeCode[(int)typeCode];
      while (currentType.BaseXmlSchemaType != DatatypeImplementation.AnySimpleType)
      {
        currentType = currentType.BaseXmlSchemaType as XmlSchemaSimpleType;
      }
      return currentType.TypeCode;
    }

    internal override XmlSchemaDatatype DeriveByList(XmlSchemaType schemaType)
    {
      return DeriveByList(0, schemaType);
    }

    internal XmlSchemaDatatype DeriveByList(int minSize, XmlSchemaType schemaType)
    {
      if (variety == XmlSchemaDatatypeVariety.List)
      {
        throw new XmlSchemaException(Res.Sch_ListFromNonatomic, string.Empty);
      }
      else if (variety == XmlSchemaDatatypeVariety.Union && !((Datatype_union)this).HasAtomicMembers())
      {
        throw new XmlSchemaException(Res.Sch_ListFromNonatomic, string.Empty);
      }
      DatatypeImplementation dt = new Datatype_List(this, minSize);
      dt.variety = XmlSchemaDatatypeVariety.List;
      dt.restriction = null;
      dt.baseType = c_anySimpleType; //Base type of a union is anySimpleType
      dt.parentSchemaType = schemaType;
      return dt;
    }

    public override bool IsDerivedFrom(XmlSchemaDatatype datatype)
    {
      if (datatype == null)
      {
        return false;
      }

      //Common case - Derived by restriction
      for (DatatypeImplementation dt = this; dt != null; dt = dt.baseType)
      {
        if (dt == datatype)
        {
          return true;
        }
      }
      if (((DatatypeImplementation)datatype).baseType == null)
      { //Both are built-in types
        Type derivedType = this.GetType();
        Type baseType = datatype.GetType();
        return baseType == derivedType || derivedType.IsSubclassOf(baseType);
      }
      else if (datatype.Variety == XmlSchemaDatatypeVariety.Union && !datatype.HasLexicalFacets && !datatype.HasValueFacets && variety != XmlSchemaDatatypeVariety.Union)
      { //base type is union (not a restriction of union) and derived type is not union
        return ((Datatype_union)datatype).IsUnionBaseOf(this);
      }
      else if ((variety == XmlSchemaDatatypeVariety.Union || variety == XmlSchemaDatatypeVariety.List) && restriction == null)
      { //derived type is union (not a restriction)
        return (datatype == anySimpleType.Datatype);
      }
      return false;
    }

    internal override bool IsEqual(object o1, object o2)
    {
      //Debug.WriteLineIf(DiagnosticsSwitches.XmlSchema.TraceVerbose, string.Format("\t\tSchemaDatatype.IsEqual({0}, {1})", o1, o2));
      return Compare(o1, o2) == 0;
    }

    internal override bool IsComparable(XmlSchemaDatatype dtype)
    {
      XmlTypeCode thisCode = this.TypeCode;
      XmlTypeCode otherCode = dtype.TypeCode;

      if (thisCode == otherCode)
      { //They are both same built-in type or one is list and the other is list's itemType
        return true;
      }
      if (GetPrimitiveTypeCode(thisCode) == GetPrimitiveTypeCode(otherCode))
      {
        return true;
      }
      if (this.IsDerivedFrom(dtype) || dtype.IsDerivedFrom(this))
      { //One is union and the other is a member of the union
        return true;
      }
      return false;
    }

    internal virtual XmlValueConverter CreateValueConverter(XmlSchemaType schemaType) { return null; }

    internal override FacetsChecker FacetsChecker { get { return miscFacetsChecker; } }

    internal override XmlValueConverter ValueConverter
    {
      get
      {
        if (valueConverter == null)
        {
          valueConverter = CreateValueConverter(this.parentSchemaType);
        }
        return valueConverter;
      }
    }

    public override XmlTokenizedType TokenizedType { get { return XmlTokenizedType.None; } }

    public override Type ValueType { get { return typeof(string); } }

    public override XmlSchemaDatatypeVariety Variety { get { return variety; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.None; } }

    internal override RestrictionFacets Restriction
    {
      get
      {
        return restriction;
      }
    }
    internal override bool HasLexicalFacets
    {
      get
      {
        RestrictionFlags flags = restriction != null ? restriction.Flags : 0;
        if (flags != 0 && (flags & (RestrictionFlags.Pattern | RestrictionFlags.WhiteSpace | RestrictionFlags.TotalDigits | RestrictionFlags.FractionDigits)) != 0)
        {
          return true;
        }
        return false;
      }
    }
    internal override bool HasValueFacets
    {
      get
      {
        RestrictionFlags flags = restriction != null ? restriction.Flags : 0;
        if (flags != 0 && (flags & (RestrictionFlags.Length | RestrictionFlags.MinLength | RestrictionFlags.MaxLength | RestrictionFlags.MaxExclusive | RestrictionFlags.MaxInclusive | RestrictionFlags.MinExclusive | RestrictionFlags.MinInclusive | RestrictionFlags.TotalDigits | RestrictionFlags.FractionDigits | RestrictionFlags.Enumeration)) != 0)
        {
          return true;
        }
        return false;
      }
    }

    internal abstract Type ListValueType { get; }

    internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet { get { return XmlSchemaWhiteSpace.Preserve; } }

    public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
    {
      object typedValue;
      Exception exception = TryParseValue(s, nameTable, nsmgr, out typedValue);
      if (exception != null)
      {
        throw new XmlSchemaException(Res.Sch_InvalidValueDetailed, new string[] { s, GetTypeName(), exception.Message }, exception, null, 0, 0, null);
      }
      if (this.Variety == XmlSchemaDatatypeVariety.Union)
      {
        XsdSimpleValue simpleValue = typedValue as XsdSimpleValue;
        return simpleValue.TypedValue;
      }
      return typedValue;
    }

    internal override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, bool createAtomicValue)
    {
      if (createAtomicValue)
      {
        object typedValue;
        Exception exception = TryParseValue(s, nameTable, nsmgr, out typedValue);
        if (exception != null)
        {
          throw new XmlSchemaException(Res.Sch_InvalidValueDetailed, new string[] { s, GetTypeName(), exception.Message }, exception, null, 0, 0, null);
        }
        return typedValue;
      }
      else
      {
        return ParseValue(s, nameTable, nsmgr);
      }
    }

    internal override Exception TryParseValue(object value, XmlNameTable nameTable, IXmlNamespaceResolver namespaceResolver, out object typedValue)
    {
      Exception exception = null;
      typedValue = null;
      if (value == null)
      {
        return new ArgumentNullException("value");
      }
      string s = value as string;
      if (s != null)
      {
        return TryParseValue(s, nameTable, namespaceResolver, out typedValue);
      }
      try
      {
        object valueToCheck = value;
        if (value.GetType() != this.ValueType)
        {
          valueToCheck = this.ValueConverter.ChangeType(value, this.ValueType, namespaceResolver);
        }
        if (this.HasLexicalFacets)
        {
          string s1 = (string)this.ValueConverter.ChangeType(value, typeof(System.String), namespaceResolver); //Using value here to avoid info loss
          exception = this.FacetsChecker.CheckLexicalFacets(ref s1, this);
          if (exception != null) goto Error;
        }
        if (this.HasValueFacets)
        {
          exception = this.FacetsChecker.CheckValueFacets(valueToCheck, this);
          if (exception != null) goto Error;
        }
        typedValue = valueToCheck;
        return null;
      }
      catch (FormatException e)
      { //Catching for exceptions thrown by ValueConverter
        exception = e;
      }
      catch (InvalidCastException e)
      { //Catching for exceptions thrown by ValueConverter
        exception = e;
      }
      catch (OverflowException e)
      { //Catching for exceptions thrown by ValueConverter
        exception = e;
      }
      catch (ArgumentException e)
      { //Catching for exceptions thrown by ValueConverter
        exception = e;
      }

    Error:
      return exception;
    }

    internal string GetTypeName()
    {
      XmlSchemaType simpleType = this.parentSchemaType;
      string typeName;
      if (simpleType == null || simpleType.QualifiedName.IsEmpty)
      { //If no QName, get typecode, no line info since it is not pertinent without file name
        typeName = TypeCodeString;
      }
      else
      {
        typeName = simpleType.QualifiedName.ToString();
      }
      return typeName;
    }

    // XSD types
    static private readonly DatatypeImplementation c_anySimpleType = new Datatype_anySimpleType();
    static private readonly DatatypeImplementation c_anyURI = new Datatype_anyURI();
    static private readonly DatatypeImplementation c_base64Binary = new Datatype_base64Binary();
    static private readonly DatatypeImplementation c_boolean = new Datatype_boolean();
    static private readonly DatatypeImplementation c_byte = new Datatype_byte();
    static private readonly DatatypeImplementation c_char = new Datatype_char(); // XDR
    static private readonly DatatypeImplementation c_date = new Datatype_date();
    static private readonly DatatypeImplementation c_dateTime = new Datatype_dateTime();
    static private readonly DatatypeImplementation c_dateTimeNoTz = new Datatype_dateTimeNoTimeZone(); // XDR
    static private readonly DatatypeImplementation c_dateTimeTz = new Datatype_dateTimeTimeZone(); // XDR
    static private readonly DatatypeImplementation c_day = new Datatype_day();
    static private readonly DatatypeImplementation c_decimal = new Datatype_decimal();
    static private readonly DatatypeImplementation c_double = new Datatype_double();
    static private readonly DatatypeImplementation c_doubleXdr = new Datatype_doubleXdr();     // XDR
    static private readonly DatatypeImplementation c_duration = new Datatype_duration();
    static private readonly DatatypeImplementation c_ENTITY = new Datatype_ENTITY();
    static private readonly DatatypeImplementation c_ENTITIES = (DatatypeImplementation)c_ENTITY.DeriveByList(1, null);
    static private readonly DatatypeImplementation c_ENUMERATION = new Datatype_ENUMERATION(); // XDR
    static private readonly DatatypeImplementation c_fixed = new Datatype_fixed();
    static private readonly DatatypeImplementation c_float = new Datatype_float();
    static private readonly DatatypeImplementation c_floatXdr = new Datatype_floatXdr(); // XDR
    static private readonly DatatypeImplementation c_hexBinary = new Datatype_hexBinary();
    static private readonly DatatypeImplementation c_ID = new Datatype_ID();
    static private readonly DatatypeImplementation c_IDREF = new Datatype_IDREF();
    static private readonly DatatypeImplementation c_IDREFS = (DatatypeImplementation)c_IDREF.DeriveByList(1, null);
    static private readonly DatatypeImplementation c_int = new Datatype_int();
    static private readonly DatatypeImplementation c_integer = new Datatype_integer();
    static private readonly DatatypeImplementation c_language = new Datatype_language();
    static private readonly DatatypeImplementation c_long = new Datatype_long();
    static private readonly DatatypeImplementation c_month = new Datatype_month();
    static private readonly DatatypeImplementation c_monthDay = new Datatype_monthDay();
    static private readonly DatatypeImplementation c_Name = new Datatype_Name();
    static private readonly DatatypeImplementation c_NCName = new Datatype_NCName();
    static private readonly DatatypeImplementation c_negativeInteger = new Datatype_negativeInteger();
    static private readonly DatatypeImplementation c_NMTOKEN = new Datatype_NMTOKEN();
    static private readonly DatatypeImplementation c_NMTOKENS = (DatatypeImplementation)c_NMTOKEN.DeriveByList(1, null);
    static private readonly DatatypeImplementation c_nonNegativeInteger = new Datatype_nonNegativeInteger();
    static private readonly DatatypeImplementation c_nonPositiveInteger = new Datatype_nonPositiveInteger();
    static private readonly DatatypeImplementation c_normalizedString = new Datatype_normalizedString();
    static private readonly DatatypeImplementation c_NOTATION = new Datatype_NOTATION();
    static private readonly DatatypeImplementation c_positiveInteger = new Datatype_positiveInteger();
    static private readonly DatatypeImplementation c_QName = new Datatype_QName();
    static private readonly DatatypeImplementation c_QNameXdr = new Datatype_QNameXdr(); //XDR
    static private readonly DatatypeImplementation c_short = new Datatype_short();
    static private readonly DatatypeImplementation c_string = new Datatype_string();
    static private readonly DatatypeImplementation c_time = new Datatype_time();
    static private readonly DatatypeImplementation c_timeNoTz = new Datatype_timeNoTimeZone(); // XDR
    static private readonly DatatypeImplementation c_timeTz = new Datatype_timeTimeZone(); // XDR
    static private readonly DatatypeImplementation c_token = new Datatype_token();
    static private readonly DatatypeImplementation c_unsignedByte = new Datatype_unsignedByte();
    static private readonly DatatypeImplementation c_unsignedInt = new Datatype_unsignedInt();
    static private readonly DatatypeImplementation c_unsignedLong = new Datatype_unsignedLong();
    static private readonly DatatypeImplementation c_unsignedShort = new Datatype_unsignedShort();
    static private readonly DatatypeImplementation c_uuid = new Datatype_uuid(); // XDR
    static private readonly DatatypeImplementation c_year = new Datatype_year();
    static private readonly DatatypeImplementation c_yearMonth = new Datatype_yearMonth();

    static private readonly DatatypeImplementation c_anyAtomicType = new Datatype_anyAtomicType();
    static private readonly DatatypeImplementation c_dayTimeDuration = new Datatype_dayTimeDuration();
    static private readonly DatatypeImplementation c_untypedAtomicType = new Datatype_untypedAtomicType();
    static private readonly DatatypeImplementation c_yearMonthDuration = new Datatype_yearMonthDuration();


    private class SchemaDatatypeMap : IComparable
    {
      string name;
      DatatypeImplementation type;
      int parentIndex;

      internal SchemaDatatypeMap(string name, DatatypeImplementation type)
      {
        this.name = name;
        this.type = type;
      }

      internal SchemaDatatypeMap(string name, DatatypeImplementation type, int parentIndex)
      {
        this.name = name;
        this.type = type;
        this.parentIndex = parentIndex;
      }
      public static explicit operator DatatypeImplementation(SchemaDatatypeMap sdm) { return sdm.type; }

      public string Name
      {
        get
        {
          return name;
        }
      }

      public int ParentIndex
      {
        get
        {
          return parentIndex;
        }
      }

      public int CompareTo(object obj) { return string.Compare(name, (string)obj, StringComparison.Ordinal); }
    }

    private static readonly DatatypeImplementation[] c_tokenizedTypes = {
            c_string,               // CDATA
            c_ID,                   // ID
            c_IDREF,                // IDREF
            c_IDREFS,               // IDREFS
            c_ENTITY,               // ENTITY
            c_ENTITIES,             // ENTITIES
            c_NMTOKEN,              // NMTOKEN
            c_NMTOKENS,             // NMTOKENS
            c_NOTATION,             // NOTATION
            c_ENUMERATION,          // ENUMERATION
            c_QNameXdr,             // QName
            c_NCName,               // NCName
            null
        };

    private static readonly DatatypeImplementation[] c_tokenizedTypesXsd = {
            c_string,               // CDATA
            c_ID,                   // ID
            c_IDREF,                // IDREF
            c_IDREFS,               // IDREFS
            c_ENTITY,               // ENTITY
            c_ENTITIES,             // ENTITIES
            c_NMTOKEN,              // NMTOKEN
            c_NMTOKENS,             // NMTOKENS
            c_NOTATION,             // NOTATION
            c_ENUMERATION,          // ENUMERATION
            c_QName,                // QName
            c_NCName,               // NCName
            null
        };

    private static readonly SchemaDatatypeMap[] c_XdrTypes = {
            new SchemaDatatypeMap("bin.base64",          c_base64Binary),
            new SchemaDatatypeMap("bin.hex",             c_hexBinary),
            new SchemaDatatypeMap("boolean",             c_boolean),
            new SchemaDatatypeMap("char",                c_char),
            new SchemaDatatypeMap("date",                c_date),
            new SchemaDatatypeMap("dateTime",            c_dateTimeNoTz),
            new SchemaDatatypeMap("dateTime.tz",         c_dateTimeTz),
            new SchemaDatatypeMap("decimal",             c_decimal),
            new SchemaDatatypeMap("entities",            c_ENTITIES),
            new SchemaDatatypeMap("entity",              c_ENTITY),
            new SchemaDatatypeMap("enumeration",         c_ENUMERATION),
            new SchemaDatatypeMap("fixed.14.4",          c_fixed),
            new SchemaDatatypeMap("float",               c_doubleXdr),
            new SchemaDatatypeMap("float.ieee.754.32",   c_floatXdr),
            new SchemaDatatypeMap("float.ieee.754.64",   c_doubleXdr),
            new SchemaDatatypeMap("i1",                  c_byte),
            new SchemaDatatypeMap("i2",                  c_short),
            new SchemaDatatypeMap("i4",                  c_int),
            new SchemaDatatypeMap("i8",                  c_long),
            new SchemaDatatypeMap("id",                  c_ID),
            new SchemaDatatypeMap("idref",               c_IDREF),
            new SchemaDatatypeMap("idrefs",              c_IDREFS),
            new SchemaDatatypeMap("int",                 c_int),
            new SchemaDatatypeMap("nmtoken",             c_NMTOKEN),
            new SchemaDatatypeMap("nmtokens",            c_NMTOKENS),
            new SchemaDatatypeMap("notation",            c_NOTATION),
            new SchemaDatatypeMap("number",              c_doubleXdr),
            new SchemaDatatypeMap("r4",                  c_floatXdr),
            new SchemaDatatypeMap("r8",                  c_doubleXdr),
            new SchemaDatatypeMap("string",              c_string),
            new SchemaDatatypeMap("time",                c_timeNoTz),
            new SchemaDatatypeMap("time.tz",             c_timeTz),
            new SchemaDatatypeMap("ui1",                 c_unsignedByte),
            new SchemaDatatypeMap("ui2",                 c_unsignedShort),
            new SchemaDatatypeMap("ui4",                 c_unsignedInt),
            new SchemaDatatypeMap("ui8",                 c_unsignedLong),
            new SchemaDatatypeMap("uri",                 c_anyURI),
            new SchemaDatatypeMap("uuid",                c_uuid)
        };


    private static readonly SchemaDatatypeMap[] c_XsdTypes = {
            new SchemaDatatypeMap("ENTITIES",           c_ENTITIES, 11),
            new SchemaDatatypeMap("ENTITY",             c_ENTITY, 11),
            new SchemaDatatypeMap("ID",                 c_ID, 5),
            new SchemaDatatypeMap("IDREF",              c_IDREF, 5),
            new SchemaDatatypeMap("IDREFS",             c_IDREFS, 11),

            new SchemaDatatypeMap("NCName",             c_NCName, 9),
            new SchemaDatatypeMap("NMTOKEN",            c_NMTOKEN, 40),
            new SchemaDatatypeMap("NMTOKENS",           c_NMTOKENS, 11),
            new SchemaDatatypeMap("NOTATION",           c_NOTATION, 11),

            new SchemaDatatypeMap("Name",               c_Name, 40),
            new SchemaDatatypeMap("QName",              c_QName, 11), //-> 10

            new SchemaDatatypeMap("anySimpleType",      c_anySimpleType, -1),
            new SchemaDatatypeMap("anyURI",             c_anyURI, 11),
            new SchemaDatatypeMap("base64Binary",       c_base64Binary, 11),
            new SchemaDatatypeMap("boolean",            c_boolean, 11),
            new SchemaDatatypeMap("byte",               c_byte, 37),
            new SchemaDatatypeMap("date",               c_date, 11),
            new SchemaDatatypeMap("dateTime",           c_dateTime, 11),
            new SchemaDatatypeMap("decimal",            c_decimal, 11),
            new SchemaDatatypeMap("double",             c_double, 11),
            new SchemaDatatypeMap("duration",           c_duration, 11), //->20

            new SchemaDatatypeMap("float",              c_float, 11),
            new SchemaDatatypeMap("gDay",               c_day, 11),
            new SchemaDatatypeMap("gMonth",             c_month, 11),
            new SchemaDatatypeMap("gMonthDay",          c_monthDay, 11),
            new SchemaDatatypeMap("gYear",              c_year, 11),
            new SchemaDatatypeMap("gYearMonth",         c_yearMonth, 11),
            new SchemaDatatypeMap("hexBinary",          c_hexBinary, 11),
            new SchemaDatatypeMap("int",                c_int, 31),
            new SchemaDatatypeMap("integer",            c_integer, 18),
            new SchemaDatatypeMap("language",           c_language, 40), //->30
            new SchemaDatatypeMap("long",               c_long, 29),

            new SchemaDatatypeMap("negativeInteger",    c_negativeInteger, 34),

            new SchemaDatatypeMap("nonNegativeInteger", c_nonNegativeInteger, 29),
            new SchemaDatatypeMap("nonPositiveInteger", c_nonPositiveInteger, 29),
            new SchemaDatatypeMap("normalizedString",   c_normalizedString, 38),

            new SchemaDatatypeMap("positiveInteger",    c_positiveInteger, 33),

            new SchemaDatatypeMap("short",              c_short, 28),
            new SchemaDatatypeMap("string",             c_string, 11),
            new SchemaDatatypeMap("time",               c_time, 11),
            new SchemaDatatypeMap("token",              c_token, 35), //->40
            new SchemaDatatypeMap("unsignedByte",       c_unsignedByte, 44),
            new SchemaDatatypeMap("unsignedInt",        c_unsignedInt, 43),
            new SchemaDatatypeMap("unsignedLong",       c_unsignedLong, 33),
            new SchemaDatatypeMap("unsignedShort",      c_unsignedShort, 42),
        };

    protected int Compare(byte[] value1, byte[] value2)
    {
      int length = value1.Length;
      if (length != value2.Length)
      {
        return -1;
      }
      for (int i = 0; i < length; i++)
      {
        if (value1[i] != value2[i])
        {
          return -1;
        }
      }
      return 0;
    }
  }
}
