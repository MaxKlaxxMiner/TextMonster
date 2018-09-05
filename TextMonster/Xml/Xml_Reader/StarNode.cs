namespace TextMonster.Xml.Xml_Reader
{
  sealed class StarNode : InteriorNode
  {
    public override void ConstructPos(BitSet firstpos, BitSet lastpos, BitSet[] followpos)
    {
      LeftChild.ConstructPos(firstpos, lastpos, followpos);
      for (int pos = lastpos.NextSet(-1); pos != -1; pos = lastpos.NextSet(pos))
      {
        followpos[pos].Or(firstpos);
      }
    }

    public override bool IsNullable
    {
      get { return true; }
    }
  }
}