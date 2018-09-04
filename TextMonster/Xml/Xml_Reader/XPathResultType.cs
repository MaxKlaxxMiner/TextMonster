namespace TextMonster.Xml.Xml_Reader
{
  public enum XPathResultType
  {
    Number = 0,
    String = 1,
    Boolean = 2,
    NodeSet = 3,
    Navigator = XPathResultType.String,
    Any = 5,
    Error
  };
}
