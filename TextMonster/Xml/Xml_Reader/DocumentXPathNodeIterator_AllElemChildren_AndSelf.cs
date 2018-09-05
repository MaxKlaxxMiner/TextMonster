namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class DocumentXPathNodeIterator_AllElemChildren_AndSelf : DocumentXPathNodeIterator_AllElemChildren
  {
    internal DocumentXPathNodeIterator_AllElemChildren_AndSelf(DocumentXPathNavigator nav)
      : base(nav)
    {
    }
    internal DocumentXPathNodeIterator_AllElemChildren_AndSelf(DocumentXPathNodeIterator_AllElemChildren_AndSelf other)
      : base(other)
    {
    }

    public override XPathNodeIterator Clone()
    {
      return new DocumentXPathNodeIterator_AllElemChildren_AndSelf(this);
    }

    public override bool MoveNext()
    {
      if (CurrentPosition == 0)
      {
        DocumentXPathNavigator nav = (DocumentXPathNavigator)this.Current;
        XmlNode node = (XmlNode)nav.UnderlyingObject;
        if (node.NodeType == XmlNodeType.Element && Match(node))
        {
          SetPosition(1);
          return true;
        }
      }
      return base.MoveNext();
    }
  }
}