namespace TextMonster.Xml.XmlReader
{
  struct AttrName
  {
    internal string prefix;
    internal string namespaceUri;
    internal string localName;
    internal int prev;

    internal void Set(string prefix, string localName, string namespaceUri)
    {
      this.prefix = prefix;
      this.namespaceUri = namespaceUri;
      this.localName = localName;
      this.prev = 0;
    }

    internal bool IsDuplicate(string prefix, string localName, string namespaceUri)
    {
      return ((this.localName == localName)
          && ((this.prefix == prefix) || (this.namespaceUri == namespaceUri)));
    }
  }
}
