namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class DocumentXPathNodeIterator_ElemChildren_AndSelf_NoLocalName : DocumentXPathNodeIterator_ElemChildren_NoLocalName
  {

    internal DocumentXPathNodeIterator_ElemChildren_AndSelf_NoLocalName(DocumentXPathNavigator nav, string nsAtom)
      : base(nav, nsAtom)
    {
    }
    internal DocumentXPathNodeIterator_ElemChildren_AndSelf_NoLocalName(DocumentXPathNodeIterator_ElemChildren_AndSelf_NoLocalName other)
      : base(other)
    {
    }

    public override XPathNodeIterator Clone()
    {
      return new DocumentXPathNodeIterator_ElemChildren_AndSelf_NoLocalName(this);
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