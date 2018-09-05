namespace TextMonster.Xml.Xml_Reader
{
  internal class DocumentXPathNodeIterator_ElemChildren_NoLocalName : DocumentXPathNodeIterator_ElemDescendants
  {
    private string nsAtom;

    internal DocumentXPathNodeIterator_ElemChildren_NoLocalName(DocumentXPathNavigator nav, string nsAtom)
      : base(nav)
    {
      this.nsAtom = nsAtom;
    }
    internal DocumentXPathNodeIterator_ElemChildren_NoLocalName(DocumentXPathNodeIterator_ElemChildren_NoLocalName other)
      : base(other)
    {
      this.nsAtom = other.nsAtom;
    }
    public override XPathNodeIterator Clone()
    {
      return new DocumentXPathNodeIterator_ElemChildren_NoLocalName(this);
    }

    protected override bool Match(XmlNode node)
    {
      return Ref.Equal(node.NamespaceURI, nsAtom);
    }
  }
}