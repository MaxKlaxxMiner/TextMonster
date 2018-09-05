namespace TextMonster.Xml.Xml_Reader
{
  struct Position
  {
    public int symbol;
    public object particle;
    public Position(int symbol, object particle)
    {
      this.symbol = symbol;
      this.particle = particle;
    }
  }
}
