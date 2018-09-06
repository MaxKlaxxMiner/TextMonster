namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Using range node as one of the terminals
  /// </summary>
  sealed class LeafRangeNode : LeafNode
  {
    decimal min;
    decimal max;
    BitSet nextIteration;

    public LeafRangeNode(int pos, decimal min, decimal max)
      : base(pos)
    {
      this.min = min;
      this.max = max;
    }

    public decimal Max
    {
      get { return max; }
    }

    public decimal Min
    {
      get { return min; }
    }

    public BitSet NextIteration
    {
      get
      {
        return nextIteration;
      }
      set
      {
        nextIteration = value;
      }
    }

    public override SyntaxTreeNode Clone(Positions positions)
    {
      return new LeafRangeNode(this.Pos, this.min, this.max);
    }

    public override bool IsRangeNode
    {
      get
      {
        return true;
      }
    }

    public override void ExpandTree(InteriorNode parent, SymbolsDictionary symbols, Positions positions)
    {
      //change the range node min to zero if left is nullable
      if (parent.LeftChild.IsNullable)
      {
        min = 0;
      }
    }
  }
}
