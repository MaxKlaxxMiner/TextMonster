using System;
using System.Collections;
using System.Text;

namespace TextMonster.Xml.Xml_Reader
{
  internal class NamespaceList
  {
    public enum ListType
    {
      Any,
      Other,
      Set
    };

    private ListType type = ListType.Any;
    private Hashtable set = null;
    private string targetNamespace;

    public NamespaceList()
    {
    }

    public NamespaceList(string namespaces, string targetNamespace)
    {
      this.targetNamespace = targetNamespace;
      namespaces = namespaces.Trim();
      if (namespaces == "##any" || namespaces.Length == 0)
      {
        type = ListType.Any;
      }
      else if (namespaces == "##other")
      {
        type = ListType.Other;
      }
      else
      {
        type = ListType.Set;
        set = new Hashtable();
        string[] splitString = XmlConvert.SplitString(namespaces);
        for (int i = 0; i < splitString.Length; ++i)
        {
          if (splitString[i] == "##local")
          {
            set[string.Empty] = string.Empty;
          }
          else if (splitString[i] == "##targetNamespace")
          {
            set[targetNamespace] = targetNamespace;
          }
          else
          {
            XmlConvert.ToUri(splitString[i]); // can throw
            set[splitString[i]] = splitString[i];
          }
        }
      }
    }

    public ListType Type
    {
      get { return type; }
    }

    public string Excluded
    {
      get { return targetNamespace; }
    }

    public ICollection Enumerate
    {
      get
      {
        switch (type)
        {
          case ListType.Set:
          return set.Keys;
          case ListType.Other:
          case ListType.Any:
          default:
          throw new InvalidOperationException();
        }
      }
    }

    public virtual bool Allows(string ns)
    {
      switch (type)
      {
        case ListType.Any:
        return true;
        case ListType.Other:
        return ns != targetNamespace && ns.Length != 0;
        case ListType.Set:
        return set[ns] != null;
      }
      return false;
    }

    public bool Allows(XmlQualifiedName qname)
    {
      return Allows(qname.Namespace);
    }

    public override string ToString()
    {
      switch (type)
      {
        case ListType.Any:
        return "##any";
        case ListType.Other:
        return "##other";
        case ListType.Set:
        StringBuilder sb = new StringBuilder();
        bool first = true;
        foreach (string s in set.Keys)
        {
          if (first)
          {
            first = false;
          }
          else
          {
            sb.Append(" ");
          }
          if (s == targetNamespace)
          {
            sb.Append("##targetNamespace");
          }
          else if (s.Length == 0)
          {
            sb.Append("##local");
          }
          else
          {
            sb.Append(s);
          }
        }
        return sb.ToString();
      }
      return string.Empty;
    }
  };
}
