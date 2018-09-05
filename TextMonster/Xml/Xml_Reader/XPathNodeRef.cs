namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// A reference to a XPathNode is composed of two values: the page on which the node is located, and the node's
  /// index in the page.
  /// </summary>
  internal struct XPathNodeRef
  {
    private XPathNode[] page;
    private int idx;

    public XPathNodeRef(XPathNode[] page, int idx)
    {
      this.page = page;
      this.idx = idx;
    }

    public XPathNode[] Page
    {
      get { return this.page; }
    }

    public int Index
    {
      get { return this.idx; }
    }

    public override int GetHashCode()
    {
      return XPathNodeHelper.GetLocation(this.page, this.idx);
    }
  }
}
