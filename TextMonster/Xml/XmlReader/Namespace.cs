namespace TextMonster.Xml.XmlReader
{
  partial struct Namespace
  {

    internal string prefix;
    internal string namespaceUri;
    internal NamespaceKind kind;
    internal int prevNsIndex;

    internal void Set(string prefix, string namespaceUri, NamespaceKind kind)
    {
      this.prefix = prefix;
      this.namespaceUri = namespaceUri;
      this.kind = kind;
      this.prevNsIndex = -1;
    }

    internal void WriteDecl(XmlWriter writer, XmlRawWriter rawWriter)
    {
      if (null != rawWriter)
      {
        rawWriter.WriteNamespaceDeclaration(prefix, namespaceUri);
      }
      else
      {
        if (prefix.Length == 0)
        {
          writer.WriteStartAttribute(string.Empty, "xmlns", XmlReservedNs.NsXmlNs);
        }
        else
        {
          writer.WriteStartAttribute("xmlns", prefix, XmlReservedNs.NsXmlNs);
        }
        writer.WriteString(namespaceUri);
        writer.WriteEndAttribute();
      }
    }
  }
}
