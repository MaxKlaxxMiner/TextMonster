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
      set { name = value; }
    }

    internal long Value
    {
      set { this.value = value; }
    }
  }
}