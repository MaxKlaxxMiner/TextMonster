
namespace TextMonster.Xml
{
  internal struct NamespaceCache
  {
    private XNamespace ns;
    private string namespaceName;

    public XNamespace Get(string namespaceName)
    {
      if (namespaceName == this.namespaceName)
        return this.ns;
      this.namespaceName = namespaceName;
      this.ns = XNamespace.Get(namespaceName);
      return this.ns;
    }
  }
}
