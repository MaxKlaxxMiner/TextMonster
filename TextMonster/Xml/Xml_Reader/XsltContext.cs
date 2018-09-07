namespace TextMonster.Xml.Xml_Reader
{
  public abstract class XsltContext : XmlNamespaceManager
  {
    // This dummy XsltContext that doesn't actualy initialize XmlNamespaceManager
    // is used by XsltCompileContext
    internal XsltContext(bool dummy)
    { }
  }
}
