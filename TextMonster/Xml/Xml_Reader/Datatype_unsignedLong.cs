using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_unsignedLong : Datatype_nonNegativeInteger
  {
    static readonly Type atomicValueType = typeof(ulong);
    static readonly Type listValueType = typeof(ulong[]);
    static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(ulong.MinValue, ulong.MaxValue);

    internal override FacetsChecker FacetsChecker { get { return numeric10FacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.UnsignedLong; } }

    internal override int Compare(object value1, object value2)
    {
      return ((ulong)value1).CompareTo(value2);
    }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override Exception TryParseValue(string s, NameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      ulong uint64Value;
      exception = XmlConvert.TryToUInt64(s, out uint64Value);
      if (exception != null) goto Error;

      exception = numeric10FacetsChecker.CheckValueFacets((decimal)uint64Value, this);
      if (exception != null) goto Error;

      typedValue = uint64Value;

      return null;

    Error:
      return exception;
    }
  }
}
