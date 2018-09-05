using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  internal class PreSiblingQuery : CacheAxisQuery
  {

    public PreSiblingQuery(Query qyInput, string name, string prefix, XPathNodeType typeTest) : base(qyInput, name, prefix, typeTest) { }
    protected PreSiblingQuery(PreSiblingQuery other) : base(other) { }

    private bool NotVisited(XPathNavigator nav, List<XPathNavigator> parentStk)
    {
      XPathNavigator nav1 = nav.Clone();
      nav1.MoveToParent();
      for (int i = 0; i < parentStk.Count; i++)
      {
        if (nav1.IsSamePosition(parentStk[i]))
        {
          return false;
        }
      }
      parentStk.Add(nav1);
      return true;
    }

    public override object Evaluate(XPathNodeIterator context)
    {
      base.Evaluate(context);

      // Fill up base.outputBuffer
      List<XPathNavigator> parentStk = new List<XPathNavigator>();
      Stack<XPathNavigator> inputStk = new Stack<XPathNavigator>();
      while ((currentNode = qyInput.Advance()) != null)
      {
        inputStk.Push(currentNode.Clone());
      }
      while (inputStk.Count != 0)
      {
        XPathNavigator input = inputStk.Pop();
        if (input.NodeType == XPathNodeType.Attribute || input.NodeType == XPathNodeType.Namespace)
        {
          continue;
        }
        if (NotVisited(input, parentStk))
        {
          XPathNavigator prev = input.Clone();
          if (prev.MoveToParent())
          {
            bool test = prev.MoveToFirstChild();
            while (!prev.IsSamePosition(input))
            {
              if (matches(prev))
              {
                Insert(outputBuffer, prev);
              }
              if (!prev.MoveToNext())
              {
                break;
              }
            }
          }
        }
      }
      return this;
    }

    public override XPathNodeIterator Clone() { return new PreSiblingQuery(this); }
    public override QueryProps Properties { get { return base.Properties | QueryProps.Reverse; } }
  }
}