using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_decimal : Datatype_anySimpleType
  {
    static readonly Type atomicValueType = typeof(decimal);
    static readonly Type listValueType = typeof(decimal[]);
    static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(decimal.MinValue, decimal.MaxValue);

    internal override XmlValueConverter CreateValueConverter(XmlSchemaType schemaType)
    {
      return XmlNumeric10Converter.Create(schemaType);
    }

    internal override FacetsChecker FacetsChecker { get { return numeric10FacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.Decimal; } }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override XmlSchemaWhiteSpace BuiltInWhitespaceFacet { get { return XmlSchemaWhiteSpace.Collapse; } }

    internal override int Compare(object value1, object value2)
    {
      return ((decimal)value1).CompareTo(value2);
    }

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      decimal decimalValue;
      exception = XmlConvert.TryToDecimal(s, out decimalValue);
      if (exception != null) goto Error;

      exception = numeric10FacetsChecker.CheckValueFacets(decimalValue, this);
      if (exception != null) goto Error;

      typedValue = decimalValue;

      return null;

    Error:
      return exception;
    }
  }
}
