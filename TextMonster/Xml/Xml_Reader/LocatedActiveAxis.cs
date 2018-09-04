namespace TextMonster.Xml.Xml_Reader
{
  // ActiveAxis plus the location plus the state of matching in the constraint table : only for field
  internal class LocatedActiveAxis : ActiveAxis
  {
    private int column;                     // the column in the table (the field sequence)
    internal bool isMatched;                  // if it's matched, then fill value in the validator later
    internal KeySequence Ks;                        // associated with a keysequence it will fills in

    internal int Column
    {
      get { return this.column; }
    }

    internal LocatedActiveAxis(Asttree astfield, KeySequence ks, int column)
      : base(astfield)
    {
      this.Ks = ks;
      this.column = column;
      this.isMatched = false;
    }

    internal void Reactivate(KeySequence ks)
    {
      Reactivate();
      this.Ks = ks;
    }

  }
}
