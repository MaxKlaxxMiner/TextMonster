namespace TextMonster.Xml.Xml_Reader
{
  internal class XsdSimpleValue
  { //Wrapper to store XmlType and TypedValue together
    XmlSchemaSimpleType xmlType;
    object typedValue;

    public XsdSimpleValue(XmlSchemaSimpleType st, object value)
    {
      xmlType = st;
      typedValue = value;
    }

    public XmlSchemaSimpleType XmlType
    {
      get
      {
        return xmlType;
      }
    }

    public object TypedValue
    {
      get
      {
        return typedValue;
      }
    }
  }
}
