namespace TextMonster.Xml.Xml_Reader
{
  class TempAssemblyCacheKey
  {
    string ns;
    object type;

    internal TempAssemblyCacheKey(string ns, object type)
    {
      this.type = type;
      this.ns = ns;
    }

    public override bool Equals(object o)
    {
      TempAssemblyCacheKey key = o as TempAssemblyCacheKey;
      if (key == null) return false;
      return (key.type == this.type && key.ns == this.ns);
    }

    public override int GetHashCode()
    {
      return ((ns != null ? ns.GetHashCode() : 0) ^ (type != null ? type.GetHashCode() : 0));
    }
  }
}