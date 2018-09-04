using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Iterate over all element children with a particular QName.
  /// </summary>
  internal class XPathDocumentElementChildIterator : XPathDocumentBaseIterator
  {
    private string localName, namespaceUri;

    /// <summary>
    /// Create an iterator that ranges over all element children of "parent" having the specified QName.
    /// </summary>
    public XPathDocumentElementChildIterator(XPathDocumentNavigator parent, string name, string namespaceURI)
      : base(parent)
    {
      if (namespaceURI == null) throw new ArgumentNullException("namespaceURI");

      this.localName = parent.NameTable.Get(name);
      this.namespaceUri = namespaceURI;
    }

    /// <summary>
    /// Create a new iterator that is a copy of "iter".
    /// </summary>
    public XPathDocumentElementChildIterator(XPathDocumentElementChildIterator iter)
      : base(iter)
    {
      this.localName = iter.localName;
      this.namespaceUri = iter.namespaceUri;
    }

    /// <summary>
    /// Create a copy of this iterator.
    /// </summary>
    public override XPathNodeIterator Clone()
    {
      return new XPathDocumentElementChildIterator(this);
    }

    /// <summary>
    /// Position the iterator to the next matching child.
    /// </summary>
    public override bool MoveNext()
    {
      if (this.pos == 0)
      {
        if (!this.ctxt.MoveToChild(this.localName, this.namespaceUri))
          return false;
      }
      else
      {
        if (!this.ctxt.MoveToNext(this.localName, this.namespaceUri))
          return false;
      }

      this.pos++;
      return true;
    }
  }
}
