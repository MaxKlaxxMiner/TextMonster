using System;

namespace TextMonster.Xml.XmlReader
{
  /// <summary>
  /// Iterate over all element descendants with a particular QName.
  /// </summary>
  internal class XPathDocumentElementDescendantIterator : XPathDocumentBaseIterator
  {
    private XPathDocumentNavigator end;
    private string localName, namespaceUri;
    private bool matchSelf;

    /// <summary>
    /// Create an iterator that ranges over all element descendants of "root" having the specified QName.
    /// </summary>
    public XPathDocumentElementDescendantIterator(XPathDocumentNavigator root, string name, string namespaceURI, bool matchSelf)
      : base(root)
    {
      if (namespaceURI == null) throw new ArgumentNullException("namespaceURI");

      this.localName = root.NameTable.Get(name);
      this.namespaceUri = namespaceURI;
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
    public XPathDocumentElementDescendantIterator(XPathDocumentElementDescendantIterator iter)
      : base(iter)
    {
      this.end = iter.end;
      this.localName = iter.localName;
      this.namespaceUri = iter.namespaceUri;
      this.matchSelf = iter.matchSelf;
    }

    /// <summary>
    /// Create a copy of this iterator.
    /// </summary>
    public override XPathNodeIterator Clone()
    {
      return new XPathDocumentElementDescendantIterator(this);
    }

    /// <summary>
    /// Position the iterator to the next descendant.
    /// </summary>
    public override bool MoveNext()
    {
      if (this.matchSelf)
      {
        this.matchSelf = false;

        if (this.ctxt.IsElementMatch(this.localName, this.namespaceUri))
        {
          this.pos++;
          return true;
        }
      }

      if (!this.ctxt.MoveToFollowing(this.localName, this.namespaceUri, this.end))
        return false;

      this.pos++;
      return true;
    }
  }
}
