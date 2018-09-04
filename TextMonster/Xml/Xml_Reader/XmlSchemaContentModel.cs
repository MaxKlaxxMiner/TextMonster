namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaContentModel.uex' path='docs/doc[@for="XmlSchemaContentModel"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public abstract class XmlSchemaContentModel : XmlSchemaAnnotated
  {
    /// <include file='doc\XmlSchemaContentModel.uex' path='docs/doc[@for="XmlSchemaContentModel.Content"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlIgnore]
    public abstract XmlSchemaContent Content { get; set; }
  }
}
