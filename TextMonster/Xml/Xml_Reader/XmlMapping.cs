using System;

namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlMapping.uex' path='docs/doc[@for="XmlMapping"]/*' />
  ///<internalonly/>
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public abstract class XmlMapping
  {
    TypeScope scope;
    bool generateSerializer = false;
    bool isSoap;
    ElementAccessor accessor;
    string key;
    bool shallow = false;
    XmlMappingAccess access;

    internal XmlMapping(TypeScope scope, ElementAccessor accessor)
      : this(scope, accessor, XmlMappingAccess.Read | XmlMappingAccess.Write)
    {
    }

    internal XmlMapping(TypeScope scope, ElementAccessor accessor, XmlMappingAccess access)
    {
      this.scope = scope;
      this.accessor = accessor;
      this.access = access;
      this.shallow = scope == null;
    }

    internal ElementAccessor Accessor
    {
      get { return accessor; }
    }

    /// <include file='doc\XmlMapping.uex' path='docs/doc[@for="XmlMapping.ElementName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string ElementName
    {
      get { return Xml_Reader.Accessor.UnescapeName(Accessor.Name); }
    }

    /// <include file='doc\XmlMapping.uex' path='docs/doc[@for="XmlMapping.Namespace"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string Namespace
    {
      get { return accessor.Namespace; }
    }

    internal bool GenerateSerializer
    {
      set { generateSerializer = value; }
    }

    internal bool IsSoap
    {
      get { return isSoap; }
    }

    /// <include file='doc\XmlMapping.uex' path='docs/doc[@for="XmlMapping.SetKeyInternal"]/*' />
    ///<internalonly/>
    internal void SetKeyInternal(string key)
    {
      this.key = key;
    }

    internal static string GenerateKey(Type type, XmlRootAttribute root, string ns)
    {
      if (root == null)
      {
        root = (XmlRootAttribute)XmlAttributes.GetAttr(type, typeof(XmlRootAttribute));
      }
      return type.FullName + ":" + (root == null ? String.Empty : root.Key) + ":" + (ns == null ? String.Empty : ns);
    }

    internal string Key { get { return key; } }
    internal void CheckShallow()
    {
      if (shallow)
      {
        throw new InvalidOperationException(Res.GetString(Res.XmlMelformMapping));
      }
    }
    internal static bool IsShallow(XmlMapping[] mappings)
    {
      for (int i = 0; i < mappings.Length; i++)
      {
        if (mappings[i] == null || mappings[i].shallow)
          return true;
      }
      return false;
    }
  }
}