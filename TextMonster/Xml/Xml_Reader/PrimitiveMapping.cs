namespace TextMonster.Xml.Xml_Reader
{
  internal class PrimitiveMapping : TypeMapping
  {
    bool isList;

    internal override bool IsList
    {
      get { return isList; }
      set { isList = value; }
    }
  }
}