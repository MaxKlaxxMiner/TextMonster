namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class AbsoluteQuery : ContextQuery
  {
    public AbsoluteQuery() : base() { }
    private AbsoluteQuery(AbsoluteQuery other) : base(other) { }

    public override object Evaluate(XPathNodeIterator context)
    {
      base.contextNode = context.Current.Clone();
      base.contextNode.MoveToRoot();
      count = 0;
      return this;
    }

    public override XPathNavigator MatchNode(XPathNavigator context)
    {
      if (context != null && context.NodeType == XPathNodeType.Root)
      {
        return context;
      }
      return null;
    }

    public override XPathNodeIterator Clone() { return new AbsoluteQuery(this); }
  }
}
