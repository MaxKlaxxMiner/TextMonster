namespace TextMonster.Xml.Xml_Reader
{
  public interface IXsltContextVariable
  {
    XPathResultType VariableType { get; }
    object Evaluate(XsltContext xsltContext);
  }
}