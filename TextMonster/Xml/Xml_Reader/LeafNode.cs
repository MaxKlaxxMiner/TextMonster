namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// Terminal of the syntax tree
  /// </summary>
  class LeafNode : SyntaxTreeNode
  {
    int pos;

    public LeafNode(int pos)
    {
      this.pos = pos;
    }

    public int Pos
    {
      get { return pos; }
      set { pos = value; }
    }

    public override void ExpandTree(InteriorNode parent, SymbolsDictionary symbols, Positions positions)
    {
      // do nothing
    }

    public override SyntaxTreeNode Clone(Positions positions)
    {
      return new LeafNode(positions.Add(positions[pos].symbol, positions[pos].particle));
    }

    public override void ConstructPos(BitSet firstpos, BitSet lastpos, BitSet[] followpos)
    {
      firstpos.Set(pos);
      lastpos.Set(pos);
    }

    public override bool IsNullable
    {
      get { return false; }
    }

#if DEBUG
        public override void Dump(StringBuilder bb, SymbolsDictionary symbols, Positions positions) {
            bb.Append("\"" + symbols.NameOf(positions[pos].symbol) + "\"");
        }
#endif
  }
}