using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_int : Datatype_long
  {
    static readonly Type atomicValueType = typeof(int);
    static readonly Type listValueType = typeof(int[]);
    static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(int.MinValue, int.MaxValue);

    internal override FacetsChecker FacetsChecker { get { return numeric10FacetsChecker; } }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.Int; } }

    internal override int Compare(object value1, object value2)
    {
      return ((int)value1).CompareTo(value2);
    }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      int int32Value;
      exception = XmlConvert.TryToInt32(s, out int32Value);
      if (exception != null) goto Error;

      exception = numeric10FacetsChecker.CheckValueFacets(int32Value, this);
      if (exception != null) goto Error;

      typedValue = int32Value;

      return null;

    Error:
      return exception;
    }
  }
}
