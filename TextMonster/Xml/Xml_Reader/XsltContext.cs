namespace TextMonster.Xml.Xml_Reader
{
  public abstract class XsltContext : XmlNamespaceManager
  {
    // This dummy XsltContext that doesn't actualy initialize XmlNamespaceManager
    // is used by XsltCompileContext
    internal XsltContext(bool dummy)
    { }
    public abstract IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes);
  }
}
