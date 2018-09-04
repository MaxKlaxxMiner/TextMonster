namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaIdentityConstraint.uex' path='docs/doc[@for="XmlSchemaKeyref"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaKeyref : XmlSchemaIdentityConstraint
  {
    XmlQualifiedName refer = XmlQualifiedName.Empty;

    /// <include file='doc\XmlSchemaIdentityConstraint.uex' path='docs/doc[@for="XmlSchemaKeyref.Refer"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("refer")]
    public XmlQualifiedName Refer
    {
      get { return refer; }
      set { refer = (value == null ? XmlQualifiedName.Empty : value); }
    }
  }
}
