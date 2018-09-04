namespace TextMonster.Xml.Xml_Reader
{
  /// <include file='doc\XmlSchemaSimpleContent.uex' path='docs/doc[@for="XmlSchemaSimpleContent"]/*' />
  /// <devdoc>
  ///    <para>[To be supplied.]</para>
  /// </devdoc>
  public class XmlSchemaSimpleContent : XmlSchemaContentModel
  {
    XmlSchemaContent content;

    /// <include file='doc\XmlSchemaSimpleContent.uex' path='docs/doc[@for="XmlSchemaSimpleContent.Content"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [XmlElement("restriction", typeof(XmlSchemaSimpleContentRestriction)),
     XmlElement("extension", typeof(XmlSchemaSimpleContentExtension))]
    public override XmlSchemaContent Content
    {
      get { return content; }
      set { content = value; }
    }
  }
}
