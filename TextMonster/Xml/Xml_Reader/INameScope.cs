namespace TextMonster.Xml.Xml_Reader
{
  internal interface INameScope
  {
    object this[string name, string ns] { get; set; }
  }
}