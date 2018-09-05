using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  /// <summary>
  /// SymbolsDictionary is a map between names that ContextValidator recognizes and symbols - int symbol[XmlQualifiedName name].
  /// There are two types of name - full names and wildcards (namespace is specified, local name is anythig).
  /// Wildcard excludes all full names that would match by the namespace part.
  /// SymbolsDictionry alwas recognizes all the symbols - the last one is a true wildcard - 
  ///      both name and namespace can be anything that none of the other symbols matched.
  /// </summary>
  class SymbolsDictionary
  {
    int last = 0;
    Hashtable names;
    Hashtable wildcards = null;
    ArrayList particles;
    object particleLast = null;
    bool isUpaEnforced = true;

    public SymbolsDictionary()
    {
      names = new Hashtable();
      particles = new ArrayList();
    }

    public int Count
    {
      // last one is a "*:*" any wildcard
      get { return last + 1; }
    }

    /// <summary>
    /// True is particle can be deterministically attributed from the symbol and conversion to DFA is possible.
    /// </summary>
    public bool IsUpaEnforced
    {
      get { return isUpaEnforced; }
      set { isUpaEnforced = value; }
    }

    /// <summary>
    /// Add name  and return it's number
    /// </summary>
    public int AddName(XmlQualifiedName name, object particle)
    {
      object lookup = names[name];
      if (lookup != null)
      {
        int symbol = (int)lookup;
        if (particles[symbol] != particle)
        {
          isUpaEnforced = false;
        }
        return symbol;
      }
      else
      {
        names.Add(name, last);
        particles.Add(particle);
        return last++;
      }
    }

    public void AddNamespaceList(NamespaceList list, object particle, bool allowLocal)
    {
      switch (list.Type)
      {
        case NamespaceList.ListType.Any:
        particleLast = particle;
        break;
        case NamespaceList.ListType.Other:
        // Create a symbol for the excluded namespace, but don't set a particle for it.
        AddWildcard(list.Excluded, null);
        if (!allowLocal)
        {
          AddWildcard(string.Empty, null); //##local is not allowed
        }
        break;
        case NamespaceList.ListType.Set:
        foreach (string wildcard in list.Enumerate)
        {
          AddWildcard(wildcard, particle);
        }
        break;
      }
    }

    private void AddWildcard(string wildcard, object particle)
    {
      if (wildcards == null)
      {
        wildcards = new Hashtable();
      }
      object lookup = wildcards[wildcard];
      if (lookup == null)
      {
        wildcards.Add(wildcard, last);
        particles.Add(particle);
        last++;
      }
      else if (particle != null)
      {
        particles[(int)lookup] = particle;
      }
    }

    public ICollection GetNamespaceListSymbols(NamespaceList list)
    {
      ArrayList match = new ArrayList();
      foreach (XmlQualifiedName name in names.Keys)
      {
        if (name != XmlQualifiedName.Empty && list.Allows(name))
        {
          match.Add(names[name]);
        }
      }
      if (wildcards != null)
      {
        foreach (string wildcard in wildcards.Keys)
        {
          if (list.Allows(wildcard))
          {
            match.Add(wildcards[wildcard]);
          }
        }
      }
      if (list.Type == NamespaceList.ListType.Any || list.Type == NamespaceList.ListType.Other)
      {
        match.Add(last); // add wildcard
      }
      return match;
    }

    /// <summary>
    /// Find the symbol for the given name. If neither names nor wilcards match it last (*.*) symbol will be returned
    /// </summary>
    public int this[XmlQualifiedName name]
    {
      get
      {
        object lookup = names[name];
        if (lookup != null)
        {
          return (int)lookup;
        }
        if (wildcards != null)
        {
          lookup = wildcards[name.Namespace];
          if (lookup != null)
          {
            return (int)lookup;
          }
        }
        return last; // true wildcard
      }
    }

    /// <summary>
    /// Check if a name exists in the symbol dictionary
    /// </summary>
    public bool Exists(XmlQualifiedName name)
    {

      object lookup = names[name];
      if (lookup != null)
      {
        return true;
      }
      return false;
    }

    /// <summary>
    /// Return content processing mode for the symbol
    /// </summary>
    public object GetParticle(int symbol)
    {
      return symbol == last ? particleLast : particles[symbol];
    }

    /// <summary>
    /// Output symbol's name
    /// </summary>
    public string NameOf(int symbol)
    {
      foreach (DictionaryEntry de in names)
      {
        if ((int)de.Value == symbol)
        {
          return ((XmlQualifiedName)de.Key).ToString();
        }
      }
      if (wildcards != null)
      {
        foreach (DictionaryEntry de in wildcards)
        {
          if ((int)de.Value == symbol)
          {
            return (string)de.Key + ":*";
          }
        }
      }
      return "##other:*";
    }
  }
}
