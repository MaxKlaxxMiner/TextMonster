namespace TextMonster.Xml.Xml_Reader
{
  internal class NullableMapping : TypeMapping
  {
    TypeMapping baseMapping;

    internal TypeMapping BaseMapping
    {
      get { return baseMapping; }
    }

    internal override string DefaultElementName
    {
      get { return BaseMapping.DefaultElementName; }
    }
  }
}