namespace TextMonster.Xml.Xml_Reader
{
  internal abstract class Mapping
  {
    bool isSoap;

    internal Mapping() { }

    protected Mapping(Mapping mapping)
    {
      this.isSoap = mapping.isSoap;
    }
  }
}