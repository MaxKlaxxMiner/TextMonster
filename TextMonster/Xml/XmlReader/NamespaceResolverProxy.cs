using System;
using System.Collections.Generic;

namespace TextMonster.Xml.XmlReader
{
  //
  // Private types
  //
  class NamespaceResolverProxy : IXmlNamespaceResolver
  {
    XmlWellFormedWriter wfWriter;

    internal NamespaceResolverProxy(XmlWellFormedWriter wfWriter)
    {
      this.wfWriter = wfWriter;
    }

    IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
    {
      throw new NotImplementedException();
    }
    string IXmlNamespaceResolver.LookupNamespace(string prefix)
    {
      return wfWriter.LookupNamespace(prefix);
    }

    string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
    {
      return wfWriter.LookupPrefix(namespaceName);
    }
  }
}
