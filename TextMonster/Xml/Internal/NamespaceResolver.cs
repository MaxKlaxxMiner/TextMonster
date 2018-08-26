
namespace TextMonster.Xml
{
  internal struct NamespaceResolver
  {
    int scope;
    NamespaceDeclaration declaration;
    NamespaceDeclaration rover;

    public void PushScope()
    {
      scope = scope + 1;
    }

    public void PopScope()
    {
      var namespaceDeclaration = declaration;
      if (namespaceDeclaration != null)
      {
        do
        {
          namespaceDeclaration = namespaceDeclaration.prev;
          if (namespaceDeclaration.scope == scope)
          {
            if (namespaceDeclaration == declaration)
              declaration = null;
            else
              declaration.prev = namespaceDeclaration.prev;
            rover = null;
          }
          else
            break;
        }
        while (namespaceDeclaration != declaration && declaration != null);
      }
      scope = scope - 1;
    }

    public void Add(string prefix, X_Namespace ns)
    {
      var namespaceDeclaration = new NamespaceDeclaration
      {
        prefix = prefix,
        ns = ns,
        scope = scope
      };
      if (declaration == null)
        declaration = namespaceDeclaration;
      else
        namespaceDeclaration.prev = declaration.prev;
      declaration.prev = namespaceDeclaration;
      rover = null;
    }

    public void AddFirst(string prefix, X_Namespace ns)
    {
      var namespaceDeclaration = new NamespaceDeclaration
      {
        prefix = prefix,
        ns = ns,
        scope = scope
      };
      if (declaration == null)
      {
        namespaceDeclaration.prev = namespaceDeclaration;
      }
      else
      {
        namespaceDeclaration.prev = declaration.prev;
        declaration.prev = namespaceDeclaration;
      }
      declaration = namespaceDeclaration;
      rover = null;
    }

    public string GetPrefixOfNamespace(X_Namespace ns, bool allowDefaultNamespace)
    {
      if (rover != null && rover.ns == ns && (allowDefaultNamespace || rover.prefix.Length > 0))
        return rover.prefix;
      var namespaceDeclaration1 = declaration;
      if (namespaceDeclaration1 != null)
      {
        do
        {
          namespaceDeclaration1 = namespaceDeclaration1.prev;
          if (namespaceDeclaration1.ns == ns)
          {
            var namespaceDeclaration2 = declaration.prev;
            while (namespaceDeclaration2 != namespaceDeclaration1 && namespaceDeclaration2.prefix != namespaceDeclaration1.prefix)
              namespaceDeclaration2 = namespaceDeclaration2.prev;
            if (namespaceDeclaration2 == namespaceDeclaration1)
            {
              if (allowDefaultNamespace)
              {
                rover = namespaceDeclaration1;
                return namespaceDeclaration1.prefix;
              }
              if (namespaceDeclaration1.prefix.Length > 0)
                return namespaceDeclaration1.prefix;
            }
          }
        }
        while (namespaceDeclaration1 != declaration);
      }
      return null;
    }

    sealed class NamespaceDeclaration
    {
      public string prefix;
      public X_Namespace ns;
      public int scope;
      public NamespaceDeclaration prev;
    }
  }
}
