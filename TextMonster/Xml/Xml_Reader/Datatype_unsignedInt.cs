using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_unsignedInt : Datatype_unsignedLong
  {
    static readonly Type atomicValueType = typeof(uint);
    static readonly Type listValueType = typeof(uint[]);
    static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(uint.MinValue, uint.MaxValue);

    internal override FacetsChecker FacetsChecker { get { return numeric10FacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.UnsignedInt; } }

    internal override int Compare(object value1, object value2)
    {
      return ((uint)value1).CompareTo(value2);
    }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      uint uint32Value;
      exception = XmlConvert.TryToUInt32(s, out uint32Value);
      if (exception != null) goto Error;

      exception = numeric10FacetsChecker.CheckValueFacets((long)uint32Value, this);
      if (exception != null) goto Error;

      typedValue = uint32Value;

      return null;

    Error:
      return exception;
    }
  }
}
