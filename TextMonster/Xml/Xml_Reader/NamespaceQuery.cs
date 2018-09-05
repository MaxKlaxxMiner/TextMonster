namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class NamespaceQuery : BaseAxisQuery
  {
    private bool onNamespace;

    public NamespaceQuery(Query qyParent, string Name, string Prefix, XPathNodeType Type) : base(qyParent, Name, Prefix, Type) { }
    private NamespaceQuery(NamespaceQuery other)
      : base(other)
    {
      this.onNamespace = other.onNamespace;
    }

    public override void Reset()
    {
      onNamespace = false;
      base.Reset();
    }

    public override XPathNavigator Advance()
    {
      while (true)
      {
        if (!onNamespace)
        {
          currentNode = qyInput.Advance();
          if (currentNode == null)
          {
            return null;
          }
          position = 0;
          currentNode = currentNode.Clone();
          onNamespace = currentNode.MoveToFirstNamespace();
        }
        else
        {
          onNamespace = currentNode.MoveToNextNamespace();
        }

        if (onNamespace)
        {
          if (matches(currentNode))
          {
            position++;
            return currentNode;
          }
        }
      } // while
    } // Advance

    public override bool matches(XPathNavigator e)
    {
      if (e.Value.Length == 0)
      {
        return false;
      }
      if (NameTest)
      {
        return Name.Equals(e.LocalName);
      }
      else
      {
        return true;
      }
    }

    public override XPathNodeIterator Clone() { return new NamespaceQuery(this); }
  }
}