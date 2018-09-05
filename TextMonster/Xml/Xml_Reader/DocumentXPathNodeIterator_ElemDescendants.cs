namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class DocumentXPathNodeIterator_ElemDescendants : XPathNodeIterator
  {
    private DocumentXPathNavigator nav;
    private int level;
    private int position;

    internal DocumentXPathNodeIterator_ElemDescendants(DocumentXPathNavigator nav)
    {
      this.nav = (DocumentXPathNavigator)(nav.Clone());
      this.level = 0;
      this.position = 0;
    }
    internal DocumentXPathNodeIterator_ElemDescendants(DocumentXPathNodeIterator_ElemDescendants other)
    {
      this.nav = (DocumentXPathNavigator)(other.nav.Clone());
      this.level = other.level;
      this.position = other.position;
    }

    protected abstract bool Match(XmlNode node);

    public override XPathNavigator Current
    {
      get { return nav; }
    }

    public override int CurrentPosition
    {
      get { return position; }
    }

    protected void SetPosition(int pos)
    {
      position = pos;
    }

    public override bool MoveNext()
    {
      for (; ; )
      {
        if (nav.MoveToFirstChild())
        {
          level++;
        }
        else
        {
          if (level == 0)
          {
            return false;
          }
          while (!nav.MoveToNext())
          {
            level--;
            if (level == 0)
            {
              return false;
            }
            if (!nav.MoveToParent())
            {
              return false;
            }
          }
        }
        XmlNode node = (XmlNode)nav.UnderlyingObject;
        if (node.NodeType == XmlNodeType.Element && Match(node))
        {
          position++;
          return true;
        }
      }
    }
  }
}