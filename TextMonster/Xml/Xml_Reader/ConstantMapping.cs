namespace TextMonster.Xml.Xml_Reader
{
  internal class ConstantMapping : Mapping
  {
    string xmlName;
    string name;
    long value;

    internal string XmlName
    {
      get { return xmlName == null ? string.Empty : xmlName; }
      set { xmlName = value; }
    }

    internal string Name
    {
      get { return name == null ? string.Empty : name; }
      set { this.name = value; }
    }

    internal long Value
    {
      get { return value; }
      set { this.value = value; }
    }
  }
}