using System.Collections.Generic;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Base class for all internal node. Note that only sequence and choice have right child
  /// </summary>
  abstract class InteriorNode : SyntaxTreeNode
  {
    SyntaxTreeNode leftChild;
    SyntaxTreeNode rightChild;

    public SyntaxTreeNode LeftChild
    {
      get { return leftChild; }
      set { leftChild = value; }
    }

    public SyntaxTreeNode RightChild
    {
      get { return rightChild; }
      set { rightChild = value; }
    }

    public override SyntaxTreeNode Clone(Positions positions)
    {
      InteriorNode other = (InteriorNode)this.MemberwiseClone();
      other.LeftChild = leftChild.Clone(positions);
      if (rightChild != null)
      {
        other.RightChild = rightChild.Clone(positions);
      }
      return other;
    }

    //no recursive version of expand tree for Sequence and Choice node
    protected void ExpandTreeNoRecursive(InteriorNode parent, SymbolsDictionary symbols, Positions positions)
    {
      Stack<InteriorNode> nodeStack = new Stack<InteriorNode>();
      InteriorNode this_ = this;
      while (true)
      {
        if (this_.leftChild is ChoiceNode || this_.leftChild is SequenceNode)
        {
          nodeStack.Push(this_);
          this_ = (InteriorNode)this_.leftChild;
          continue;
        }
        this_.leftChild.ExpandTree(this_, symbols, positions);

        ProcessRight:
        if (this_.rightChild != null)
        {
          this_.rightChild.ExpandTree(this_, symbols, positions);
        }

        if (nodeStack.Count == 0)
          break;

        this_ = nodeStack.Pop();
        goto ProcessRight;
      }
    }

    public override void ExpandTree(InteriorNode parent, SymbolsDictionary symbols, Positions positions)
    {
      leftChild.ExpandTree(this, symbols, positions);
      if (rightChild != null)
      {
        rightChild.ExpandTree(this, symbols, positions);
      }
    }
  }
}