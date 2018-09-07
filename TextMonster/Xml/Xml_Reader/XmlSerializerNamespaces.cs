using System.Collections;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSerializerNamespaces.uex' path='docs/doc[@for="XmlSerializerNamespaces"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSerializerNamespaces
  {
    Hashtable namespaces = null;
    
    /// <include file='doc\XmlSerializerNamespaces.uex' path='docs/doc[@for="XmlSerializerNamespaces.Add"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public void Add(string prefix, string ns)
    {
      // parameter value check
      if (prefix != null && prefix.Length > 0)
        XmlConvert.VerifyNCName(prefix);

      if (ns != null && ns.Length > 0)
        XmlConvert.ToUri(ns);
      AddInternal(prefix, ns);
    }

    internal void AddInternal(string prefix, string ns)
    {
      Namespaces[prefix] = ns;
    }

    internal Hashtable Namespaces
    {
      get
      {
        if (namespaces == null)
          namespaces = new Hashtable();
        return namespaces;
      }
      set { namespaces = value; }
    }
  }
}
