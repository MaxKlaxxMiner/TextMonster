namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class DescendantOverDescendantQuery : DescendantBaseQuery
  {
    private int level = 0;

    public DescendantOverDescendantQuery(Query qyParent, bool matchSelf, string name, string prefix, XPathNodeType typeTest, bool abbrAxis) :
      base(qyParent, name, prefix, typeTest, matchSelf, abbrAxis) { }
    private DescendantOverDescendantQuery(DescendantOverDescendantQuery other)
      : base(other)
    {
      this.level = other.level;
    }

    public override void Reset()
    {
      level = 0;
      base.Reset();
    }

    public override XPathNavigator Advance()
    {
      while (true)
      {
        if (level == 0)
        {
          currentNode = qyInput.Advance();
          position = 0;
          if (currentNode == null)
          {
            return null;
          }
          if (matchSelf && matches(currentNode))
          {
            position = 1;
            return currentNode;
          }
          currentNode = currentNode.Clone();
          if (!MoveToFirstChild())
          {
            continue;
          }
        }
        else
        {
          if (!MoveUpUntillNext())
          {
            continue;
          }
        }
        do
        {
          if (matches(currentNode))
          {
            position++;
            return currentNode;
          }
        } while (MoveToFirstChild());
      }
    }

    private bool MoveToFirstChild()
    {
      if (currentNode.MoveToFirstChild())
      {
        level++;
        return true;
      }
      return false;
    }

    private bool MoveUpUntillNext()
    { // move up untill we can move next
      while (!currentNode.MoveToNext())
      {
        --level;
        if (level == 0)
        {
          return false;
        }
        bool result = currentNode.MoveToParent();
      }
      return true;
    }

    public override XPathNodeIterator Clone() { return new DescendantOverDescendantQuery(this); }
  }
}