
namespace TextMonster.Xml
{
  internal struct NamespaceResolver
  {
    private int scope;
    private NamespaceResolver.NamespaceDeclaration declaration;
    private NamespaceResolver.NamespaceDeclaration rover;

    public void PushScope()
    {
      this.scope = this.scope + 1;
    }

    public void PopScope()
    {
      NamespaceResolver.NamespaceDeclaration namespaceDeclaration = this.declaration;
      if (namespaceDeclaration != null)
      {
        do
        {
          namespaceDeclaration = namespaceDeclaration.prev;
          if (namespaceDeclaration.scope == this.scope)
          {
            if (namespaceDeclaration == this.declaration)
              this.declaration = (NamespaceResolver.NamespaceDeclaration)null;
            else
              this.declaration.prev = namespaceDeclaration.prev;
            this.rover = (NamespaceResolver.NamespaceDeclaration)null;
          }
          else
            break;
        }
        while (namespaceDeclaration != this.declaration && this.declaration != null);
      }
      this.scope = this.scope - 1;
    }

    public void Add(string prefix, XNamespace ns)
    {
      NamespaceResolver.NamespaceDeclaration namespaceDeclaration = new NamespaceResolver.NamespaceDeclaration();
      namespaceDeclaration.prefix = prefix;
      namespaceDeclaration.ns = ns;
      namespaceDeclaration.scope = this.scope;
      if (this.declaration == null)
        this.declaration = namespaceDeclaration;
      else
        namespaceDeclaration.prev = this.declaration.prev;
      this.declaration.prev = namespaceDeclaration;
      this.rover = (NamespaceResolver.NamespaceDeclaration)null;
    }

    public void AddFirst(string prefix, XNamespace ns)
    {
      NamespaceResolver.NamespaceDeclaration namespaceDeclaration = new NamespaceResolver.NamespaceDeclaration();
      namespaceDeclaration.prefix = prefix;
      namespaceDeclaration.ns = ns;
      namespaceDeclaration.scope = this.scope;
      if (this.declaration == null)
      {
        namespaceDeclaration.prev = namespaceDeclaration;
      }
      else
      {
        namespaceDeclaration.prev = this.declaration.prev;
        this.declaration.prev = namespaceDeclaration;
      }
      this.declaration = namespaceDeclaration;
      this.rover = (NamespaceResolver.NamespaceDeclaration)null;
    }

    public string GetPrefixOfNamespace(XNamespace ns, bool allowDefaultNamespace)
    {
      if (this.rover != null && this.rover.ns == ns && (allowDefaultNamespace || this.rover.prefix.Length > 0))
        return this.rover.prefix;
      NamespaceResolver.NamespaceDeclaration namespaceDeclaration1 = this.declaration;
      if (namespaceDeclaration1 != null)
      {
        do
        {
          namespaceDeclaration1 = namespaceDeclaration1.prev;
          if (namespaceDeclaration1.ns == ns)
          {
            NamespaceResolver.NamespaceDeclaration namespaceDeclaration2 = this.declaration.prev;
            while (namespaceDeclaration2 != namespaceDeclaration1 && namespaceDeclaration2.prefix != namespaceDeclaration1.prefix)
              namespaceDeclaration2 = namespaceDeclaration2.prev;
            if (namespaceDeclaration2 == namespaceDeclaration1)
            {
              if (allowDefaultNamespace)
              {
                this.rover = namespaceDeclaration1;
                return namespaceDeclaration1.prefix;
              }
              if (namespaceDeclaration1.prefix.Length > 0)
                return namespaceDeclaration1.prefix;
            }
          }
        }
        while (namespaceDeclaration1 != this.declaration);
      }
      return (string)null;
    }

    private class NamespaceDeclaration
    {
      public string prefix;
      public XNamespace ns;
      public int scope;
      public NamespaceResolver.NamespaceDeclaration prev;
    }
  }
}
