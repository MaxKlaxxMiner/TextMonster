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
  }
}