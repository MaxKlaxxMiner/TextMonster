namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlTypeMapping.uex' path='docs/doc[@for="XmlTypeMapping"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlTypeMapping : XmlMapping
  {

    internal XmlTypeMapping(TypeScope scope, ElementAccessor accessor)
      : base(scope, accessor)
    {
    }

    internal TypeMapping Mapping
    {
      get { return Accessor.Mapping; }
    }

    /// <include file='doc\XmlTypeMapping.uex' path='docs/doc[@for="XmlTypeMapping.TypeFullName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string TypeFullName
    {
      get { return Mapping.TypeDesc.FullName; }
    }

    /// <include file='doc\XmlTypeMapping.uex' path='docs/doc[@for="XmlTypeMapping.XsdTypeName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string XsdTypeName
    {
      get { return Mapping.TypeName; }
    }

    /// <include file='doc\XmlTypeMapping.uex' path='docs/doc[@for="XmlTypeMapping.XsdTypeNamespace"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    public string XsdTypeNamespace
    {
      get { return Mapping.Namespace; }
    }
  }
}