namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class DocumentXPathNodeIterator_ElemChildren_AndSelf : DocumentXPathNodeIterator_ElemChildren
  {

    internal DocumentXPathNodeIterator_ElemChildren_AndSelf(DocumentXPathNavigator nav, string localNameAtom, string nsAtom)
      : base(nav, localNameAtom, nsAtom)
    {
    }
    internal DocumentXPathNodeIterator_ElemChildren_AndSelf(DocumentXPathNodeIterator_ElemChildren_AndSelf other)
      : base(other)
    {
    }

    public override XPathNodeIterator Clone()
    {
      return new DocumentXPathNodeIterator_ElemChildren_AndSelf(this);
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