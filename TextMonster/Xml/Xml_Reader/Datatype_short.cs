using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_short : Datatype_int
  {
    static readonly Type atomicValueType = typeof(short);
    static readonly Type listValueType = typeof(short[]);
    static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(short.MinValue, short.MaxValue);

    internal override FacetsChecker FacetsChecker { get { return numeric10FacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.Short; } }

    internal override int Compare(object value1, object value2)
    {
      return ((short)value1).CompareTo(value2);
    }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override Exception TryParseValue(string s, NameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      short int16Value;
      exception = XmlConvert.TryToInt16(s, out int16Value);
      if (exception != null) goto Error;

      exception = numeric10FacetsChecker.CheckValueFacets(int16Value, this);
      if (exception != null) goto Error;

      typedValue = int16Value;

      return null;

    Error:
      return exception;
    }
  }
}
