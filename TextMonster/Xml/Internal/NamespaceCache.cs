
namespace TextMonster.Xml
{
  internal struct NamespaceCache
  {
    X_Namespace ns;
    string namespaceName;

    public X_Namespace Get(string namespaceName)
    {
      if (namespaceName == this.namespaceName)
        return ns;
      this.namespaceName = namespaceName;
      ns = X_Namespace.Get(namespaceName);
      return ns;
    }
  }
}
