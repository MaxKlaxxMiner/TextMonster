using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_byte : Datatype_short
  {
    static readonly Type atomicValueType = typeof(sbyte);
    static readonly Type listValueType = typeof(sbyte[]);
    static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(sbyte.MinValue, sbyte.MaxValue);

    internal override FacetsChecker FacetsChecker { get { return numeric10FacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.Byte; } }

    internal override int Compare(object value1, object value2)
    {
      return ((sbyte)value1).CompareTo(value2);
    }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      sbyte sbyteValue;
      exception = XmlConvert.TryToSByte(s, out sbyteValue);
      if (exception != null) goto Error;

      exception = numeric10FacetsChecker.CheckValueFacets((short)sbyteValue, this);
      if (exception != null) goto Error;

      typedValue = sbyteValue;

      return null;

    Error:
      return exception;
    }
  }
}
