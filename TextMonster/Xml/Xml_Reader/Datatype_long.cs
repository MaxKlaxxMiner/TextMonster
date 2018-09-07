using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_long : Datatype_integer
  {
    static readonly Type atomicValueType = typeof(long);
    static readonly Type listValueType = typeof(long[]);
    static readonly FacetsChecker numeric10FacetsChecker = new Numeric10FacetsChecker(long.MinValue, long.MaxValue);

    internal override FacetsChecker FacetsChecker { get { return numeric10FacetsChecker; } }

    internal override bool HasValueFacets
    {
      get
      {
        return true; //Built-in facet to check range
      }
    }

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.Long; } }

    internal override int Compare(object value1, object value2)
    {
      return ((long)value1).CompareTo(value2);
    }

    public override Type ValueType { get { return atomicValueType; } }

    internal override Type ListValueType { get { return listValueType; } }

    internal override Exception TryParseValue(string s, NameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = numeric10FacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      long int64Value;
      exception = XmlConvert.TryToInt64(s, out int64Value);
      if (exception != null) goto Error;

      exception = numeric10FacetsChecker.CheckValueFacets(int64Value, this);
      if (exception != null) goto Error;

      typedValue = int64Value;

      return null;

    Error:
      return exception;
    }
  }
}
