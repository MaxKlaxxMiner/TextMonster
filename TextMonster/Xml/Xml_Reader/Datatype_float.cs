using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_float : Datatype_anySimpleType
  {
    static readonly Type atomicValueType = typeof(float);
    static readonly Type listValueType = typeof(float[]);

    internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
    {
      return XmlNumeric2Converter.Create(schemaType);
    }

    internal override FacetsChecker FacetsChecker { get { return numeric2FacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.Float; } }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet { get { return XmlSchemaWhiteSpace.Collapse; } }

    internal override int Compare(object value1, object value2)
    {
      return ((float)value1).CompareTo(value2);
    }

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = numeric2FacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      float singleValue;
      exception = XmlConvert.TryToSingle(s, out singleValue);
      if (exception != null) goto Error;

      exception = numeric2FacetsChecker.CheckValueFacets(singleValue, this);
      if (exception != null) goto Error;

      typedValue = singleValue;

      return null;

    Error:
      return exception;
    }
  }
}
