namespace TextMonster.Xml.Xml_Reader
{
  partial struct ElementScope
  {

    internal int prevNSTop;
    internal string prefix;
    internal string localName;
    internal string namespaceUri;
    internal XmlSpace xmlSpace;
    internal string xmlLang;

    internal void Set(string prefix, string localName, string namespaceUri, int prevNSTop)
    {
      this.prevNSTop = prevNSTop;
      this.prefix = prefix;
      this.namespaceUri = namespaceUri;
      this.localName = localName;
      this.xmlSpace = (XmlSpace)(int)-1;
      this.xmlLang = null;
    }

    internal void WriteEndElement(XmlRawWriter rawWriter)
    {
      rawWriter.WriteEndElement(prefix, localName, namespaceUri);
    }

    internal void WriteFullEndElement(XmlRawWriter rawWriter)
    {
      rawWriter.WriteFullEndElement(prefix, localName, namespaceUri);
    }
  }
}
