namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaGroupRef.uex' path='docs/doc[@for="XmlSchemaGroupRef"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaGroupRef : XmlSchemaParticle
  {
    XmlQualifiedName refName = XmlQualifiedName.Empty;
    XmlSchemaGroupBase particle;
    XmlSchemaGroup refined;

    /// <include file='doc\XmlSchemaGroupRef.uex' path='docs/doc[@for="XmlSchemaGroupRef.RefName"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlAttribute("ref")]
    public XmlQualifiedName RefName
    {
      get { return refName; }
      set { refName = (value == null ? XmlQualifiedName.Empty : value); }
    }

    [XmlIgnore]
    internal XmlSchemaGroup Redefined
    {
      set { refined = value; }
    }
  }
}
