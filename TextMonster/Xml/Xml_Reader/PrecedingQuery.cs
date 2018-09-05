namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class PrecedingQuery : BaseAxisQuery
  {
    private XPathNodeIterator workIterator;
    private ClonableStack<XPathNavigator> ancestorStk;

    public PrecedingQuery(Query qyInput, string name, string prefix, XPathNodeType typeTest)
      : base(qyInput, name, prefix, typeTest)
    {
      ancestorStk = new ClonableStack<XPathNavigator>();
    }
    private PrecedingQuery(PrecedingQuery other)
      : base(other)
    {
      this.workIterator = Clone(other.workIterator);
      this.ancestorStk = other.ancestorStk.Clone();
    }

    public override void Reset()
    {
      workIterator = null;
      ancestorStk.Clear();
      base.Reset();
    }

    public override XPathNavigator Advance()
    {
      if (workIterator == null)
      {
        XPathNavigator last;
        {
          XPathNavigator input = qyInput.Advance();
          if (input == null)
          {
            return null;
          }
          last = input.Clone();
          do
          {
            last.MoveTo(input);
          } while ((input = qyInput.Advance()) != null);

          if (last.NodeType == XPathNodeType.Attribute || last.NodeType == XPathNodeType.Namespace)
          {
            last.MoveToParent();
          }
        }
        // Fill ancestorStk :
        do
        {
          ancestorStk.Push(last.Clone());
        } while (last.MoveToParent());
        // Create workIterator :
        // last.MoveToRoot(); We are on root already
        workIterator = last.SelectDescendants(XPathNodeType.All, true);
      }

      while (workIterator.MoveNext())
      {
        currentNode = workIterator.Current;
        if (currentNode.IsSamePosition(ancestorStk.Peek()))
        {
          ancestorStk.Pop();
          if (ancestorStk.Count == 0)
          {
            currentNode = null;
            workIterator = null;
            return null;
          }
          continue;
        }
        if (matches(currentNode))
        {
          position++;
          return currentNode;
        }
      }
      return null;
    }

    public override XPathNodeIterator Clone() { return new PrecedingQuery(this); }
    public override QueryProps Properties { get { return base.Properties | QueryProps.Reverse; } }
  }
}