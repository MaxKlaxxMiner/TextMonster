namespace TextMonster.Xml.Xml_Reader
{
  internal class KsStruct
  {
    public int depth;                       // depth of selector when it matches
    public KeySequence ks;                  // ks of selector when it matches and assigned -- needs to new each time
    public LocatedActiveAxis[] fields;      // array of fields activeaxis when it matches and assigned

    public KsStruct(KeySequence ks, int dim)
    {
      this.ks = ks;
      this.fields = new LocatedActiveAxis[dim];
    }
  }
}
