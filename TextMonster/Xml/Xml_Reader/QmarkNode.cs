namespace TextMonster.Xml.Xml_Reader
{
  sealed class QmarkNode : InteriorNode
  {
    public override void ConstructPos(BitSet firstpos, BitSet lastpos, BitSet[] followpos)
    {
      LeftChild.ConstructPos(firstpos, lastpos, followpos);
    }

    public override bool IsNullable
    {
      get { return true; }
    }

#if DEBUG
        public override void Dump(StringBuilder bb, SymbolsDictionary symbols, Positions positions) {
            LeftChild.Dump(bb, symbols, positions);
            bb.Append("?");
        }
#endif
  }
}