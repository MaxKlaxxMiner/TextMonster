namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaAttributeGroupRef.uex' path='docs/doc[@for="XmlSchemaAttributeGroupRef"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaAttributeGroupRef : XmlSchemaAnnotated
  {
    XmlQualifiedName refName = XmlQualifiedName.Empty;

    /// <include file='doc\XmlSchemaAttributeGroupRef.uex' path='docs/doc[@for="XmlSchemaAttributeGroupRef.RefName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("ref")]
    public XmlQualifiedName RefName
    {
      get { return refName; }
      set { refName = (value == null ? XmlQualifiedName.Empty : value); }
    }
  }
}
