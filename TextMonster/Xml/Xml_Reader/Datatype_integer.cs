using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_integer : Datatype_decimal
  {
    public override XmlTypeCode TypeCode { get { return XmlTypeCode.Integer; } }

    internal override Exception TryParseValue(string s, NameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = FacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      decimal decimalValue;
      exception = XmlConvert.TryToInteger(s, out decimalValue);
      if (exception != null) goto Error;

      exception = FacetsChecker.CheckValueFacets(decimalValue, this);
      if (exception != null) goto Error;

      typedValue = decimalValue;

      return null;

    Error:
      return exception;
    }
  }
}
