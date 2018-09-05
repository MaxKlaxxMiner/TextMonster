namespace TextMonster.Xml.Xml_Reader
{
  public abstract class XsltContext : XmlNamespaceManager
  {
    // This dummy XsltContext that doesn't actualy initialize XmlNamespaceManager
    // is used by XsltCompileContext
    internal XsltContext(bool dummy) : base() { }
    public abstract IXsltContextVariable ResolveVariable(string prefix, string name);
    public abstract IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes);
    public abstract bool Whitespace { get; }
    public abstract bool PreserveWhitespace(XPathNavigator node);
  }
}
