
namespace TextMonster.Xml
{
  internal struct NamespaceCache
  {
    XNamespace ns;
    string namespaceName;

    public XNamespace Get(string namespaceName)
    {
      if (namespaceName == this.namespaceName)
        return ns;
      this.namespaceName = namespaceName;
      ns = XNamespace.Get(namespaceName);
      return ns;
    }
  }
}
