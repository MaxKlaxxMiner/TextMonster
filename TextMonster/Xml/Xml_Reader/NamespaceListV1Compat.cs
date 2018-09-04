namespace TextMonster.Xml.Xml_Reader
{
  internal class NamespaceListV1Compat : NamespaceList
  {
    public NamespaceListV1Compat(string namespaces, string targetNamespace) : base(namespaces, targetNamespace) { }

    public override bool Allows(string ns)
    {
      if (this.Type == ListType.Other)
      {
        return ns != Excluded;
      }
      else
      {
        return base.Allows(ns);
      }
    }
  }
}
