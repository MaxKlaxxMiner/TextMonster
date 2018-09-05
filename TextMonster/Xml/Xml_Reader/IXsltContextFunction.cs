namespace TextMonster.Xml.Xml_Reader
{
  public interface IXsltContextFunction
  {
    int Minargs { get; }
    int Maxargs { get; }
    XPathResultType ReturnType { get; }
    XPathResultType[] ArgTypes { get; }
    object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext);
  }
}