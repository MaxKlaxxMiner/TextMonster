using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_boolean : Datatype_anySimpleType
  {
    static readonly Type atomicValueType = typeof(bool);
    static readonly Type listValueType = typeof(bool[]);

    internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
    {
      return XmlBooleanConverter.Create(schemaType);
    }

    internal override FacetsChecker FacetsChecker { get { return miscFacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.Boolean; } }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet { get { return XmlSchemaWhiteSpace.Collapse; } }

    internal override RestrictionFlags ValidRestrictionFlags
    {
      get
      {
        return RestrictionFlags.Pattern |
               RestrictionFlags.WhiteSpace;
      }
    }

    internal override int Compare(object value1, object value2)
    {
      return ((bool)value1).CompareTo(value2);
    }

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;
      typedValue = null;

      exception = miscFacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      bool boolValue;
      exception = XmlConvert.TryToBoolean(s, out boolValue);
      if (exception != null) goto Error;

      typedValue = boolValue;

      return null;

    Error:
      return exception;
    }
  }
}
