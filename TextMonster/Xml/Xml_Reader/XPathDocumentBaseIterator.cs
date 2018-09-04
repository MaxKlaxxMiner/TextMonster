namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Base internal class of all XPathDocument XPathNodeIterator implementations.
  /// </summary>
  internal abstract class XPathDocumentBaseIterator : XPathNodeIterator
  {
    protected XPathDocumentNavigator ctxt;
    protected int pos;

    /// <summary>
    /// Create a new iterator that is initially positioned on the "ctxt" node.
    /// </summary>
    protected XPathDocumentBaseIterator(XPathDocumentNavigator ctxt)
    {
      this.ctxt = new XPathDocumentNavigator(ctxt);
    }

    /// <summary>
    /// Create a new iterator that is a copy of "iter".
    /// </summary>
    protected XPathDocumentBaseIterator(XPathDocumentBaseIterator iter)
    {
      this.ctxt = new XPathDocumentNavigator(iter.ctxt);
      this.pos = iter.pos;
    }

    /// <summary>
    /// Return the current navigator.
    /// </summary>
    public override XPathNavigator Current
    {
      get { return this.ctxt; }
    }

    /// <summary>
    /// Return the iterator's current position.
    /// </summary>
    public override int CurrentPosition
    {
      get { return this.pos; }
    }
  }
}
