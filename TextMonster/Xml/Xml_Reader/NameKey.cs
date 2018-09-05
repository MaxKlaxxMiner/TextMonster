namespace TextMonster.Xml.Xml_Reader
{
  internal class NameKey
  {
    string ns;
    string name;

    internal NameKey(string name, string ns)
    {
      this.name = name;
      this.ns = ns;
    }

    public override bool Equals(object other)
    {
      if (!(other is NameKey)) return false;
      NameKey key = (NameKey)other;
      return name == key.name && ns == key.ns;
    }

    public override int GetHashCode()
    {
      return (ns == null ? "<null>".GetHashCode() : ns.GetHashCode()) ^ (name == null ? 0 : name.GetHashCode());
    }
  }
}