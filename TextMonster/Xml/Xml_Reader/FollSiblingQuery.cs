using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  internal sealed class FollSiblingQuery : BaseAxisQuery
  {
    ClonableStack<XPathNavigator> elementStk;
    List<XPathNavigator> parentStk;
    XPathNavigator nextInput;

    public FollSiblingQuery(Query qyInput, string name, string prefix, XPathNodeType type)
      : base(qyInput, name, prefix, type)
    {
      this.elementStk = new ClonableStack<XPathNavigator>();
      this.parentStk = new List<XPathNavigator>();
    }
    private FollSiblingQuery(FollSiblingQuery other)
      : base(other)
    {
      this.elementStk = other.elementStk.Clone();
      this.parentStk = new List<XPathNavigator>(other.parentStk);
      this.nextInput = Clone(other.nextInput);
    }

    public override void Reset()
    {
      elementStk.Clear();
      parentStk.Clear();
      nextInput = null;
      base.Reset();
    }

    private bool Visited(XPathNavigator nav)
    {
      XPathNavigator parent = nav.Clone();
      parent.MoveToParent();
      for (int i = 0; i < parentStk.Count; i++)
      {
        if (parent.IsSamePosition(parentStk[i]))
        {
          return true;
        }
      }
      parentStk.Add(parent);
      return false;
    }

    private XPathNavigator FetchInput()
    {
      XPathNavigator input;
      do
      {
        input = qyInput.Advance();
        if (input == null)
        {
          return null;
        }
      } while (Visited(input));
      return input.Clone();
    }

    public override XPathNavigator Advance()
    {
      while (true)
      {
        if (currentNode == null)
        {
          if (nextInput == null)
          {
            nextInput = FetchInput(); // This can happen at the begining and at the end 
          }
          if (elementStk.Count == 0)
          {
            if (nextInput == null)
            {
              return null;
            }
            currentNode = nextInput;
            nextInput = FetchInput();
          }
          else
          {
            currentNode = elementStk.Pop();
          }
        }

        while (currentNode.IsDescendant(nextInput))
        {
          elementStk.Push(currentNode);
          currentNode = nextInput;
          nextInput = qyInput.Advance();
          if (nextInput != null)
          {
            nextInput = nextInput.Clone();
          }
        }

        while (currentNode.MoveToNext())
        {
          if (matches(currentNode))
          {
            position++;
            return currentNode;
          }
        }
        currentNode = null;
      }
    } // Advance

    public override XPathNodeIterator Clone() { return new FollSiblingQuery(this); }
  }
}