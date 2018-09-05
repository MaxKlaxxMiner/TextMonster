namespace TextMonster.Xml.Xml_Reader
{
  sealed class ChoiceNode : InteriorNode
  {

    private static void ConstructChildPos(SyntaxTreeNode child, BitSet firstpos, BitSet lastpos, BitSet[] followpos)
    {
      BitSet firstPosTemp = new BitSet(firstpos.Count);
      BitSet lastPosTemp = new BitSet(lastpos.Count);
      child.ConstructPos(firstPosTemp, lastPosTemp, followpos);
      firstpos.Or(firstPosTemp);
      lastpos.Or(lastPosTemp);
    }

    public override void ConstructPos(BitSet firstpos, BitSet lastpos, BitSet[] followpos)
    {

      BitSet firstPosTemp = new BitSet(firstpos.Count);
      BitSet lastPosTemp = new BitSet(lastpos.Count);
      SyntaxTreeNode n;
      ChoiceNode this_ = this;
      do
      {
        ConstructChildPos(this_.RightChild, firstPosTemp, lastPosTemp, followpos);
        n = this_.LeftChild;
        this_ = n as ChoiceNode;
      } while (this_ != null);

      n.ConstructPos(firstpos, lastpos, followpos);
      firstpos.Or(firstPosTemp);
      lastpos.Or(lastPosTemp);
    }

    public override bool IsNullable
    {
      get
      {
        SyntaxTreeNode n;
        ChoiceNode this_ = this;
        do
        {
          if (this_.RightChild.IsNullable)
            return true;
          n = this_.LeftChild;
          this_ = n as ChoiceNode;
        }
        while (this_ != null);
        return n.IsNullable;
      }
    }

    public override void ExpandTree(InteriorNode parent, SymbolsDictionary symbols, Positions positions)
    {
      ExpandTreeNoRecursive(parent, symbols, positions);
    }
  }
}