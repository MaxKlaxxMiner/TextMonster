using System;

namespace TextMonster.Xml.XmlReader
{
  // Primitive datatypes
  internal class Datatype_anySimpleType : DatatypeImplementation
  {
    static readonly Type atomicValueType = typeof(string);
    static readonly Type listValueType = typeof(string[]);

    internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
    {
      return XmlUntypedConverter.Untyped;
    }

    internal override FacetsChecker FacetsChecker { get { return miscFacetsChecker; } }

    public override Type ValueType { get { return atomicValueType; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.AnyAtomicType; } }

    internal override Type ListValueType { get { return listValueType; } }

    public override XmlTokenizedType TokenizedType { get { return XmlTokenizedType.None; } }

    internal override RestrictionFlags ValidRestrictionFlags { get { return 0; } }

    internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet { get { return XmlSchemaWhiteSpace.Collapse; } }

    internal override int Compare(object value1, object value2)
    {
      //Changed StringComparison.CurrentCulture to StringComparison.Ordinal to handle zero-weight code points like the cyrillic E
      return String.Compare(value1.ToString(), value2.ToString(), StringComparison.Ordinal);
    }

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      typedValue = XmlComplianceUtil.NonCDataNormalize(s); //Whitespace facet is treated as collapse since thats the way it was in Everett
      return null;
    }
  }

}
