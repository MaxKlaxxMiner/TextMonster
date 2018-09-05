namespace TextMonster.Xml.Xml_Reader
{
  public interface IXsltContextVariable
  {
    bool IsLocal { get; }
    bool IsParam { get; }
    XPathResultType VariableType { get; }
    object Evaluate(XsltContext xsltContext);
  }
}