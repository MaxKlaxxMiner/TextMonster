namespace TextMonster.Xml.Xml_Reader
{
  public interface IXsltContextFunction
  {
    XPathResultType ReturnType { get; }
    object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext);
  }
}