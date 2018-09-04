namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Iterate over all content children with a particular XPathNodeType.
  /// </summary>
  internal class XPathDocumentKindChildIterator : XPathDocumentBaseIterator
  {
    private XPathNodeType typ;

    /// <summary>
    /// Create an iterator that ranges over all content children of "parent" having the specified XPathNodeType.
    /// </summary>
    public XPathDocumentKindChildIterator(XPathDocumentNavigator parent, XPathNodeType typ)
      : base(parent)
    {
      this.typ = typ;
    }

    /// <summary>
    /// Create a new iterator that is a copy of "iter".
    /// </summary>
    public XPathDocumentKindChildIterator(XPathDocumentKindChildIterator iter)
      : base(iter)
    {
      this.typ = iter.typ;
    }

    /// <summary>
    /// Create a copy of this iterator.
    /// </summary>
    public override XPathNodeIterator Clone()
    {
      return new XPathDocumentKindChildIterator(this);
    }

    /// <summary>
    /// Position the iterator to the next descendant.
    /// </summary>
    public override bool MoveNext()
    {
      if (this.pos == 0)
      {
        if (!this.ctxt.MoveToChild(this.typ))
          return false;
      }
      else
      {
        if (!this.ctxt.MoveToNext(this.typ))
          return false;
      }

      this.pos++;
      return true;
    }
  }
}
