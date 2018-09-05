using System;

namespace TextMonster.Xml.Xml_Reader
{
  internal class Datatype_NCName : Datatype_Name
  {

    public override XmlTypeCode TypeCode { get { return XmlTypeCode.NCName; } }

    internal override Exception TryParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr, out object typedValue)
    {
      Exception exception;

      typedValue = null;

      exception = stringFacetsChecker.CheckLexicalFacets(ref s, this);
      if (exception != null) goto Error;

      exception = stringFacetsChecker.CheckValueFacets(s, this);
      if (exception != null) goto Error;

      nameTable.Add(s);

      typedValue = s;
      return null;

    Error:
      return exception;
    }
  }
}
