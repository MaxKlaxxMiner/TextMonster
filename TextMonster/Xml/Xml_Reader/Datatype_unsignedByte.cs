using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_unsignedByte : Datatype_unsignedShort
  {
    static readonly Type atomicValueType = typeof(byte);
    static readonly Type listValueType = typeof(byte[]);
    static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(byte.MinValue, byte.MaxValue);

    internal override FacetsChecker FacetsChecker { get { return numeric10FacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.UnsignedByte; } }

    internal override int Compare(object value1, object value2)
    {
      return ((byte)value1).CompareTo(value2);
    }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override Exception TryParseValue(string s, NameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      byte byteValue;
      exception = XmlConvert.TryToByte(s, out byteValue);
      if (exception != null) goto Error;

      exception = numeric10FacetsChecker.CheckValueFacets((short)byteValue, this);
      if (exception != null) goto Error;

      typedValue = byteValue;

      return null;

    Error:
      return exception;
    }
  }
}
