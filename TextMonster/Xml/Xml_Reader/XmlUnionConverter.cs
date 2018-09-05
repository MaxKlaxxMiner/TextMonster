using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class XmlUnionConverter : XmlBaseConverter
  {
    private XmlValueConverter[] converters;
    private bool hasAtomicMember, hasListMember;

    protected XmlUnionConverter(XmlSchemaType schemaType)
      : base(schemaType)
    {
      // Skip restrictions. It is safe to do that because this is a union, so it's not a built-in type 
      while (schemaType.DerivedBy == XmlSchemaDerivationMethod.Restriction)
        schemaType = schemaType.BaseXmlSchemaType;

      // Get a converter for each member type in the union
      XmlSchemaSimpleType[] memberTypes = ((XmlSchemaSimpleTypeUnion)((XmlSchemaSimpleType)schemaType).Content).BaseMemberTypes;

      this.converters = new XmlValueConverter[memberTypes.Length];
      for (int i = 0; i < memberTypes.Length; i++)
      {
        this.converters[i] = memberTypes[i].ValueConverter;

        // Track whether this union's member types include a list type
        if (memberTypes[i].Datatype.Variety == XmlSchemaDatatypeVariety.List)
          this.hasListMember = true;
        else if (memberTypes[i].Datatype.Variety == XmlSchemaDatatypeVariety.Atomic)
          this.hasAtomicMember = true;
      }
    }

    public static XmlValueConverter Create(XmlSchemaType schemaType)
    {
      return new XmlUnionConverter(schemaType);
    }


    //-----------------------------------------------
    // ChangeType
    //-----------------------------------------------

    public override object ChangeType(object value, Type destinationType, IXmlNamespaceResolver nsResolver)
    {
      if (value == null) throw new ArgumentNullException("value");
      if (destinationType == null) throw new ArgumentNullException("destinationType");

      Type sourceType = value.GetType();

      // If source value is an XmlAtomicValue, then allow it to perform the conversion
      if (sourceType == XmlAtomicValueType && hasAtomicMember)
        return ((XmlAtomicValue)value).ValueAs(destinationType, nsResolver);

      // If source value is an XmlAtomicValue[], then use item* converter
      if (sourceType == XmlAtomicValueArrayType && hasListMember)
        return XmlAnyListConverter.ItemList.ChangeType(value, destinationType, nsResolver);

      // If source value is a string, then validate the string in order to determine the member type
      if (sourceType == StringType)
      {
        if (destinationType == StringType) return value;

        XsdSimpleValue simpleValue = (XsdSimpleValue)SchemaType.Datatype.ParseValue((string)value, new NameTable(), nsResolver, true);

        // Allow the member type to perform the conversion
        return simpleValue.XmlType.ValueConverter.ChangeType((string)value, destinationType, nsResolver);
      }

      throw CreateInvalidClrMappingException(sourceType, destinationType);
    }
  }
}
