namespace TextMonster.Xml.Xml_Reader
{
  internal class SpecialMapping : TypeMapping
  {
    bool namedAny;

    internal bool NamedAny
    {
      get { return namedAny; }
      set { namedAny = value; }
    }
  }
}