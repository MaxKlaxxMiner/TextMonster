namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlMemberMapping.uex' path='docs/doc[@for="XmlMemberMapping"]/*' />
  /// <internalonly/>
  public class XmlMemberMapping
  {
    MemberMapping mapping;

    internal XmlMemberMapping(MemberMapping mapping)
    {
      this.mapping = mapping;
    }

    internal MemberMapping Mapping
    {
      get { return mapping; }
    }

    internal Accessor Accessor
    {
      get { return mapping.Accessor; }
    }

    /// <include file='doc\XmlMemberMapping.uex' path='docs/doc[@for="XmlMemberMapping.Any"]/*' />
    public bool Any
    {
      get { return Accessor.Any; }
    }

    /// <include file='doc\XmlMemberMapping.uex' path='docs/doc[@for="XmlMemberMapping.ElementName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string ElementName
    {
      get { return Accessor.UnescapeName(Accessor.Name); }
    }

    /// <include file='doc\XmlMemberMapping.uex' path='docs/doc[@for="XmlMemberMapping.Namespace"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string Namespace
    {
      get { return Accessor.Namespace; }
    }
  }
}