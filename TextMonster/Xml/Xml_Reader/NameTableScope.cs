using System;
using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class NameTableScope : INameScope
  {
    Hashtable table = new Hashtable();

    internal void Add(XmlQualifiedName qname, object value)
    {
      Add(qname.Name, qname.Namespace, value);
    }

    internal void Add(string name, string ns, object value)
    {
      NameKey key = new NameKey(name, ns);
      table.Add(key, value);
    }

    internal object this[XmlQualifiedName qname]
    {
      get
      {
        return table[new NameKey(qname.Name, qname.Namespace)];
      }
      set
      {
        table[new NameKey(qname.Name, qname.Namespace)] = value;
      }
    }
    internal object this[string name, string ns]
    {
      get
      {
        return table[new NameKey(name, ns)];
      }
      set
      {
        table[new NameKey(name, ns)] = value;
      }
    }
    object INameScope.this[string name, string ns]
    {
      get
      {
        return table[new NameKey(name, ns)];
      }
      set
      {
        table[new NameKey(name, ns)] = value;
      }
    }

    internal ICollection Values
    {
      get { return table.Values; }
    }

    internal Array ToArray(Type type)
    {
      Array a = Array.CreateInstance(type, table.Count);
      table.Values.CopyTo(a, 0);
      return a;
    }
  }
}
