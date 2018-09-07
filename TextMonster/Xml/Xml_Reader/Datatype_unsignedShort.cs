using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_unsignedShort : Datatype_unsignedInt
  {
    static readonly Type atomicValueType = typeof(ushort);
    static readonly Type listValueType = typeof(ushort[]);
    static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(ushort.MinValue, ushort.MaxValue);

    internal override FacetsChecker FacetsChecker { get { return numeric10FacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.UnsignedShort; } }

    internal override int Compare(object value1, object value2)
    {
      return ((ushort)value1).CompareTo(value2);
    }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override Exception TryParseValue(string s, NameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      ushort uint16Value;
      exception = XmlConvert.TryToUInt16(s, out uint16Value);
      if (exception != null) goto Error;

      exception = numeric10FacetsChecker.CheckValueFacets((int)uint16Value, this);
      if (exception != null) goto Error;

      typedValue = uint16Value;

      return null;

    Error:
      return exception;
    }
  }
}
