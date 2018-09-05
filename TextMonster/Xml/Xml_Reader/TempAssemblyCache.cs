using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  internal class TempAssemblyCache
  {
    Hashtable cache = new Hashtable();

    internal TempAssembly this[string ns, object o]
    {
      get { return (TempAssembly)cache[new TempAssemblyCacheKey(ns, o)]; }
    }

    internal void Add(string ns, object o, TempAssembly assembly)
    {
      TempAssemblyCacheKey key = new TempAssemblyCacheKey(ns, o);
      lock (this)
      {
        if (cache[key] == assembly) return;
        Hashtable clone = new Hashtable();
        foreach (object k in cache.Keys)
        {
          clone.Add(k, cache[k]);
        }
        cache = clone;
        cache[key] = assembly;
      }
    }
  }
}