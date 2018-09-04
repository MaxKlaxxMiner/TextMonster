namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Iterate over all content descendants with a particular XPathNodeType.
  /// </summary>
  internal class XPathDocumentKindDescendantIterator : XPathDocumentBaseIterator
  {
    private XPathDocumentNavigator end;
    private XPathNodeType typ;
    private bool matchSelf;

    /// <summary>
    /// Create an iterator that ranges over all content descendants of "root" having the specified XPathNodeType.
    /// </summary>
    public XPathDocumentKindDescendantIterator(XPathDocumentNavigator root, XPathNodeType typ, bool matchSelf)
      : base(root)
    {
      this.typ = typ;
      this.matchSelf = matchSelf;

      // Find the next non-descendant node that follows "root" in document order
      if (root.NodeType != XPathNodeType.Root)
      {
        this.end = new XPathDocumentNavigator(root);
        this.end.MoveToNonDescendant();
      }
    }

    /// <summary>
    /// Create a new iterator that is a copy of "iter".
    /// </summary>
    public XPathDocumentKindDescendantIterator(XPathDocumentKindDescendantIterator iter)
      : base(iter)
    {
      this.end = iter.end;
      this.typ = iter.typ;
      this.matchSelf = iter.matchSelf;
    }

    /// <summary>
    /// Create a copy of this iterator.
    /// </summary>
    public override XPathNodeIterator Clone()
    {
      return new XPathDocumentKindDescendantIterator(this);
    }

    /// <summary>
    /// Position the iterator to the next descendant.
    /// </summary>
    public override bool MoveNext()
    {
      if (this.matchSelf)
      {
        this.matchSelf = false;

        if (this.ctxt.IsKindMatch(this.typ))
        {
          this.pos++;
          return true;
        }
      }

      if (!this.ctxt.MoveToFollowing(this.typ, this.end))
        return false;

      this.pos++;
      return true;
    }
  }
}
