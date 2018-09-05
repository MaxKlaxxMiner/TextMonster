using System.Collections;
using System.Collections.Specialized;

namespace TextMonster.Xml.Xml_Reader
{
  internal class SchemaObjectCache
  {
    Hashtable graph;
    Hashtable hash;
    Hashtable objectCache;
    StringCollection warnings;
    // 
    internal Hashtable looks = new Hashtable();
    Hashtable Graph
    {
      get
      {
        if (graph == null)
          graph = new Hashtable();
        return graph;
      }
    }

    Hashtable Hash
    {
      get
      {
        if (hash == null)
          hash = new Hashtable();
        return hash;
      }
    }

    Hashtable ObjectCache
    {
      get
      {
        if (objectCache == null)
          objectCache = new Hashtable();
        return objectCache;
      }
    }

    internal StringCollection Warnings
    {
      get
      {
        if (warnings == null)
          warnings = new StringCollection();
        return warnings;
      }
    }

    internal XmlSchemaObject AddItem(XmlSchemaObject item, XmlQualifiedName qname, XmlSchemas schemas)
    {
      if (item == null)
        return null;
      if (qname == null || qname.IsEmpty)
        return null;

      string key = item.GetType().Name + ":" + qname.ToString();
      ArrayList list = (ArrayList)ObjectCache[key];
      if (list == null)
      {
        list = new ArrayList();
        ObjectCache[key] = list;
      }

      for (int i = 0; i < list.Count; i++)
      {
        XmlSchemaObject cachedItem = (XmlSchemaObject)list[i];
        if (cachedItem == item)
          return cachedItem;

        if (Match(cachedItem, item, true))
        {
          return cachedItem;
        }
        else
        {
          Warnings.Add(Res.GetString(Res.XmlMismatchSchemaObjects, item.GetType().Name, qname.Name, qname.Namespace));
          Warnings.Add("DEBUG:Cached item key:\r\n" + (string)looks[cachedItem] + "\r\nnew item key:\r\n" + (string)looks[item]);
        }
      }
      // no match found we need to insert the new type in the cache
      list.Add(item);
      return item;
    }

    internal bool Match(XmlSchemaObject o1, XmlSchemaObject o2, bool shareTypes)
    {
      if (o1 == o2)
        return true;
      if (o1.GetType() != o2.GetType())
        return false;
      if (Hash[o1] == null)
        Hash[o1] = GetHash(o1);
      int hash1 = (int)Hash[o1];
      int hash2 = GetHash(o2);
      if (hash1 != hash2)
        return false;

      if (shareTypes)
        return CompositeHash(o1, hash1) == CompositeHash(o2, hash2);
      return true;
    }

    private ArrayList GetDependencies(XmlSchemaObject o, ArrayList deps, Hashtable refs)
    {
      if (refs[o] == null)
      {
        refs[o] = o;
        deps.Add(o);
        ArrayList list = Graph[o] as ArrayList;
        if (list != null)
        {
          for (int i = 0; i < list.Count; i++)
          {
            GetDependencies((XmlSchemaObject)list[i], deps, refs);
          }
        }
      }
      return deps;
    }

    private int CompositeHash(XmlSchemaObject o, int hash)
    {
      ArrayList list = GetDependencies(o, new ArrayList(), new Hashtable());
      double tmp = 0;
      for (int i = 0; i < list.Count; i++)
      {
        object cachedHash = Hash[list[i]];
        if (cachedHash is int)
        {
          tmp += (int)cachedHash / list.Count;
        }
      }
      return (int)tmp;
    }

    internal void GenerateSchemaGraph(XmlSchemas schemas)
    {
      SchemaGraph graph = new SchemaGraph(Graph, schemas);
      ArrayList items = graph.GetItems();

      for (int i = 0; i < items.Count; i++)
      {
        GetHash((XmlSchemaObject)items[i]);
      }
    }

    private int GetHash(XmlSchemaObject o)
    {
      object hash = Hash[o];
      if (hash != null)
      {
        if (hash is XmlSchemaObject)
        {
        }
        else
        {
          return (int)hash;
        }
      }
      // new object, generate the hash
      string hashString = ToString(o, new SchemaObjectWriter());
      looks[o] = hashString;
      int code = hashString.GetHashCode();
      Hash[o] = code;
      return code;
    }

    string ToString(XmlSchemaObject o, SchemaObjectWriter writer)
    {
      return writer.WriteXmlSchemaObject(o);
    }
  }
}